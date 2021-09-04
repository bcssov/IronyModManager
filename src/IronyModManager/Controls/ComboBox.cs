// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 08-23-2021
//
// Last Modified By : Mario
// Last Modified On : 08-27-2021
// ***********************************************************************
// <copyright file="ComboBox.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using Avalonia.Styling;
using IronyModManager.Controls.Helper;
using IronyModManager.Shared.Models;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class ComboBox.
    /// Implements the <see cref="Avalonia.Controls.ComboBox" />
    /// Implements the <see cref="Avalonia.Styling.IStyleable" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.ComboBox" />
    /// <seealso cref="Avalonia.Styling.IStyleable" />
    public class ComboBox : Avalonia.Controls.ComboBox, IStyleable
    {
        #region Fields

        /// <summary>
        /// The search select
        /// </summary>
        private SearchSelect searchSelect;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the type by which the control is styled.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(Avalonia.Controls.ComboBox);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Handles the <see cref="E:TextInput" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TextInputEventArgs" /> instance containing the event data.</param>
        protected override void OnTextInput(TextInputEventArgs e)
        {
            if (searchSelect == null)
            {
                searchSelect = new SearchSelect();
            }
            var index = searchSelect.FindMatch(e, IsTextSearchEnabled, ItemContainerGenerator, Items as IEnumerable<IQueryableModel>);
            if (index.HasValue)
            {
                SelectedIndex = index.GetValueOrDefault();
            }
            base.OnTextInput(e);
        }

        #endregion Methods
    }
}
