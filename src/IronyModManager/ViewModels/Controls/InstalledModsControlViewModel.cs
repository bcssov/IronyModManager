// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 07-10-2020
// ***********************************************************************
// <copyright file="InstalledModsControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using IronyModManager.Common;
using IronyModManager.Common.ViewModels;
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
    /// Class InstalledModsControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class InstalledModsControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The mod name key
        /// </summary>
        private const string ModNameKey = "modName";

        /// <summary>
        /// The mod selected key
        /// </summary>
        private const string ModSelectedKey = "modSelected";

        /// <summary>
        /// The mod version key
        /// </summary>
        private const string ModVersionKey = "modVersion";

        /// <summary>
        /// The URL action
        /// </summary>
        private readonly IAppAction appAction;

        /// <summary>
        /// The preferences service
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
        /// The mod service
        /// </summary>
        private readonly IModService modService;

        /// <summary>
        /// The notification action
        /// </summary>
        private readonly INotificationAction notificationAction;

        /// <summary>
        /// The checking state
        /// </summary>
        private bool checkingState = false;

        /// <summary>
        /// The mod order changed
        /// </summary>
        private IDisposable modChanged;

        /// <summary>
        /// The showing prompt
        /// </summary>
        private bool showingPrompt = false;

        /// <summary>
        /// The sort orders
        /// </summary>
        private Dictionary<string, SortOrderControlViewModel> sortOrders;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledModsControlViewModel" /> class.
        /// </summary>
        /// <param name="gameService">The game service.</param>
        /// <param name="localizationManager">The localization manager.</param>
        /// <param name="modService">The mod service.</param>
        /// <param name="appStateService">The application state service.</param>
        /// <param name="modSelectedSortOrder">The mod selected sort order.</param>
        /// <param name="modNameSortOrder">The mod name sort order.</param>
        /// <param name="modVersionSortOrder">The mod version sort order.</param>
        /// <param name="filterMods">The filter mods.</param>
        /// <param name="appAction">The application action.</param>
        /// <param name="notificationAction">The notification action.</param>
        public InstalledModsControlViewModel(IGameService gameService, ILocalizationManager localizationManager,
            IModService modService, IAppStateService appStateService, SortOrderControlViewModel modSelectedSortOrder,
            SortOrderControlViewModel modNameSortOrder, SortOrderControlViewModel modVersionSortOrder,
            SearchModsControlViewModel filterMods, IAppAction appAction, INotificationAction notificationAction)
        {
            this.modService = modService;
            this.gameService = gameService;
            this.appStateService = appStateService;
            this.appAction = appAction;
            this.notificationAction = notificationAction;
            this.localizationManager = localizationManager;
            ModNameSortOrder = modNameSortOrder;
            ModVersionSortOrder = modVersionSortOrder;
            ModSelectedSortOrder = modSelectedSortOrder;
            FilterMods = filterMods;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the achievement compatible.
        /// </summary>
        /// <value>The achievement compatible.</value>
        [StaticLocalization(LocalizationResources.Achievements.AchievementCompatible)]
        public virtual string AchievementCompatible { get; protected set; }

        /// <summary>
        /// Gets or sets all mods.
        /// </summary>
        /// <value>All mods.</value>
        public virtual HashSet<IMod> AllMods { get; protected set; }

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
        /// Gets or sets the check new mods.
        /// </summary>
        /// <value>The check new mods.</value>
        [StaticLocalization(LocalizationResources.Descriptor_Actions.CheckNew)]
        public virtual string CheckNewMods { get; protected set; }

        /// <summary>
        /// Gets or sets the check new mods command.
        /// </summary>
        /// <value>The check new mods command.</value>
        public virtual ReactiveCommand<Unit, Unit> CheckNewModsCommand { get; protected set; }

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
        /// Gets or sets the delete all descriptors.
        /// </summary>
        /// <value>The delete all descriptors.</value>
        [StaticLocalization(LocalizationResources.Descriptor_Actions.Delete_All)]
        public virtual string DeleteAllDescriptors { get; protected set; }

        /// <summary>
        /// Gets or sets the delete all descriptors command.
        /// </summary>
        /// <value>The delete all descriptors command.</value>
        public virtual ReactiveCommand<Unit, Unit> DeleteAllDescriptorsCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the delete descriptor.
        /// </summary>
        /// <value>The delete descriptor.</value>
        [StaticLocalization(LocalizationResources.Descriptor_Actions.Delete)]
        public virtual string DeleteDescriptor { get; protected set; }

        /// <summary>
        /// Gets or sets the delete descriptor command.
        /// </summary>
        /// <value>The delete descriptor command.</value>
        public virtual ReactiveCommand<Unit, Unit> DeleteDescriptorCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the enable all command.
        /// </summary>
        /// <value>The enable all command.</value>
        public virtual ReactiveCommand<Unit, Unit> EnableAllCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the filtered mods.
        /// </summary>
        /// <value>The filtered mods.</value>
        public virtual IEnumerable<IMod> FilteredMods { get; protected set; }

        /// <summary>
        /// Gets or sets the filter mods.
        /// </summary>
        /// <value>The filter mods.</value>
        public SearchModsControlViewModel FilterMods { get; protected set; }

        /// <summary>
        /// Gets or sets the filter mods watermark.
        /// </summary>
        /// <value>The filter mods watermark.</value>
        [StaticLocalization(LocalizationResources.Installed_Mods.Filter)]
        public virtual string FilterModsWatermark { get; protected set; }

        /// <summary>
        /// Gets or sets the hovered mod.
        /// </summary>
        /// <value>The hovered mod.</value>
        public virtual IMod HoveredMod { get; set; }

        /// <summary>
        /// Gets or sets the lock all descriptors.
        /// </summary>
        /// <value>The lock all descriptors.</value>
        [StaticLocalization(LocalizationResources.Descriptor_Actions.Lock_All)]
        public virtual string LockAllDescriptors { get; protected set; }

        /// <summary>
        /// Gets or sets the lock all descriptors command.
        /// </summary>
        /// <value>The lock all descriptors command.</value>
        public virtual ReactiveCommand<Unit, Unit> LockAllDescriptorsCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the lock descriptor.
        /// </summary>
        /// <value>The lock descriptor.</value>
        [StaticLocalization(LocalizationResources.Descriptor_Actions.Lock)]
        public virtual string LockDescriptor { get; protected set; }

        /// <summary>
        /// Gets or sets the lock descriptor command.
        /// </summary>
        /// <value>The lock descriptor command.</value>
        public virtual ReactiveCommand<Unit, Unit> LockDescriptorCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the mod.
        /// </summary>
        /// <value>The name of the mod.</value>
        [StaticLocalization(LocalizationResources.Installed_Mods.Mod_Name)]
        public virtual string ModName { get; protected set; }

        /// <summary>
        /// Gets or sets the mod name sort order.
        /// </summary>
        /// <value>The mod name sort order.</value>
        public virtual SortOrderControlViewModel ModNameSortOrder { get; protected set; }

        /// <summary>
        /// Gets or sets the mods.
        /// </summary>
        /// <value>The mods.</value>
        public virtual IEnumerable<IMod> Mods { get; protected set; }

        /// <summary>
        /// Gets or sets the mod selected.
        /// </summary>
        /// <value>The mod selected.</value>
        [StaticLocalization(LocalizationResources.Installed_Mods.Selected)]
        public virtual string ModSelected { get; protected set; }

        /// <summary>
        /// Gets or sets the mod selected sort order.
        /// </summary>
        /// <value>The mod selected sort order.</value>
        public virtual SortOrderControlViewModel ModSelectedSortOrder { get; protected set; }

        /// <summary>
        /// Gets or sets the mod version.
        /// </summary>
        /// <value>The mod version.</value>
        [StaticLocalization(LocalizationResources.Installed_Mods.Supported_Version)]
        public virtual string ModVersion { get; protected set; }

        /// <summary>
        /// Gets or sets the mod version sort order.
        /// </summary>
        /// <value>The mod version sort order.</value>
        public virtual SortOrderControlViewModel ModVersionSortOrder { get; protected set; }

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
        /// Gets or sets a value indicating whether [refreshing mods].
        /// </summary>
        /// <value><c>true</c> if [refreshing mods]; otherwise, <c>false</c>.</value>
        public virtual bool RefreshingMods { get; protected set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [StaticLocalization(LocalizationResources.Installed_Mods.Name)]
        public virtual string Title { get; protected set; }

        /// <summary>
        /// Gets or sets the unlock all descriptors.
        /// </summary>
        /// <value>The unlock all descriptors.</value>
        [StaticLocalization(LocalizationResources.Descriptor_Actions.Unlock_All)]
        public virtual string UnlockAllDescriptors { get; protected set; }

        /// <summary>
        /// Gets or sets the unlock all descriptors command.
        /// </summary>
        /// <value>The unlock all descriptors command.</value>
        public virtual ReactiveCommand<Unit, Unit> UnlockAllDescriptorsCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the unlock descriptor.
        /// </summary>
        /// <value>The unlock descriptor.</value>
        [StaticLocalization(LocalizationResources.Descriptor_Actions.Unlock)]
        public virtual string UnlockDescriptor { get; protected set; }

        /// <summary>
        /// Gets or sets the unlock descriptor command.
        /// </summary>
        /// <value>The unlock descriptor command.</value>
        public virtual ReactiveCommand<Unit, Unit> UnlockDescriptorCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [performing enable all].
        /// </summary>
        /// <value><c>true</c> if [performing enable all]; otherwise, <c>false</c>.</value>
        public virtual bool PerformingEnableAll { get; protected set; }

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
        /// Called when [locale changed].
        /// </summary>
        /// <param name="newLocale">The new locale.</param>
        /// <param name="oldLocale">The old locale.</param>
        public override void OnLocaleChanged(string newLocale, string oldLocale)
        {
            ModVersionSortOrder.Text = ModVersion;
            ModNameSortOrder.Text = ModName;
            ModSelectedSortOrder.Text = ModSelected;
            FilterMods.WatermarkText = FilterModsWatermark;

            base.OnLocaleChanged(newLocale, oldLocale);
        }

        /// <summary>
        /// Refreshes the mods.
        /// </summary>
        public virtual void RefreshMods()
        {
            RefreshModsAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// refresh mods as an asynchronous operation.
        /// </summary>
        public virtual async Task RefreshModsAsync()
        {
            RefreshingMods = true;
            var previousMods = Mods;
            await BindAsync();
            if (Mods?.Count() > 0)
            {
                foreach (var item in previousMods.Where(p => p.IsSelected))
                {
                    var mod = Mods.FirstOrDefault(p => p.DescriptorFile.Equals(item.DescriptorFile, StringComparison.OrdinalIgnoreCase));
                    if (mod != null)
                    {
                        mod.IsSelected = true;
                    }
                }
            }
            RefreshingMods = false;
        }

        /// <summary>
        /// Applies the default sort.
        /// </summary>
        protected virtual void ApplyDefaultSort()
        {
            var sortModel = sortOrders.FirstOrDefault(p => p.Value.SortOrder != Implementation.SortOrder.None);
            ApplySort(sortModel.Key);
        }

        /// <summary>
        /// Applies the sort.
        /// </summary>
        /// <param name="sortBy">The sort by.</param>
        protected virtual void ApplySort(string sortBy)
        {
            var sortModel = sortOrders.FirstOrDefault(p => p.Key == sortBy);
            switch (sortModel.Key)
            {
                case ModNameKey:
                    SortFunction(x => x.Name, sortModel.Key);
                    break;

                case ModSelectedKey:
                    SortFunction(x => x.IsSelected, sortModel.Key);
                    break;

                case ModVersionKey:
                    SortFunction(x => x.Version, sortModel.Key);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Binds the specified game.
        /// </summary>
        /// <param name="game">The game.</param>
        protected virtual void Bind(IGame game = null)
        {
            BindAsync(game).ConfigureAwait(false);
        }

        /// <summary>
        /// Binds the asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>Task.</returns>
        protected virtual async Task BindAsync(IGame game = null)
        {
            await TriggerOverlayAsync(true, localizationManager.GetResource(LocalizationResources.Installed_Mods.LoadingMods));
            if (game == null)
            {
                game = gameService.GetSelected();
            }
            if (game != null)
            {
                var mods = await Task.Run(() => modService.GetInstalledMods(game));
                await Task.Run(async () =>
                {
                    await EvalAchievementCompatibilityAsync(mods).ConfigureAwait(false);
                });
                Mods = mods.ToObservableCollection();
                AllMods = Mods.ToHashSet();
                var invalidMods = AllMods.Where(p => !p.IsValid);
                if (invalidMods.Count() > 0)
                {
                    await RemoveInvalidModsPromptAsync(invalidMods).ConfigureAwait(false);
                }
                var searchString = FilterMods.Text ?? string.Empty;
                FilteredMods = Mods.Where(p => p.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase) ||
                    (p.RemoteId.HasValue && p.RemoteId.GetValueOrDefault().ToString().Contains(searchString))).ToObservableCollection();
                AllModsEnabled = FilteredMods.Count() > 0 && FilteredMods.Where(p => p.IsValid).All(p => p.IsSelected);

                if (Disposables != null)
                {
                    modChanged?.Dispose();
                    modChanged = null;
                    modChanged = Mods.ToSourceList().Connect().WhenPropertyChanged(s => s.IsSelected).Subscribe(s =>
                    {
                        if (!checkingState)
                        {
                            CheckModEnabledStateAsync().ConfigureAwait(false);
                        }
                    }).DisposeWith(Disposables);
                }

                var state = appStateService.Get();
                InitSortersAndFilters(state);

                ApplyDefaultSort();
            }
            else
            {
                Mods = FilteredMods = new System.Collections.ObjectModel.ObservableCollection<IMod>();
                AllMods = Mods.ToHashSet();
            }
            await TriggerOverlayAsync(false);
        }

        /// <summary>
        /// check mod enabled state as an asynchronous operation.
        /// </summary>
        protected virtual async Task CheckModEnabledStateAsync()
        {
            checkingState = true;
            await Task.Delay(50);
            AllModsEnabled = FilteredMods.Count() > 0 && FilteredMods.Where(p => p.IsValid).All(p => p.IsSelected);
            checkingState = false;
        }

        /// <summary>
        /// check new mods as an asynchronous operation.
        /// </summary>
        protected virtual async Task CheckNewModsAsync()
        {
            await TriggerOverlayAsync(true, message: localizationManager.GetResource(LocalizationResources.Installed_Mods.RefreshingModList));
            await modService.InstallModsAsync();
            RefreshMods();
            var title = localizationManager.GetResource(LocalizationResources.Notifications.NewDescriptorsChecked.Title);
            var message = localizationManager.GetResource(LocalizationResources.Notifications.NewDescriptorsChecked.Message);
            notificationAction.ShowNotification(title, message, NotificationType.Info);
            await TriggerOverlayAsync(false);
        }

        /// <summary>
        /// delete descriptor as an asynchronous operation.
        /// </summary>
        /// <param name="mods">The mods.</param>
        protected virtual async Task DeleteDescriptorAsync(IEnumerable<IMod> mods)
        {
            if (mods?.Count() > 0)
            {
                await TriggerOverlayAsync(true, message: localizationManager.GetResource(LocalizationResources.Installed_Mods.RefreshingModList));
                await modService.DeleteDescriptorsAsync(mods);
                await modService.InstallModsAsync();
                RefreshMods();
                var title = localizationManager.GetResource(LocalizationResources.Notifications.DescriptorsRefreshed.Title);
                var message = localizationManager.GetResource(LocalizationResources.Notifications.DescriptorsRefreshed.Message);
                notificationAction.ShowNotification(title, message, NotificationType.Info);
                await TriggerOverlayAsync(false);
            }
        }

        /// <summary>
        /// eval achievement compatibility as an asynchronous operation.
        /// </summary>
        /// <param name="mods">The mods.</param>
        protected virtual async Task EvalAchievementCompatibilityAsync(IEnumerable<IMod> mods)
        {
            await modService.PopulateModFilesAsync(mods);
            modService.EvalAchievementCompatibility(mods);
        }

        /// <summary>
        /// Initializes the default sort order.
        /// </summary>
        /// <param name="dictKey">The dictionary key.</param>
        /// <param name="vm">The vm.</param>
        /// <param name="defaultOrder">The default order.</param>
        /// <param name="text">The text.</param>
        /// <param name="appState">State of the application.</param>
        protected virtual void InitDefaultSortOrder(string dictKey, SortOrderControlViewModel vm, Implementation.SortOrder defaultOrder, string text, IAppState appState)
        {
            if (!string.IsNullOrWhiteSpace(appState.InstalledModsSortColumn) && Enum.IsDefined(typeof(Implementation.SortOrder), appState.InstalledModsSortMode))
            {
                if (dictKey.Equals(appState.InstalledModsSortColumn))
                {
                    var sort = (Implementation.SortOrder)appState.InstalledModsSortMode;
                    vm.SortOrder = sort;
                }
                else
                {
                    vm.SortOrder = Implementation.SortOrder.None;
                }
            }
            else
            {
                vm.SortOrder = defaultOrder;
            }
            vm.Text = text;
            sortOrders.Add(dictKey, vm);
        }

        /// <summary>
        /// Initializes the sorters and filters.
        /// </summary>
        /// <param name="appState">The preferences.</param>
        protected virtual void InitSortersAndFilters(IAppState appState)
        {
            sortOrders = new Dictionary<string, SortOrderControlViewModel>();
            InitDefaultSortOrder(ModNameKey, ModNameSortOrder, Implementation.SortOrder.Asc, ModName, appState);
            InitDefaultSortOrder(ModVersionKey, ModVersionSortOrder, Implementation.SortOrder.None, ModVersion, appState);
            InitDefaultSortOrder(ModSelectedKey, ModSelectedSortOrder, Implementation.SortOrder.None, ModSelected, appState);
            FilterMods.Text = appState?.InstalledModsSearchTerm;
            FilterMods.WatermarkText = FilterModsWatermark;
        }

        /// <summary>
        /// lock descriptor as an asynchronous operation.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <param name="isLocked">if set to <c>true</c> [is locked].</param>
        protected virtual async Task LockDescriptorAsync(IEnumerable<IMod> mods, bool isLocked)
        {
            if (mods?.Count() > 0)
            {
                await modService.LockDescriptorsAsync(mods, isLocked);
                var title = isLocked ? localizationManager.GetResource(LocalizationResources.Notifications.DescriptorsLocked.Title) : localizationManager.GetResource(LocalizationResources.Notifications.DescriptorsUnlocked.Title);
                var message = isLocked ? localizationManager.GetResource(LocalizationResources.Notifications.DescriptorsLocked.Message) : localizationManager.GetResource(LocalizationResources.Notifications.DescriptorsUnlocked.Message);
                notificationAction.ShowNotification(title, message, NotificationType.Info);
            }
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            // Set default order and sort order text
            var state = appStateService.Get();
            InitSortersAndFilters(state);

            Bind();

            this.WhenAnyValue(v => v.ModNameSortOrder.IsActivated, v => v.ModVersionSortOrder.IsActivated, v => v.ModSelectedSortOrder.IsActivated).Where(s => s.Item1 && s.Item2 && s.Item3)
            .Subscribe(s =>
             {
                 Observable.Merge(ModNameSortOrder.SortCommand.Select(_ => ModNameKey),
                     ModVersionSortOrder.SortCommand.Select(_ => ModVersionKey),
                     ModSelectedSortOrder.SortCommand.Select(_ => ModSelectedKey)).Subscribe(s =>
                 {
                     ApplySort(s);
                 }).DisposeWith(disposables);
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

            this.WhenAnyValue(s => s.FilterMods.Text).Subscribe(s =>
            {
                var searchString = FilterMods.Text ?? string.Empty;
                FilteredMods = Mods?.Where(p => p.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase) ||
                    (p.RemoteId.HasValue && p.RemoteId.GetValueOrDefault().ToString().Contains(searchString))).ToObservableCollection();
                AllModsEnabled = FilteredMods?.Count() > 0 && FilteredMods.Where(p => p.IsValid).All(p => p.IsSelected);
                ApplyDefaultSort();
                SaveState();
            }).DisposeWith(disposables);

            EnableAllCommand = ReactiveCommand.Create(() =>
            {
                if (FilteredMods?.Count() > 0)
                {
                    PerformingEnableAll = true;
                    bool enabled = Mods.Where(p => p.IsValid).All(p => p.IsSelected);
                    foreach (var item in FilteredMods)
                    {
                        item.IsSelected = !enabled;
                    }
                    AllModsEnabled = FilteredMods?.Count() > 0 && FilteredMods.Where(p => p.IsValid).All(p => p.IsSelected);
                    PerformingEnableAll = false;
                }
            }).DisposeWith(disposables);

            DeleteDescriptorCommand = ReactiveCommand.Create(() =>
            {
                if (HoveredMod != null)
                {
                    DeleteDescriptorAsync(new List<IMod>() { HoveredMod }).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            DeleteAllDescriptorsCommand = ReactiveCommand.Create(() =>
            {
                if (FilteredMods != null)
                {
                    DeleteDescriptorAsync(FilteredMods).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            LockDescriptorCommand = ReactiveCommand.Create(() =>
            {
                if (HoveredMod != null)
                {
                    LockDescriptorAsync(new List<IMod>() { HoveredMod }, true).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            LockAllDescriptorsCommand = ReactiveCommand.Create(() =>
            {
                if (FilteredMods != null)
                {
                    LockDescriptorAsync(FilteredMods, true).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            UnlockDescriptorCommand = ReactiveCommand.Create(() =>
            {
                if (HoveredMod != null)
                {
                    LockDescriptorAsync(new List<IMod>() { HoveredMod }, false).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            UnlockAllDescriptorsCommand = ReactiveCommand.Create(() =>
            {
                if (FilteredMods != null)
                {
                    LockDescriptorAsync(FilteredMods, false).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            CheckNewModsCommand = ReactiveCommand.Create(() =>
            {
                CheckNewModsAsync().ConfigureAwait(true);
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// Called when [selected game changed].
        /// </summary>
        /// <param name="game">The game.</param>
        protected override void OnSelectedGameChanged(IGame game)
        {
            Bind(game);

            base.OnSelectedGameChanged(game);
        }

        /// <summary>
        /// remove invalid mods prompt as an asynchronous operation.
        /// </summary>
        /// <param name="mods">The mods.</param>
        protected virtual async Task RemoveInvalidModsPromptAsync(IEnumerable<IMod> mods)
        {
            if (showingPrompt)
            {
                return;
            }
            showingPrompt = true;
            var messages = new List<string>();
            foreach (var item in mods)
            {
                messages.Add($"{item.Name} ({item.DescriptorFile})");
            }
            var title = localizationManager.GetResource(LocalizationResources.Installed_Mods.InvalidMods.Title);
            var message = Smart.Format(localizationManager.GetResource(LocalizationResources.Installed_Mods.InvalidMods.Message), new
            {
                Mods = string.Join(Environment.NewLine, messages),
                Environment.NewLine
            });
            if (await notificationAction.ShowPromptAsync(title, title, message, NotificationType.Warning))
            {
                await DeleteDescriptorAsync(mods);
            }
            showingPrompt = false;
        }

        /// <summary>
        /// Resolves the comparer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictKey">The dictionary key.</param>
        /// <returns>IComparer&lt;T&gt;.</returns>
        protected virtual IComparer<T> ResolveComparer<T>(string dictKey)
        {
            if (dictKey.Equals(ModNameKey))
            {
                return (IComparer<T>)StringComparer.OrdinalIgnoreCase;
            }
            return null;
        }

        /// <summary>
        /// Saves the state.
        /// </summary>
        protected virtual void SaveState()
        {
            var state = appStateService.Get();
            state.InstalledModsSearchTerm = FilterMods.Text;
            var sortModel = sortOrders.FirstOrDefault(p => p.Value.SortOrder != Implementation.SortOrder.None);
            state.InstalledModsSortColumn = sortModel.Key;
            state.InstalledModsSortMode = (int)sortModel.Value.SortOrder;
            appStateService.Save(state);
        }

        /// <summary>
        /// Sorts the function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sortProp">The sort property.</param>
        /// <param name="dictKey">The dictionary key.</param>
        protected virtual void SortFunction<T>(Func<IMod, T> sortProp, string dictKey)
        {
            if (FilteredMods != null)
            {
                var sortOrder = sortOrders[dictKey];
                IComparer<T> comparer = ResolveComparer<T>(dictKey);
                switch (sortOrder.SortOrder)
                {
                    case Implementation.SortOrder.Asc:
                        if (comparer != null)
                        {
                            FilteredMods = FilteredMods.OrderBy(sortProp, comparer).ToObservableCollection();
                            AllMods = AllMods.OrderBy(sortProp, comparer).ToHashSet();
                        }
                        else
                        {
                            FilteredMods = FilteredMods.OrderBy(sortProp).ToObservableCollection();
                            AllMods = AllMods.OrderBy(sortProp).ToHashSet();
                        }
                        break;

                    case Implementation.SortOrder.Desc:
                        if (comparer != null)
                        {
                            FilteredMods = FilteredMods.OrderByDescending(sortProp, comparer).ToObservableCollection();
                            AllMods = AllMods.OrderByDescending(sortProp, comparer).ToHashSet();
                        }
                        else
                        {
                            FilteredMods = FilteredMods.OrderByDescending(sortProp).ToObservableCollection();
                            AllMods = AllMods.OrderByDescending(sortProp).ToHashSet();
                        }
                        break;

                    default:
                        break;
                }
                foreach (var sort in sortOrders.Where(p => p.Value != sortOrder))
                {
                    sort.Value.SetSortOrder(Implementation.SortOrder.None);
                }
                SaveState();
            }
        }

        #endregion Methods
    }
}
