// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 06-01-2020
// ***********************************************************************
// <copyright file="ModHolderControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using IronyModManager.Common.Events;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation.Actions;
using IronyModManager.Localization;
using IronyModManager.Localization.Attributes;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
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
        /// The application action
        /// </summary>
        private readonly IAppAction appAction;

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

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModHolderControlViewModel" /> class.
        /// </summary>
        /// <param name="modService">The mod service.</param>
        /// <param name="modPatchCollectionService">The mod patch collection service.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="notificationAction">The notification action.</param>
        /// <param name="appAction">The application action.</param>
        /// <param name="localizationManager">The localization manager.</param>
        /// <param name="installedModsControlViewModel">The installed mods control view model.</param>
        /// <param name="collectionModsControlViewModel">The collection mods control view model.</param>
        /// <param name="logger">The logger.</param>
        public ModHolderControlViewModel(IModService modService, IModPatchCollectionService modPatchCollectionService, IGameService gameService,
            INotificationAction notificationAction, IAppAction appAction, ILocalizationManager localizationManager,
            InstalledModsControlViewModel installedModsControlViewModel, CollectionModsControlViewModel collectionModsControlViewModel, ILogger logger)
        {
            this.modService = modService;
            this.modPatchCollectionService = modPatchCollectionService;
            this.notificationAction = notificationAction;
            this.localizationManager = localizationManager;
            this.gameService = gameService;
            this.logger = logger;
            this.appAction = appAction;
            InstalledMods = installedModsControlViewModel;
            CollectionMods = collectionModsControlViewModel;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the analyze.
        /// </summary>
        /// <value>The analyze.</value>
        [StaticLocalization(LocalizationResources.Mod_Actions.Conflict)]
        public virtual string Analyze { get; protected set; }

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
        /// Gets or sets the collection mods.
        /// </summary>
        /// <value>The collection mods.</value>
        public virtual CollectionModsControlViewModel CollectionMods { get; protected set; }

        /// <summary>
        /// Gets or sets the installed mods.
        /// </summary>
        /// <value>The installed mods.</value>
        public virtual InstalledModsControlViewModel InstalledMods { get; protected set; }

        /// <summary>
        /// Gets or sets the launch game.
        /// </summary>
        /// <value>The launch game.</value>
        [StaticLocalization(LocalizationResources.Mod_Actions.LaunchGame)]
        public virtual string LaunchGame { get; protected set; }

        /// <summary>
        /// Gets or sets the launch game command.
        /// </summary>
        /// <value>The launch game command.</value>
        public virtual ReactiveCommand<Unit, Unit> LaunchGameCommand { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Analyzes the mods asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        protected virtual async Task AnalyzeModsAsync()
        {
            var game = gameService.GetSelected();
            if (game != null && CollectionMods.SelectedMods?.Count > 0 && CollectionMods.SelectedModCollection != null)
            {
                MessageBus.Current.SendMessage(new ForceClosePopulsEventArgs());
                var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Conflict_Solver_Progress), new
                {
                    PercentDone = 0,
                    Count = 1,
                    TotalCount = 3
                });
                var message = localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Conflict_Solver_Loading_Definitions);
                await TriggerOverlayAsync(true, message, overlayProgress);
                var definitions = await Task.Run(() =>
                {
                    return modPatchCollectionService.GetModObjects(game, CollectionMods.SelectedMods);
                });
                var conflicts = await Task.Run(() =>
                {
                    if (definitions != null)
                    {
                        return modPatchCollectionService.FindConflicts(definitions, CollectionMods.SelectedModCollection.Mods.ToList());
                    }
                    return null;
                });
                var syncedConflicts = await Task.Run(async () =>
                {
                    return await modPatchCollectionService.LoadPatchStateAsync(conflicts, CollectionMods.SelectedModCollection.Name);
                });
                if (syncedConflicts != null)
                {
                    conflicts = syncedConflicts;
                }
                await TriggerOverlayAsync(false);
                var args = new NavigationEventArgs()
                {
                    SelectedCollection = CollectionMods.SelectedModCollection,
                    Results = conflicts,
                    State = NavigationState.ConflictSolver,
                    SelectedMods = CollectionMods.SelectedMods.Select(p => p.Name).ToList()
                };
                MessageBus.Current.SendMessage(args);
            }
        }

        /// <summary>
        /// apply collection as an asynchronous operation.
        /// </summary>
        protected virtual async Task ApplyCollectionAsync()
        {
            if (ApplyingCollection)
            {
                return;
            }
            ApplyingCollection = true;
            if (CollectionMods.SelectedModCollection != null)
            {
                await TriggerOverlayAsync(true, localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Apply_Message));
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
                await TriggerOverlayAsync(false);
            }
            ApplyingCollection = false;
        }

        /// <summary>
        /// install mods as an asynchronous operation.
        /// </summary>
        protected virtual async Task InstallModsAsync()
        {
            if (await modService.InstallModsAsync())
            {
                if (InstalledMods.IsActivated)
                {
                    InstalledMods.RefreshMods();
                }
            }
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            var applyEnabled = this.WhenAnyValue(v => v.ApplyingCollection, v => !v);

            this.WhenAnyValue(v => v.CollectionMods.SelectedModCollection).Where(v => v != null).Subscribe(s =>
            {
                InstallModsAsync().ConfigureAwait(true);
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.InstalledMods.Mods).Subscribe(v =>
            {
                CollectionMods.SetMods(v);
            });

            this.WhenAnyValue(v => v.InstalledMods.RefreshingMods).Subscribe(s =>
            {
                CollectionMods.HandleModRefresh(s, InstalledMods.Mods);
            }).DisposeWith(disposables);

            ApplyCommand = ReactiveCommand.Create(() =>
            {
                ApplyCollectionAsync().ConfigureAwait(true);
            }, applyEnabled).DisposeWith(disposables);

            AnalyzeCommand = ReactiveCommand.Create(() =>
            {
                AnalyzeModsAsync().ConfigureAwait(true);
            }).DisposeWith(disposables);

            LaunchGameCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var game = gameService.GetSelected();
                if (game != null)
                {
                    var cmd = gameService.GetLaunchArguments(game);
                    if (!string.IsNullOrWhiteSpace(cmd))
                    {
                        await modService.DeleteDescriptorsAsync(InstalledMods.Mods);
                        await modService.InstallModsAsync();
                        await appAction.OpenAsync(cmd);
                        await appAction.ExitAppAsync();
                    }
                }
            }).DisposeWith(disposables);

            modPatchCollectionService.ModDefinitionLoad += (percentage) =>
            {
                var message = localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Conflict_Solver_Loading_Definitions);
                var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Conflict_Solver_Progress), new
                {
                    PercentDone = percentage,
                    Count = 1,
                    TotalCount = 3
                });
                TriggerOverlay(true, message, overlayProgress);
            };

            modPatchCollectionService.ModDefinitionAnalyze += (percentage) =>
            {
                var message = localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Conflict_Solver_Analyzing_Conflicts);
                var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Conflict_Solver_Progress), new
                {
                    PercentDone = percentage,
                    Count = 2,
                    TotalCount = 3
                });
                TriggerOverlay(true, message, overlayProgress);
            };

            modPatchCollectionService.ModDefinitionPatchLoad += (percentage) =>
            {
                var message = localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Conflict_Solver_Analyzing_Resolved_Conflicts);
                var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Conflict_Solver_Progress), new
                {
                    PercentDone = percentage,
                    Count = 3,
                    TotalCount = 3
                });
                TriggerOverlay(true, message, overlayProgress);
            };

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
