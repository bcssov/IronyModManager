// ***********************************************************************
// Assembly         : IronyModManager.Tests
// ***********************************************************************

using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using AwesomeAssertions;
using IronyModManager.Common;
using IronyModManager.Common.Events;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.Hotkey;
using IronyModManager.Implementation.MessageBus;
using IronyModManager.Implementation.MessageBus.Events;
using IronyModManager.Implementation.Overlay;
using IronyModManager.Implementation.AppState;
using IronyModManager.IO.Common.MessageBus;
using IronyModManager.Localization;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Services.Common.MessageBus;
using IronyModManager.Shared;
using IronyModManager.Tests.Common;
using IronyModManager.ViewModels.Controls;
using Moq;
using Xunit;

namespace IronyModManager.Tests.ViewModels.Controls
{
    /// <summary>
    /// Tests behavioral collaborators extracted from the dependency-heavy mod controls.
    /// </summary>
    public class ViewModelCoordinatorTests
    {
        [Fact]
        public void Installed_presentation_should_restore_exclusive_sort_and_save_changes()
        {
            DISetup.SetupContainer();
            var state = new AppState
            {
                InstalledModsSearchTerm = "initial",
                InstalledModsSortColumn = InstalledModsPresentationCoordinator.ModVersionKey,
                InstalledModsSortMode = (int)Implementation.SortOrder.Desc
            };
            var appState = new Mock<IAppStateService>();
            appState.Setup(p => p.Get()).Returns(state);
            var localization = new Mock<ILocalizationManager>().Object;
            var selected = new SortOrderControlViewModel(localization);
            var name = new SortOrderControlViewModel(localization);
            var version = new SortOrderControlViewModel(localization);
            var search = new SearchModsControlViewModel();
            var coordinator = new InstalledModsPresentationCoordinator(appState.Object, selected, name, version, search);

            coordinator.Initialize("Name", "Version", "Enabled", "Filter");

            coordinator.GetActiveSortKey().Should().Be(InstalledModsPresentationCoordinator.ModVersionKey);
            version.SortOrder.Should().Be(Implementation.SortOrder.Desc);
            name.SortOrder.Should().Be(Implementation.SortOrder.None);
            search.Text.Should().Be("initial");

            search.Text = "changed";
            name.SetSortOrder(Implementation.SortOrder.Asc);
            coordinator.ResetOtherSortOrders(name);
            coordinator.SaveState();

            state.InstalledModsSearchTerm.Should().Be("changed");
            state.InstalledModsSortColumn.Should().Be(InstalledModsPresentationCoordinator.ModNameKey);
            state.InstalledModsSortMode.Should().Be((int)Implementation.SortOrder.Asc);
            appState.Verify(p => p.Save(state), Times.Once);
        }

        [Fact]
        public async Task Collection_progress_should_map_operation_and_end_subscription()
        {
            DISetup.SetupContainer();
            var export = new ModExportProgressHandler();
            var rename = new PatchModRenameProgressHandler();
            var report = new ModReportExportHandler();
            var localization = CreateLocalization();
            var coordinator = new CollectionOperationProgressCoordinator(export, rename, report, localization.Object);
            var overlays = new List<(long Id, string Message)>();
            using var disposables = new CompositeDisposable();

            coordinator.SubscribeCollectionTransfer(42, true, disposables, (id, _, message, _) => overlays.Add((id, message)));
            await export.OnHandle(new ModExportProgressEvent(0.5));

            overlays.Should().ContainSingle();
            overlays[0].Should().Be((42, Shared.LocalizationResources.Collection_Mods.Overlay_Importing_Message));

            coordinator.CompleteCollectionTransfer();
            await export.OnHandle(new ModExportProgressEvent(0.75));
            overlays.Should().ContainSingle();
        }

        [Fact]
        public void Collection_presentation_should_restore_save_and_coordinate_scroll_state()
        {
            DISetup.SetupContainer();
            var state = new AppState
            {
                CollectionModsSearchTerm = "search",
                CollectionModsSelectedMod = "mod/example.mod",
                CollectionJumpOnPositionChange = true
            };
            var appState = new Mock<IAppStateService>();
            appState.Setup(p => p.Get()).Returns(state);
            var scrollState = new Mock<IScrollState>();
            var search = new SearchModsControlViewModel();
            var sort = new SortOrderControlViewModel(new Mock<ILocalizationManager>().Object);
            var coordinator = new CollectionModsPresentationCoordinator(appState.Object, scrollState.Object, search, sort);

            var restored = coordinator.Restore("Filter", "Name");

            restored.Should().Be((true, "mod/example.mod"));
            search.Text.Should().Be("search");
            search.WatermarkText.Should().Be("Filter");
            sort.Text.Should().Be("Name");

            search.Text = "changed";
            coordinator.Save("mod/changed.mod", false, false);
            coordinator.SetScrollState(false);
            coordinator.SetScrollState(true);

            state.CollectionModsSearchTerm.Should().Be("changed");
            state.CollectionModsSelectedMod.Should().Be("mod/changed.mod");
            state.CollectionJumpOnPositionChange.Should().BeFalse();
            state.CollectionModsSortColumn.Should().Be(CollectionModsPresentationCoordinator.ModNameKey);
            appState.Verify(p => p.Save(state), Times.Once);
            scrollState.Verify(p => p.SetState(false), Times.Once);
            scrollState.Verify(p => p.SetState(true), Times.Once);
        }

