// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-09-2020
//
// Last Modified By : Mario
// Last Modified On : 12-09-2020
// ***********************************************************************
// <copyright file="ModifyCollectionControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using IronyModManager.Common;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.AppState;
using IronyModManager.Implementation.MessageBus;
using IronyModManager.Implementation.Overlay;
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
    /// Class ModifyCollectionControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ModifyCollectionControlViewModel : BaseViewModel
    {
        #region Fields

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
        /// The mod collection service
        /// </summary>
        private readonly IModCollectionService modCollectionService;

        /// <summary>
        /// The mod compress merge progress handler
        /// </summary>
        private readonly ModCompressMergeProgressHandler modCompressMergeProgressHandler;

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
        /// The mod merge progress handler
        /// </summary>
        private readonly ModDefinitionMergeProgressHandler modDefinitionMergeProgressHandler;

        /// <summary>
        /// The mod file merge progress handler
        /// </summary>
        private readonly ModFileMergeProgressHandler modFileMergeProgressHandler;

        /// <summary>
        /// The mod merge service
        /// </summary>
        private readonly IModMergeService modMergeService;

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
        /// The definition progress handler
        /// </summary>
        private IDisposable definitionMergeProgressHandler = null;

        /// <summary>
        /// The file merge progress handler
        /// </summary>
        private IDisposable fileMergeProgressHandler = null;

        /// <summary>
        /// The mod compress progress handler
        /// </summary>
        private IDisposable modCompressProgressHandler = null;

        /// <summary>
        /// The mod invalid replace handler
        /// </summary>
        private IDisposable modInvalidReplaceHandler = null;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyCollectionControlViewModel" /> class.
        /// </summary>
        /// <param name="modDefinitionInvalidReplaceHandler">The mod definition invalid replace handler.</param>
        /// <param name="modService">The mod service.</param>
        /// <param name="idGenerator">The identifier generator.</param>
        /// <param name="modCompressMergeProgressHandler">The mod compress merge progress handler.</param>
        /// <param name="modDefinitionAnalyzeHandler">The mod definition analyze handler.</param>
        /// <param name="modDefinitionLoadHandler">The mod definition load handler.</param>
        /// <param name="modDefinitionMergeProgressHandler">The mod definition merge progress handler.</param>
        /// <param name="modFileMergeProgressHandler">The mod file merge progress handler.</param>
        /// <param name="shutDownState">State of the shut down.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="modMergeService">The mod merge service.</param>
        /// <param name="modCollectionService">The mod collection service.</param>
        /// <param name="modPatchCollectionService">The mod patch collection service.</param>
        /// <param name="localizationManager">The localization manager.</param>
        /// <param name="notificationAction">The notification action.</param>
        public ModifyCollectionControlViewModel(ModDefinitionInvalidReplaceHandler modDefinitionInvalidReplaceHandler, IModService modService, IIDGenerator idGenerator, ModCompressMergeProgressHandler modCompressMergeProgressHandler,
            ModDefinitionAnalyzeHandler modDefinitionAnalyzeHandler, ModDefinitionLoadHandler modDefinitionLoadHandler, ModDefinitionMergeProgressHandler modDefinitionMergeProgressHandler,
            ModFileMergeProgressHandler modFileMergeProgressHandler, IShutDownState shutDownState, IGameService gameService, IModMergeService modMergeService,
            IModCollectionService modCollectionService, IModPatchCollectionService modPatchCollectionService,
            ILocalizationManager localizationManager, INotificationAction notificationAction)
        {
            this.modCollectionService = modCollectionService;
            this.modPatchCollectionService = modPatchCollectionService;
            this.localizationManager = localizationManager;
            this.gameService = gameService;
            this.modMergeService = modMergeService;
            this.modDefinitionLoadHandler = modDefinitionLoadHandler;
            this.modDefinitionAnalyzeHandler = modDefinitionAnalyzeHandler;
            this.modDefinitionMergeProgressHandler = modDefinitionMergeProgressHandler;
            this.shutDownState = shutDownState;
            this.modFileMergeProgressHandler = modFileMergeProgressHandler;
            this.notificationAction = notificationAction;
            this.idGenerator = idGenerator;
            this.modCompressMergeProgressHandler = modCompressMergeProgressHandler;
            this.modService = modService;
            this.modDefinitionInvalidReplaceHandler = modDefinitionInvalidReplaceHandler;
        }

        #endregion Constructors

        #region Enums

        /// <summary>
        /// Enum MergeType
        /// </summary>
        public enum MergeType
        {
            /// <summary>
            /// The basic
            /// </summary>
            Basic,

            /// <summary>
            /// The advanced
            /// </summary>
            Advanced,

            /// <summary>
            /// The compress
            /// </summary>
            Compress
        }

        /// <summary>
        /// Enum ModifyAction
        /// </summary>
        public enum ModifyAction
        {
            /// <summary>
            /// The rename
            /// </summary>
            Rename,

            /// <summary>
            /// The merge
            /// </summary>
            Merge,

            /// <summary>
            /// The duplicate
            /// </summary>
            Duplicate
        }

        #endregion Enums

        #region Properties

        /// <summary>
        /// Gets or sets the active collection.
        /// </summary>
        /// <value>The active collection.</value>
        public virtual IModCollection ActiveCollection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow mod selection].
        /// </summary>
        /// <value><c>true</c> if [allow mod selection]; otherwise, <c>false</c>.</value>
        public virtual bool AllowModSelection { get; set; }

        /// <summary>
        /// Gets or sets the duplicate.
        /// </summary>
        /// <value>The duplicate.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Duplicate)]
        public virtual string Duplicate { get; protected set; }

        /// <summary>
        /// Gets or sets the duplicate command.
        /// </summary>
        /// <value>The duplicate command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<ModifyAction>> DuplicateCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is merge open.
        /// </summary>
        /// <value><c>true</c> if this instance is merge open; otherwise, <c>false</c>.</value>
        public virtual bool IsMergeOpen { get; protected set; }

        /// <summary>
        /// Gets or sets the merge advanced.
        /// </summary>
        /// <value>The merge advanced.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.MergeCollection.Options.Advanced)]
        public virtual string MergeAdvanced { get; protected set; }

        /// <summary>
        /// Gets or sets the merge advanced command.
        /// </summary>
        /// <value>The merge advanced command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<ModifyAction>> MergeAdvancedCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the merge basic.
        /// </summary>
        /// <value>The merge basic.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.MergeCollection.Options.Basic)]
        public virtual string MergeBasic { get; protected set; }

        /// <summary>
        /// Gets or sets the merge basic command.
        /// </summary>
        /// <value>The merge basic command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<ModifyAction>> MergeBasicCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the merge close.
        /// </summary>
        /// <value>The merge close.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.MergeCollection.Options.Close)]
        public virtual string MergeClose { get; protected set; }

        /// <summary>
        /// Gets or sets the merge close command.
        /// </summary>
        /// <value>The merge close command.</value>
        public virtual ReactiveCommand<Unit, Unit> MergeCloseCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the merge compress.
        /// </summary>
        /// <value>The merge compress.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.MergeCollection.Options.Compress)]
        public virtual string MergeCompress { get; protected set; }

        /// <summary>
        /// Gets or sets the merge compress command.
        /// </summary>
        /// <value>The merge compress command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<ModifyAction>> MergeCompressCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the merge open.
        /// </summary>
        /// <value>The merge open.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.MergeCollection.Name)]
        public virtual string MergeOpen { get; protected set; }

        /// <summary>
        /// Gets or sets the merge open command.
        /// </summary>
        /// <value>The merge open command.</value>
        public virtual ReactiveCommand<Unit, Unit> MergeOpenCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the merge select mode.
        /// </summary>
        /// <value>The merge select mode.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.MergeCollection.Options.Title)]
        public virtual string MergeSelectMode { get; protected set; }

        /// <summary>
        /// Gets or sets the rename.
        /// </summary>
        /// <value>The rename.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Rename)]
        public virtual string Rename { get; protected set; }

        /// <summary>
        /// Gets or sets the rename command.
        /// </summary>
        /// <value>The rename command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<ModifyAction>> RenameCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the selected mods.
        /// </summary>
        /// <value>The selected mods.</value>
        public virtual IEnumerable<IMod> SelectedMods { get; set; }

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
        public void ForceClosePopups()
        {
            IsMergeOpen = false;
        }

        /// <summary>
        /// Copies the collection.
        /// </summary>
        /// <param name="requestedName">Name of the requested.</param>
        /// <param name="skipNameCheck">if set to <c>true</c> [skip name check].</param>
        /// <returns>IModCollection.</returns>
        protected virtual IModCollection CopyCollection(string requestedName, bool skipNameCheck = false)
        {
            string name = requestedName;
            if (!skipNameCheck)
            {
                var collections = modCollectionService.GetAll();
                var count = collections.Where(p => p.Name.Equals(requestedName, StringComparison.OrdinalIgnoreCase)).Count();
                name = string.Empty;
                if (count == 0)
                {
                    name = requestedName;
                }
                else
                {
                    name = $"{requestedName} ({count})";
                }
                while (collections.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    count++;
                    name = $"{requestedName} ({count})";
                }
            }
            var copied = modCollectionService.Create();
            copied.IsSelected = true;
            copied.Mods = ActiveCollection.Mods;
            copied.Name = name;
            return copied;
        }

        /// <summary>
        /// get merged collection as an asynchronous operation.
        /// </summary>
        /// <returns>IModCollection.</returns>
        protected virtual async Task<IModCollection> GetMergedCollectionAsync()
        {
            string prefix = localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.MergedCollectionPrefix);
            var requestedName = $"{prefix} {ActiveCollection.Name}";
            var exists = modCollectionService.GetAll().Any(p => p.Name.Equals(requestedName, StringComparison.OrdinalIgnoreCase));
            bool skipNameCheck = false;
            if (exists)
            {
                var title = localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.OverwritePrompt.Title);
                var message = localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.OverwritePrompt.Message);
                skipNameCheck = await notificationAction.ShowPromptAsync(title, title, message, NotificationType.Info);
            }
            var copy = CopyCollection(requestedName, skipNameCheck);
            copy.MergedFolderName = copy.Name.GenerateValidFileName();
            return copy;
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            ShowAdvancedFeatures = (gameService.GetSelected()?.AdvancedFeaturesSupported).GetValueOrDefault();

            var allowModSelectionEnabled = this.WhenAnyValue(v => v.AllowModSelection);

            RenameCommand = ReactiveCommand.Create(() =>
            {
                return new CommandResult<ModifyAction>(ModifyAction.Rename, CommandState.Success);
            }, allowModSelectionEnabled).DisposeWith(disposables);

            DuplicateCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (ActiveCollection != null)
                {
                    var copy = CopyCollection(ActiveCollection.Name);
                    if (modCollectionService.Save(copy))
                    {
                        var id = idGenerator.GetNextId();
                        await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.Collection_Mods.Overlay_Duplicate_Message));
                        await Task.Run(async () =>
                        {
                            await modPatchCollectionService.CopyPatchCollectionAsync(ActiveCollection.Name, copy.Name).ConfigureAwait(false);
                        }).ConfigureAwait(false);
                        await TriggerOverlayAsync(id, false);
                        return new CommandResult<ModifyAction>(ModifyAction.Duplicate, CommandState.Success);
                    }
                    else
                    {
                        return new CommandResult<ModifyAction>(ModifyAction.Duplicate, CommandState.Failed);
                    }
                }
                return new CommandResult<ModifyAction>(ModifyAction.Duplicate, CommandState.NotExecuted);
            }, allowModSelectionEnabled).DisposeWith(disposables);

            MergeOpenCommand = ReactiveCommand.Create(() =>
            {
                IsMergeOpen = true;
            }, allowModSelectionEnabled).DisposeWith(disposables);

            MergeCloseCommand = ReactiveCommand.Create(() =>
            {
                IsMergeOpen = false;
            }).DisposeWith(disposables);

            MergeAdvancedCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (ActiveCollection != null && ActiveCollection.Mods?.Count() > 0)
                {
                    var copy = await GetMergedCollectionAsync();

                    var id = idGenerator.GetNextId();
                    await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.App.WaitBackgroundOperationMessage));
                    await shutDownState.WaitUntilFreeAsync();

                    var savedCollection = modCollectionService.GetAll().FirstOrDefault(p => p.IsSelected);
                    while (savedCollection == null)
                    {
                        await Task.Delay(25);
                        savedCollection = modCollectionService.GetAll().FirstOrDefault(p => p.IsSelected);
                    }

                    SubscribeToProgressReports(id, disposables, MergeType.Advanced);

                    var mode = await modPatchCollectionService.GetPatchStateModeAsync(ActiveCollection.Name);
                    if (mode == PatchStateMode.None)
                    {
                        // fallback to default mod if no patch collection specified
                        mode = PatchStateMode.Default;
                    }

                    var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Overlay_Progress), new
                    {
                        PercentDone = 0.ToLocalizedPercentage(),
                        Count = 1,
                        TotalCount = 4
                    });
                    var message = localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Advanced.Overlay_Loading_Definitions);
                    await TriggerOverlayAsync(id, true, message, overlayProgress);

                    modPatchCollectionService.ResetPatchStateCache();
                    var definitions = await Task.Run(async () =>
                    {
                        return await modPatchCollectionService.GetModObjectsAsync(gameService.GetSelected(), SelectedMods, copy.Name).ConfigureAwait(false);
                    }).ConfigureAwait(false);

                    var conflicts = await Task.Run(() =>
                    {
                        if (definitions != null)
                        {
                            return modPatchCollectionService.FindConflicts(definitions, SelectedMods.Select(p => p.Name).ToList(), mode);
                        }
                        return null;
                    }).ConfigureAwait(false);

                    await modPatchCollectionService.CleanPatchCollectionAsync(copy.Name);

                    var mergeMod = await Task.Run(async () =>
                    {
                        return await modMergeService.MergeCollectionByDefinitionsAsync(conflicts, SelectedMods.Select(p => p.Name).ToList(), copy.Name).ConfigureAwait(false);
                    }).ConfigureAwait(false);
                    copy.Mods = new List<string>() { mergeMod.DescriptorFile };

                    await TriggerOverlayAsync(id, false);

                    definitionAnalyzeLoadHandler?.Dispose();
                    definitionLoadHandler?.Dispose();
                    definitionMergeProgressHandler?.Dispose();

                    modCollectionService.Delete(copy.Name);
                    modPatchCollectionService.InvalidatePatchModState(copy.Name);
                    if (modCollectionService.Save(copy))
                    {
                        return new CommandResult<ModifyAction>(ModifyAction.Merge, CommandState.Success);
                    }
                    else
                    {
                        return new CommandResult<ModifyAction>(ModifyAction.Merge, CommandState.Failed);
                    }
                }
                return new CommandResult<ModifyAction>(ModifyAction.Merge, CommandState.NotExecuted);
            }, allowModSelectionEnabled).DisposeWith(disposables);

            MergeBasicCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (ActiveCollection != null && ActiveCollection.Mods?.Count() > 0)
                {
                    var copy = await GetMergedCollectionAsync();
                    var id = idGenerator.GetNextId();

                    await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.App.WaitBackgroundOperationMessage));
                    await shutDownState.WaitUntilFreeAsync();

                    var savedCollection = modCollectionService.GetAll().FirstOrDefault(p => p.IsSelected);
                    while (savedCollection == null)
                    {
                        await Task.Delay(25);
                        savedCollection = modCollectionService.GetAll().FirstOrDefault(p => p.IsSelected);
                    }

                    SubscribeToProgressReports(id, disposables, MergeType.Basic);

                    var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Overlay_Progress), new
                    {
                        PercentDone = 0.ToLocalizedPercentage(),
                        Count = 1,
                        TotalCount = 2
                    });
                    var message = localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Basic.Overlay_Gathering_Mod_Info);
                    await TriggerOverlayAsync(id, true, message, overlayProgress);

                    await modPatchCollectionService.CleanPatchCollectionAsync(copy.Name);

                    var mergeMod = await Task.Run(async () =>
                    {
                        return await modMergeService.MergeCollectionByFilesAsync(copy.Name);
                    }).ConfigureAwait(false);
                    copy.Mods = new List<string>() { mergeMod.DescriptorFile };

                    await TriggerOverlayAsync(id, false);
                    fileMergeProgressHandler?.Dispose();

                    modCollectionService.Delete(copy.Name);
                    modPatchCollectionService.InvalidatePatchModState(copy.Name);
                    if (modCollectionService.Save(copy))
                    {
                        return new CommandResult<ModifyAction>(ModifyAction.Merge, CommandState.Success);
                    }
                    else
                    {
                        return new CommandResult<ModifyAction>(ModifyAction.Merge, CommandState.Failed);
                    }
                }
                return new CommandResult<ModifyAction>(ModifyAction.Merge, CommandState.NotExecuted);
            }, allowModSelectionEnabled).DisposeWith(disposables);

            MergeCompressCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (ActiveCollection != null && ActiveCollection.Mods?.Count() > 0)
                {
                    var copy = await GetMergedCollectionAsync();
                    var id = idGenerator.GetNextId();

                    await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.App.WaitBackgroundOperationMessage));
                    await shutDownState.WaitUntilFreeAsync();

                    var savedCollection = modCollectionService.GetAll().FirstOrDefault(p => p.IsSelected);
                    while (savedCollection == null)
                    {
                        await Task.Delay(25);
                        savedCollection = modCollectionService.GetAll().FirstOrDefault(p => p.IsSelected);
                    }

                    SubscribeToProgressReports(id, disposables, MergeType.Compress);

                    var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Overlay_Progress), new
                    {
                        PercentDone = 0.ToLocalizedPercentage(),
                        Count = 1,
                        TotalCount = 2
                    });
                    var message = localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Compress.Overlay_Gathering_Mod_Info);
                    await TriggerOverlayAsync(id, true, message, overlayProgress);

                    await modPatchCollectionService.CleanPatchCollectionAsync(copy.Name);

                    var mergeMods = await Task.Run(async () =>
                    {
                        return await modMergeService.MergeCompressCollectionAsync(copy.Name, Smart.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.MergeCompressModPrefix), new
                        {
                            Name = copy.Name.Replace($"{localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.MergedCollectionPrefix)} ", string.Empty)
                        }));
                    }).ConfigureAwait(false);
                    copy.Mods = mergeMods.Select(p => p.DescriptorFile).ToList();

                    await TriggerOverlayAsync(id, false);
                    fileMergeProgressHandler?.Dispose();

                    modCollectionService.Delete(copy.Name);
                    modPatchCollectionService.InvalidatePatchModState(copy.Name);
                    await modService.InstallModsAsync(null);
                    if (modCollectionService.Save(copy))
                    {
                        return new CommandResult<ModifyAction>(ModifyAction.Merge, CommandState.Success);
                    }
                    else
                    {
                        return new CommandResult<ModifyAction>(ModifyAction.Merge, CommandState.Failed);
                    }
                }
                return new CommandResult<ModifyAction>(ModifyAction.Merge, CommandState.NotExecuted);
            }, allowModSelectionEnabled).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// Called when [selected game changed].
        /// </summary>
        /// <param name="game">The game.</param>
        protected override void OnSelectedGameChanged(IGame game)
        {
            ShowAdvancedFeatures = (game?.AdvancedFeaturesSupported).GetValueOrDefault();
            base.OnSelectedGameChanged(game);
        }

        /// <summary>
        /// Subscribes to progress reports.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="disposables">The disposables.</param>
        /// <param name="mergeType">Type of the merge.</param>
        protected virtual void SubscribeToProgressReports(long id, CompositeDisposable disposables, MergeType mergeType)
        {
            definitionLoadHandler?.Dispose();
            definitionAnalyzeLoadHandler?.Dispose();
            definitionMergeProgressHandler?.Dispose();
            fileMergeProgressHandler?.Dispose();
            modCompressProgressHandler?.Dispose();
            modInvalidReplaceHandler?.Dispose();

            if (mergeType == MergeType.Advanced)
            {
                definitionLoadHandler = modDefinitionLoadHandler.Message.Subscribe(s =>
                {
                    var message = localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Advanced.Overlay_Loading_Definitions);
                    var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Overlay_Progress), new
                    {
                        PercentDone = s.Percentage.ToLocalizedPercentage(),
                        Count = 1,
                        TotalCount = 4
                    });
                    TriggerOverlay(id, true, message, overlayProgress);
                }).DisposeWith(disposables);
                modInvalidReplaceHandler = modDefinitionInvalidReplaceHandler.Message.Subscribe(s =>
                {
                    var message = localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Advanced.Overlay_Replacing_Definitions);
                    var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Overlay_Progress), new
                    {
                        PercentDone = s.Percentage.ToLocalizedPercentage(),
                        Count = 2,
                        TotalCount = 4
                    });
                    TriggerOverlay(id, true, message, overlayProgress);
                }).DisposeWith(disposables);
                definitionAnalyzeLoadHandler = modDefinitionAnalyzeHandler.Message.Subscribe(s =>
                {
                    var message = localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Advanced.Overlay_Analyzing_Definitions);
                    var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Overlay_Progress), new
                    {
                        PercentDone = s.Percentage.ToLocalizedPercentage(),
                        Count = 3,
                        TotalCount = 4
                    });
                    TriggerOverlay(id, true, message, overlayProgress);
                }).DisposeWith(disposables);
                definitionMergeProgressHandler = modDefinitionMergeProgressHandler.Message.Subscribe(s =>
                {
                    var message = localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Advanced.Overlay_Merging_Collection);
                    var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Overlay_Progress), new
                    {
                        PercentDone = s.Percentage.ToLocalizedPercentage(),
                        Count = 4,
                        TotalCount = 4
                    });
                    TriggerOverlay(id, true, message, overlayProgress);
                }).DisposeWith(disposables);
            }
            else if (mergeType == MergeType.Basic)
            {
                fileMergeProgressHandler = modFileMergeProgressHandler.Message.Subscribe(s =>
                {
                    string message;
                    if (s.Step == 1)
                    {
                        message = localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Basic.Overlay_Gathering_Mod_Info);
                    }
                    else
                    {
                        message = localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Basic.Overlay_Writting_Files);
                    }
                    var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Overlay_Progress), new
                    {
                        PercentDone = s.Percentage.ToLocalizedPercentage(),
                        Count = s.Step,
                        TotalCount = 2
                    });
                    TriggerOverlay(id, true, message, overlayProgress);
                }).DisposeWith(disposables);
            }
            else
            {
                modCompressProgressHandler = modCompressMergeProgressHandler.Message.Subscribe(s =>
                {
                    string message;
                    if (s.Step == 1)
                    {
                        message = localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Compress.Overlay_Gathering_Mod_Info);
                    }
                    else
                    {
                        message = localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Compress.Overlay_Compressing_Files);
                    }
                    var overlayProgress = Smart.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.MergeCollection.Overlay_Progress), new
                    {
                        PercentDone = s.Percentage.ToLocalizedPercentage(),
                        Count = s.Step,
                        TotalCount = 2
                    });
                    TriggerOverlay(id, true, message, overlayProgress);
                }).DisposeWith(disposables);
            }
        }

        #endregion Methods
    }
}
