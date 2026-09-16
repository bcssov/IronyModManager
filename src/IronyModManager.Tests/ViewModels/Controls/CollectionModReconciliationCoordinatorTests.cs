// ***********************************************************************
// Assembly         : IronyModManager.Tests
// ***********************************************************************

using System.Collections.Generic;
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
    /// Tests authoritative collection resolution coordination.
    /// </summary>
    public class CollectionModReconciliationCoordinatorTests
    {
        [Fact]
        public void Locked_game_should_not_resolve_or_begin_import_warning_ownership()
        {
            var game = new Game { Type = "game" };
            var safety = new Mock<IGameStateSafetyService>();
            safety.Setup(p => p.IsLocked(game)).Returns(true);
            var modService = new Mock<IModService>();
            var coordinator = new CollectionModReconciliationCoordinator(modService.Object, safety.Object);
            var collection = new ModCollection { Name = "Collection", Game = game.Type };

            coordinator.TryResolve(game, [], collection, out var resolved).Should().BeFalse();
            coordinator.BeginImport(game, collection.Name);

            resolved.Should().BeEmpty();
            coordinator.ShouldShowOrdinaryWarning(game.Type, game.Type, collection.Name, ["missing.mod"]).Should().BeTrue();
            modService.Verify(p => p.ResolveCollectionMods(It.IsAny<IEnumerable<IMod>>(), It.IsAny<IModCollection>(), It.IsAny<IEnumerable<IMod>>()), Times.Never);
        }

        [Fact]
        public void Authoritative_resolution_should_supply_previous_objects_and_preserve_import_warning_policy()
        {
            var game = new Game { Type = "game" };
            var safety = new Mock<IGameStateSafetyService>();
            safety.Setup(p => p.IsLocked(game)).Returns(false);
            var modService = new Mock<IModService>();
            var coordinator = new CollectionModReconciliationCoordinator(modService.Object, safety.Object);
            var collection = new ModCollection { Name = "Collection", Game = game.Type };
            var installed = new List<IMod> { new Mod { Name = "Installed" } };
            var previous = new List<IMod> { new Mod { Name = "Previous" } };
            IReadOnlyCollection<IMod> expected = new List<IMod> { installed[0] };
            coordinator.RecordCollectionMods(collection.Name, previous);
            modService.Setup(p => p.ResolveCollectionMods(installed, collection, previous)).Returns(expected);

            coordinator.BeginImport(game, collection.Name);
            coordinator.TryResolve(game, installed, collection, out var resolved).Should().BeTrue();

            resolved.Should().BeSameAs(expected);
            coordinator.ShouldShowOrdinaryWarning(game.Type, game.Type, collection.Name, ["missing.mod"]).Should().BeFalse();
            modService.Verify(p => p.ResolveCollectionMods(installed, collection, previous), Times.Once);
        }

        [Fact]
        public void Current_revalidation_should_allow_only_its_authoritative_locked_resolution()
        {
            var game = new Game { Type = "game" };
            var current = new GameStateLockInfo
                { GameType = game.Type, Reason = GameStateLockReason.ConfigurationChanged };
            var stale = new GameStateLockInfo
                { GameType = game.Type, Reason = GameStateLockReason.ConfigurationChanged };
            var safety = new Mock<IGameStateSafetyService>();
            safety.Setup(p => p.IsLocked(game)).Returns(true);
            safety.Setup(p => p.IsCurrentRevalidation(game, It.IsAny<GameStateLockInfo>()))
                .Returns((IGame _, GameStateLockInfo token) => ReferenceEquals(token, current));
            var modService = new Mock<IModService>();
            var coordinator = new CollectionModReconciliationCoordinator(modService.Object, safety.Object);
            var collection = new ModCollection { Name = "Collection", Game = game.Type };
            var installed = new List<IMod> { new Mod { Name = "Installed" } };
            IReadOnlyCollection<IMod> expected = [installed[0]];
            modService.Setup(p => p.ResolveCollectionMods(installed, collection, It.IsAny<IEnumerable<IMod>>()))
                .Returns(expected);

            coordinator.TryResolve(game, installed, collection, out var staleResult, stale).Should().BeFalse();
            coordinator.TryResolve(game, installed, collection, out var currentResult, current).Should().BeTrue();

            staleResult.Should().BeEmpty();
            currentResult.Should().BeSameAs(expected);
            modService.Verify(p => p.ResolveCollectionMods(installed, collection, It.IsAny<IEnumerable<IMod>>()),
                Times.Once);
        }
    }
}
