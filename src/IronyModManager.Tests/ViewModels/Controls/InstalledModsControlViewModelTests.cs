// ***********************************************************************
// Assembly         : IronyModManager.Tests
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
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
    }
}
