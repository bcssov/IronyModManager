// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 07-30-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2022
// ***********************************************************************
// <copyright file="ActionsControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.MessageBus;
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
        /// The game service
        /// </summary>
        private readonly IGameService gameService;

        /// <summary>
        /// The game executable changed handler
        /// </summary>
        private GameExeChangedHandler gameExeChangedHandler;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortcutsControlViewModel" /> class.
        /// </summary>
        /// <param name="gameExeChangedHandler">The game executable changed handler.</param>
        /// <param name="dlcManager">The DLC manager.</param>
        /// <param name="appAction">The application action.</param>
        /// <param name="gameService">The game service.</param>
        public ActionsControlViewModel(GameExeChangedHandler gameExeChangedHandler, DLCManagerControlViewModel dlcManager, IAppAction appAction, IGameService gameService)
        {
            this.appAction = appAction;
            this.gameService = gameService;
            DLCManager = dlcManager;
            this.gameExeChangedHandler = gameExeChangedHandler;
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
        [StaticLocalization(LocalizationResources.App.Actions.DLC)]
        public virtual string DLC { get; protected set; }

        /// <summary>
        /// Gets or sets the DLC command.
        /// </summary>
        /// <value>The DLC command.</value>
        public virtual ReactiveCommand<Unit, Unit> DLCCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the DLC manager.
        /// </summary>
        /// <value>The DLC manager.</value>
        public virtual DLCManagerControlViewModel DLCManager { get; protected set; }

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
        /// Gets or sets the error log visible.
        /// </summary>
        /// <value>The error log visible.</value>
        public virtual bool ErrorLogVisible { get; protected set; }

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
        /// Gets or sets the logs visible.
        /// </summary>
        /// <value>The logs visible.</value>
        public virtual bool LogsVisible { get; protected set; }

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
            Task.Run(async () => await DLCManager.RefreshDLCAsync(gameService.GetSelected()).ConfigureAwait(false)).ConfigureAwait(false);

            WikiCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await appAction.OpenAsync(Constants.WikiUrl);
            }).DisposeWith(disposables);

            LogsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (Directory.Exists(StaticResources.GetLogLocation()))
                {
                    await appAction.OpenAsync(StaticResources.GetLogLocation());
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

            DLCCommand = ReactiveCommand.Create(() =>
            {
                IsOpen = false;
                DLCManager.Open();
            }).DisposeWith(disposables);

            void evaluateVisibility()
            {
                LogsVisible = Directory.Exists(StaticResources.GetLogLocation());
                var game = gameService.GetSelected();
                ErrorLogVisible = game != null && File.Exists(game.LogLocation);
            }

            evaluateVisibility();
            this.WhenAnyValue(p => p.IsOpen).Where(p => p).Subscribe(s =>
            {
                evaluateVisibility();
            }).DisposeWith(disposables);

            gameExeChangedHandler.Subscribe(s =>
            {
                Task.Run(async () => await DLCManager.RefreshDLCAsync(gameService.GetSelected()).ConfigureAwait(false)).ConfigureAwait(false);
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// Called when [selected game changed].
        /// </summary>
        /// <param name="game">The game.</param>
        protected override void OnSelectedGameChanged(IGame game)
        {
            Task.Run(async () => await DLCManager.RefreshDLCAsync(gameService.GetSelected()).ConfigureAwait(false)).ConfigureAwait(false);
            base.OnSelectedGameChanged(game);
        }

        #endregion Methods
    }
}