        [Fact]
        public void Collection_factory_should_create_one_typed_set_of_separate_collaborators()
        {
            DISetup.SetupContainer();
            var search = new SearchModsControlViewModel();
            var sort = new SortOrderControlViewModel(Mock.Of<ILocalizationManager>());
            var factory = new CollectionModsCoordinatorFactory(Mock.Of<IModService>(), Mock.Of<IGameStateSafetyService>(),
                new ModExportProgressHandler(), new PatchModRenameProgressHandler(), new ModReportExportHandler(),
                Mock.Of<ILocalizationManager>(), Mock.Of<IAppStateService>(), Mock.Of<IScrollState>(), search, sort,
                new ModCollectionChangeRequestHandler(), new MainViewHotkeyPressedHandler());

            var coordinators = factory.Create();

            coordinators.Membership.Should().BeOfType<CollectionModMembership>();
            coordinators.Reconciliation.Should().BeOfType<CollectionModReconciliationCoordinator>();
            coordinators.Operations.Should().BeOfType<CollectionOperationProgressCoordinator>();
            coordinators.Presentation.Should().BeOfType<CollectionModsPresentationCoordinator>();
            coordinators.Events.Should().BeOfType<CollectionModsEventCoordinator>();
            coordinators.Presentation.SearchMods.Should().BeSameAs(search);
            coordinators.Presentation.ModNameSortOrder.Should().BeSameAs(sort);
            var secondScope = factory.Create();
            secondScope.Membership.Should().NotBeSameAs(coordinators.Membership);
            secondScope.Reconciliation.Should().NotBeSameAs(coordinators.Reconciliation);
            secondScope.Operations.Should().NotBeSameAs(coordinators.Operations);
            secondScope.Presentation.Should().NotBeSameAs(coordinators.Presentation);
            secondScope.Events.Should().NotBeSameAs(coordinators.Events);
        }

        [Fact]
        public async Task Collection_event_scope_should_release_both_bus_subscriptions()
        {
            var collectionChanges = new ModCollectionChangeRequestHandler();
            var hotkeys = new MainViewHotkeyPressedHandler();
            var coordinator = new CollectionModsEventCoordinator(collectionChanges, hotkeys);
            var collectionCount = 0;
            var hotkeyCount = 0;
            using (var subscriptions = new CompositeDisposable
            {
                coordinator.SubscribeCollectionChanges(_ => collectionCount++),
                coordinator.SubscribeHotkeys(_ => hotkeyCount++)
            })
            {
                await collectionChanges.OnHandle(new ModCollectionChangeRequestEvent(new ModCollection()));
                await hotkeys.OnHandle(new MainViewHotkeyPressedEvent(Enums.HotKeys.Ctrl_Z));
            }

            await collectionChanges.OnHandle(new ModCollectionChangeRequestEvent(new ModCollection()));
            await hotkeys.OnHandle(new MainViewHotkeyPressedEvent(Enums.HotKeys.Ctrl_Z));

            collectionCount.Should().Be(1);
            hotkeyCount.Should().Be(1);
        }

        [Fact]
        public void Installed_factory_should_preserve_presentation_identity_and_compose_events()
        {
            DISetup.SetupContainer();
            var localization = Mock.Of<ILocalizationManager>();
            var presentation = new InstalledModsPresentationCoordinator(Mock.Of<IAppStateService>(),
                new SortOrderControlViewModel(localization), new SortOrderControlViewModel(localization),
                new SortOrderControlViewModel(localization), new SearchModsControlViewModel());
            var factory = new InstalledModsCoordinatorFactory(Mock.Of<IAppStateService>(),
                presentation.ModSelectedSortOrder, presentation.ModNameSortOrder, presentation.ModVersionSortOrder,
                presentation.FilterMods, new EvalModAchievementCompatibilityHandler());

            var coordinators = factory.Create();

            coordinators.Presentation.ModSelectedSortOrder.Should().BeSameAs(presentation.ModSelectedSortOrder);
            coordinators.Presentation.ModNameSortOrder.Should().BeSameAs(presentation.ModNameSortOrder);
            coordinators.Presentation.ModVersionSortOrder.Should().BeSameAs(presentation.ModVersionSortOrder);
            coordinators.Presentation.FilterMods.Should().BeSameAs(presentation.FilterMods);
            coordinators.Events.Should().BeOfType<InstalledModsEventCoordinator>();
            var secondScope = factory.Create();
            secondScope.Presentation.Should().NotBeSameAs(coordinators.Presentation);
            secondScope.Events.Should().NotBeSameAs(coordinators.Events);
        }

