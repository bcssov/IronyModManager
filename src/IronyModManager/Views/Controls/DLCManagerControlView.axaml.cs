// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-15-2021
//
// Last Modified By : Mario
// Last Modified On : 02-15-2021
// ***********************************************************************
// <copyright file="DLCManagerControlView.axaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using IronyModManager.Common.Views;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;

namespace IronyModManager.Views.Controls
{
    /// <summary>
    /// Class DLCManagerControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.DLCManagerControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.DLCManagerControlViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class DLCManagerControlView : BaseControl<DLCManagerControlViewModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DLCManagerControlView" /> class.
        /// </summary>
        public DLCManagerControlView()
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
                var window = Helpers.GetMainWindow();
                var verticalOffset = window.Bounds.Height / 2;
                popup.Host.ConfigurePosition(window, popup.PlacementMode, new Avalonia.Point(popup.HorizontalOffset, verticalOffset),
                   Avalonia.Controls.Primitives.PopupPositioning.PopupPositioningEdge.Top, Avalonia.Controls.Primitives.PopupPositioning.PopupPositioningEdge.None);
            };

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
