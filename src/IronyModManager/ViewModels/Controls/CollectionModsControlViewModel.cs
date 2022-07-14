// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-03-2020
//
// Last Modified By : Mario
// Last Modified On : 07-14-2022
// ***********************************************************************
// <copyright file="CollectionModsControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using DynamicData;
using IronyModManager.Common;
using IronyModManager.Common.Events;
using IronyModManager.Common.ViewModels;
using IronyModManager.DI;
using IronyModManager.Implementation;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.Hotkey;
using IronyModManager.Implementation.MessageBus;
using IronyModManager.Implementation.MessageBus.Events;
using IronyModManager.Implementation.Overlay;
using IronyModManager.Localization;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using Nito.AsyncEx;
using ReactiveUI;
using static IronyModManager.ViewModels.Controls.ModifyCollectionControlViewModel;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class CollectionModsControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class CollectionModsControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The mod name key
        /// </summary>
        private const string ModNameKey = "modName";

        /// <summary>
        /// The application action
        /// </summary>
        private readonly IAppAction appAction;

        /// <summary>
        /// The application state service
        /// </summary>
        private readonly IAppStateService appStateService;

        /// <summary>
        /// The file dialog action
        /// </summary>
        private readonly IFileDialogAction fileDialogAction;

        /// <summary>
        /// The game service
        /// </summary>
        private readonly IGameService gameService;

        /// <summary>
        /// The hotkey pressed handler
        /// </summary>
        private readonly MainViewHotkeyPressedHandler hotkeyPressedHandler;

        /// <summary>
        /// The identifier generator
        /// </summary>
        private readonly IIDGenerator idGenerator;

        /// <summary>
        /// The localization manager
        /// </summary>
        private readonly ILocalizationManager localizationManager;

        /// <summary>
        /// The mod collection service
        /// </summary>
        private readonly IModCollectionService modCollectionService;

        /// <summary>
        /// The mod export progress handler
        /// </summary>
        private readonly ModExportProgressHandler modExportProgressHandler;

        /// <summary>
        /// The mod patch collection service
        /// </summary>
        private readonly IModPatchCollectionService modPatchCollectionService;

        /// <summary>
        /// The mod report export handler
        /// </summary>
        private readonly ModReportExportHandler modReportExportHandler;

        /// <summary>
        /// The mod service
        /// </summary>
        private readonly IModService modService;

        /// <summary>
        /// The notification action
        /// </summary>
        private readonly INotificationAction notificationAction;

        /// <summary>
        /// The previous validated mods
        /// </summary>
        private readonly ConcurrentDictionary<string, IEnumerable<IMod>> previousValidatedMods = new();

        /// <summary>
        /// The reorder lock
        /// </summary>
        private readonly AsyncLock reorderLock = new();

        /// <summary>
        /// The reorder queue
        /// </summary>
        private readonly ConcurrentBag<IMod> reorderQueue;

        /// <summary>
        /// The report export service
        /// </summary>
        private readonly IReportExportService reportExportService;

        /// <summary>
        /// The active game
        /// </summary>
        private IGame activeGame;

        /// <summary>
        /// The enable all toggled state
        /// </summary>
        private bool enableAllToggledState = false;

        /// <summary>
        /// The mod export progress
        /// </summary>
        private IDisposable modExportProgress;

        /// <summary>
        /// The mod order changed
        /// </summary>
        private IDisposable modOrderChanged;

        /// <summary>
        /// The mod selected changed
        /// </summary>
        private IDisposable modSelectedChanged;

        /// <summary>
        /// The refresh in progress
        /// </summary>
        private bool refreshInProgress = false;

        /// <summary>
        /// The reorder token
        /// </summary>
        private CancellationTokenSource reorderToken;

        /// <summary>
        /// The restore collection selection
        /// </summary>
        private string restoreCollectionSelection = string.Empty;

        /// <summary>
        /// The skip mod collection save
        /// </summary>
        private bool skipModCollectionSave = false;

        /// <summary>
        /// The skip mod selection save
        /// </summary>
        private bool skipModSelectionSave = false;

        /// <summary>
        /// The skip reorder
        /// </summary>
        private bool skipReorder = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionModsControlViewModel" /> class.
        /// </summary>
        /// <param name="modExportProgressHandler">The mod export progress handler.</param>
        /// <param name="reportExportService">The report export service.</param>
        /// <param name="hotkeyPressedHandler">The hotkey pressed handler.</param>
        /// <param name="patchMod">The patch mod.</param>
        /// <param name="idGenerator">The identifier generator.</param>
        /// <param name="hashReportView">The hash report view.</param>
        /// <param name="modReportExportHandler">The mod report export handler.</param>
        /// <param name="fileDialogAction">The file dialog action.</param>
        /// <param name="modCollectionService">The mod collection service.</param>
        /// <param name="appStateService">The application state service.</param>
        /// <param name="modPatchCollectionService">The mod patch collection service.</param>
        /// <param name="modService">The mod service.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="addNewCollection">The add new collection.</param>
        /// <param name="exportCollection">The export collection.</param>
        /// <param name="modifyCollection">The modify collection.</param>
        /// <param name="searchMods">The search mods.</param>
        /// <param name="modNameSort">The mod name sort.</param>
        /// <param name="localizationManager">The localization manager.</param>
        /// <param name="notificationAction">The notification action.</param>
        /// <param name="appAction">The application action.</param>
        public CollectionModsControlViewModel(ModExportProgressHandler modExportProgressHandler, IReportExportService reportExportService,
            MainViewHotkeyPressedHandler hotkeyPressedHandler, PatchModControlViewModel patchMod,
            IIDGenerator idGenerator, HashReportControlViewModel hashReportView, ModReportExportHandler modReportExportHandler,
            IFileDialogAction fileDialogAction, IModCollectionService modCollectionService,
            IAppStateService appStateService, IModPatchCollectionService modPatchCollectionService, IModService modService, IGameService gameService,
            AddNewCollectionControlViewModel addNewCollection, ExportModCollectionControlViewModel exportCollection, ModifyCollectionControlViewModel modifyCollection,
            SearchModsControlViewModel searchMods, SortOrderControlViewModel modNameSort, ILocalizationManager localizationManager,
            INotificationAction notificationAction, IAppAction appAction)
        {
            this.reportExportService = reportExportService;
            PatchMod = patchMod;
            this.idGenerator = idGenerator;
            this.modCollectionService = modCollectionService;
            this.appStateService = appStateService;
            AddNewCollection = addNewCollection;
            ExportCollection = exportCollection;
            SearchMods = searchMods;
            ModNameSortOrder = modNameSort;
            ModifyCollection = modifyCollection;
            this.localizationManager = localizationManager;
            this.notificationAction = notificationAction;
            this.appAction = appAction;
            this.modService = modService;
            this.gameService = gameService;
            this.modPatchCollectionService = modPatchCollectionService;
            this.fileDialogAction = fileDialogAction;
            this.modReportExportHandler = modReportExportHandler;
            this.hotkeyPressedHandler = hotkeyPressedHandler;
            this.modExportProgressHandler = modExportProgressHandler;
            HashReportView = hashReportView;
            SearchMods.ShowArrows = true;
            reorderQueue = new ConcurrentBag<IMod>();
        }

        #endregion Constructors

        #region Delegates

        /// <summary>
        /// Delegate ConflictSolverStateChangedDelegate
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="isValid">if set to <c>true</c> [is valid].</param>
        public delegate void ConflictSolverStateChangedDelegate(string collectionName, bool isValid);

        /// <summary>
        /// Delegate ModReorderedDelegate
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <param name="instant">if set to <c>true</c> [instant].</param>
        public delegate void ModReorderedDelegate(IMod mod, bool instant);

        #endregion Delegates

        #region Events

        /// <summary>
        /// Occurs when [conflict solver state changed].
        /// </summary>
        public event ConflictSolverStateChangedDelegate ConflictSolverStateChanged;

        /// <summary>
        /// Occurs when [mod reordered].
        /// </summary>
        public event ModReorderedDelegate ModReordered;

        #endregion Events

        #region Enums

        /// <summary>
        /// Enum ImportActionType
        /// </summary>
        public enum ImportActionType
        {
            /// <summary>
            /// The import
            /// </summary>
            Import,

            /// <summary>
            /// The export
            /// </summary>
            Export
        }

        /// <summary>
        /// Enum ImportProviderType
        /// </summary>
        public enum ImportProviderType
        {
            /// <summary>
            /// The default
            /// </summary>
            Default,

            /// <summary>
            /// The default order only
            /// </summary>
            DefaultOrderOnly,

            /// <summary>
            /// The default with all mods
            /// </summary>
            DefaultWithAllMods,

            /// <summary>
            /// The paradoxos
            /// </summary>
            Paradoxos,

            /// <summary>
            /// The paradox
            /// </summary>
            Paradox,

            /// <summary>
            /// The paradox launcher
            /// </summary>
            ParadoxLauncher,

            /// <summary>
            /// The paradox launcher beta
            /// </summary>
            ParadoxLauncherBeta,

            /// <summary>
            /// The paradox launcher json
            /// </summary>
            ParadoxLauncherJson,

            /// <summary>
            /// The paradox launcher json202110
            /// </summary>
            ParadoxLauncherJson202110
        }

        #endregion Enums

        #region Properties

        /// <summary>
        /// Gets or sets the achievement compatible.
        /// </summary>
        /// <value>The achievement compatible.</value>
        [StaticLocalization(LocalizationResources.Achievements.AchievementCompatible)]
        public virtual string AchievementCompatible { get; protected set; }

        /// <summary>
        /// Gets or sets the add new collection.
        /// </summary>
        /// <value>The add new collection.</value>
        public virtual AddNewCollectionControlViewModel AddNewCollection { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [all mods enabled].
        /// </summary>
        /// <value><c>true</c> if [all mods enabled]; otherwise, <c>false</c>.</value>
        public virtual bool AllModsEnabled { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow mod selection].
        /// </summary>
        /// <value><c>true</c> if [allow mod selection]; otherwise, <c>false</c>.</value>
        public virtual bool AllowModSelection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can export mod hash report.
        /// </summary>
        /// <value><c>true</c> if this instance can export mod hash report; otherwise, <c>false</c>.</value>
        public virtual bool CanExportModHashReport { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [collection jump on position change].
        /// </summary>
        /// <value><c>true</c> if [collection jump on position change]; otherwise, <c>false</c>.</value>
        public virtual bool CollectionJumpOnPositionChange { get; protected set; }

        /// <summary>
        /// Gets or sets the collection jump on position change command.
        /// </summary>
        /// <value>The collection jump on position change command.</value>
        public virtual ReactiveCommand<Unit, Unit> CollectionJumpOnPositionChangeCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the collection jump on position change label.
        /// </summary>
        /// <value>The collection jump on position change label.</value>
        public virtual string CollectionJumpOnPositionChangeLabel { get; protected set; }

        /// <summary>
        /// Gets or sets the context menu mod.
        /// </summary>
        /// <value>The context menu mod.</value>
        public virtual IMod ContextMenuMod { get; set; }

        /// <summary>
        /// Gets or sets the copy URL.
        /// </summary>
        /// <value>The copy URL.</value>
        [StaticLocalization(LocalizationResources.Mod_App_Actions.Copy)]
        public virtual string CopyUrl { get; protected set; }

        /// <summary>
        /// Gets or sets the copy URL command.
        /// </summary>
        /// <value>The copy URL command.</value>
        public virtual ReactiveCommand<Unit, Unit> CopyUrlCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the create.
        /// </summary>
        /// <value>The create.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Create)]
        public virtual string Create { get; protected set; }

        /// <summary>
        /// Gets or sets the create command.
        /// </summary>
        /// <value>The create command.</value>
        public virtual ReactiveCommand<Unit, Unit> CreateCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the enable all command.
        /// </summary>
        /// <value>The enable all command.</value>
        public virtual ReactiveCommand<Unit, Unit> EnableAllCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [entering new collection].
        /// </summary>
        /// <value><c>true</c> if [entering new collection]; otherwise, <c>false</c>.</value>
        public virtual bool EnteringNewCollection { get; protected set; }

        /// <summary>
        /// Gets or sets the export collection.
        /// </summary>
        /// <value>The export collection.</value>
        public virtual ExportModCollectionControlViewModel ExportCollection { get; protected set; }

        /// <summary>
        /// Gets or sets the export collection report.
        /// </summary>
        /// <value>The export collection report.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.FileHash.ExportCollection)]
        public virtual string ExportCollectionReport { get; protected set; }

        /// <summary>
        /// Gets or sets the export collection report command.
        /// </summary>
        /// <value>The export collection report command.</value>
        public virtual ReactiveCommand<Unit, Unit> ExportCollectionReportCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the export collection to clipboard.
        /// </summary>
        /// <value>The export collection to clipboard.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.ExportToClipboard)]
        public virtual string ExportCollectionToClipboard { get; protected set; }

        /// <summary>
        /// Gets or sets the export collection to clipboard command.
        /// </summary>
        /// <value>The export collection to clipboard command.</value>
        public virtual ReactiveCommand<Unit, Unit> ExportCollectionToClipboardCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the export game report.
        /// </summary>
        /// <value>The export game report.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.FileHash.ExportGame)]
        public virtual string ExportGameReport { get; protected set; }

        /// <summary>
        /// Gets or sets the export game report command.
        /// </summary>
        /// <value>The export game report command.</value>
        public virtual ReactiveCommand<Unit, Unit> ExportGameReportCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the hash report view.
        /// </summary>
        /// <value>The hash report view.</value>
        public virtual HashReportControlViewModel HashReportView { get; protected set; }

        /// <summary>
        /// Gets or sets the hovered mod.
        /// </summary>
        /// <value>The hovered mod.</value>
        public virtual IMod HoveredMod { get; set; }

        /// <summary>
        /// Gets or sets the import collection from clipboard.
        /// </summary>
        /// <value>The import collection from clipboard.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.ImportFromClipboard.Title)]
        public virtual string ImportCollectionFromClipboard { get; protected set; }

        /// <summary>
        /// Gets or sets the import collection from clipboard command.
        /// </summary>
        /// <value>The import collection from clipboard command.</value>
        public virtual ReactiveCommand<Unit, Unit> ImportCollectionFromClipboardCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the import report.
        /// </summary>
        /// <value>The import report.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.FileHash.Import)]
        public virtual string ImportReport { get; protected set; }

        /// <summary>
        /// Gets or sets the import report command.
        /// </summary>
        /// <value>The import report command.</value>
        public virtual ReactiveCommand<Unit, Unit> ImportReportCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the local mod tooltip.
        /// </summary>
        /// <value>The local mod tooltip.</value>
        [StaticLocalization(LocalizationResources.ModSource.Local)]
        public virtual string LocalModTooltip { get; protected set; }

        /// <summary>
        /// Gets or sets the maximum order.
        /// </summary>
        /// <value>The maximum order.</value>
        public virtual int MaxOrder { get; protected set; }

        /// <summary>
        /// Gets or sets the mod collections.
        /// </summary>
        /// <value>The mod collections.</value>
        public virtual IEnumerable<IModCollection> ModCollections { get; protected set; }

        /// <summary>
        /// Gets or sets the modify collection.
        /// </summary>
        /// <value>The modify collection.</value>
        public virtual ModifyCollectionControlViewModel ModifyCollection { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the mod.
        /// </summary>
        /// <value>The name of the mod.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Mod_Name)]
        public virtual string ModName { get; protected set; }

        /// <summary>
        /// Gets or sets the mod name sort order.
        /// </summary>
        /// <value>The mod name sort order.</value>
        public virtual SortOrderControlViewModel ModNameSortOrder { get; protected set; }

        /// <summary>
        /// Gets or sets the mod order.
        /// </summary>
        /// <value>The mod order.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Order)]
        public virtual string ModOrder { get; protected set; }

        /// <summary>
        /// Gets or sets the mods.
        /// </summary>
        /// <value>The mods.</value>
        public virtual IEnumerable<IMod> Mods { get; protected set; }

        /// <summary>
        /// Gets or sets the mod selected.
        /// </summary>
        /// <value>The mod selected.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Selected)]
        public virtual string ModSelected { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [needs mod list refresh].
        /// </summary>
        /// <value><c>true</c> if [needs mod list refresh]; otherwise, <c>false</c>.</value>
        public virtual bool NeedsModListRefresh { get; protected set; }

        /// <summary>
        /// Gets or sets the not achievement compatible.
        /// </summary>
        /// <value>The not achievement compatible.</value>
        [StaticLocalization(LocalizationResources.Achievements.NotAchievementCompatible)]
        public virtual string NotAchievementCompatible { get; protected set; }

        /// <summary>
        /// Gets or sets the open in associated application.
        /// </summary>
        /// <value>The open in associated application.</value>
        [StaticLocalization(LocalizationResources.Mod_App_Actions.OpenInAssociatedApp)]
        public virtual string OpenInAssociatedApp { get; protected set; }

        /// <summary>
        /// Gets or sets the open in associated application command.
        /// </summary>
        /// <value>The open in associated application command.</value>
        public virtual ReactiveCommand<Unit, Unit> OpenInAssociatedAppCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the open in steam.
        /// </summary>
        /// <value>The open in steam.</value>
        [StaticLocalization(LocalizationResources.Mod_App_Actions.OpenInSteam)]
        public virtual string OpenInSteam { get; protected set; }

        /// <summary>
        /// Gets or sets the open in steam command.
        /// </summary>
        /// <value>The open in steam command.</value>
        public virtual ReactiveCommand<Unit, Unit> OpenInSteamCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the open URL.
        /// </summary>
        /// <value>The open URL.</value>
        [StaticLocalization(LocalizationResources.Mod_App_Actions.Open)]
        public virtual string OpenUrl { get; protected set; }

        /// <summary>
        /// Gets or sets the open URL command.
        /// </summary>
        /// <value>The open URL command.</value>
        public virtual ReactiveCommand<Unit, Unit> OpenUrlCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the paradox mod tooltip.
        /// </summary>
        /// <value>The paradox mod tooltip.</value>
        [StaticLocalization(LocalizationResources.ModSource.Paradox)]
        public virtual string ParadoxModTooltip { get; protected set; }

        /// <summary>
        /// Gets or sets the patch mod.
        /// </summary>
        /// <value>The patch mod.</value>
        public virtual PatchModControlViewModel PatchMod { get; protected set; }

        /// <summary>
        /// Gets or sets the remove.
        /// </summary>
        /// <value>The remove.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Remove)]
        public virtual string Remove { get; protected set; }

        /// <summary>
        /// Gets or sets the remove command.
        /// </summary>
        /// <value>The remove command.</value>
        public virtual ReactiveCommand<Unit, Unit> RemoveCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the search mods.
        /// </summary>
        /// <value>The search mods.</value>
        public virtual SearchModsControlViewModel SearchMods { get; protected set; }

        /// <summary>
        /// Gets or sets the search mods col span.
        /// </summary>
        /// <value>The search mods col span.</value>
        public virtual int SearchModsColSpan { get; protected set; }

        /// <summary>
        /// Gets or sets the search mods watermark.
        /// </summary>
        /// <value>The search mods watermark.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Filter)]
        public virtual string SearchModsWatermark { get; protected set; }

        /// <summary>
        /// Gets or sets the selected mod.
        /// </summary>
        /// <value>The selected mod.</value>
        public virtual IMod SelectedMod { get; protected set; }

        /// <summary>
        /// Gets or sets the selected mod collection.
        /// </summary>
        /// <value>The selected mod collection.</value>
        public virtual IModCollection SelectedModCollection { get; protected set; }

        /// <summary>
        /// Gets or sets the selected mods.
        /// </summary>
        /// <value>The selected mods.</value>
        public virtual IList<IMod> SelectedMods { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show advanced features].
        /// </summary>
        /// <value><c>true</c> if [show advanced features]; otherwise, <c>false</c>.</value>
        public virtual bool ShowAdvancedFeatures { get; protected set; }

        /// <summary>
        /// Gets or sets the steam mod tooltip.
        /// </summary>
        /// <value>The steam mod tooltip.</value>
        [StaticLocalization(LocalizationResources.ModSource.Steam)]
        public virtual string SteamModTooltip { get; protected set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Name)]
        public virtual string Title { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance [can export game].
        /// </summary>
        /// <returns>bool.</returns>
        public virtual bool CanExportGame()
        {
            return activeGame != null && !string.IsNullOrWhiteSpace(activeGame.ExecutableLocation) && System.IO.File.Exists(activeGame.ExecutableLocation);
        }

        /// <summary>
        /// Gets the context menu mod steam URL.
        /// </summary>
        /// <returns>System.String.</returns>
        public virtual string GetContextMenuModSteamUrl()
        {
            if (ContextMenuMod != null)
            {
                var url = modService.BuildSteamUrl(ContextMenuMod);
                return url;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the context menu mod URL.
        /// </summary>
        /// <returns>System.String.</returns>
        public virtual string GetContextMenuModUrl()
        {
            if (ContextMenuMod != null)
            {
                var url = modService.BuildModUrl(ContextMenuMod);
                return url;
            }
            return string.Empty;
        }

        /// <summary>
        /// Handles the enable all toggled.
        /// </summary>
        /// <param name="toggledState">if set to <c>true</c> [toggled state].</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <param name="enabledMods">The enabled mods.</param>
        public virtual void HandleEnableAllToggled(bool toggledState, bool enabled, IEnumerable<IMod> enabledMods)
        {
            if (toggledState)
            {
                skipModCollectionSave = true;
            }
            if (!toggledState && enableAllToggledState)
            {
                skipModCollectionSave = false;

                var mods = new List<IMod>(SelectedMods);
                if (enabledMods != null)
                {
                    foreach (var item in enabledMods)
                    {
                        if (enabled)
                        {
                            if (!mods.Contains(item))
                            {
                                mods.Add(item);
                            }
                        }
                        else
                        {
                            mods.Remove(item);
                        }
                    }
                }

                SetSelectedMods(mods.ToObservableCollection());

                AllModsEnabled = SelectedMods?.Count > 0 && SelectedMods.All(p => p.IsSelected);
                var state = appStateService.Get();
                InitSortersAndFilters(state);
                SaveSelectedCollection();
                RecognizeSortOrder(SelectedModCollection);
            }
            enableAllToggledState = toggledState;
        }

        /// <summary>
        /// Handles the mod refresh.
        /// </summary>
        /// <param name="isRefreshing">if set to <c>true</c> [is refreshing].</param>
        /// <param name="mods">The mods.</param>
        /// <param name="activeGame">The active game.</param>
        public virtual void HandleModRefresh(bool isRefreshing, IEnumerable<IMod> mods, IGame activeGame)
        {
            if (isRefreshing)
            {
                refreshInProgress = true;
            }
            if (!isRefreshing && mods?.Count() > 0)
            {
                SetMods(mods, activeGame);
                refreshInProgress = false;
            }
        }

        /// <summary>
        /// Instants the reorder selected items.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <param name="newOrder">The new order.</param>
        public virtual void InstantReorderSelectedItems(IMod mod, int newOrder)
        {
            async Task reorder()
            {
                if (reorderToken != null)
                {
                    reorderToken.Cancel();
                }
                reorderToken = new CancellationTokenSource();
                mod.Order = newOrder;
                if (!reorderQueue.Contains(mod))
                {
                    reorderQueue.Add(mod);
                }
                await PerformModReorderAsync(true, reorderToken.Token);
            }
            reorder().ConfigureAwait(false);
        }

        /// <summary>
        /// Called when [locale changed].
        /// </summary>
        /// <param name="newLocale">The new locale.</param>
        /// <param name="oldLocale">The old locale.</param>
        public override void OnLocaleChanged(string newLocale, string oldLocale)
        {
            SetAutoFocusLabel();
            SearchMods.WatermarkText = SearchModsWatermark;
            ModNameSortOrder.Text = ModName;
            base.OnLocaleChanged(newLocale, oldLocale);
        }

        /// <summary>
        /// Reloads the mod collection.
        /// </summary>
        public virtual void ReloadModCollection()
        {
            LoadModCollections();
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        /// <param name="fullReset">The full reset.</param>
        public virtual void Reset(bool fullReset = false)
        {
            if (fullReset)
            {
                previousValidatedMods.Clear();
            }
            PatchMod.SetParameters(SelectedModCollection);
            HandleCollectionPatchStateAsync(SelectedModCollection?.Name).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets the mods.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <param name="activeGame">The active game.</param>
        public virtual void SetMods(IEnumerable<IMod> mods, IGame activeGame)
        {
            this.activeGame = activeGame;
            Mods = mods;

            skipModCollectionSave = true;
            SubscribeToMods();
            HandleModCollectionChange();
            skipModCollectionSave = false;
        }

        /// <summary>
        /// Applies the sort.
        /// </summary>
        protected virtual void ApplySort()
        {
            switch (ModNameSortOrder.SortOrder)
            {
                case SortOrder.Asc:
                    SelectedMod = null;
                    SetSelectedMods(SelectedMods.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase).ToObservableCollection());
                    break;

                case SortOrder.Desc:
                    SelectedMod = null;
                    SetSelectedMods(SelectedMods.OrderByDescending(x => x.Name, StringComparer.OrdinalIgnoreCase).ToObservableCollection());
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Assigns the optional collection metadata.
        /// </summary>
        /// <param name="collection">The collection.</param>
        protected virtual void AssignOptionalCollectionMetadata(IModCollection collection)
        {
            collection.ModNames = SelectedMods?.Where(p => p.IsSelected).Select(p => p.Name).ToList();
            collection.ModIds = SelectedMods?.Where(p => p.IsSelected).Select(p =>
            {
                var result = DIResolver.Get<IModCollectionSourceInfo>();
                switch (p.Source)
                {
                    case ModSource.Steam:
                        result.SteamId = p.RemoteId;
                        break;

                    case ModSource.Paradox:
                        result.ParadoxId = p.RemoteId;
                        break;

                    default:
                        break;
                }
                return result;
            }).ToList();
        }

        /// <summary>
        /// Evals the advanced features visibility.
        /// </summary>
        protected virtual void EvalAdvancedFeaturesVisibility()
        {
            var game = gameService.GetSelected();
            ShowAdvancedFeatures = (game?.AdvancedFeatures) == GameAdvancedFeatures.Full;
            SearchModsColSpan = ShowAdvancedFeatures ? 1 : 2;
        }

        /// <summary>
        /// export collection as an asynchronous operation.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="providerType">Type of the provider.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        protected virtual async Task ExportCollectionAsync(string path, ImportProviderType providerType)
        {
            var id = idGenerator.GetNextId();
            var overlayProgress = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.Overlay_Import_Export_Progress), new
            {
                PercentDone = 0.ToLocalizedPercentage()
            });
            await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.Collection_Mods.Overlay_Exporting_Message), overlayProgress);
            var collection = modCollectionService.Get(SelectedModCollection.Name);
            if (providerType == ImportProviderType.ParadoxLauncherJson)
            {
                await Task.Run(async () => await modCollectionService.ExportParadoxLauncherJsonAsync(path, collection).ConfigureAwait(false)).ConfigureAwait(false);
            }
            else if (providerType == ImportProviderType.ParadoxLauncherJson202110)
            {
                await Task.Run(async () => await modCollectionService.ExportParadoxLauncher202110JsonAsync(path, collection).ConfigureAwait(false)).ConfigureAwait(false);
            }
            else
            {
                modExportProgress?.Dispose();
                modExportProgress = modExportProgressHandler.Subscribe(s =>
                {
                    var overlayProgress = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.Overlay_Import_Export_Progress), new
                    {
                        PercentDone = s.Progress.ToLocalizedPercentage()
                    });
                    TriggerOverlay(id, true, localizationManager.GetResource(LocalizationResources.Collection_Mods.Overlay_Exporting_Message), overlayProgress);
                }).DisposeWith(Disposables);
                await Task.Run(async () => await modCollectionService.ExportAsync(path, collection, providerType == ImportProviderType.DefaultOrderOnly, providerType == ImportProviderType.DefaultWithAllMods).ConfigureAwait(false)).ConfigureAwait(false);
                modExportProgress?.Dispose();
            }
            var title = localizationManager.GetResource(LocalizationResources.Notifications.CollectionExported.Title);
            var message = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionExported.Message), new { CollectionName = collection.Name });
            notificationAction.ShowNotification(title, message, NotificationType.Success);
            await TriggerOverlayAsync(id, false);
        }

        /// <summary>
        /// handle collection patch as an asynchronous operation.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        protected virtual async Task HandleCollectionPatchStateAsync(string collection)
        {
            var currentCollection = SelectedModCollection?.Name ?? string.Empty;
            if (activeGame != null && SelectedMods?.Count > 0)
            {
                ConflictSolverStateChanged?.Invoke(collection, true);
                if (!string.IsNullOrWhiteSpace(collection) && currentCollection.Equals(collection, StringComparison.OrdinalIgnoreCase) && SelectedModCollection.Game.Equals(activeGame.Type))
                {
                    var result = await Task.Run(async () => await modPatchCollectionService.PatchModNeedsUpdateAsync(collection, SelectedMods.Select(p => p.DescriptorFile).ToList()));
                    ConflictSolverStateChanged?.Invoke(collection, !result);
                }
            }
            else
            {
                ConflictSolverStateChanged?.Invoke(collection, true);
            }
        }

        /// <summary>
        /// Handles the mod collection change.
        /// </summary>
        protected virtual void HandleModCollectionChange()
        {
            if (!string.IsNullOrWhiteSpace(restoreCollectionSelection))
            {
                if (ModCollections?.Count() > 0)
                {
                    var restored = ModCollections.FirstOrDefault(p => p.Name.Equals(restoreCollectionSelection));
                    if (restored != null)
                    {
                        SelectedModCollection = restored;
                    }
                }
                restoreCollectionSelection = string.Empty;
            }
            skipModCollectionSave = true;
            skipModSelectionSave = true;
            ExportCollection.CollectionName = SelectedModCollection?.Name;
            SaveState(true);
            if (Mods != null)
            {
                foreach (var item in Mods)
                {
                    item.IsSelected = false;
                }
            }
            var existingCollection = modCollectionService.Get(SelectedModCollection?.Name ?? string.Empty);
            var selectedMods = new ObservableCollection<IMod>();
            if (existingCollection?.Mods?.Count() > 0 && Mods != null)
            {
                var missingMods = new List<string>();
                var hasModNames = existingCollection.ModNames != null && existingCollection.ModNames.Count() == existingCollection.Mods.Count();
                var index = -1;
                foreach (var item in existingCollection.Mods)
                {
                    index++;
                    var mod = Mods.FirstOrDefault(p => p.DescriptorFile.Equals(item, StringComparison.InvariantCultureIgnoreCase));
                    if (mod != null)
                    {
                        mod.IsSelected = true;
                        selectedMods.Add(mod);
                    }
                    else
                    {
                        if (hasModNames)
                        {
                            missingMods.Add($"{existingCollection.ModNames.ElementAt(index)} ({item})");
                        }
                        else
                        {
                            missingMods.Add(item);
                        }
                    }
                }
                if (missingMods.Any())
                {
                    var title = localizationManager.GetResource(LocalizationResources.Collection_Mods.Prompts.ModsMissingTitle);
                    var message = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.Prompts.ModsMissingMessage), new { Environment.NewLine, Mods = string.Join(Environment.NewLine, missingMods) });
                    Dispatcher.UIThread.SafeInvoke(() => notificationAction.ShowPromptAsync(title, title, message, NotificationType.Warning, PromptType.OK));
                }
            }
            SetSelectedMods(selectedMods);
            AllModsEnabled = SelectedMods?.Count > 0 && SelectedMods.All(p => p.IsSelected);
            var state = appStateService.Get();
            InitSortersAndFilters(state, false);
            ApplySort();
            SaveSelectedCollection();
            skipModCollectionSave = false;
            skipModSelectionSave = false;
        }

        /// <summary>
        /// import collection as an asynchronous operation.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        protected virtual async Task ImportCollectionAsync(string path, ImportProviderType type)
        {
            List<string> modNames = null;
            async Task<IModCollection> importDefault(long messageId)
            {
                modExportProgress?.Dispose();
                modExportProgress = modExportProgressHandler.Subscribe(s =>
                {
                    var overlayProgress = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.Overlay_Import_Export_Progress), new
                    {
                        PercentDone = s.Progress.ToLocalizedPercentage()
                    });
                    TriggerOverlay(messageId, true, localizationManager.GetResource(LocalizationResources.Collection_Mods.Overlay_Importing_Message), overlayProgress);
                }).DisposeWith(Disposables);
                var collection = await Task.Run(async () => await modCollectionService.ImportAsync(path));
                modExportProgress?.Dispose();
                if (collection != null)
                {
                    collection.IsSelected = true;
                    modNames = collection.ModNames.ToList();
                    if (modCollectionService.Save(collection))
                    {
                        return collection;
                    }
                }
                return null;
            }

            Task<IModCollection> importInstance(IModCollection importData)
            {
                if (importData != null)
                {
                    importData.IsSelected = true;
                    modNames = importData.ModNames != null ? importData.ModNames.ToList() : new List<string>();
                    if (modCollectionService.Save(importData))
                    {
                        return Task.FromResult(importData);
                    }
                }
                return Task.FromResult((IModCollection)null);
            }

            var id = idGenerator.GetNextId();
            var overlayProgress = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.Overlay_Import_Export_Progress), new
            {
                PercentDone = 0.ToLocalizedPercentage()
            });
            await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.Collection_Mods.Overlay_Importing_Message), overlayProgress);
            var importData = type switch
            {
                ImportProviderType.Paradoxos => await modCollectionService.ImportParadoxosAsync(path),
                ImportProviderType.Paradox => await modCollectionService.ImportParadoxAsync(),
                ImportProviderType.ParadoxLauncher => await modCollectionService.ImportParadoxLauncherAsync(),
                ImportProviderType.ParadoxLauncherBeta => await modCollectionService.ImportParadoxLauncherBetaAsync(),
                ImportProviderType.ParadoxLauncherJson => await modCollectionService.ImportParadoxLauncherJsonAsync(path),
                _ => await modCollectionService.GetImportedCollectionDetailsAsync(path),
            };
            if (importData == null)
            {
                await TriggerOverlayAsync(id, false);
                return;
            }
            bool proceed;
            if (modCollectionService.Exists(importData.Name))
            {
                var title = localizationManager.GetResource(LocalizationResources.Collection_Mods.ImportPrompt.Title);
                var message = localizationManager.GetResource(LocalizationResources.Collection_Mods.ImportPrompt.Message);
                proceed = await notificationAction.ShowPromptAsync(title, title, message, NotificationType.Warning);
            }
            else
            {
                proceed = true;
            }
            var endOverlay = true;
            if (proceed)
            {
                var result = type switch
                {
                    ImportProviderType.Default => await importDefault(id),
                    _ => await importInstance(importData),
                };
                if (result != null)
                {
                    var game = gameService.Get().FirstOrDefault(p => p.Type.Equals(result.Game, StringComparison.OrdinalIgnoreCase));
                    await MessageBus.PublishAsync(new ActiveGameRequestEvent(game));
                    await MessageBus.PublishAsync(new ModListInstallRefreshRequestEvent(true));
                    var hasMods = (modNames?.Any()).GetValueOrDefault();
                    var mods = game != null ? await modService.GetAvailableModsAsync(game) ?? new List<IMod>() : new List<IMod>();
                    if (hasMods && !string.IsNullOrWhiteSpace(result.MergedFolderName))
                    {
                        var importedMods = new List<string>();
                        var descriptors = result.Mods.ToList();
                        for (int i = 0; i < descriptors.Count; i++)
                        {
                            var descriptor = descriptors[i];
                            var name = modNames[i];
                            var mod = mods.FirstOrDefault(p => p.Name.Equals(name) && System.IO.Path.GetDirectoryName(p.FullPath).EndsWith(System.IO.Path.DirectorySeparatorChar + result.MergedFolderName));
                            importedMods.Add(mod == null ? descriptor : mod.DescriptorFile);
                        }
                        result.Mods = importedMods;
                        modCollectionService.Save(result);
                    }
                    var modPaths = result.Mods != null ? result.Mods.ToList() : new List<string>();
                    restoreCollectionSelection = result.Name;
                    LoadModCollections();
                    var showImportNotification = true;
                    // Check if any mods do not exist
                    if (hasMods && mods.Any())
                    {
                        var nonExistingModPaths = modPaths.Where(p => !mods.Any(m => m.DescriptorFile.Equals(p)));
                        if (nonExistingModPaths.Any())
                        {
                            var nonExistingModNames = new List<string>();
                            var hasModNames = modNames != null && modNames.Any() && modPaths.Count == modNames.Count;
                            foreach (var item in nonExistingModPaths)
                            {
                                var index = modPaths.IndexOf(item);
                                if (hasModNames)
                                {
                                    nonExistingModNames.Add($"{modNames[index]} ({item})");
                                }
                                else
                                {
                                    nonExistingModNames.Add(item);
                                }
                            }
                            var notExistingModTitle = localizationManager.GetResource(LocalizationResources.Collection_Mods.ImportNonExistingMods.Title);
                            var nonExistingModMessage = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.ImportNonExistingMods.Message), new { Environment.NewLine, Mods = string.Join(Environment.NewLine, nonExistingModNames) });
                            endOverlay = false;
                            showImportNotification = false;
                            var title = localizationManager.GetResource(LocalizationResources.Notifications.CollectionImported.Title);
                            var message = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionImported.Message), new { CollectionName = result.Name });
                            notificationAction.ShowNotification(title, message, NotificationType.Warning);
                            await TriggerOverlayAsync(id, false);
                            await notificationAction.ShowPromptAsync(notExistingModTitle, notExistingModTitle, nonExistingModMessage, NotificationType.Warning, PromptType.OK);
                        }
                    }
                    if (showImportNotification)
                    {
                        var title = localizationManager.GetResource(LocalizationResources.Notifications.CollectionImported.Title);
                        var message = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionImported.Message), new { CollectionName = result.Name });
                        notificationAction.ShowNotification(title, message, NotificationType.Success);
                    }
                }
            }
            if (endOverlay)
            {
                await TriggerOverlayAsync(id, false);
            }
        }

        /// <summary>
        /// Initializes the sorters and filters.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="setFlag">if set to <c>true</c> [set flag].</param>
        protected virtual void InitSortersAndFilters(IAppState state, bool setFlag = true)
        {
            if (setFlag)
            {
                skipModSelectionSave = true;
            }
            CollectionJumpOnPositionChange = state.CollectionJumpOnPositionChange;
            SearchMods.WatermarkText = SearchModsWatermark;
            SearchMods.Text = state?.CollectionModsSearchTerm;
            ModNameSortOrder.Text = ModName;
            if (!string.IsNullOrWhiteSpace(state.CollectionModsSelectedMod) && SelectedMods != null)
            {
                SelectedMod = SelectedMods.FirstOrDefault(p => p.DescriptorFile.Equals(state.CollectionModsSelectedMod));
            }
            RecognizeSortOrder(SelectedModCollection);
            if (setFlag)
            {
                skipModSelectionSave = false;
            }
        }

        /// <summary>
        /// Loads the mod collections.
        /// </summary>
        /// <param name="recognizeSortOrder">if set to <c>true</c> [recognize sort order].</param>
        protected virtual void LoadModCollections(bool recognizeSortOrder = true)
        {
            ModCollections = modCollectionService.GetAll();
            var selected = ModCollections?.FirstOrDefault(p => p.IsSelected);
            if (selected != null)
            {
                SelectedModCollection = selected;
                if (recognizeSortOrder)
                {
                    RecognizeSortOrder(selected);
                }
            }
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            EvalAdvancedFeaturesVisibility();
            SetSelectedMods(Mods != null ? Mods.Where(p => p.IsSelected).ToObservableCollection() : new ObservableCollection<IMod>());
            SubscribeToMods();

            skipModSelectionSave = true;
            var state = appStateService.Get();
            InitSortersAndFilters(state, false);
            LoadModCollections(false);
            ApplySort();
            skipModSelectionSave = false;

            var allowModSelectionEnabled = this.WhenAnyValue(v => v.AllowModSelection);

            CreateCommand = ReactiveCommand.Create(() =>
            {
                if (gameService.GetSelected() != null)
                {
                    EnteringNewCollection = true;
                    MessageBus.Publish(new AllowEnterHotKeysEvent(true));
                    AddNewCollection.NewCollectionName = string.Empty;
                    AddNewCollection.RenamingCollection = null;
                }
            }).DisposeWith(disposables);

            RemoveCommand = ReactiveCommand.CreateFromTask(async () =>
           {
               if (SelectedModCollection != null)
               {
                   await RemoveCollectionAsync(SelectedModCollection.Name);
               }
           }, allowModSelectionEnabled).DisposeWith(disposables);

            this.WhenAnyValue(c => c.SelectedModCollection).Subscribe(o =>
            {
                ModifyCollection.ActiveCollection = o;
                PatchMod.SetParameters(o);
                HandleModCollectionChange();
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.AddNewCollection.IsActivated).Where(p => p == true).Subscribe(activated =>
            {
                Observable.Merge(AddNewCollection.CreateCommand, AddNewCollection.CancelCommand.Select(_ => new CommandResult<string>(string.Empty, CommandState.NotExecuted))).Subscribe(result =>
                {
                    var notification = new
                    {
                        CollectionName = result.Result
                    };
                    switch (result.State)
                    {
                        case CommandState.Success:
                            skipModCollectionSave = true;
                            var id = idGenerator.GetNextId();
                            TriggerOverlay(id, true, localizationManager.GetResource(LocalizationResources.Collection_Mods.Overlay_Rename_Message));
                            if (Mods != null)
                            {
                                foreach (var mod in Mods)
                                {
                                    mod.IsSelected = false;
                                }
                            }
                            SetSelectedMods(Mods != null ? Mods.Where(p => p.IsSelected).ToObservableCollection() : new ObservableCollection<IMod>());
                            AllModsEnabled = SelectedMods?.Count > 0 && SelectedMods.All(p => p.IsSelected);
                            LoadModCollections();
                            SaveState();
                            skipModCollectionSave = EnteringNewCollection = false;
                            MessageBus.Publish(new AllowEnterHotKeysEvent(false));
                            string successTitle;
                            string successMessage;
                            if (AddNewCollection.RenamingCollection != null)
                            {
                                async Task handleRenamePatchCollection()
                                {
                                    await Task.Run(async () =>
                                    {
                                        await modPatchCollectionService.RenamePatchCollectionAsync(AddNewCollection.RenamingCollection.Name, result.Result).ConfigureAwait(false);
                                    }).ConfigureAwait(false);
                                    successTitle = localizationManager.GetResource(LocalizationResources.Notifications.CollectionRenamed.Title);
                                    successMessage = localizationManager.GetResource(LocalizationResources.Notifications.CollectionRenamed.Message);
                                    await TriggerOverlayAsync(id, false);
                                    notificationAction.ShowNotification(successTitle, successMessage, NotificationType.Success);
                                    PatchMod.SetParameters(SelectedModCollection);
                                }
                                handleRenamePatchCollection().ConfigureAwait(true);
                            }
                            else
                            {
                                TriggerOverlay(id, false);
                                successTitle = localizationManager.GetResource(LocalizationResources.Notifications.CollectionCreated.Title);
                                successMessage = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionCreated.Message), notification);
                                notificationAction.ShowNotification(successTitle, successMessage, NotificationType.Success);
                            }
                            break;

                        case CommandState.Exists:
                            var existsTitle = localizationManager.GetResource(LocalizationResources.Notifications.CollectionExists.Title);
                            var existsMessage = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionExists.Message), notification);
                            notificationAction.ShowNotification(existsTitle, existsMessage, NotificationType.Warning);
                            break;

                        case CommandState.NotExecuted:
                            MessageBus.Publish(new AllowEnterHotKeysEvent(false));
                            EnteringNewCollection = false;
                            break;

                        default:
                            break;
                    }
                }).DisposeWith(disposables);
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.ExportCollection.IsActivated).Where(p => p).Subscribe(activated =>
            {
                Observable.Merge(ExportCollection.ExportCommand.Select(p => Tuple.Create(ImportActionType.Export, p, ImportProviderType.Default)),
                    ExportCollection.ExportOrderOnlyCommand.Select(p => Tuple.Create(ImportActionType.Export, p, ImportProviderType.DefaultOrderOnly)),
                    ExportCollection.ExportWholeCollectionCommand.Select(p => Tuple.Create(ImportActionType.Export, p, ImportProviderType.DefaultWithAllMods)),
                    ExportCollection.ImportCommand.Select(p => Tuple.Create(ImportActionType.Import, p, ImportProviderType.Default)),
                    ExportCollection.ImportOtherParadoxosCommand.Select(p => Tuple.Create(ImportActionType.Import, p, ImportProviderType.Paradoxos)),
                    ExportCollection.ImportOtherParadoxCommand.Select(p => Tuple.Create(ImportActionType.Import, p, ImportProviderType.Paradox)),
                    ExportCollection.ImportOtherParadoxLauncherCommand.Select(p => Tuple.Create(ImportActionType.Import, p, ImportProviderType.ParadoxLauncher)),
                    ExportCollection.ImportOtherParadoxLauncherBetaCommand.Select(p => Tuple.Create(ImportActionType.Import, p, ImportProviderType.ParadoxLauncherBeta)),
                    ExportCollection.ImportOtherParadoxLauncherJsonCommand.Select(p => Tuple.Create(ImportActionType.Import, p, ImportProviderType.ParadoxLauncherJson)),
                    ExportCollection.ExportParadoxLauncherJsonCommand.Select(p => Tuple.Create(ImportActionType.Export, p, ImportProviderType.ParadoxLauncherJson)),
                    ExportCollection.ExportParadoxLauncherJson202110Command.Select(p => Tuple.Create(ImportActionType.Export, p, ImportProviderType.ParadoxLauncherJson202110)))
                .Subscribe(s =>
                {
                    if (s.Item2.State == CommandState.Success)
                    {
                        if (s.Item1 == ImportActionType.Export)
                        {
                            ExportCollectionAsync(s.Item2.Result, s.Item3).ConfigureAwait(true);
                        }
                        else
                        {
                            ImportCollectionAsync(s.Item2.Result, s.Item3).ConfigureAwait(true);
                        }
                    }
                }).DisposeWith(disposables);
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.ModifyCollection.IsActivated).Where(p => p).Subscribe(activated =>
            {
                Observable.Merge(ModifyCollection.RenameCommand, ModifyCollection.DuplicateCommand, ModifyCollection.MergeBasicCommand, ModifyCollection.MergeCompressCommand).Subscribe(s =>
                {
                    if (SelectedModCollection == null)
                    {
                        return;
                    }
                    if (s.Result == ModifyAction.Rename)
                    {
                        EnteringNewCollection = true;
                        MessageBus.Publish(new AllowEnterHotKeysEvent(true));
                        AddNewCollection.RenamingCollection = SelectedModCollection;
                        AddNewCollection.NewCollectionName = SelectedModCollection.Name;
                    }
                    else if (s.Result == ModifyAction.Merge)
                    {
                        if (s.State == CommandState.Success)
                        {
                            ModCollections = modCollectionService.GetAll();
                            var selected = ModCollections?.FirstOrDefault(p => p.IsSelected);
                            restoreCollectionSelection = selected.Name;
                            NeedsModListRefresh = true;
                            var existsTitle = localizationManager.GetResource(LocalizationResources.Notifications.CollectionMerged.Title);
                            var existsMessage = localizationManager.GetResource(LocalizationResources.Notifications.CollectionMerged.Message);
                            notificationAction.ShowNotification(existsTitle, existsMessage, NotificationType.Success);
                            NeedsModListRefresh = false;
                        }
                    }
                    else if (s.Result == ModifyAction.Duplicate)
                    {
                        if (s.State == CommandState.Success)
                        {
                            skipModCollectionSave = true;
                            if (Mods != null)
                            {
                                foreach (var mod in Mods)
                                {
                                    mod.IsSelected = false;
                                }
                            }
                            SetSelectedMods(Mods != null ? Mods.Where(p => p.IsSelected).ToObservableCollection() : new ObservableCollection<IMod>());
                            AllModsEnabled = SelectedMods?.Count > 0 && SelectedMods.All(p => p.IsSelected);
                            LoadModCollections();
                            SaveState();
                            skipModCollectionSave = EnteringNewCollection = false;
                            MessageBus.Publish(new AllowEnterHotKeysEvent(false));
                            var existsTitle = localizationManager.GetResource(LocalizationResources.Notifications.CollectionDuplicated.Title);
                            var existsMessage = localizationManager.GetResource(LocalizationResources.Notifications.CollectionDuplicated.Message);
                            notificationAction.ShowNotification(existsTitle, existsMessage, NotificationType.Success);
                        }
                    }
                }).DisposeWith(disposables);
            }).DisposeWith(disposables);

            this.WhenAnyValue(s => s.SearchMods.IsActivated).Where(p => p).Subscribe(s =>
            {
                this.WhenAnyValue(s => s.SearchMods.Text).Subscribe(s =>
                {
                    skipModSelectionSave = true;
                    if (!string.IsNullOrWhiteSpace(s) && SelectedMods != null)
                    {
                        SelectedMod = modService.FindMod(SelectedMods, s, false, null);
                    }
                    var modCount = SelectedModCollection?.Mods.Count();
                    var selectedModsCount = SelectedMods?.Count;
                    // Save only if the collection is fully loaded
                    if (modCount.GetValueOrDefault() == selectedModsCount.GetValueOrDefault())
                    {
                        if (string.IsNullOrWhiteSpace(s))
                        {
                            SelectedMod = null;
                        }
                        SaveState();
                    }
                    skipModSelectionSave = false;
                }).DisposeWith(disposables);

                Observable.Merge(SearchMods.DownArrowCommand, SearchMods.UpArrowCommand).Subscribe(s =>
                {
                    if (SelectedMods == null)
                    {
                        return;
                    }
                    skipModSelectionSave = true;
                    var index = -1;
                    if (SelectedMod != null)
                    {
                        index = SelectedMods.IndexOf(SelectedMod);
                    }
                    var searchString = SearchMods.Text ?? string.Empty;
                    if (!s.Result)
                    {
                        var mod = modService.FindMod(SelectedMods, searchString, false, index + 1);
                        if (mod != null && mod != SelectedMod)
                        {
                            SelectedMod = mod;
                        }
                    }
                    else
                    {
                        var reverseIndex = SelectedMods.Count - index;
                        var mod = modService.FindMod(SelectedMods, searchString, true, reverseIndex);
                        if (mod != null && mod != SelectedMod)
                        {
                            SelectedMod = mod;
                        }
                    }
                    skipModSelectionSave = false;
                    SaveState();
                }).DisposeWith(disposables);
            }).DisposeWith(disposables);

            this.WhenAnyValue(s => s.ModNameSortOrder.IsActivated).Where(p => p == true).Subscribe(s =>
            {
                ModNameSortOrder.SortCommand.Subscribe(s =>
                {
                    ApplySort();
                    SaveState();
                    SaveSelectedCollection();
                }).DisposeWith(disposables);
            }).DisposeWith(disposables);

            EnableAllCommand = ReactiveCommand.Create(() =>
            {
                skipModCollectionSave = true;
                if (SelectedMods?.Count > 0)
                {
                    foreach (var item in SelectedMods)
                    {
                        item.IsSelected = false;
                    }
                    SetSelectedMods(new List<IMod>());
                    SaveSelectedCollection();
                }
                skipModCollectionSave = false;
            }).DisposeWith(disposables);

            OpenUrlCommand = ReactiveCommand.Create(() =>
            {
                var url = GetContextMenuModUrl();
                if (!string.IsNullOrWhiteSpace(url))
                {
                    appAction.OpenAsync(url).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            CopyUrlCommand = ReactiveCommand.Create(() =>
            {
                var url = GetContextMenuModUrl();
                if (!string.IsNullOrWhiteSpace(url))
                {
                    appAction.CopyAsync(url).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            OpenInSteamCommand = ReactiveCommand.Create(() =>
            {
                var url = GetContextMenuModSteamUrl();
                if (!string.IsNullOrWhiteSpace(url))
                {
                    appAction.OpenAsync(url).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            OpenInAssociatedAppCommand = ReactiveCommand.Create(() =>
            {
                if (!string.IsNullOrWhiteSpace(ContextMenuMod?.FullPath))
                {
                    appAction.OpenAsync(ContextMenuMod.FullPath).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            this.WhenAnyValue(p => p.AllowModSelection).Subscribe(s =>
            {
                ExportCollection.AllowModSelection = s;
                ModifyCollection.AllowModSelection = s;
            }).DisposeWith(disposables);

            this.WhenAnyValue(p => p.CollectionJumpOnPositionChange).Subscribe(s =>
            {
                SetAutoFocusLabel();
            }).DisposeWith(disposables);

            CollectionJumpOnPositionChangeCommand = ReactiveCommand.Create(() =>
            {
                CollectionJumpOnPositionChange = !CollectionJumpOnPositionChange;
                SaveState();
            }).DisposeWith(disposables);

            ExportCollectionToClipboardCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (SelectedModCollection != null && SelectedMods.Count > 0)
                {
                    var sb = new StringBuilder();
                    foreach (var item in SelectedMods)
                    {
                        sb.AppendLine(item.Name);
                    }
                    await appAction.CopyAsync(sb.ToString());
                }
            }).DisposeWith(disposables);

            ImportCollectionFromClipboardCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var text = await appAction.GetAsync();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    var modNames = text.SplitOnNewLine().Select(p => p.Trim());
                    if (modNames.Any(p => Mods.Any(m => m.Name.Equals(p, StringComparison.OrdinalIgnoreCase))))
                    {
                        var title = localizationManager.GetResource(LocalizationResources.Collection_Mods.ImportFromClipboard.PromptTitle);
                        var message = localizationManager.GetResource(LocalizationResources.Collection_Mods.ImportFromClipboard.PromptMessage);
                        if (await notificationAction.ShowPromptAsync(title, title, message, NotificationType.Warning))
                        {
                            await MessageBus.PublishAsync(new ModListInstallRefreshRequestEvent(true));
                            skipModSelectionSave = true;
                            skipModCollectionSave = true;
                            var mods = new List<IMod>();
                            foreach (var item in Mods)
                            {
                                item.IsSelected = modNames.Any(p => p.Equals(item.Name));
                                if (item.IsSelected)
                                {
                                    mods.Add(item);
                                }
                            }
                            SetSelectedMods(mods.OrderBy(p => modNames.IndexOf(p.Name)).ToObservableCollection());
                            AllModsEnabled = SelectedMods?.Count > 0 && SelectedMods.All(p => p.IsSelected);
                            var state = appStateService.Get();
                            InitSortersAndFilters(state, false);
                            SaveSelectedCollection();
                            RecognizeSortOrder(SelectedModCollection);
                            skipModCollectionSave = false;
                            skipModSelectionSave = false;
                            var nonExistingMods = modNames.Where(p => !mods.Any(m => m.Name.Equals(p)));
                            if (nonExistingMods.Any())
                            {
                                var notExistingModTitle = localizationManager.GetResource(LocalizationResources.Collection_Mods.ImportNonExistingMods.Title);
                                var nonExistingModMessage = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.ImportNonExistingMods.Message), new { Environment.NewLine, Mods = string.Join(Environment.NewLine, nonExistingMods) });
                                await notificationAction.ShowPromptAsync(notExistingModTitle, notExistingModTitle, nonExistingModMessage, NotificationType.Warning, PromptType.OK);
                            }
                        }
                    }
                }
            }).DisposeWith(disposables);

            IDisposable reportDisposable = null;

            void registerReportHandlers(long id, bool useImportOverlay = false)
            {
                reportDisposable = modReportExportHandler.Subscribe(s =>
                {
                    if (useImportOverlay)
                    {
                        TriggerOverlay(id, true, localizationManager.GetResource(LocalizationResources.Collection_Mods.FileHash.ImportOverlay),
                        IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.FileHash.ProgressImport), new { Progress = s.Percentage.ToLocalizedPercentage(), Count = s.Step, TotalCount = 2 }));
                    }
                    else
                    {
                        TriggerOverlay(id, true, localizationManager.GetResource(LocalizationResources.Collection_Mods.FileHash.ExportOverlay),
                        IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.FileHash.ProgressExport), new { Progress = s.Percentage.ToLocalizedPercentage() }));
                    }
                }).DisposeWith(disposables);
            }

            ImportReportCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var title = localizationManager.GetResource(LocalizationResources.Collection_Mods.FileHash.DialogTitleImport);
                var path = await fileDialogAction.OpenDialogAsync(title, SelectedModCollection?.Name, Shared.Constants.JsonExtensionWithoutDot);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    var id = idGenerator.GetNextId();
                    await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.Collection_Mods.FileHash.ImportOverlay));
                    registerReportHandlers(id, true);
                    var rawReports = await reportExportService.ImportAsync(path);
                    var reports = new List<IHashReport>();
                    var collectionReports = await Task.Run(() => modCollectionService.ImportHashReportAsync(SelectedMods, rawReports.ToList()));
                    if (collectionReports != null && collectionReports.Any())
                    {
                        reports.AddRange(collectionReports);
                    }
                    var gameReports = await Task.Run(() => gameService.ImportHashReportAsync(activeGame, rawReports.ToList()));
                    if (gameReports != null && gameReports.Any())
                    {
                        reports.AddRange(gameReports);
                    }
                    if (reports.Any())
                    {
                        await TriggerOverlayAsync(id, false);
                        HashReportView.SetParameters(reports);
                    }
                    else
                    {
                        await TriggerOverlayAsync(id, false);
                        notificationAction.ShowNotification(localizationManager.GetResource(LocalizationResources.Notifications.ReportValid.Title),
                            localizationManager.GetResource(LocalizationResources.Notifications.ReportValid.Message), NotificationType.Success);
                    }
                    reportDisposable?.Dispose();
                }
            }).DisposeWith(disposables);

            ExportCollectionReportCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var title = localizationManager.GetResource(LocalizationResources.Collection_Mods.FileHash.DialogTitleExport);
                var path = await fileDialogAction.SaveDialogAsync(title, SelectedModCollection?.Name, Shared.Constants.JsonExtensionWithoutDot);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    var id = idGenerator.GetNextId();
                    await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.Collection_Mods.FileHash.ExportOverlay));
                    registerReportHandlers(id);
                    if (await Task.Run(() => modCollectionService.ExportHashReportAsync(SelectedMods, path)))
                    {
                        notificationAction.ShowNotification(localizationManager.GetResource(LocalizationResources.Notifications.ReportExported.Title),
                            localizationManager.GetResource(LocalizationResources.Notifications.ReportExported.Message), NotificationType.Success);
                    }
                    await TriggerOverlayAsync(id, false);
                    reportDisposable?.Dispose();
                }
            }).DisposeWith(disposables);

            ExportGameReportCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var title = localizationManager.GetResource(LocalizationResources.Collection_Mods.FileHash.DialogTitleExport);
                var path = await fileDialogAction.SaveDialogAsync(title, activeGame?.Name, Shared.Constants.JsonExtensionWithoutDot);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    var id = idGenerator.GetNextId();
                    await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.Collection_Mods.FileHash.ExportOverlay));
                    registerReportHandlers(id);
                    if (await Task.Run(() => gameService.ExportHashReportAsync(activeGame, path)))
                    {
                        notificationAction.ShowNotification(localizationManager.GetResource(LocalizationResources.Notifications.ReportExported.Title),
                            localizationManager.GetResource(LocalizationResources.Notifications.ReportExported.Message), NotificationType.Success);
                    }
                    await TriggerOverlayAsync(id, false);
                    reportDisposable?.Dispose();
                }
            }).DisposeWith(disposables);

            this.WhenAnyValue(p => p.SelectedMod).Where(p => !skipModSelectionSave).Subscribe(s =>
            {
                var modCount = SelectedModCollection?.Mods.Count();
                var selectedModsCount = SelectedMods?.Count;
                // Save only if the collection is fully loaded
                if (modCount.GetValueOrDefault() == selectedModsCount.GetValueOrDefault())
                {
                    SaveState();
                }
            }).DisposeWith(disposables);

            hotkeyPressedHandler.Subscribe(async hotkey =>
            {
                var mod = HoveredMod;
                if (mod != null)
                {
                    var order = mod.Order;
                    var delay = 50;
                    switch (hotkey.Hotkey)
                    {
                        case Enums.HotKeys.Ctrl_Up:
                            await Task.Delay(delay);
                            order--;
                            break;

                        case Enums.HotKeys.Ctrl_Down:
                            await Task.Delay(delay);
                            order++;
                            break;

                        case Enums.HotKeys.Ctrl_Shift_Up:
                            await Task.Delay(delay);
                            order = 1;
                            break;

                        case Enums.HotKeys.Ctrl_Shift_Down:
                            await Task.Delay(delay);
                            order = MaxOrder;
                            break;

                        default:
                            break;
                    }
                    if (!(order < 1 || order > MaxOrder) && order != mod.Order)
                    {
                        // Check access because it's probably coming from a background thread
                        await Dispatcher.UIThread.SafeInvokeAsync(() => mod.Order = order);
                    }
                }
            }).DisposeWith(disposables);

            this.WhenAnyValue(p => p.PatchMod.IsActivated).Where(p => p == true).Subscribe(state =>
            {
                this.WhenAnyValue(p => p.PatchMod.PatchDeleted).Where(p => p == true).Subscribe(state =>
                {
                    modPatchCollectionService.InvalidatePatchModState(PatchMod.CollectionName);
                    modPatchCollectionService.ResetPatchStateCache();
                    HandleCollectionPatchStateAsync(SelectedModCollection?.Name).ConfigureAwait(false);
                }).DisposeWith(disposables);
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// Called when [selected game changed].
        /// </summary>
        /// <param name="game">The game.</param>
        protected override void OnSelectedGameChanged(IGame game)
        {
            base.OnSelectedGameChanged(game);
            EvalAdvancedFeaturesVisibility();
        }

        /// <summary>
        /// perform mod reorder as an asynchronous operation.
        /// </summary>
        /// <param name="instant">if set to <c>true</c> [instant].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        protected virtual async Task PerformModReorderAsync(bool instant, CancellationToken cancellationToken)
        {
            if (!instant)
            {
                await Task.Delay(450, cancellationToken);
            }
            if (!cancellationToken.IsCancellationRequested)
            {
                if (SelectedMods != null)
                {
                    using var mutex = await reorderLock.LockAsync(cancellationToken);
                    if (reorderQueue.Any())
                    {
                        skipModSelectionSave = true;
                        var mods = SelectedMods.Select(m => new OrderedMod { Order = m.Order, Mod = m }).ToList();
                        foreach (var mod in reorderQueue)
                        {
                            var swapItem = mods.FirstOrDefault(p => p.Order.Equals(mod.Order) && p.Mod != mod);
                            var modItem = mods.FirstOrDefault(p => p.Mod == mod);
                            if (swapItem != null && modItem != null)
                            {
                                var index = mods.IndexOf(swapItem);
                                mods.Remove(modItem);
                                mods.Insert(index, modItem);
                                index = mods.IndexOf(swapItem);
                                for (int i = 0; i <= index; i++)
                                {
                                    if (!reorderQueue.Any(m => m == mods[i].Mod))
                                    {
                                        mods[i].Order = i + 1;
                                    }
                                }
                            }
                        }
                        SetSelectedMods(mods.Select(p => p.Mod).ToList());
                        if (CollectionJumpOnPositionChange)
                        {
                            SelectedMod = reorderQueue.Last();
                        }
                        if (!string.IsNullOrWhiteSpace(SelectedModCollection?.Name))
                        {
                            SaveSelectedCollection();
                        }
                        SaveState();
                        RecognizeSortOrder(SelectedModCollection);
                        ModReordered?.Invoke(reorderQueue.Last(), instant);
                        skipModSelectionSave = false;
                        reorderQueue.Clear();
                    }
                    mutex.Dispose();
                }
            }
        }

        /// <summary>
        /// Recognizes the sort order.
        /// </summary>
        /// <param name="modCollection">The mod collection.</param>
        protected virtual void RecognizeSortOrder(IModCollection modCollection)
        {
            // modCollection sort order
            if (modCollection?.Mods.Count() > 0 && Mods?.Count() > 0)
            {
                var mods = Mods.Where(p => modCollection.Mods.Any(a => a.Equals(p.DescriptorFile, StringComparison.OrdinalIgnoreCase)));
                if (mods.Any())
                {
                    var ascending = mods.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase).Select(p => p.DescriptorFile);
                    var descending = mods.OrderByDescending(p => p.Name, StringComparer.OrdinalIgnoreCase).Select(p => p.DescriptorFile);
                    if (ascending.SequenceEqual(modCollection.Mods))
                    {
                        ModNameSortOrder.SetSortOrder(SortOrder.Asc);
                    }
                    else if (descending.SequenceEqual(modCollection.Mods))
                    {
                        ModNameSortOrder.SetSortOrder(SortOrder.Desc);
                    }
                    else
                    {
                        ModNameSortOrder.SetSortOrder(SortOrder.None);
                    }
                }
            }
        }

        /// <summary>
        /// remove collection as an asynchronous operation.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        protected async Task RemoveCollectionAsync(string collectionName)
        {
            var noti = new { CollectionName = collectionName };
            var title = localizationManager.GetResource(LocalizationResources.Collection_Mods.Delete_Title);
            var header = localizationManager.GetResource(LocalizationResources.Collection_Mods.Delete_Header);
            var message = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.Delete_Message), noti);
            if (await notificationAction.ShowPromptAsync(title, header, message, NotificationType.Info))
            {
                var id = idGenerator.GetNextId();
                await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.Collection_Mods.Deleting_Message));
                var collection = modCollectionService.Get(collectionName);
                var deleteMergedMod = false;
                if (!string.IsNullOrWhiteSpace(collection?.MergedFolderName) && await modService.ModDirectoryExistsAsync(collection.MergedFolderName))
                {
                    deleteMergedMod = await notificationAction.ShowPromptAsync(localizationManager.GetResource(LocalizationResources.Collection_Mods.DeleteMerge.DeleteTitle),
                        localizationManager.GetResource(LocalizationResources.Collection_Mods.DeleteMerge.DeleteHeader),
                        localizationManager.GetResource(LocalizationResources.Collection_Mods.DeleteMerge.DeleteMessage), NotificationType.Info);
                }
                if (modCollectionService.Delete(collectionName))
                {
                    modPatchCollectionService.InvalidatePatchModState(collectionName);
                    await Task.Run(async () =>
                    {
                        await modPatchCollectionService.CleanPatchCollectionAsync(collectionName).ConfigureAwait(false);
                        if (deleteMergedMod)
                        {
                            await modService.PurgeModDirectoryAsync(collection.MergedFolderName).ConfigureAwait(false);
                        }
                    }).ConfigureAwait(false);
                    LoadModCollections();
                    if (deleteMergedMod)
                    {
                        NeedsModListRefresh = true;
                        var notificationTitle = localizationManager.GetResource(LocalizationResources.Notifications.CollectionAndModDeleted.Title);
                        var notificationMessage = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionAndModDeleted.Message), noti);
                        notificationAction.ShowNotification(notificationTitle, notificationMessage, NotificationType.Success);
                        NeedsModListRefresh = false;
                    }
                    else
                    {
                        var notificationTitle = localizationManager.GetResource(LocalizationResources.Notifications.CollectionDeleted.Title);
                        var notificationMessage = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionDeleted.Message), noti);
                        notificationAction.ShowNotification(notificationTitle, notificationMessage, NotificationType.Success);
                    }
                }
                await TriggerOverlayAsync(id, false);
            }
        }

        /// <summary>
        /// Saves the selected collection.
        /// </summary>
        protected virtual void SaveSelectedCollection()
        {
            var game = gameService.GetSelected()?.Type ?? string.Empty;
            var collection = modCollectionService.Create();
            // Due to async nature ensure that the game and mods are from the same source before saving
            if (collection != null && SelectedModCollection != null && Mods != null && game.Equals(SelectedModCollection.Game) && activeGame != null && activeGame.Type.Equals(game))
            {
                if (Mods.Any() && !Mods.All(p => p.Game.Equals(game)))
                {
                    return;
                }
                collection.Game = game;
                collection.Name = SelectedModCollection.Name;
                collection.Mods = SelectedMods?.Where(p => p.IsSelected).Select(p => p.DescriptorFile).ToList();
                collection.IsSelected = true;
                collection.MergedFolderName = SelectedModCollection.MergedFolderName;
                collection.PatchModEnabled = SelectedModCollection.PatchModEnabled;
                AssignOptionalCollectionMetadata(collection);
                if (modCollectionService.Save(collection))
                {
                    SelectedModCollection.Mods = collection.Mods.ToList();
                    SelectedModCollection.ModIds = collection.ModIds;
                    SelectedModCollection.ModNames = collection.ModNames;
                }
            }
        }

        /// <summary>
        /// Saves the state.
        /// </summary>
        /// <param name="useOldModSelection">if set to <c>true</c> [use old mod selection].</param>
        protected virtual void SaveState(bool useOldModSelection = false)
        {
            var state = appStateService.Get();
            state.CollectionModsSelectedMod = !useOldModSelection ? SelectedMod?.DescriptorFile : state.CollectionModsSelectedMod;
            state.CollectionModsSearchTerm = !useOldModSelection ? SearchMods.Text : state.CollectionModsSearchTerm;
            state.CollectionModsSortColumn = ModNameKey;
            state.CollectionJumpOnPositionChange = CollectionJumpOnPositionChange;
            appStateService.Save(state);
        }

        /// <summary>
        /// schedule to reorder queue as an asynchronous operation.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        protected virtual async Task ScheduleToReorderQueueAsync(IMod mod)
        {
            if (!reorderQueue.Contains(mod))
            {
                reorderQueue.Add(mod);
            }
            if (reorderToken != null)
            {
                reorderToken.Cancel();
            }
            reorderToken = new CancellationTokenSource();
            await PerformModReorderAsync(false, reorderToken.Token);
        }

        /// <summary>
        /// Sets the automatic focus label.
        /// </summary>
        protected virtual void SetAutoFocusLabel()
        {
            var focusLabel = localizationManager.GetResource(LocalizationResources.Collection_Mods.JumpOnDragAndDrop.Title);
            var focusState = localizationManager.GetResource(CollectionJumpOnPositionChange ? LocalizationResources.Collection_Mods.JumpOnDragAndDrop.On : LocalizationResources.Collection_Mods.JumpOnDragAndDrop.Off);
            var label = IronyFormatter.Format(focusLabel, new { State = focusState });
            CollectionJumpOnPositionChangeLabel = label;
        }

        /// <summary>
        /// Sets the selected mods.
        /// </summary>
        /// <param name="selectedMods">The selected mods.</param>
        /// <param name="canShutdownReorder">if set to <c>true</c> [can shutdown reorder].</param>
        protected virtual void SetSelectedMods(IList<IMod> selectedMods, bool canShutdownReorder = true)
        {
            if (canShutdownReorder)
            {
                skipReorder = true;
            }
            int counter = 0;
            if (selectedMods != null)
            {
                foreach (var item in selectedMods)
                {
                    counter++;
                    item.Order = counter;
                }
            }
            SelectedMods = selectedMods;
            if (SelectedModCollection != null)
            {
                var oldMods = new List<IMod>();
                if (SelectedMods != null)
                {
                    oldMods.AddRange(SelectedMods);
                }
                previousValidatedMods.TryGetValue(SelectedModCollection.Name, out var prevMods);
                if (SelectedMods?.Count > 0 && (prevMods == null ||
                    !(prevMods.Count() == SelectedMods.Count && !prevMods.Select(p => p.DescriptorFile).Except(SelectedMods.Select(p => p.DescriptorFile)).Any())
                    || !prevMods.Select(p => p.DescriptorFile).SequenceEqual(SelectedMods.Select(p => p.DescriptorFile))))
                {
                    modPatchCollectionService.InvalidatePatchModState(SelectedModCollection.Name);
                }
                previousValidatedMods.AddOrUpdate(SelectedModCollection.Name, oldMods, (k, v) => oldMods);
            }
            ModifyCollection.SelectedMods = selectedMods;
            HandleCollectionPatchStateAsync(SelectedModCollection?.Name).ConfigureAwait(false);
            var order = 1;
            if (SelectedMods?.Count > 0)
            {
                order = SelectedMods.Count;
            }
            MaxOrder = order;
            if (canShutdownReorder)
            {
                skipReorder = false;
            }
        }

        /// <summary>
        /// Subscribes to mods.
        /// </summary>
        protected virtual void SubscribeToMods()
        {
            modSelectedChanged?.Dispose();
            modSelectedChanged = null;
            modOrderChanged?.Dispose();
            modOrderChanged = null;
            if (Mods != null && Disposables != null)
            {
                AllModsEnabled = SelectedMods?.Count > 0 && SelectedMods.All(p => p.IsSelected);

                var sourceList = Mods.ToSourceList();

                modSelectedChanged = sourceList.Connect().WhenPropertyChanged(s => s.IsSelected).Subscribe(s =>
                {
                    skipReorder = true;
                    if (!skipModCollectionSave && !refreshInProgress)
                    {
                        var needsSave = false;
                        if (SelectedMods != null)
                        {
                            if (s.Value)
                            {
                                if (!SelectedMods.Contains(s.Sender))
                                {
                                    SelectedMods.Add(s.Sender);
                                    if (!string.IsNullOrWhiteSpace(SelectedModCollection?.Name))
                                    {
                                        SaveState();
                                    }
                                    needsSave = true;
                                }
                            }
                            else
                            {
                                if (SelectedMods.Contains(s.Sender))
                                {
                                    SelectedMods.Remove(s.Sender);
                                    needsSave = true;
                                }
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(SelectedModCollection?.Name) && needsSave)
                        {
                            SaveSelectedCollection();
                        }
                        SetSelectedMods(SelectedMods, false);
                        if (s.Sender != null)
                        {
                            InstantReorderSelectedItems(s.Sender, s.Sender.Order);
                        }
                        RecognizeSortOrder(SelectedModCollection);
                    }
                    AllModsEnabled = SelectedMods?.Count > 0 && SelectedMods.All(p => p.IsSelected);
                    skipReorder = false;
                }).DisposeWith(Disposables);

                modOrderChanged = sourceList.Connect().WhenPropertyChanged(s => s.Order).Where(s => s.Value > 0).Subscribe(s =>
                {
                    if (!refreshInProgress && !skipReorder)
                    {
                        ScheduleToReorderQueueAsync(s.Sender).ConfigureAwait(false);
                    }
                }).DisposeWith(Disposables);
            }
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class OrderedMod.
        /// </summary>
        private class OrderedMod
        {
            #region Properties

            /// <summary>
            /// Gets or sets the mod.
            /// </summary>
            /// <value>The mod.</value>
            public IMod Mod { get; set; }

            /// <summary>
            /// Gets or sets the order.
            /// </summary>
            /// <value>The order.</value>
            public int Order { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
