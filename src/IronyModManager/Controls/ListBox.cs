// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 12-14-2020
//
// Last Modified By : Mario
// Last Modified On : 12-14-2020
// ***********************************************************************
// <copyright file="ListBox.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
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
        /// The CTX raised by pointer
        /// </summary>
        private bool ctxRaisedByPointer = false;

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
        /// Sets the context menu.
        /// </summary>
        /// <param name="menuItems">The menu items.</param>
        public void SetContextMenu(IReadOnlyCollection<MenuItem> menuItems)
        {
            if (menuItems?.Count > 0)
            {
                ContextMenu = new ContextMenu
                {
                    Items = menuItems
                };
            }
            else
            {
                ContextMenu = null;
            }
        }

        /// <summary>
        /// Handles the <see cref="E:PointerPressed" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerPressedEventArgs" /> instance containing the event data.</param>
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed)
            {
                ctxRaisedByPointer = true;
                RaiseContextMenuOpening();
                ctxRaisedByPointer = false;
            }
        }

        /// <summary>
        /// Handles the <see cref="E:PropertyChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="AvaloniaPropertyChangedEventArgs" /> instance containing the event data.</param>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == ContextMenuProperty)
            {
                HandleContextMenuEvents(e.OldValue as ContextMenu);
            }
        }

        /// <summary>
        /// Handles the context menu events.
        /// </summary>
        /// <param name="oldContextMenu">The old context menu.</param>
        private void HandleContextMenuEvents(ContextMenu oldContextMenu)
        {
            if (oldContextMenu != null)
            {
                oldContextMenu.ContextMenuOpening -= OnContextMenuOpening;
                oldContextMenu.Close();
            }
            if (ContextMenu != null && ContextMenu != oldContextMenu)
            {
                ContextMenu.ContextMenuOpening += OnContextMenuOpening;
            }
        }

        /// <summary>
        /// Handles the <see cref="E:ContextMenuOpening" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs" /> instance containing the event data.</param>
        private void OnContextMenuOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!ctxRaisedByPointer)
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
