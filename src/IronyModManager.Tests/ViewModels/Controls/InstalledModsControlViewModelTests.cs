// ***********************************************************************
// Assembly         : IronyModManager.Tests
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using AwesomeAssertions;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Tests.Common;
using IronyModManager.ViewModels.Controls;
using Moq;
using Xunit;

namespace IronyModManager.Tests.ViewModels.Controls
{
    public class InstalledModsControlViewModelTests
    {
        [Fact]
        public async Task Achievement_evaluation_should_ignore_virtual_mods()
        {
            DISetup.SetupContainer();
            var modService = new Mock<IModService>();
            var viewModel = CreateViewModel(modService);

            await viewModel.EvalModAchievementForTestAsync([new Mod { IsVirtual = true }]);

            modService.Verify(service => service.PopulateModFilesAsync(It.IsAny<IEnumerable<IMod>>()), Times.Never);
            modService.Verify(service => service.EvalAchievementCompatibility(It.IsAny<IEnumerable<IMod>>()), Times.Never);
        }

        [Fact]
        public async Task Achievement_evaluation_should_pass_only_real_mods_to_service()
        {
            DISetup.SetupContainer();
            var realMod = new Mod { Name = "Real", IsValid = true };
            var virtualMod = new Mod { Name = "Virtual", IsVirtual = true };
            var modService = new Mock<IModService>();
            modService.Setup(service => service.PopulateModFilesAsync(It.IsAny<IEnumerable<IMod>>())).ReturnsAsync(true);

            var viewModel = CreateViewModel(modService);

            await viewModel.EvalModAchievementForTestAsync([realMod, virtualMod]);

            modService.Verify(service => service.PopulateModFilesAsync(
                It.Is<IEnumerable<IMod>>(mods => mods.SequenceEqual(new[] { realMod }))), Times.Once);
            modService.Verify(service => service.EvalAchievementCompatibility(
                It.Is<IEnumerable<IMod>>(mods => mods.SequenceEqual(new[] { realMod }))), Times.Once);
        }

