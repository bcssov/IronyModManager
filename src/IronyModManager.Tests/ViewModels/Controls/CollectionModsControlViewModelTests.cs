// ***********************************************************************
// Assembly         : IronyModManager.Tests
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using AwesomeAssertions;
using Castle.DynamicProxy;
using IronyModManager.DI;
using IronyModManager.Common.Events;
using IronyModManager.Implementation.AppState;
using IronyModManager.Implementation.Hotkey;
using IronyModManager.Implementation.MessageBus.Events;
using IronyModManager.IO.Common.Mods;
using IronyModManager.Localization;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Mod.Search;
using IronyModManager.Services.Common;
using IronyModManager.Tests.Common;
using IronyModManager.ViewModels.Controls;
using Moq;
using ReactiveUI;
using Xunit;

namespace IronyModManager.Tests.ViewModels.Controls
{
    /// <summary>
    /// Tests collection commands at the ViewModel boundary.
    /// </summary>
    public class CollectionModsControlViewModelTests
    {
        [Fact]
        public void Conflict_solver_input_should_preserve_all_real_members()
        {
            var first = CreateMod("first.mod");
            var second = CreateMod("second.mod");
            var viewModel = CreateSolverInputViewModel([first, second]);

            viewModel.GetConflictSolverMods().Should().Equal(first, second);
            viewModel.SelectedMods.Should().Equal(first, second);
        }

        [Fact]
        public void Conflict_solver_input_should_filter_virtual_members_without_mutating_collection_order()
        {
            var first = CreateMod("first.mod");
            var missingFirst = CreateMod("missing-first.mod", isVirtual: true);
            var middle = CreateMod("middle.mod");
            var missingLast = CreateMod("missing-last.mod", isVirtual: true);
            var last = CreateMod("last.mod");
            IMod[] membership = [first, missingFirst, middle, missingLast, last];
            var viewModel = CreateSolverInputViewModel(membership);

            viewModel.GetConflictSolverMods().Should().Equal(first, middle, last);
            viewModel.SelectedMods.Should().Equal(membership);
        }

        [Fact]
        public void Conflict_solver_input_should_include_reappeared_real_shared_instance()
        {
            var missing = CreateMod("missing.mod", isVirtual: true);
            var reappeared = CreateMod(missing.DescriptorFile);
            var viewModel = CreateSolverInputViewModel([missing]);
            viewModel.GetConflictSolverMods().Should().BeEmpty();

            viewModel.ApplySelectedMods([reappeared], true);

            viewModel.GetConflictSolverMods().Should().ContainSingle().Which.Should().BeSameAs(reappeared);
        }

