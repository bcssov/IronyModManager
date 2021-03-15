// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 12-14-2020
//
// Last Modified By : Mario
// Last Modified On : 03-14-2021
// ***********************************************************************
// <copyright file="ListBox.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Styling;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class ListBox.
    /// Implements the <see cref="Avalonia.Controls.ListBox" />
    /// Implements the <see cref="Avalonia.Styling.IStyleable" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.ListBox" />
    /// <seealso cref="Avalonia.Styling.IStyleable" />
    public class ListBox : Avalonia.Controls.ListBox, IStyleable
    {
        #region Fields

        /// <summary>
        /// The context menu
        /// </summary>
        private ContextMenu contextMenu;

        #endregion Fields

        #region Events

        /// <summary>
        /// Occurs when [context menu opening].
        /// </summary>
        public event EventHandler ContextMenuOpening;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets the style key.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(Avalonia.Controls.ListBox);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Sets the context menu items.
        /// </summary>
        /// <param name="menuItems">The menu items.</param>
        public void SetContextMenuItems(IReadOnlyCollection<MenuItem> menuItems)
        {
            ContextMenu = null;
            if (contextMenu == null)
            {
                contextMenu = new ContextMenu();
            }
            if (menuItems?.Count > 0)
            {
                contextMenu.Items = menuItems;
                contextMenu.Open(this);
            }
        }

        /// <summary>
        /// Handles the <see cref="E:PointerPressed" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerPressedEventArgs" /> instance containing the event data.</param>
        /// <inheritdoc />
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed)
            {
                RaiseContextMenuOpening();
            }
        }

        /// <summary>
        /// Raises the context menu opening.
        /// </summary>
        private void RaiseContextMenuOpening()
        {
            ContextMenuOpening?.Invoke(this, EventArgs.Empty);
        }

        #endregion Methods
    }
}
