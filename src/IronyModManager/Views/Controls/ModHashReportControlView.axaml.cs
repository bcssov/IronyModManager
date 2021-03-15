// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 10-01-2020
//
// Last Modified By : Mario
// Last Modified On : 03-10-2021
// ***********************************************************************
// <copyright file="ModHashReportControlView.axaml.cs" company="Mario">
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
    /// Class ModHashReportControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ModHashReportControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ModHashReportControlViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ModHashReportControlView : BaseControl<ModHashReportControlViewModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModHashReportControlView" /> class.
        /// </summary>
        public ModHashReportControlView()
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
            base.OnActivated(disposables);

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
                    Avalonia.Controls.Primitives.PopupPositioning.PopupAnchor.Top, Avalonia.Controls.Primitives.PopupPositioning.PopupGravity.None);
            };
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
