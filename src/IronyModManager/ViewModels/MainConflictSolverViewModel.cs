// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-18-2020
//
// Last Modified By : Mario
// Last Modified On : 03-23-2020
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
        public MainConflictSolverControlViewModel(MergeViewerControlViewModel mergeViewer)
        {
            MergeViewer = mergeViewer;
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
        /// Gets or sets the merge viewer.
        /// </summary>
        /// <value>The merge viewer.</value>
        public virtual MergeViewerControlViewModel MergeViewer { get; protected set; }

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

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
