// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-30-2020
//
// Last Modified By : Mario
// Last Modified On : 11-04-2021
// ***********************************************************************
// <copyright file="OptionsControlViewModel.cs" company="Mario">
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
using Avalonia;
using IronyModManager.Common;
using IronyModManager.Common.Events;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.Overlay;
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
        /// The external editor service
        /// </summary>
        private readonly IExternalEditorService externalEditorService;

        /// <summary>
        /// The file dialog action
        /// </summary>
        private readonly IFileDialogAction fileDialogAction;

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
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The mod service
        /// </summary>
        private readonly IModService modService;

        /// <summary>
        /// The notification action
        /// </summary>
        private readonly INotificationAction notificationAction;

        /// <summary>
        /// The position settings service
        /// </summary>
        private readonly INotificationPositionSettingsService positionSettingsService;

        /// <summary>
        /// The updater
        /// </summary>
        private readonly IUpdater updater;

        /// <summary>
        /// The updater service
        /// </summary>
        private readonly IUpdaterService updaterService;

        /// <summary>
        /// The automatic update changed
        /// </summary>
        private IDisposable autoUpdateChanged;

        /// <summary>
        /// The check for prerelease changed
        /// </summary>
        private IDisposable checkForPrereleaseChanged;

        /// <summary>
        /// The close game changed
        /// </summary>
        private IDisposable closeGameChanged;

        /// <summary>
        /// The editor arguments changed
        /// </summary>
        private IDisposable editorArgsChanged;

        /// <summary>
        /// The arguments changed
        /// </summary>
        private IDisposable gameArgsChanged;

        /// <summary>
        /// The is editor reloading
        /// </summary>
        private bool isEditorReloading = false;

        /// <summary>
        /// The is game reloading
        /// </summary>
        private bool isGameReloading = false;

        /// <summary>
        /// The is notification position reloading
        /// </summary>
        private bool isNotificationPositionReloading = false;

        /// <summary>
        /// The is update reloading
        /// </summary>
        private bool isUpdateReloading = false;

        /// <summary>
        /// The notification position changed
        /// </summary>
        private IDisposable notificationPositionChanged;

        /// <summary>
        /// The refresh descriptors changed
        /// </summary>
        private IDisposable refreshDescriptorsChanged;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsControlViewModel" /> class.
        /// </summary>
        /// <param name="modService">The mod service.</param>
        /// <param name="positionSettingsService">The position settings service.</param>
        /// <param name="externalEditorService">The external editor service.</param>
        /// <param name="idGenerator">The identifier generator.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="notificationAction">The notification action.</param>
        /// <param name="localizationManager">The localization manager.</param>
        /// <param name="updater">The updater.</param>
        /// <param name="updaterService">The updater service.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="fileDialogAction">The file dialog action.</param>
        public OptionsControlViewModel(IModService modService, INotificationPositionSettingsService positionSettingsService,
            IExternalEditorService externalEditorService, IIDGenerator idGenerator, ILogger logger,
            INotificationAction notificationAction, ILocalizationManager localizationManager, IUpdater updater,
            IUpdaterService updaterService, IGameService gameService, IFileDialogAction fileDialogAction)
        {
            this.positionSettingsService = positionSettingsService;
            this.gameService = gameService;
            this.fileDialogAction = fileDialogAction;
            this.updaterService = updaterService;
            this.updater = updater;
            this.localizationManager = localizationManager;
            this.notificationAction = notificationAction;
            this.logger = logger;
            this.idGenerator = idGenerator;
            this.externalEditorService = externalEditorService;
            this.modService = modService;
            LeftMargin = new Thickness(20, 0, 0, 0);
            LeftChildMargin = new Thickness(20, 10, 0, 0);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the application options title.
        /// </summary>
        /// <value>The application options title.</value>
        [StaticLocalization(LocalizationResources.Options.AppOptions.Title)]
        public virtual string AppOptionsTitle { get; protected set; }

        /// <summary>
        /// Gets or sets the automatic configure.
        /// </summary>
        /// <value>The automatic configure.</value>
        [StaticLocalization(LocalizationResources.Options.Game.AutoConfigure)]
        public virtual string AutoConfigure { get; protected set; }

        /// <summary>
        /// Gets or sets the automatic configure command.
        /// </summary>
        /// <value>The automatic configure command.</value>
        public virtual ReactiveCommand<Unit, Unit> AutoConfigureCommand { get; protected set; }

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
        /// Gets or sets the close application after game launch.
        /// </summary>
        /// <value>The close application after game launch.</value>
        [StaticLocalization(LocalizationResources.Options.Game.CloseAfterLaunch)]
        public virtual string CloseAppAfterGameLaunch { get; protected set; }

        /// <summary>
        /// Gets or sets the close command.
        /// </summary>
        /// <value>The close command.</value>
        public virtual ReactiveCommand<Unit, Unit> CloseCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the custom mod path.
        /// </summary>
        /// <value>The custom mod path.</value>
        [StaticLocalization(LocalizationResources.Options.Game.CustomModPath)]
        public virtual string CustomModPath { get; protected set; }

        /// <summary>
        /// Gets or sets the editor.
        /// </summary>
        /// <value>The editor.</value>
        public virtual IExternalEditor Editor { get; protected set; }

        /// <summary>
        /// Gets or sets the editor arguments.
        /// </summary>
        /// <value>The editor arguments.</value>
        [StaticLocalization(LocalizationResources.Options.Editor.EditorArgs)]
        public virtual string EditorArgs { get; protected set; }

        /// <summary>
        /// Gets or sets the editor arguments placeholder.
        /// </summary>
        /// <value>The editor arguments placeholder.</value>
        [StaticLocalization(LocalizationResources.Options.Editor.EditorArgsPlaceholder)]
        public virtual string EditorArgsPlaceholder { get; protected set; }

        /// <summary>
        /// Gets or sets the editor executable.
        /// </summary>
        /// <value>The editor executable.</value>
        [StaticLocalization(LocalizationResources.Options.Editor.EditorExecutable)]
        public virtual string EditorExecutable { get; protected set; }

        /// <summary>
        /// Gets or sets the editor title.
        /// </summary>
        /// <value>The editor title.</value>
        [StaticLocalization(LocalizationResources.Options.Editor.Title)]
        public virtual string EditorTitle { get; protected set; }

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
        /// Gets or sets the left child margin.
        /// </summary>
        /// <value>The left child margin.</value>
        public virtual Thickness LeftChildMargin { get; protected set; }

        /// <summary>
        /// Gets or sets the left margin.
        /// </summary>
        /// <value>The left margin.</value>
        public virtual Thickness LeftMargin { get; protected set; }

        /// <summary>
        /// Gets or sets the navigate.
        /// </summary>
        /// <value>The navigate.</value>
        [StaticLocalization(LocalizationResources.Options.NavigateTo)]
        public virtual string Navigate { get; protected set; }

        /// <summary>
        /// Gets or sets the navigate custom directory command.
        /// </summary>
        /// <value>The navigate custom directory command.</value>
        public virtual ReactiveCommand<Unit, Unit> NavigateCustomDirectoryCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the navigate custom user dir title.
        /// </summary>
        /// <value>The navigate custom user dir title.</value>
        [StaticLocalization(LocalizationResources.Options.Dialog.CustomModPathTitle)]
        public virtual string NavigateCustomUserDirTitle { get; protected set; }

        /// <summary>
        /// Gets or sets the navigate directory command.
        /// </summary>
        /// <value>The navigate directory command.</value>
        public virtual ReactiveCommand<Unit, Unit> NavigateDirectoryCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the navigate editor command.
        /// </summary>
        /// <value>The navigate editor command.</value>
        public virtual ReactiveCommand<Unit, Unit> NavigateEditorCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the navigate editor title.
        /// </summary>
        /// <value>The navigate editor title.</value>
        [StaticLocalization(LocalizationResources.Options.Dialog.EditorTitle)]
        public virtual string NavigateEditorTitle { get; protected set; }

        /// <summary>
        /// Gets or sets the navigate executable command.
        /// </summary>
        /// <value>The navigate executable command.</value>
        public virtual ReactiveCommand<Unit, Unit> NavigateExeCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the navigate executable title.
        /// </summary>
        /// <value>The navigate executable title.</value>
        [StaticLocalization(LocalizationResources.Options.Dialog.ExeTitle)]
        public virtual string NavigateExeTitle { get; protected set; }

        /// <summary>
        /// Gets or sets the navigate game title.
        /// </summary>
        /// <value>The navigate game title.</value>
        [StaticLocalization(LocalizationResources.Options.Dialog.GameRootTitle)]
        public virtual string NavigateGameTitle { get; protected set; }

        /// <summary>
        /// Gets or sets the navigate user dir title.
        /// </summary>
        /// <value>The navigate user dir title.</value>
        [StaticLocalization(LocalizationResources.Options.Dialog.UserDirTitle)]
        public virtual string NavigateUserDirTitle { get; protected set; }

        /// <summary>
        /// Gets or sets the notification position.
        /// </summary>
        /// <value>The notification position.</value>
        public virtual INotificationPosition NotificationPosition { get; protected set; }

        /// <summary>
        /// Gets or sets the notification position label.
        /// </summary>
        /// <value>The notification position label.</value>
        [StaticLocalization(LocalizationResources.Options.AppOptions.NotificationPosition)]
        public virtual string NotificationPositionLabel { get; protected set; }

        /// <summary>
        /// Gets or sets the notification positions.
        /// </summary>
        /// <value>The notification positions.</value>
        public virtual IEnumerable<INotificationPosition> NotificationPositions { get; protected set; }

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
        [StaticLocalization(LocalizationResources.Options.Reset)]
        public virtual string Reset { get; protected set; }

        /// <summary>
        /// Gets or sets the reset arguments command.
        /// </summary>
        /// <value>The reset arguments command.</value>
        public virtual ReactiveCommand<Unit, Unit> ResetArgsCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the reset custom directory command.
        /// </summary>
        /// <value>The reset custom directory command.</value>
        public virtual ReactiveCommand<Unit, Unit> ResetCustomDirectoryCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the reset directory command.
        /// </summary>
        /// <value>The reset directory command.</value>
        public virtual ReactiveCommand<Unit, Unit> ResetDirectoryCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the reset editor arguments command.
        /// </summary>
        /// <value>The reset editor arguments command.</value>
        public virtual ReactiveCommand<Unit, Unit> ResetEditorArgsCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the reset editor executable command.
        /// </summary>
        /// <value>The reset editor executable command.</value>
        public virtual ReactiveCommand<Unit, Unit> ResetEditorExeCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the reset command.
        /// </summary>
        /// <value>The reset command.</value>
        public virtual ReactiveCommand<Unit, Unit> ResetExeCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show game options].
        /// </summary>
        /// <value><c>true</c> if [show game options]; otherwise, <c>false</c>.</value>
        public virtual bool ShowGameOptions { get; protected set; }

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
        /// Gets or sets the update release information.
        /// </summary>
        /// <value>The update release information.</value>
        [StaticLocalization(LocalizationResources.Options.Updates.UpdateInfoTitle)]
        public virtual string UpdateReleaseInfo { get; protected set; }

        /// <summary>
        /// Gets or sets the update settings.
        /// </summary>
        /// <value>The update settings.</value>
        public virtual IUpdateSettings UpdateSettings { get; protected set; }

        /// <summary>
        /// Gets or sets the user directory.
        /// </summary>
        /// <value>The user directory.</value>
        [StaticLocalization(LocalizationResources.Options.Game.UserDirectory)]
        public virtual string UserDirectory { get; protected set; }

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
        /// <returns>System.Threading.Tasks.Task.</returns>
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
                var openState = IsOpen;
                IsOpen = false;
                await Task.Delay(100);
                IsOpen = openState;
                if (autoUpdateCheck)
                {
                    var title = localizationManager.GetResource(LocalizationResources.Options.Updates.UpdateNotification.Title);
                    var message = localizationManager.GetResource(LocalizationResources.Options.Updates.UpdateNotification.Message);
                    notificationAction.ShowNotification(title, message, NotificationType.Info, 30, onClick: () => { IsOpen = true; });
                }
            }
            else
            {
                if (!autoUpdateCheck)
                {
                    var title = localizationManager.GetResource(LocalizationResources.Notifications.NoUpdatesAvailable.Title);
                    var message = localizationManager.GetResource(LocalizationResources.Notifications.NoUpdatesAvailable.Message);
                    notificationAction.ShowNotification(title, message, NotificationType.Info);
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
            SetEditor(externalEditorService.Get());
            SetNotificationPosition(positionSettingsService.Get());
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

            NavigateExeCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                IsOpen = false;
                var result = await fileDialogAction.OpenDialogAsync(NavigateExeTitle);
                IsOpen = true;
                if (!string.IsNullOrWhiteSpace(result))
                {
                    Game.ExecutableLocation = result;
                    if (string.IsNullOrWhiteSpace(Game.LaunchArguments) || string.IsNullOrWhiteSpace(Game.UserDirectory))
                    {
                        var defaultSettings = gameService.GetDefaultGameSettings(Game);
                        if (string.IsNullOrWhiteSpace(Game.LaunchArguments))
                        {
                            Game.LaunchArguments = defaultSettings.LaunchArguments;
                        }
                        if (string.IsNullOrWhiteSpace(Game.UserDirectory))
                        {
                            Game.UserDirectory = defaultSettings.UserDirectory;
                        }
                    }
                    SaveGame();
                }
            }).DisposeWith(disposables);

            NavigateEditorCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                IsOpen = false;
                var result = await fileDialogAction.OpenDialogAsync(NavigateEditorTitle);
                IsOpen = true;
                if (!string.IsNullOrWhiteSpace(result))
                {
                    Editor.ExternalEditorLocation = result;
                    SaveEditor();
                }
            }).DisposeWith(disposables);

            NavigateDirectoryCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                IsOpen = false;
                var result = await fileDialogAction.OpenFolderDialogAsync(NavigateUserDirTitle);
                IsOpen = true;
                if (!string.IsNullOrWhiteSpace(result))
                {
                    Game.UserDirectory = result;
                    SaveGame();
                }
            }).DisposeWith(disposables);

            ResetExeCommand = ReactiveCommand.Create(() =>
            {
                var defaultSettings = gameService.GetDefaultGameSettings(Game);
                Game.ExecutableLocation = defaultSettings.ExecutableLocation;
                if (string.IsNullOrWhiteSpace(Game.LaunchArguments))
                {
                    Game.LaunchArguments = defaultSettings.LaunchArguments;
                }
                if (string.IsNullOrWhiteSpace(Game.UserDirectory))
                {
                    Game.UserDirectory = defaultSettings.UserDirectory;
                }
                SaveGame();
            }).DisposeWith(disposables);

            ResetEditorExeCommand = ReactiveCommand.Create(() =>
            {
                Editor.ExternalEditorLocation = string.Empty;
                SaveEditor();
            }).DisposeWith(disposables);

            ResetDirectoryCommand = ReactiveCommand.Create(() =>
            {
                var defaultSettings = gameService.GetDefaultGameSettings(Game);
                Game.UserDirectory = defaultSettings.UserDirectory;
                SaveGame();
            }).DisposeWith(disposables);

            ResetArgsCommand = ReactiveCommand.Create(() =>
            {
                Game.LaunchArguments = gameService.GetDefaultGameSettings(Game).LaunchArguments;
            }).DisposeWith(disposables);

            ResetEditorArgsCommand = ReactiveCommand.Create(() =>
            {
                Editor.ExternalEditorParameters = string.Empty;
            }).DisposeWith(disposables);

            AutoConfigureCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                IsOpen = false;
                var result = await fileDialogAction.OpenFolderDialogAsync(NavigateGameTitle);
                IsOpen = true;
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var settings = gameService.GetGameSettingsFromJson(Game, result);
                    if (settings != null)
                    {
                        Game.UserDirectory = settings.UserDirectory;
                        Game.ExecutableLocation = settings.ExecutableLocation;
                        Game.LaunchArguments = settings.LaunchArguments;
                        SaveGame();
                    }
                }
            }).DisposeWith(disposables);

            CheckForUpdatesCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await CheckForUpdatesAsync();
                IsOpen = true;
            }, updateCheckAllowed).DisposeWith(disposables);

            var downloadingUpdates = false;
            var installingUpdates = false;
            long messageId = 0;
            InstallUpdatesCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                messageId = idGenerator.GetNextId();
                downloadingUpdates = true;
                await TriggerOverlayAsync(messageId, true, localizationManager.GetResource(LocalizationResources.Options.Updates.Overlay.UpdateDownloading));
                if (await updater.DownloadUpdateAsync())
                {
                    downloadingUpdates = false;
                    await TriggerOverlayAsync(messageId, true, localizationManager.GetResource(LocalizationResources.Options.Updates.Overlay.UpdateInstalling));
                    installingUpdates = true;
                    await updater.InstallUpdateAsync();
                    installingUpdates = false;
                    await TriggerOverlayAsync(messageId, false);
                }
                else
                {
                    downloadingUpdates = false;
                    await TriggerOverlayAsync(messageId, false);
                }
            }).DisposeWith(disposables);

            NavigateCustomDirectoryCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                IsOpen = false;
                var result = await fileDialogAction.OpenFolderDialogAsync(NavigateUserDirTitle);
                var save = !string.IsNullOrEmpty(result);
                if (save && !await modService.CustomModDirectoryEmptyAsync(Game.Type))
                {
                    var title = localizationManager.GetResource(LocalizationResources.Options.Prompts.CustomModDirectory.Title);
                    var message = localizationManager.GetResource(LocalizationResources.Options.Prompts.CustomModDirectory.Message);
                    save = await notificationAction.ShowPromptAsync(title, title, message, NotificationType.Warning);
                }
                IsOpen = true;
                if (save)
                {
                    Game.CustomModDirectory = result;
                    SaveGame();
                }
            }).DisposeWith(disposables);

            ResetCustomDirectoryCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var save = true;
                if (!await modService.CustomModDirectoryEmptyAsync(Game.Type))
                {
                    var title = localizationManager.GetResource(LocalizationResources.Options.Prompts.CustomModDirectory.Title);
                    var message = localizationManager.GetResource(LocalizationResources.Options.Prompts.CustomModDirectory.Message);
                    save = await notificationAction.ShowPromptAsync(title, title, message, NotificationType.Warning);
                }
                if (save)
                {
                    var defaultSettings = gameService.GetDefaultGameSettings(Game);
                    Game.CustomModDirectory = defaultSettings.CustomModDirectory;
                    SaveGame();
                }
            }).DisposeWith(disposables);

            updater.Error.Subscribe(s =>
            {
                var title = localizationManager.GetResource(LocalizationResources.Options.Updates.Errors.DownloadErrorTitle);
                var message = localizationManager.GetResource(LocalizationResources.Options.Updates.Errors.DownloadErrorMessage);
                logger.Error(s);
                notificationAction.ShowNotification(title, message, NotificationType.Error, 30);
                TriggerOverlay(messageId, false);
            }).DisposeWith(disposables);

            updater.Progress.Subscribe(s =>
            {
                if (downloadingUpdates)
                {
                    var message = localizationManager.GetResource(LocalizationResources.Options.Updates.Overlay.UpdateDownloading);
                    var progress = Smart.Format(localizationManager.GetResource(LocalizationResources.Options.Updates.Overlay.UpdateDownloadProgress), new { Progress = s.ToLocalizedPercentage() });
                    TriggerOverlay(messageId, true, message, progress);
                }
                else if (installingUpdates)
                {
                    var message = localizationManager.GetResource(LocalizationResources.Options.Updates.Overlay.UpdateInstalling);
                    var progress = Smart.Format(localizationManager.GetResource(LocalizationResources.Options.Updates.Overlay.UpdateDownloadProgress), new { Progress = s.ToLocalizedPercentage() });
                    TriggerOverlay(messageId, true, message, progress);
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
        /// Saves the editor.
        /// </summary>
        protected virtual void SaveEditor()
        {
            var settings = externalEditorService.Get();
            settings.ExternalEditorLocation = Editor.ExternalEditorLocation;
            settings.ExternalEditorParameters = Editor.ExternalEditorParameters;
            externalEditorService.Save(settings);
            SetEditor(settings);
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
            game.CloseAppAfterGameLaunch = Game.CloseAppAfterGameLaunch;
            bool dirChanged = game.UserDirectory != Game.UserDirectory;
            game.UserDirectory = Game.UserDirectory;
            bool customDirectoryChanged = game.CustomModDirectory != Game.CustomModDirectory;
            game.CustomModDirectory = Game.CustomModDirectory;
            if (gameService.Save(game) && (dirChanged) || customDirectoryChanged)
            {
                MessageBus.PublishAsync(new GameUserDirectoryChangedEvent(game, customDirectoryChanged));
            }
            SetGame(game);
        }

        /// <summary>
        /// Saves the notification option.
        /// </summary>
        protected virtual void SaveNotificationOption()
        {
            positionSettingsService.Save(NotificationPosition);
            SetNotificationPosition(resubscribeOnly: true);
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
        /// Sets the editor.
        /// </summary>
        /// <param name="externalEditor">The external editor.</param>
        protected virtual void SetEditor(IExternalEditor externalEditor)
        {
            isEditorReloading = true;
            Editor = externalEditor;
            editorArgsChanged?.Dispose();
            editorArgsChanged = this.WhenAnyValue(p => p.Editor.ExternalEditorParameters).Where(p => !isEditorReloading).Subscribe(s =>
            {
                SaveEditor();
            }).DisposeWith(Disposables);
            isEditorReloading = false;
        }

        /// <summary>
        /// Sets the game.
        /// </summary>
        /// <param name="game">The game.</param>
        protected virtual void SetGame(IGame game)
        {
            isGameReloading = true;
            gameArgsChanged?.Dispose();
            refreshDescriptorsChanged?.Dispose();
            closeGameChanged?.Dispose();
            Game = game;
            gameArgsChanged = this.WhenAnyValue(p => p.Game.LaunchArguments).Where(p => !isGameReloading).Subscribe(s =>
            {
                SaveGame();
            }).DisposeWith(Disposables);
            refreshDescriptorsChanged = this.WhenAnyValue(p => p.Game.RefreshDescriptors).Where(p => !isGameReloading).Subscribe(s =>
            {
                SaveGame();
            }).DisposeWith(Disposables);
            closeGameChanged = this.WhenAnyValue(p => p.Game.CloseAppAfterGameLaunch).Where(p => !isGameReloading).Subscribe(s =>
            {
                SaveGame();
            }).DisposeWith(Disposables);
            ShowGameOptions = game != null;
            LeftMargin = new Thickness(ShowGameOptions ? 20 : 0, 0, 0, 0);
            LeftChildMargin = new Thickness(ShowGameOptions ? 20 : 0, 10, 0, 0);
            isGameReloading = false;
        }

        /// <summary>
        /// Sets the notification position.
        /// </summary>
        /// <param name="notificationPositions">The notification positions.</param>
        /// <param name="resubscribeOnly">if set to <c>true</c> [resubscribe only].</param>
        protected virtual void SetNotificationPosition(IEnumerable<INotificationPosition> notificationPositions = null, bool resubscribeOnly = false)
        {
            isNotificationPositionReloading = true;
            if (!resubscribeOnly && notificationPositions != null)
            {
                NotificationPositions = notificationPositions.ToAvaloniaList();
            }
            notificationPositionChanged?.Dispose();
            notificationPositionChanged = this.WhenAnyValue(p => p.NotificationPosition).Where(p => !isNotificationPositionReloading).Subscribe(s =>
            {
                foreach (var item in NotificationPositions)
                {
                    item.IsSelected = item == s;
                }
                SaveNotificationOption();
            }).DisposeWith(Disposables);
            if (!resubscribeOnly && notificationPositions != null)
            {
                NotificationPosition = NotificationPositions.FirstOrDefault(p => p.IsSelected);
            }
            isNotificationPositionReloading = false;
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