        [Fact]
        public void Conflict_solver_input_should_be_empty_for_all_virtual_members()
        {
            var members = new[]
            {
                CreateMod("missing-first.mod", isVirtual: true),
                CreateMod("missing-second.mod", isVirtual: true)
            };
            var viewModel = CreateSolverInputViewModel(members);

            viewModel.GetConflictSolverMods().Should().BeEmpty();
            viewModel.SelectedMods.Should().Equal(members);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void Remove_from_collection_command_should_remove_equivalent_virtual_and_persist_aligned_metadata(int removedIndex)
        {
            DISetup.SetupContainer();
            var game = new Game { Type = "game" };
            var first = CreateMod("mod/first.mod", "first-path", "First", game.Type, true,
                source: ModSource.Steam, remoteId: 11);
            var virtualMember = CreateMod("mod/missing.mod", "missing-path", "Missing", game.Type, true, true,
                ModSource.Steam, 22);
            var last = CreateMod("mod/last.mod", "last-path", "Last", game.Type,
                source: ModSource.Paradox, remoteId: 33);
            var equivalentContextMember = CreateMod(virtualMember.DescriptorFile, isVirtual: true);
            var collectionMembers = new List<IMod> { first, last };
            collectionMembers.Insert(removedIndex, virtualMember);
            var selectedCollection = new ModCollection
            {
                Name = "Collection", Game = game.Type,
                Mods = collectionMembers.Select(p => p.DescriptorFile).ToList(),
                ModPaths = null,
                ModNames = [first.Name],
                ModIds = null
            };
            var modService = CreateIdentityContractModService();
            var collectionService = new Mock<IModCollectionService>();
            collectionService.Setup(p => p.Create()).Returns(() => new ModCollection());
            IModCollection persisted = null;
            collectionService.Setup(p => p.SaveExplicitMembershipChange(It.IsAny<IModCollection>()))
                .Callback((IModCollection collection) => persisted = collection)
                .Returns(true);
            var patchService = new Mock<IModPatchCollectionService>();
            patchService.Setup(p => p.PatchModNeedsUpdateAsync(It.IsAny<string>(), It.IsAny<IReadOnlyCollection<string>>()))
                .ReturnsAsync(false);
            var gameService = new Mock<IGameService>();
            gameService.Setup(p => p.GetSelected()).Returns(game);
            var viewModel = CreateViewModel(modService, collectionService, patchService, gameService);
            using var disposables = new CompositeDisposable();
            viewModel.InitializeRemoval(disposables, game, selectedCollection, [first, last], collectionMembers);
            viewModel.ContextMenuMod = equivalentContextMember;

            using var execution = viewModel.RemoveFromCollectionCommand.Execute().Subscribe();

            viewModel.SelectedMods.Should().Equal(first, last);
            viewModel.Mods.Should().Equal(first, last);
            virtualMember.GetType().Should().NotBe(typeof(Mod));
            equivalentContextMember.GetType().Should().NotBe(typeof(Mod));
            virtualMember.Should().NotBeSameAs(equivalentContextMember);
            first.IsSelected.Should().BeTrue();
            last.IsSelected.Should().BeFalse();
            persisted.Should().NotBeNull();
            persisted.Mods.Should().Equal(first.DescriptorFile, last.DescriptorFile);
            persisted.ModPaths.Should().Equal(first.FullPath, last.FullPath);
            persisted.ModNames.Should().Equal(first.Name, last.Name);
            persisted.ModIds.Should().HaveCount(2);
            persisted.ModIds.First().SteamId.Should().Be(11);
            persisted.ModIds.Last().ParadoxId.Should().Be(33);
            selectedCollection.Mods.Should().Equal(first.DescriptorFile, last.DescriptorFile);
            selectedCollection.ModPaths.Should().Equal(first.FullPath, last.FullPath);
            selectedCollection.ModNames.Should().Equal(first.Name, last.Name);
            selectedCollection.ModIds.Should().HaveCount(2);
            collectionService.Verify(p => p.SaveExplicitMembershipChange(It.IsAny<IModCollection>()), Times.Once);
            collectionService.Verify(p => p.Save(It.IsAny<IModCollection>()), Times.Never);
            patchService.Verify(p => p.PatchModNeedsUpdateAsync(It.IsAny<string>(), It.IsAny<IReadOnlyCollection<string>>()), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void Remove_virtual_from_collection_undo_redo_should_restore_ordered_persisted_membership(int removedIndex)
        {
            DISetup.SetupContainer();
            var game = new Game { Type = "game" };
            var first = CreateMod("first.mod", "first-path", "First", game.Type, true, source: ModSource.Steam, remoteId: 11);
            var missing = CreateMod("missing.mod", "missing-path", "Missing", game.Type, true, true, ModSource.Steam, 22);
            var last = CreateMod("last.mod", "last-path", "Last", game.Type, true, source: ModSource.Paradox, remoteId: 33);
            var members = new List<IMod> { first, last };
            members.Insert(removedIndex, missing);
            var selectedCollection = new ModCollection { Name = "Collection", Game = game.Type };
            var modService = CreateResolvingModService();
            var collectionService = new Mock<IModCollectionService>();
            collectionService.Setup(p => p.Create()).Returns(() => new ModCollection());
            var persisted = new List<IModCollection>();
            collectionService.Setup(p => p.Save(It.IsAny<IModCollection>()))
                .Callback((IModCollection collection) => persisted.Add(collection)).Returns(true);
            collectionService.Setup(p => p.SaveExplicitMembershipChange(It.IsAny<IModCollection>())).Returns(true);
            var patchService = new Mock<IModPatchCollectionService>();
            var gameService = new Mock<IGameService>();
            gameService.Setup(p => p.GetSelected()).Returns(game);
            var reconciliation = new CollectionModReconciliationCoordinator(modService.Object, Mock.Of<IGameStateSafetyService>());
            var viewModel = CreateViewModel(modService.Object, collectionService, patchService, gameService, reconciliation: reconciliation);
            using var disposables = new CompositeDisposable();
            viewModel.InitializeRemoval(disposables, game, selectedCollection, [first, last], members);
            viewModel.InitializeHistoryCommands(disposables);
            viewModel.ContextMenuMod = CreateMod(missing.DescriptorFile, missing.FullPath, isVirtual: true);

            using var remove = viewModel.RemoveFromCollectionCommand.Execute().Subscribe();
            using var undo = viewModel.UndoCommand.Execute().Subscribe();

            viewModel.SelectedMods.Should().HaveCount(3);
            viewModel.SelectedMods[removedIndex].Should().NotBeSameAs(missing);
            viewModel.SelectedMods[removedIndex].IsVirtual.Should().BeTrue();
            viewModel.SelectedMods[removedIndex].DescriptorFile.Should().Be(missing.DescriptorFile);
            viewModel.SelectedMods[removedIndex].FullPath.Should().Be(missing.FullPath);
            viewModel.SelectedMods[removedIndex].Name.Should().Be(missing.Name);
            viewModel.SelectedMods[removedIndex].Source.Should().Be(ModSource.Steam);
            viewModel.SelectedMods[removedIndex].RemoteId.Should().Be(22);
            viewModel.SelectedMods.Select(p => p.DescriptorFile).Should().Equal(members.Select(p => p.DescriptorFile));
            persisted.Last().Mods.Should().Equal(members.Select(p => p.DescriptorFile));
            persisted.Last().ModPaths.Should().Equal(members.Select(p => p.FullPath));
            persisted.Last().ModNames.Should().Equal(members.Select(p => p.Name));
            persisted.Last().ModIds.Should().HaveCount(3);
            persisted.Last().ModIds.ElementAt(removedIndex).SteamId.Should().Be(22);

            using var redo = viewModel.RedoCommand.Execute().Subscribe();
            viewModel.SelectedMods.Should().Equal(first, last);
            persisted.Last().Mods.Should().Equal(first.DescriptorFile, last.DescriptorFile);

            using var repeatUndo = viewModel.UndoCommand.Execute().Subscribe();
            viewModel.SelectedMods.Select(p => p.DescriptorFile).Should().Equal(members.Select(p => p.DescriptorFile));
            modService.Verify(p => p.ResolveCollectionMods(It.IsAny<IEnumerable<IMod>>(), It.IsAny<IModCollection>(),
                It.IsAny<IEnumerable<IMod>>()), Times.Exactly(3));
        }

        [Fact]
        public void Undo_should_resolve_historical_virtual_membership_to_the_current_shared_real_mod()
        {
            DISetup.SetupContainer();
            var game = new Game { Type = "game" };
            var first = CreateMod("first.mod", game: game.Type, isSelected: true);
            var missing = CreateMod("missing.mod", "missing-path", "Missing", game.Type, true, true, ModSource.Steam, 22);
            var currentReal = CreateMod(missing.DescriptorFile, missing.FullPath, "Installed", game.Type);
            var collection = new ModCollection { Name = "Collection", Game = game.Type };
            var modService = CreateResolvingModService();
            var collectionService = new Mock<IModCollectionService>();
            collectionService.Setup(p => p.Create()).Returns(() => new ModCollection());
            collectionService.Setup(p => p.Save(It.IsAny<IModCollection>())).Returns(true);
            collectionService.Setup(p => p.SaveExplicitMembershipChange(It.IsAny<IModCollection>())).Returns(true);
            var gameService = new Mock<IGameService>();
            gameService.Setup(p => p.GetSelected()).Returns(game);
            var reconciliation = new CollectionModReconciliationCoordinator(modService.Object, Mock.Of<IGameStateSafetyService>());
            var viewModel = CreateViewModel(modService.Object, collectionService, new Mock<IModPatchCollectionService>(), gameService,
                reconciliation: reconciliation);
            using var disposables = new CompositeDisposable();
            viewModel.InitializeRemoval(disposables, game, collection, [first], [first, missing]);
            viewModel.InitializeHistoryCommands(disposables);
            viewModel.ContextMenuMod = missing;
            using var remove = viewModel.RemoveFromCollectionCommand.Execute().Subscribe();
            viewModel.ReplaceInstalledMods([first, currentReal]);

            using var undo = viewModel.UndoCommand.Execute().Subscribe();

            viewModel.SelectedMods.Should().Contain(currentReal);
            viewModel.SelectedMods.Single(p => p.DescriptorFile == missing.DescriptorFile).Should().BeSameAs(currentReal);
            currentReal.IsVirtual.Should().BeFalse();
        }

        [Fact]
        public void Castle_proxied_view_model_should_remove_undo_and_redo_virtual_collection_members()
        {
            DISetup.SetupContainer();
            var game = new Game { Type = "game" };
            var first = CreateMod("first.mod", game: game.Type, isSelected: true);
            var missing = CreateMod("missing.mod", "missing-path", "Missing", game.Type, true, true, ModSource.Steam, 22);
            var last = CreateMod("last.mod", game: game.Type, isSelected: true);
            var collection = new ModCollection { Name = "Collection", Game = game.Type };
            var modService = CreateResolvingModService();
            var collectionService = new Mock<IModCollectionService>();
            collectionService.Setup(p => p.Create()).Returns(() => new ModCollection());
            collectionService.Setup(p => p.GetAll()).Returns([]);
            collectionService.Setup(p => p.Save(It.IsAny<IModCollection>())).Returns(true);
            collectionService.Setup(p => p.SaveExplicitMembershipChange(It.IsAny<IModCollection>())).Returns(true);
            var gameService = new Mock<IGameService>();
            gameService.Setup(p => p.GetSelected()).Returns(game);
            var reconciliation = new CollectionModReconciliationCoordinator(modService.Object, Mock.Of<IGameStateSafetyService>());
            var viewModel = CreateCastleProxiedViewModel(modService.Object, collectionService,
                new Mock<IModPatchCollectionService>(), gameService, reconciliation);
            using var disposables = new CompositeDisposable();
            InitializeCastleProxy(viewModel, disposables, game, collection, [first, last], [first, missing, last]);
            viewModel.ContextMenuMod = missing;

            Exception commandError = null;
            using var remove = viewModel.RemoveFromCollectionCommand.Execute().Subscribe(_ => { }, exception => commandError = exception);
            using var undo = viewModel.UndoCommand.Execute().Subscribe(_ => { }, exception => commandError = exception);
            using var redo = viewModel.RedoCommand.Execute().Subscribe(_ => { }, exception => commandError = exception);

            commandError.Should().BeNull();
            viewModel.SelectedMods.Should().Equal(first, last);
            viewModel.IsUndoAvailable().Should().BeTrue();
            modService.Verify(p => p.ResolveCollectionMods(It.IsAny<IEnumerable<IMod>>(), It.IsAny<IModCollection>(),
                It.IsAny<IEnumerable<IMod>>()), Times.Exactly(2));
        }

        [Fact]
        public void Enable_all_propagation_should_match_collection_members_by_domain_equivalence()
        {
            DISetup.SetupContainer();
            var game = new Game { Type = "game" };
            var member = CreateMod("member", "member-path", "Same", game.Type, true);
            var equivalentFilteredMod = CreateMod("member", "member-path", "Same", game.Type, true);
            var similarNonMember = CreateMod("different", "different-path", "Same", game.Type, true);
            var virtualMember = CreateMod("missing", "missing-path", game: game.Type, isSelected: true, isVirtual: true);
            var collection = new ModCollection
            {
                Name = "Collection", Game = game.Type,
                Mods = [member.DescriptorFile, similarNonMember.DescriptorFile, virtualMember.DescriptorFile]
            };
            var modService = new Mock<IModService>();
            modService.Setup(p => p.AreModIdentitiesEquivalent(It.IsAny<IMod>(), It.IsAny<IMod>()))
                .Returns((IMod candidate, IMod requested) => candidate.DescriptorFile == requested.DescriptorFile);
            var collectionService = new Mock<IModCollectionService>();
            collectionService.Setup(p => p.Create()).Returns(() => new ModCollection());
            collectionService.Setup(p => p.Save(It.IsAny<IModCollection>())).Returns(true);
            var patchService = new Mock<IModPatchCollectionService>();
            patchService.Setup(p => p.PatchModNeedsUpdateAsync(It.IsAny<string>(), It.IsAny<IReadOnlyCollection<string>>()))
                .ReturnsAsync(false);
            var gameService = new Mock<IGameService>();
            gameService.Setup(p => p.GetSelected()).Returns(game);
            var viewModel = CreateViewModel(modService.Object, collectionService, patchService, gameService);
            viewModel.InitializeState(game, collection, [member, similarNonMember]);
            viewModel.ApplySelectedMods([member, similarNonMember, virtualMember]);

            viewModel.HandleEnableAllToggled(true, true, [equivalentFilteredMod]);
            viewModel.HandleEnableAllToggled(false, true, [equivalentFilteredMod]);

            viewModel.SelectedMods.Should().Equal(member, similarNonMember, virtualMember);

            viewModel.HandleEnableAllToggled(true, false, [equivalentFilteredMod]);
            viewModel.HandleEnableAllToggled(false, false, [equivalentFilteredMod]);

            viewModel.SelectedMods.Should().Equal(similarNonMember, virtualMember);
            similarNonMember.IsSelected.Should().BeTrue();
            virtualMember.IsSelected.Should().BeTrue();
        }

        [Fact]
        public void Equivalent_reconstructed_projection_should_not_create_undo_history()
        {
            DISetup.SetupContainer();
            var game = new Game { Type = "game" };
            var previousFirst = CreateMod("first", name: "Same");
            var previousSecond = CreateMod("second", name: "Second");
            var reconstructedFirst = CreateMod("first", name: "Same");
            var reconstructedSecond = CreateMod("second", name: "Second");
            var similarButDifferent = CreateMod("different", name: "Same");
            var collection = new ModCollection { Name = "Collection", Game = game.Type, Mods = ["first", "second"] };
            var modService = new Mock<IModService>();
            modService.Setup(p => p.AreModIdentitiesEquivalent(It.IsAny<IMod>(), It.IsAny<IMod>()))
                .Returns((IMod candidate, IMod requested) => candidate.DescriptorFile == requested.DescriptorFile);
            var collectionService = new Mock<IModCollectionService>();
            var patchService = new Mock<IModPatchCollectionService>();
            var gameService = new Mock<IGameService>();
            var viewModel = CreateViewModel(modService.Object, collectionService, patchService, gameService,
                [previousFirst, previousSecond]);
            viewModel.InitializeState(game, collection, [reconstructedFirst, reconstructedSecond, similarButDifferent]);

            viewModel.ApplySelectedMods([reconstructedFirst, reconstructedSecond]);

            viewModel.IsUndoAvailable().Should().BeFalse();

            viewModel.ApplySelectedMods([similarButDifferent, reconstructedSecond]);

            viewModel.IsUndoAvailable().Should().BeTrue();
        }

        [Fact]
        public void Reordered_equivalent_projection_should_create_undo_history()
        {
            DISetup.SetupContainer();
            var game = new Game { Type = "game" };
            var previousFirst = CreateMod("first");
            var previousSecond = CreateMod("second");
            var reconstructedFirst = CreateMod("first");
            var reconstructedSecond = CreateMod("second");
            var collection = new ModCollection { Name = "Collection", Game = game.Type, Mods = ["first", "second"] };
            var modService = new Mock<IModService>();
            modService.Setup(p => p.AreModIdentitiesEquivalent(It.IsAny<IMod>(), It.IsAny<IMod>()))
                .Returns((IMod candidate, IMod requested) => candidate.DescriptorFile == requested.DescriptorFile);
            var viewModel = CreateViewModel(modService.Object, new Mock<IModCollectionService>(),
                new Mock<IModPatchCollectionService>(), new Mock<IGameService>(), [previousFirst, previousSecond]);
            viewModel.InitializeState(game, collection, [reconstructedFirst, reconstructedSecond]);

            viewModel.ApplySelectedMods([reconstructedSecond, reconstructedFirst]);

            viewModel.IsUndoAvailable().Should().BeTrue();
        }

        [Fact]
        public async Task Directory_revalidation_should_publish_one_live_proxy_graph_before_unlock()
        {
            DISetup.SetupContainer();
            var game = new Game { Type = "game" };
            var previousMod = CreateMod("mod/member.mod", "member-path", game: game.Type, isSelected: true);
            var refreshedMod = CreateMod("mod/member.mod", "member-path", game: game.Type);
            var collection = new ModCollection
            {
                Name = "Collection", Game = game.Type, IsSelected = true,
                Mods = [previousMod.DescriptorFile], ModPaths = [previousMod.FullPath]
            };
            GameStateLockInfo currentLock = null;
            var safety = new Mock<IGameStateSafetyService>();
            safety.Setup(p => p.BeginRevalidation(game, It.IsAny<string>())).Returns(() =>
            {
                currentLock = new GameStateLockInfo
                    { GameType = game.Type, Reason = GameStateLockReason.ConfigurationChanged };
                return currentLock;
            });
            safety.Setup(p => p.IsLocked(game)).Returns(() => currentLock != null);
            safety.Setup(p => p.IsCurrentRevalidation(game, It.IsAny<GameStateLockInfo>()))
                .Returns((IGame _, GameStateLockInfo token) => ReferenceEquals(currentLock, token));
            safety.Setup(p => p.CompleteRevalidation(game, It.IsAny<GameStateLockInfo>(), It.IsAny<Action>()))
                .Returns((IGame _, GameStateLockInfo token, Action beforeUnlock) =>
                {
                    if (!ReferenceEquals(currentLock, token))
                    {
                        return false;
                    }

                    beforeUnlock();
                    currentLock = null;
                    return true;
                });
            var modService = new Mock<IModService>();
            modService.Setup(p => p.AreModIdentitiesEquivalent(It.IsAny<IMod>(), It.IsAny<IMod>()))
                .Returns((IMod candidate, IMod requested) =>
                    string.Equals(candidate.DescriptorFile, requested.DescriptorFile,
                        StringComparison.OrdinalIgnoreCase));
            modService.Setup(p => p.ResolveCollectionMods(It.IsAny<IEnumerable<IMod>>(), collection,
                    It.IsAny<IEnumerable<IMod>>()))
                .Returns((IEnumerable<IMod> installed, IModCollection _, IEnumerable<IMod> _) =>
                    installed.Where(p => string.Equals(p.DescriptorFile, previousMod.DescriptorFile,
                        StringComparison.OrdinalIgnoreCase)).ToList());
            var collectionService = new Mock<IModCollectionService>();
            collectionService.Setup(p => p.Create()).Returns(() => new ModCollection());
            collectionService.Setup(p => p.Get(collection.Name)).Returns(collection);
            collectionService.Setup(p => p.GetAll()).Returns([collection]);
            collectionService.Setup(p => p.Save(It.IsAny<IModCollection>())).Returns(true);
            var patchService = new Mock<IModPatchCollectionService>();
            patchService.Setup(p => p.PatchModNeedsUpdateAsync(It.IsAny<string>(),
                It.IsAny<IReadOnlyCollection<string>>())).ReturnsAsync(false);
            var gameService = new Mock<IGameService>();
            gameService.Setup(p => p.GetSelected()).Returns(game);
            var collectionReconciliation = new CollectionModReconciliationCoordinator(
                modService.Object, safety.Object);
            collectionReconciliation.RecordCollectionMods(collection.Name, [previousMod]);
            var viewModel = CreateViewModel(modService.Object, collectionService, patchService, gameService,
                [previousMod], collectionReconciliation);
            viewModel.InitializeState(game, collection, [previousMod]);
            viewModel.ApplySelectedMods([previousMod], ignoreStack: true);
            using var subscriptions = viewModel.StartModSubscriptions();
            var installedMods = new List<IMod> { previousMod };
            var filteredMods = installedMods.ToList();
            var stateReconciliation = new ModStateReconciliationCoordinator(safety.Object);

            var recovered = await stateReconciliation.RevalidateConfigurationAsync(game,
                (_, _) =>
                {
                    installedMods = [refreshedMod];
                    filteredMods = installedMods.ToList();
                    return Task.FromResult(true);
                },
                collectionReconciliation.Reset,
                token => viewModel.PublishMods(installedMods, game, token));

            recovered.Should().BeTrue();
            safety.Object.IsLocked(game).Should().BeFalse();
            previousMod.Should().NotBeSameAs(refreshedMod);
            installedMods.Single().Should().BeSameAs(refreshedMod);
            filteredMods.Single().Should().BeSameAs(refreshedMod);
            viewModel.Mods.Single().Should().BeSameAs(refreshedMod);
            viewModel.SelectedMods.Single().Should().BeSameAs(refreshedMod);
            refreshedMod.IsSelected.Should().BeTrue();

            refreshedMod.IsSelected = false;
            await Task.Delay(25);
            viewModel.SelectedMods.Should().BeEmpty();
            refreshedMod.IsSelected = true;
            await Task.Delay(25);
            viewModel.SelectedMods.Single().Should().BeSameAs(refreshedMod);
            refreshedMod.IsSelected = false;
            await Task.Delay(25);
            viewModel.SelectedMods.Should().BeEmpty();
        }

        private static IMod CreateMod(string descriptor, string fullPath = null, string name = null, string game = null,
            bool isSelected = false, bool isVirtual = false, ModSource source = ModSource.Local, long? remoteId = null)
        {
            var mod = DIResolver.Get<IMod>();
            mod.DescriptorFile = descriptor;
            mod.FullPath = fullPath ?? descriptor;
            mod.Name = name ?? descriptor;
            mod.Game = game;
            mod.IsSelected = isSelected;
            mod.IsVirtual = isVirtual;
            mod.Source = source;
            mod.RemoteId = remoteId;
            return mod;
        }

        private static TestCollectionModsControlViewModel CreateSolverInputViewModel(IList<IMod> members)
        {
            DISetup.SetupContainer();
            var viewModel = CreateViewModel(Mock.Of<IModService>(), new Mock<IModCollectionService>(),
                new Mock<IModPatchCollectionService>(), new Mock<IGameService>());
            viewModel.ApplySelectedMods(members, true);
            return viewModel;
        }

        private static IModService CreateIdentityContractModService()
        {
            var modService = new Mock<IModService>();
            modService.Setup(p => p.AreModIdentitiesEquivalent(It.IsAny<IMod>(), It.IsAny<IMod>()))
                .Returns((IMod mod, IMod otherMod) => mod != null && otherMod != null &&
                    ((!string.IsNullOrWhiteSpace(otherMod.DescriptorFile) &&
                      string.Equals(mod.DescriptorFile, otherMod.DescriptorFile, StringComparison.OrdinalIgnoreCase)) ||
                     (!string.IsNullOrWhiteSpace(otherMod.FullPath) &&
                      string.Equals(mod.FullPath, otherMod.FullPath, StringComparison.OrdinalIgnoreCase))));
            return modService.Object;
        }

        private static Mock<IModService> CreateResolvingModService()
        {
            var modService = new Mock<IModService>();
            modService.Setup(p => p.AreModIdentitiesEquivalent(It.IsAny<IMod>(), It.IsAny<IMod>()))
                .Returns((IMod mod, IMod otherMod) => mod != null && otherMod != null &&
                    (string.Equals(mod.DescriptorFile, otherMod.DescriptorFile, StringComparison.OrdinalIgnoreCase) ||
                     (!string.IsNullOrWhiteSpace(otherMod.FullPath) &&
                      string.Equals(mod.FullPath, otherMod.FullPath, StringComparison.OrdinalIgnoreCase))));
            modService.Setup(p => p.ResolveCollectionMods(It.IsAny<IEnumerable<IMod>>(), It.IsAny<IModCollection>(),
                    It.IsAny<IEnumerable<IMod>>()))
                .Returns((IEnumerable<IMod> installed, IModCollection collection, IEnumerable<IMod> _) =>
                {
                    var installedMods = installed.ToList();
                    var descriptors = collection.Mods.ToList();
                    var paths = collection.ModPaths.ToList();
                    var names = collection.ModNames.ToList();
                    var sourceInfos = collection.ModIds.ToList();
                    return descriptors.Select((descriptor, index) =>
                    {
                        var current = installedMods.FirstOrDefault(mod =>
                            string.Equals(mod.DescriptorFile, descriptor, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(mod.FullPath, paths[index], StringComparison.OrdinalIgnoreCase));
                        if (current != null)
                        {
                            return current;
                        }

                        var virtualMod = CreateMod(descriptor, paths[index], names[index], collection.Game, true, true);
                        virtualMod.Source = sourceInfos[index].SteamId != null ? ModSource.Steam :
                            sourceInfos[index].ParadoxId != null ? ModSource.Paradox : ModSource.Local;
                        virtualMod.RemoteId = sourceInfos[index].SteamId ?? sourceInfos[index].ParadoxId;
                        return virtualMod;
                    }).ToList();
                });
            return modService;
        }

        private static TestCollectionModsControlViewModel CreateViewModel(IModService modService,
            Mock<IModCollectionService> collectionService, Mock<IModPatchCollectionService> patchService,
            Mock<IGameService> gameService, IEnumerable<IMod> previousMods = null,
            CollectionModReconciliationCoordinator reconciliation = null)
        {
            var search = new SearchModsControlViewModel();
            var sort = new SortOrderControlViewModel(Mock.Of<ILocalizationManager>());
            var presentation = new Mock<CollectionModsPresentationCoordinator>(MockBehavior.Loose,
                new object[] { null, null, search, sort });
            presentation.SetupGet(p => p.SearchMods).Returns(search);
            presentation.SetupGet(p => p.ModNameSortOrder).Returns(sort);
            presentation.Setup(p => p.Restore(It.IsAny<string>(), It.IsAny<string>())).Returns((false, null));
            if (reconciliation == null)
            {
                var mockReconciliation = new Mock<CollectionModReconciliationCoordinator>(MockBehavior.Loose,
                    new object[] { modService, Mock.Of<IGameStateSafetyService>() });
                mockReconciliation.Setup(p => p.GetPreviousCollectionMods(It.IsAny<string>()))
                    .Returns(previousMods ?? []);
                reconciliation = mockReconciliation.Object;
            }

            var factory = new Mock<ICollectionModsCoordinatorFactory>();
            var events = new Mock<CollectionModsEventCoordinator>(MockBehavior.Loose,
                new object[] { null, null });
            events.Setup(p => p.SubscribeHotkeys(It.IsAny<Action<MainViewHotkeyPressedEvent>>())).Returns(Disposable.Empty);
            events.Setup(p => p.SubscribeCollectionChanges(It.IsAny<Action<ModCollectionChangeRequestEvent>>())).Returns(Disposable.Empty);
            factory.Setup(p => p.Create()).Returns(new CollectionModsCoordinators(
                new CollectionModMembership(modService), reconciliation,
                new Mock<CollectionOperationProgressCoordinator>(MockBehavior.Loose,
                    new object[] { null, null, null, null }).Object,
                presentation.Object,
                events.Object));
            var patch = new Mock<PatchModControlViewModel>(MockBehavior.Loose,
                new object[] { null, collectionService.Object, modService, null });
            var modify = new Mock<ModifyCollectionControlViewModel>(MockBehavior.Loose,
                new object[] { null, null, null, null, null, null, null, null, null, null, null, null });
            var interaction = new Mock<CollectionModsInteraction>(MockBehavior.Loose,
                new object[] { null, null, null, null, null });
            interaction.Setup(p => p.GetText(It.IsAny<string>())).Returns("{State}");

            return new TestCollectionModsControlViewModel(null, patch.Object, new HashReportControlViewModel(null),
                collectionService.Object, patchService.Object, modService, gameService.Object,
                new AddNewCollectionControlViewModel(null, collectionService.Object, patchService.Object),
                new ExportModCollectionControlViewModel(gameService.Object, null), modify.Object,
                interaction.Object, factory.Object);
        }

        private static CollectionModsControlViewModel CreateCastleProxiedViewModel(IModService modService,
            Mock<IModCollectionService> collectionService, Mock<IModPatchCollectionService> patchService,
            Mock<IGameService> gameService, CollectionModReconciliationCoordinator reconciliation)
        {
            var search = new SearchModsControlViewModel();
            var sort = new SortOrderControlViewModel(Mock.Of<ILocalizationManager>());
            var presentation = new Mock<CollectionModsPresentationCoordinator>(MockBehavior.Loose,
                new object[] { null, null, search, sort });
            presentation.SetupGet(p => p.SearchMods).Returns(search);
            presentation.SetupGet(p => p.ModNameSortOrder).Returns(sort);
            presentation.Setup(p => p.Restore(It.IsAny<string>(), It.IsAny<string>())).Returns((false, null));
            var factory = new Mock<ICollectionModsCoordinatorFactory>();
            var events = new Mock<CollectionModsEventCoordinator>(MockBehavior.Loose,
                new object[] { null, null });
            events.Setup(p => p.SubscribeHotkeys(It.IsAny<Action<MainViewHotkeyPressedEvent>>())).Returns(Disposable.Empty);
            events.Setup(p => p.SubscribeCollectionChanges(It.IsAny<Action<ModCollectionChangeRequestEvent>>())).Returns(Disposable.Empty);
            factory.Setup(p => p.Create()).Returns(new CollectionModsCoordinators(
                new CollectionModMembership(modService), reconciliation,
                new Mock<CollectionOperationProgressCoordinator>(MockBehavior.Loose,
                    new object[] { null, null, null, null }).Object,
                presentation.Object,
                events.Object));
            var patch = new Mock<PatchModControlViewModel>(MockBehavior.Loose,
                new object[] { null, collectionService.Object, modService, null });
            var modify = new Mock<ModifyCollectionControlViewModel>(MockBehavior.Loose,
                new object[] { null, null, null, null, null, null, null, null, null, null, null, null });
            var interaction = new Mock<CollectionModsInteraction>(MockBehavior.Loose,
                new object[] { null, null, null, null, null });
            interaction.Setup(p => p.GetText(It.IsAny<string>())).Returns("{State}");
            return new ProxyGenerator().CreateClassProxy<CollectionModsControlViewModel>(
                [null, patch.Object, new HashReportControlViewModel(null), collectionService.Object, patchService.Object,
                    modService, gameService.Object, new AddNewCollectionControlViewModel(null, collectionService.Object, patchService.Object),
                    new ExportModCollectionControlViewModel(gameService.Object, null), modify.Object, interaction.Object, factory.Object],
                new ProceedingInterceptor());
        }

        private static void InitializeCastleProxy(CollectionModsControlViewModel viewModel, CompositeDisposable disposables,
            IGame game, IModCollection collection, IEnumerable<IMod> installedMods, IList<IMod> collectionMembers)
        {
            typeof(CollectionModsControlViewModel).GetField("activeGame", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(viewModel, game);
            SetProtectedProperty(viewModel, nameof(CollectionModsControlViewModel.Mods), installedMods);
            SetProtectedProperty(viewModel, nameof(CollectionModsControlViewModel.SelectedModCollection), collection);
            typeof(CollectionModsControlViewModel).GetMethod("OnActivated", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(viewModel, [disposables]);
            typeof(CollectionModsControlViewModel).GetMethod("SetSelectedModsState", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(viewModel, [collectionMembers, true, true, false]);
        }

        private static void SetProtectedProperty(CollectionModsControlViewModel viewModel, string name, object value)
        {
            typeof(CollectionModsControlViewModel).GetProperty(name, BindingFlags.Instance | BindingFlags.Public)!
                .SetValue(viewModel, value);
        }

        private sealed class ProceedingInterceptor : IInterceptor
        {
            public void Intercept(Castle.DynamicProxy.IInvocation invocation)
            {
                invocation.Proceed();
            }
        }

        private sealed class TestCollectionModsControlViewModel : CollectionModsControlViewModel
        {
            public TestCollectionModsControlViewModel(IReportExportService reportExportService, PatchModControlViewModel patchMod,
                HashReportControlViewModel hashReportView, IModCollectionService modCollectionService,
                IModPatchCollectionService modPatchCollectionService, IModService modService, IGameService gameService,
                AddNewCollectionControlViewModel addNewCollection, ExportModCollectionControlViewModel exportCollection,
                ModifyCollectionControlViewModel modifyCollection, CollectionModsInteraction interaction,
                ICollectionModsCoordinatorFactory coordinatorFactory)
                : base(reportExportService, patchMod, hashReportView, modCollectionService, modPatchCollectionService,
                    modService, gameService, addNewCollection, exportCollection, modifyCollection, interaction, coordinatorFactory)
            {
            }

            public void InitializeRemoval(CompositeDisposable disposables, IGame game, IModCollection selectedCollection,
                IEnumerable<IMod> installedMods, IList<IMod> collectionMembers)
            {
                InitializeState(game, selectedCollection, installedMods);
                ApplySelectedMods(collectionMembers, ignoreStack: true);
                InitializeRemoveFromCollectionCommand(disposables);
            }

            public void InitializeState(IGame game, IModCollection selectedCollection, IEnumerable<IMod> installedMods)
            {
                typeof(CollectionModsControlViewModel).GetField("activeGame", BindingFlags.Instance | BindingFlags.NonPublic)!
                    .SetValue(this, game);
                Mods = installedMods;
                SelectedModCollection = selectedCollection;
            }

            public CompositeDisposable StartModSubscriptions()
            {
                var disposables = new CompositeDisposable();
                typeof(IronyModManager.Common.ViewModels.BaseViewModel)
                    .GetProperty("Disposables", BindingFlags.Instance | BindingFlags.NonPublic)!
                    .SetValue(this, disposables);
                SubscribeToMods();
                return disposables;
            }

            public void ApplySelectedMods(IList<IMod> collectionMembers, bool ignoreStack = false)
            {
                SetSelectedModsState(collectionMembers, ignoreStack: ignoreStack, evaluatePatchState: false);
            }

            public void PublishMods(IEnumerable<IMod> mods, IGame game, GameStateLockInfo revalidationLock)
            {
                SetMods(mods, game, revalidationLock);
            }

            public void InitializeHistoryCommands(CompositeDisposable disposables)
            {
                UndoCommand = ReactiveCommand.Create(PerformUndo).DisposeWith(disposables);
                RedoCommand = ReactiveCommand.Create(PerformRedo).DisposeWith(disposables);
            }

            public void ReplaceInstalledMods(IEnumerable<IMod> mods)
            {
                Mods = mods;
            }
        }
    }
}