        [Fact]
        public async Task Installed_event_scope_should_release_achievement_subscription()
        {
            var achievementHandler = new EvalModAchievementCompatibilityHandler();
            var coordinator = new InstalledModsEventCoordinator(achievementHandler);
            var count = 0;
            using (coordinator.SubscribeAchievementChecks(_ => count++))
            {
                await achievementHandler.OnHandle(new EvalModAchievementsCompatibilityEvent([]));
            }

            await achievementHandler.OnHandle(new EvalModAchievementsCompatibilityEvent([]));
            count.Should().Be(1);
        }

        [Fact]
        public void Holder_factory_should_preserve_behavioral_collaborators_and_compose_events()
        {
            var localization = CreateLocalization();
            var factory = new ModHolderCoordinatorFactory(Mock.Of<IGameStateSafetyService>(),
                new ModDefinitionLoadHandler(), new ModDefinitionInvalidReplaceHandler(),
                new GameIndexProgressHandler(), new GameDefinitionLoadProgressHandler(),
                new ModDefinitionAnalyzeHandler(), new ModDefinitionPatchLoadHandler(), localization.Object,
                Mock.Of<INotificationAction>(), new GameUserDirectoryChangedHandler(),
                new ModListInstallRefreshRequestHandler());

            var coordinators = factory.Create();

            coordinators.Reconciliation.Should().BeOfType<ModStateReconciliationCoordinator>();
            coordinators.ConflictProgress.Should().BeOfType<ConflictAnalysisProgressCoordinator>();
            coordinators.ApplyResultPresenter.Should().BeOfType<ModApplyResultPresenter>();
            coordinators.Events.Should().BeOfType<ModHolderEventCoordinator>();
            var secondScope = factory.Create();
            secondScope.Reconciliation.Should().NotBeSameAs(coordinators.Reconciliation);
            secondScope.ConflictProgress.Should().NotBeSameAs(coordinators.ConflictProgress);
            secondScope.ApplyResultPresenter.Should().NotBeSameAs(coordinators.ApplyResultPresenter);
            secondScope.Events.Should().NotBeSameAs(coordinators.Events);
        }

        [Fact]
        public async Task Holder_event_scope_should_release_both_bus_subscriptions()
        {
            var directoryChanges = new GameUserDirectoryChangedHandler();
            var installRefresh = new ModListInstallRefreshRequestHandler();
            var coordinator = new ModHolderEventCoordinator(directoryChanges, installRefresh);
            var directoryCount = 0;
            var refreshCount = 0;
            using (var subscriptions = new CompositeDisposable
            {
                coordinator.SubscribeDirectoryChanges(_ => directoryCount++),
                coordinator.SubscribeInstallRefresh(_ => refreshCount++)
            })
            {
                await directoryChanges.OnHandle(new GameUserDirectoryChangedEvent(Mock.Of<IGame>(), false));
                await installRefresh.OnHandle(new ModListInstallRefreshRequestEvent(false));
            }

            await directoryChanges.OnHandle(new GameUserDirectoryChangedEvent(Mock.Of<IGame>(), false));
            await installRefresh.OnHandle(new ModListInstallRefreshRequestEvent(false));

            directoryCount.Should().Be(1);
            refreshCount.Should().Be(1);
        }

        [Fact]
        public void Holder_interaction_should_own_logging_and_user_presentation_of_save_failures()
        {
            var identifiers = new Mock<IIDGenerator>();
            var localization = CreateLocalization();
            var notifications = new Mock<INotificationAction>();
            var logger = new Mock<ILogger>();
            var interaction = new ModHolderInteraction(identifiers.Object, localization.Object,
                notifications.Object, Mock.Of<IAppAction>(), logger.Object);
            var exception = new System.InvalidOperationException("failure");

            interaction.ReportSavingFailure(exception);

            logger.Verify(p => p.Error(exception), Times.Once);
            notifications.Verify(p => p.ShowNotification(Shared.LocalizationResources.SavingError.Title,
                Shared.LocalizationResources.SavingError.Message, NotificationType.Error, 30, null), Times.Once);
        }

