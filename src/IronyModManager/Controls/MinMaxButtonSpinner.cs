// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-14-2020
//
// Last Modified By : Mario
// Last Modified On : 10-14-2020
// ***********************************************************************
// <copyright file="MinMaxButtonSpinner.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using Avalonia.Styling;
using IronyModManager.Shared;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class MinMaxButtonSpinner.
    /// Implements the <see cref="IronyModManager.Controls.NumericButtonSpinner" />
    /// Implements the <see cref="Avalonia.Styling.IStyleable" />
    /// </summary>
    /// <seealso cref="IronyModManager.Controls.NumericButtonSpinner" />
    /// <seealso cref="Avalonia.Styling.IStyleable" />
    [ExcludeFromCoverage("Should be tested in functional testing.")]
    public class MinMaxButtonSpinner : NumericButtonSpinner, IStyleable
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
