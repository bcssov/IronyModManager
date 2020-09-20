// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-30-2020
//
// Last Modified By : Mario
// Last Modified On : 09-19-2020
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
using System.Threading.Tasks;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.Updater;
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
        /// The localization manager
        /// </summary>
        private readonly ILocalizationManager localizationManager;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The notification action
        /// </summary>
        private readonly INotificationAction notificationAction;

        /// <summary>
        /// The updater
        /// </summary>
        private readonly IUpdater updater;

        /// <summary>
        /// The updater service
        /// </summary>
        private readonly IUpdaterService updaterService;

        /// <summary>
        /// The arguments changed
        /// </summary>
        private IDisposable argsChanged;

        /// <summary>
        /// The automatic update changed
        /// </summary>
        private IDisposable autoUpdateChanged;

        /// <summary>
        /// The check for prerelease changed
        /// </summary>
        private IDisposable checkForPrereleaseChanged;

        /// <summary>
        /// The is game reloading
        /// </summary>
        private bool isGameReloading = false;

        /// <summary>
        /// The is update reloading
        /// </summary>
        private bool isUpdateReloading = false;

        /// <summary>
        /// The refresh descriptors changed
        /// </summary>
        private IDisposable refreshDescriptorsChanged;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsControlViewModel" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="notificationAction">The notification action.</param>
        /// <param name="localizationManager">The localization manager.</param>
        /// <param name="updater">The updater.</param>
        /// <param name="updaterService">The updater service.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="fileDialogAction">The file dialog action.</param>
        public OptionsControlViewModel(ILogger logger, INotificationAction notificationAction, ILocalizationManager localizationManager, IUpdater updater,
            IUpdaterService updaterService, IGameService gameService, IFileDialogAction fileDialogAction)
        {
            this.gameService = gameService;
            this.fileDialogAction = fileDialogAction;
            this.updaterService = updaterService;
            this.updater = updater;
            this.localizationManager = localizationManager;
            this.notificationAction = notificationAction;
            this.logger = logger;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the automatic update.
        /// </summary>
        /// <value>The automatic update.</value>
        [StaticLocalization(LocalizationResources.Options.Updates.AutoUpdates)]
        public virtual string AutoUpdate { get; protected set; }

        /// <summary>
        /// Gets or sets the changelog.
        /// </summary>
        /// <value>The changelog.</value>
        public virtual string Changelog { get; protected set; }

        /// <summary>
        /// Gets or sets the check for prerelease.
        /// </summary>
        /// <value>The check for prerelease.</value>
        [StaticLocalization(LocalizationResources.Options.Updates.CheckPrerelease)]
        public virtual string CheckForPrerelease { get; protected set; }

        /// <summary>
        /// Gets or sets the check for updates.
        /// </summary>
        /// <value>The check for updates.</value>
        [StaticLocalization(LocalizationResources.Options.Updates.CheckForUpdates)]
        public virtual string CheckForUpdates { get; protected set; }

        /// <summary>
        /// Gets or sets the check for updates command.
        /// </summary>
        /// <value>The check for updates command.</value>
        public virtual ReactiveCommand<Unit, Unit> CheckForUpdatesCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [checking for updates].
        /// </summary>
        /// <value><c>true</c> if [checking for updates]; otherwise, <c>false</c>.</value>
        public virtual bool CheckingForUpdates { get; protected set; }

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
        [StaticLocalization(LocalizationResources.Options.Game.GameArgs)]
        public virtual string GameArgs { get; protected set; }

        /// <summary>
        /// Gets or sets the game executable.
        /// </summary>
        /// <value>The game executable.</value>
        [StaticLocalization(LocalizationResources.Options.Game.GameExecutable)]
        public virtual string GameExecutable { get; protected set; }

        /// <summary>
        /// Gets or sets the game options.
        /// </summary>
        /// <value>The game options.</value>
        [StaticLocalization(LocalizationResources.Options.Game.Title)]
        public virtual string GameOptions { get; protected set; }

        /// <summary>
        /// Gets or sets the install updates.
        /// </summary>
        /// <value>The install updates.</value>
        [StaticLocalization(LocalizationResources.Options.Updates.Install)]
        public virtual string InstallUpdates { get; protected set; }

        /// <summary>
        /// Gets or sets the install updates command.
        /// </summary>
        /// <value>The install updates command.</value>
        public virtual ReactiveCommand<Unit, Unit> InstallUpdatesCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public virtual bool IsOpen { get; protected set; }

        /// <summary>
        /// Gets or sets the navigate.
        /// </summary>
        /// <value>The navigate.</value>
        [StaticLocalization(LocalizationResources.Options.Game.NavigateToExe)]
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
        [StaticLocalization(LocalizationResources.Options.Game.RefreshMods)]
        public virtual string RefreshDescriptors { get; protected set; }

        /// <summary>
        /// Gets or sets the reset.
        /// </summary>
        /// <value>The reset.</value>
        [StaticLocalization(LocalizationResources.Options.Game.Reset)]
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

        /// <summary>
        /// Gets or sets a value indicating whether [update information visible].
        /// </summary>
        /// <value><c>true</c> if [update information visible]; otherwise, <c>false</c>.</value>
        public virtual bool UpdateInfoVisible { get; protected set; }

        /// <summary>
        /// Gets or sets the update options.
        /// </summary>
        /// <value>The update options.</value>
        [StaticLocalization(LocalizationResources.Options.Updates.Title)]
        public virtual string UpdateOptions { get; protected set; }

        /// <summary>
        /// Gets or sets the update settings.
        /// </summary>
        /// <value>The update settings.</value>
        public virtual IUpdateSettings UpdateSettings { get; protected set; }

        /// <summary>
        /// Gets or sets the content of the version.
        /// </summary>
        /// <value>The content of the version.</value>
        public virtual string VersionContent { get; protected set; }

        /// <summary>
        /// Gets or sets the version title.
        /// </summary>
        /// <value>The version title.</value>
        [StaticLocalization(LocalizationResources.Options.Updates.Version)]
        public virtual string VersionTitle { get; protected set; }

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
        /// check for updates as an asynchronous operation.
        /// </summary>
        /// <param name="autoUpdateCheck">if set to <c>true</c> [automatic update check].</param>
        protected virtual async Task CheckForUpdatesAsync(bool autoUpdateCheck = false)
        {
            CheckingForUpdates = true;
            UpdateInfoVisible = false;
            var updatesAvailable = await updater.CheckForUpdatesAsync();
            if (updatesAvailable)
            {
                Changelog = updater.GetChangeLog();
                VersionContent = updater.GetVersion();
                UpdateInfoVisible = true;
                if (autoUpdateCheck)
                {
                    var title = localizationManager.GetResource(LocalizationResources.Options.Updates.UpdateNotification.Title);
                    var message = localizationManager.GetResource(LocalizationResources.Options.Updates.UpdateNotification.Message);
                    notificationAction.ShowNotification(title, message, NotificationType.Info, onClick: () => { IsOpen = true; });
                }
            }
            else
            {
                if (!autoUpdateCheck)
                {
                    var title = localizationManager.GetResource(LocalizationResources.Notifications.NoUpdatesAvailable.Title);
                    var message = localizationManager.GetResource(LocalizationResources.Notifications.NoUpdatesAvailable.Message);
                    notificationAction.ShowNotification(title, message, NotificationType.Info, 10);
                }
            }
            CheckingForUpdates = false;
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            SetGame(gameService.GetSelected());
            var updateSettings = updaterService.Get();
            if (updateSettings.AutoUpdates == null)
            {
                async Task showPrompt()
                {
                    var title = localizationManager.GetResource(LocalizationResources.Options.Updates.AutoUpdatePrompts.Title);
                    var message = localizationManager.GetResource(LocalizationResources.Options.Updates.AutoUpdatePrompts.Message);
                    if (await notificationAction.ShowPromptAsync(title, title, message, NotificationType.Info))
                    {
                        updateSettings.AutoUpdates = true;
                        SaveUpdateSettings();
                        await CheckForUpdatesAsync(true);
                    }
                    else
                    {
                        updateSettings.AutoUpdates = false;
                        SaveUpdateSettings();
                    }
                }
                showPrompt().ConfigureAwait(false);
            }
            else if (updateSettings.AutoUpdates.GetValueOrDefault())
            {
                CheckForUpdatesAsync(true).ConfigureAwait(false);
            }
            SetUpdateSettings(updateSettings);

            var updateCheckAllowed = this.WhenAnyValue(p => p.CheckingForUpdates, v => !v);

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
                    if (string.IsNullOrWhiteSpace(Game.LaunchArguments))
                    {
                        Game.LaunchArguments = gameService.GetDefaultGameSettings(Game).LaunchArguments;
                    }
                    SaveGame();
                }
            }).DisposeWith(disposables);

            ResetExeCommand = ReactiveCommand.Create(() =>
            {
                Game.ExecutableLocation = gameService.GetDefaultGameSettings(Game).ExecutableLocation;
                if (string.IsNullOrWhiteSpace(Game.LaunchArguments))
                {
                    Game.LaunchArguments = gameService.GetDefaultGameSettings(Game).LaunchArguments;
                }
                SaveGame();
            }).DisposeWith(disposables);

            ResetArgsCommand = ReactiveCommand.Create(() =>
            {
                Game.LaunchArguments = gameService.GetDefaultGameSettings(Game).LaunchArguments;
            }).DisposeWith(disposables);

            CheckForUpdatesCommand = ReactiveCommand.CreateFromTask(() =>
            {
                return CheckForUpdatesAsync();
            }, updateCheckAllowed).DisposeWith(disposables);

            var downloadingUpdates = false;
            InstallUpdatesCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                downloadingUpdates = true;
                await TriggerOverlayAsync(true, localizationManager.GetResource(LocalizationResources.Options.Updates.Overlay.UpdateDownloading));
                await updater.DownloadUpdateAsync();
                downloadingUpdates = false;
                await TriggerOverlayAsync(true, localizationManager.GetResource(LocalizationResources.Options.Updates.Overlay.UpdateInstalling));
                await updater.InstallUpdateAsync();
            }).DisposeWith(disposables);

            updater.Error.Subscribe(s =>
            {
                var title = localizationManager.GetResource(LocalizationResources.Options.Updates.Errors.DownloadErrorTitle);
                var message = localizationManager.GetResource(LocalizationResources.Options.Updates.Errors.DownloadErrorMessage);
                logger.Error(s);
                notificationAction.ShowNotification(title, message, NotificationType.Error, 30);
                TriggerOverlay(false);
            }).DisposeWith(disposables);

            updater.Progress.Subscribe(s =>
            {
                if (downloadingUpdates)
                {
                    var message = localizationManager.GetResource(LocalizationResources.Options.Updates.Overlay.UpdateDownloading);
                    var progress = Smart.Format(localizationManager.GetResource(LocalizationResources.Options.Updates.Overlay.UpdateInstalling), new { Progress = s });
                    TriggerOverlay(true, message, progress);
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
            SetGame(game);
            base.OnSelectedGameChanged(game);
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        protected virtual void SaveGame()
        {
            var game = gameService.GetSelected();
            game.ExecutableLocation = Game.ExecutableLocation;
            game.LaunchArguments = Game.LaunchArguments;
            game.RefreshDescriptors = Game.RefreshDescriptors;
            gameService.Save(game);
            SetGame(game);
        }

        /// <summary>
        /// Saves the update settings.
        /// </summary>
        protected virtual void SaveUpdateSettings()
        {
            var settings = updaterService.Get();
            settings.AutoUpdates = UpdateSettings.AutoUpdates;
            settings.CheckForPrerelease = UpdateSettings.CheckForPrerelease;
            updaterService.Save(settings);
            SetUpdateSettings(settings);
        }

        /// <summary>
        /// Sets the game.
        /// </summary>
        /// <param name="game">The game.</param>
        protected virtual void SetGame(IGame game)
        {
            isGameReloading = true;
            argsChanged?.Dispose();
            refreshDescriptorsChanged?.Dispose();
            Game = game;
            argsChanged = this.WhenAnyValue(p => p.Game.LaunchArguments).Where(p => !isGameReloading).Subscribe(s =>
            {
                SaveGame();
            }).DisposeWith(Disposables);
            refreshDescriptorsChanged = this.WhenAnyValue(p => p.Game.RefreshDescriptors).Where(p => !isGameReloading).Subscribe(s =>
            {
                SaveGame();
            }).DisposeWith(Disposables);
            isGameReloading = false;
        }

        /// <summary>
        /// Sets the update settings.
        /// </summary>
        /// <param name="updateSettings">The update settings.</param>
        protected virtual void SetUpdateSettings(IUpdateSettings updateSettings)
        {
            isUpdateReloading = true;
            autoUpdateChanged?.Dispose();
            checkForPrereleaseChanged?.Dispose();
            UpdateSettings = updateSettings;
            autoUpdateChanged = this.WhenAnyValue(p => p.UpdateSettings.AutoUpdates).Where(v => !isUpdateReloading).Subscribe(s =>
            {
                SaveUpdateSettings();
            }).DisposeWith(Disposables);
            checkForPrereleaseChanged = this.WhenAnyValue(p => p.UpdateSettings.CheckForPrerelease).Where(v => !isUpdateReloading).Subscribe(s =>
            {
                UpdateInfoVisible = false;
                SaveUpdateSettings();
            }).DisposeWith(Disposables);
            isUpdateReloading = false;
        }

        #endregion Methods
    }
}
