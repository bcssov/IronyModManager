// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-30-2020
//
// Last Modified By : Mario
// Last Modified On : 06-01-2020
// ***********************************************************************
// <copyright file="OptionsControlView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Reactive.Disposables;
using Avalonia.Markup.Xaml;
using IronyModManager.Common;
using IronyModManager.Common.Events;
using IronyModManager.Common.Views;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;
using ReactiveUI;

namespace IronyModManager.Views.Controls
{
    /// <summary>
    /// Class OptionsControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.OptionsControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.OptionsControlViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class OptionsControlView : BaseControl<OptionsControlViewModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsControlView" /> class.
        /// </summary>
        public OptionsControlView()
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
                    ViewModel.ForceClose();
                }).DisposeWith(disposables);
            MessageBus.Current.Listen<ForceClosePopulsEventArgs>()
                .SubscribeObservable(x =>
                {
                    ViewModel.ForceClose();
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
