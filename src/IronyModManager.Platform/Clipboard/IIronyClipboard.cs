// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 03-15-2021
//
// Last Modified By : Mario
// Last Modified On : 03-15-2021
// ***********************************************************************
// <copyright file="IIronyClipboard.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input.Platform;

namespace IronyModManager.Platform.Clipboard
{
    /// <summary>
    /// Interface IIronyClipboard
    /// Implements the <see cref="Avalonia.Input.Platform.IClipboard" />
    /// </summary>
    /// <seealso cref="Avalonia.Input.Platform.IClipboard" />
    public interface IIronyClipboard : IClipboard
    {
        #region Properties

        // This is solely here to signal the clipboard to return/set only single line statements and ignore carriage returns.
        // Why am I doing this dirty hack you ask?
        // Well because avalonia (again). No methods can be safely overridden or are totally private.
        /// <summary>
        /// Gets or sets a value indicating whether [prevent multi line text].
        /// </summary>
        /// <value><c>true</c> if [prevent multi line text]; otherwise, <c>false</c>.</value>
        bool PreventMultiLineText { get; set; }

        #endregion Properties
    }
}
