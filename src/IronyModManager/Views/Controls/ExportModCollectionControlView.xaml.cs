// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-09-2020
//
// Last Modified By : Mario
// Last Modified On : 03-10-2021
// ***********************************************************************
// <copyright file="ExportModCollectionControlView.xaml.cs" company="Mario">
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
    /// Class ExportModCollectionControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ExportModCollectionControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ExportModCollectionControlViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ExportModCollectionControlView : BaseControl<ExportModCollectionControlViewModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportModCollectionControlView" /> class.
        /// </summary>
        public ExportModCollectionControlView()
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
            var popupImport = this.FindControl<Popup>("popupImport");
            popupImport.Closed += (sender, args) =>
            {
                ViewModel.ForceClose();
            };
            popupImport.Opened += (sender, args) =>
            {
                popupImport.Host.ConfigurePosition(popupImport.PlacementTarget, popupImport.PlacementMode, new Avalonia.Point(popupImport.HorizontalOffset, 15),
                    Avalonia.Controls.Primitives.PopupPositioning.PopupAnchor.None, Avalonia.Controls.Primitives.PopupPositioning.PopupGravity.Bottom);
            };
            var popupExport = this.FindControl<Popup>("popupExport");
            popupExport.Closed += (sender, args) =>
            {
                ViewModel.ForceClose();
            };
            popupExport.Opened += (sender, args) =>
            {
                popupExport.Host.ConfigurePosition(popupExport.PlacementTarget, popupExport.PlacementMode, new Avalonia.Point(popupExport.HorizontalOffset, 15),
                    Avalonia.Controls.Primitives.PopupPositioning.PopupAnchor.None, Avalonia.Controls.Primitives.PopupPositioning.PopupGravity.Bottom);
            };
            MessageBus.Current.Listen<ForceClosePopulsEventArgs>()
            .SubscribeObservable(x =>
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ViewModel.ForceClose();
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
