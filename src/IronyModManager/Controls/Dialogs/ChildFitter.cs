// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Avalonia
// Created          : 05-07-2020
//
// Last Modified By : Mario
// Last Modified On : 05-07-2020
// ***********************************************************************
// <copyright file="ChildFitter.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>
// Based on Avalonia ChildFitter. Why of why would
// the Avalonia guys expose some of this stuff?
// </summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using Avalonia.Controls;
using Size = Avalonia.Size;

namespace IronyModManager.Controls.Dialogs
{
    /// <summary>
    /// Class ChildFitter.
    /// Implements the <see cref="Avalonia.Controls.Decorator" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.Decorator" />
    public class ChildFitter : Decorator
    {
        #region Methods

        /// <summary>
        /// Arranges the override.
        /// </summary>
        /// <param name="finalSize">The final size.</param>
        /// <returns>Size.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Child.Measure(finalSize);
            base.ArrangeOverride(finalSize);
            return finalSize;
        }

        /// <summary>
        /// Measures the override.
        /// </summary>
        /// <param name="availableSize">Size of the available.</param>
        /// <returns>Size.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(0, 0);
        }

        #endregion Methods
    }
}
