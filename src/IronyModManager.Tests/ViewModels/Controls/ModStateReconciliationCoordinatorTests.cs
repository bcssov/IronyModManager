// ***********************************************************************
// Assembly         : IronyModManager.Tests
// ***********************************************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using AwesomeAssertions;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.ViewModels.Controls;
using Moq;
using Xunit;

namespace IronyModManager.Tests.ViewModels.Controls
{
    /// <summary>
    /// Tests the installed-mod to collection reconciliation ordering boundary.
    /// </summary>
    public class ModStateReconciliationCoordinatorTests
    {
        [Fact]
        public void Refresh_should_reconcile_once_at_authoritative_completion()
        {
            var safety = GetSafety();
            var coordinator = new ModStateReconciliationCoordinator(safety);
            var game = new Game { Type = "game" };
            var reconciliations = 0;

            coordinator.ReconcileModsPublication(game, true, true, () => reconciliations++).Should().BeFalse();
            coordinator.ReconcileRefreshPublication(game, false, true, () => reconciliations++).Should().BeTrue();

            reconciliations.Should().Be(1);
        }

        [Fact]
        public void Failed_or_locked_publications_should_not_reconcile()
        {
            var safety = GetSafety();
            var coordinator = new ModStateReconciliationCoordinator(safety);
            var game = new Game { Type = "game" };
            var reconciliations = 0;

            coordinator.ReconcileModsPublication(game, false, false, () => reconciliations++).Should().BeFalse();
            coordinator.ReconcileRefreshPublication(game, false, false, () => reconciliations++).Should().BeFalse();
            Mock.Get(safety).Setup(p => p.IsLocked(game)).Returns(true);
            coordinator.ReconcileModsPublication(game, false, true, () => reconciliations++).Should().BeFalse();
            coordinator.ReconcileRefreshPublication(game, false, true, () => reconciliations++).Should().BeFalse();

            reconciliations.Should().Be(0);
        }

        [Fact]
        public void Game_lock_subscription_should_follow_the_callers_disposable_lifetime()
        {
            var safety = GetSafety();
            var coordinator = new ModStateReconciliationCoordinator(safety);
            var notifications = 0;
            var subscription = coordinator.SubscribeToGameLocks(_ => notifications++);

            Mock.Get(safety).Raise(p => p.GameLocked += null, new GameStateLockInfo { GameType = "game" });
            subscription.Dispose();
            Mock.Get(safety).Raise(p => p.GameLocked += null, new GameStateLockInfo { GameType = "game" });

            notifications.Should().Be(1);
        }

        [Fact]
        public async Task Successful_configuration_revalidation_should_reset_then_unlock_then_reconcile()
        {
            var locked = false;
            var safety = GetSafety();
            var revalidationLock = new GameStateLockInfo { GameType = "game", Reason = GameStateLockReason.ConfigurationChanged };
            Mock.Get(safety).Setup(p => p.BeginRevalidation(It.IsAny<IGame>(), It.IsAny<string>())).Returns(() =>
            {
                locked = true;
                return revalidationLock;
            });
            Mock.Get(safety).Setup(p => p.IsLocked(It.IsAny<IGame>())).Returns(() => locked);
            Mock.Get(safety).Setup(p => p.IsCurrentRevalidation(It.IsAny<IGame>(), revalidationLock)).Returns(() => locked);
            Mock.Get(safety).Setup(p => p.CompleteRevalidation(It.IsAny<IGame>(), revalidationLock, It.IsAny<System.Action>())).Returns(
                (IGame _, GameStateLockInfo _, System.Action beforeUnlock) =>
            {
                beforeUnlock?.Invoke();
                locked = false;
                return true;
            });
            var coordinator = new ModStateReconciliationCoordinator(safety);
            var game = new Game { Type = "game" };
            var order = new List<string>();

            var result = await coordinator.RevalidateConfigurationAsync(game,
                (_, _) =>
                {
                    safety.IsLocked(game).Should().BeTrue();
                    order.Add("refresh");
                    return Task.FromResult(true);
                },
                () =>
                {
                    safety.IsLocked(game).Should().BeTrue();
                    order.Add("reset");
                },
                () =>
                {
                    safety.IsLocked(game).Should().BeFalse();
                    order.Add("reconcile");
                });

            result.Should().BeTrue();
            coordinator.IsRevalidating.Should().BeFalse();
            order.Should().Equal("refresh", "reset", "reconcile");
        }

