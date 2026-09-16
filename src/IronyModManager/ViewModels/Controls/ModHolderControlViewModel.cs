// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 12-07-2025
// ***********************************************************************
// <copyright file="ModHolderControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
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
using IronyModManager.Shared.Configuration;
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
        /// Owns common mod-control user interactions.
        /// </summary>
        private readonly ModHolderInteraction interaction;

        /// <summary>
        /// The creation steam application identifier file flag
        /// </summary>
        private readonly bool createSteamAppIdFile;

        /// <summary>
        /// The external process handler service
        /// </summary>
        private readonly IExternalProcessHandlerService externalProcessHandlerService;

        /// <summary>
        /// Coordinates activation-scoped holder event subscriptions.
        /// </summary>
        private readonly ModHolderEventCoordinator eventCoordinator;

        /// <summary>
        /// The game index service
        /// </summary>
        private readonly IGameIndexService gameIndexService;

        /// <summary>
        /// The game language service
        /// </summary>
        private readonly IGameLanguageService gameLanguageService;

        /// <summary>
        /// The game service
        /// </summary>
        private readonly IGameService gameService;

        /// <summary>
        /// Coordinates conflict-analysis progress subscriptions and presentation.
        /// </summary>
        private readonly ConflictAnalysisProgressCoordinator conflictAnalysisProgressCoordinator;

        /// <summary>
        /// Coordinates authoritative installed-mod publication with collection reconciliation.
        /// </summary>
        private readonly ModStateReconciliationCoordinator modStateReconciliationCoordinator;

        /// <summary>
        /// The mod service
        /// </summary>
        private readonly IModPatchCollectionService modPatchCollectionService;

        /// <summary>
        /// The mod service
        /// </summary>
        private readonly IModService modService;

        private readonly ModApplyResultPresenter modApplyResultPresenter;

        /// <summary>
        /// The prompt notifications service
        /// </summary>
        private readonly IPromptNotificationsService promptNotificationsService;

        /// <summary>
        /// The shut-down state
        /// </summary>
        private readonly IShutDownState shutDownState;

        /// <summary>
        /// The force enable resume button
        /// </summary>
        private bool forceEnableResumeButton;

        /// <summary>
        /// The showing invalid notification
        /// </summary>
        private bool showingInvalidNotification;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModHolderControlViewModel" /> class.
        /// </summary>
        /// <param name="gameLanguageService">The game language service.</param>
        /// <param name="externalProcessHandlerService">The external process handler service.</param>
        /// <param name="gameIndexService">The game index service.</param>
        /// <param name="promptNotificationsService">The prompt notifications service.</param>
        /// <param name="shutDownState">State of the shut-down.</param>
        /// <param name="modService">The mod service.</param>
        /// <param name="modPatchCollectionService">The mod patch collection service.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="interaction">The mod-control user interaction facade.</param>
        /// <param name="installedModsControlViewModel">The installed mods control view model.</param>
        /// <param name="collectionModsControlViewModel">The collection mods control view model.</param>
        /// <param name="coordinatorFactory">The Mod Holder collaborator factory.</param>
        public ModHolderControlViewModel(IGameLanguageService gameLanguageService, IExternalProcessHandlerService externalProcessHandlerService,
            IGameIndexService gameIndexService, IPromptNotificationsService promptNotificationsService,
            IShutDownState shutDownState, IModService modService, IModPatchCollectionService modPatchCollectionService, IGameService gameService,
            ModHolderInteraction interaction,
            InstalledModsControlViewModel installedModsControlViewModel, CollectionModsControlViewModel collectionModsControlViewModel,
            IModHolderCoordinatorFactory coordinatorFactory)
        {
            var coordinators = coordinatorFactory.Create();
            this.promptNotificationsService = promptNotificationsService;
            this.interaction = interaction;
            this.shutDownState = shutDownState;
            this.modService = modService;
            this.modPatchCollectionService = modPatchCollectionService;
            this.gameService = gameService;
            modStateReconciliationCoordinator = coordinators.Reconciliation;
            conflictAnalysisProgressCoordinator = coordinators.ConflictProgress;
            modApplyResultPresenter = coordinators.ApplyResultPresenter;
            eventCoordinator = coordinators.Events;
            this.gameIndexService = gameIndexService;
            this.externalProcessHandlerService = externalProcessHandlerService;
            this.gameLanguageService = gameLanguageService;
            InstalledMods = installedModsControlViewModel;
            CollectionMods = collectionModsControlViewModel;
            UseSimpleLayout = !DIResolver.Get<IPlatformConfiguration>().GetOptions().ConflictSolver.UseSubMenus;
            createSteamAppIdFile = DIResolver.Get<IDomainConfiguration>().GetOptions().Steam.GenerateSteamAppIdFile;
            if (StaticResources.CommandLineOptions.EnableResumeGameButton)
            {
                forceEnableResumeButton = true;
            }

            StaticResources.CommandLineArgsChanged += () =>
            {
                Dispatcher.UIThread.SafeInvoke(() =>
                {
                    forceEnableResumeButton = StaticResources.CommandLineOptions.EnableResumeGameButton;
                    EvalResumeAvailability();
                });
            };
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
        /// Gets or sets the analysis.
        /// </summary>
        /// <value>The analysis.</value>
        [StaticLocalization(LocalizationResources.Mod_Actions.ConflictSolver.Conflict)]
        public virtual string Analyze { get; protected set; }

        /// <summary>
        /// Gets or sets the analysis class.
        /// </summary>
        /// <value>The analysis class.</value>
        public virtual string AnalyzeClass { get; protected set; }

        /// <summary>
        /// Gets or sets the analysis command.
        /// </summary>
        /// <value>The analysis command.</value>
        public virtual ReactiveCommand<Unit, Unit> AnalyzeCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the analysis mode.
        /// </summary>
        /// <value>The analysis mode.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.Modes.Analyze)]
        public virtual string AnalyzeMode { get; protected set; }

        /// <summary>
        /// Gets or sets the analysis mode command.
        /// </summary>
        /// <value>The analysis mode command.</value>
        public virtual ReactiveCommand<Unit, Unit> AnalyzeModeCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the analysis mode without localization command.
        /// </summary>
        /// <value>The analysis mode without localization command.</value>
        public virtual ReactiveCommand<Unit, Unit> AnalyzeModeWithoutLocalizationCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the analysis without localization mode.
        /// </summary>
        /// <value>The analysis without localization mode.</value>
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
            var selectedMods = CollectionMods.SelectedMods.Where(p => !p.IsVirtual).ToList();
            var totalSteps = versions != null && versions.Any() ? 6 : 4;

            SubscribeToProgressReport(id, Disposables, totalSteps);

            var overlayProgress = IronyFormatter.Format(interaction.GetText(LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Progress),
                new { PercentDone = 0.ToLocalizedPercentage(), Count = 1, TotalCount = totalSteps });
            var message = interaction.GetText(LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Loading_Definitions);
            await TriggerOverlayAsync(id, true, message, overlayProgress);
            modPatchCollectionService.InvalidatePatchModState(CollectionMods.SelectedModCollection.Name);
            modPatchCollectionService.ResetPatchStateCache();

            IReadOnlyCollection<IGameLanguage> allowedLanguages;
            var usedAllowedLanguages = await modPatchCollectionService.GetAllowedLanguagesAsync(CollectionMods.SelectedModCollection.Name);
            if (usedAllowedLanguages != null)
            {
                allowedLanguages = gameLanguageService.GetByAbrv(usedAllowedLanguages);
            }
            else
            {
                allowedLanguages = gameLanguageService.GetSelected();
            }

            var tooLargeMod = false;
            var game = gameService.GetSelected();
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var definitions = await Task.Run(async () =>
            {
                IIndexedDefinitions result = null;
                try
                {
                    result = await modPatchCollectionService
                        .GetModObjectsAsync(gameService.GetSelected(), selectedMods, CollectionMods.SelectedModCollection.Name, mode, allowedLanguages).ConfigureAwait(false);
                }
                catch (ModTooLargeException)
                {
                    tooLargeMod = true;
                }

                GCRunner.RunGC(GCCollectionMode.Aggressive, true);
                return result;
            }).ConfigureAwait(false);

            Debug.WriteLine("Conflict Solver Stage 1: " + stopWatch.Elapsed.FormatElapsed());
            if (tooLargeMod)
            {
                await TriggerOverlayAsync(id, false);

                conflictAnalysisProgressCoordinator.Reset();

                GCRunner.RunGC(GCCollectionMode.Aggressive, true);
                var largeMessageTitle = interaction.GetText(LocalizationResources.Mod_Actions.ConflictSolver.TooLargePrompt.Title);
                var largeMessageBody = interaction.GetText(LocalizationResources.Mod_Actions.ConflictSolver.TooLargePrompt.Message);
                interaction.Notify(largeMessageTitle, largeMessageBody, NotificationType.Error, 60);
                return;
            }

            if (versions != null && versions.Any())
            {
                stopWatch.Restart();
                await Task.Run(async () =>
                {
                    await gameIndexService.IndexDefinitionsAsync(game, versions, definitions);

                    GCRunner.RunGC(GCCollectionMode.Aggressive, true);
                });
                Debug.WriteLine("Conflict Solver Stage 2: " + stopWatch.Elapsed.FormatElapsed());

                stopWatch.Restart();
                definitions = await Task.Run(async () =>
                {
                    var result = await gameIndexService.LoadDefinitionsAsync(definitions, game, versions, allowedLanguages);

                    GCRunner.RunGC(GCCollectionMode.Aggressive, true);
                    return result;
                }).ConfigureAwait(false);
                Debug.WriteLine("Conflict Solver Stage 3: " + stopWatch.Elapsed.FormatElapsed());
            }

            stopWatch.Restart();
            var conflicts = await Task.Run(async () =>
            {
                if (definitions != null)
                {
                    var result = await modPatchCollectionService.FindConflictsAsync(definitions, [.. selectedMods.Select(p => p.Name)], mode, allowedLanguages);
                    GCRunner.RunGC(GCCollectionMode.Aggressive, true);
                    return result;
                }

                return null;
            }).ConfigureAwait(false);
            Debug.WriteLine("Conflict Solver Stage 4: " + stopWatch.Elapsed.FormatElapsed());

            stopWatch.Restart();
            var syncedConflicts = await Task.Run(async () =>
            {
                var result = await modPatchCollectionService.InitializePatchStateAsync(conflicts, CollectionMods.SelectedModCollection.Name).ConfigureAwait(false);

                GCRunner.RunGC(GCCollectionMode.Aggressive, true);
                return result;
            }).ConfigureAwait(false);
            if (syncedConflicts != null)
            {
                conflicts = syncedConflicts;
            }

            Debug.WriteLine("Conflict Solver Stage 5: " + stopWatch.Elapsed.FormatElapsed());
            var args = new NavigationEventArgs
            {
                SelectedCollection = CollectionMods.SelectedModCollection,
                Results = conflicts,
                State = mode is PatchStateMode.ReadOnly or PatchStateMode.ReadOnlyWithoutLocalization ? NavigationState.ReadOnlyConflictSolver : NavigationState.ConflictSolver,
                SelectedMods = [.. selectedMods.Select(p => p.Name)]
            };
            ReactiveUI.MessageBus.Current.SendMessage(args);
            await TriggerOverlayAsync(id, false);

            conflictAnalysisProgressCoordinator.Reset();

            GCRunner.RunGC(GCCollectionMode.Aggressive, true);
        }

        /// <summary>
        /// apply collection as an asynchronous operation.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="showOverlay">if set to <c>true</c> [show overlay].</param>
        /// <param name="validateParadoxLauncher">if set to <c>true</c> [validate paradox launcher].</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected virtual async Task<ModApplyResult> ApplyCollectionAsync(long id, bool showOverlay = true,
            bool validateParadoxLauncher = false, bool launchingGame = false)
        {
            if (ApplyingCollection)
            {
                return new ModApplyResult();
            }

            if (validateParadoxLauncher)
            {
                if (await externalProcessHandlerService.IsParadoxLauncherRunningAsync())
                {
                    var title = interaction.GetText(LocalizationResources.Notifications.ParadoxLauncherRunning.Title);
                    var message = interaction.GetText(LocalizationResources.Notifications.ParadoxLauncherRunning.Message);
                    interaction.Notify(title, message, NotificationType.Error, 30);
                    return new ModApplyResult();
                }
            }

            ApplyingCollection = true;
            var applyResult = new ModApplyResult();
            try
            {
                if (CollectionMods.SelectedModCollection != null)
                {
                    if (showOverlay)
                    {
                        await TriggerOverlayAsync(id, true, interaction.GetText(LocalizationResources.Mod_Actions.Overlay_Apply_Message));
                    }

                    try
                    {
                        applyResult = await modService.ExportModsAsync([.. CollectionMods.SelectedMods], [.. InstalledMods.AllMods], CollectionMods.SelectedModCollection);
                        await modApplyResultPresenter.ShowAsync(CollectionMods.SelectedModCollection, applyResult, launchingGame);
                    }
                    catch (Exception ex)
                    {
                        interaction.ReportSavingFailure(ex);
                    }

                    if (showOverlay)
                    {
                        await TriggerOverlayAsync(id, false);
                    }
                }

                return applyResult;
            }
            finally
            {
                ApplyingCollection = false;
            }
        }

        /// <summary>
        /// Evaluate the resume availability.
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
                if (result.Any(p => p.Installed))
                {
                    if (InstalledMods.IsActivated)
                    {
                        await InstalledMods.RefreshModsAsync(skipOverlay);
                    }
                }

                if (result.Any(p => p.Invalid))
                {
                    await ShowInvalidModsNotificationAsync([.. result.Where(p => p.Invalid)]);
                }
            }

        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            void onGameLocked(GameStateLockInfo info)
            {
                if (!string.Equals(gameService.GetSelected()?.Type, info.GameType, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                Dispatcher.UIThread.SafeInvoke(() =>
                {
                    AllowModSelection = false;
                    InstalledMods.AllowModSelection = false;
                    CollectionMods.AllowModSelection = false;
                    var title = interaction.GetText(LocalizationResources.Notifications.FileSystemSafetyLock.Title);
                    var message = interaction.GetText(LocalizationResources.Notifications.FileSystemSafetyLock.Message);
                    interaction.Notify(title, message, NotificationType.Error, 10);
                });
            }

            modStateReconciliationCoordinator.SubscribeToGameLocks(onGameLocked).DisposeWith(disposables);

            async Task runAnalysis(PatchStateMode mode)
            {
                var id = interaction.BeginOperation();
                await TriggerOverlayAsync(id, true, interaction.GetText(LocalizationResources.App.WaitBackgroundOperationMessage));
                var game = gameService.GetSelected();
                var versions = gameService.GetVersions(game);
                var hasGameDefinitions = await modPatchCollectionService.PatchHasGameDefinitionsAsync(CollectionMods.SelectedModCollection.Name);
                var shouldAnalyzePatchState = versions != null && versions.Any();
                var proceed = true;
                if (hasGameDefinitions && !shouldAnalyzePatchState)
                {
                    var title = interaction.GetText(LocalizationResources.Mod_Actions.ConflictSolver.GameExecutableNotSetPrompt.Title);
                    var message = interaction.GetText(LocalizationResources.Mod_Actions.ConflictSolver.GameExecutableNotSetPrompt.Message);
                    proceed = await interaction.PromptAsync(title, title, message, NotificationType.Info, PromptType.YesNo);
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

            ShowAdvancedFeatures = gameService.GetSelected()?.AdvancedFeatures != GameAdvancedFeatures.None;
            AnalyzeClass = string.Empty;

            var allowModSelectionEnabled = this.WhenAnyValue(v => v.AllowModSelection);
            var applyEnabled = this.WhenAnyValue(v => v.ApplyingCollection, v => !v).Merge(allowModSelectionEnabled);

            this.WhenAnyValue(v => v.CollectionMods.SelectedModCollection).Subscribe(s =>
            {
                if (s != null && !modStateReconciliationCoordinator.IsLocked(InstalledMods.ActiveGame))
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
                modStateReconciliationCoordinator.ReconcileModsPublication(
                    InstalledMods.ActiveGame, InstalledMods.RefreshingMods, InstalledMods.LastRefreshAuthoritative,
                    () => CollectionMods.SetMods(v, InstalledMods.ActiveGame));
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.InstalledMods.RefreshingMods).Subscribe(s =>
            {
                modStateReconciliationCoordinator.ReconcileRefreshPublication(
                    InstalledMods.ActiveGame, s, InstalledMods.LastRefreshAuthoritative,
                    () => CollectionMods.HandleModRefresh(s, InstalledMods.Mods, InstalledMods.ActiveGame));
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.CollectionMods.NeedsModListRefresh).Where(x => x).Subscribe(async _ =>
            {
                await InstalledMods.RefreshModsAsync();
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.InstalledMods.GameChangedRefresh).Where(x => x).Subscribe(_ =>
            {
                CollectionMods.ReloadModCollection();
            }).DisposeWith(disposables);

            this.WhenAnyValue(p => p.InstalledMods.PerformingEnableAll).Subscribe(s =>
            {
                CollectionMods.HandleEnableAllToggled(s, InstalledMods.AllModsEnabled, InstalledMods.FilteredMods);
            }).DisposeWith(disposables);

            ApplyCommand = ReactiveCommand.Create(() =>
            {
                ApplyCollectionAsync(interaction.BeginOperation(), validateParadoxLauncher: true).ConfigureAwait(true);
            }, applyEnabled).DisposeWith(disposables);

            AnalyzeCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var game = gameService.GetSelected();
                if (game != null && CollectionMods.SelectedMods?.Any(p => !p.IsVirtual) == true && CollectionMods.SelectedModCollection != null)
                {
                    var messageState = promptNotificationsService.Get();
                    if (!messageState.ConflictSolverPromptShown)
                    {
                        var title = interaction.GetText(LocalizationResources.Mod_Actions.ConflictSolver.FirstUsePrompt.Title);
                        var message = interaction.GetText(LocalizationResources.Mod_Actions.ConflictSolver.FirstUsePrompt.Message);
                        await interaction.PromptAsync(title, title, message, NotificationType.Info, PromptType.OK);
                        messageState.ConflictSolverPromptShown = true;
                        promptNotificationsService.Save(messageState);
                    }

                    var id = interaction.BeginOperation();
                    await TriggerOverlayAsync(id, true, interaction.GetText(LocalizationResources.App.WaitBackgroundOperationMessage));
                    await shutDownState.WaitUntilFreeAsync();
                    modPatchCollectionService.ResetPatchStateCache();
                    if (game.AdvancedFeatures == GameAdvancedFeatures.Full)
                    {
                        var mode = await modPatchCollectionService.GetPatchStateModeAsync(CollectionMods.SelectedModCollection.Name);
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

            async Task ensureSteamIsRunning(IGameSettings args)
            {
                if (gameService.IsSteamGame(args))
                {
                    await externalProcessHandlerService.LaunchSteamAsync(gameService.GetSelected(), gameService.IsFlatpakSteamGame(args));
                }
            }

            async Task launchGame(bool continueGame)
            {
                var game = gameService.GetSelected();
                if (game != null)
                {
                    if (await externalProcessHandlerService.IsParadoxLauncherRunningAsync())
                    {
                        var title = interaction.GetText(LocalizationResources.Notifications.ParadoxLauncherRunning.Title);
                        var message = interaction.GetText(LocalizationResources.Notifications.ParadoxLauncherRunning.Message);
                        interaction.Notify(title, message, NotificationType.Error, 30);
                        return;
                    }

                    var args = gameService.GetLaunchSettings(game, continueGame);
                    if (!string.IsNullOrWhiteSpace(args.ExecutableLocation))
                    {
                        var id = interaction.BeginOperation();
                        await TriggerOverlayAsync(id, true, interaction.GetText(LocalizationResources.Mod_Actions.LaunchGame.Overlay));
                        if (game.RefreshDescriptors)
                        {
                            await modService.DeleteDescriptorsAsync(InstalledMods.Mods);
                            await modService.InstallModsAsync(InstalledMods.Mods);
                        }

                        await ApplyCollectionAsync(id, false, launchingGame: true);
                        await MessageBus.PublishAsync(new LaunchingGameEvent(game.Type));
                        if (gameService.IsSteamLaunchPath(args))
                        {
                            var launched = false;
                            if (gameService.IsFlatpakSteamGame(args))
                            {
                                // ReSharper disable once StringLiteralTypo
                                if (await interaction.OpenFlatpakAsync("com.valvesoftware.Steam", args.ExecutableLocation))
                                {
                                    launched = true;
                                    if (game.CloseAppAfterGameLaunch)
                                    {
                                        await interaction.ExitApplicationAsync();
                                    }
                                }
                            }
                            else
                            {
                                if (await interaction.OpenAsync(args.ExecutableLocation))
                                {
                                    launched = true;
                                    if (game.CloseAppAfterGameLaunch)
                                    {
                                        await interaction.ExitApplicationAsync();
                                    }
                                }
                            }

                            if (!launched)
                            {
                                interaction.Notify(interaction.GetText(LocalizationResources.Mod_Actions.LaunchGame.LaunchError.Title),
                                    interaction.GetText(LocalizationResources.Mod_Actions.LaunchGame.LaunchError.Message), NotificationType.Error, 10);
                                await TriggerOverlayAsync(id, false);
                            }
                        }
                        else
                        {
                            await ensureSteamIsRunning(args);
                            var generateAppIdFile = gameService.IsSteamGame(args) && createSteamAppIdFile;
                            if (await interaction.RunGameAsync(generateAppIdFile, args.ExecutableLocation, game.SteamRoot, game.LinuxProtonVersion, game.SteamAppId, args.LaunchArguments))
                            {
                                if (game.CloseAppAfterGameLaunch)
                                {
                                    await interaction.ExitApplicationAsync();
                                }
                                else
                                {
                                    await TriggerOverlayAsync(id, false);
                                }
                            }
                            else
                            {
                                interaction.Notify(interaction.GetText(LocalizationResources.Mod_Actions.LaunchGame.LaunchError.Title),
                                    interaction.GetText(LocalizationResources.Mod_Actions.LaunchGame.LaunchError.Message), NotificationType.Error, 10);
                                await TriggerOverlayAsync(id, false);
                            }
                        }
                    }
                    else
                    {
                        interaction.Notify(interaction.GetText(LocalizationResources.Mod_Actions.LaunchGame.NotSet.Title),
                            interaction.GetText(LocalizationResources.Mod_Actions.LaunchGame.NotSet.Message), NotificationType.Warning, 10);
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

            AdvancedModeCommand = ReactiveCommand.CreateFromTask(() => runAnalysis(PatchStateMode.Advanced)).DisposeWith(disposables);

            DefaultModeCommand = ReactiveCommand.CreateFromTask(() => runAnalysis(PatchStateMode.Default)).DisposeWith(disposables);

            AnalyzeModeCommand = ReactiveCommand.CreateFromTask(() => runAnalysis(PatchStateMode.ReadOnly)).DisposeWith(disposables);

            AnalyzeModeWithoutLocalizationCommand = ReactiveCommand.CreateFromTask(() => runAnalysis(PatchStateMode.ReadOnlyWithoutLocalization)).DisposeWith(disposables);

            DefaultWithoutLocalizationModeCommand = ReactiveCommand.CreateFromTask(() => runAnalysis(PatchStateMode.DefaultWithoutLocalization)).DisposeWith(disposables);

            AdvancedWithoutLocalizationModeCommand = ReactiveCommand.CreateFromTask(() => runAnalysis(PatchStateMode.AdvancedWithoutLocalization)).DisposeWith(disposables);

            CloseModeCommand = ReactiveCommand.Create(ForceClosePopups).DisposeWith(disposables);

            var previousCollectionNotification = string.Empty;
            CollectionMods.ConflictSolverStateChanged += (collectionName, state) =>
            {
                AnalyzeClass = !state ? InvalidConflictSolverClass : string.Empty;
                if (!state && previousCollectionNotification != collectionName)
                {
                    interaction.Notify(interaction.GetText(LocalizationResources.Notifications.ConflictSolverUpdate.Title),
                        interaction.GetText(LocalizationResources.Notifications.ConflictSolverUpdate.Message), NotificationType.Warning, 30);
                    previousCollectionNotification = collectionName;
                }
            };

            eventCoordinator.SubscribeDirectoryChanges(async s =>
            {
                AllowModSelection = false;
                InstalledMods.AllowModSelection = false;
                CollectionMods.AllowModSelection = false;
                var recovered = await modStateReconciliationCoordinator.RevalidateConfigurationAsync(
                    s.Game,
                    (revalidationLock, isCurrent) => InstalledMods.RevalidateModsAsync(
                        s.Game, revalidationLock, isCurrent),
                    s.CustomDirectoryChanged ? () => CollectionMods.Reset(true) : null,
                    revalidationLock => CollectionMods.SetMods(
                        InstalledMods.Mods, InstalledMods.ActiveGame, revalidationLock),
                    s.CustomDirectoryChanged
                        ? (revalidationLock, _) => InstalledMods.SynchronizeNewModsAsync(s.Game, revalidationLock)
                        : null);
                if (recovered)
                {
                    var allowSelection = CollectionMods.SelectedModCollection != null;
                    AllowModSelection = allowSelection;
                    InstalledMods.AllowModSelection = allowSelection;
                    CollectionMods.AllowModSelection = allowSelection;
                }

                EvalResumeAvailability(s.Game);
            }).DisposeWith(disposables);

            eventCoordinator.SubscribeInstallRefresh(async m =>
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
            ShowAdvancedFeatures = game?.AdvancedFeatures != GameAdvancedFeatures.None;
            base.OnSelectedGameChanged(game);
        }

        /// <summary>
        /// Shows the invalid mods notification asynchronous.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <returns>Task.</returns>
        protected virtual async Task ShowInvalidModsNotificationAsync(IReadOnlyCollection<IModInstallationResult> mods)
        {
            var title = interaction.GetText(LocalizationResources.InvalidModsDetected.Title);
            var message = interaction.GetText(LocalizationResources.InvalidModsDetected.Message).FormatIronySmart(new { Environment.NewLine, Mods = string.Join(Environment.NewLine, mods.Select(p => p.Path)) });

            if (!showingInvalidNotification)
            {
                showingInvalidNotification = true;
                await interaction.PromptAsync(title, title, message, NotificationType.Error, PromptType.OK);
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
            conflictAnalysisProgressCoordinator.Subscribe(id, disposables, totalSteps, TriggerOverlay);
        }

        #endregion Methods
    }
}
