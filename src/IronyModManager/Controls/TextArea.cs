// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-24-2024
//
// Last Modified By : Mario
// Last Modified On : 02-24-2024
// ***********************************************************************
// <copyright file="TextArea.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using Avalonia.Styling;
using IronyModManager.Shared;

namespace IronyModManager.Controls
{
    /// <summary>
    /// The text area.
    /// </summary>
    /// <seealso cref="AvaloniaEdit.Editing.TextArea" />
    [ExcludeFromCoverage("Should be tested in functional testing.")]
    public class TextArea : AvaloniaEdit.Editing.TextArea, IStyleable
    {
        /// <summary>
        /// Occurs when [left pointer pressed].
        /// </summary>
        public event EventHandler<PointerPressedEventArgs> LeftPointerPressed;

        #region Properties

        /// <summary>
        /// Gets a value representing the style key.<see cref="System.Type" />
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(AvaloniaEdit.Editing.TextArea);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Handles the <see cref="E:PointerPressed" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerPressedEventArgs" /> instance containing the event data.</param>
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            // You're asking why? Ask whoever designed avaloia which prevents it from routing events even if they are freaking handled
            LeftPointerPressed?.Invoke(this, e);
            base.OnPointerPressed(e);
        }

        #endregion Methods
    }
}
