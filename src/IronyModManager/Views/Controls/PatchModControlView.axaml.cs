// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-16-2021
//
// Last Modified By : Mario
// Last Modified On : 02-16-2021
// ***********************************************************************
// <copyright file="PatchModControlView.axaml.cs" company="Mario">
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
    /// Class PatchModControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.PatchModControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.PatchModControlViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class PatchModControlView : BaseControl<PatchModControlViewModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PatchModControlView" /> class.
        /// </summary>
        public PatchModControlView()
        {
            InitializeComponent();
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
                ViewModel.ForceClose();
            };
            popup.Opened += (sender, args) =>
            {
                popup.Host.ConfigurePosition(popup.PlacementTarget, popup.PlacementMode, new Avalonia.Point(popup.HorizontalOffset, 15),
                    Avalonia.Controls.Primitives.PopupPositioning.PopupPositioningEdge.None, Avalonia.Controls.Primitives.PopupPositioning.PopupPositioningEdge.Bottom);
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
