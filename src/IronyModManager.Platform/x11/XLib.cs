// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="XLib.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia;
using IronyModManager.Shared;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable NotAccessedField.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Class XLib.
    /// </summary>
    [ExcludeFromCoverage("External component.")]
    internal static unsafe class XLib
    {
        #region Fields

        /// <summary>
        /// The library X11
        /// </summary>
        private const string LibX11 = "libX11.so.6";

        /// <summary>
        /// The library X11 ext
        /// </summary>
        private const string LibX11Ext = "libXext.so.6";

        /// <summary>
        /// The library X11 randr
        /// </summary>
        private const string LibX11Randr = "libXrandr.so.2";

        /// <summary>
        /// The library x input
        /// </summary>
        private const string LibXInput = "libXi.so.6";

        #endregion Fields

        #region Enums

        /// <summary>
        /// Enum XLookupStatus
        /// </summary>
        public enum XLookupStatus
        {
            /// <summary>
            /// The x buffer overflow
            /// </summary>
            XBufferOverflow = -1,

            /// <summary>
            /// The x lookup none
            /// </summary>
            XLookupNone = 1,

            /// <summary>
            /// The x lookup chars
            /// </summary>
            XLookupChars = 2,

            /// <summary>
            /// The x lookup key sym
            /// </summary>
            XLookupKeySym = 3,

            /// <summary>
            /// The x lookup both
            /// </summary>
            XLookupBoth = 4
        }

        #endregion Enums

        #region Properties

        /// <summary>
        /// Gets the length of the xi event mask.
        /// </summary>
        /// <value>The length of the xi event mask.</value>
        public static int XiEventMaskLen { get; } = 4;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Creates the event window.
        /// </summary>
        /// <param name="plat">The plat.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>IntPtr.</returns>
        public static IntPtr CreateEventWindow(AvaloniaX11Platform plat, Action<XEvent> handler)
        {
            var win = XCreateSimpleWindow(plat.Display, plat.Info.DefaultRootWindow,
                0, 0, 1, 1, 0, IntPtr.Zero, IntPtr.Zero);
            plat.Windows[win] = handler;
            return win;
        }

        /// <summary>
        /// Gets the name of the atom.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="atom">The atom.</param>
        /// <returns>System.String.</returns>
        public static string GetAtomName(IntPtr display, IntPtr atom)
        {
            var ptr = XGetAtomName(display, atom);
            if (ptr == IntPtr.Zero)
                return null;
            var s = Marshal.PtrToStringAnsi(ptr);
            XFree(ptr);
            return s;
        }

        /// <summary>
        /// Gets the cursor position.
        /// </summary>
        /// <param name="x11">The X11.</param>
        /// <param name="handle">The handle.</param>
        /// <returns>System.ValueTuple&lt;System.Int32, System.Int32&gt;.</returns>
        public static (int x, int y) GetCursorPos(X11Info x11, IntPtr? handle = null)
        {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            QueryPointer(x11.Display, handle ?? x11.RootWindow, out var root, out var child, out var root_x, out var root_y, out var win_x, out var win_y,
                out var keys_buttons);
#pragma warning restore IDE0059 // Unnecessary assignment of a value

            if (handle != null)
            {
                return (win_x, win_y);
            }
            else
            {
                return (root_x, root_y);
            }
        }

        /// <summary>
        /// Queries the pointer.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="w">The w.</param>
        /// <param name="root">The root.</param>
        /// <param name="child">The child.</param>
        /// <param name="root_x">The root x.</param>
        /// <param name="root_y">The root y.</param>
        /// <param name="child_x">The child x.</param>
        /// <param name="child_y">The child y.</param>
        /// <param name="mask">The mask.</param>
        public static void QueryPointer(IntPtr display, IntPtr w, out IntPtr root, out IntPtr child,
            out int root_x, out int root_y, out int child_x, out int child_y,
            out int mask)
        {
            XGrabServer(display);

            XQueryPointer(display, w, out root, out var c,
                out root_x, out root_y, out child_x, out child_y,
                out mask);

            if (root != w)
                c = root;

            IntPtr child_last = IntPtr.Zero;
            while (c != IntPtr.Zero)
            {
                child_last = c;
                XQueryPointer(display, c, out root, out c,
                    out root_x, out root_y, out child_x, out child_y,
                    out mask);
            }
            XUngrabServer(display);
            XFlush(display);

            child = child_last;
        }

        /// <summary>
        /// xes the color of the alloc.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="Colormap">The colormap.</param>
        /// <param name="colorcell_def">The colorcell definition.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XAllocColor(IntPtr display, IntPtr Colormap, ref XColor colorcell_def);

        /// <summary>
        /// xes the bell.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="percent">The percent.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XBell(IntPtr display, int percent);

        /// <summary>
        /// xes the black pixel.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="screen_no">The screen no.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XBlackPixel(IntPtr display, int screen_no);

        /// <summary>
        /// xes the change active pointer grab.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="event_mask">The event mask.</param>
        /// <param name="cursor">The cursor.</param>
        /// <param name="time">The time.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XChangeActivePointerGrab(IntPtr display, EventMask event_mask, IntPtr cursor,
            IntPtr time);

        /// <summary>
        /// xes the change property.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="property">The property.</param>
        /// <param name="type">The type.</param>
        /// <param name="format">The format.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="data">The data.</param>
        /// <param name="nelements">The nelements.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type,
            int format, PropertyMode mode, ref MotifWmHints data, int nelements);

        /// <summary>
        /// xes the change property.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="property">The property.</param>
        /// <param name="type">The type.</param>
        /// <param name="format">The format.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="value">The value.</param>
        /// <param name="nelements">The nelements.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type,
            int format, PropertyMode mode, ref uint value, int nelements);

        /// <summary>
        /// xes the change property.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="property">The property.</param>
        /// <param name="type">The type.</param>
        /// <param name="format">The format.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="value">The value.</param>
        /// <param name="nelements">The nelements.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type,
            int format, PropertyMode mode, ref IntPtr value, int nelements);

        /// <summary>
        /// xes the change property.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="property">The property.</param>
        /// <param name="type">The type.</param>
        /// <param name="format">The format.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="data">The data.</param>
        /// <param name="nelements">The nelements.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type,
            int format, PropertyMode mode, uint[] data, int nelements);

        /// <summary>
        /// xes the change property.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="property">The property.</param>
        /// <param name="type">The type.</param>
        /// <param name="format">The format.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="data">The data.</param>
        /// <param name="nelements">The nelements.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type,
            int format, PropertyMode mode, int[] data, int nelements);

        /// <summary>
        /// xes the change property.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="property">The property.</param>
        /// <param name="type">The type.</param>
        /// <param name="format">The format.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="data">The data.</param>
        /// <param name="nelements">The nelements.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type,
            int format, PropertyMode mode, IntPtr[] data, int nelements);

        /// <summary>
        /// xes the change property.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="property">The property.</param>
        /// <param name="type">The type.</param>
        /// <param name="format">The format.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="data">The data.</param>
        /// <param name="nelements">The nelements.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type,
            int format, PropertyMode mode, void* data, int nelements);

        /// <summary>
        /// xes the change property.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="property">The property.</param>
        /// <param name="type">The type.</param>
        /// <param name="format">The format.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="atoms">The atoms.</param>
        /// <param name="nelements">The nelements.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type,
            int format, PropertyMode mode, IntPtr atoms, int nelements);

        /// <summary>
        /// xes the change property.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="property">The property.</param>
        /// <param name="type">The type.</param>
        /// <param name="format">The format.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="text">The text.</param>
        /// <param name="text_length">Length of the text.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11, CharSet = CharSet.Ansi)]
        public static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type,
            int format, PropertyMode mode, string text, int text_length);

        /// <summary>
        /// xes the clear area.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="exposures">if set to <c>true</c> [exposures].</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XClearArea(IntPtr display, IntPtr window, int x, int y, int width, int height,
            bool exposures);

        /// <summary>
        /// xes the clear window.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XClearWindow(IntPtr display, IntPtr window);

        /// <summary>
        /// xes the close display.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XCloseDisplay(IntPtr display);

        /// <summary>
        /// xes the close im.
        /// </summary>
        /// <param name="xim">The xim.</param>
        [DllImport(LibX11)]
        public static extern void XCloseIM(IntPtr xim);

        /// <summary>
        /// xes the configure resize window.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="size">The size.</param>
        /// <returns>System.UInt32.</returns>
        public static uint XConfigureResizeWindow(IntPtr display, IntPtr window, PixelSize size)
            => XConfigureResizeWindow(display, window, size.Width, size.Height);

        /// <summary>
        /// xes the configure resize window.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>System.UInt32.</returns>
        public static uint XConfigureResizeWindow(IntPtr display, IntPtr window, int width, int height)
        {
            var changes = new XWindowChanges
            {
                width = width,
                height = height
            };

            return XConfigureWindow(display, window, ChangeWindowFlags.CWHeight | ChangeWindowFlags.CWWidth,
                ref changes);
        }

        /// <summary>
        /// xes the configure window.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="value_mask">The value mask.</param>
        /// <param name="values">The values.</param>
        /// <returns>System.UInt32.</returns>
        [DllImport(LibX11)]
        public static extern uint XConfigureWindow(IntPtr display, IntPtr window, ChangeWindowFlags value_mask,
            ref XWindowChanges values);

        /// <summary>
        /// xes the connection number.
        /// </summary>
        /// <param name="diplay">The diplay.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XConnectionNumber(IntPtr diplay);

        /// <summary>
        /// xes the convert selection.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="selection">The selection.</param>
        /// <param name="target">The target.</param>
        /// <param name="property">The property.</param>
        /// <param name="requestor">The requestor.</param>
        /// <param name="time">The time.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XConvertSelection(IntPtr display, IntPtr selection, IntPtr target, IntPtr property,
            IntPtr requestor, IntPtr time);

        /// <summary>
        /// xes the copy area.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="src">The source.</param>
        /// <param name="dest">The dest.</param>
        /// <param name="gc">The gc.</param>
        /// <param name="src_x">The source x.</param>
        /// <param name="src_y">The source y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="dest_x">The dest x.</param>
        /// <param name="dest_y">The dest y.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XCopyArea(IntPtr display, IntPtr src, IntPtr dest, IntPtr gc, int src_x, int src_y,
            int width, int height, int dest_x, int dest_y);

        /// <summary>
        /// xes the create bitmap from data.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="drawable">The drawable.</param>
        /// <param name="data">The data.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XCreateBitmapFromData(IntPtr display, IntPtr drawable, byte[] data, int width, int height);

        /// <summary>
        /// xes the create colormap.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="visual">The visual.</param>
        /// <param name="create">The create.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XCreateColormap(IntPtr display, IntPtr window, IntPtr visual, int create);

        /// <summary>
        /// xes the create font cursor.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="shape">The shape.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XCreateFontCursor(IntPtr display, CursorFontShape shape);

        // Drawing
        /// <summary>
        /// xes the create gc.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="valuemask">The valuemask.</param>
        /// <param name="values">The values.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XCreateGC(IntPtr display, IntPtr window, IntPtr valuemask, ref XGCValues values);

        /// <summary>
        /// xes the create gc.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="drawable">The drawable.</param>
        /// <param name="valuemask">The valuemask.</param>
        /// <param name="values">The values.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XCreateGC(IntPtr display, IntPtr drawable, ulong valuemask, IntPtr values);

        /// <summary>
        /// xes the create ic.
        /// </summary>
        /// <param name="xim">The xim.</param>
        /// <param name="name">The name.</param>
        /// <param name="im_style">The im style.</param>
        /// <param name="name2">The name2.</param>
        /// <param name="value2">The value2.</param>
        /// <param name="terminator">The terminator.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XCreateIC(IntPtr xim, string name, XIMProperties im_style, string name2, IntPtr value2, IntPtr terminator);

        /// <summary>
        /// xes the create ic.
        /// </summary>
        /// <param name="xim">The xim.</param>
        /// <param name="name">The name.</param>
        /// <param name="im_style">The im style.</param>
        /// <param name="name2">The name2.</param>
        /// <param name="value2">The value2.</param>
        /// <param name="name3">The name3.</param>
        /// <param name="value3">The value3.</param>
        /// <param name="terminator">The terminator.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XCreateIC(IntPtr xim, string name, XIMProperties im_style, string name2, IntPtr value2, string name3, IntPtr value3, IntPtr terminator);

        /// <summary>
        /// xes the create pixmap.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="d">The d.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XCreatePixmap(IntPtr display, IntPtr d, int width, int height, int depth);

        /// <summary>
        /// xes the create pixmap cursor.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="source">The source.</param>
        /// <param name="mask">The mask.</param>
        /// <param name="foreground_color">Color of the foreground.</param>
        /// <param name="background_color">Color of the background.</param>
        /// <param name="x_hot">The x hot.</param>
        /// <param name="y_hot">The y hot.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XCreatePixmapCursor(IntPtr display, IntPtr source, IntPtr mask,
            ref XColor foreground_color, ref XColor background_color, int x_hot, int y_hot);

        /// <summary>
        /// xes the create pixmap from bitmap data.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="drawable">The drawable.</param>
        /// <param name="data">The data.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="fg">The fg.</param>
        /// <param name="bg">The bg.</param>
        /// <param name="depth">The depth.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XCreatePixmapFromBitmapData(IntPtr display, IntPtr drawable, byte[] data, int width,
            int height, IntPtr fg, IntPtr bg, int depth);

        /// <summary>
        /// xes the create simple window.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="border_width">Width of the border.</param>
        /// <param name="border">The border.</param>
        /// <param name="background">The background.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XCreateSimpleWindow(IntPtr display, IntPtr parent, int x, int y, int width,
            int height, int border_width, IntPtr border, IntPtr background);

        /// <summary>
        /// xes the create window.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="border_width">Width of the border.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="xclass">The xclass.</param>
        /// <param name="visual">The visual.</param>
        /// <param name="valuemask">The valuemask.</param>
        /// <param name="attributes">The attributes.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XCreateWindow(IntPtr display, IntPtr parent, int x, int y, int width, int height,
            int border_width, int depth, int xclass, IntPtr visual, UIntPtr valuemask,
            ref XSetWindowAttributes attributes);

        /// <summary>
        /// xes the default colormap.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="screen_number">The screen number.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XDefaultColormap(IntPtr display, int screen_number);

        /// <summary>
        /// xes the default depth.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="screen_number">The screen number.</param>
        /// <returns>System.UInt32.</returns>
        [DllImport(LibX11)]
        public static extern uint XDefaultDepth(IntPtr display, int screen_number);

        /// <summary>
        /// xes the default root window.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XDefaultRootWindow(IntPtr display);

        /// <summary>
        /// xes the default screen.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XDefaultScreen(IntPtr display);

        // Colormaps
        /// <summary>
        /// xes the default screen of display.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XDefaultScreenOfDisplay(IntPtr display);

        /// <summary>
        /// xes the default visual.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="screen_number">The screen number.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XDefaultVisual(IntPtr display, int screen_number);

        /// <summary>
        /// xes the define cursor.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="cursor">The cursor.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XDefineCursor(IntPtr display, IntPtr window, IntPtr cursor);

        /// <summary>
        /// xes the delete property.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="property">The property.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XDeleteProperty(IntPtr display, IntPtr window, IntPtr property);

        /// <summary>
        /// xes the destroy ic.
        /// </summary>
        /// <param name="xic">The xic.</param>
        [DllImport(LibX11)]
        public static extern void XDestroyIC(IntPtr xic);

        /// <summary>
        /// xes the destroy image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XDestroyImage(ref XImage image);

        /// <summary>
        /// xes the destroy window.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XDestroyWindow(IntPtr display, IntPtr window);

        /// <summary>
        /// xes the draw line.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="drawable">The drawable.</param>
        /// <param name="gc">The gc.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XDrawLine(IntPtr display, IntPtr drawable, IntPtr gc, int x1, int y1, int x2, int y2);

        /// <summary>
        /// xes the draw rectangle.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="drawable">The drawable.</param>
        /// <param name="gc">The gc.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XDrawRectangle(IntPtr display, IntPtr drawable, IntPtr gc, int x1, int y1, int width,
            int height);

        /// <summary>
        /// xes the name of the fetch.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="window_name">Name of the window.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XFetchName(IntPtr display, IntPtr window, ref IntPtr window_name);

        /// <summary>
        /// xes the fill rectangle.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="drawable">The drawable.</param>
        /// <param name="gc">The gc.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XFillRectangle(IntPtr display, IntPtr drawable, IntPtr gc, int x1, int y1, int width,
            int height);

        /// <summary>
        /// xes the filter event.
        /// </summary>
        /// <param name="xevent">The xevent.</param>
        /// <param name="window">The window.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport(LibX11)]
        public static extern bool XFilterEvent(ref XEvent xevent, IntPtr window);

        /// <summary>
        /// xes the flush.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XFlush(IntPtr display);

        /// <summary>
        /// xes the free.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XFree(IntPtr data);

        /// <summary>
        /// xes the free cursor.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="cursor">The cursor.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XFreeCursor(IntPtr display, IntPtr cursor);

        /// <summary>
        /// xes the free event data.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="cookie">The cookie.</param>
        [DllImport(LibX11)]
        public static extern void XFreeEventData(IntPtr display, void* cookie);

        /// <summary>
        /// xes the free gc.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="gc">The gc.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XFreeGC(IntPtr display, IntPtr gc);

        /// <summary>
        /// xes the free pixmap.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="pixmap">The pixmap.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XFreePixmap(IntPtr display, IntPtr pixmap);

        /// <summary>
        /// xes the name of the get atom.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="atom">The atom.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XGetAtomName(IntPtr display, IntPtr atom);

        /// <summary>
        /// xes the get error text.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="code">The code.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="length">The length.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XGetErrorText(IntPtr display, byte code, StringBuilder buffer, int length);

        /// <summary>
        /// xes the get event data.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport(LibX11)]
        public static extern bool XGetEventData(IntPtr display, void* cookie);

        /// <summary>
        /// xes the get geometry.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="root">The root.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="border_width">Width of the border.</param>
        /// <param name="depth">The depth.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport(LibX11)]
        public static extern bool XGetGeometry(IntPtr display, IntPtr window, out IntPtr root, out int x, out int y,
            out int width, out int height, out int border_width, out int depth);

        /// <summary>
        /// xes the get geometry.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="root">The root.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="border_width">Width of the border.</param>
        /// <param name="depth">The depth.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport(LibX11)]
        public static extern bool XGetGeometry(IntPtr display, IntPtr window, IntPtr root, out int x, out int y,
            out int width, out int height, IntPtr border_width, IntPtr depth);

        /// <summary>
        /// xes the get geometry.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="root">The root.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="border_width">Width of the border.</param>
        /// <param name="depth">The depth.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport(LibX11)]
        public static extern bool XGetGeometry(IntPtr display, IntPtr window, IntPtr root, out int x, out int y,
            IntPtr width, IntPtr height, IntPtr border_width, IntPtr depth);

        /// <summary>
        /// xes the get geometry.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="root">The root.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="border_width">Width of the border.</param>
        /// <param name="depth">The depth.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport(LibX11)]
        public static extern bool XGetGeometry(IntPtr display, IntPtr window, IntPtr root, IntPtr x, IntPtr y,
            out int width, out int height, IntPtr border_width, IntPtr depth);

        /// <summary>
        /// xes the get geometry.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="geo">The geo.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool XGetGeometry(IntPtr display, IntPtr window, out XGeometry geo)
        {
            geo = new XGeometry();
            return XGetGeometry(display, window, out geo.root, out geo.x, out geo.y, out geo.width, out geo.height,
                out geo.bw, out geo.d);
        }

        /// <summary>
        /// xes the get icon sizes.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="size_list">The size list.</param>
        /// <param name="count">The count.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XGetIconSizes(IntPtr display, IntPtr window, out IntPtr size_list, out int count);

        /// <summary>
        /// xes the get selection owner.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="selection">The selection.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XGetSelectionOwner(IntPtr display, IntPtr selection);

        /// <summary>
        /// xes the get window attributes.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="attributes">The attributes.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XGetWindowAttributes(IntPtr display, IntPtr window, ref XWindowAttributes attributes);

        /// <summary>
        /// xes the get window property.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="atom">The atom.</param>
        /// <param name="long_offset">The long offset.</param>
        /// <param name="long_length">The long length.</param>
        /// <param name="delete">if set to <c>true</c> [delete].</param>
        /// <param name="req_type">Type of the req.</param>
        /// <param name="actual_type">The actual type.</param>
        /// <param name="actual_format">The actual format.</param>
        /// <param name="nitems">The nitems.</param>
        /// <param name="bytes_after">The bytes after.</param>
        /// <param name="prop">The property.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XGetWindowProperty(IntPtr display, IntPtr window, IntPtr atom, IntPtr long_offset,
            IntPtr long_length, bool delete, IntPtr req_type, out IntPtr actual_type, out int actual_format,
            out IntPtr nitems, out IntPtr bytes_after, out IntPtr prop);

        /// <summary>
        /// xes the get wm normal hints.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="hints">The hints.</param>
        /// <param name="supplied_return">The supplied return.</param>
        [DllImport(LibX11)]
        public static extern void XGetWMNormalHints(IntPtr display, IntPtr window, ref XSizeHints hints,
            out IntPtr supplied_return);

        /// <summary>
        /// xes the grab pointer.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="owner_events">if set to <c>true</c> [owner events].</param>
        /// <param name="event_mask">The event mask.</param>
        /// <param name="pointer_mode">The pointer mode.</param>
        /// <param name="keyboard_mode">The keyboard mode.</param>
        /// <param name="confine_to">The confine to.</param>
        /// <param name="cursor">The cursor.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XGrabPointer(IntPtr display, IntPtr window, bool owner_events, EventMask event_mask,
            GrabMode pointer_mode, GrabMode keyboard_mode, IntPtr confine_to, IntPtr cursor, IntPtr timestamp);

        /// <summary>
        /// xes the grab server.
        /// </summary>
        /// <param name="display">The display.</param>
        [DllImport(LibX11)]
        public static extern void XGrabServer(IntPtr display);

        /// <summary>
        /// xes the iconify window.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="screen_number">The screen number.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XIconifyWindow(IntPtr display, IntPtr window, int screen_number);

        /// <summary>
        /// Xis the free device information.
        /// </summary>
        /// <param name="info">The information.</param>
        [DllImport(LibXInput)]
        public static extern void XIFreeDeviceInfo(XIDeviceInfo* info);

        /// <summary>
        /// Xis the mask is set.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        /// <param name="shift">The shift.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool XIMaskIsSet(void* ptr, int shift) =>
            (((byte*)(ptr))[(shift) >> 3] & (1 << (shift & 7))) != 0;

        /// <summary>
        /// xes the initialize image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XInitImage(ref XImage image);

        /// <summary>
        /// xes the initialize threads.
        /// </summary>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XInitThreads();

        /// <summary>
        /// xes the intern atom.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="atom_name">Name of the atom.</param>
        /// <param name="only_if_exists">if set to <c>true</c> [only if exists].</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XInternAtom(IntPtr display, string atom_name, bool only_if_exists);

        /// <summary>
        /// xes the intern atoms.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="atom_names">The atom names.</param>
        /// <param name="atom_count">The atom count.</param>
        /// <param name="only_if_exists">if set to <c>true</c> [only if exists].</param>
        /// <param name="atoms">The atoms.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XInternAtoms(IntPtr display, string[] atom_names, int atom_count, bool only_if_exists,
            IntPtr[] atoms);

        /// <summary>
        /// Xis the query device.
        /// </summary>
        /// <param name="dpy">The dpy.</param>
        /// <param name="deviceid">The deviceid.</param>
        /// <param name="ndevices_return">The ndevices return.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibXInput)]
        public static extern IntPtr XIQueryDevice(IntPtr dpy, int deviceid, out int ndevices_return);

        /// <summary>
        /// Xis the query version.
        /// </summary>
        /// <param name="dpy">The dpy.</param>
        /// <param name="major">The major.</param>
        /// <param name="minor">The minor.</param>
        /// <returns>Status.</returns>
        [DllImport(LibXInput)]
        public static extern Status XIQueryVersion(IntPtr dpy, ref int major, ref int minor);

        /// <summary>
        /// Xis the select events.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="devices">The devices.</param>
        /// <returns>Status.</returns>
        public static Status XiSelectEvents(IntPtr display, IntPtr window, Dictionary<int, List<XiEventType>> devices)
        {
            var masks = stackalloc int[devices.Count];
            var emasks = stackalloc XIEventMask[devices.Count];
            int c = 0;
            foreach (var d in devices)
            {
                foreach (var ev in d.Value)
                    XISetMask(ref masks[c], ev);
                emasks[c] = new XIEventMask
                {
                    Mask = &masks[c],
                    Deviceid = d.Key,
                    MaskLen = XiEventMaskLen
                };
                c++;
            }

            return XISelectEvents(display, window, emasks, devices.Count);
        }

        /// <summary>
        /// Xis the select events.
        /// </summary>
        /// <param name="dpy">The dpy.</param>
        /// <param name="win">The win.</param>
        /// <param name="masks">The masks.</param>
        /// <param name="num_masks">The number masks.</param>
        /// <returns>Status.</returns>
        [DllImport(LibXInput)]
        public static extern Status XISelectEvents(
            IntPtr dpy,
            IntPtr win,
            XIEventMask* masks,
            int num_masks
        );

        /// <summary>
        /// Xis the set mask.
        /// </summary>
        /// <param name="mask">The mask.</param>
        /// <param name="ev">The ev.</param>
        public static void XISetMask(ref int mask, XiEventType ev)
        {
            mask |= (1 << (int)ev);
        }

        /// <summary>
        /// XKBs the set detectable automatic repeat.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="detectable">if set to <c>true</c> [detectable].</param>
        /// <param name="supported">The supported.</param>
        [DllImport(LibX11)]
        public static extern void XkbSetDetectableAutoRepeat(IntPtr display, bool detectable, IntPtr supported);

        /// <summary>
        /// xes the keycode to keysym.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="keycode">The keycode.</param>
        /// <param name="index">The index.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern unsafe IntPtr XKeycodeToKeysym(IntPtr display, int keycode, int index);

        /// <summary>
        /// xes the lock display.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XLockDisplay(IntPtr display);

        /// <summary>
        /// xes the color of the lookup.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="Colormap">The colormap.</param>
        /// <param name="Coloranem">The coloranem.</param>
        /// <param name="exact_def_color">Color of the exact definition.</param>
        /// <param name="screen_def_color">Color of the screen definition.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XLookupColor(IntPtr display, IntPtr Colormap, string Coloranem,
            ref XColor exact_def_color, ref XColor screen_def_color);

        /// <summary>
        /// xes the lookup string.
        /// </summary>
        /// <param name="xevent">The xevent.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="num_bytes">The number bytes.</param>
        /// <param name="keysym">The keysym.</param>
        /// <param name="status">The status.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern unsafe int XLookupString(ref XEvent xevent, void* buffer, int num_bytes, out IntPtr keysym, out IntPtr status);

        /// <summary>
        /// xes the lower window.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <returns>System.UInt32.</returns>
        [DllImport(LibX11)]
        public static extern uint XLowerWindow(IntPtr display, IntPtr window);

        /// <summary>
        /// xes the map subindows.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XMapSubindows(IntPtr display, IntPtr window);

        /// <summary>
        /// xes the map window.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XMapWindow(IntPtr display, IntPtr window);

        /// <summary>
        /// xes the match visual information.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="screen">The screen.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="klass">The klass.</param>
        /// <param name="info">The information.</param>
        [DllImport(LibX11)]
        public static extern void XMatchVisualInfo(IntPtr display, int screen, int depth, int klass, out XVisualInfo info);

        /// <summary>
        /// xes the move resize window.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XMoveResizeWindow(IntPtr display, IntPtr window, int x, int y, int width, int height);

        /// <summary>
        /// xes the next event.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="xevent">The xevent.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XNextEvent(IntPtr display, out XEvent xevent);

        /// <summary>
        /// xes the open display.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XOpenDisplay(IntPtr display);

        /// <summary>
        /// xes the open im.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="rdb">The RDB.</param>
        /// <param name="res_name">Name of the resource.</param>
        /// <param name="res_class">The resource class.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XOpenIM(IntPtr display, IntPtr rdb, IntPtr res_name, IntPtr res_class);

        /// <summary>
        /// xes the peek event.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="xevent">The xevent.</param>
        [DllImport(LibX11)]
        public static extern void XPeekEvent(IntPtr display, out XEvent xevent);

        /// <summary>
        /// xes the pending.
        /// </summary>
        /// <param name="diplay">The diplay.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XPending(IntPtr diplay);

        /// <summary>
        /// xes the put image.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="drawable">The drawable.</param>
        /// <param name="gc">The gc.</param>
        /// <param name="image">The image.</param>
        /// <param name="srcx">The SRCX.</param>
        /// <param name="srcy">The srcy.</param>
        /// <param name="destx">The destx.</param>
        /// <param name="desty">The desty.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XPutImage(IntPtr display, IntPtr drawable, IntPtr gc, ref XImage image,
            int srcx, int srcy, int destx, int desty, uint width, uint height);

        /// <summary>
        /// xes the query best cursor.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="drawable">The drawable.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="best_width">Width of the best.</param>
        /// <param name="best_height">Height of the best.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XQueryBestCursor(IntPtr display, IntPtr drawable, int width, int height,
            out int best_width, out int best_height);

        /// <summary>
        /// xes the query extension.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="name">The name.</param>
        /// <param name="majorOpcode">The major opcode.</param>
        /// <param name="firstEvent">The first event.</param>
        /// <param name="firstError">The first error.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport(LibX11)]
        public static extern bool XQueryExtension(IntPtr display, [MarshalAs(UnmanagedType.LPStr)] string name,
            out int majorOpcode, out int firstEvent, out int firstError);

        /// <summary>
        /// xes the query pointer.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="root">The root.</param>
        /// <param name="child">The child.</param>
        /// <param name="root_x">The root x.</param>
        /// <param name="root_y">The root y.</param>
        /// <param name="win_x">The win x.</param>
        /// <param name="win_y">The win y.</param>
        /// <param name="keys_buttons">The keys buttons.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport(LibX11)]
        public static extern bool XQueryPointer(IntPtr display, IntPtr window, out IntPtr root, out IntPtr child,
            out int root_x, out int root_y, out int win_x, out int win_y, out int keys_buttons);

        /// <summary>
        /// xes the query tree.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="root_return">The root return.</param>
        /// <param name="parent_return">The parent return.</param>
        /// <param name="children_return">The children return.</param>
        /// <param name="nchildren_return">The nchildren return.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XQueryTree(IntPtr display, IntPtr window, out IntPtr root_return,
            out IntPtr parent_return, out IntPtr children_return, out int nchildren_return);

        /// <summary>
        /// xes the raise window.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XRaiseWindow(IntPtr display, IntPtr window);

        /// <summary>
        /// xes the reparent window.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XReparentWindow(IntPtr display, IntPtr window, IntPtr parent, int x, int y);

        /// <summary>
        /// xes the resize window.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XResizeWindow(IntPtr display, IntPtr window, int width, int height);

        /// <summary>
        /// xes the root window.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="screen_number">The screen number.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XRootWindow(IntPtr display, int screen_number);

        /// <summary>
        /// XRRs the get monitors.
        /// </summary>
        /// <param name="dpy">The dpy.</param>
        /// <param name="window">The window.</param>
        /// <param name="get_active">if set to <c>true</c> [get active].</param>
        /// <param name="nmonitors">The nmonitors.</param>
        /// <returns>XRRMonitorInfo.</returns>
        [DllImport(LibX11Randr)]
        public static extern XRRMonitorInfo*
            XRRGetMonitors(IntPtr dpy, IntPtr window, bool get_active, out int nmonitors);

        /// <summary>
        /// XRRs the query extension.
        /// </summary>
        /// <param name="dpy">The dpy.</param>
        /// <param name="event_base_return">The event base return.</param>
        /// <param name="error_base_return">The error base return.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11Randr)]
        public static extern int XRRQueryExtension(IntPtr dpy,
            out int event_base_return,
            out int error_base_return);

        /// <summary>
        /// XRRs the query version.
        /// </summary>
        /// <param name="dpy">The dpy.</param>
        /// <param name="major_version_return">The major version return.</param>
        /// <param name="minor_version_return">The minor version return.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11Randr)]
        public static extern int XRRQueryVersion(IntPtr dpy,
            out int major_version_return,
            out int minor_version_return);

        /// <summary>
        /// XRRs the select input.
        /// </summary>
        /// <param name="dpy">The dpy.</param>
        /// <param name="window">The window.</param>
        /// <param name="mask">The mask.</param>
        [DllImport(LibX11Randr)]
        public static extern void XRRSelectInput(IntPtr dpy, IntPtr window, RandrEventMask mask);

        /// <summary>
        /// xes the screen number of screen.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="Screen">The screen.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XScreenNumberOfScreen(IntPtr display, IntPtr Screen);

        /// <summary>
        /// xes the select input.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="mask">The mask.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XSelectInput(IntPtr display, IntPtr window, IntPtr mask);

        /// <summary>
        /// xes the send event.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="propagate">if set to <c>true</c> [propagate].</param>
        /// <param name="event_mask">The event mask.</param>
        /// <param name="send_event">The send event.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XSendEvent(IntPtr display, IntPtr window, bool propagate, IntPtr event_mask,
            ref XEvent send_event);

        /// <summary>
        /// xes the set background.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="gc">The gc.</param>
        /// <param name="background">The background.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XSetBackground(IntPtr display, IntPtr gc, UIntPtr background);

        /// <summary>
        /// xes the set error handler.
        /// </summary>
        /// <param name="error_handler">The error handler.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XSetErrorHandler(XErrorHandler error_handler);

        /// <summary>
        /// xes the set foreground.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="gc">The gc.</param>
        /// <param name="foreground">The foreground.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XSetForeground(IntPtr display, IntPtr gc, UIntPtr foreground);

        /// <summary>
        /// xes the set function.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="gc">The gc.</param>
        /// <param name="function">The function.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XSetFunction(IntPtr display, IntPtr gc, GXFunction function);

        /// <summary>
        /// xes the set input focus.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="revert_to">The revert to.</param>
        /// <param name="time">The time.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XSetInputFocus(IntPtr display, IntPtr window, RevertTo revert_to, IntPtr time);

        /// <summary>
        /// xes the set locale modifiers.
        /// </summary>
        /// <param name="modifiers">The modifiers.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern unsafe IntPtr XSetLocaleModifiers(string modifiers);

        /// <summary>
        /// xes the set plane mask.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="gc">The gc.</param>
        /// <param name="mask">The mask.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XSetPlaneMask(IntPtr display, IntPtr gc, IntPtr mask);

        /// <summary>
        /// xes the set selection owner.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="selection">The selection.</param>
        /// <param name="owner">The owner.</param>
        /// <param name="time">The time.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XSetSelectionOwner(IntPtr display, IntPtr selection, IntPtr owner, IntPtr time);

        /// <summary>
        /// xes the set transient for hint.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="parent">The parent.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XSetTransientForHint(IntPtr display, IntPtr window, IntPtr parent);

        /// <summary>
        /// xes the set window background.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="background">The background.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XSetWindowBackground(IntPtr display, IntPtr window, IntPtr background);

        /// <summary>
        /// xes the set wm hints.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="wmhints">The wmhints.</param>
        [DllImport(LibX11)]
        public static extern void XSetWMHints(IntPtr display, IntPtr window, ref XWMHints wmhints);

        /// <summary>
        /// xes the name of the set wm.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="text_prop">The text property.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XSetWMName(IntPtr display, IntPtr window, ref XTextProperty text_prop);

        /// <summary>
        /// xes the set wm normal hints.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="hints">The hints.</param>
        [DllImport(LibX11)]
        public static extern void XSetWMNormalHints(IntPtr display, IntPtr window, ref XSizeHints hints);

        /// <summary>
        /// xes the set wm protocols.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="protocols">The protocols.</param>
        /// <param name="count">The count.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XSetWMProtocols(IntPtr display, IntPtr window, IntPtr[] protocols, int count);

        /// <summary>
        /// xes the set zoom hints.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="hints">The hints.</param>
        [DllImport(LibX11)]
        public static extern void XSetZoomHints(IntPtr display, IntPtr window, ref XSizeHints hints);

        /// <summary>
        /// xes the name of the store.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <param name="window_name">Name of the window.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XStoreName(IntPtr display, IntPtr window, string window_name);

        /// <summary>
        /// xes the synchronize.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="discard">if set to <c>true</c> [discard].</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XSync(IntPtr display, bool discard);

        /// <summary>
        /// xes the synchronize.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="onoff">if set to <c>true</c> [onoff].</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XSynchronize(IntPtr display, bool onoff);

        /// <summary>
        /// xes the translate coordinates.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="src_w">The source w.</param>
        /// <param name="dest_w">The dest w.</param>
        /// <param name="src_x">The source x.</param>
        /// <param name="src_y">The source y.</param>
        /// <param name="intdest_x_return">The intdest x return.</param>
        /// <param name="dest_y_return">The dest y return.</param>
        /// <param name="child_return">The child return.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport(LibX11)]
        public static extern bool XTranslateCoordinates(IntPtr display, IntPtr src_w, IntPtr dest_w, int src_x,
            int src_y, out int intdest_x_return, out int dest_y_return, out IntPtr child_return);

        /// <summary>
        /// xes the undefine cursor.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XUndefineCursor(IntPtr display, IntPtr window);

        /// <summary>
        /// xes the ungrab pointer.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XUngrabPointer(IntPtr display, IntPtr timestamp);

        /// <summary>
        /// xes the ungrab server.
        /// </summary>
        /// <param name="display">The display.</param>
        [DllImport(LibX11)]
        public static extern void XUngrabServer(IntPtr display);

        /// <summary>
        /// xes the unlock display.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XUnlockDisplay(IntPtr display);

        /// <summary>
        /// xes the unmap subwindows.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XUnmapSubwindows(IntPtr display, IntPtr window);

        /// <summary>
        /// xes the unmap window.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="window">The window.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern int XUnmapWindow(IntPtr display, IntPtr window);

        /// <summary>
        /// Xutf8s the lookup string.
        /// </summary>
        /// <param name="xic">The xic.</param>
        /// <param name="xevent">The xevent.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="num_bytes">The number bytes.</param>
        /// <param name="keysym">The keysym.</param>
        /// <param name="status">The status.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        public static extern unsafe int Xutf8LookupString(IntPtr xic, ref XEvent xevent, void* buffer, int num_bytes, out IntPtr keysym, out IntPtr status);

        /// <summary>
        /// xes the warp pointer.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="src_w">The source w.</param>
        /// <param name="dest_w">The dest w.</param>
        /// <param name="src_x">The source x.</param>
        /// <param name="src_y">The source y.</param>
        /// <param name="src_width">Width of the source.</param>
        /// <param name="src_height">Height of the source.</param>
        /// <param name="dest_x">The dest x.</param>
        /// <param name="dest_y">The dest y.</param>
        /// <returns>System.UInt32.</returns>
        [DllImport(LibX11)]
        public static extern uint XWarpPointer(IntPtr display, IntPtr src_w, IntPtr dest_w, int src_x, int src_y,
            uint src_width, uint src_height, int dest_x, int dest_y);

        /// <summary>
        /// xes the white pixel.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="screen_no">The screen no.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibX11)]
        public static extern IntPtr XWhitePixel(IntPtr display, int screen_no);

        /// <summary>
        /// xes the set line attributes.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="gc">The gc.</param>
        /// <param name="line_width">Width of the line.</param>
        /// <param name="line_style">The line style.</param>
        /// <param name="cap_style">The cap style.</param>
        /// <param name="join_style">The join style.</param>
        /// <returns>System.Int32.</returns>
        [DllImport(LibX11)]
        internal static extern int XSetLineAttributes(IntPtr display, IntPtr gc, int line_width, GCLineStyle line_style,
            GCCapStyle cap_style, GCJoinStyle join_style);

        #endregion Methods

        #region Structs

        /// <summary>
        /// Struct XGeometry
        /// </summary>
        public struct XGeometry
        {
            #region Fields

            /// <summary>
            /// The bw
            /// </summary>
            public int bw;

            /// <summary>
            /// The d
            /// </summary>
            public int d;

            /// <summary>
            /// The height
            /// </summary>
            public int height;

            /// <summary>
            /// The root
            /// </summary>
            public IntPtr root;

            /// <summary>
            /// The width
            /// </summary>
            public int width;

            /// <summary>
            /// The x
            /// </summary>
            public int x;

            /// <summary>
            /// The y
            /// </summary>
            public int y;

            #endregion Fields
        }

        #endregion Structs
    }
}
