// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 11-23-2022
//
// Last Modified By : Mario
// Last Modified On : 11-23-2022
// ***********************************************************************
// <copyright file="IronyClientSideDecorations.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Taken from avalonia 0.11.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Styling;

namespace IronyModManager.Controls.ClientSideDecorations
{
#nullable enable

    /// <summary>
    /// Class IronyClientSideDecorations.
    /// </summary>
    public class IronyClientSideDecorations : TemplatedControl, IStyleable
    {
        #region Fields

        /// <summary>
        /// The toggle visibility disposable
        /// </summary>
        private IDisposable? _toggleVisibilityDisposable;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the type by which the control is styled.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(IronyClientSideDecorations);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Handles the <see cref="E:ApplyTemplate" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TemplateAppliedEventArgs" /> instance containing the event data.</param>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            SetupResizeBorder(e, "PART_ResizeTop", StandardCursorType.TopSide, WindowEdge.North);
            SetupResizeBorder(e, "PART_ResizeRight", StandardCursorType.RightSide, WindowEdge.East);
            SetupResizeBorder(e, "PART_ResizeBottom", StandardCursorType.BottomSide, WindowEdge.South);
            SetupResizeBorder(e, "PART_ResizeLeft", StandardCursorType.LeftSide, WindowEdge.West);
            SetupResizeBorder(e, "PART_ResizeTopLeft", StandardCursorType.TopLeftCorner, WindowEdge.NorthWest);
            SetupResizeBorder(e, "PART_ResizeTopRight", StandardCursorType.TopRightCorner, WindowEdge.NorthEast);
            SetupResizeBorder(e, "PART_ResizeBottomLeft", StandardCursorType.BottomLeftCorner, WindowEdge.SouthWest);
            SetupResizeBorder(e, "PART_ResizeBottomRight", StandardCursorType.BottomRightCorner, WindowEdge.SouthEast);
        }

        /// <summary>
        /// Called when the control is added to a rooted visual tree.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (VisualRoot is Window window)
                _toggleVisibilityDisposable = window.GetObservable(Window.ExtendClientAreaChromeHintsProperty)
                    .Subscribe(_ => IsVisible = window.PlatformImpl?.NeedsManagedDecorations ?? false);
        }

        /// <summary>
        /// Called when the control is removed from a rooted visual tree.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            _toggleVisibilityDisposable?.Dispose();
        }

        /// <summary>
        /// Setups the resize border.
        /// </summary>
        /// <param name="e">The <see cref="TemplateAppliedEventArgs" /> instance containing the event data.</param>
        /// <param name="name">The name.</param>
        /// <param name="cursor">The cursor.</param>
        /// <param name="edge">The edge.</param>
        private void SetupResizeBorder(TemplateAppliedEventArgs e, string name, StandardCursorType cursor, WindowEdge edge)
        {
            var control = e.NameScope.Get<IronyResizeBorder>(name);
            control.Cursor = new Cursor(cursor);
            control.PointerPressed += (_, args) => (VisualRoot as Window)?.PlatformImpl?.BeginResizeDrag(edge, args);
        }

        #endregion Methods
    }
}
