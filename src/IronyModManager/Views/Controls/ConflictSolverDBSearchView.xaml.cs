// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 06-14-2020
//
// Last Modified By : Mario
// Last Modified On : 06-14-2020
// ***********************************************************************
// <copyright file="ConflictSolverDBSearchView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Reactive.Disposables;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using IronyModManager.Common;
using IronyModManager.Common.Events;
using IronyModManager.Common.Views;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;
using ReactiveUI;

namespace IronyModManager.Views.Controls
{
    /// <summary>
    /// Class ConflictSolverDBSearchView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ConflictSolverDBSearchViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ConflictSolverDBSearchViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ConflictSolverDBSearchView : BaseControl<ConflictSolverDBSearchViewModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictSolverDBSearchView" /> class.
        /// </summary>
        public ConflictSolverDBSearchView()
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
