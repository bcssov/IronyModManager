// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-11-2021
//
// Last Modified By : Mario
// Last Modified On : 01-27-2022
// ***********************************************************************
// <copyright file="CustomMessageBox.axaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using Avalonia.Markup.Xaml;
using IronyModManager.DI;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using MessageBox.Avalonia.Views;

namespace IronyModManager.Controls.Themes
{
    /// <summary>
    /// Class CustomMessageBox.
    /// Implements the <see cref="MessageBox.Avalonia.Views.MsBoxCustomWindow" />
    /// </summary>
    /// <seealso cref="MessageBox.Avalonia.Views.MsBoxCustomWindow" />
    public class CustomMessageBox : MsBoxCustomWindow
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomMessageBox" /> class.
        /// </summary>
        public CustomMessageBox()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Shows the window.
        /// </summary>
        public override void Show()
        {
            base.Show();
            InitWindowSize();
        }

        /// <summary>
        /// Initializes the size of the window.
        /// </summary>
        protected virtual void InitWindowSize()
        {
            static int calculateCenterPosition(double pos, double otherSize, double thisSize)
            {
                var otherCenter = pos + otherSize / 2d;
                if (!double.IsNaN(thisSize))
                {
                    return Convert.ToInt32(otherCenter - thisSize / 2d);
                }
                return Convert.ToInt32(otherCenter);
            }

            var service = DIResolver.Get<IWindowStateService>();
            if (service.IsDefined() && !service.IsMaximized())
            {
                var oldPos = Position;
                try
                {
                    var state = service.Get();
                    var pos = Position.WithX(calculateCenterPosition(state.LocationX.GetValueOrDefault(), state.Width.GetValueOrDefault(), Width));
                    pos = pos.WithY(calculateCenterPosition(state.LocationY.GetValueOrDefault(), state.Height.GetValueOrDefault(), Height));
                    Position = pos;
                }
                catch (Exception ex)
                {
                    // Sometimes people change their monitor configuration or their system breaks down, so fix this
                    var log = DIResolver.Get<ILogger>();
                    log.Error(ex);
                    WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterScreen;
                    Position = oldPos;
                }
            }
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
