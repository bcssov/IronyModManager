// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-18-2020
//
// Last Modified By : Mario
// Last Modified On : 05-11-2020
// ***********************************************************************
// <copyright file="MainConflictSolverViewModel.cs" company="Mario">
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
using IronyModManager.Localization;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;
using ReactiveUI;
using SmartFormat;

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
        /// The localization
        /// </summary>
        private const string Localization = "localisation";

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
        /// The take left binary
        /// </summary>
        private bool takeLeftBinary = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainConflictSolverControlViewModel" /> class.
        /// </summary>
        /// <param name="modService">The mod service.</param>
        /// <param name="localizationManager">The localization manager.</param>
        /// <param name="mergeViewer">The merge viewer.</param>
        /// <param name="binaryMergeViewer">The binary merge viewer.</param>
        /// <param name="modCompareSelector">The mod compare selector.</param>
        /// <param name="ignoreConflictsRules">The ignore conflicts rules.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="notificationAction">The notification action.</param>
        public MainConflictSolverControlViewModel(IModService modService, ILocalizationManager localizationManager,
            MergeViewerControlViewModel mergeViewer, MergeViewerBinaryControlViewModel binaryMergeViewer,
            ModCompareSelectorControlViewModel modCompareSelector, ModConflictIgnoreControlViewModel ignoreConflictsRules,
            ILogger logger, INotificationAction notificationAction)
        {
            this.modService = modService;
            this.localizationManager = localizationManager;
            this.logger = logger;
            this.notificationAction = notificationAction;
            MergeViewer = mergeViewer;
            ModCompareSelector = modCompareSelector;
            BinaryMergeViewer = binaryMergeViewer;
            IgnoreConflictsRules = ignoreConflictsRules;
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
        /// Gets or sets the back command.
        /// </summary>
        /// <value>The back command.</value>
        public virtual ReactiveCommand<Unit, Unit> BackCommand { get; set; }

        /// <summary>
        /// Gets or sets the binary merge viewer.
        /// </summary>
        /// <value>The binary merge viewer.</value>
        public MergeViewerBinaryControlViewModel BinaryMergeViewer { get; protected set; }

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
        /// Gets or sets a value indicating whether [editing ignore conflicts rules].
        /// </summary>
        /// <value><c>true</c> if [editing ignore conflicts rules]; otherwise, <c>false</c>.</value>
        public virtual bool EditingIgnoreConflictsRules { get; protected set; }

        /// <summary>
        /// Gets or sets the hierarchal conflicts.
        /// </summary>
        /// <value>The hierarchal conflicts.</value>
        public virtual IEnumerable<IHierarchicalDefinitions> HierarchalConflicts { get; protected set; }

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
        /// Gets or sets the index of the previous conflict.
        /// </summary>
        /// <value>The index of the previous conflict.</value>
        public virtual int? PreviousConflictIndex { get; set; }

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
        /// Called when [locale changed].
        /// </summary>
        /// <param name="newLocale">The new locale.</param>
        /// <param name="oldLocale">The old locale.</param>
        public override void OnLocaleChanged(string newLocale, string oldLocale)
        {
            FilterHierarchalConflicts(Conflicts);
            base.OnLocaleChanged(newLocale, oldLocale);
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            ModCompareSelector.Reset();
            IgnoreEnabled = false;
        }

        /// <summary>
        /// Evals the viewer visibility.
        /// </summary>
        protected virtual void EvalViewerVisibility()
        {
            IsBinaryViewerVisible = IsBinaryConflict && IsConflictSolverAvailable;
            IsMergeViewerVisible = !IsBinaryConflict && IsConflictSolverAvailable;
        }

        /// <summary>
        /// Filters the hierarchal conflicts.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        protected virtual void FilterHierarchalConflicts(IConflictResult conflictResult)
        {
            var index = PreviousConflictIndex;
            PreviousConflictIndex = null;
            if (conflictResult != null && conflictResult.Conflicts != null)
            {
                var conflicts = conflictResult.Conflicts.GetHierarchicalDefinitions().ToList();

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
                    if (topLevelResolvedConflicts.Name.StartsWith(Localization, StringComparison.OrdinalIgnoreCase))
                    {
                        topLevelConflicts = conflicts.Where(p => p.Name.StartsWith(Localization, StringComparison.OrdinalIgnoreCase));
                    }
                    else
                    {
                        topLevelConflicts = conflicts.Where(p => p.Name.Equals(topLevelResolvedConflicts.Name));
                    }
                    if (topLevelConflicts.Count() > 0)
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
                    conflicts.RemoveAll(p => p.Children == null || p.Children.Count == 0);
                }
                var invalid = conflictResult.AllConflicts.GetByValueType(Parser.Common.ValueType.Invalid);
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
                        invalidChild.Key = Smart.Format(message, new
                        {
                            item.ModName,
                            Line = item.ErrorLine,
                            Column = item.ErrorColumn,
                            Environment.NewLine,
                            Message = item.ErrorMessage,
                            item.File
                        });
                        children.Add(invalidChild);
                    }
                    invalidDef.Children = children;
                    conflicts.Add(invalidDef);
                }
                HierarchalConflicts = conflicts.ToObservableCollection();
                if (SelectedParentConflict != null)
                {
                    var conflictName = SelectedParentConflict.Name;
                    SelectedParentConflict = null;
                    var newSelected = HierarchalConflicts.FirstOrDefault(p => p.Name.Equals(conflictName));
                    if (newSelected != null)
                    {
                        PreviousConflictIndex = index;
                        if (PreviousConflictIndex.GetValueOrDefault() > (newSelected.Children.Count - 1))
                        {
                            PreviousConflictIndex = newSelected.Children.Count - 1;
                        }
                        SelectedParentConflict = newSelected;
                    }
                }
            }
            else
            {
                HierarchalConflicts = null;
            }
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            var resolvingEnabled = this.WhenAnyValue(v => v.ResolvingConflict, v => !v);

            modService.ShutdownState += (args) =>
            {
                TriggerPreventShutdown(args);
            };

            BackCommand = ReactiveCommand.Create(() =>
            {
                Conflicts?.AllConflicts.Dispose();
                Conflicts?.Conflicts.Dispose();
                Conflicts?.IgnoredConflicts.Dispose();
                Conflicts?.OrphanConflicts.Dispose();
                Conflicts?.ResolvedConflicts.Dispose();
                Conflicts?.RuleIgnoredConflicts.Dispose();
                Conflicts = null;
                SelectedModsOrder = null;
                SelectedModCollection = null;
                var args = new NavigationEventArgs()
                {
                    State = NavigationState.Main
                };
                MessageBus.Current.SendMessage(args);
            }).DisposeWith(disposables);

            ResolveCommand = ReactiveCommand.Create(() =>
            {
                ResolveConflictAsync(true).ConfigureAwait(true);
            }, resolvingEnabled).DisposeWith(disposables);

            IgnoreCommand = ReactiveCommand.Create(() =>
            {
                ResolveConflictAsync(false).ConfigureAwait(true);
            }, resolvingEnabled).DisposeWith(disposables);

            this.WhenAnyValue(p => p.Conflicts).Subscribe(s =>
            {
                FilterHierarchalConflicts(s);
                IgnoreConflictsRules.CollectionName = SelectedModCollection.Name;
                IgnoreConflictsRules.ConflictResult = s;
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.SelectedParentConflict).Subscribe(s =>
            {
                IsConflictSolverAvailable = !(s?.Key == InvalidKey);
                EvalViewerVisibility();
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.SelectedConflict).Subscribe(s =>
            {
                if (Conflicts?.Conflicts != null && !string.IsNullOrWhiteSpace(s?.Key) && IsConflictSolverAvailable)
                {
                    PreviousConflictIndex = SelectedParentConflict.Children.ToList().IndexOf(s);
                    var conflicts = Conflicts.Conflicts.GetByTypeAndId(s.Key).ToObservableCollection();
                    ModCompareSelector.SelectedModsOrder = SelectedModsOrder;
                    ModCompareSelector.CollectionName = SelectedModCollection.Name;
                    ModCompareSelector.IsBinaryConflict = IsBinaryConflict = conflicts?.FirstOrDefault()?.ValueType == Parser.Common.ValueType.Binary;
                    ModCompareSelector.Definitions = conflicts;
                    MergeViewer.SetSidePatchMod(modService.IsPatchMod(ModCompareSelector.LeftSelectedDefinition?.ModName), modService.IsPatchMod(ModCompareSelector.RightSelectedDefinition?.ModName));
                    MergeViewer.SetText(string.Empty, string.Empty);
                    MergeViewer.ExitEditMode();
                    EvalViewerVisibility();
                    IgnoreEnabled = true;
                }
                else
                {
                    PreviousConflictIndex = null;
                    IgnoreEnabled = false;
                }
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.ModCompareSelector.IsActivated).Where(p => p).Subscribe(s =>
            {
                this.WhenAnyValue(v => v.ModCompareSelector.LeftSelectedDefinition).Subscribe(s =>
                {
                    if (s != null && IsConflictSolverAvailable)
                    {
                        MergeViewer.EditingYaml = s.Type.StartsWith(Localization);
                        MergeViewer.SetSidePatchMod(modService.IsPatchMod(ModCompareSelector.LeftSelectedDefinition?.ModName), modService.IsPatchMod(ModCompareSelector.RightSelectedDefinition?.ModName));
                        MergeViewer.SetText(s.Code, MergeViewer.RightSide);
                        MergeViewer.ExitEditMode();
                        if (!IsBinaryConflict)
                        {
                            BinaryMergeViewer.EnableSelection = ResolveEnabled = ModCompareSelector.LeftSelectedDefinition != null &&
                                ModCompareSelector.RightSelectedDefinition != null &&
                                ModCompareSelector.LeftSelectedDefinition != ModCompareSelector.RightSelectedDefinition &&
                                (modService.IsPatchMod(ModCompareSelector.LeftSelectedDefinition.ModName) || modService.IsPatchMod(ModCompareSelector.RightSelectedDefinition.ModName));
                        }
                        else
                        {
                            BinaryMergeViewer.Reset();
                            ResolveEnabled = false;
                            BinaryMergeViewer.EnableSelection = ModCompareSelector.LeftSelectedDefinition != null &&
                                ModCompareSelector.RightSelectedDefinition != null &&
                                ModCompareSelector.LeftSelectedDefinition != ModCompareSelector.RightSelectedDefinition;
                        }
                    }
                    else
                    {
                        BinaryMergeViewer.Reset();
                        ResolveEnabled = false;
                    }
                }).DisposeWith(disposables);

                this.WhenAnyValue(v => v.ModCompareSelector.RightSelectedDefinition).Subscribe(s =>
                {
                    if (s != null && IsConflictSolverAvailable)
                    {
                        MergeViewer.EditingYaml = s.Type.StartsWith(Localization);
                        MergeViewer.SetSidePatchMod(modService.IsPatchMod(ModCompareSelector.LeftSelectedDefinition?.ModName), modService.IsPatchMod(ModCompareSelector.RightSelectedDefinition?.ModName));
                        MergeViewer.SetText(MergeViewer.LeftSide, s.Code);
                        MergeViewer.ExitEditMode();
                        if (!IsBinaryConflict)
                        {
                            BinaryMergeViewer.EnableSelection = ResolveEnabled = ModCompareSelector.LeftSelectedDefinition != null &&
                                ModCompareSelector.RightSelectedDefinition != null &&
                                ModCompareSelector.LeftSelectedDefinition != ModCompareSelector.RightSelectedDefinition &&
                                (modService.IsPatchMod(ModCompareSelector.LeftSelectedDefinition.ModName) || modService.IsPatchMod(ModCompareSelector.RightSelectedDefinition.ModName));
                        }
                        else
                        {
                            BinaryMergeViewer.Reset();
                            ResolveEnabled = false;
                            BinaryMergeViewer.EnableSelection = ModCompareSelector.LeftSelectedDefinition != null &&
                                ModCompareSelector.RightSelectedDefinition != null &&
                                ModCompareSelector.LeftSelectedDefinition != ModCompareSelector.RightSelectedDefinition;
                        }
                    }
                    else
                    {
                        BinaryMergeViewer.Reset();
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
                if (MergeViewer.LeftSidePatchMod)
                {
                    var patchDefinition = ModCompareSelector.VirtualDefinitions.FirstOrDefault(p => modService.IsPatchMod(p.ModName));
                    SyncCode(patchDefinition);
                }
            }).DisposeWith(disposables);

            this.WhenAnyValue(p => p.MergeViewer.RightSide).Where(p => !string.IsNullOrWhiteSpace(p)).Subscribe(s =>
            {
                if (MergeViewer.RightSidePatchMod)
                {
                    var patchDefinition = ModCompareSelector.VirtualDefinitions.FirstOrDefault(p => modService.IsPatchMod(p.ModName));
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
                            FilterHierarchalConflicts(Conflicts);
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

            base.OnActivated(disposables);
        }

        /// <summary>
        /// resolve conflict as an asynchronous operation.
        /// </summary>
        /// <param name="resolve">if set to <c>true</c> [resolve].</param>
        protected virtual async Task ResolveConflictAsync(bool resolve)
        {
            if (ResolvingConflict)
            {
                return;
            }
            ResolvingConflict = true;
            if (ModCompareSelector.VirtualDefinitions?.Count() > 0)
            {
                IHierarchicalDefinitions conflictParent = null;
                int? conflictParentIdx = null;
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
                await TriggerOverlayAsync(true, localizationManager.GetResource(LocalizationResources.Conflict_Solver.OverlayResolve));
                IDefinition patchDefinition = null;
                if (!IsBinaryConflict)
                {
                    patchDefinition = ModCompareSelector.VirtualDefinitions.FirstOrDefault(p => modService.IsPatchMod(p.ModName));
                }
                else
                {
                    patchDefinition = takeLeftBinary ? ModCompareSelector.LeftSelectedDefinition : ModCompareSelector.RightSelectedDefinition;
                }
                if (patchDefinition != null)
                {
                    foreach (var fileNames in ModCompareSelector.Definitions.Select(p => p.FileNames))
                    {
                        foreach (var item in fileNames)
                        {
                            patchDefinition.FileNames.Add(item);
                        }
                    }
                    SyncCode(patchDefinition);
                    try
                    {
                        if (resolve ?
                            await modService.ApplyModPatchAsync(Conflicts, patchDefinition, SelectedModCollection.Name) :
                            await modService.IgnoreModPatchAsync(Conflicts, patchDefinition, SelectedModCollection.Name))
                        {
                            FilterHierarchalConflicts(Conflicts);
                            IHierarchicalDefinitions selectedConflict = null;
                            if (conflictParentIdx.HasValue && HierarchalConflicts.Count() > 0)
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
                    ModCompareSelector.Reset();
                    MergeViewer.Reset();
                    BinaryMergeViewer.Reset();
                }
                await TriggerOverlayAsync(false);
            }
            ResolvingConflict = false;
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
