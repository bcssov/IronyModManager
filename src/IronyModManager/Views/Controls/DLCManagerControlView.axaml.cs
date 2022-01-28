// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-15-2021
//
// Last Modified By : Mario
// Last Modified On : 01-27-2022
// ***********************************************************************
// <copyright file="DLCManagerControlView.axaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
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
        #region Fields

        /// <summary>
        /// The popup
        /// </summary>
        private Popup popup;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DLCManagerControlView" /> class.
        /// </summary>
        public DLCManagerControlView()
        {
            InitializeComponent();
            LayoutUpdated += DLCManagerControlView_LayoutUpdated;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            popup = this.FindControl<Popup>("popup");
            UpdateOffsets();
            popup.Closed += (sender, args) =>
            {
                ViewModel.ForceClose();
            };
            base.OnActivated(disposables);
        }

        /// <summary>
        /// Handles the LayoutUpdated event of the DLCManagerControlView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void DLCManagerControlView_LayoutUpdated(object sender, System.EventArgs e)
        {
            if (popup != null)
            {
                UpdateOffsets();
            }
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Updates the offsets.
        /// </summary>
        private void UpdateOffsets()
        {
            var window = Helpers.GetMainWindow();
            var verticalOffset = Helpers.CalculatePopupCenterPosition(400, window.Bounds.Height, 200);
            var horizontalOffset = Helpers.CalculatePopupCenterPosition(600, window.Bounds.Width, 100);
            popup.VerticalOffset = verticalOffset;
            popup.HorizontalOffset = horizontalOffset;
        }

        #endregion Methods
    }
}
