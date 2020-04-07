// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-18-2020
//
// Last Modified By : Mario
// Last Modified On : 04-07-2020
// ***********************************************************************
// <copyright file="MainConflictSolverControlView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using IronyModManager.Common.Views;
using IronyModManager.Shared;
using IronyModManager.ViewModels;
using ReactiveUI;

namespace IronyModManager.Views
{
    /// <summary>
    /// Class MainConflictSolverControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.MainConflictSolverControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.MainConflictSolverControlViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class MainConflictSolverControlView : BaseControl<MainConflictSolverControlViewModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainConflictSolverControlView" /> class.
        /// </summary>
        public MainConflictSolverControlView()
        {
            this.InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            base.OnActivated(disposables);
            var conflicts = this.FindControl<TreeView>("conflicts");
            this.WhenAnyValue(v => v.ViewModel.SelectedConflict).Where(p => p != null).Subscribe(s =>
            {
                int idx = 0;
                bool matchFound = false;
                foreach (var item in ViewModel.HierarchalConflicts)
                {
                    if (item.Children.Contains(ViewModel.SelectedConflict))
                    {
                        matchFound = true;
                        break;
                    }
                    idx++;
                }
                if (idx > (ViewModel.HierarchalConflicts.Count() - 1))
                {
                    idx = ViewModel.HierarchalConflicts.Count() - 1;
                }
                if (matchFound && conflicts.Presenter.Panel.Children[idx] is TreeViewItem tvItem && !tvItem.IsExpanded)
                {
                    tvItem.IsExpanded = true;
                }
            }).DisposeWith(disposables);
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        #endregion Methods
    }
}