        [Fact]
        public async Task Controlled_authoritative_refresh_should_publish_refreshed_mods()
        {
            DISetup.SetupContainer();
            var game = new Game { Type = "publish-revalidation" };
            var revalidationLock = new GameStateLockInfo
                { GameType = game.Type, Reason = GameStateLockReason.ConfigurationChanged };
            var discovered = new Mod { Name = "New custom mod", DescriptorFile = "mod/new-custom.mod", IsValid = true };
            var modService = new Mock<IModService>();
            modService.Setup(service => service.RevalidateInstalledModsAsync(game, revalidationLock)).ReturnsAsync(
                new InstalledModsResult { IsAuthoritative = true, Mods = [discovered] });
            modService.Setup(service => service.FilterMods(It.IsAny<IEnumerable<IMod>>(), It.IsAny<string>()))
                .Returns((IEnumerable<IMod> mods, string _) => mods);

            var filter = new SearchModsControlViewModel();
            var presentation = new Mock<InstalledModsPresentationCoordinator>(MockBehavior.Loose,
                new object[] { null, null, null, null, null });
            presentation.SetupGet(value => value.FilterMods).Returns(filter);
            var events = new Mock<InstalledModsEventCoordinator>(MockBehavior.Loose, new object[] { null });
            var factory = new Mock<IInstalledModsCoordinatorFactory>();
            factory.Setup(value => value.Create()).Returns(new InstalledModsCoordinators(presentation.Object, events.Object));
            var interaction = new Mock<ModControlInteraction>(MockBehavior.Loose,
                new object[] { null, null, null, null });
            interaction.Setup(value => value.BeginOperation()).Returns(1);
            var viewModel = new InstalledModsControlViewModel(Mock.Of<IGameService>(), modService.Object,
                Mock.Of<IGameStateSafetyService>(), interaction.Object, factory.Object);

            var authoritative = await viewModel.RevalidateModsAsync(game, revalidationLock, () => true, true);

            authoritative.Should().BeTrue();
            viewModel.Mods.Should().ContainSingle().Which.Should().BeSameAs(discovered);
            viewModel.LastRefreshAuthoritative.Should().BeTrue();
            modService.Verify(service => service.RevalidateInstalledModsAsync(game, revalidationLock), Times.Once);
            modService.Verify(service => service.RefreshInstalledModsAsync(It.IsAny<IGame>()), Times.Never);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Switching_to_locked_game_should_present_only_its_own_cached_graph(bool hasTargetCache)
        {
            DISetup.SetupContainer();
            var a = new Game { Type = "A" };
            var b = new Game { Type = "B" };
            var modA = new Mod { Game = a.Type, Name = "A", DescriptorFile = "mod/shared.mod", IsValid = true };
            var modB = new Mod { Game = b.Type, Name = "B", DescriptorFile = "mod/shared.mod", IsValid = true };
            var game = new Mock<IGameService>();
            IGame selected = a;
            game.Setup(p => p.GetSelected()).Returns(() => selected);
            var safety = new Mock<IGameStateSafetyService>();
            safety.Setup(p => p.IsLocked(b)).Returns(true);
            var service = new Mock<IModService>();
            service.Setup(p => p.RefreshInstalledModsAsync(a)).ReturnsAsync(new InstalledModsResult { IsAuthoritative = true, Mods = [modA] });
            service.Setup(p => p.RefreshInstalledModsAsync(b)).ReturnsAsync(new InstalledModsResult { IsAuthoritative = false, Mods = hasTargetCache ? [modB] : [] });
            service.Setup(p => p.FilterMods(It.IsAny<IEnumerable<IMod>>(), It.IsAny<string>())).Returns((IEnumerable<IMod> mods, string _) => mods);
            var viewModel = CreateViewModel(service, game, safety);
            await viewModel.BindForTestAsync(a);
            viewModel.ContextMenuMod = modA;

            selected = b;
            var refresh = viewModel.BindForTestAsync(b);
            viewModel.Mods.Should().BeEmpty("the source projection must clear before discovery completes");
            await refresh;

            viewModel.ActiveGame.Should().BeSameAs(b);
            viewModel.LastRefreshAuthoritative.Should().BeFalse();
            viewModel.Mods.Should().OnlyContain(p => p.Game == b.Type);
            viewModel.FilteredMods.Should().OnlyContain(p => p.Game == b.Type);
            viewModel.AllMods.Should().NotContain(modA);
            viewModel.ContextMenuMod.Should().BeNull();
            viewModel.Mods.Should().HaveCount(hasTargetCache ? 1 : 0);
            if (hasTargetCache) viewModel.Mods.Single().Should().BeSameAs(modB);

            selected = a;
            await viewModel.BindForTestAsync(a);
            viewModel.Mods.Should().Equal(modA);
            safety.Verify(p => p.Lock(a, It.IsAny<GameStateLockReason>(), It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task Same_game_failed_refresh_should_keep_that_games_known_good_objects()
        {
            DISetup.SetupContainer();
            var game = new Game { Type = "B" };
            var mod = new Mod { Game = game.Type, Name = "B", IsValid = true };
            var service = new Mock<IModService>();
            service.SetupSequence(p => p.RefreshInstalledModsAsync(game))
                .ReturnsAsync(new InstalledModsResult { IsAuthoritative = true, Mods = [mod] })
                .ReturnsAsync(new InstalledModsResult { IsAuthoritative = false, Mods = [mod] });
            service.Setup(p => p.FilterMods(It.IsAny<IEnumerable<IMod>>(), It.IsAny<string>())).Returns((IEnumerable<IMod> mods, string _) => mods);
            var viewModel = CreateViewModel(service);

            await viewModel.BindForTestAsync(game);
            await viewModel.BindForTestAsync(game);

            viewModel.Mods.Single().Should().BeSameAs(mod);
            viewModel.FilteredMods.Single().Should().BeSameAs(mod);
            viewModel.LastRefreshAuthoritative.Should().BeFalse();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Rapid_switches_should_reject_old_completions_even_after_returning_to_the_same_game(bool lateFailure)
        {
            DISetup.SetupContainer();
            var a = new Game { Type = "A" };
            var b = new Game { Type = "B" };
            var stale = new Mod { Game = "A", Name = "stale", IsValid = true };
            var current = new Mod { Game = "A", Name = "current", IsValid = true };
            var firstA = new TaskCompletionSource<InstalledModsResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            var firstB = new TaskCompletionSource<InstalledModsResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            var startedA = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var startedB = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var calls = 0;
            var service = new Mock<IModService>();
            service.Setup(p => p.RefreshInstalledModsAsync(a)).Returns(() =>
            {
                if (System.Threading.Interlocked.Increment(ref calls) == 1) { startedA.TrySetResult(); return firstA.Task; }
                return Task.FromResult(new InstalledModsResult { IsAuthoritative = true, Mods = [current] });
            });
            service.Setup(p => p.RefreshInstalledModsAsync(b)).Returns(() => { startedB.TrySetResult(); return firstB.Task; });
            service.Setup(p => p.FilterMods(It.IsAny<IEnumerable<IMod>>(), It.IsAny<string>())).Returns((IEnumerable<IMod> mods, string _) => mods);
            var viewModel = CreateViewModel(service);
            var pendingA = viewModel.BindForTestAsync(a);
            await startedA.Task;
            var pendingB = viewModel.BindForTestAsync(b);
            await startedB.Task;
            await viewModel.BindForTestAsync(a);
            firstB.SetResult(new InstalledModsResult { IsAuthoritative = !lateFailure, Mods = [new Mod { Game = "B", IsValid = true }] });
            firstA.SetResult(new InstalledModsResult { IsAuthoritative = true, Mods = [stale] });
            await Task.WhenAll(pendingA, pendingB);

            viewModel.ActiveGame.Should().BeSameAs(a);
            viewModel.Mods.Should().Equal(current);
            viewModel.FilteredMods.Should().Equal(current);
            viewModel.LastRefreshAuthoritative.Should().BeTrue();
        }

        [Fact]
        public async Task Game_switch_should_release_superseded_refresh_ownership()
        {
            DISetup.SetupContainer();
            var a = new Game { Type = "A" };
            var b = new Game { Type = "B" };
            var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var pending = new TaskCompletionSource<InstalledModsResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            var service = new Mock<IModService>();
            service.Setup(p => p.RefreshInstalledModsAsync(a)).Returns(() => { started.SetResult(); return pending.Task; });
            service.Setup(p => p.RefreshInstalledModsAsync(b)).ReturnsAsync(new InstalledModsResult { IsAuthoritative = true, Mods = [] });
            service.Setup(p => p.FilterMods(It.IsAny<IEnumerable<IMod>>(), It.IsAny<string>())).Returns((IEnumerable<IMod> mods, string _) => mods);
            var gameService = new Mock<IGameService>();
            gameService.Setup(p => p.GetSelected()).Returns(a);
            var viewModel = CreateViewModel(service, gameService);
            await viewModel.BindForTestAsync(a, discover: false);
            var refresh = viewModel.RefreshModsAsync(true);
            await started.Task;

            await viewModel.BindForTestAsync(b);
            pending.SetResult(new InstalledModsResult { IsAuthoritative = true, Mods = [] });
            await refresh;

            viewModel.ActiveGame.Should().BeSameAs(b);
            viewModel.RefreshingMods.Should().BeFalse();
            viewModel.LastRefreshAuthoritative.Should().BeTrue();
        }

        private static TestInstalledModsControlViewModel CreateViewModel(Mock<IModService> modService,
            Mock<IGameService> game = null, Mock<IGameStateSafetyService> safety = null)
        {
            var filter = new SearchModsControlViewModel();
            var presentation = new Mock<InstalledModsPresentationCoordinator>(MockBehavior.Loose,
                new object[] { null, null, null, null, null });
            presentation.SetupGet(value => value.FilterMods).Returns(filter);
            var events = new Mock<InstalledModsEventCoordinator>(MockBehavior.Loose, new object[] { null });
            var factory = new Mock<IInstalledModsCoordinatorFactory>();
            factory.Setup(value => value.Create()).Returns(new InstalledModsCoordinators(presentation.Object, events.Object));
            var interaction = new Mock<ModControlInteraction>(MockBehavior.Loose,
                new object[] { null, null, null, null });

            return new TestInstalledModsControlViewModel(game?.Object ?? Mock.Of<IGameService>(), modService.Object,
                safety?.Object ?? Mock.Of<IGameStateSafetyService>(), interaction.Object, factory.Object);
        }

        private sealed class TestInstalledModsControlViewModel(IGameService gameService, IModService modService,
            IGameStateSafetyService gameStateSafetyService, ModControlInteraction interaction,
            IInstalledModsCoordinatorFactory coordinatorFactory)
            : InstalledModsControlViewModel(gameService, modService, gameStateSafetyService, interaction, coordinatorFactory)
        {
            public Task<bool> BindForTestAsync(IGame game, bool discover = true)
            {
                if (!discover) { ActiveGame = game; return Task.FromResult(true); }
                return BindAsync(game, skipOverlay: true);
            }
            public Task EvalModAchievementForTestAsync(IEnumerable<IMod> mods) => EvalModAchievementAsync(mods, true);
        }
    }
}
