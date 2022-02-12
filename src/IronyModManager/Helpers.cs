// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-12-2020
//
// Last Modified By : Mario
// Last Modified On : 01-27-2022
// ***********************************************************************
// <copyright file="Helpers.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace IronyModManager
{
    /// <summary>
    /// Class Helpers.
    /// </summary>
    public static class Helpers
    {
        #region Methods

        /// <summary>
        /// Calculates the popup center position.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="otherSize">Size of the other.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>System.Int32.</returns>
        public static int CalculatePopupCenterPosition(double size, double otherSize, double offset)
        {
            var center = size / 2d;
            var value = 0;
            if (!double.IsNaN(otherSize) && otherSize > 0)
            {
                value = Convert.ToInt32(otherSize / 2d - center);
            }
            else
            {
                value = Convert.ToInt32(center);
            }
            return value + Convert.ToInt32(offset);
        }

        /// <summary>
        /// Gets the main window.
        /// </summary>
        /// <returns>Window.</returns>
        public static Window GetMainWindow()
        {
            if (Application.Current?.ApplicationLifetime != null)
            {
                return ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;
            }
            return null;
        }

        #endregion Methods
    }
}
