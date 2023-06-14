
// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-18-2020
//
// Last Modified By : Mario
// Last Modified On : 06-24-2023
// ***********************************************************************
// <copyright file="MainConflictSolverViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Threading;
using IronyModManager.Common;
using IronyModManager.Common.Events;
using IronyModManager.Common.ViewModels;
using IronyModManager.DI;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.Hotkey;
using IronyModManager.Implementation.Overlay;
using IronyModManager.Localization;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using IronyModManager.ViewModels.Controls;
using ReactiveUI;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.ViewModels
{

    /// <summary>
    /// Class MainConflictSolverControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class MainConflictSolverControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The invalid key
        /// </summary>
        private const string InvalidKey = "invalid";

        /// <summary>
        /// The localization directory
        /// </summary>
        private static readonly string LocalizationDirectory = $"{Shared.Constants.LocalizationDirectory}{Path.DirectorySeparatorChar}";

        /// <summary>
        /// The application action
        /// </summary>
        private readonly IAppAction appAction;

        /// <summary>
        /// The external process handler service
        /// </summary>
        private readonly IExternalProcessHandlerService externalProcessHandlerService;

        /// <summary>
        /// The hotkey pressed handler
        /// </summary>
        private readonly ConflictSolverViewHotkeyPressedHandler hotkeyPressedHandler;

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
        /// The mod patch collection service
        /// </summary>
        private readonly IModPatchCollectionService modPatchCollectionService;

        /// <summary>
        /// The notification action
        /// </summary>
        private readonly INotificationAction notificationAction;

        /// <summary>
        /// The cached invalids
        /// </summary>
        private IHierarchicalDefinitions cachedInvalids;

        /// <summary>
        /// The filtering conflicts
        /// </summary>
        private bool filteringConflicts = false;

        /// <summary>
        /// The invalids checked
        /// </summary>
        private bool invalidsChecked;

        /// <summary>
        /// The take left binary
        /// </summary>
        private bool takeLeftBinary = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainConflictSolverControlViewModel" /> class.
        /// </summary>
        /// <param name="externalProcessHandlerService">The external process handler service.</param>
        /// <param name="hotkeyPressedHandler">The hotkey pressed handler.</param>
        /// <param name="idGenerator">The identifier generator.</param>
        /// <param name="modPatchCollectionService">The mod patch collection service.</param>
        /// <param name="localizationManager">The localization manager.</param>
        /// <param name="mergeViewer">The merge viewer.</param>
        /// <param name="binaryMergeViewer">The binary merge viewer.</param>
        /// <param name="modCompareSelector">The mod compare selector.</param>
        /// <param name="ignoreConflictsRules">The ignore conflicts rules.</param>
        /// <param name="modFilter">The mod filter.</param>
        /// <param name="resetConflicts">The reset conflicts.</param>
        /// <param name="dbSearch">The database search.</param>
        /// <param name="customConflicts">The custom conflicts.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="notificationAction">The notification action.</param>
        /// <param name="appAction">The application action.</param>
        public MainConflictSolverControlViewModel(IExternalProcessHandlerService externalProcessHandlerService,
            ConflictSolverViewHotkeyPressedHandler hotkeyPressedHandler, IIDGenerator idGenerator,
            IModPatchCollectionService modPatchCollectionService, ILocalizationManager localizationManager,
            MergeViewerControlViewModel mergeViewer, MergeViewerBinaryControlViewModel binaryMergeViewer,
            ModCompareSelectorControlViewModel modCompareSelector, ModConflictIgnoreControlViewModel ignoreConflictsRules,
            ConflictSolverModFilterControlViewModel modFilter, ConflictSolverResetConflictsControlViewModel resetConflicts,
            ConflictSolverDBSearchControlViewModel dbSearch, ConflictSolverCustomConflictsControlViewModel customConflicts,
            ILogger logger, INotificationAction notificationAction, IAppAction appAction)
        {
            this.idGenerator = idGenerator;
            this.modPatchCollectionService = modPatchCollectionService;
            this.localizationManager = localizationManager;
            this.logger = logger;
            this.notificationAction = notificationAction;
            this.appAction = appAction;
            this.hotkeyPressedHandler = hotkeyPressedHandler;
            this.externalProcessHandlerService = externalProcessHandlerService;
            MergeViewer = mergeViewer;
            ModCompareSelector = modCompareSelector;
            BinaryMergeViewer = binaryMergeViewer;
            IgnoreConflictsRules = ignoreConflictsRules;
            ModFilter = modFilter;
            ResetConflicts = resetConflicts;
            DatabaseSearch = dbSearch;
            CustomConflicts = customConflicts;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the back.
        /// </summary>
        /// <value>The back.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.Back)]
        public virtual string Back { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [back allowed].
        /// </summary>
        /// <value><c>true</c> if [back allowed]; otherwise, <c>false</c>.</value>
        public virtual bool BackAllowed { get; protected set; }

        /// <summary>
        /// Gets or sets the back command.
        /// </summary>
        /// <value>The back command.</value>
        public virtual ReactiveCommand<Unit, Unit> BackCommand { get; set; }
        /// <summary>
        /// Gets or sets the binary merge viewer.
        /// </summary>
        /// <value>The binary merge viewer.</value>
        public virtual MergeViewerBinaryControlViewModel BinaryMergeViewer { get; protected set; }

        /// <summary>
        /// Gets or sets the conflicted objects.
        /// </summary>
        /// <value>The conflicted objects.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ConflictedObjects)]
        public virtual string ConflictedObjects { get; protected set; }

        /// <summary>
        /// Gets or sets the conflicts.
        /// </summary>
        /// <value>The conflicts.</value>
        public virtual IConflictResult Conflicts { get; set; }

        /// <summary>
        /// Gets or sets the context menu definition.
        /// </summary>
        /// <value>The context menu definition.</value>
        public virtual IHierarchicalDefinitions ContextMenuDefinition { get; set; }

        /// <summary>
        /// Gets or sets the custom conflicts.
        /// </summary>
        /// <value>The custom conflicts.</value>
        public virtual ConflictSolverCustomConflictsControlViewModel CustomConflicts { get; protected set; }

        /// <summary>
        /// Gets or sets the database search.
        /// </summary>
        /// <value>The database search.</value>
        public virtual ConflictSolverDBSearchControlViewModel DatabaseSearch { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [editing ignore conflicts rules].
        /// </summary>
        /// <value><c>true</c> if [editing ignore conflicts rules]; otherwise, <c>false</c>.</value>
        public virtual bool EditingIgnoreConflictsRules { get; protected set; }

        /// <summary>
        /// Gets or sets the hierarchal conflicts.
        /// </summary>
        /// <value>The hierarchal conflicts.</value>
        public virtual AvaloniaList<IHierarchicalDefinitions> HierarchalConflicts { get; protected set; }

        /// <summary>
        /// Gets or sets the ignore.
        /// </summary>
        /// <value>The ignore.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.Ignore)]
        public virtual string Ignore { get; protected set; }

        /// <summary>
        /// Gets or sets the ignore command.
        /// </summary>
        /// <value>The ignore command.</value>
        public virtual ReactiveCommand<Unit, Unit> IgnoreCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the ignore conflicts rules.
        /// </summary>
        /// <value>The ignore conflicts rules.</value>
        public virtual ModConflictIgnoreControlViewModel IgnoreConflictsRules { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [ignore enabled].
        /// </summary>
        /// <value><c>true</c> if [ignore enabled]; otherwise, <c>false</c>.</value>
        public virtual bool IgnoreEnabled { get; protected set; }

        /// <summary>
        /// Gets or sets the ignore rules.
        /// </summary>
        /// <value>The ignore rules.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.IgnoreRules)]
        public virtual string IgnoreRules { get; protected set; }

        /// <summary>
        /// Gets or sets the ignore rules command.
        /// </summary>
        /// <value>The ignore rules command.</value>
        public virtual ReactiveCommand<Unit, Unit> IgnoreRulesCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the invalid.
        /// </summary>
        /// <value>The invalid.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.InvalidConflicts.Name)]
        public virtual string Invalid { get; protected set; }

        /// <summary>
        /// Gets or sets the invalid conflict path.
        /// </summary>
        /// <value>The invalid conflict path.</value>
        public virtual string InvalidConflictPath { get; protected set; }

        /// <summary>
        /// Gets or sets the invalid custom patch.
        /// </summary>
        /// <value>The invalid custom patch.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.InvalidConflicts.ContextMenu.CustomPatch)]
        public virtual string InvalidCustomPatch { get; protected set; }

        /// <summary>
        /// Gets or sets the invalid custom patch command.
        /// </summary>
        /// <value>The invalid custom patch command.</value>
        public virtual ReactiveCommand<Unit, Unit> InvalidCustomPatchCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the invalid open directory.
        /// </summary>
        /// <value>The invalid open directory.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.InvalidConflicts.ContextMenu.OpenDirectory)]
        public virtual string InvalidOpenDirectory { get; protected set; }

        /// <summary>
        /// Gets or sets the invalid open directory command.
        /// </summary>
        /// <value>The invalid open directory command.</value>
        public virtual ReactiveCommand<Unit, Unit> InvalidOpenDirectoryCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the invalid open file.
        /// </summary>
        /// <value>The invalid open file.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.InvalidConflicts.ContextMenu.OpenFile)]
        public virtual string InvalidOpenFile { get; protected set; }

        /// <summary>
        /// Gets or sets the invalid open file command.
        /// </summary>
        /// <value>The invalid open file command.</value>
        public virtual ReactiveCommand<Unit, Unit> InvalidOpenFileCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is binary conflict.
        /// </summary>
        /// <value><c>true</c> if this instance is binary conflict; otherwise, <c>false</c>.</value>
        public virtual bool IsBinaryConflict { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is binary viewer visible.
        /// </summary>
        /// <value><c>true</c> if this instance is binary viewer visible; otherwise, <c>false</c>.</value>
        public virtual bool IsBinaryViewerVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is conflict solver available.
        /// </summary>
        /// <value><c>true</c> if this instance is conflict solver available; otherwise, <c>false</c>.</value>
        public virtual bool IsConflictSolverAvailable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is merge viewer visible.
        /// </summary>
        /// <value><c>true</c> if this instance is merge viewer visible; otherwise, <c>false</c>.</value>
        public virtual bool IsMergeViewerVisible { get; set; }

        /// <summary>
        /// Gets or sets the left side.
        /// </summary>
        /// <value>The left side.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.LeftSide)]
        public virtual string LeftSide { get; protected set; }

        /// <summary>
        /// Gets or sets the merge viewer.
        /// </summary>
        /// <value>The merge viewer.</value>
        public virtual MergeViewerControlViewModel MergeViewer { get; protected set; }

        /// <summary>
        /// Gets or sets the mod compare selector.
        /// </summary>
        /// <value>The mod compare selector.</value>
        public virtual ModCompareSelectorControlViewModel ModCompareSelector { get; protected set; }

        /// <summary>
        /// Gets or sets the mod filter.
        /// </summary>
        /// <value>The mod filter.</value>
        public virtual ConflictSolverModFilterControlViewModel ModFilter { get; protected set; }

        /// <summary>
        /// Gets or sets the number of conflicts caption.
        /// </summary>
        /// <value>The number of conflicts caption.</value>
        public virtual string NumberOfConflictsCaption { get; protected set; }

        /// <summary>
        /// Gets or sets the index of the previous conflict.
        /// </summary>
        /// <value>The index of the previous conflict.</value>
        public virtual int? PreviousConflictIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [read only].
        /// </summary>
        /// <value><c>true</c> if [read only]; otherwise, <c>false</c>.</value>
        public virtual bool ReadOnly { get; protected set; }

        /// <summary>
        /// Gets or sets the reset conflicts.
        /// </summary>
        /// <value>The reset conflicts.</value>
        public virtual ConflictSolverResetConflictsControlViewModel ResetConflicts { get; protected set; }

        /// <summary>
        /// Gets or sets the reset conflicts column.
        /// </summary>
        /// <value>The reset conflicts column.</value>
        public virtual int ResetConflictsColumn { get; protected set; }

        /// <summary>
        /// Gets or sets the resolve.
        /// </summary>
        /// <value>The resolve.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.Resolve)]
        public virtual string Resolve { get; protected set; }

        /// <summary>
        /// Gets or sets the resolve command.
        /// </summary>
        /// <value>The resolve command.</value>
        public virtual ReactiveCommand<Unit, Unit> ResolveCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [resolve enabled].
        /// </summary>
        /// <value><c>true</c> if [resolve enabled]; otherwise, <c>false</c>.</value>
        public virtual bool ResolveEnabled { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [resolving conflict].
        /// </summary>
        /// <value><c>true</c> if [resolving conflict]; otherwise, <c>false</c>.</value>
        public virtual bool ResolvingConflict { get; protected set; }

        /// <summary>
        /// Gets or sets the right side.
        /// </summary>
        /// <value>The right side.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.RightSide)]
        public virtual string RightSide { get; protected set; }

        /// <summary>
        /// Gets or sets the selected conflict.
        /// </summary>
        /// <value>The selected conflict.</value>
        public virtual IHierarchicalDefinitions SelectedConflict { get; set; }

        /// <summary>
        /// Gets or sets the selected conflict override.
        /// </summary>
        /// <value>The selected conflict override.</value>
        public virtual int? SelectedConflictOverride { get; protected set; }

        /// <summary>
        /// Gets or sets the selected mod collection.
        /// </summary>
        /// <value>The selected mod collection.</value>
        public virtual IModCollection SelectedModCollection { get; set; }

        /// <summary>
        /// Gets or sets the selected mods order.
        /// </summary>
        /// <value>The selected mods order.</value>
        public virtual IList<string> SelectedModsOrder { get; set; }

        /// <summary>
        /// Gets or sets the selected parent conflict.
        /// </summary>
        /// <value>The selected parent conflict.</value>
        public virtual IHierarchicalDefinitions SelectedParentConflict { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initializes the specified read only.
        /// </summary>
        /// <param name="readOnly">if set to <c>true</c> [read only].</param>
        public void Initialize(bool readOnly)
        {
            ReadOnly = readOnly;
            ResetConflictsColumn = readOnly ? 0 : 1;
            ResetConflicts.SetParameters(readOnly);
            BinaryMergeViewer.SetParameters(readOnly);
            MergeViewer.SetParameters(readOnly);
            ModCompareSelector.Reset();
            IgnoreEnabled = false;
            BackAllowed = true;
            if (Conflicts.Conflicts.HasResetDefinitions())
            {
                var sbResolved = new StringBuilder();
                var sbIgnored = new StringBuilder();
                var allHierarchalDefinitions = Conflicts.Conflicts.GetHierarchicalDefinitions();
                var modListFormat = localizationManager.GetResource(LocalizationResources.Conflict_Solver.ResetWarning.ListOfConflictsFormat);
                foreach (var conflict in allHierarchalDefinitions.Where(p => p.ResetType != ResetType.None))
                {
                    foreach (var child in conflict.Children.Where(p => p.ResetType != ResetType.None))
                    {
                        switch (child.ResetType)
                        {
                            case ResetType.Resolved:
                                sbResolved.AppendLine(IronyFormatter.Format(modListFormat, new { ParentDirectory = conflict.Name, child.Name }));
                                break;

                            case ResetType.Ignored:
                                sbIgnored.AppendLine(IronyFormatter.Format(modListFormat, new { ParentDirectory = conflict.Name, child.Name }));
                                break;

                            default:
                                break;
                        }
                    }
                }
                var msgFormat = string.Empty;
                if (readOnly)
                {
                    if (sbIgnored.Length > 0 && sbResolved.Length > 0)
                    {
                        msgFormat = localizationManager.GetResource(LocalizationResources.Conflict_Solver.ResetWarning.AnalyzeModeAll);
                    }
                    else if (sbResolved.Length > 0)
                    {
                        msgFormat = localizationManager.GetResource(LocalizationResources.Conflict_Solver.ResetWarning.AnalyzeModeResolvedOnly);
                    }
                    else
                    {
                        msgFormat = localizationManager.GetResource(LocalizationResources.Conflict_Solver.ResetWarning.AnalyzeModeIgnoredOnly);
                    }
                }
                else
                {
                    if (sbIgnored.Length > 0 && sbResolved.Length > 0)
                    {
                        msgFormat = localizationManager.GetResource(LocalizationResources.Conflict_Solver.ResetWarning.RegularModeAll);
                    }
                    else if (sbResolved.Length > 0)
                    {
                        msgFormat = localizationManager.GetResource(LocalizationResources.Conflict_Solver.ResetWarning.RegularModeResolvedOnly);
                    }
                    else
                    {
                        msgFormat = localizationManager.GetResource(LocalizationResources.Conflict_Solver.ResetWarning.RegularModeIgnoredOnly);
                    }
                }
                var msg = IronyFormatter.Format(msgFormat, new
                {
                    Environment.NewLine,
                    ListOfConflictsResolved = sbResolved.ToString(),
                    ListOfConflictsIgnored = sbIgnored.ToString()
                });
                var title = localizationManager.GetResource(LocalizationResources.Conflict_Solver.ResetWarning.Title);
                Dispatcher.UIThread.SafeInvoke(() => notificationAction.ShowPromptAsync(title, title, msg, NotificationType.Warning, PromptType.OK));
            }
        }

        /// <summary>
        /// Called when [locale changed].
        /// </summary>
        /// <param name="newLocale">The new locale.</param>
        /// <param name="oldLocale">The old locale.</param>
        public override void OnLocaleChanged(string newLocale, string oldLocale)
        {
            FilterHierarchalConflictsAsync(Conflicts).ConfigureAwait(false);
            base.OnLocaleChanged(newLocale, oldLocale);
        }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="hierarchicalDefinition">The hierarchical definition.</param>
        public virtual void SetParameters(IHierarchicalDefinitions hierarchicalDefinition)
        {
            InvalidConflictPath = string.Empty;
            if (!IsConflictSolverAvailable && hierarchicalDefinition != null)
            {
                if (hierarchicalDefinition.AdditionalData is IDefinition definition)
                {
                    ContextMenuDefinition = hierarchicalDefinition;
                    InvalidConflictPath = modPatchCollectionService.ResolveFullDefinitionPath(definition);
                }
            }
        }

        /// <summary>
        /// Evaluates the definition validity.
        /// </summary>
        /// <returns>System.Threading.Tasks.Task.</returns>
        protected virtual async Task EvaluateDefinitionValidity()
        {
            BackAllowed = false;
            var virtualDefinitions = ModCompareSelector.VirtualDefinitions;
            var patchDefinition = virtualDefinitions.FirstOrDefault(p => modPatchCollectionService.IsPatchMod(p.ModName));
            if (patchDefinition != null)
            {
                patchDefinition.UseSimpleValidation = false;
                if (virtualDefinitions.Any(p => p.UseSimpleValidation == null))
                {
                    patchDefinition.UseSimpleValidation = null;
                }
                else if (virtualDefinitions.Any(p => p.UseSimpleValidation.GetValueOrDefault()))
                {
                    patchDefinition.UseSimpleValidation = true;
                }
            }            
            var validationResult = modPatchCollectionService.Validate(patchDefinition);
            if (!validationResult.IsValid)
            {
                var title = localizationManager.GetResource(LocalizationResources.Conflict_Solver.ResolutionSaveError.Title);
                var message = localizationManager.GetResource(validationResult.ErrorLine.HasValue ? LocalizationResources.Conflict_Solver.ResolutionSaveError.MessageLine : LocalizationResources.Conflict_Solver.ResolutionSaveError.MessageNoLine);
                await Dispatcher.UIThread.SafeInvokeAsync(async () => await notificationAction.ShowPromptAsync(title, title, message.FormatIronySmart(new { Environment.NewLine, validationResult.ErrorMessage, Line = validationResult.ErrorLine, Column = validationResult.ErrorColumn }), NotificationType.Error, PromptType.OK));
            }
            else
            {
                await Dispatcher.UIThread.SafeInvokeAsync(async () => await ResolveConflictAsync(true).ConfigureAwait(true));
            }
            BackAllowed = true;
        }

        /// <summary>
        /// Evals the viewer visibility.
        /// </summary>
        protected virtual void EvalViewerVisibility()
        {
            IsBinaryViewerVisible = IsBinaryConflict && IsConflictSolverAvailable;
            IsMergeViewerVisible = !IsBinaryConflict && IsConflictSolverAvailable;
            MergeViewer.CanPerformHotKeyActions = IsMergeViewerVisible;
        }

        /// <summary>
        /// Filter hierarchal conflicts as an asynchronous operation.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="selectedDefinitionOverride">The selected definition override.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected virtual async Task FilterHierarchalConflictsAsync(IConflictResult conflictResult, IHierarchicalDefinitions selectedDefinitionOverride = null)
        {
            while (filteringConflicts)
            {
                await Task.Delay(25);
            }
            filteringConflicts = true;
            var index = PreviousConflictIndex;
            PreviousConflictIndex = null;
            if (conflictResult != null && conflictResult.Conflicts != null)
            {
                var conflicts = conflictResult.Conflicts.GetHierarchicalDefinitions().ToAvaloniaList();

                var resolved = new List<IHierarchicalDefinitions>();
                if (conflictResult.ResolvedConflicts != null)
                {
                    resolved.AddRange(conflictResult.ResolvedConflicts.GetHierarchicalDefinitions());
                }
                if (conflictResult.IgnoredConflicts != null)
                {
                    resolved.AddRange(conflictResult.IgnoredConflicts.GetHierarchicalDefinitions());
                }
                if (conflictResult.RuleIgnoredConflicts != null)
                {
                    resolved.AddRange(conflictResult.RuleIgnoredConflicts.GetHierarchicalDefinitions());
                }
                foreach (var topLevelResolvedConflicts in resolved)
                {
                    IEnumerable<IHierarchicalDefinitions> topLevelConflicts;
                    if (topLevelResolvedConflicts.Name.StartsWith(LocalizationDirectory, StringComparison.OrdinalIgnoreCase))
                    {
                        topLevelConflicts = conflicts.Where(p => p.Name.StartsWith(LocalizationDirectory, StringComparison.OrdinalIgnoreCase));
                    }
                    else
                    {
                        topLevelConflicts = conflicts.Where(p => p.Name.Equals(topLevelResolvedConflicts.Name));
                    }
                    if (topLevelConflicts.Any())
                    {
                        foreach (var topLevelConflict in topLevelConflicts)
                        {
                            foreach (var childResolvedConflict in topLevelResolvedConflicts.Children)
                            {
                                var child = topLevelConflict.Children.FirstOrDefault(p => p.Key.Equals(childResolvedConflict.Key));
                                if (child != null)
                                {
                                    topLevelConflict.Children.Remove(child);
                                }
                            }
                        }
                    }
                }
                conflicts.RemoveAll(conflicts.Where(p => p.Children == null || p.Children.Count == 0).ToList());
                if (!invalidsChecked)
                {
                    invalidsChecked = true;
                    var invalid = (await conflictResult.AllConflicts.GetByValueTypeAsync(ValueType.Invalid));
                    if (invalid?.Count() > 0)
                    {
                        var invalidDef = DIResolver.Get<IHierarchicalDefinitions>();
                        invalidDef.Name = Invalid;
                        invalidDef.Key = InvalidKey;
                        var children = new List<IHierarchicalDefinitions>();
                        foreach (var item in invalid)
                        {
                            var invalidChild = DIResolver.Get<IHierarchicalDefinitions>();
                            invalidChild.Name = item.File;
                            var message = item.ErrorColumn.HasValue || item.ErrorLine.HasValue ?
                                localizationManager.GetResource(LocalizationResources.Conflict_Solver.InvalidConflicts.Error) :
                                localizationManager.GetResource(LocalizationResources.Conflict_Solver.InvalidConflicts.ErrorNoLine);
                            invalidChild.Key = IronyFormatter.Format(message, new
                            {
                                item.ModName,
                                Line = item.ErrorLine,
                                Column = item.ErrorColumn,
                                Environment.NewLine,
                                Message = item.ErrorMessage,
                                item.File
                            });
                            invalidChild.AdditionalData = item;
                            children.Add(invalidChild);
                        }
                        invalidDef.Children = children;
                        cachedInvalids = invalidDef;
                        conflicts.Add(invalidDef);
                    }
                }
                if (cachedInvalids != null && !conflicts.Any(p => p.Key == cachedInvalids.Key))
                {
                    conflicts.Add(cachedInvalids);
                }
                var selectedParentConflict = SelectedParentConflict;
                HierarchalConflicts = conflicts;
                NumberOfConflictsCaption = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Conflict_Solver.ConflictCount), new { Count = conflicts.Where(p => p.Key != InvalidKey).SelectMany(p => p.Children).Count() });
                int? previousConflictIndex = null;
                if (HierarchalConflicts.Any() && selectedParentConflict == null)
                {
                    selectedParentConflict = HierarchalConflicts.FirstOrDefault();
                }
                if (selectedParentConflict != null)
                {
                    var conflictName = selectedParentConflict.Name;
                    selectedParentConflict = null;
                    var newSelected = HierarchalConflicts.FirstOrDefault(p => p.Name.Equals(conflictName));
                    if (newSelected != null)
                    {
                        previousConflictIndex = index;
                        if (selectedDefinitionOverride != null)
                        {
                            var overrideMatch = newSelected.Children.FirstOrDefault(p => p.Key.Equals(selectedDefinitionOverride.Key));
                            if (overrideMatch != null)
                            {
                                previousConflictIndex = newSelected.Children.ToList().IndexOf(overrideMatch);
                            }
                        }
                        if (previousConflictIndex.GetValueOrDefault() > (newSelected.Children.Count - 1))
                        {
                            previousConflictIndex = newSelected.Children.Count - 1;
                        }
                        selectedParentConflict = newSelected;
                    }
                }
                PreviousConflictIndex = previousConflictIndex;
                SelectedParentConflict = selectedParentConflict;
            }
            else
            {
                HierarchalConflicts = null;
            }
            filteringConflicts = false;
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            var resolvingEnabled = this.WhenAnyValue(v => v.ResolvingConflict, v => !v);
            var backAllowed = this.WhenAnyValue(v => v.BackAllowed, v => v == true);

            BackCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var id = idGenerator.GetNextId();
                await TriggerOverlayAsync(id, true);
                await Task.Delay(100);
                BinaryMergeViewer.Reset(true);
                BackAllowed = false;
                Conflicts?.Dispose();
                Conflicts = null;
                modPatchCollectionService.ResetPatchStateCache();
                SelectedModsOrder = null;
                SelectedModCollection = null;
                cachedInvalids = null;
                invalidsChecked = false;
                var args = new NavigationEventArgs()
                {
                    State = NavigationState.Main
                };
                ReactiveUI.MessageBus.Current.SendMessage(args);
                BackAllowed = true;
                await TriggerOverlayAsync(id, false);
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
            }, backAllowed).DisposeWith(disposables);

            ResolveCommand = ReactiveCommand.CreateFromTask(() =>
            {
                return EvaluateDefinitionValidity();
            }, resolvingEnabled).DisposeWith(disposables);

            IgnoreCommand = ReactiveCommand.Create(() =>
            {
                ResolveConflictAsync(false).ConfigureAwait(true);
            }, resolvingEnabled).DisposeWith(disposables);

            InvalidOpenDirectoryCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (!string.IsNullOrWhiteSpace(InvalidConflictPath))
                {
                    await appAction.OpenAsync(Path.GetDirectoryName(InvalidConflictPath));
                }
            }).DisposeWith(disposables);

            InvalidOpenFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (!string.IsNullOrWhiteSpace(InvalidConflictPath))
                {
                    await appAction.OpenAsync(InvalidConflictPath);
                }
            }).DisposeWith(disposables);

            this.WhenAnyValue(p => p.Conflicts).Subscribe(s =>
            {
                FilterHierarchalConflictsAsync(s).ConfigureAwait(false);
                IgnoreConflictsRules.CollectionName = SelectedModCollection.Name;
                IgnoreConflictsRules.ConflictResult = s;
                ResetConflicts.SetParameters(s, SelectedModCollection.Name);
                DatabaseSearch.SetParameters(s);
                CustomConflicts.SetParameters(s, SelectedModCollection.Name);
                MergeViewer.InitParameters();
                if (ModFilter.IsActivated)
                {
                    ModFilter.SetConflictResult(Conflicts, SelectedModsOrder.ToList(), SelectedModCollection.Name);
                }
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.SelectedParentConflict).Subscribe(s =>
            {
                IsConflictSolverAvailable = !(s?.Key == InvalidKey);
                EvalViewerVisibility();
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.SelectedConflict).Subscribe(async s =>
            {
                if (Conflicts?.Conflicts != null && !string.IsNullOrWhiteSpace(s?.Key) && IsConflictSolverAvailable)
                {
                    PreviousConflictIndex = SelectedParentConflict.Children.ToList().IndexOf(s);
                    var conflicts = (await Conflicts.Conflicts.GetByTypeAndIdAsync(s.Key)).ToObservableCollection();
                    ModCompareSelector.SelectedModsOrder = SelectedModsOrder;
                    ModCompareSelector.CollectionName = SelectedModCollection.Name;
                    ModCompareSelector.IsBinaryConflict = IsBinaryConflict = conflicts?.FirstOrDefault()?.ValueType == ValueType.Binary;
                    ModCompareSelector.Definitions = conflicts;
                    var left = ModCompareSelector.DefinitionSelection?.LeftSelectedDefinition;
                    var right = ModCompareSelector.DefinitionSelection?.RightSelectedDefinition;
                    MergeViewer.SetSidePatchMod(left, right);
                    MergeViewer.SetText(string.Empty, string.Empty, true, lockScroll: true);
                    MergeViewer.ExitEditMode();
                    EvalViewerVisibility();
                    IgnoreEnabled = true;
                }
                else
                {
                    if (HierarchalConflicts == null || !HierarchalConflicts.Any())
                    {
                        ModCompareSelector.Reset();
                        BinaryMergeViewer.Reset(false);
                        MergeViewer.SetText(string.Empty, string.Empty, true, lockScroll: true);
                    }
                    PreviousConflictIndex = null;
                    IgnoreEnabled = false;
                }
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.ModCompareSelector.IsActivated).Where(p => p).Subscribe(s =>
            {
                this.WhenAnyValue(v => v.ModCompareSelector.DefinitionSelection).Subscribe(s =>
                {
                    if (s != null && IsConflictSolverAvailable)
                    {
                        MergeViewer.EditingYaml = s.LeftSelectedDefinition.Type.StartsWith(Shared.Constants.LocalizationDirectory);
                        MergeViewer.EditingLua = s.LeftSelectedDefinition.File.EndsWith(Parser.Common.Constants.LuaExtension, StringComparison.OrdinalIgnoreCase);
                        MergeViewer.SetSidePatchMod(s.LeftSelectedDefinition, s.RightSelectedDefinition);
                        MergeViewer.SetText(s.LeftSelectedDefinition.Code, s.RightSelectedDefinition.Code, lockScroll: true);
                        MergeViewer.ExitEditMode();
                        if (!IsBinaryConflict)
                        {
                            BinaryMergeViewer.EnableSelection = ResolveEnabled = s.LeftSelectedDefinition != null &&
                                s.RightSelectedDefinition != null &&
                                s.LeftSelectedDefinition != s.RightSelectedDefinition &&
                                (modPatchCollectionService.IsPatchMod(s.LeftSelectedDefinition.ModName) || modPatchCollectionService.IsPatchMod(s.RightSelectedDefinition.ModName));
                        }
                        else
                        {
                            BinaryMergeViewer.Reset(false);
                            ResolveEnabled = false;
                            BinaryMergeViewer.EnableSelection = s.LeftSelectedDefinition != null &&
                                s.RightSelectedDefinition != null &&
                                s.LeftSelectedDefinition != s.RightSelectedDefinition;
                            BinaryMergeViewer.SetLeft(s.LeftSelectedDefinition);
                            BinaryMergeViewer.SetRight(s.RightSelectedDefinition);
                        }
                    }
                    else
                    {
                        BinaryMergeViewer.Reset(false);
                        ResolveEnabled = false;
                    }
                }).DisposeWith(disposables);
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.BinaryMergeViewer.IsActivated).Where(p => p).Subscribe(s =>
            {
                Observable.Merge(BinaryMergeViewer.TakeLeftCommand.Select(s => true), BinaryMergeViewer.TakeRightCommand.Select(s => false)).Subscribe(s =>
                {
                    takeLeftBinary = s;
                    ResolveEnabled = true;
                }).DisposeWith(disposables);
            }).DisposeWith(disposables);

            this.WhenAnyValue(p => p.MergeViewer.LeftSide).Where(p => !string.IsNullOrWhiteSpace(p)).Subscribe(s =>
            {
                var virtualDefinitions = ModCompareSelector.VirtualDefinitions;
                if (MergeViewer.LeftSidePatchMod && virtualDefinitions != null)
                {
                    var patchDefinition = virtualDefinitions.FirstOrDefault(p => modPatchCollectionService.IsPatchMod(p.ModName));
                    SyncCode(patchDefinition);
                }
            }).DisposeWith(disposables);

            this.WhenAnyValue(p => p.MergeViewer.RightSide).Where(p => !string.IsNullOrWhiteSpace(p)).Subscribe(s =>
            {
                var virtualDefinitions = ModCompareSelector.VirtualDefinitions;
                if (MergeViewer.RightSidePatchMod && virtualDefinitions != null)
                {
                    var patchDefinition = virtualDefinitions.FirstOrDefault(p => modPatchCollectionService.IsPatchMod(p.ModName));
                    SyncCode(patchDefinition);
                }
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.IgnoreConflictsRules.IsActivated).Where(p => p).Subscribe(s =>
            {
                Observable.Merge(IgnoreConflictsRules.SaveCommand, IgnoreConflictsRules.CancelCommand).Subscribe(result =>
                {
                    switch (result.State)
                    {
                        case Implementation.CommandState.Success:
                            EditingIgnoreConflictsRules = false;
                            FilterHierarchalConflictsAsync(Conflicts, SelectedConflict).ConfigureAwait(false);
                            break;

                        case Implementation.CommandState.NotExecuted:
                            EditingIgnoreConflictsRules = false;
                            break;

                        default:
                            break;
                    }
                }).DisposeWith(disposables);
            }).DisposeWith(disposables);

            IgnoreRulesCommand = ReactiveCommand.Create(() =>
            {
                EditingIgnoreConflictsRules = true;
            }).DisposeWith(disposables);

            InvalidCustomPatchCommand = ReactiveCommand.Create(() =>
            {
                if (ContextMenuDefinition.AdditionalData is IDefinition definition)
                {
                    CustomConflicts.SetContent(definition.File, definition.OriginalCode);
                }
            }).DisposeWith(disposables);

            this.WhenAnyValue(p => p.ModFilter.IsActivated).Where(p => p).Subscribe(s =>
            {
                ModFilter.SetConflictResult(Conflicts, SelectedModsOrder.ToList(), SelectedModCollection.Name);
                this.WhenAnyValue(p => p.ModFilter.HasSavedState).Where(p => p).Subscribe(s =>
                {
                    FilterHierarchalConflictsAsync(Conflicts, SelectedConflict).ConfigureAwait(false);
                }).DisposeWith(disposables);
            }).DisposeWith(disposables);

            this.WhenAnyValue(p => p.ResetConflicts.IsActivated).Where(p => p).Subscribe(s =>
            {
                ResetConflicts.ResetCommand.Subscribe(s =>
                {
                    if (s.State == Implementation.CommandState.Success)
                    {
                        FilterHierarchalConflictsAsync(Conflicts, SelectedConflict).ConfigureAwait(false);
                    }
                }).DisposeWith(disposables);
            }).DisposeWith(disposables);

            this.WhenAnyValue(p => p.CustomConflicts.Saved).Where(p => p).Subscribe(s =>
            {
                ResetConflicts.Refresh();
            }).DisposeWith(disposables);

            var previousEditTextState = false;
            this.WhenAnyValue(v => v.EditingIgnoreConflictsRules).Subscribe(s =>
            {
                if (s != previousEditTextState)
                {
                    previousEditTextState = s;
                }
            }).DisposeWith(disposables);

            hotkeyPressedHandler.Subscribe(m =>
            {
                async Task performModSelectionAction()
                {
                    if (!filteringConflicts && (SelectedParentConflict?.Children.Any()).GetValueOrDefault())
                    {
                        int? newSelectedConflict = null;
                        var col = SelectedParentConflict != null ? SelectedParentConflict.Children.ToList() : new List<IHierarchicalDefinitions>();
                        switch (m.Hotkey)
                        {
                            case Enums.HotKeys.Shift_Up:
                                if (SelectedConflict == null)
                                {
                                    newSelectedConflict = col.Count - 1;
                                }
                                else
                                {
                                    var index = col.IndexOf(SelectedConflict);
                                    index--;
                                    if (index < 0)
                                    {
                                        index = 0;
                                    }
                                    newSelectedConflict = index;
                                }
                                break;

                            case Enums.HotKeys.Shift_Down:
                                if (SelectedConflict == null)
                                {
                                    newSelectedConflict = 0;
                                }
                                else
                                {
                                    var index = col.IndexOf(SelectedConflict);
                                    index++;
                                    if (index > col.Count - 1)
                                    {
                                        index = col.Count - 1;
                                    }
                                    newSelectedConflict = index;
                                }
                                break;

                            default:
                                break;
                        }
                        if (newSelectedConflict != null)
                        {
                            await Dispatcher.UIThread.SafeInvokeAsync(() =>
                            {
                                SelectedConflictOverride = newSelectedConflict;
                            });
                        }
                    }
                }
                async Task performModParentSelectionAction()
                {
                    if (!filteringConflicts && (HierarchalConflicts?.Any()).GetValueOrDefault())
                    {
                        IHierarchicalDefinitions parent = null;
                        var col = HierarchalConflicts.ToList();
                        switch (m.Hotkey)
                        {
                            case Enums.HotKeys.Ctrl_Shift_P:
                                if (SelectedParentConflict == null)
                                {
                                    parent = col.LastOrDefault();
                                }
                                else
                                {
                                    var index = col.IndexOf(SelectedParentConflict);
                                    index--;
                                    if (index < 0)
                                    {
                                        index = 0;
                                    }
                                    parent = col[index];
                                }
                                break;

                            case Enums.HotKeys.Ctrl_Shift_N:
                                if (SelectedParentConflict == null)
                                {
                                    parent = col.FirstOrDefault();
                                }
                                else
                                {
                                    var index = col.IndexOf(SelectedParentConflict);
                                    index++;
                                    if (index > col.Count - 1)
                                    {
                                        index = col.Count - 1;
                                    }
                                    parent = col[index];
                                }
                                break;

                            default:
                                break;
                        }
                        if (parent != null)
                        {
                            await Dispatcher.UIThread.SafeInvokeAsync(() =>
                            {
                                SelectedParentConflict = parent;
                            });
                        }
                    }
                }
                if (m.Hotkey == Enums.HotKeys.Shift_Down || m.Hotkey == Enums.HotKeys.Shift_Up)
                {
                    performModSelectionAction().ConfigureAwait(false);
                }
                else if (m.Hotkey == Enums.HotKeys.Ctrl_R)
                {
                    if (ResolveEnabled && !ResolvingConflict)
                    {
                        EvaluateDefinitionValidity().ConfigureAwait(false);
                    }
                }
                else if (m.Hotkey == Enums.HotKeys.Ctrl_I)
                {
                    if (IgnoreEnabled && !ResolvingConflict)
                    {
                        Dispatcher.UIThread.SafeInvoke(() => ResolveConflictAsync(false).ConfigureAwait(true));
                    }
                }
                else
                {
                    performModParentSelectionAction().ConfigureAwait(false);
                }
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// resolve conflict as an asynchronous operation.
        /// </summary>
        /// <param name="resolve">if set to <c>true</c> [resolve].</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected virtual async Task ResolveConflictAsync(bool resolve)
        {
            BackAllowed = false;
            if (ResolvingConflict)
            {
                return;
            }
            if (await externalProcessHandlerService.IsParadoxLauncherRunningAsync())
            {
                var title = localizationManager.GetResource(LocalizationResources.Notifications.ParadoxLauncherRunning.Title);
                var message = localizationManager.GetResource(LocalizationResources.Notifications.ParadoxLauncherRunning.Message);
                notificationAction.ShowNotification(title, message, NotificationType.Error, 30);
                return;
            }
            ResolvingConflict = true;
            if (ModCompareSelector.VirtualDefinitions != null && ModCompareSelector.VirtualDefinitions.Any())
            {
                IHierarchicalDefinitions conflictParent = null;
                int? conflictParentIdx = null;
                int parentIdx = HierarchalConflicts.ToList().IndexOf(SelectedParentConflict);
                foreach (var item in HierarchalConflicts)
                {
                    if (item.Children.Contains(SelectedConflict))
                    {
                        conflictParent = item;
                        var conflictParentChildren = new List<IHierarchicalDefinitions>(item.Children);
                        conflictParentIdx = conflictParentChildren.IndexOf(SelectedConflict);
                        break;
                    }
                }
                var id = idGenerator.GetNextId();
                await TriggerOverlayAsync(id, true, localizationManager.GetResource(LocalizationResources.Conflict_Solver.OverlayResolve));
                IDefinition patchDefinition = null;
                if (!IsBinaryConflict)
                {
                    patchDefinition = ModCompareSelector.VirtualDefinitions.FirstOrDefault(p => modPatchCollectionService.IsPatchMod(p.ModName));
                }
                else
                {
                    if (resolve)
                    {
                        patchDefinition = takeLeftBinary ? ModCompareSelector.DefinitionSelection.LeftSelectedDefinition : ModCompareSelector.DefinitionSelection.RightSelectedDefinition;
                    }
                    else
                    {
                        patchDefinition = ModCompareSelector.Definitions.FirstOrDefault();
                    }
                }
                if (patchDefinition != null)
                {
                    var generatedFileNames = patchDefinition.GeneratedFileNames;
                    foreach (var fileNames in ModCompareSelector.Definitions.Select(p => p.GeneratedFileNames))
                    {
                        foreach (var item in fileNames)
                        {
                            generatedFileNames.Add(item);
                        }
                    }
                    patchDefinition.GeneratedFileNames = generatedFileNames;
                    SyncCode(patchDefinition);
                    try
                    {
                        if (await Task.Run(async () => resolve ?
                            await modPatchCollectionService.ApplyModPatchAsync(Conflicts, patchDefinition, SelectedModCollection.Name) :
                            await modPatchCollectionService.IgnoreModPatchAsync(Conflicts, patchDefinition, SelectedModCollection.Name)))
                        {
                            await FilterHierarchalConflictsAsync(Conflicts);
                            IHierarchicalDefinitions selectedConflict = null;
                            if (conflictParentIdx.HasValue && HierarchalConflicts.Any())
                            {
                                foreach (var item in HierarchalConflicts)
                                {
                                    if (item.Name.Equals(conflictParent.Name))
                                    {
                                        if (conflictParentIdx.Value > (item.Children.Count - 1))
                                        {
                                            conflictParentIdx = item.Children.Count - 1;
                                        }
                                        else if (conflictParentIdx.Value < 0)
                                        {
                                            conflictParentIdx = 0;
                                        }
                                        selectedConflict = item.Children.Select(p => p).ToList()[conflictParentIdx.GetValueOrDefault()];
                                        break;
                                    }
                                }
                            }
                            SelectedConflict = selectedConflict;
                            ResetConflicts.Refresh();
                        }
                    }
                    catch (Exception ex)
                    {
                        var title = localizationManager.GetResource(LocalizationResources.SavingError.Title);
                        var message = localizationManager.GetResource(LocalizationResources.SavingError.Message);
                        logger.Error(ex);
                        notificationAction.ShowNotification(title, message, NotificationType.Error, 30);
                    }
                }
                if (SelectedConflict == null)
                {
                    if (parentIdx > (HierarchalConflicts.Count - 1))
                    {
                        parentIdx = HierarchalConflicts.Count - 1;
                    }
                    else if (parentIdx < 0)
                    {
                        parentIdx = 0;
                    }
                    if (HierarchalConflicts.Any())
                    {
                        // Force a refresh of the UI
                        SelectedParentConflict = null;
                        SelectedParentConflict = HierarchalConflicts.ElementAt(parentIdx);
                    }
                }
                await TriggerOverlayAsync(id, false);
            }
            ResolvingConflict = false;
            BackAllowed = true;
        }

        /// <summary>
        /// Synchronizes the code.
        /// </summary>
        /// <param name="definition">The definition.</param>
        protected virtual void SyncCode(IDefinition definition)
        {
            if (definition != null)
            {
                if (MergeViewer.LeftSidePatchMod)
                {
                    definition.Code = MergeViewer.LeftSide;
                }
                else
                {
                    definition.Code = MergeViewer.RightSide;
                }
            }
        }

        #endregion Methods
    }
}
