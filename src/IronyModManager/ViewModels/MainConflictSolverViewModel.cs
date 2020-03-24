// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-18-2020
//
// Last Modified By : Mario
// Last Modified On : 03-24-2020
// ***********************************************************************
// <copyright file="MainConflictSolverViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using IronyModManager.Common;
using IronyModManager.Common.Events;
using IronyModManager.Common.ViewModels;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Definitions;
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
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainConflictSolverControlViewModel" /> class.
        /// </summary>
        /// <param name="mergeViewer">The merge viewer.</param>
        /// <param name="modCompareSelector">The mod compare selector.</param>
        public MainConflictSolverControlViewModel(MergeViewerControlViewModel mergeViewer, ModCompareSelectorControlViewModel modCompareSelector)
        {
            MergeViewer = mergeViewer;
            ModCompareSelector = modCompareSelector;
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

        #endregion Properties

        #region Methods

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
                    Results = Conflicts,
                    State = NavigationState.Main
                };
                MessageBus.Current.SendMessage(args);
            }).DisposeWith(disposables);

            this.WhenAnyValue(p => p.Conflicts).Subscribe(s =>
            {
                if (s != null && s.Conflicts != null)
                {
                    HierarchalConflicts = s.Conflicts.GetHierarchicalDefinitions().ToObservableCollection();
                }
                else
                {
                    HierarchalConflicts = null;
                }
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.SelectedConflict).Subscribe(s =>
            {
                if (Conflicts?.Conflicts != null && !string.IsNullOrWhiteSpace(s?.Key))
                {
                    ModCompareSelector.Definitions = Conflicts.Conflicts.GetByTypeAndId(s.Key).ToObservableCollection();
                    MergeViewer.SetText(string.Empty, string.Empty);
                }
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.ModCompareSelector.IsActivated).Where(p => p).Subscribe(s =>
            {
                this.WhenAnyValue(v => v.ModCompareSelector.LeftSelectedDefinition).Where(p => p != null).Subscribe(s =>
                {
                    MergeViewer.SetText(s.Code, MergeViewer.RightSide);
                }).DisposeWith(disposables);

                this.WhenAnyValue(v => v.ModCompareSelector.RightSelectedDefinition).Where(p => p != null).Subscribe(s =>
                {
                    MergeViewer.SetText(MergeViewer.LeftSide, s.Code);
                }).DisposeWith(disposables);
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
