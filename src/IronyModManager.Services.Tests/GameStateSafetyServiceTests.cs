// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AwesomeAssertions;
using IronyModManager.IO.Common.FileSystem;
using IronyModManager.Models;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using Moq;
using Xunit;

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Tests process-lifetime, per-game filesystem safety state.
    /// </summary>
    public class GameStateSafetyServiceTests
    {
        [Fact]
        public void Lock_should_be_per_game_sticky_and_retain_first_reason()
        {
            var probe = new Mock<IFileSystemStateProbe>();
            var service = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var gameA = new Game { Type = "game-a" };
            var gameB = new Game { Type = "game-b" };

            service.Lock(gameA, GameStateLockReason.ExpectedSourceMissing, "missing path").Should().BeTrue();
            service.IsLocked(gameA).Should().BeTrue();
            service.IsLocked(gameB).Should().BeFalse();

            service.Lock(gameA, GameStateLockReason.WriteAccessFailure, "later success cannot unlock or replace").Should().BeFalse();
            service.GetLock(gameA).Reason.Should().Be(GameStateLockReason.ExpectedSourceMissing);
            service.GetLock(gameA).Context.Should().Be("missing path");
        }

        [Fact]
        public async Task Mutation_guard_should_reject_locked_game_without_invoking_mutation()
        {
            var probe = new Mock<IFileSystemStateProbe>();
            var service = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var game = new Game { Type = "locked" };
            var invoked = false;
            service.Lock(game, GameStateLockReason.DiscoveryUnavailable, "provider");

            var result = await service.ExecuteMutationAsync(game, () =>
            {
                invoked = true;
                return Task.FromResult(true);
            }, false, "write");

            result.Should().BeFalse();
            invoked.Should().BeFalse();
        }

        [Fact]
        public async Task Mutation_guard_should_lock_on_classified_access_failure()
        {
            var probe = new Mock<IFileSystemStateProbe>();
            probe.Setup(p => p.IsFileSystemAccessFailure(It.IsAny<Exception>())).Returns(true);
            var service = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var game = new Game { Type = "write-failure" };

            var result = await service.ExecuteMutationAsync<bool>(game, () => throw new UnauthorizedAccessException(), false, "custom directory");

            result.Should().BeFalse();
            service.GetLock(game).Reason.Should().Be(GameStateLockReason.WriteAccessFailure);
            service.GetLock(game).ExceptionType.Should().Be(typeof(UnauthorizedAccessException).FullName);
        }

        [Fact]
        public async Task Read_guard_should_lock_as_discovery_failure_without_returning_empty_as_success()
        {
            var probe = new Mock<IFileSystemStateProbe>();
            probe.Setup(p => p.IsFileSystemAccessFailure(It.IsAny<Exception>())).Returns(true);
            var service = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var game = new Game { Type = "read-failure" };

            var result = await service.ExecuteReadAsync<IReadOnlyCollection<string>>(game,
                () => throw new IOException(), null, "cloud content");

            result.Should().BeNull();
            service.GetLock(game).Reason.Should().Be(GameStateLockReason.DiscoveryUnavailable);
        }

        [Fact]
        public void Phase_guard_should_lock_with_the_explicit_reason_only_for_classified_failures()
        {
            var probe = new Mock<IFileSystemStateProbe>();
            probe.Setup(p => p.IsFileSystemAccessFailure(It.IsAny<Exception>()))
                .Returns((Exception exception) => exception is IOException);
            var service = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var game = new Game { Type = "phase" };

            service.LockIfFileSystemAccessFailure(game, GameStateLockReason.DiscoveryUnavailable,
                "read phase", new InvalidOperationException()).Should().BeFalse();
            service.IsLocked(game).Should().BeFalse();
            service.LockIfFileSystemAccessFailure(game, GameStateLockReason.WriteAccessFailure,
                "write phase", new IOException()).Should().BeTrue();
            service.GetLock(game).Reason.Should().Be(GameStateLockReason.WriteAccessFailure);
            service.GetLock(game).Context.Should().Be("write phase");
        }

        [Fact]
        public void Controlled_revalidation_should_unlock_only_the_lock_that_was_validated()
        {
            var service = new GameStateSafetyService(Mock.Of<IFileSystemStateProbe>(), Mock.Of<ILogger>());
            var game = new Game { Type = "game" };
            var revalidationLock = service.BeginRevalidation(game, "path changed");

            service.IsLocked(game).Should().BeTrue();
            service.CompleteRevalidation(game, new GameStateLockInfo { GameType = game.Type }).Should().BeFalse();
            service.IsLocked(game).Should().BeTrue();
            service.CompleteRevalidation(game, revalidationLock).Should().BeTrue();
            service.IsLocked(game).Should().BeFalse();
        }

        [Fact]
        public void Failed_revalidation_should_replace_provisional_state_and_remain_locked()
        {
            var service = new GameStateSafetyService(Mock.Of<IFileSystemStateProbe>(), Mock.Of<ILogger>());
            var game = new Game { Type = "game" };
            var notifications = 0;
            service.GameLocked += _ => notifications++;
            var revalidationLock = service.BeginRevalidation(game, "path changed");

            service.Lock(game, GameStateLockReason.ExpectedSourceMissing, "new path").Should().BeTrue();

            service.CompleteRevalidation(game, revalidationLock).Should().BeFalse();
            service.GetLock(game).Reason.Should().Be(GameStateLockReason.ExpectedSourceMissing);
            service.GetLock(game).Context.Should().Be("new path");
            notifications.Should().Be(1);
        }

        [Fact]
        public void New_revalidation_should_supersede_older_ownership()
        {
            var service = new GameStateSafetyService(Mock.Of<IFileSystemStateProbe>(), Mock.Of<ILogger>());
            var game = new Game { Type = "game" };
            var first = service.BeginRevalidation(game, "first path");
            var second = service.BeginRevalidation(game, "second path");

            service.IsCurrentRevalidation(game, first).Should().BeFalse();
            service.IsCurrentRevalidation(game, second).Should().BeTrue();
            service.CompleteRevalidation(game, first).Should().BeFalse();
            service.CompleteRevalidation(game, second).Should().BeTrue();
        }

        [Fact]
        public void Stale_revalidation_failure_should_not_replace_current_reason()
        {
            var service = new GameStateSafetyService(Mock.Of<IFileSystemStateProbe>(), Mock.Of<ILogger>());
            var game = new Game { Type = "game" };
            var first = service.BeginRevalidation(game, "first path");
            var second = service.BeginRevalidation(game, "second path");

            service.LockRevalidationFailure(game, first, GameStateLockReason.DiscoveryUnavailable, "old path").Should().BeFalse();
            service.GetLock(game).Should().BeSameAs(second);
            service.LockRevalidationFailure(game, second, GameStateLockReason.ExpectedSourceMissing, "new path").Should().BeTrue();
            service.GetLock(game).Reason.Should().Be(GameStateLockReason.ExpectedSourceMissing);
            service.GetLock(game).Context.Should().Be("new path");
        }
    }
}
