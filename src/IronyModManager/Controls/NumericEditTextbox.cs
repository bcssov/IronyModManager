// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 04-15-2021
//
// Last Modified By : Mario
// Last Modified On : 04-15-2021
// ***********************************************************************
// <copyright file="NumericEditTextbox.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Styling;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class NumericEditTextbox.
    /// Implements the <see cref="IronyModManager.Controls.TextBox" />
    /// Implements the <see cref="Avalonia.Styling.IStyleable" />
    /// </summary>
    /// <seealso cref="IronyModManager.Controls.TextBox" />
    /// <seealso cref="Avalonia.Styling.IStyleable" />
    public class NumericEditTextbox : TextBox, IStyleable
    {
        #region Properties

        /// <summary>
        /// Gets the type by which the control is styled.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(NumericEditTextbox);

        #endregion Properties
    }
}
