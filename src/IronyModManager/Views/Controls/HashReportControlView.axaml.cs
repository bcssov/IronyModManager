// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 10-01-2020
//
// Last Modified By : Mario
// Last Modified On : 01-27-2022
// ***********************************************************************
// <copyright file="HashReportControlView.axaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
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
    /// Class HashReportControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl`1" />
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.HashReportControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.HashReportControlViewModel}" />
    /// <seealso cref="IronyModManager.Common.Views.BaseControl`1" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class HashReportControlView : BaseControl<HashReportControlViewModel>
    {
        #region Fields

        /// <summary>
        /// The popup
        /// </summary>
        private Popup popup;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModHashReportControlView" /> class.
        /// </summary>
        public HashReportControlView()
        {
            InitializeComponent();
            LayoutUpdated += HashReportControlView_LayoutUpdated;
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

            popup = this.FindControl<Popup>("popup");
            UpdateOffsets();
            popup.Closed += (sender, args) =>
            {
                ViewModel.ForceClose();
            };
        }

        /// <summary>
        /// Handles the LayoutUpdated event of the HashReportControlView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void HashReportControlView_LayoutUpdated(object sender, System.EventArgs e)
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
            var verticalOffset = Helpers.CalculatePopupCenterPosition(400, window.Bounds.Height, 125);
            popup.VerticalOffset = verticalOffset;
        }

        #endregion Methods
    }
}
