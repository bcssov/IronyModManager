// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 05-30-2020
// ***********************************************************************
// <copyright file="MainControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation.Actions;
using IronyModManager.Localization.Attributes;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;
using ReactiveUI;

namespace IronyModManager.ViewModels
{
    /// <summary>
    /// Class MainControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class MainControlViewModel : BaseViewModel
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

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainControlViewModel" /> class.
        /// </summary>
        /// <param name="themeControl">The theme control.</param>
        /// <param name="languageControl">The language control.</param>
        /// <param name="gameControl">The game control.</param>
        /// <param name="modControl">The mod control.</param>
        /// <param name="options">The options.</param>
        /// <param name="appAction">The URL action.</param>
        /// <param name="gameService">The game service.</param>
        public MainControlViewModel(ThemeControlViewModel themeControl,
            LanguageControlViewModel languageControl,
            GameControlViewModel gameControl,
            ModHolderControlViewModel modControl,
            OptionsControlViewModel options,
            IAppAction appAction, IGameService gameService)
        {
            ThemeSelector = themeControl;
            LanguageSelector = languageControl;
            GameSelector = gameControl;
            ModHolder = modControl;
            Options = options;
            this.appAction = appAction;
            this.gameService = gameService;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the error log.
        /// </summary>
        /// <value>The error log.</value>
        [StaticLocalization(LocalizationResources.App.Shortcuts.ErrorLog)]
        public virtual string ErrorLog { get; protected set; }

        /// <summary>
        /// Gets or sets the error log command.
        /// </summary>
        /// <value>The error log command.</value>
        public virtual ReactiveCommand<Unit, Unit> ErrorLogCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the game selector.
        /// </summary>
        /// <value>The game selector.</value>
        public virtual GameControlViewModel GameSelector { get; protected set; }

        /// <summary>
        /// Gets or sets the language selector.
        /// </summary>
        /// <value>The language selector.</value>
        public virtual LanguageControlViewModel LanguageSelector { get; protected set; }

        /// <summary>
        /// Gets or sets the logs.
        /// </summary>
        /// <value>The logs.</value>
        [StaticLocalization(LocalizationResources.App.Shortcuts.Logs)]
        public virtual string Logs { get; protected set; }

        /// <summary>
        /// Gets or sets the logs command.
        /// </summary>
        /// <value>The logs command.</value>
        public virtual ReactiveCommand<Unit, Unit> LogsCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the mod holder.
        /// </summary>
        /// <value>The mod holder.</value>
        public virtual ModHolderControlViewModel ModHolder { get; protected set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>The options.</value>
        public virtual OptionsControlViewModel Options { get; protected set; }

        /// <summary>
        /// Gets or sets the shortcuts.
        /// </summary>
        /// <value>The shortcuts.</value>
        [StaticLocalization(LocalizationResources.App.Shortcuts.Name)]
        public virtual string Shortcuts { get; protected set; }

        /// <summary>
        /// Gets the theme selector.
        /// </summary>
        /// <value>The theme selector.</value>
        public virtual ThemeControlViewModel ThemeSelector { get; protected set; }

        /// <summary>
        /// Gets or sets the wiki.
        /// </summary>
        /// <value>The wiki.</value>
        [StaticLocalization(LocalizationResources.App.Shortcuts.Wiki)]
        public virtual string Wiki { get; protected set; }

        /// <summary>
        /// Gets or sets the wiki command.
        /// </summary>
        /// <value>The wiki command.</value>
        public virtual ReactiveCommand<Unit, Unit> WikiCommand { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
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

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
