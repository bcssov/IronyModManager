// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-03-2020
//
// Last Modified By : Mario
// Last Modified On : 09-30-2020
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
using System.Threading.Tasks;
using Avalonia.Threading;
using DynamicData;
using IronyModManager.Common;
using IronyModManager.Common.Events;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation;
using IronyModManager.Implementation.Actions;
using IronyModManager.Localization;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using ReactiveUI;
using SmartFormat;
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
        /// The game service
        /// </summary>
        private readonly IGameService gameService;

        /// <summary>
        /// The localization manager
        /// </summary>
        private readonly ILocalizationManager localizationManager;

        /// <summary>
        /// The message bus
        /// </summary>
        private readonly Shared.MessageBus.IMessageBus messageBus;

        /// <summary>
        /// The mod collection service
        /// </summary>
        private readonly IModCollectionService modCollectionService;

        /// <summary>
        /// The mod patch collection service
        /// </summary>
        private readonly IModPatchCollectionService modPatchCollectionService;

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
        private readonly ConcurrentDictionary<string, IEnumerable<IMod>> previousValidatedMods = new ConcurrentDictionary<string, IEnumerable<IMod>>();

        /// <summary>
        /// The reorder queue
        /// </summary>
        private readonly ConcurrentBag<IMod> reorderQueue;

        /// <summary>
        /// The active game
        /// </summary>
        private IGame activeGame;

        /// <summary>
        /// The enable all toggled state
        /// </summary>
        private bool enableAllToggledState = false;

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
        /// The reorder counter
        /// </summary>
        private int reorderCounter = 0;

        /// <summary>
        /// The restore collection selection
        /// </summary>
        private string restoreCollectionSelection = string.Empty;

        /// <summary>
        /// The skip mod collection save
        /// </summary>
        private bool skipModCollectionSave = false;

        /// <summary>
        /// The skip reorder
        /// </summary>
        private bool skipReorder = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionModsControlViewModel" /> class.
        /// </summary>
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
        /// <param name="messageBus">The message bus.</param>
        public CollectionModsControlViewModel(IModCollectionService modCollectionService,
            IAppStateService appStateService, IModPatchCollectionService modPatchCollectionService, IModService modService, IGameService gameService,
            AddNewCollectionControlViewModel addNewCollection, ExportModCollectionControlViewModel exportCollection, ModifyCollectionControlViewModel modifyCollection,
            SearchModsControlViewModel searchMods, SortOrderControlViewModel modNameSort, ILocalizationManager localizationManager,
            INotificationAction notificationAction, IAppAction appAction, Shared.MessageBus.IMessageBus messageBus)
        {
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
            this.messageBus = messageBus;
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
        public delegate void ModReorderedDelegate(IMod mod);

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
            ParadoxLauncher
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
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Name)]
        public virtual string Title { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the hovered mod steam URL.
        /// </summary>
        /// <returns>System.String.</returns>
        public virtual string GetHoveredModSteamUrl()
        {
            if (HoveredMod != null)
            {
                var url = modService.BuildSteamUrl(HoveredMod);
                return url;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the selected mod URL.
        /// </summary>
        /// <returns>System.String.</returns>
        public virtual string GetHoveredModUrl()
        {
            if (HoveredMod != null)
            {
                var url = modService.BuildModUrl(HoveredMod);
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

                AllModsEnabled = SelectedMods?.Count() > 0 && SelectedMods.All(p => p.IsSelected);
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
            async Task waitForQueue()
            {
                while (reorderCounter != 0)
                {
                    await Task.Delay(50);
                }
                mod.Order = newOrder;
                PerformModReorder(mod);
            }
            waitForQueue().ConfigureAwait(false);
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
        /// Resets this instance.
        /// </summary>
        public virtual void Reset()
        {
            ValidateCollectionPatchStateAsync(SelectedModCollection?.Name).ConfigureAwait(false);
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
                    SetSelectedMods(SelectedMods.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase).ToObservableCollection());
                    break;

                case SortOrder.Desc:
                    SetSelectedMods(SelectedMods.OrderByDescending(x => x.Name, StringComparer.OrdinalIgnoreCase).ToObservableCollection());
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Assigns the mod collection names.
        /// </summary>
        /// <param name="collection">The collection.</param>
        protected virtual void AssignModCollectionNames(IModCollection collection)
        {
            if (collection == null)
            {
                collection = SelectedModCollection;
            }
            collection.ModNames = SelectedMods?.Where(p => p.IsSelected).Select(p => p.Name).ToList();
        }

        /// <summary>
        /// export collection as an asynchronous operation.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="providerType">Type of the provider.</param>
        protected virtual async Task ExportCollectionAsync(string path, ImportProviderType providerType)
        {
            await TriggerOverlayAsync(true, localizationManager.GetResource(LocalizationResources.Collection_Mods.Overlay_Exporting_Message));
            var collection = modCollectionService.Get(SelectedModCollection.Name);
            AssignModCollectionNames(collection);
            await Task.Run(async () => await modCollectionService.ExportAsync(path, collection, providerType == ImportProviderType.DefaultOrderOnly));
            var title = localizationManager.GetResource(LocalizationResources.Notifications.CollectionExported.Title);
            var message = Smart.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionExported.Message), new { CollectionName = collection.Name });
            notificationAction.ShowNotification(title, message, NotificationType.Success);
            await TriggerOverlayAsync(false);
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
            ExportCollection.CollectionName = SelectedModCollection?.Name;
            SaveState();
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
                foreach (var item in existingCollection.Mods)
                {
                    var mod = Mods.FirstOrDefault(p => p.DescriptorFile.Equals(item, StringComparison.InvariantCultureIgnoreCase));
                    if (mod != null)
                    {
                        mod.IsSelected = true;
                        selectedMods.Add(mod);
                    }
                }
            }
            SetSelectedMods(selectedMods);
            AllModsEnabled = SelectedMods?.Count() > 0 && SelectedMods.All(p => p.IsSelected);
            var state = appStateService.Get();
            InitSortersAndFilters(state);
            ApplySort();
            SaveSelectedCollection();
            skipModCollectionSave = false;
        }

        /// <summary>
        /// import collection as an asynchronous operation.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="type">The type.</param>
        protected virtual async Task ImportCollectionAsync(string path, ImportProviderType type)
        {
            async Task<IModCollection> importDefault()
            {
                var collection = await Task.Run(async () => await modCollectionService.ImportAsync(path));
                if (collection != null)
                {
                    collection.IsSelected = true;
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
                    if (modCollectionService.Save(importData))
                    {
                        return Task.FromResult(importData);
                    }
                }
                return Task.FromResult((IModCollection)null);
            }

            await TriggerOverlayAsync(true, localizationManager.GetResource(LocalizationResources.Collection_Mods.Overlay_Importing_Message));
            var importData = type switch
            {
                ImportProviderType.Paradoxos => await modCollectionService.ImportParadoxosAsync(path),
                ImportProviderType.Paradox => await modCollectionService.ImportParadoxAsync(),
                ImportProviderType.ParadoxLauncher => await modCollectionService.ImportParadoxLauncherAsync(),
                _ => await modCollectionService.GetImportedCollectionDetailsAsync(path),
            };
            if (importData == null)
            {
                await TriggerOverlayAsync(false);
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
            if (proceed)
            {
                var result = type switch
                {
                    ImportProviderType.Default => await importDefault(),
                    _ => await importInstance(importData),
                };
                if (result != null)
                {
                    var game = gameService.Get().FirstOrDefault(p => p.Type.Equals(result.Game, StringComparison.OrdinalIgnoreCase));
                    await messageBus.PublishAsync(new ActiveGameRequestEvent(game));
                    restoreCollectionSelection = result.Name;
                    LoadModCollections();
                    var title = localizationManager.GetResource(LocalizationResources.Notifications.CollectionImported.Title);
                    var message = Smart.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionImported.Message), new { CollectionName = result.Name });
                    notificationAction.ShowNotification(title, message, NotificationType.Success);
                }
            }
            await TriggerOverlayAsync(false);
        }

        /// <summary>
        /// Initializes the sorters and filters.
        /// </summary>
        /// <param name="state">The state.</param>
        protected virtual void InitSortersAndFilters(IAppState state)
        {
            CollectionJumpOnPositionChange = state.CollectionJumpOnPositionChange;
            SearchMods.WatermarkText = SearchModsWatermark;
            SearchMods.Text = state?.CollectionModsSearchTerm;
            ModNameSortOrder.Text = ModName;
            if (!string.IsNullOrWhiteSpace(state.CollectionModsSelectedMod) && SelectedMods != null)
            {
                SelectedMod = SelectedMods.FirstOrDefault(p => p.DescriptorFile.Equals(state.CollectionModsSelectedMod));
            }
            RecognizeSortOrder(SelectedModCollection);
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
                if (recognizeSortOrder)
                {
                    RecognizeSortOrder(selected);
                }
                SelectedModCollection = selected;
            }
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            SetSelectedMods(Mods != null ? Mods.Where(p => p.IsSelected).ToObservableCollection() : new ObservableCollection<IMod>());
            SubscribeToMods();

            var state = appStateService.Get();
            InitSortersAndFilters(state);
            LoadModCollections(false);
            ApplySort();

            var allowModSelectionEnabled = this.WhenAnyValue(v => v.AllowModSelection);

            CreateCommand = ReactiveCommand.Create(() =>
            {
                if (gameService.GetSelected() != null)
                {
                    EnteringNewCollection = true;
                    AddNewCollection.NewCollectionName = string.Empty;
                    AddNewCollection.RenamingCollection = null;
                }
            }).DisposeWith(disposables);

            RemoveCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedModCollection != null)
                {
                    RemoveCollectionAsync(SelectedModCollection.Name).ConfigureAwait(true);
                }
            }, allowModSelectionEnabled).DisposeWith(disposables);

            this.WhenAnyValue(c => c.SelectedModCollection).Subscribe(o =>
            {
                ModifyCollection.ActiveCollection = o;
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
                            if (Mods != null)
                            {
                                foreach (var mod in Mods)
                                {
                                    mod.IsSelected = false;
                                }
                            }
                            SetSelectedMods(Mods != null ? Mods.Where(p => p.IsSelected).ToObservableCollection() : new ObservableCollection<IMod>());
                            AllModsEnabled = SelectedMods?.Count() > 0 && SelectedMods.All(p => p.IsSelected);
                            LoadModCollections();
                            SaveState();
                            skipModCollectionSave = EnteringNewCollection = false;
                            string successTitle;
                            string successMessage;
                            if (AddNewCollection.RenamingCollection != null)
                            {
                                async Task handleRenamePatchCollection()
                                {
                                    await TriggerOverlayAsync(true, localizationManager.GetResource(LocalizationResources.Collection_Mods.Overlay_Rename_Message));
                                    await modPatchCollectionService.RenamePatchCollectionAsync(AddNewCollection.RenamingCollection.Name, result.Result).ConfigureAwait(false);
                                    successTitle = localizationManager.GetResource(LocalizationResources.Notifications.CollectionRenamed.Title);
                                    successMessage = localizationManager.GetResource(LocalizationResources.Notifications.CollectionRenamed.Message);
                                    await TriggerOverlayAsync(false);
                                    await Dispatcher.UIThread.InvokeAsync(() =>
                                    {
                                        notificationAction.ShowNotification(successTitle, successMessage, NotificationType.Success);
                                    });
                                }
                                handleRenamePatchCollection().ConfigureAwait(true);
                            }
                            else
                            {
                                successTitle = localizationManager.GetResource(LocalizationResources.Notifications.CollectionCreated.Title);
                                successMessage = Smart.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionCreated.Message), notification);
                                notificationAction.ShowNotification(successTitle, successMessage, NotificationType.Success);
                            }
                            break;

                        case CommandState.Exists:
                            var existsTitle = localizationManager.GetResource(LocalizationResources.Notifications.CollectionExists.Title);
                            var existsMessage = Smart.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionExists.Message), notification);
                            notificationAction.ShowNotification(existsTitle, existsMessage, NotificationType.Warning);
                            break;

                        case CommandState.NotExecuted:
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
                    ExportCollection.ImportCommand.Select(p => Tuple.Create(ImportActionType.Import, p, ImportProviderType.Default)),
                    ExportCollection.ImportOtherParadoxosCommand.Select(p => Tuple.Create(ImportActionType.Import, p, ImportProviderType.Paradoxos)),
                    ExportCollection.ImportOtherParadoxCommand.Select(p => Tuple.Create(ImportActionType.Import, p, ImportProviderType.Paradox)),
                    ExportCollection.ImportOtherParadoxLauncherCommand.Select(p => Tuple.Create(ImportActionType.Import, p, ImportProviderType.ParadoxLauncher)))
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
                Observable.Merge(ModifyCollection.RenameCommand, ModifyCollection.DuplicateCommand, ModifyCollection.MergeAdvancedCommand, ModifyCollection.MergeBasicCommand).Subscribe(s =>
                {
                    if (SelectedModCollection == null)
                    {
                        return;
                    }
                    if (s.Result == ModifyAction.Rename)
                    {
                        EnteringNewCollection = true;
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
                            AllModsEnabled = SelectedMods?.Count() > 0 && SelectedMods.All(p => p.IsSelected);
                            LoadModCollections();
                            SaveState();
                            skipModCollectionSave = EnteringNewCollection = false;
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
                    if (!string.IsNullOrWhiteSpace(s) && SelectedMods != null)
                    {
                        SelectedMod = SelectedMods.FirstOrDefault(p => p.Name.Contains(s, StringComparison.InvariantCultureIgnoreCase) || (p.RemoteId.HasValue && p.RemoteId.GetValueOrDefault().ToString().Contains(s)));
                    }
                    SaveState();
                }).DisposeWith(disposables);

                Observable.Merge(SearchMods.DownArrowCommand, SearchMods.UpArrowCommand).Subscribe(s =>
                {
                    if (SelectedMods == null)
                    {
                        return;
                    }
                    var index = -1;
                    if (SelectedMod != null)
                    {
                        index = SelectedMods.IndexOf(SelectedMod);
                    }
                    var searchString = SearchMods.Text ?? string.Empty;
                    if (!s.Result)
                    {
                        var mod = SelectedMods.Skip(index + 1).FirstOrDefault(s => s.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase) ||
                                        (s.RemoteId.HasValue && s.RemoteId.GetValueOrDefault().ToString().Contains(searchString)));
                        if (mod != null && mod != SelectedMod)
                        {
                            SelectedMod = mod;
                        }
                    }
                    else
                    {
                        var reverseIndex = SelectedMods.Count - index;
                        var mod = SelectedMods.Reverse().Skip(reverseIndex).FirstOrDefault(s => s.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase) ||
                                        (s.RemoteId.HasValue && s.RemoteId.GetValueOrDefault().ToString().Contains(searchString)));
                        if (mod != null && mod != SelectedMod)
                        {
                            SelectedMod = mod;
                        }
                    }
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
                if (SelectedMods?.Count() > 0)
                {
                    foreach (var item in SelectedMods)
                    {
                        item.IsSelected = false;
                    }
                    SelectedMods.Clear();
                    SaveSelectedCollection();
                }
                skipModCollectionSave = false;
            }).DisposeWith(disposables);

            OpenUrlCommand = ReactiveCommand.Create(() =>
            {
                var url = GetHoveredModUrl();
                if (!string.IsNullOrWhiteSpace(url))
                {
                    appAction.OpenAsync(url).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            CopyUrlCommand = ReactiveCommand.Create(() =>
            {
                var url = GetHoveredModUrl();
                if (!string.IsNullOrWhiteSpace(url))
                {
                    appAction.CopyAsync(url).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            OpenInSteamCommand = ReactiveCommand.Create(() =>
            {
                var url = GetHoveredModSteamUrl();
                if (!string.IsNullOrWhiteSpace(url))
                {
                    appAction.OpenAsync(url).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            OpenInAssociatedAppCommand = ReactiveCommand.Create(() =>
            {
                if (!string.IsNullOrWhiteSpace(HoveredMod?.FullPath))
                {
                    appAction.OpenAsync(HoveredMod.FullPath).ConfigureAwait(true);
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
                    var modNames = text.SplitOnNewLine();
                    if (modNames.Any(p => Mods.Any(m => m.Name.Equals(p, StringComparison.OrdinalIgnoreCase))))
                    {
                        var title = localizationManager.GetResource(LocalizationResources.Collection_Mods.ImportFromClipboard.PromptTitle);
                        var message = localizationManager.GetResource(LocalizationResources.Collection_Mods.ImportFromClipboard.PromptMessage);
                        if (await notificationAction.ShowPromptAsync(title, title, message, NotificationType.Warning))
                        {
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
                            AllModsEnabled = SelectedMods?.Count() > 0 && SelectedMods.All(p => p.IsSelected);
                            var state = appStateService.Get();
                            InitSortersAndFilters(state);
                            SaveSelectedCollection();
                            RecognizeSortOrder(SelectedModCollection);
                            skipModCollectionSave = false;
                        }
                    }
                }
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
            LoadModCollections();
        }

        /// <summary>
        /// Performs the mod reorder.
        /// </summary>
        /// <param name="mods">The mods.</param>
        protected virtual void PerformModReorder(params IMod[] mods)
        {
            if (SelectedMods != null && mods.Count() > 0)
            {
                foreach (var mod in mods)
                {
                    var swapItem = SelectedMods.FirstOrDefault(p => p.Order.Equals(mod.Order) && p != mod);
                    if (swapItem != null)
                    {
                        var index = SelectedMods.IndexOf(swapItem);
                        SelectedMods.Remove(mod);
                        SelectedMods.Insert(index, mod);
                    }
                }
                SetSelectedMods(SelectedMods);
                if (CollectionJumpOnPositionChange)
                {
                    SelectedMod = mods.Last();
                }
                if (!string.IsNullOrWhiteSpace(SelectedModCollection?.Name))
                {
                    SaveSelectedCollection();
                }
                SaveState();
                RecognizeSortOrder(SelectedModCollection);
                ModReordered?.Invoke(mods.Last());
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
                if (mods.Count() > 0)
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
        protected async Task RemoveCollectionAsync(string collectionName)
        {
            var noti = new { CollectionName = collectionName };
            var title = localizationManager.GetResource(LocalizationResources.Collection_Mods.Delete_Title);
            var header = localizationManager.GetResource(LocalizationResources.Collection_Mods.Delete_Header);
            var message = Smart.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.Delete_Message), noti);
            if (await notificationAction.ShowPromptAsync(title, header, message, NotificationType.Info))
            {
                if (modCollectionService.Delete(collectionName))
                {
                    modPatchCollectionService.InvalidatePatchModState(collectionName);
                    await Task.Run(async () => await modPatchCollectionService.CleanPatchCollectionAsync(collectionName));
                    var notificationTitle = localizationManager.GetResource(LocalizationResources.Notifications.CollectionDeleted.Title);
                    var notificationMessage = Smart.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionDeleted.Title), noti);
                    notificationAction.ShowNotification(notificationTitle, notificationMessage, NotificationType.Success);
                    LoadModCollections();
                }
            }
        }

        /// <summary>
        /// reorder selected items as an asynchronous operation.
        /// </summary>
        /// <param name="queueNumber">The queue number.</param>
        protected virtual async Task ReorderQueuedItemsAsync(int queueNumber)
        {
            await Task.Delay(300);
            if (reorderCounter == queueNumber)
            {
                PerformModReorder(reorderQueue.ToArray());
                reorderCounter = 0;
                reorderQueue.Clear();
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
            if (collection != null && SelectedModCollection != null && Mods != null && game.Equals(SelectedModCollection.Game) && activeGame.Type.Equals(game))
            {
                if (Mods.Count() > 0 && !Mods.All(p => p.Game.Equals(game)))
                {
                    return;
                }
                collection.Game = game;
                collection.Name = SelectedModCollection.Name;
                collection.Mods = SelectedMods?.Where(p => p.IsSelected).Select(p => p.DescriptorFile).ToList();
                collection.IsSelected = true;
                if (modCollectionService.Save(collection))
                {
                    SelectedModCollection.Mods = collection.Mods.ToList();
                }
            }
        }

        /// <summary>
        /// Saves the state.
        /// </summary>
        protected virtual void SaveState()
        {
            var state = appStateService.Get();
            state.CollectionModsSelectedMod = SelectedMod?.DescriptorFile;
            state.CollectionModsSearchTerm = SearchMods.Text;
            state.CollectionModsSortColumn = ModNameKey;
            state.CollectionJumpOnPositionChange = CollectionJumpOnPositionChange;
            appStateService.Save(state);
        }

        /// <summary>
        /// Sets the automatic focus label.
        /// </summary>
        protected virtual void SetAutoFocusLabel()
        {
            var focusLabel = localizationManager.GetResource(LocalizationResources.Collection_Mods.JumpOnDragAndDrop.Title);
            var focusState = localizationManager.GetResource(CollectionJumpOnPositionChange ? LocalizationResources.Collection_Mods.JumpOnDragAndDrop.On : LocalizationResources.Collection_Mods.JumpOnDragAndDrop.Off);
            var label = Smart.Format(focusLabel, new { State = focusState });
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
                if (SelectedMods?.Count > 0 && (prevMods == null || !(prevMods.Count() == SelectedMods.Count && !prevMods.Select(p => p.DescriptorFile).Except(SelectedMods.Select(p => p.DescriptorFile)).Any())))
                {
                    modPatchCollectionService.InvalidatePatchModState(SelectedModCollection.Name);
                }
                previousValidatedMods.AddOrUpdate(SelectedModCollection.Name, oldMods, (k, v) => oldMods);
            }
            ModifyCollection.SelectedMods = selectedMods;
            ValidateCollectionPatchStateAsync(SelectedModCollection?.Name).ConfigureAwait(false);
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
                AllModsEnabled = SelectedMods?.Count() > 0 && SelectedMods.All(p => p.IsSelected);

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
                        RecognizeSortOrder(SelectedModCollection);
                    }
                    AllModsEnabled = SelectedMods?.Count() > 0 && SelectedMods.All(p => p.IsSelected);
                    skipReorder = false;
                }).DisposeWith(Disposables);

                modOrderChanged = sourceList.Connect().WhenPropertyChanged(s => s.Order).Where(s => s.Value > 0).Subscribe(s =>
                {
                    if (!refreshInProgress && !skipReorder)
                    {
                        reorderCounter++;
                        var queue = reorderCounter;
                        if (!reorderQueue.Contains(s.Sender))
                        {
                            reorderQueue.Add(s.Sender);
                        }
                        ReorderQueuedItemsAsync(queue).ConfigureAwait(false);
                    }
                }).DisposeWith(Disposables);
            }
        }

        /// <summary>
        /// validate collection patch state as an asynchronous operation.
        /// </summary>
        /// <param name="collection">The collection.</param>
        protected virtual async Task ValidateCollectionPatchStateAsync(string collection)
        {
            var currentCollection = SelectedModCollection?.Name ?? string.Empty;
            if (activeGame != null && SelectedMods?.Count > 0)
            {
                ConflictSolverStateChanged?.Invoke(collection, true);
                if (!string.IsNullOrWhiteSpace(collection) && currentCollection.Equals(collection, StringComparison.OrdinalIgnoreCase) && SelectedModCollection.Game.Equals(activeGame.Type))
                {
                    var result = await Task.Run(async () => await modPatchCollectionService.PatchModNeedsUpdateAsync(collection));
                    ConflictSolverStateChanged?.Invoke(collection, !result);
                }
            }
            else
            {
                ConflictSolverStateChanged?.Invoke(collection, true);
            }
        }

        #endregion Methods
    }
}
