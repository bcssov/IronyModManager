// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-15-2021
//
// Last Modified By : Mario
// Last Modified On : 03-09-2025
// ***********************************************************************
// <copyright file="DLCManagerControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using IronyModManager.Common.ViewModels;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class DLCManagerControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class DLCManagerControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The DLC service
        /// </summary>
        private readonly IDLCService dlcService;

        /// <summary>
        /// The game service
        /// </summary>
        private readonly IGameService gameService;

        /// <summary>
        /// The selected game
        /// </summary>
        private IGame selectedGame;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DLCManagerControlViewModel" /> class.
        /// </summary>
        /// <param name="dlcService">The DLC service.</param>
        /// <param name="gameService">The game service.</param>
        public DLCManagerControlViewModel(IDLCService dlcService, IGameService gameService)
        {
            this.dlcService = dlcService;
            this.gameService = gameService;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the close.
        /// </summary>
        /// <value>The close.</value>
        [StaticLocalization(LocalizationResources.App.Actions.Close)]
        public virtual string Close { get; protected set; }

        /// <summary>
        /// Gets or sets the close command.
        /// </summary>
        /// <value>The close command.</value>
        public virtual ReactiveCommand<Unit, Unit> CloseCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the DLC.
        /// </summary>
        /// <value>The DLC.</value>
        public virtual IReadOnlyCollection<IDLC> DLC { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public virtual bool IsOpen { get; protected set; }

        /// <summary>
        /// Gets or sets the toggle DLC.
        /// </summary>
        /// <value>The toggle DLC.</value>
        [StaticLocalization(LocalizationResources.DLC.Toggle)]
        public virtual string ToggleDLC { get; protected set; }

        /// <summary>
        /// Gets or sets the toggle DLC command.
        /// </summary>
        /// <value>The toggle DLC command.</value>
        public virtual ReactiveCommand<Unit, Unit> ToggleDLCCommand { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Forces the close.
        /// </summary>
        public virtual void ForceClose()
        {
            IsOpen = false;
        }

        /// <summary>
        /// Opens this instance.
        /// </summary>
        public virtual void Open()
        {
            IsOpen = true;
        }

        /// <summary>
        /// refresh DLC as an asynchronous operation.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public virtual async Task RefreshDLCAsync(IGame game)
        {
            selectedGame = game;
            var dlc = await dlcService.GetAsync(selectedGame);
            await dlcService.SyncStateAsync(selectedGame, dlc);
            DLC = dlc?.OrderBy(p => p.Name).ToList();
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            CloseCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await SaveDLCAsync(gameService.GetSelected(), selectedGame);
                ForceClose();
            }).DisposeWith(disposables);

            ToggleDLCCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (DLC != null && DLC.Count() != 0)
                {
                    var isEnabled = !DLC.All(p => p.IsEnabled);
                    foreach (var dlc in DLC)
                    {
                        dlc.IsEnabled = isEnabled;
                    }

                    await SaveDLCAsync(gameService.GetSelected(), selectedGame);
                }
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// save DLC as an asynchronous operation.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="selectedGame">The selected game.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected virtual async Task SaveDLCAsync(IGame game, IGame selectedGame)
        {
            if (game != null && selectedGame != null && game.Type.Equals(selectedGame.Type))
            {
                await dlcService.ExportAsync(gameService.GetSelected(), DLC);
            }
        }

        #endregion Methods
    }
}