        [Fact]
        public async Task Conflict_progress_should_map_steps_and_reset_all_subscriptions()
        {
            DISetup.SetupContainer();
            var definitionLoad = new ModDefinitionLoadHandler();
            var invalidReplace = new ModDefinitionInvalidReplaceHandler();
            var gameIndex = new GameIndexProgressHandler();
            var gameDefinitions = new GameDefinitionLoadProgressHandler();
            var analyze = new ModDefinitionAnalyzeHandler();
            var patchLoad = new ModDefinitionPatchLoadHandler();
            var localization = CreateLocalization();
            var coordinator = new ConflictAnalysisProgressCoordinator(definitionLoad, invalidReplace, gameIndex, gameDefinitions, analyze, patchLoad, localization.Object);
            var messages = new List<string>();
            using var disposables = new CompositeDisposable();

            coordinator.Subscribe(7, disposables, 6, (_, _, message, _) => messages.Add(message));
            await definitionLoad.OnHandle(new ModDefinitionLoadEvent(0.25));
            await patchLoad.OnHandle(new ModDefinitionPatchLoadEvent(0.75));

            messages.Should().ContainInOrder(
                Shared.LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Loading_Definitions,
                Shared.LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Analyzing_Resolved_Conflicts);

            coordinator.Reset();
            await definitionLoad.OnHandle(new ModDefinitionLoadEvent(0.5));
            messages.Should().HaveCount(2);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(4)]
        public async Task Apply_result_should_show_exactly_one_warning_when_virtual_members_were_skipped(int skippedVirtualMods)
        {
            var localization = CreateLocalization();
            var notifications = new Mock<INotificationAction>();
            var presenter = new ModApplyResultPresenter(localization.Object, notifications.Object);

            await presenter.ShowAsync(new ModCollection { Name = "collection" },
                new ModApplyResult { Succeeded = true, SkippedVirtualMods = skippedVirtualMods });

            notifications.Verify(p => p.ShowNotification(It.IsAny<string>(), It.IsAny<string>(), NotificationType.Success, 5, null), Times.Once);
            notifications.Verify(p => p.ShowNotification(It.IsAny<string>(), It.IsAny<string>(), NotificationType.Warning, 10, null), Times.Once);
        }

        [Theory]
        [InlineData(true, 0, NotificationType.Success)]
        [InlineData(false, 3, NotificationType.Error)]
        public async Task Apply_result_should_not_warn_without_a_successful_virtual_skip(bool succeeded, int skippedVirtualMods, NotificationType expectedType)
        {
            var localization = CreateLocalization();
            var notifications = new Mock<INotificationAction>();
            var presenter = new ModApplyResultPresenter(localization.Object, notifications.Object);

            await presenter.ShowAsync(new ModCollection { Name = "collection" },
                new ModApplyResult { Succeeded = succeeded, SkippedVirtualMods = skippedVirtualMods });

            notifications.Verify(p => p.ShowNotification(It.IsAny<string>(), It.IsAny<string>(), expectedType, 5, null), Times.Once);
            notifications.Verify(p => p.ShowNotification(It.IsAny<string>(), It.IsAny<string>(), NotificationType.Warning, It.IsAny<int>(), null), Times.Never);
        }

        [Fact]
        public async Task Launch_result_should_wait_for_one_visible_warning_before_external_launch_can_continue()
        {
            var localization = CreateLocalization();
            var notifications = new Mock<INotificationAction>();
            var warningAcknowledged = new TaskCompletionSource<bool>();
            notifications.Setup(p => p.ShowPromptAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    NotificationType.Warning, PromptType.OK))
                .Returns(warningAcknowledged.Task);
            var presenter = new ModApplyResultPresenter(localization.Object, notifications.Object);

            var presentation = presenter.ShowAsync(new ModCollection { Name = "collection" },
                new ModApplyResult { Succeeded = true, SkippedVirtualMods = 3 }, true);

            presentation.IsCompleted.Should().BeFalse();
            notifications.Verify(p => p.ShowPromptAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                NotificationType.Warning, PromptType.OK), Times.Once);
            notifications.Verify(p => p.ShowNotification(It.IsAny<string>(), It.IsAny<string>(), NotificationType.Warning,
                It.IsAny<int>(), null), Times.Never);

            warningAcknowledged.SetResult(true);
            await presentation;
        }

        private static Mock<ILocalizationManager> CreateLocalization()
        {
            var localization = new Mock<ILocalizationManager>();
            localization.Setup(p => p.GetResource(It.IsAny<string>())).Returns((string key) => key);
            return localization;
        }
    }
}
