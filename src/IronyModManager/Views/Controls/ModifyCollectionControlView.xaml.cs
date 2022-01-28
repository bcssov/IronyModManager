// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-09-2020
//
// Last Modified By : Mario
// Last Modified On : 01-27-2022
// ***********************************************************************
// <copyright file="ModifyCollectionControlView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Reactive.Disposables;
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
    /// Class ModifyCollectionControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ModifyCollectionControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ModifyCollectionControlViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ModifyCollectionControlView : BaseControl<ModifyCollectionControlViewModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyCollectionControlView" /> class.
        /// </summary>
        public ModifyCollectionControlView()
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
                ViewModel.ForceClosePopups();
            };
            MessageBus.Current.Listen<ForceClosePopulsEventArgs>()
            .SubscribeObservable(x =>
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ViewModel.ForceClosePopups();
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
