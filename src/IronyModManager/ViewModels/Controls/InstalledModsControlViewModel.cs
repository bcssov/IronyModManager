// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 03-02-2020
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
using DynamicData;
using IronyModManager.Common;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation.MenuActions;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
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
        /// The game service
        /// </summary>
        private readonly IGameService gameService;

        /// <summary>
        /// The mod service
        /// </summary>
        private readonly IModService modService;

        /// <summary>
        /// The sort orders
        /// </summary>
        private readonly Dictionary<string, SortOrderControlViewModel> sortOrders;

        /// <summary>
        /// The URL action
        /// </summary>
        private readonly IUrlAction urlAction;

        /// <summary>
        /// The mods changed
        /// </summary>
        private IDisposable modsChanged;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledModsControlViewModel" /> class.
        /// </summary>
        /// <param name="gameService">The game service.</param>
        /// <param name="modService">The mod service.</param>
        /// <param name="modSelectedSortOrder">The mod selected sort order.</param>
        /// <param name="modNameSortOrder">The mod name sort order.</param>
        /// <param name="modVersionSortOrder">The mod version sort order.</param>
        /// <param name="filterMods">The filter mods.</param>
        /// <param name="urlAction">The URL action.</param>
        public InstalledModsControlViewModel(IGameService gameService,
            IModService modService, SortOrderControlViewModel modSelectedSortOrder,
            SortOrderControlViewModel modNameSortOrder, SortOrderControlViewModel modVersionSortOrder,
            SearchModsControlViewModel filterMods, IUrlAction urlAction)
        {
            this.modService = modService;
            this.gameService = gameService;
            this.urlAction = urlAction;
            ModNameSortOrder = modNameSortOrder;
            ModVersionSortOrder = modVersionSortOrder;
            ModSelectedSortOrder = modSelectedSortOrder;
            FilterMods = filterMods;

            // Set default order and sort order text
            sortOrders = new Dictionary<string, SortOrderControlViewModel>();
            InitDefaultSortOrder(ModNameKey, modNameSortOrder, Implementation.SortOrder.Asc, ModName);
            InitDefaultSortOrder(ModVersionKey, modVersionSortOrder, Implementation.SortOrder.None, ModVersion);
            InitDefaultSortOrder(ModSelectedKey, modSelectedSortOrder, Implementation.SortOrder.None, ModSelected);
            FilterMods.WatermarkText = FilterModsWatermark;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the copy URL.
        /// </summary>
        /// <value>The copy URL.</value>
        [StaticLocalization(LocalizationResources.Mod_Url.Copy)]
        public virtual string CopyUrl { get; protected set; }

        /// <summary>
        /// Gets or sets the copy URL command.
        /// </summary>
        /// <value>The copy URL command.</value>
        public virtual ReactiveCommand<Unit, Unit> CopyUrlCommand { get; protected set; }

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
        /// Gets or sets the open URL.
        /// </summary>
        /// <value>The open URL.</value>
        [StaticLocalization(LocalizationResources.Mod_Url.Open)]
        public virtual string OpenUrl { get; protected set; }

        /// <summary>
        /// Gets or sets the open URL command.
        /// </summary>
        /// <value>The open URL command.</value>
        public virtual ReactiveCommand<Unit, Unit> OpenUrlCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the selected mods.
        /// </summary>
        /// <value>The selected mods.</value>
        public virtual IEnumerable<IMod> SelectedMods { get; protected set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [StaticLocalization(LocalizationResources.Installed_Mods.Name)]
        public virtual string Title { get; protected set; }

        #endregion Properties

        #region Methods

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
        /// Binds the specified game.
        /// </summary>
        /// <param name="game">The game.</param>
        protected virtual void Bind(IGame game = null)
        {
            if (game == null)
            {
                game = gameService.GetSelected();
            }
            if (game != null)
            {
                FilterMods.Text = string.Empty;
                Mods = FilteredMods = modService.GetInstalledMods(game).ToObservableCollection();
                SelectedMods = Mods.Where(p => p.IsSelected);

                modsChanged?.Dispose();
                modsChanged = Mods.ToSourceList().Connect().WhenAnyPropertyChanged().Subscribe(s =>
                {
                    SelectedMods = Mods.Where(p => p.IsSelected);
                }).DisposeWith(Disposables);
            }
        }

        /// <summary>
        /// Initializes the default sort order.
        /// </summary>
        /// <param name="dictKey">The dictionary key.</param>
        /// <param name="vm">The vm.</param>
        /// <param name="order">The order.</param>
        /// <param name="text">The text.</param>
        protected virtual void InitDefaultSortOrder(string dictKey, SortOrderControlViewModel vm, Implementation.SortOrder order, string text)
        {
            vm.SortOrder = order;
            vm.Text = text;
            sortOrders.Add(dictKey, vm);
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            Bind();

            Func<Func<IMod, object>, string, bool> sortFunc = (sortProp, dictKey) =>
            {
                var sortOrder = sortOrders[dictKey];
                switch (sortOrder.SortOrder)
                {
                    case Implementation.SortOrder.Asc:
                        FilteredMods = FilteredMods.OrderBy(sortProp).ToObservableCollection();
                        break;

                    case Implementation.SortOrder.Desc:
                        FilteredMods = FilteredMods.OrderByDescending(sortProp).ToObservableCollection();
                        break;

                    default:
                        break;
                }
                foreach (var sort in sortOrders.Where(p => p.Value != sortOrder))
                {
                    sort.Value.SetSortOrder(Implementation.SortOrder.None);
                }
                return true;
            };

            ModNameSortOrder.SortCommand.Subscribe(s =>
            {
                sortFunc(x => x.Name, ModNameKey);
            }).DisposeWith(disposables);

            ModVersionSortOrder.SortCommand.Subscribe(s =>
            {
                sortFunc(x => x.ActualSupportedVersion, ModVersionKey);
            }).DisposeWith(disposables);

            ModSelectedSortOrder.SortCommand.Subscribe(s =>
            {
                sortFunc(x => x.IsSelected, ModSelectedKey);
            }).DisposeWith(disposables);

            OpenUrlCommand = ReactiveCommand.Create(() =>
            {
                var url = GetHoveredModUrl();
                if (!string.IsNullOrWhiteSpace(url))
                {
                    urlAction.OpenAsync(url);
                }
            }).DisposeWith(disposables);

            CopyUrlCommand = ReactiveCommand.Create(() =>
            {
                var url = GetHoveredModUrl();
                if (!string.IsNullOrWhiteSpace(url))
                {
                    urlAction.CopyAsync(url);
                }
            }).DisposeWith(disposables);

            this.WhenAnyValue(s => s.FilterMods.Text).Subscribe(s =>
            {
                FilteredMods = Mods.Where(p => p.Name.Contains(s, StringComparison.InvariantCultureIgnoreCase));
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

        #endregion Methods
    }
}
