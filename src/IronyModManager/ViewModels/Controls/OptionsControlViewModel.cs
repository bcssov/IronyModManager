// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-30-2020
//
// Last Modified By : Mario
// Last Modified On : 07-30-2020
// ***********************************************************************
// <copyright file="OptionsControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
    /// Class OptionsControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class OptionsControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The file dialog action
        /// </summary>
        private readonly IFileDialogAction fileDialogAction;

        /// <summary>
        /// The game service
        /// </summary>
        private readonly IGameService gameService;

        /// <summary>
        /// The arguments changed
        /// </summary>
        private IDisposable argsChanged;

        /// <summary>
        /// The is reloading
        /// </summary>
        private bool isReloading = false;

        /// <summary>
        /// The refresh descriptors changed
        /// </summary>
        private IDisposable refreshDescriptorsChanged;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsControlViewModel" /> class.
        /// </summary>
        /// <param name="gameService">The game service.</param>
        /// <param name="fileDialogAction">The file dialog action.</param>
        public OptionsControlViewModel(IGameService gameService, IFileDialogAction fileDialogAction)
        {
            this.gameService = gameService;
            this.fileDialogAction = fileDialogAction;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the close.
        /// </summary>
        /// <value>The close.</value>
        [StaticLocalization(LocalizationResources.Options.Close)]
        public virtual string Close { get; protected set; }

        /// <summary>
        /// Gets or sets the close command.
        /// </summary>
        /// <value>The close command.</value>
        public virtual ReactiveCommand<Unit, Unit> CloseCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        /// <value>The game.</value>
        public virtual IGame Game { get; protected set; }

        /// <summary>
        /// Gets or sets the game arguments.
        /// </summary>
        /// <value>The game arguments.</value>
        [StaticLocalization(LocalizationResources.Options.GameArgs)]
        public virtual string GameArgs { get; protected set; }

        /// <summary>
        /// Gets or sets the game executable.
        /// </summary>
        /// <value>The game executable.</value>
        [StaticLocalization(LocalizationResources.Options.GameExecutable)]
        public virtual string GameExecutable { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public virtual bool IsOpen { get; protected set; }

        /// <summary>
        /// Gets or sets the navigate.
        /// </summary>
        /// <value>The navigate.</value>
        [StaticLocalization(LocalizationResources.Options.NavigateToExe)]
        public virtual string Navigate { get; protected set; }

        /// <summary>
        /// Gets or sets the navigate command.
        /// </summary>
        /// <value>The navigate command.</value>
        public virtual ReactiveCommand<Unit, Unit> NavigateCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the navigate title.
        /// </summary>
        /// <value>The navigate title.</value>
        [StaticLocalization(LocalizationResources.Options.Dialog.Title)]
        public virtual string NavigateTitle { get; protected set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>The options.</value>
        [StaticLocalization(LocalizationResources.Options.Name)]
        public virtual string Options { get; protected set; }

        /// <summary>
        /// Gets or sets the options command.
        /// </summary>
        /// <value>The options command.</value>
        public virtual ReactiveCommand<Unit, Unit> OptionsCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the refresh descriptors.
        /// </summary>
        /// <value>The refresh descriptors.</value>
        [StaticLocalization(LocalizationResources.Options.RefreshMods)]
        public virtual string RefreshDescriptors { get; protected set; }

        /// <summary>
        /// Gets or sets the reset.
        /// </summary>
        /// <value>The reset.</value>
        [StaticLocalization(LocalizationResources.Options.Reset)]
        public virtual string Reset { get; protected set; }

        /// <summary>
        /// Gets or sets the reset arguments command.
        /// </summary>
        /// <value>The reset arguments command.</value>
        public virtual ReactiveCommand<Unit, Unit> ResetArgsCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the reset command.
        /// </summary>
        /// <value>The reset command.</value>
        public virtual ReactiveCommand<Unit, Unit> ResetExeCommand { get; protected set; }

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
            SetGame(gameService.GetSelected());

            OptionsCommand = ReactiveCommand.Create(() =>
            {
                IsOpen = true;
            }).DisposeWith(disposables);

            CloseCommand = ReactiveCommand.Create(() =>
            {
                IsOpen = false;
            }).DisposeWith(disposables);

            NavigateCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                IsOpen = false;
                var result = await fileDialogAction.OpenDialogAsync(NavigateTitle);
                IsOpen = true;
                if (!string.IsNullOrWhiteSpace(result))
                {
                    Game.ExecutableLocation = result;
                    Save();
                }
            }).DisposeWith(disposables);

            ResetExeCommand = ReactiveCommand.Create(() =>
            {
                Game.ExecutableLocation = gameService.GetDefaultExecutableLocation(Game);
                Save();
            }).DisposeWith(disposables);

            ResetArgsCommand = ReactiveCommand.Create(() =>
            {
                Game.LaunchArguments = string.Empty;
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// Called when [selected game changed].
        /// </summary>
        /// <param name="game">The game.</param>
        protected override void OnSelectedGameChanged(IGame game)
        {
            SetGame(game);
            base.OnSelectedGameChanged(game);
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        protected virtual void Save()
        {
            var game = gameService.GetSelected();
            game.ExecutableLocation = Game.ExecutableLocation;
            game.LaunchArguments = Game.LaunchArguments;
            game.RefreshDescriptors = Game.RefreshDescriptors;
            gameService.Save(game);
            SetGame(game);
        }

        /// <summary>
        /// Sets the game.
        /// </summary>
        /// <param name="game">The game.</param>
        protected virtual void SetGame(IGame game)
        {
            isReloading = true;
            argsChanged?.Dispose();
            refreshDescriptorsChanged?.Dispose();
            Game = game;
            argsChanged = this.WhenAnyValue(p => p.Game.LaunchArguments).Where(p => !isReloading).Subscribe(s =>
            {
                Save();
            }).DisposeWith(Disposables);
            refreshDescriptorsChanged = this.WhenAnyValue(p => p.Game.RefreshDescriptors).Where(p => !isReloading).Subscribe(s =>
            {
                Save();
            }).DisposeWith(Disposables);
            isReloading = false;
        }

        #endregion Methods
    }
}
