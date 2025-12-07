// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 12-07-2025
// ***********************************************************************
// <copyright file="InstalledModsControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using DynamicData;
using IronyModManager.Common;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation.Actions;
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
        /// The eval lock
        /// </summary>
        private readonly AsyncLock evalLock = new();

        /// <summary>
        /// The eval mod achievements compatibility handler
        /// </summary>
        private readonly EvalModAchievementCompatibilityHandler evalModAchievementsCompatibilityHandler;

        /// <summary>
        /// The eval mods queue
        /// </summary>
        private readonly ConcurrentBag<IMod> evalModsQueue;

        /// <summary>
        /// The game service
        /// </summary>
        private readonly IGameService gameService;

        /// <summary>
        /// The identifier generator
        /// </summary>
        private readonly IIDGenerator idGenerator;

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
        private bool checkingState;

        /// <summary>
        /// The eval mod token
        /// </summary>
        private CancellationTokenSource evalModToken;

        /// <summary>
        /// The mod order changed
        /// </summary>
        private IDisposable modChanged;

        /// <summary>
        /// The showing invalid notification
        /// </summary>
        private bool showingInvalidNotification;

        /// <summary>
        /// The showing prompt
        /// </summary>
        private bool showingPrompt;

        /// <summary>
        /// The sort orders
        /// </summary>
        private Dictionary<string, SortOrderControlViewModel> sortOrders;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledModsControlViewModel" /> class.
        /// </summary>
        /// <param name="evalModAchievementsCompatibilityHandler">The eval mod achievements compatibility handler.</param>
        /// <param name="idGenerator">The identifier generator.</param>
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
        public InstalledModsControlViewModel(EvalModAchievementCompatibilityHandler evalModAchievementsCompatibilityHandler, IIDGenerator idGenerator, IGameService gameService, ILocalizationManager localizationManager,
            IModService modService, IAppStateService appStateService, SortOrderControlViewModel modSelectedSortOrder,
            SortOrderControlViewModel modNameSortOrder, SortOrderControlViewModel modVersionSortOrder,
            SearchModsControlViewModel filterMods, IAppAction appAction, INotificationAction notificationAction)
        {
            this.idGenerator = idGenerator;
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
            this.evalModAchievementsCompatibilityHandler = evalModAchievementsCompatibilityHandler;
            evalModsQueue = new ConcurrentBag<IMod>();
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
        /// Gets or sets the active game.
        /// </summary>
        /// <value>The active game.</value>
        public IGame ActiveGame { get; protected set; }

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
        /// Gets or sets the context menu mod.
        /// </summary>
        /// <value>The context menu mod.</value>
        public virtual IMod ContextMenuMod { get; set; }

        /// <summary>
        /// Gets or sets a value representing the copy mod path.
        /// </summary>
        /// <value>The copy mod path.</value>
        [StaticLocalization(LocalizationResources.Mod_App_Actions.CopyPath)]
        public virtual string CopyModPath { get; protected set; }

        /// <summary>
        /// Gets or sets a value representing the copy mod path command.<see cref="ReactiveUI.ReactiveCommand{System.Reactive.Unit, System.Reactive.Unit}" />
        /// </summary>
        /// <value>The copy mod path command.</value>
        public virtual ReactiveCommand<Unit, Unit> CopyModPathCommand { get; protected set; }

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
        /// Gets or sets a value indicating whether [game changed refresh].
        /// </summary>
        /// <value><c>true</c> if [game changed refresh]; otherwise, <c>false</c>.</value>
        public virtual bool GameChangedRefresh { get; protected set; }

        /// <summary>
        /// Gets or sets the local mod tooltip.
        /// </summary>
        /// <value>The local mod tooltip.</value>
        [StaticLocalization(LocalizationResources.ModSource.Local)]
        public virtual string LocalModTooltip { get; protected set; }

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
        /// Gets or sets the paradox mod tooltip.
        /// </summary>
        /// <value>The paradox mod tooltip.</value>
        [StaticLocalization(LocalizationResources.ModSource.Paradox)]
        public virtual string ParadoxModTooltip { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [performing enable all].
        /// </summary>
        /// <value><c>true</c> if [performing enable all]; otherwise, <c>false</c>.</value>
        public virtual bool PerformingEnableAll { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [refreshing mods].
        /// </summary>
        /// <value><c>true</c> if [refreshing mods]; otherwise, <c>false</c>.</value>
        public virtual bool RefreshingMods { get; protected set; }

        /// <summary>
        /// Gets or sets the selected mod.
        /// </summary>
        /// <value>The selected mod.</value>
        public virtual IMod SelectedMod { get; protected set; }

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

        #endregion Properties

        #region Methods

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
        /// refresh mods as an asynchronous operation.
        /// </summary>
        /// <param name="skipOverlay">if set to <c>true</c> [skip overlay].</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public virtual async Task RefreshModsAsync(bool skipOverlay = false)
        {
            RefreshingMods = true;
            var previousMods = Mods;
            await BindAsync(skipOverlay: skipOverlay);
            if (Mods?.Count() > 0 && previousMods?.Count() > 0)
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
                    SortFunction(x => x.VersionData, sortModel.Key);
                    break;
            }
        }

        /// <summary>
        /// Binds the specified game.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="skipOverlay">if set to <c>true</c> [skip overlay].</param>
        protected virtual void Bind(IGame game = null, bool skipOverlay = false)
        {
            BindAsync(game, skipOverlay).ConfigureAwait(false);
        }

        /// <summary>
        /// Binds the asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="skipOverlay">if set to <c>true</c> [skip overlay].</param>
        /// <returns>Task.</returns>
        protected virtual async Task BindAsync(IGame game = null, bool skipOverlay = false)
        {
            var raiseGameChanged = game != null;
            GameChangedRefresh = false;
            var id = idGenerator.GetNextId();
            if (!skipOverlay)
            {
                await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.Installed_Mods.LoadingMods));
            }

            game ??= gameService.GetSelected();
            ActiveGame = game;
            if (game != null)
            {
                if (raiseGameChanged)
                {
                    GameChangedRefresh = true;
                }

                var mods = await Task.Run(async () => await modService.GetInstalledModsAsync(game));
                await Task.Delay(100);
                Mods = mods.ToObservableCollection();
                AllMods = Mods.ToHashSet();
                var invalidMods = AllMods.Where(p => !p.IsValid);
                if (invalidMods.Any())
                {
                    await Dispatcher.UIThread.SafeInvokeAsync(async () =>
                    {
                        await RemoveInvalidModsPromptAsync(invalidMods).ConfigureAwait(false);
                    });
                }

                var searchString = FilterMods.Text ?? string.Empty;
                if (Mods != null && Mods.Any(p => p.AchievementStatus == AchievementStatus.NotEvaluated) && modService.QueryContainsAchievements(searchString))
                {
                    await MessageBus.PublishAsync(new EvalModAchievementsCompatibilityEvent(Mods, skipOverlay, true));
                }

                FilteredMods = modService.FilterMods(Mods, searchString).ToObservableCollection();
                AllModsEnabled = FilteredMods.Any(p => p.IsValid) && FilteredMods.Where(p => p.IsValid).All(p => p.IsSelected);

                if (Disposables != null)
                {
                    modChanged?.Dispose();
                    modChanged = null;
                    modChanged = Mods.ToSourceList().Connect().WhenPropertyChanged(s => s.IsSelected).Subscribe(_ =>
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

            if (!skipOverlay)
            {
                await TriggerOverlayAsync(id, false);
            }
        }

        /// <summary>
        /// check mod enabled state as an asynchronous operation.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected virtual async Task CheckModEnabledStateAsync()
        {
            checkingState = true;
            await Task.Delay(50);
            AllModsEnabled = FilteredMods.Where(p => p.IsValid).Any() && FilteredMods.Where(p => p.IsValid).All(p => p.IsSelected);
            checkingState = false;
        }

        /// <summary>
        /// check new mods as an asynchronous operation.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected virtual async Task CheckNewModsAsync()
        {
            var id = idGenerator.GetNextId();
            await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.Installed_Mods.RefreshingModList));
            var result = await modService.InstallModsAsync(Mods);
            if (result != null && result.Any(p => p.Invalid))
            {
                await ShowInvalidModsNotificationAsync(result.Where(p => p.Invalid).ToList());
            }

            await RefreshModsAsync();
            var title = localizationManager.GetResource(LocalizationResources.Notifications.NewDescriptorsChecked.Title);
            var message = localizationManager.GetResource(LocalizationResources.Notifications.NewDescriptorsChecked.Message);
            notificationAction.ShowNotification(title, message, NotificationType.Info);
            await TriggerOverlayAsync(id, false);
        }

        /// <summary>
        /// delete descriptor as an asynchronous operation.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected virtual async Task DeleteDescriptorAsync(IEnumerable<IMod> mods)
        {
            if (mods?.Count() > 0)
            {
                var id = idGenerator.GetNextId();
                await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.Installed_Mods.RefreshingModList));
                await modService.DeleteDescriptorsAsync(mods);
                var result = await modService.InstallModsAsync(Mods);
                if (result != null && result.Any(p => p.Invalid))
                {
                    await ShowInvalidModsNotificationAsync(result.Where(p => p.Invalid).ToList());
                }

                await RefreshModsAsync();
                var title = localizationManager.GetResource(LocalizationResources.Notifications.DescriptorsRefreshed.Title);
                var message = localizationManager.GetResource(LocalizationResources.Notifications.DescriptorsRefreshed.Message);
                notificationAction.ShowNotification(title, message, NotificationType.Info);
                await TriggerOverlayAsync(id, false);
            }
        }

        /// <summary>
        /// Eval mod achievement as an asynchronous operation.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <param name="hasPriority">if set to <c>true</c> [has priority].</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected virtual async Task EvalModAchievementAsync(IEnumerable<IMod> mods, bool hasPriority = false)
        {
            async Task performEval(CancellationToken token)
            {
                if (!hasPriority)
                {
                    await Task.Delay(100, token);
                }

                if (!token.IsCancellationRequested || hasPriority)
                {
                    if (evalModsQueue.Any())
                    {
                        var col = evalModsQueue.Distinct().ToList();
                        evalModsQueue.Clear();
                        await modService.PopulateModFilesAsync(col);
                        modService.EvalAchievementCompatibility(col);
                    }
                }
            }

            evalModToken?.Cancel();
            evalModToken = new CancellationTokenSource();
            if (mods.Any())
            {
                IDisposable mutex = null;
                if (hasPriority)
                {
                    mutex = await evalLock.LockAsync();
                }

                mods.ToList().ForEach(m => evalModsQueue.Add(m));
                try
                {
                    await performEval(evalModToken.Token);
                }
                catch (OperationCanceledException)
                {
                }

                mutex?.Dispose();
            }
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
            sortOrders[dictKey] = vm;
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
        /// <returns>A Task representing the asynchronous operation.</returns>
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
                .Subscribe(_ =>
                {
                    Observable.Merge(ModNameSortOrder.SortCommand.Select(_ => ModNameKey),
                        ModVersionSortOrder.SortCommand.Select(_ => ModVersionKey),
                        ModSelectedSortOrder.SortCommand.Select(_ => ModSelectedKey)).Subscribe(ApplySort).DisposeWith(disposables);
                }).DisposeWith(disposables);

            OpenUrlCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var url = GetContextMenuModUrl();
                if (!string.IsNullOrWhiteSpace(url))
                {
                    await appAction.OpenAsync(url).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            CopyUrlCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var url = GetContextMenuModUrl();
                if (!string.IsNullOrWhiteSpace(url))
                {
                    await appAction.CopyAsync(url).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            OpenInSteamCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var url = GetContextMenuModSteamUrl();
                if (!string.IsNullOrWhiteSpace(url))
                {
                    var args = gameService.GetLaunchSettings(ActiveGame);
                    var launchDefault = true;
                    if (gameService.IsFlatpakSteamGame(args))
                    {
                        // ReSharper disable once StringLiteralTypo
                        if (await appAction.OpenFlatpakAsync("com.valvesoftware.Steam", args.ExecutableLocation))
                        {
                            launchDefault = false;
                        }
                    }

                    if (launchDefault)
                    {
                        await appAction.OpenAsync(url).ConfigureAwait(true);
                    }
                }
            }).DisposeWith(disposables);

            OpenInAssociatedAppCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (!string.IsNullOrWhiteSpace(ContextMenuMod?.FullPath))
                {
                    await appAction.OpenAsync(ContextMenuMod.FullPath).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            CopyModPathCommand = ReactiveCommand.Create(() =>
            {
                if (!string.IsNullOrWhiteSpace(ContextMenuMod?.FullPath))
                {
                    appAction.CopyAsync(ContextMenuMod.FullPath).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            this.WhenAnyValue(s => s.FilterMods.Text).Subscribe(async _ =>
            {
                var searchString = FilterMods.Text ?? string.Empty;
                if (Mods != null && Mods.Any(p => p.AchievementStatus == AchievementStatus.NotEvaluated) && modService.QueryContainsAchievements(searchString))
                {
                    await MessageBus.PublishAsync(new EvalModAchievementsCompatibilityEvent(Mods, true, true));
                }

                FilteredMods = modService.FilterMods(Mods, searchString);
                AllModsEnabled = FilteredMods != null && FilteredMods.Any(p => p.IsValid) && FilteredMods.Where(p => p.IsValid).All(p => p.IsSelected);
                ApplyDefaultSort();
                SaveState();
            }).DisposeWith(disposables);

            EnableAllCommand = ReactiveCommand.Create(() =>
            {
                if (FilteredMods?.Count() > 0)
                {
                    PerformingEnableAll = true;
                    var enabled = FilteredMods.Where(p => p.IsValid).All(p => p.IsSelected);
                    foreach (var item in FilteredMods)
                    {
                        item.IsSelected = !enabled;
                    }

                    AllModsEnabled = FilteredMods.Any(p => p.IsValid) && FilteredMods.Where(p => p.IsValid).All(p => p.IsSelected);
                    PerformingEnableAll = false;
                }
            }).DisposeWith(disposables);

            DeleteDescriptorCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (ContextMenuMod != null)
                {
                    await DeleteDescriptorAsync(new List<IMod> { ContextMenuMod }).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            DeleteAllDescriptorsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (FilteredMods != null)
                {
                    await DeleteDescriptorAsync(FilteredMods).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            LockDescriptorCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (ContextMenuMod != null)
                {
                    await LockDescriptorAsync(new List<IMod> { ContextMenuMod }, true).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            LockAllDescriptorsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (FilteredMods != null)
                {
                    await LockDescriptorAsync(FilteredMods, true).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            UnlockDescriptorCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (ContextMenuMod != null)
                {
                    await LockDescriptorAsync(new List<IMod> { ContextMenuMod }, false).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            UnlockAllDescriptorsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (FilteredMods != null)
                {
                    await LockDescriptorAsync(FilteredMods, false).ConfigureAwait(true);
                }
            }).DisposeWith(disposables);

            CheckNewModsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await CheckNewModsAsync().ConfigureAwait(true);
            }).DisposeWith(disposables);

            evalModAchievementsCompatibilityHandler.Subscribe(async s =>
            {
                var id = idGenerator.GetNextId();
                if (s.ShowOverlay)
                {
                    await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.Installed_Mods.LoadingMods));
                }

                await EvalModAchievementAsync(s.Mods, s.HasPriority).ConfigureAwait(false);
                if (s.ShowOverlay)
                {
                    await TriggerOverlayAsync(id, false);
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
            Bind(game);

            base.OnSelectedGameChanged(game);
        }

        /// <summary>
        /// remove invalid mods prompt as an asynchronous operation.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
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
            var message = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Installed_Mods.InvalidMods.Message), new { Mods = string.Join(Environment.NewLine, messages), Environment.NewLine });
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
        /// Shows the invalid mods notification asynchronous.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <returns>Task.</returns>
        protected virtual async Task ShowInvalidModsNotificationAsync(IReadOnlyCollection<IModInstallationResult> mods)
        {
            var title = localizationManager.GetResource(LocalizationResources.InvalidModsDetected.Title);
            var message = localizationManager.GetResource(LocalizationResources.InvalidModsDetected.Message).FormatIronySmart(new { Environment.NewLine, Mods = string.Join(Environment.NewLine, mods.Select(p => p.Path)) });

            if (!showingInvalidNotification)
            {
                showingInvalidNotification = true;
                await notificationAction.ShowPromptAsync(title, title, message, NotificationType.Error, PromptType.OK);
                showingInvalidNotification = false;
            }
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
                var comparer = ResolveComparer<T>(dictKey);
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

                        SelectedMod = null;
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

                        SelectedMod = null;
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
