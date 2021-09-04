// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 12-14-2020
//
// Last Modified By : Mario
// Last Modified On : 08-29-2021
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
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.VisualTree;
using IronyModManager.Controls.Helper;
using IronyModManager.DI;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;

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
        private MenuFlyout contextMenu;

        /// <summary>
        /// The search select
        /// </summary>
        private SearchSelect searchSelect;

        #endregion Fields

        #region Delegates

        /// <summary>
        /// Delegate ContextMenuOpeningDelegate
        /// </summary>
        /// <param name="listBoxItem">The list box item.</param>
        public delegate void ContextMenuOpeningDelegate(ListBoxItem listBoxItem);

        #endregion Delegates

        #region Events

        /// <summary>
        /// Occurs when [context menu opening].
        /// </summary>
        public event ContextMenuOpeningDelegate ContextMenuOpening;

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
                // Of course Avalonia update is going to fuck up something! It's no longer respecting and setting pointer value of the popup (which is private btw) in the context menu implementation
                contextMenu = new MenuFlyout()
                {
                    Placement = FlyoutPlacementMode.Bottom
                };
            }
            if (menuItems?.Count > 0)
            {
                contextMenu.Items = menuItems;
                contextMenu.ShowAt(this, true);
            }
        }

        /// <summary>
        /// Gets the hovered item.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>ListBoxItem.</returns>
        protected virtual ListBoxItem GetHoveredItem(Point position)
        {
            var visuals = this.GetVisualsAt(position);
            if (visuals?.Count() > 0)
            {
                var contentPresenter = visuals.OfType<ContentPresenter>().FirstOrDefault(p => (p.TemplatedParent as ListBoxItem) != null);
                return contentPresenter?.TemplatedParent as ListBoxItem;
            }
            return null;
        }

        /// <summary>
        /// Handles the <see cref="E:PointerPressed" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerPressedEventArgs" /> instance containing the event data.</param>
        /// <inheritdoc />
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            try
            {
                base.OnPointerPressed(e);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // Undetermined bug in Avalonia
                var logger = DIResolver.Get<ILogger>();
                logger.Error(ex);
            }
            catch (Exception)
            {
                throw;
            }

            if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed)
            {
                var hoveredItem = GetHoveredItem(e.GetPosition(this));
                RaiseContextMenuOpening(hoveredItem);
            }
        }

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

        /// <summary>
        /// Raises the context menu opening.
        /// </summary>
        /// <param name="listBoxItem">The list box item.</param>
        private void RaiseContextMenuOpening(ListBoxItem listBoxItem)
        {
            ContextMenuOpening?.Invoke(listBoxItem);
        }

        #endregion Methods
    }
}
