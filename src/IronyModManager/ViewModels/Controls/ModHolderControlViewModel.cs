// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2021
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
using IronyModManager.Common;
using IronyModManager.Common.Events;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.AppState;
using IronyModManager.Implementation.MessageBus;
using IronyModManager.Implementation.Overlay;
using IronyModManager.Localization;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.MessageBus.Events;
using ReactiveUI;
using SmartFormat;

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
        /// The steam launch
        /// </summary>
        private const string SteamLaunch = SteamProcess + "://open/main";

        /// <summary>
        /// The steam process
        /// </summary>
        private const string SteamProcess = "steam";

        /// <summary>
        /// The application action
        /// </summary>
        private readonly IAppAction appAction;

        /// <summary>
        /// The game directory changed handler
        /// </summary>
        private readonly GameUserDirectoryChangedHandler gameDirectoryChangedHandler;

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
        public ModHolderControlViewModel(ModDefinitionInvalidReplaceHandler modDefinitionInvalidReplaceHandler, IIDGenerator idGenerator, IShutDownState shutDownState,
            IModService modService, IModPatchCollectionService modPatchCollectionService, IGameService gameService,
            INotificationAction notificationAction, IAppAction appAction, ILocalizationManager localizationManager,
            InstalledModsControlViewModel installedModsControlViewModel, CollectionModsControlViewModel collectionModsControlViewModel,
            ModDefinitionAnalyzeHandler modDefinitionAnalyzeHandler, ModDefinitionLoadHandler modDefinitionLoadHandler, ModDefinitionPatchLoadHandler modDefinitionPatchLoadHandler,
            GameUserDirectoryChangedHandler gameDirectoryChangedHandler, ILogger logger)
        {
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
            InstalledMods = installedModsControlViewModel;
            CollectionMods = collectionModsControlViewModel;
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
        /// Gets or sets a value indicating whether [allow mod selection].
        /// </summary>
        /// <value><c>true</c> if [allow mod selection]; otherwise, <c>false</c>.</value>
        public virtual bool AllowModSelection { get; protected set; }

        /// <summary>
        /// Gets or sets the analyze.
        /// </summary>
        /// <value>The analyze.</value>
        [StaticLocalization(LocalizationResources.Mod_Actions.Conflict)]
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
        /// <returns>Task.</returns>
        protected virtual async Task AnalyzeModsAsync(long id, PatchStateMode mode)
        {
            SubscribeToProgressReport(id, Disposables);

            var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Conflict_Solver_Progress), new
            {
                PercentDone = 0.ToLocalizedPercentage(),
                Count = 1,
                TotalCount = 4
            });
            var message = localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Conflict_Solver_Loading_Definitions);
            await TriggerOverlayAsync(id, true, message, overlayProgress);
            modPatchCollectionService.InvalidatePatchModState(CollectionMods.SelectedModCollection.Name);
            modPatchCollectionService.ResetPatchStateCache();

            var definitions = await Task.Run(async () =>
            {
                return await modPatchCollectionService.GetModObjectsAsync(gameService.GetSelected(), CollectionMods.SelectedMods, CollectionMods.SelectedModCollection.Name).ConfigureAwait(false);
            }).ConfigureAwait(false);
            var conflicts = await Task.Run(() =>
            {
                if (definitions != null)
                {
                    return modPatchCollectionService.FindConflicts(definitions, CollectionMods.SelectedMods.Select(p => p.Name).ToList(), mode);
                }
                return null;
            }).ConfigureAwait(false);
            var syncedConflicts = await Task.Run(async () =>
            {
                return await modPatchCollectionService.SyncPatchStateAsync(conflicts, CollectionMods.SelectedModCollection.Name).ConfigureAwait(false);
            }).ConfigureAwait(false);
            if (syncedConflicts != null)
            {
                conflicts = syncedConflicts;
            }
            var args = new NavigationEventArgs()
            {
                SelectedCollection = CollectionMods.SelectedModCollection,
                Results = conflicts,
                State = NavigationState.ConflictSolver,
                SelectedMods = CollectionMods.SelectedMods.Select(p => p.Name).ToList()
            };
            ReactiveUI.MessageBus.Current.SendMessage(args);
            await TriggerOverlayAsync(id, false);

            definitionAnalyzeLoadHandler?.Dispose();
            definitionLoadHandler?.Dispose();
            definitionSyncHandler?.Dispose();
        }

        /// <summary>
        /// apply collection as an asynchronous operation.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="showOverlay">if set to <c>true</c> [show overlay].</param>
        protected virtual async Task ApplyCollectionAsync(long id, bool showOverlay = true)
        {
            if (ApplyingCollection)
            {
                return;
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
                    var result = await modService.ExportModsAsync(CollectionMods.SelectedMods.ToList(), InstalledMods.AllMods.ToList(), CollectionMods.SelectedModCollection.Name);
                    string title;
                    string message;
                    if (result)
                    {
                        title = localizationManager.GetResource(LocalizationResources.Notifications.CollectionApplied.Title);
                        message = Smart.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionApplied.Message), new { CollectionName = CollectionMods.SelectedModCollection.Name });
                    }
                    else
                    {
                        title = localizationManager.GetResource(LocalizationResources.Notifications.CollectionNotApplied.Title);
                        message = Smart.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionNotApplied.Message), new { CollectionName = CollectionMods.SelectedModCollection.Name });
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
            if (game == null)
            {
                game = gameService.GetSelected();
            }
            if (game != null)
            {
                ResumeGameVisible = gameService.IsContinueGameAllowed(game);
            }
            else
            {
                ResumeGameVisible = false;
            }
        }

        /// <summary>
        /// eval resume availability loop as an asynchronous operation.
        /// </summary>
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
        protected virtual async Task InstallModsAsync()
        {
            var result = await modService.InstallModsAsync(InstalledMods.Mods);
            if (result != null)
            {
                if (result.Any(p => p.Installed == true))
                {
                    if (InstalledMods.IsActivated)
                    {
                        InstalledMods.RefreshMods();
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
            Task.Run(() => EvalResumeAvailabilityLoopAsync().ConfigureAwait(false));

            ShowAdvancedFeatures = (gameService.GetSelected()?.AdvancedFeaturesSupported).GetValueOrDefault();
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

            this.WhenAnyValue(v => v.CollectionMods.NeedsModListRefresh).Where(x => x).Subscribe(s =>
            {
                InstalledMods.RefreshMods();
            }).DisposeWith(disposables);

            this.WhenAnyValue(p => p.InstalledMods.PerformingEnableAll).Subscribe(s =>
            {
                CollectionMods.HandleEnableAllToggled(s, InstalledMods.AllModsEnabled, InstalledMods.FilteredMods);
            }).DisposeWith(disposables);

            ApplyCommand = ReactiveCommand.Create(() =>
            {
                ApplyCollectionAsync(idGenerator.GetNextId()).ConfigureAwait(true);
            }, applyEnabled).DisposeWith(disposables);

            AnalyzeCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var game = gameService.GetSelected();
                if (game != null && CollectionMods.SelectedMods?.Count > 0 && CollectionMods.SelectedModCollection != null)
                {
                    var id = idGenerator.GetNextId();
                    await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.App.WaitBackgroundOperationMessage));
                    await shutDownState.WaitUntilFreeAsync();
                    modPatchCollectionService.ResetPatchStateCache();
                    var mode = await modPatchCollectionService.GetPatchStateModeAsync(CollectionMods.SelectedModCollection.Name);
                    if (mode == PatchStateMode.None)
                    {
                        await TriggerOverlayAsync(id, false);
                        await Task.Delay(50);
                        IsModeOpen = true;
                    }
                    else
                    {
                        await AnalyzeModsAsync(id, mode);
                    }
                }
            }, allowModSelectionEnabled).DisposeWith(disposables);

            async Task<bool> ensureSteamIsRunning(IGameSettings args)
            {
                if (gameService.IsSteamGame(args))
                {
                    // Check if process is running
                    var processes = Process.GetProcesses();
                    if (!processes.Any(p => p.ProcessName.Equals(SteamProcess, StringComparison.OrdinalIgnoreCase)))
                    {
                        await appAction.OpenAsync(SteamLaunch);
                        var attempts = 0;
                        while (!processes.Any(p => p.ProcessName.Equals(SteamProcess, StringComparison.OrdinalIgnoreCase)))
                        {
                            if (attempts > 3)
                            {
                                break;
                            }
                            await Task.Delay(3000);
                            processes = Process.GetProcesses();
                            attempts++;
                        }
                    }
                }
                return true;
            }

            async Task launchGame(bool continueGame)
            {
                var game = gameService.GetSelected();
                if (game != null)
                {
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

            AdvancedModeCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await AnalyzeModsAsync(idGenerator.GetNextId(), PatchStateMode.Advanced);
            }).DisposeWith(disposables);

            DefaultModeCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await AnalyzeModsAsync(idGenerator.GetNextId(), PatchStateMode.Default);
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

            gameDirectoryChangedHandler.Message.Subscribe(s =>
            {
                InstalledMods.RefreshMods();
                EvalResumeAvailability(s.Game);
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.InstalledMods.ModFilePopulationInCompleted).Subscribe(s =>
            {
                CollectionMods.CanExportModHashReport = s;
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// Called when [selected game changed].
        /// </summary>
        /// <param name="game">The game.</param>
        protected override void OnSelectedGameChanged(IGame game)
        {
            EvalResumeAvailability(game);
            ShowAdvancedFeatures = (game?.AdvancedFeaturesSupported).GetValueOrDefault();
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
            var message = localizationManager.GetResource(LocalizationResources.InvalidModsDetected.Message).FormatSmart(new { Environment.NewLine, Mods = string.Join(Environment.NewLine, mods.Select(p => p.Path)) });

            if (!showingInvalidNotification)
            {
                showingInvalidNotification = true;
                await notificationAction.ShowPromptAsync(title, title, message, NotificationType.Info, PromptType.OK);
                showingInvalidNotification = false;
            }
        }

        /// <summary>
        /// Subscribes to progress report.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="disposables">The disposables.</param>
        private void SubscribeToProgressReport(long id, CompositeDisposable disposables)
        {
            definitionLoadHandler?.Dispose();
            definitionLoadHandler = modDefinitionLoadHandler.Message.Subscribe(s =>
            {
                var message = localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Conflict_Solver_Loading_Definitions);
                var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Conflict_Solver_Progress), new
                {
                    PercentDone = s.Percentage.ToLocalizedPercentage(),
                    Count = 1,
                    TotalCount = 4
                });
                TriggerOverlay(id, true, message, overlayProgress);
            }).DisposeWith(disposables);

            modInvalidReplaceHandler?.Dispose();
            modInvalidReplaceHandler = modDefinitionInvalidReplaceHandler.Message.Subscribe(s =>
            {
                var message = localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Conflict_Solver_Replacing_Definitions);
                var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Conflict_Solver_Progress), new
                {
                    PercentDone = s.Percentage.ToLocalizedPercentage(),
                    Count = 2,
                    TotalCount = 4
                });
                TriggerOverlay(id, true, message, overlayProgress);
            }).DisposeWith(disposables);

            definitionAnalyzeLoadHandler?.Dispose();
            definitionAnalyzeLoadHandler = modDefinitionAnalyzeHandler.Message.Subscribe(s =>
            {
                var message = localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Conflict_Solver_Analyzing_Conflicts);
                var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Conflict_Solver_Progress), new
                {
                    PercentDone = s.Percentage.ToLocalizedPercentage(),
                    Count = 3,
                    TotalCount = 4
                });
                TriggerOverlay(id, true, message, overlayProgress);
            }).DisposeWith(disposables);

            definitionSyncHandler?.Dispose();
            definitionSyncHandler = modDefinitionPatchLoadHandler.Message.Subscribe(s =>
            {
                var message = localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Conflict_Solver_Analyzing_Resolved_Conflicts);
                var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Conflict_Solver_Progress), new
                {
                    PercentDone = s.Percentage.ToLocalizedPercentage(),
                    Count = 4,
                    TotalCount = 4
                });
                TriggerOverlay(id, true, message, overlayProgress);
            }).DisposeWith(disposables);
        }

        #endregion Methods
    }
}
