// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-14-2020
//
// Last Modified By : Mario
// Last Modified On : 03-14-2020
// ***********************************************************************
// <copyright file="MinMaxButtonSpinner.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Styling;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class MinMaxButtonSpinner.
    /// Implements the <see cref="Avalonia.Controls.ButtonSpinner" />
    /// Implements the <see cref="Avalonia.Styling.IStyleable" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.ButtonSpinner" />
    /// <seealso cref="Avalonia.Styling.IStyleable" />
    public class MinMaxButtonSpinner : ButtonSpinner, IStyleable
    {
        #region Properties

        /// <summary>
        /// Gets the style key.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(MinMaxButtonSpinner);

        #endregion Properties
    }
}
