// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 07-30-2020
//
// Last Modified By : Mario
// Last Modified On : 02-14-2021
// ***********************************************************************
// <copyright file="ActionsControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation.Actions;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class ActionsControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ActionsControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The URL action
        /// </summary>
        private readonly IAppAction appAction;

        /// <summary>
        /// The DLC service
        /// </summary>
        private readonly IDLCService dlcService;

        /// <summary>
        /// The game service
        /// </summary>
        private readonly IGameService gameService;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortcutsControlViewModel" /> class.
        /// </summary>
        /// <param name="dlcService">The DLC service.</param>
        /// <param name="appAction">The application action.</param>
        /// <param name="gameService">The game service.</param>
        public ActionsControlViewModel(IDLCService dlcService, IAppAction appAction, IGameService gameService)
        {
            this.dlcService = dlcService;
            this.appAction = appAction;
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
        /// Gets or sets the error log.
        /// </summary>
        /// <value>The error log.</value>
        [StaticLocalization(LocalizationResources.App.Actions.ErrorLog)]
        public virtual string ErrorLog { get; protected set; }

        /// <summary>
        /// Gets or sets the error log command.
        /// </summary>
        /// <value>The error log command.</value>
        public virtual ReactiveCommand<Unit, Unit> ErrorLogCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public virtual bool IsOpen { get; protected set; }

        /// <summary>
        /// Gets or sets the logs.
        /// </summary>
        /// <value>The logs.</value>
        [StaticLocalization(LocalizationResources.App.Actions.Logs)]
        public virtual string Logs { get; protected set; }

        /// <summary>
        /// Gets or sets the logs command.
        /// </summary>
        /// <value>The logs command.</value>
        public virtual ReactiveCommand<Unit, Unit> LogsCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the shortcuts.
        /// </summary>
        /// <value>The shortcuts.</value>
        [StaticLocalization(LocalizationResources.App.Actions.Name)]
        public virtual string Shortcuts { get; protected set; }

        /// <summary>
        /// Gets or sets the shortcuts command.
        /// </summary>
        /// <value>The shortcuts command.</value>
        public virtual ReactiveCommand<Unit, Unit> ShortcutsCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the wiki.
        /// </summary>
        /// <value>The wiki.</value>
        [StaticLocalization(LocalizationResources.App.Actions.Wiki)]
        public virtual string Wiki { get; protected set; }

        /// <summary>
        /// Gets or sets the wiki command.
        /// </summary>
        /// <value>The wiki command.</value>
        public virtual ReactiveCommand<Unit, Unit> WikiCommand { get; protected set; }

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
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            Task.Run(async () => await SetDLCAsync(gameService.GetSelected()).ConfigureAwait(false)).ConfigureAwait(false);

            WikiCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await appAction.OpenAsync(Constants.WikiUrl);
            }).DisposeWith(disposables);

            LogsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (Directory.Exists(Constants.LogsLocation))
                {
                    await appAction.OpenAsync(Constants.LogsLocation);
                }
            }).DisposeWith(disposables);

            ErrorLogCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var game = gameService.GetSelected();
                if (game != null)
                {
                    if (File.Exists(game.LogLocation))
                    {
                        await appAction.OpenAsync(game.LogLocation);
                    }
                }
            }).DisposeWith(disposables);

            ShortcutsCommand = ReactiveCommand.Create(() =>
            {
                IsOpen = true;
            }).DisposeWith(disposables);

            CloseCommand = ReactiveCommand.Create(() =>
            {
                IsOpen = false;
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// Called when [selected game changed].
        /// </summary>
        /// <param name="game">The game.</param>
        protected override void OnSelectedGameChanged(IGame game)
        {
            Task.Run(async () => await SetDLCAsync(game).ConfigureAwait(false)).ConfigureAwait(false);
            base.OnSelectedGameChanged(game);
        }

        /// <summary>
        /// set DLC as an asynchronous operation.
        /// </summary>
        /// <param name="game">The game.</param>
        protected virtual async Task SetDLCAsync(IGame game)
        {
            var dlc = await dlcService.GetAsync(game);
        }

        #endregion Methods
    }
}
