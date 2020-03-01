// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 03-01-2020
// ***********************************************************************
// <copyright file="InstalledModsControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using DynamicData;
using IronyModManager.Common;
using IronyModManager.Common.ViewModels;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;

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
        /// The game service
        /// </summary>
        private readonly IGameService gameService;

        /// <summary>
        /// The mod service
        /// </summary>
        private readonly IModService modService;

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
        /// <param name="sortOrderControl">The sort order control.</param>
        public InstalledModsControlViewModel(IGameService gameService, IModService modService, SortOrderControlViewModel sortOrderControl)
        {
            this.modService = modService;
            this.gameService = gameService;
            SortOrder = sortOrderControl;
            // Set default order and sort order text
            SortOrder.SortOrder = Implementation.SortOrder.Asc;
            SortOrder.Text = ModName;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the name of the mod.
        /// </summary>
        /// <value>The name of the mod.</value>
        [StaticLocalization(LocalizationResources.Installed_Mods.Mod_Name)]
        public virtual string ModName { get; protected set; }

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
        /// Gets or sets the selected mods.
        /// </summary>
        /// <value>The selected mods.</value>
        public virtual IEnumerable<IMod> SelectedMods { get; protected set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        public virtual SortOrderControlViewModel SortOrder { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [locale changed].
        /// </summary>
        /// <param name="newLocale">The new locale.</param>
        /// <param name="oldLocale">The old locale.</param>
        public override void OnLocaleChanged(string newLocale, string oldLocale)
        {
            SortOrder.Text = ModName;

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
                Mods = modService.GetInstalledMods(game).ToObservableCollection();
                SelectedMods = Mods.Where(p => p.IsSelected);

                modsChanged?.Dispose();
                modsChanged = Mods.ToSourceList().Connect().WhenAnyPropertyChanged().Subscribe(s =>
                {
                    SelectedMods = Mods.Where(p => p.IsSelected);
                }).DisposeWith(Disposables);
            }
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            Bind();

            SortOrder.SortCommand.Subscribe(s =>
            {
                switch (SortOrder.SortOrder)
                {
                    case Implementation.SortOrder.Asc:
                        Mods = Mods.OrderBy(s => s.Name).ToObservableCollection();
                        break;

                    case Implementation.SortOrder.Desc:
                        Mods = Mods.OrderByDescending(s => s.Name).ToObservableCollection();
                        break;

                    default:
                        break;
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

        #endregion Methods
    }
}
