// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 06-11-2020
//
// Last Modified By : Mario
// Last Modified On : 06-12-2020
// ***********************************************************************
// <copyright file="ConflictSolverResetConflictsView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Linq;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using IronyModManager.Common;
using IronyModManager.Common.Events;
using IronyModManager.Common.Views;
using IronyModManager.ViewModels.Controls;
using ReactiveUI;

namespace IronyModManager.Views.Controls
{
    /// <summary>
    /// Class ConflictSolverResetConflictsView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ConflictSolverResetConflictsViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ConflictSolverResetConflictsViewModel}" />
    public class ConflictSolverResetConflictsView : BaseControl<ConflictSolverResetConflictsViewModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictSolverResetConflictsView" /> class.
        /// </summary>
        public ConflictSolverResetConflictsView()
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
            MessageBus.Current.Listen<WindowSizeChangedEventArgs>()
                .SubscribeObservable(x =>
                {
                    ViewModel.ForceClosePopup();
                }).DisposeWith(disposables);
            MessageBus.Current.Listen<ForceClosePopulsEventArgs>()
                .SubscribeObservable(x =>
                {
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        ViewModel.ForceClosePopup();
                    });
                }).DisposeWith(disposables);

            var conflictList = this.FindControl<ListBox>("conflictList");
            conflictList.SelectionChanged += (sender, args) =>
            {
                if (conflictList?.SelectedIndex > -1 && ViewModel.SelectedParentHierarchicalDefinition != null)
                {
                    ViewModel.SelectedHierarchicalDefinition = ViewModel.SelectedParentHierarchicalDefinition.Children.ElementAt(conflictList.SelectedIndex);
                }
                else
                {
                    ViewModel.SelectedHierarchicalDefinition = null;
                }
            };
            this.WhenAnyValue(v => v.ViewModel.SelectedParentHierarchicalDefinition).SubscribeObservable(s =>
            {
                if (s?.Children.Count > 0)
                {
                    if (ViewModel.PreviousConflictIndex.HasValue)
                    {
                        conflictList.SelectedIndex = ViewModel.PreviousConflictIndex.GetValueOrDefault();
                    }
                    else
                    {
                        conflictList.SelectedIndex = 0;
                    }
                }
            }).DisposeWith(disposables);
            base.OnActivated(disposables);
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
