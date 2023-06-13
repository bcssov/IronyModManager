
// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 06-13-2023
// ***********************************************************************
// <copyright file="ModHolderControlViewModel.cs" company="Mario">
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
using IronyModManager.Common;
using IronyModManager.Common.Events;
using IronyModManager.Common.ViewModels;
using IronyModManager.DI;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.AppState;
using IronyModManager.Implementation.MessageBus;
using IronyModManager.Implementation.Overlay;
using IronyModManager.Localization;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Platform.Configuration;
using IronyModManager.Services.Common;
using IronyModManager.Services.Common.Exceptions;
using IronyModManager.Shared;
using IronyModManager.Shared.MessageBus.Events;
using IronyModManager.Shared.Models;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{

    /// <summary>
    /// Class ModHolderViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ModHolderControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The block selected
        /// </summary>
        private const string InvalidConflictSolverClass = "InvalidConflictSolver";

        /// <summary>
        /// The application action
        /// </summary>
        private readonly IAppAction appAction;

        /// <summary>
        /// The external process handler service
        /// </summary>
        private readonly IExternalProcessHandlerService externalProcessHandlerService;

        /// <summary>
        /// The game definition load progress handler
        /// </summary>
        private readonly GameDefinitionLoadProgressHandler gameDefinitionLoadProgressHandler;

        /// <summary>
        /// The game directory changed handler
        /// </summary>
        private readonly GameUserDirectoryChangedHandler gameDirectoryChangedHandler;

        /// <summary>
        /// The game index progress handler
        /// </summary>
        private readonly GameIndexProgressHandler gameIndexProgressHandler;

        /// <summary>
        /// The game index service
        /// </summary>
        private readonly IGameIndexService gameIndexService;

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
        /// The mod definition analyze handler
        /// </summary>
        private readonly ModDefinitionAnalyzeHandler modDefinitionAnalyzeHandler;

        /// <summary>
        /// The mod definition invalid replace handler
        /// </summary>
        private readonly ModDefinitionInvalidReplaceHandler modDefinitionInvalidReplaceHandler;

        /// <summary>
        /// The mod definition load handler
        /// </summary>
        private readonly ModDefinitionLoadHandler modDefinitionLoadHandler;

        /// <summary>
        /// The mod definition patch load handler
        /// </summary>
        private readonly ModDefinitionPatchLoadHandler modDefinitionPatchLoadHandler;

        /// <summary>
        /// The mod list refresh request handler
        /// </summary>
        private readonly ModListInstallRefreshRequestHandler modListInstallRefreshRequestHandler;

        /// <summary>
        /// The mod service
        /// </summary>
        private readonly IModPatchCollectionService modPatchCollectionService;

        /// <summary>
        /// The mod service
        /// </summary>
        private readonly IModService modService;

        /// <summary>
        /// The notification action
        /// </summary>
        private readonly INotificationAction notificationAction;

        /// <summary>
        /// The prompt notifications service
        /// </summary>
        private readonly IPromptNotificationsService promptNotificationsService;

        /// <summary>
        /// The shut down state
        /// </summary>
        private readonly IShutDownState shutDownState;

        /// <summary>
        /// The definition analyze load handler
        /// </summary>
        private IDisposable definitionAnalyzeLoadHandler = null;

        /// <summary>
        /// The definition load handler
        /// </summary>
        private IDisposable definitionLoadHandler = null;

        /// <summary>
        /// The definition synchronize handler
        /// </summary>
        private IDisposable definitionSyncHandler = null;

        /// <summary>
        /// The force enable resume button
        /// </summary>
        private bool forceEnableResumeButton = false;

        /// <summary>
        /// The game definition load handler
        /// </summary>
        private IDisposable gameDefinitionLoadHandler = null;

        /// <summary>
        /// The game index handler
        /// </summary>
        private IDisposable gameIndexHandler = null;

        /// <summary>
        /// The mod invalid replace handler
        /// </summary>
        private IDisposable modInvalidReplaceHandler = null;

        /// <summary>
        /// The showing invalid notification
        /// </summary>
        private bool showingInvalidNotification = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModHolderControlViewModel" /> class.
        /// </summary>
        /// <param name="externalProcessHandlerService">The external process handler service.</param>
        /// <param name="gameDefinitionLoadProgressHandler">The game definition load progress handler.</param>
        /// <param name="gameIndexProgressHandler">The game index progress handler.</param>
        /// <param name="gameIndexService">The game index service.</param>
        /// <param name="promptNotificationsService">The prompt notifications service.</param>
        /// <param name="modListInstallRefreshRequestHandler">The mod list install refresh request handler.</param>
        /// <param name="modDefinitionInvalidReplaceHandler">The mod definition invalid replace handler.</param>
        /// <param name="idGenerator">The identifier generator.</param>
        /// <param name="shutDownState">State of the shut down.</param>
        /// <param name="modService">The mod service.</param>
        /// <param name="modPatchCollectionService">The mod patch collection service.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="notificationAction">The notification action.</param>
        /// <param name="appAction">The application action.</param>
        /// <param name="localizationManager">The localization manager.</param>
        /// <param name="installedModsControlViewModel">The installed mods control view model.</param>
        /// <param name="collectionModsControlViewModel">The collection mods control view model.</param>
        /// <param name="modDefinitionAnalyzeHandler">The mod definition analyze handler.</param>
        /// <param name="modDefinitionLoadHandler">The mod definition load handler.</param>
        /// <param name="modDefinitionPatchLoadHandler">The mod definition patch load handler.</param>
        /// <param name="gameDirectoryChangedHandler">The game directory changed handler.</param>
        /// <param name="logger">The logger.</param>
        public ModHolderControlViewModel(IExternalProcessHandlerService externalProcessHandlerService, GameDefinitionLoadProgressHandler gameDefinitionLoadProgressHandler, GameIndexProgressHandler gameIndexProgressHandler,
            IGameIndexService gameIndexService, IPromptNotificationsService promptNotificationsService,
            ModListInstallRefreshRequestHandler modListInstallRefreshRequestHandler, ModDefinitionInvalidReplaceHandler modDefinitionInvalidReplaceHandler,
            IIDGenerator idGenerator, IShutDownState shutDownState, IModService modService, IModPatchCollectionService modPatchCollectionService, IGameService gameService,
            INotificationAction notificationAction, IAppAction appAction, ILocalizationManager localizationManager,
            InstalledModsControlViewModel installedModsControlViewModel, CollectionModsControlViewModel collectionModsControlViewModel,
            ModDefinitionAnalyzeHandler modDefinitionAnalyzeHandler, ModDefinitionLoadHandler modDefinitionLoadHandler, ModDefinitionPatchLoadHandler modDefinitionPatchLoadHandler,
            GameUserDirectoryChangedHandler gameDirectoryChangedHandler, ILogger logger)
        {
            // Oh boy ctor injection really needs some cleanup huge code smell
            this.promptNotificationsService = promptNotificationsService;
            this.modDefinitionInvalidReplaceHandler = modDefinitionInvalidReplaceHandler;
            this.idGenerator = idGenerator;
            this.shutDownState = shutDownState;
            this.modService = modService;
            this.modPatchCollectionService = modPatchCollectionService;
            this.notificationAction = notificationAction;
            this.localizationManager = localizationManager;
            this.gameService = gameService;
            this.logger = logger;
            this.appAction = appAction;
            this.modDefinitionLoadHandler = modDefinitionLoadHandler;
            this.modDefinitionPatchLoadHandler = modDefinitionPatchLoadHandler;
            this.modDefinitionAnalyzeHandler = modDefinitionAnalyzeHandler;
            this.gameDirectoryChangedHandler = gameDirectoryChangedHandler;
            this.modListInstallRefreshRequestHandler = modListInstallRefreshRequestHandler;
            this.gameIndexService = gameIndexService;
            this.gameIndexProgressHandler = gameIndexProgressHandler;
            this.gameDefinitionLoadProgressHandler = gameDefinitionLoadProgressHandler;
            this.externalProcessHandlerService = externalProcessHandlerService;
            InstalledMods = installedModsControlViewModel;
            CollectionMods = collectionModsControlViewModel;
            UseSimpleLayout = !DIResolver.Get<IPlatformConfiguration>().GetOptions().ConflictSolver.UseSubMenus;
            if (StaticResources.CommandLineOptions != null && StaticResources.CommandLineOptions.EnableResumeGameButton)
            {
                forceEnableResumeButton = true;
            }
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the advanced mode.
        /// </summary>
        /// <value>The advanced mode.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.Modes.Advanced)]
        public virtual string AdvancedMode { get; protected set; }

        /// <summary>
        /// Gets or sets the advanced mode command.
        /// </summary>
        /// <value>The advanced mode command.</value>
        public virtual ReactiveCommand<Unit, Unit> AdvancedModeCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [advanced mode visible].
        /// </summary>
        /// <value><c>true</c> if [advanced mode visible]; otherwise, <c>false</c>.</value>
        public virtual bool AdvancedModeVisible { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [advanced parent visible].
        /// </summary>
        /// <value><c>true</c> if [advanced parent visible]; otherwise, <c>false</c>.</value>
        public virtual bool AdvancedParentVisible { get; protected set; }

        /// <summary>
        /// Gets or sets the advanced without localization mode.
        /// </summary>
        /// <value>The advanced without localization mode.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.Modes.AdvancedWithoutLocalization)]
        public virtual string AdvancedWithoutLocalizationMode { get; protected set; }

        /// <summary>
        /// Gets or sets the advanced without localization mode command.
        /// </summary>
        /// <value>The advanced without localization mode command.</value>
        public virtual ReactiveCommand<Unit, Unit> AdvancedWithoutLocalizationModeCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [advanced without localization mode visible].
        /// </summary>
        /// <value><c>true</c> if [advanced without localization mode visible]; otherwise, <c>false</c>.</value>
        public virtual bool AdvancedWithoutLocalizationModeVisible { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow mod selection].
        /// </summary>
        /// <value><c>true</c> if [allow mod selection]; otherwise, <c>false</c>.</value>
        public virtual bool AllowModSelection { get; protected set; }

        /// <summary>
        /// Gets or sets the analyze.
        /// </summary>
        /// <value>The analyze.</value>
        [StaticLocalization(LocalizationResources.Mod_Actions.ConflictSolver.Conflict)]
        public virtual string Analyze { get; protected set; }

        /// <summary>
        /// Gets or sets the analyze class.
        /// </summary>
        /// <value>The analyze class.</value>
        public virtual string AnalyzeClass { get; protected set; }

        /// <summary>
        /// Gets or sets the analyze command.
        /// </summary>
        /// <value>The analyze command.</value>
        public virtual ReactiveCommand<Unit, Unit> AnalyzeCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the analyze mode.
        /// </summary>
        /// <value>The analyze mode.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.Modes.Analyze)]
        public virtual string AnalyzeMode { get; protected set; }

        /// <summary>
        /// Gets or sets the analyze mode command.
        /// </summary>
        /// <value>The analyze mode command.</value>
        public virtual ReactiveCommand<Unit, Unit> AnalyzeModeCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the analyze mode without localization command.
        /// </summary>
        /// <value>The analyze mode without localization command.</value>
        public virtual ReactiveCommand<Unit, Unit> AnalyzeModeWithoutLocalizationCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the analyze without localization mode.
        /// </summary>
        /// <value>The analyze without localization mode.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.Modes.AnalyzeWithoutLocalization)]
        public virtual string AnalyzeWithoutLocalizationMode { get; protected set; }

        /// <summary>
        /// Gets or sets the apply.
        /// </summary>
        /// <value>The apply.</value>
        [StaticLocalization(LocalizationResources.Mod_Actions.Apply)]
        public virtual string Apply { get; protected set; }

        /// <summary>
        /// Gets or sets the apply command.
        /// </summary>
        /// <value>The apply command.</value>
        public virtual ReactiveCommand<Unit, Unit> ApplyCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [applying collection].
        /// </summary>
        /// <value><c>true</c> if [applying collection]; otherwise, <c>false</c>.</value>
        public virtual bool ApplyingCollection { get; protected set; }

        /// <summary>
        /// Gets or sets the close mode.
        /// </summary>
        /// <value>The close mode.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.Modes.Close)]
        public virtual string CloseMode { get; protected set; }

        /// <summary>
        /// Gets or sets the close mode command.
        /// </summary>
        /// <value>The close mode command.</value>
        public virtual ReactiveCommand<Unit, Unit> CloseModeCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the collection mods.
        /// </summary>
        /// <value>The collection mods.</value>
        public virtual CollectionModsControlViewModel CollectionMods { get; protected set; }

        /// <summary>
        /// Gets or sets the default mode.
        /// </summary>
        /// <value>The default mode.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.Modes.Default)]
        public virtual string DefaultMode { get; protected set; }

        /// <summary>
        /// Gets or sets the default mode command.
        /// </summary>
        /// <value>The default mode command.</value>
        public virtual ReactiveCommand<Unit, Unit> DefaultModeCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [default mode visible].
        /// </summary>
        /// <value><c>true</c> if [default mode visible]; otherwise, <c>false</c>.</value>
        public virtual bool DefaultModeVisible { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [default parent visible].
        /// </summary>
        /// <value><c>true</c> if [default parent visible]; otherwise, <c>false</c>.</value>
        public virtual bool DefaultParentVisible { get; protected set; }

        /// <summary>
        /// Gets or sets the default without localization mode.
        /// </summary>
        /// <value>The default without localization mode.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.Modes.DefaultWithoutLocalization)]
        public virtual string DefaultWithoutLocalizationMode { get; protected set; }

        /// <summary>
        /// Gets or sets the default without localization mode command.
        /// </summary>
        /// <value>The default without localization mode command.</value>
        public virtual ReactiveCommand<Unit, Unit> DefaultWithoutLocalizationModeCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [default without localization mode visible].
        /// </summary>
        /// <value><c>true</c> if [default without localization mode visible]; otherwise, <c>false</c>.</value>
        public virtual bool DefaultWithoutLocalizationModeVisible { get; protected set; }

        /// <summary>
        /// Gets or sets the installed mods.
        /// </summary>
        /// <value>The installed mods.</value>
        public virtual InstalledModsControlViewModel InstalledMods { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is mode open.
        /// </summary>
        /// <value><c>true</c> if this instance is mode open; otherwise, <c>false</c>.</value>
        public virtual bool IsModeOpen { get; protected set; }

        /// <summary>
        /// Gets or sets the launch game.
        /// </summary>
        /// <value>The launch game.</value>
        [StaticLocalization(LocalizationResources.Mod_Actions.LaunchGame.Launch)]
        public virtual string LaunchGame { get; protected set; }

        /// <summary>
        /// Gets or sets the launch game command.
        /// </summary>
        /// <value>The launch game command.</value>
        public virtual ReactiveCommand<Unit, Unit> LaunchGameCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the mode title.
        /// </summary>
        /// <value>The mode title.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.Modes.Title)]
        public virtual string ModeTitle { get; protected set; }

        /// <summary>
        /// Gets or sets the resume game.
        /// </summary>
        /// <value>The resume game.</value>
        [StaticLocalization(LocalizationResources.Mod_Actions.LaunchGame.Resume)]
        public virtual string ResumeGame { get; protected set; }

        /// <summary>
        /// Gets or sets the resume game command.
        /// </summary>
        /// <value>The resume game command.</value>
        public virtual ReactiveCommand<Unit, Unit> ResumeGameCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [resume game visible].
        /// </summary>
        /// <value><c>true</c> if [resume game visible]; otherwise, <c>false</c>.</value>
        public virtual bool ResumeGameVisible { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show advanced features].
        /// </summary>
        /// <value><c>true</c> if [show advanced features]; otherwise, <c>false</c>.</value>
        public virtual bool ShowAdvancedFeatures { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use simple layout].
        /// </summary>
        /// <value><c>true</c> if [use simple layout]; otherwise, <c>false</c>.</value>
        public virtual bool UseSimpleLayout { get; protected set; }

        /// <summary>
        /// Gets or sets the height of the vertical menu.
        /// </summary>
        /// <value>The height of the vertical menu.</value>
        public virtual int VerticalMenuHeight { get; protected set; } = 140;

        /// <summary>
        /// Gets the height of the vertical menu item.
        /// </summary>
        /// <value>The height of the vertical menu item.</value>
        public virtual int VerticalMenuItemHeight { get; } = 30;

        /// <summary>
        /// Gets the vertical menu spacing.
        /// </summary>
        /// <value>The vertical menu spacing.</value>
        public virtual int VerticalMenuSpacing { get; } = 5;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Forces the close popups.
        /// </summary>
        public virtual void ForceClosePopups()
        {
            IsModeOpen = false;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public virtual void Reset()
        {
            CollectionMods.Reset();
        }

        /// <summary>
        /// Analyzes the mods asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="versions">The versions.</param>
        /// <returns>Task.</returns>
        protected virtual async Task AnalyzeModsAsync(long id, PatchStateMode mode, IEnumerable<string> versions)
        {
            var totalSteps = versions != null && versions.Any() ? 6 : 4;

            SubscribeToProgressReport(id, Disposables, totalSteps);

            var overlayProgress = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Progress), new
            {
                PercentDone = 0.ToLocalizedPercentage(),
                Count = 1,
                TotalCount = totalSteps
            });
            var message = localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Loading_Definitions);
            await TriggerOverlayAsync(id, true, message, overlayProgress);
            modPatchCollectionService.InvalidatePatchModState(CollectionMods.SelectedModCollection.Name);
            modPatchCollectionService.ResetPatchStateCache();

            var tooLargeMod = false;
            var game = gameService.GetSelected();
            var definitions = await Task.Run(async () =>
            {
                IIndexedDefinitions result = null;
                try
                {
                    result = await modPatchCollectionService.GetModObjectsAsync(gameService.GetSelected(), CollectionMods.SelectedMods, CollectionMods.SelectedModCollection.Name, mode).ConfigureAwait(false);
                }
                catch (ModTooLargeException)
                {
                    tooLargeMod = true;
                }

                // To stop people from whining
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
                return result;
            }).ConfigureAwait(false);
            if (tooLargeMod)
            {
                await TriggerOverlayAsync(id, false);

                definitionAnalyzeLoadHandler?.Dispose();
                definitionLoadHandler?.Dispose();
                definitionSyncHandler?.Dispose();
                gameIndexHandler?.Dispose();
                gameDefinitionLoadHandler?.Dispose();

                // I know, I know... but I wanna force a cleanup
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

                var largeMessageTitle = localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.TooLargePrompt.Title);
                var largeMessageBody = localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.TooLargePrompt.Message);
                notificationAction.ShowNotification(largeMessageTitle, largeMessageBody, NotificationType.Error, 60);
                return;
            }
            if (versions != null && versions.Any())
            {
                await Task.Run(async () =>
                {
                    await gameIndexService.IndexDefinitionsAsync(game, versions, definitions);

                    // To stop people from whining
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
                });
                definitions = await Task.Run(async () =>
                {
                    var result = await gameIndexService.LoadDefinitionsAsync(definitions, game, versions);

                    // To stop people from whining
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
                    return result;
                }).ConfigureAwait(false);
            }
            var conflicts = await Task.Run(async () =>
            {
                if (definitions != null)
                {
                    // To stop people from whining
                    var result = await modPatchCollectionService.FindConflictsAsync(definitions, CollectionMods.SelectedMods.Select(p => p.Name).ToList(), mode);
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
                    return result;
                }
                return null;
            }).ConfigureAwait(false);
            var syncedConflicts = await Task.Run(async () =>
            {
                var result = await modPatchCollectionService.InitializePatchStateAsync(conflicts, CollectionMods.SelectedModCollection.Name).ConfigureAwait(false);

                // To stop people from whining
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
                return result;
            }).ConfigureAwait(false);
            if (syncedConflicts != null)
            {
                conflicts = syncedConflicts;
            }
            var args = new NavigationEventArgs()
            {
                SelectedCollection = CollectionMods.SelectedModCollection,
                Results = conflicts,
                State = mode == PatchStateMode.ReadOnly || mode == PatchStateMode.ReadOnlyWithoutLocalization ? NavigationState.ReadOnlyConflictSolver : NavigationState.ConflictSolver,
                SelectedMods = CollectionMods.SelectedMods.Select(p => p.Name).ToList()
            };
            ReactiveUI.MessageBus.Current.SendMessage(args);
            await TriggerOverlayAsync(id, false);

            definitionAnalyzeLoadHandler?.Dispose();
            definitionLoadHandler?.Dispose();
            definitionSyncHandler?.Dispose();
            gameIndexHandler?.Dispose();
            gameDefinitionLoadHandler?.Dispose();

            // I know, I know... but I wanna force a cleanup
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
        }

        /// <summary>
        /// apply collection as an asynchronous operation.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="showOverlay">if set to <c>true</c> [show overlay].</param>
        /// <param name="validateParadoxLauncher">if set to <c>true</c> [validate paradox launcher].</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected virtual async Task ApplyCollectionAsync(long id, bool showOverlay = true, bool validateParadoxLauncher = false)
        {
            if (ApplyingCollection)
            {
                return;
            }
            if (validateParadoxLauncher)
            {
                if (await externalProcessHandlerService.IsParadoxLauncherRunningAsync())
                {
                    var title = localizationManager.GetResource(LocalizationResources.Notifications.ParadoxLauncherRunning.Title);
                    var message = localizationManager.GetResource(LocalizationResources.Notifications.ParadoxLauncherRunning.Message);
                    notificationAction.ShowNotification(title, message, NotificationType.Error, 30);
                    return;
                }
            }
            ApplyingCollection = true;
            if (CollectionMods.SelectedModCollection != null)
            {
                if (showOverlay)
                {
                    await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Apply_Message));
                }
                var notificationType = NotificationType.Success;
                try
                {
                    var result = await modService.ExportModsAsync(CollectionMods.SelectedMods.ToList(), InstalledMods.AllMods.ToList(), CollectionMods.SelectedModCollection);
                    string title;
                    string message;
                    if (result)
                    {
                        title = localizationManager.GetResource(LocalizationResources.Notifications.CollectionApplied.Title);
                        message = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionApplied.Message), new { CollectionName = CollectionMods.SelectedModCollection.Name });
                    }
                    else
                    {
                        title = localizationManager.GetResource(LocalizationResources.Notifications.CollectionNotApplied.Title);
                        message = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionNotApplied.Message), new { CollectionName = CollectionMods.SelectedModCollection.Name });
                        notificationType = NotificationType.Error;
                    }
                    notificationAction.ShowNotification(title, message, notificationType, 5);
                }
                catch (Exception ex)
                {
                    var title = localizationManager.GetResource(LocalizationResources.SavingError.Title);
                    var message = localizationManager.GetResource(LocalizationResources.SavingError.Message);
                    logger.Error(ex);
                    notificationAction.ShowNotification(title, message, NotificationType.Error, 30);
                }
                if (showOverlay)
                {
                    await TriggerOverlayAsync(id, false);
                }
            }
            ApplyingCollection = false;
        }

        /// <summary>
        /// Evals the resume availability.
        /// </summary>
        /// <param name="game">The game.</param>
        protected virtual void EvalResumeAvailability(IGame game = null)
        {
            game ??= gameService.GetSelected();
            if (game != null)
            {
                ResumeGameVisible = gameService.IsContinueGameAllowed(game) || forceEnableResumeButton;
            }
            else
            {
                ResumeGameVisible = false;
            }
        }

        /// <summary>
        /// eval resume availability loop as an asynchronous operation.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected virtual async Task EvalResumeAvailabilityLoopAsync()
        {
            while (true)
            {
                EvalResumeAvailability();
                await Task.Delay(15000);
            }
        }

        /// <summary>
        /// install mods as an asynchronous operation.
        /// </summary>
        /// <param name="skipOverlay">if set to <c>true</c> [skip overlay].</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected virtual async Task InstallModsAsync(bool skipOverlay = false)
        {
            var result = await modService.InstallModsAsync(InstalledMods.Mods);
            if (result != null)
            {
                if (result.Any(p => p.Installed == true))
                {
                    if (InstalledMods.IsActivated)
                    {
                        await InstalledMods.RefreshModsAsync(skipOverlay: skipOverlay);
                    }
                }
                if (result.Any(p => p.Invalid))
                {
                    await ShowInvalidModsNotificationAsync(result.Where(p => p.Invalid).ToList());
                }
            }
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            async Task runAnalysis(PatchStateMode mode)
            {
                var id = idGenerator.GetNextId();
                await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.App.WaitBackgroundOperationMessage));
                var game = gameService.GetSelected();
                var versions = gameService.GetVersions(game);
                var hasGameDefinitions = await modPatchCollectionService.PatchHasGameDefinitionsAsync(CollectionMods.SelectedModCollection.Name);
                var shouldAnalyzePatchState = versions != null && versions.Any();
                var proceed = true;
                if (hasGameDefinitions && !shouldAnalyzePatchState)
                {
                    var title = localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.GameExecutableNotSetPrompt.Title);
                    var message = localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.GameExecutableNotSetPrompt.Message);
                    proceed = await notificationAction.ShowPromptAsync(title, title, message, NotificationType.Info, PromptType.YesNo);
                }
                if (proceed)
                {
                    await AnalyzeModsAsync(id, mode, versions);
                }
                else
                {
                    await TriggerOverlayAsync(id, false);
                }
            }

            Task.Run(() => EvalResumeAvailabilityLoopAsync().ConfigureAwait(false));

            ShowAdvancedFeatures = (gameService.GetSelected()?.AdvancedFeatures) != GameAdvancedFeatures.None;
            AnalyzeClass = string.Empty;

            var allowModSelectionEnabled = this.WhenAnyValue(v => v.AllowModSelection);
            var applyEnabled = Observable.Merge(this.WhenAnyValue(v => v.ApplyingCollection, v => !v), allowModSelectionEnabled);

            this.WhenAnyValue(v => v.CollectionMods.SelectedModCollection).Subscribe(s =>
            {
                if (s != null)
                {
                    AllowModSelection = true;
                    InstalledMods.AllowModSelection = true;
                    CollectionMods.AllowModSelection = true;
                }
                else
                {
                    AllowModSelection = false;
                    InstalledMods.AllowModSelection = false;
                    CollectionMods.AllowModSelection = false;
                }
                InstallModsAsync().ConfigureAwait(true);
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.InstalledMods.Mods).Subscribe(v =>
            {
                CollectionMods.SetMods(v, InstalledMods.ActiveGame);
            });

            this.WhenAnyValue(v => v.InstalledMods.RefreshingMods).Subscribe(s =>
            {
                CollectionMods.HandleModRefresh(s, InstalledMods.Mods, InstalledMods.ActiveGame);
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.CollectionMods.NeedsModListRefresh).Where(x => x).Subscribe(async s =>
            {
                await InstalledMods.RefreshModsAsync();
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.InstalledMods.GameChangedRefresh).Where(x => x).Subscribe(s =>
            {
                CollectionMods.ReloadModCollection();
            }).DisposeWith(disposables);

            this.WhenAnyValue(p => p.InstalledMods.PerformingEnableAll).Subscribe(s =>
            {
                CollectionMods.HandleEnableAllToggled(s, InstalledMods.AllModsEnabled, InstalledMods.FilteredMods);
            }).DisposeWith(disposables);

            ApplyCommand = ReactiveCommand.Create(() =>
            {
                ApplyCollectionAsync(idGenerator.GetNextId(), validateParadoxLauncher: true).ConfigureAwait(true);
            }, applyEnabled).DisposeWith(disposables);

            AnalyzeCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var game = gameService.GetSelected();
                if (game != null && CollectionMods.SelectedMods?.Count > 0 && CollectionMods.SelectedModCollection != null)
                {
                    var messageState = promptNotificationsService.Get();
                    if (!messageState.ConflictSolverPromptShown)
                    {
                        var title = localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.FirstUsePrompt.Title);
                        var message = localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.FirstUsePrompt.Message);
                        await notificationAction.ShowPromptAsync(title, title, message, NotificationType.Info, PromptType.OK);
                        messageState.ConflictSolverPromptShown = true;
                        promptNotificationsService.Save(messageState);
                    }
                    var id = idGenerator.GetNextId();
                    await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.App.WaitBackgroundOperationMessage));
                    await shutDownState.WaitUntilFreeAsync();
                    modPatchCollectionService.ResetPatchStateCache();
                    if (game.AdvancedFeatures == GameAdvancedFeatures.Full)
                    {
                        var mode = await modPatchCollectionService.GetPatchStateModeAsync(CollectionMods.SelectedModCollection.Name);
                        var versions = gameService.GetVersions(game);
                        switch (mode)
                        {
                            case PatchStateMode.Default:
                                AdvancedModeVisible = false;
                                DefaultModeVisible = true;
                                DefaultWithoutLocalizationModeVisible = false;
                                AdvancedWithoutLocalizationModeVisible = false;
                                break;

                            case PatchStateMode.Advanced:
                                AdvancedModeVisible = true;
                                DefaultModeVisible = false;
                                DefaultWithoutLocalizationModeVisible = false;
                                AdvancedWithoutLocalizationModeVisible = false;
                                break;

                            case PatchStateMode.DefaultWithoutLocalization:
                                AdvancedModeVisible = false;
                                DefaultModeVisible = false;
                                DefaultWithoutLocalizationModeVisible = true;
                                AdvancedWithoutLocalizationModeVisible = false;
                                break;

                            case PatchStateMode.AdvancedWithoutLocalization:
                                AdvancedModeVisible = false;
                                DefaultModeVisible = false;
                                DefaultWithoutLocalizationModeVisible = false;
                                AdvancedWithoutLocalizationModeVisible = true;
                                break;

                            default:
                                AdvancedModeVisible = true;
                                DefaultModeVisible = true;
                                DefaultWithoutLocalizationModeVisible = true;
                                AdvancedWithoutLocalizationModeVisible = true;
                                break;
                        }
                    }
                    else
                    {
                        AdvancedModeVisible = false;
                        DefaultModeVisible = false;
                        AdvancedWithoutLocalizationModeVisible = false;
                        DefaultWithoutLocalizationModeVisible = false;
                    }
                    var height = (VerticalMenuSpacing + VerticalMenuItemHeight) * 2;
                    AdvancedParentVisible = AdvancedModeVisible || AdvancedWithoutLocalizationModeVisible;
                    DefaultParentVisible = DefaultModeVisible || DefaultWithoutLocalizationModeVisible;
                    if (AdvancedParentVisible)
                    {
                        height += VerticalMenuSpacing + VerticalMenuItemHeight;
                    }
                    if (DefaultParentVisible)
                    {
                        height += VerticalMenuSpacing + VerticalMenuItemHeight;
                    }
                    VerticalMenuHeight = height;
                    await TriggerOverlayAsync(id, false);
                    await Task.Delay(50);
                    IsModeOpen = true;
                }
            }, allowModSelectionEnabled).DisposeWith(disposables);

            async Task<bool> ensureSteamIsRunning(IGameSettings args)
            {
                if (gameService.IsSteamGame(args))
                {
                    return await externalProcessHandlerService.LaunchSteamAsync(gameService.GetSelected());
                }
                return true;
            }

            async Task launchGame(bool continueGame)
            {
                var game = gameService.GetSelected();
                if (game != null)
                {
                    if (await externalProcessHandlerService.IsParadoxLauncherRunningAsync())
                    {
                        var title = localizationManager.GetResource(LocalizationResources.Notifications.ParadoxLauncherRunning.Title);
                        var message = localizationManager.GetResource(LocalizationResources.Notifications.ParadoxLauncherRunning.Message);
                        notificationAction.ShowNotification(title, message, NotificationType.Error, 30);
                        return;
                    }
                    var args = gameService.GetLaunchSettings(game, continueGame);
                    if (!string.IsNullOrWhiteSpace(args.ExecutableLocation))
                    {
                        var id = idGenerator.GetNextId();
                        await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.Mod_Actions.LaunchGame.Overlay));
                        if (game.RefreshDescriptors)
                        {
                            await modService.DeleteDescriptorsAsync(InstalledMods.Mods);
                            await modService.InstallModsAsync(InstalledMods.Mods);
                        }
                        await ApplyCollectionAsync(id, false);
                        await MessageBus.PublishAsync(new LaunchingGameEvent(game.Type));
                        if (gameService.IsSteamLaunchPath(args))
                        {
                            if (await appAction.OpenAsync(args.ExecutableLocation))
                            {
                                if (game.CloseAppAfterGameLaunch)
                                {
                                    await appAction.ExitAppAsync();
                                }
                            }
                            else
                            {
                                notificationAction.ShowNotification(localizationManager.GetResource(LocalizationResources.Mod_Actions.LaunchGame.LaunchError.Title),
                                    localizationManager.GetResource(LocalizationResources.Mod_Actions.LaunchGame.LaunchError.Message), NotificationType.Error, 10);
                                await TriggerOverlayAsync(id, false);
                            }
                        }
                        else
                        {
                            await ensureSteamIsRunning(args);
                            if (await appAction.RunAsync(args.ExecutableLocation, args.LaunchArguments))
                            {
                                if (game.CloseAppAfterGameLaunch)
                                {
                                    await appAction.ExitAppAsync();
                                }
                                else
                                {
                                    await TriggerOverlayAsync(id, false);
                                }
                            }
                            else
                            {
                                notificationAction.ShowNotification(localizationManager.GetResource(LocalizationResources.Mod_Actions.LaunchGame.LaunchError.Title),
                                    localizationManager.GetResource(LocalizationResources.Mod_Actions.LaunchGame.LaunchError.Message), NotificationType.Error, 10);
                                await TriggerOverlayAsync(id, false);
                            }
                        }
                    }
                    else
                    {
                        notificationAction.ShowNotification(localizationManager.GetResource(LocalizationResources.Mod_Actions.LaunchGame.NotSet.Title),
                            localizationManager.GetResource(LocalizationResources.Mod_Actions.LaunchGame.NotSet.Message), NotificationType.Warning, 10);
                    }
                }
            }

            LaunchGameCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await launchGame(false);
            }, allowModSelectionEnabled).DisposeWith(disposables);

            ResumeGameCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await launchGame(true);
            }, allowModSelectionEnabled).DisposeWith(disposables);

            AdvancedModeCommand = ReactiveCommand.CreateFromTask(() =>
            {
                return runAnalysis(PatchStateMode.Advanced);
            }).DisposeWith(disposables);

            DefaultModeCommand = ReactiveCommand.CreateFromTask(() =>
            {
                return runAnalysis(PatchStateMode.Default);
            }).DisposeWith(disposables);

            AnalyzeModeCommand = ReactiveCommand.CreateFromTask(() =>
            {
                return runAnalysis(PatchStateMode.ReadOnly);
            }).DisposeWith(disposables);

            AnalyzeModeWithoutLocalizationCommand = ReactiveCommand.CreateFromTask(() =>
            {
                return runAnalysis(PatchStateMode.ReadOnlyWithoutLocalization);
            }).DisposeWith(disposables);

            DefaultWithoutLocalizationModeCommand = ReactiveCommand.CreateFromTask(() =>
            {
                return runAnalysis(PatchStateMode.DefaultWithoutLocalization);
            }).DisposeWith(disposables);

            AdvancedWithoutLocalizationModeCommand = ReactiveCommand.CreateFromTask(() =>
            {
                return runAnalysis(PatchStateMode.AdvancedWithoutLocalization);
            }).DisposeWith(disposables);

            CloseModeCommand = ReactiveCommand.Create(() =>
            {
                ForceClosePopups();
            }).DisposeWith(disposables);

            var previousCollectionNotification = string.Empty;
            CollectionMods.ConflictSolverStateChanged += (collectionName, state) =>
            {
                AnalyzeClass = !state ? InvalidConflictSolverClass : string.Empty;
                if (!state && previousCollectionNotification != collectionName)
                {
                    notificationAction.ShowNotification(localizationManager.GetResource(LocalizationResources.Notifications.ConflictSolverUpdate.Title),
                        localizationManager.GetResource(LocalizationResources.Notifications.ConflictSolverUpdate.Message), NotificationType.Warning, 30);
                    previousCollectionNotification = collectionName;
                }
            };

            gameDirectoryChangedHandler.Subscribe(async s =>
            {
                if (s.CustomDirectoryChanged)
                {
                    CollectionMods.Reset(true);
                }
                await InstalledMods.RefreshModsAsync();
                EvalResumeAvailability(s.Game);
            }).DisposeWith(disposables);

            modListInstallRefreshRequestHandler.Subscribe(async m =>
            {
                await InstallModsAsync(m.SkipOverlay);
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// Called when [selected game changed].
        /// </summary>
        /// <param name="game">The game.</param>
        protected override void OnSelectedGameChanged(IGame game)
        {
            forceEnableResumeButton = false;
            EvalResumeAvailability(game);
            ShowAdvancedFeatures = (game?.AdvancedFeatures) != GameAdvancedFeatures.None;
            base.OnSelectedGameChanged(game);
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
        /// Subscribes to progress report.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="disposables">The disposables.</param>
        /// <param name="totalSteps">The total steps.</param>
        private void SubscribeToProgressReport(long id, CompositeDisposable disposables, int totalSteps)
        {
            definitionLoadHandler?.Dispose();
            definitionLoadHandler = modDefinitionLoadHandler.Subscribe(s =>
            {
                var message = localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Loading_Definitions);
                var overlayProgress = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Progress), new
                {
                    PercentDone = s.Percentage.ToLocalizedPercentage(),
                    Count = 1,
                    TotalCount = totalSteps
                });
                TriggerOverlay(id, true, message, overlayProgress);
            }).DisposeWith(disposables);

            modInvalidReplaceHandler?.Dispose();
            modInvalidReplaceHandler = modDefinitionInvalidReplaceHandler.Subscribe(s =>
            {
                var message = localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Replacing_Definitions);
                var overlayProgress = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Progress), new
                {
                    PercentDone = s.Percentage.ToLocalizedPercentage(),
                    Count = 2,
                    TotalCount = totalSteps
                });
                TriggerOverlay(id, true, message, overlayProgress);
            }).DisposeWith(disposables);

            gameIndexHandler?.Dispose();
            gameIndexHandler = gameIndexProgressHandler.Subscribe(s =>
            {
                var message = localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Indexing_Game);
                var overlayProgress = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Progress), new
                {
                    PercentDone = s.Percentage.ToLocalizedPercentage(),
                    Count = 3,
                    TotalCount = totalSteps
                });
                TriggerOverlay(id, true, message, overlayProgress);
            }).DisposeWith(disposables);

            gameDefinitionLoadHandler?.Dispose();
            gameDefinitionLoadHandler = gameDefinitionLoadProgressHandler.Subscribe(s =>
            {
                var message = localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Loading_Game_Definitions);
                var overlayProgress = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Progress), new
                {
                    PercentDone = s.Percentage.ToLocalizedPercentage(),
                    Count = 4,
                    TotalCount = totalSteps
                });
                TriggerOverlay(id, true, message, overlayProgress);
            }).DisposeWith(disposables);

            definitionAnalyzeLoadHandler?.Dispose();
            definitionAnalyzeLoadHandler = modDefinitionAnalyzeHandler.Subscribe(s =>
            {
                var message = localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Analyzing_Conflicts);
                var overlayProgress = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Progress), new
                {
                    PercentDone = s.Percentage.ToLocalizedPercentage(),
                    Count = totalSteps == 6 ? 5 : 3,
                    TotalCount = totalSteps
                });
                TriggerOverlay(id, true, message, overlayProgress);
            }).DisposeWith(disposables);

            definitionSyncHandler?.Dispose();
            definitionSyncHandler = modDefinitionPatchLoadHandler.Subscribe(s =>
            {
                var message = localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Analyzing_Resolved_Conflicts);
                var overlayProgress = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Progress), new
                {
                    PercentDone = s.Percentage.ToLocalizedPercentage(),
                    Count = totalSteps == 6 ? 6 : 4,
                    TotalCount = totalSteps
                });
                TriggerOverlay(id, true, message, overlayProgress);
            }).DisposeWith(disposables);
        }

        #endregion Methods
    }
}
