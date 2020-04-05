// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-13-2020
//
// Last Modified By : Mario
// Last Modified On : 04-05-2020
// ***********************************************************************
// <copyright file="DragDropListBox.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Styling;
using Avalonia.VisualTree;
using IronyModManager.Shared;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class DragDropListBox.
    /// Implements the <see cref="Avalonia.Controls.ListBox" />
    /// Implements the <see cref="Avalonia.Styling.IStyleable" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.ListBox" />
    /// <seealso cref="Avalonia.Styling.IStyleable" />
    [ExcludeFromCoverage("Should be tested in functional testing.")]
    public class DragDropListBox : ListBox, IStyleable
    {
        #region Fields

        /// <summary>
        /// The move bottom
        /// </summary>
        private const string MoveBottom = "MoveBottom";

        /// <summary>
        /// The move top
        /// </summary>
        private const string MoveTop = "MoveTop";

        /// <summary>
        /// The classes
        /// </summary>
        private static readonly string[] classes = new string[] { MoveTop, MoveBottom };

        /// <summary>
        /// The cached item list
        /// </summary>
        private List<object> cachedItemList;

        /// <summary>
        /// The dragged item content
        /// </summary>
        private object draggedItemContent;

        /// <summary>
        /// The original cursor
        /// </summary>
        private Cursor originalCursor;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DragDropListBox" /> class.
        /// </summary>
        public DragDropListBox()
        {
            cachedItemList = new List<object>();
        }

        #endregion Constructors

        #region Delegates

        /// <summary>
        /// Delegate DraggedItemDelegate
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public delegate void DraggedItemDelegate(object source, object destination);

        #endregion Delegates

        #region Events

        /// <summary>
        /// Occurs when [mod reordered].
        /// </summary>
        public event DraggedItemDelegate ItemDragged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets the style key.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(ListBox);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Clears the drag styles.
        /// </summary>
        protected virtual void ClearDragStyles()
        {
            foreach (ListBoxItem child in this.GetLogicalChildren())
            {
                child.Classes.RemoveAll(classes);
            }
        }

        /// <summary>
        /// Itemses the changed.
        /// </summary>
        /// <param name="e">The <see cref="AvaloniaPropertyChangedEventArgs" /> instance containing the event data.</param>
        protected override void ItemsChanged(AvaloniaPropertyChangedEventArgs e)
        {
            base.ItemsChanged(e);

            if (e.NewValue != null)
            {
                cachedItemList = (e.NewValue as IEnumerable<object>).ToList();
            }
            else
            {
                cachedItemList = new List<object>();
            }
        }

        /// <summary>
        /// Itemses the collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        protected override void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.ItemsCollectionChanged(sender, e);
            if (Items != null)
            {
                cachedItemList = (Items as IEnumerable<object>).ToList();
            }
            else
            {
                cachedItemList = new List<object>();
            }
        }

        /// <summary>
        /// Handles the <see cref="E:PointerMoved" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerEventArgs" /> instance containing the event data.</param>
        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            if (draggedItemContent != null)
            {
                var hoveredItem = GetHoveredItem(e.GetPosition(this));
                if (hoveredItem != null)
                {
                    var dragIndex = cachedItemList.IndexOf(draggedItemContent);
                    var hoveredIndex = cachedItemList.IndexOf(hoveredItem.Content);

                    ClearDragStyles();

                    if (hoveredItem.Content != draggedItemContent)
                    {
                        hoveredItem.Classes.Add(dragIndex > hoveredIndex ? MoveTop : MoveBottom);
                    };
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="E:PointerPressed" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerPressedEventArgs" /> instance containing the event data.</param>
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
            {
                var hoveredItem = GetHoveredItem(e.GetPosition(this));
                if (hoveredItem != null)
                {
                    draggedItemContent = hoveredItem.Content;
                    originalCursor = Cursor;
                    Cursor = new Cursor(StandardCursorType.DragMove);
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="E:PointerReleased" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerReleasedEventArgs" /> instance containing the event data.</param>
        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
            {
                var hoveredItemContent = GetHoveredItem(e.GetPosition(this))?.Content;
                if (draggedItemContent != null && hoveredItemContent != null && hoveredItemContent != draggedItemContent)
                {
                    ItemDragged?.Invoke(draggedItemContent, hoveredItemContent);
                }
                ClearDragStyles();
                draggedItemContent = null;
                Cursor = originalCursor;
            }
        }

        /// <summary>
        /// Gets the hovered item.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>ListBoxItem.</returns>
        private ListBoxItem GetHoveredItem(Point position)
        {
            var visuals = this.GetVisualsAt(position);
            if (visuals?.Count() > 0)
            {
                var contentPresenter = visuals.OfType<ContentPresenter>().FirstOrDefault();
                return contentPresenter?.TemplatedParent as ListBoxItem;
            }
            return null;
        }

        #endregion Methods
    }
}
