// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 03-15-2021
//
// Last Modified By : Mario
// Last Modified On : 03-15-2021
// ***********************************************************************
// <copyright file="IronyClipboard.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;
using IronyModManager.Shared;

namespace IronyModManager.Platform.Clipboard
{
    /// <summary>
    /// Class IronyClipboard.
    /// Implements the <see cref="IronyModManager.Platform.Clipboard.IIronyClipboard" />
    /// </summary>
    /// <seealso cref="IronyModManager.Platform.Clipboard.IIronyClipboard" />
    internal class IronyClipboard : IIronyClipboard
    {
        #region Fields

        /// <summary>
        /// The clipboard
        /// </summary>
        private static IClipboard clipboard;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IronyClipboard" /> class.
        /// </summary>
        /// <param name="underlyingClipboard">The underlying clipboard.</param>
        public IronyClipboard(IClipboard underlyingClipboard)
        {
            clipboard = underlyingClipboard;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [prevent multi line text].
        /// </summary>
        /// <value><c>true</c> if [prevent multi line text]; otherwise, <c>false</c>.</value>
        public bool PreventMultiLineText { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Clears the asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        public Task ClearAsync()
        {
            return clipboard.ClearAsync();
        }

        /// <summary>
        /// Gets the data asynchronous.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>Task&lt;System.Object&gt;.</returns>
        public Task<object> GetDataAsync(string format)
        {
            return clipboard.GetDataAsync(format);
        }

        /// <summary>
        /// Gets the formats asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.String[]&gt;.</returns>
        public Task<string[]> GetFormatsAsync()
        {
            return clipboard.GetFormatsAsync();
        }

        /// <summary>
        /// Gets the text asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public async Task<string> GetTextAsync()
        {
            var text = await clipboard.GetTextAsync();
            if (PreventMultiLineText)
            {
                return CleanupCarriageReturns(text);
            }
            return text;
        }

        /// <summary>
        /// Sets the data object asynchronous.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Task.</returns>
        public Task SetDataObjectAsync(IDataObject data)
        {
            return clipboard.SetDataObjectAsync(data);
        }

        /// <summary>
        /// Sets the text asynchronous.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Task.</returns>
        public async Task SetTextAsync(string text)
        {
            if (PreventMultiLineText)
            {
                text = CleanupCarriageReturns(text);
            }
            await clipboard.SetTextAsync(text);
        }

        /// <summary>
        /// Cleanups the carriage returns.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        private string CleanupCarriageReturns(string text)
        {
            if (text.Contains('\r') || text.Contains('\n'))
            {
                return text.SplitOnNewLine().ToList()[0];
            }
            return text;
        }

        #endregion Methods
    }
}
