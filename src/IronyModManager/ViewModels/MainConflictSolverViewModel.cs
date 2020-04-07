// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-18-2020
//
// Last Modified By : Mario
// Last Modified On : 04-07-2020
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
using IronyModManager.Localization;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;
using ReactiveUI;

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
        /// The localization manager
        /// </summary>
        private readonly ILocalizationManager localizationManager;

        /// <summary>
        /// The mod service
        /// </summary>
        private readonly IModService modService;

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
        public MainConflictSolverControlViewModel(IModService modService, ILocalizationManager localizationManager,
            MergeViewerControlViewModel mergeViewer, MergeViewerBinaryControlViewModel binaryMergeViewer,
            ModCompareSelectorControlViewModel modCompareSelector)
        {
            this.modService = modService;
            this.localizationManager = localizationManager;
            MergeViewer = mergeViewer;
            ModCompareSelector = modCompareSelector;
            BinaryMergeViewer = binaryMergeViewer;
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
        /// Gets or sets the hierarchal conflicts.
        /// </summary>
        /// <value>The hierarchal conflicts.</value>
        public virtual IEnumerable<IHierarchicalDefinitions> HierarchalConflicts { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is binary conflict.
        /// </summary>
        /// <value><c>true</c> if this instance is binary conflict; otherwise, <c>false</c>.</value>
        public virtual bool IsBinaryConflict { get; protected set; }

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

        #endregion Properties

        #region Methods

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            ModCompareSelector.Reset();
        }

        /// <summary>
        /// Filters the hierarchal conflicts.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        protected virtual void FilterHierarchalConflicts(IConflictResult conflictResult)
        {
            if (conflictResult != null && conflictResult.Conflicts != null)
            {
                var conflicts = conflictResult.Conflicts.GetHierarchicalDefinitions().ToHashSet();
                if (conflictResult.ResolvedConflicts != null)
                {
                    foreach (var topLevelResolvedConflicts in conflictResult.ResolvedConflicts.GetHierarchicalDefinitions())
                    {
                        var topLevelConflict = conflicts.FirstOrDefault(p => p.Name.Equals(topLevelResolvedConflicts.Name));
                        if (topLevelResolvedConflicts != null && topLevelConflict != null)
                        {
                            foreach (var childResolvedConflict in topLevelResolvedConflicts.Children)
                            {
                                var child = topLevelConflict.Children.FirstOrDefault(p => p.Name.Equals(childResolvedConflict.Name));
                                if (child != null)
                                {
                                    topLevelConflict.Children.Remove(child);
                                }
                            }
                        }
                    }
                    conflicts.RemoveWhere(p => p.Children == null || p.Children.Count == 0);
                }
                HierarchalConflicts = conflicts.ToObservableCollection();
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
            BackCommand = ReactiveCommand.Create(() =>
            {
                var args = new NavigationEventArgs()
                {
                    SelectedCollection = SelectedModCollection,
                    Results = Conflicts,
                    State = NavigationState.Main
                };
                MessageBus.Current.SendMessage(args);
            }).DisposeWith(disposables);

            ResolveCommand = ReactiveCommand.Create(() =>
            {
                ResolveConflictAsync().ConfigureAwait(true);
            }).DisposeWith(disposables);

            this.WhenAnyValue(p => p.Conflicts).Subscribe(s =>
            {
                FilterHierarchalConflicts(s);
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.SelectedConflict).Subscribe(s =>
            {
                if (Conflicts?.Conflicts != null && !string.IsNullOrWhiteSpace(s?.Key))
                {
                    var conflicts = Conflicts.Conflicts.GetByTypeAndId(s.Key).ToObservableCollection();
                    ModCompareSelector.CollectionName = SelectedModCollection.Name;
                    ModCompareSelector.IsBinaryConflict = IsBinaryConflict = conflicts?.FirstOrDefault()?.ValueType == Parser.Common.ValueType.Binary;
                    ModCompareSelector.Definitions = conflicts;
                    MergeViewer.SetText(string.Empty, string.Empty);
                    MergeViewer.SetSidePatchMod(modService.IsPatchMod(ModCompareSelector.LeftSelectedDefinition?.ModName), modService.IsPatchMod(ModCompareSelector.RightSelectedDefinition?.ModName));
                    MergeViewer.ExitEditMode();
                }
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.ModCompareSelector.IsActivated).Where(p => p).Subscribe(s =>
            {
                this.WhenAnyValue(v => v.ModCompareSelector.LeftSelectedDefinition).Subscribe(s =>
                {
                    if (s != null)
                    {
                        MergeViewer.SetText(s.Code, MergeViewer.RightSide);
                        MergeViewer.SetSidePatchMod(modService.IsPatchMod(ModCompareSelector.LeftSelectedDefinition?.ModName), modService.IsPatchMod(ModCompareSelector.RightSelectedDefinition?.ModName));
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
                    if (s != null)
                    {
                        MergeViewer.SetText(MergeViewer.LeftSide, s.Code);
                        MergeViewer.SetSidePatchMod(modService.IsPatchMod(ModCompareSelector.LeftSelectedDefinition?.ModName), modService.IsPatchMod(ModCompareSelector.RightSelectedDefinition?.ModName));
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

            base.OnActivated(disposables);
        }

        /// <summary>
        /// add new definition as an asynchronous operation.
        /// </summary>
        protected virtual async Task ResolveConflictAsync()
        {
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
                var selectedConflictParent = SelectedConflict;
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
                    if (MergeViewer.LeftSidePatchMod)
                    {
                        patchDefinition.Code = MergeViewer.LeftSide;
                    }
                    else
                    {
                        patchDefinition.Code = MergeViewer.RightSide;
                    }
                    if (await modService.ApplyModPatchAsync(Conflicts, patchDefinition, SelectedModCollection.Name))
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
                if (SelectedConflict == null)
                {
                    ModCompareSelector.Reset();
                }
                await TriggerOverlayAsync(false);
            }
        }

        #endregion Methods
    }
}