        [Fact]
        public async Task Failed_configuration_revalidation_should_remain_locked_without_reset_or_reconciliation()
        {
            var locked = false;
            var safety = GetSafety();
            Mock.Get(safety).Setup(p => p.BeginRevalidation(It.IsAny<IGame>(), It.IsAny<string>())).Returns(() =>
            {
                locked = true;
                return new GameStateLockInfo { GameType = "game", Reason = GameStateLockReason.ConfigurationChanged };
            });
            Mock.Get(safety).Setup(p => p.IsLocked(It.IsAny<IGame>())).Returns(() => locked);
            Mock.Get(safety).Setup(p => p.IsCurrentRevalidation(It.IsAny<IGame>(), It.IsAny<GameStateLockInfo>())).Returns(true);
            var coordinator = new ModStateReconciliationCoordinator(safety);
            var game = new Game { Type = "game" };
            var reset = false;
            var reconciled = false;

            var result = await coordinator.RevalidateConfigurationAsync(game, (_, _) => Task.FromResult(false),
                () => reset = true, () => reconciled = true);

            result.Should().BeFalse();
            safety.IsLocked(game).Should().BeTrue();
            reset.Should().BeFalse();
            reconciled.Should().BeFalse();
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public async Task Out_of_order_revalidation_should_allow_only_the_newest_configuration_to_complete(
            bool firstSucceeds, bool secondSucceeds)
        {
            GameStateLockInfo current = null;
            var safety = GetSafety();
            Mock.Get(safety).Setup(p => p.BeginRevalidation(It.IsAny<IGame>(), It.IsAny<string>())).Returns(() =>
            {
                current = new GameStateLockInfo { GameType = "game", Reason = GameStateLockReason.ConfigurationChanged };
                return current;
            });
            Mock.Get(safety).Setup(p => p.IsCurrentRevalidation(It.IsAny<IGame>(), It.IsAny<GameStateLockInfo>()))
                .Returns((IGame _, GameStateLockInfo token) => ReferenceEquals(current, token) &&
                    current.Reason == GameStateLockReason.ConfigurationChanged);
            Mock.Get(safety).Setup(p => p.CompleteRevalidation(It.IsAny<IGame>(), It.IsAny<GameStateLockInfo>(), It.IsAny<System.Action>()))
                .Returns((IGame _, GameStateLockInfo token, System.Action beforeUnlock) =>
                {
                    if (!ReferenceEquals(current, token) || current.Reason != GameStateLockReason.ConfigurationChanged)
                    {
                        return false;
                    }

                    beforeUnlock?.Invoke();
                    current = null;
                    return true;
                });
            var coordinator = new ModStateReconciliationCoordinator(safety);
            var game = new Game { Type = "game" };
            var firstCompletion = new TaskCompletionSource<bool>();
            var secondCompletion = new TaskCompletionSource<bool>();
            var effects = new List<string>();

            async Task<bool> refresh(GameStateLockInfo token, TaskCompletionSource<bool> completion)
            {
                var succeeded = await completion.Task;
                if (!succeeded && ReferenceEquals(current, token))
                {
                    current = new GameStateLockInfo
                    {
                        GameType = "game",
                        Reason = GameStateLockReason.DiscoveryUnavailable
                    };
                }

                return succeeded;
            }

            var first = coordinator.RevalidateConfigurationAsync(game,
                (token, _) => refresh(token, firstCompletion),
                () => effects.Add("first-reset"), () => effects.Add("first-reconcile"));
            var second = coordinator.RevalidateConfigurationAsync(game,
                (token, _) => refresh(token, secondCompletion),
                () => effects.Add("second-reset"), () => effects.Add("second-reconcile"));

            secondCompletion.SetResult(secondSucceeds);
            (await second).Should().Be(secondSucceeds);
            firstCompletion.SetResult(firstSucceeds);
            (await first).Should().BeFalse();

            effects.Should().Equal(secondSucceeds
                ? ["second-reset", "second-reconcile"]
                : []);
            (current != null).Should().Be(!secondSucceeds);
        }

        private static IGameStateSafetyService GetSafety()
        {
            var safety = new Mock<IGameStateSafetyService>();
            safety.Setup(p => p.IsLocked(It.IsAny<IGame>())).Returns(false);
            return safety.Object;
        }
    }
}
