// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="X11CursorFactory.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using Avalonia.Platform;

namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Class X11CursorFactory.
    /// Implements the <see cref="Avalonia.Platform.IStandardCursorFactory" />
    /// </summary>
    /// <seealso cref="Avalonia.Platform.IStandardCursorFactory" />
    internal class X11CursorFactory : IStandardCursorFactory
    {
        #region Fields

        /// <summary>
        /// The null cursor data
        /// </summary>
        private static readonly byte[] NullCursorData = new byte[] { 0 };

        /// <summary>
        /// The s mapping
        /// </summary>
        private static readonly Dictionary<StandardCursorType, CursorFontShape> s_mapping =
            new Dictionary<StandardCursorType, CursorFontShape>
            {
                {StandardCursorType.Arrow, CursorFontShape.XC_top_left_arrow},
                {StandardCursorType.Cross, CursorFontShape.XC_cross},
                {StandardCursorType.Hand, CursorFontShape.XC_hand1},
                {StandardCursorType.Help, CursorFontShape.XC_question_arrow},
                {StandardCursorType.Ibeam, CursorFontShape.XC_xterm},
                {StandardCursorType.No, CursorFontShape.XC_X_cursor},
                {StandardCursorType.Wait, CursorFontShape.XC_watch},
                {StandardCursorType.AppStarting, CursorFontShape.XC_watch},
                {StandardCursorType.BottomSide, CursorFontShape.XC_bottom_side},
                {StandardCursorType.DragCopy, CursorFontShape.XC_center_ptr},
                {StandardCursorType.DragLink, CursorFontShape.XC_fleur},
                {StandardCursorType.DragMove, CursorFontShape.XC_diamond_cross},
                {StandardCursorType.LeftSide, CursorFontShape.XC_left_side},
                {StandardCursorType.RightSide, CursorFontShape.XC_right_side},
                {StandardCursorType.SizeAll, CursorFontShape.XC_sizing},
                {StandardCursorType.TopSide, CursorFontShape.XC_top_side},
                {StandardCursorType.UpArrow, CursorFontShape.XC_sb_up_arrow},
                {StandardCursorType.BottomLeftCorner, CursorFontShape.XC_bottom_left_corner},
                {StandardCursorType.BottomRightCorner, CursorFontShape.XC_bottom_right_corner},
                {StandardCursorType.SizeNorthSouth, CursorFontShape.XC_sb_v_double_arrow},
                {StandardCursorType.SizeWestEast, CursorFontShape.XC_sb_h_double_arrow},
                {StandardCursorType.TopLeftCorner, CursorFontShape.XC_top_left_corner},
                {StandardCursorType.TopRightCorner, CursorFontShape.XC_top_right_corner},
            };

        /// <summary>
        /// The null cursor
        /// </summary>
        private static IntPtr _nullCursor;

        /// <summary>
        /// The cursors
        /// </summary>
        private readonly Dictionary<CursorFontShape, IntPtr> _cursors;

        /// <summary>
        /// The display
        /// </summary>
        private readonly IntPtr _display;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="X11CursorFactory"/> class.
        /// </summary>
        /// <param name="display">The display.</param>
        public X11CursorFactory(IntPtr display)
        {
            _display = display;
            _nullCursor = GetNullCursor(display);
            _cursors = Enum.GetValues(typeof(CursorFontShape)).Cast<CursorFontShape>()
                .ToDictionary(id => id, id => XLib.XCreateFontCursor(_display, id));
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the cursor.
        /// </summary>
        /// <param name="cursorType">Type of the cursor.</param>
        /// <returns>IPlatformHandle.</returns>
        public IPlatformHandle GetCursor(StandardCursorType cursorType)
        {
            IntPtr handle;
            if (cursorType == StandardCursorType.None)
            {
                handle = _nullCursor;
            }
            else
            {
                handle = s_mapping.TryGetValue(cursorType, out var shape)
                ? _cursors[shape]
                : _cursors[CursorFontShape.XC_top_left_arrow];
            }
            return new PlatformHandle(handle, "XCURSOR");
        }

        /// <summary>
        /// Gets the null cursor.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <returns>IntPtr.</returns>
        private static IntPtr GetNullCursor(IntPtr display)
        {
            XColor color = new XColor();
            IntPtr window = XLib.XRootWindow(display, 0);
            IntPtr pixmap = XLib.XCreateBitmapFromData(display, window, NullCursorData, 1, 1);
            return XLib.XCreatePixmapCursor(display, pixmap, pixmap, ref color, ref color, 0, 0);
        }

        #endregion Methods
    }
}
