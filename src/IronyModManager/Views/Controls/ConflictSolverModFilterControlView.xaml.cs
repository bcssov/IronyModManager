﻿// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 06-08-2020
//
// Last Modified By : Mario
// Last Modified On : 02-01-2022
// ***********************************************************************
// <copyright file="ConflictSolverModFilterControlView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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
    /// Class ConflictSolverModFilterControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ConflictSolverModFilterControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ConflictSolverModFilterControlViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ConflictSolverModFilterControlView : BaseControl<ConflictSolverModFilterControlViewModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictSolverModFilterControlView" /> class.
        /// </summary>
        public ConflictSolverModFilterControlView()
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
            var popup = this.FindControl<Popup>("popup");
            popup.Closed += (sender, args) =>
            {
                ViewModel.ForceClosePopup();
            };
            MessageBus.Current.Listen<ForceClosePopulsEventArgs>()
            .SubscribeObservable(x =>
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ViewModel.ForceClosePopup();
                });
            }).DisposeWith(disposables);

            var modList = this.FindControl<ListBox>("modList");
            this.WhenAnyValue(v => v.ViewModel.ResetView).Where(p => p).SubscribeObservable(s =>
            {
                Dispatcher.UIThread.SafeInvoke(() =>
                {
                    modList.ScrollIntoView(0);
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
