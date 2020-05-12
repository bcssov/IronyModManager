// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-03-2020
//
// Last Modified By : Mario
// Last Modified On : 05-11-2020
// ***********************************************************************
// <copyright file="CollectionModsControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using IronyModManager.Common;
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
        /// The mod collection service
        /// </summary>
        private readonly IModCollectionService modCollectionService;

        /// <summary>
        /// The mod service
        /// </summary>
        private readonly IModService modService;

        /// <summary>
        /// The notification action
        /// </summary>
        private readonly INotificationAction notificationAction;

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
        public CollectionModsControlViewModel(IModCollectionService modCollectionService,
            IAppStateService appStateService, IModService modService, IGameService gameService,
            AddNewCollectionControlViewModel addNewCollection, ExportModCollectionControlViewModel exportCollection, ModifyCollectionControlViewModel modifyCollection,
            SearchModsControlViewModel searchMods, SortOrderControlViewModel modNameSort, ILocalizationManager localizationManager,
            INotificationAction notificationAction, IAppAction appAction)
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
            SearchMods.ShowArrows = true;
        }

        #endregion Constructors

        #region Delegates

        /// <summary>
        /// Delegate ModReorderedDelegate
        /// </summary>
        /// <param name="mod">The mod.</param>
        public delegate void ModReorderedDelegate(IMod mod);

        #endregion Delegates

        #region Events

        /// <summary>
        /// Occurs when [mod reordered].
        /// </summary>
        public event ModReorderedDelegate ModReordered;

        #endregion Events

        #region Properties

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
        /// Gets or sets the hovered mod.
        /// </summary>
        /// <value>The hovered mod.</value>
        public virtual IMod HoveredMod { get; set; }

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
        /// Handles the mod refresh.
        /// </summary>
        /// <param name="isRefreshing">if set to <c>true</c> [is refreshing].</param>
        /// <param name="mods">The mods.</param>
        public virtual void HandleModRefresh(bool isRefreshing, IEnumerable<IMod> mods)
        {
            if (isRefreshing)
            {
                refreshInProgress = true;
            }
            if (!isRefreshing && mods?.Count() > 0)
            {
                SetMods(mods);
                HandleModCollectionChange();
                refreshInProgress = false;
            }
        }

        /// <summary>
        /// Called when [locale changed].
        /// </summary>
        /// <param name="newLocale">The new locale.</param>
        /// <param name="oldLocale">The old locale.</param>
        public override void OnLocaleChanged(string newLocale, string oldLocale)
        {
            SearchMods.WatermarkText = SearchModsWatermark;
            ModNameSortOrder.Text = ModName;
            base.OnLocaleChanged(newLocale, oldLocale);
        }

        /// <summary>
        /// Sets the mods.
        /// </summary>
        /// <param name="mods">The mods.</param>
        public void SetMods(IEnumerable<IMod> mods)
        {
            Mods = mods;

            SetSelectedMods(mods != null ? mods.Where(p => p.IsSelected).ToObservableCollection() : new ObservableCollection<IMod>());
            SubscribeToMods();

            var state = appStateService.Get();
            InitSortersAndFilters(state);
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
        /// export collection as an asynchronous operation.
        /// </summary>
        /// <param name="path">The path.</param>
        protected virtual async Task ExportCollectionAsync(string path)
        {
            await TriggerOverlayAsync(true, localizationManager.GetResource(LocalizationResources.Collection_Mods.Overlay_Exporting_Message));
            var collection = modCollectionService.Get(SelectedModCollection.Name);
            await modCollectionService.ExportAsync(path, collection);
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
            skipModCollectionSave = true;
            ExportCollection.CanExport = SelectedModCollection != null;
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
        protected virtual async Task ImportCollectionAsync(string path)
        {
            await TriggerOverlayAsync(true, localizationManager.GetResource(LocalizationResources.Collection_Mods.Overlay_Importing_Message));
            var collection = await modCollectionService.ImportAsync(path);
            if (collection != null)
            {
                collection.IsSelected = true;
                modCollectionService.Save(collection);
                LoadModCollections();
                var title = localizationManager.GetResource(LocalizationResources.Notifications.CollectionImported.Title);
                var message = Smart.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionImported.Message), new { CollectionName = collection.Name });
                notificationAction.ShowNotification(title, message, NotificationType.Success);
            }
            await TriggerOverlayAsync(false);
        }

        /// <summary>
        /// Initializes the sorters and filters.
        /// </summary>
        /// <param name="state">The state.</param>
        protected virtual void InitSortersAndFilters(IAppState state)
        {
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
            }).DisposeWith(disposables);

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
                            var successTitle = localizationManager.GetResource(LocalizationResources.Notifications.CollectionCreated.Title);
                            var successMessage = Smart.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionCreated.Message), notification);
                            notificationAction.ShowNotification(successTitle, successMessage, NotificationType.Success);
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
                Observable.Merge(ExportCollection.ExportCommand.Select(p => KeyValuePair.Create(true, p)), ExportCollection.ImportCommand.Select(p => KeyValuePair.Create(false, p))).Subscribe(s =>
                {
                    if (s.Value.State == CommandState.Success)
                    {
                        if (s.Key)
                        {
                            ExportCollectionAsync(s.Value.Result).ConfigureAwait(true);
                        }
                        else
                        {
                            ImportCollectionAsync(s.Value.Result).ConfigureAwait(true);
                        }
                    }
                }).DisposeWith(disposables);
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.ModifyCollection.IsActivated).Where(p => p).Subscribe(activated =>
            {
                Observable.Merge(ModifyCollection.RenameCommand.Select(p => KeyValuePair.Create(true, p)), ModifyCollection.DuplicateCommand.Select(p => KeyValuePair.Create(false, p))).Subscribe(s =>
                {
                    if (SelectedModCollection == null)
                    {
                        return;
                    }
                    if (s.Value.Result)
                    {
                        EnteringNewCollection = true;
                        AddNewCollection.RenamingCollection = SelectedModCollection;
                        AddNewCollection.NewCollectionName = SelectedModCollection.Name;
                    }
                    else
                    {
                        if (s.Value.State == CommandState.Success)
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
                    await modService.CleanPatchCollectionAsync(collectionName);
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
        /// <param name="mod">The mod.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        protected virtual async Task ReorderSelectedItemsAsync(IMod mod, CancellationToken cancellationToken)
        {
            // Allow task to be canceled
            await Task.Delay(500, cancellationToken);
            if (SelectedMods != null)
            {
                var swapItem = SelectedMods.FirstOrDefault(p => p.Order.Equals(mod.Order) && p != mod);
                if (swapItem != null)
                {
                    var index = SelectedMods.IndexOf(swapItem);
                    SelectedMods.Remove(mod);
                    SelectedMods.Insert(index, mod);
                    SetSelectedMods(SelectedMods);
                    SelectedMod = mod;
                    if (!string.IsNullOrWhiteSpace(SelectedModCollection?.Name))
                    {
                        SaveSelectedCollection();
                    }
                    SaveState();
                    RecognizeSortOrder(SelectedModCollection);
                }
                ModReordered?.Invoke(mod);
            }
            reorderToken = null;
        }

        /// <summary>
        /// Saves the selected collection.
        /// </summary>
        protected virtual void SaveSelectedCollection()
        {
            var collection = modCollectionService.Create();
            if (collection != null && SelectedModCollection != null)
            {
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
            appStateService.Save(state);
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

                modOrderChanged = sourceList.Connect().WhenPropertyChanged(s => s.Order).Where(s => !refreshInProgress && !skipReorder && s.Value > 0).Subscribe(s =>
                {
                    if (reorderToken == null)
                    {
                        reorderToken = new CancellationTokenSource();
                    }
                    else
                    {
                        reorderToken.Cancel();
                        reorderToken = new CancellationTokenSource();
                    }
                    ReorderSelectedItemsAsync(s.Sender, reorderToken.Token).ConfigureAwait(true);
                }).DisposeWith(Disposables);
            }
        }

        #endregion Methods
    }
}
