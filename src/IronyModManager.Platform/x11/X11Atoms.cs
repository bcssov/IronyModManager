// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Novell
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="X11Atoms.cs" company="Novell">
//     Novell
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ************************************************************************

// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2006 Novell, Inc. (http://www.novell.com)
//
//
using System;
using System.Linq;
using IronyModManager.Shared;
using static IronyModManager.Platform.x11.XLib;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable IdentifierTypo
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable CommentTypo
// ReSharper disable ArrangeThisQualifier
// ReSharper disable NotAccessedField.Global
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo
#pragma warning disable 649

namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Class X11Atoms.
    /// </summary>
    [ExcludeFromCoverage("External component.")]
    internal class X11Atoms
    {
        #region Fields

        /// <summary>
        /// The motif wm hints
        /// </summary>
        public readonly IntPtr _MOTIF_WM_HINTS;

        /// <summary>
        /// The net active window
        /// </summary>
        public readonly IntPtr _NET_ACTIVE_WINDOW;

        /// <summary>
        /// The net client list
        /// </summary>
        public readonly IntPtr _NET_CLIENT_LIST;

        /// <summary>
        /// The net close window
        /// </summary>
        public readonly IntPtr _NET_CLOSE_WINDOW;

        /// <summary>
        /// The net current desktop
        /// </summary>
        public readonly IntPtr _NET_CURRENT_DESKTOP;

        /// <summary>
        /// The net desktop geometry
        /// </summary>
        public readonly IntPtr _NET_DESKTOP_GEOMETRY;

        /// <summary>
        /// The net desktop layout
        /// </summary>
        public readonly IntPtr _NET_DESKTOP_LAYOUT;

        /// <summary>
        /// The net desktop names
        /// </summary>
        public readonly IntPtr _NET_DESKTOP_NAMES;

        /// <summary>
        /// The net desktop viewport
        /// </summary>
        public readonly IntPtr _NET_DESKTOP_VIEWPORT;

        /// <summary>
        /// The net frame extents
        /// </summary>
        public readonly IntPtr _NET_FRAME_EXTENTS;

        /// <summary>
        /// The net moveresize window
        /// </summary>
        public readonly IntPtr _NET_MOVERESIZE_WINDOW;

        /// <summary>
        /// The net number of desktops
        /// </summary>
        public readonly IntPtr _NET_NUMBER_OF_DESKTOPS;

        /// <summary>
        /// The net request frame extents
        /// </summary>
        public readonly IntPtr _NET_REQUEST_FRAME_EXTENTS;

        /// <summary>
        /// The net restack window
        /// </summary>
        public readonly IntPtr _NET_RESTACK_WINDOW;

        /// <summary>
        /// The net showing desktop
        /// </summary>
        public readonly IntPtr _NET_SHOWING_DESKTOP;

        /// <summary>
        /// The net supported
        /// </summary>
        public readonly IntPtr _NET_SUPPORTED;

        /// <summary>
        /// The net supporting wm check
        /// </summary>
        public readonly IntPtr _NET_SUPPORTING_WM_CHECK;

        /// <summary>
        /// The net system tray opcode
        /// </summary>
        public readonly IntPtr _NET_SYSTEM_TRAY_OPCODE;

        /// <summary>
        /// The net system tray orientation
        /// </summary>
        public readonly IntPtr _NET_SYSTEM_TRAY_ORIENTATION;

        /// <summary>
        /// The net system tray s
        /// </summary>
        public readonly IntPtr _NET_SYSTEM_TRAY_S;

        /// <summary>
        /// The net virtual roots
        /// </summary>
        public readonly IntPtr _NET_VIRTUAL_ROOTS;

        /// <summary>
        /// The net wm allowed actions
        /// </summary>
        public readonly IntPtr _NET_WM_ALLOWED_ACTIONS;

        /// <summary>
        /// The net wm context help
        /// </summary>
        public readonly IntPtr _NET_WM_CONTEXT_HELP;

        /// <summary>
        /// The net wm desktop
        /// </summary>
        public readonly IntPtr _NET_WM_DESKTOP;

        /// <summary>
        /// The net wm handled icons
        /// </summary>
        public readonly IntPtr _NET_WM_HANDLED_ICONS;

        /// <summary>
        /// The net wm icon
        /// </summary>
        public readonly IntPtr _NET_WM_ICON;

        /// <summary>
        /// The net wm icon geometry
        /// </summary>
        public readonly IntPtr _NET_WM_ICON_GEOMETRY;

        /// <summary>
        /// The net wm icon name
        /// </summary>
        public readonly IntPtr _NET_WM_ICON_NAME;

        /// <summary>
        /// The net wm moveresize
        /// </summary>
        public readonly IntPtr _NET_WM_MOVERESIZE;

        /// <summary>
        /// The net wm name
        /// </summary>
        public readonly IntPtr _NET_WM_NAME;

        /// <summary>
        /// The net wm pid
        /// </summary>
        public readonly IntPtr _NET_WM_PID;

        /// <summary>
        /// The net wm ping
        /// </summary>
        public readonly IntPtr _NET_WM_PING;

        /// <summary>
        /// The net wm state
        /// </summary>
        public readonly IntPtr _NET_WM_STATE;

        /// <summary>
        /// The net wm state above
        /// </summary>
        public readonly IntPtr _NET_WM_STATE_ABOVE;

        /// <summary>
        /// The net wm state hidden
        /// </summary>
        public readonly IntPtr _NET_WM_STATE_HIDDEN;

        /// <summary>
        /// The net wm state maximized horz
        /// </summary>
        public readonly IntPtr _NET_WM_STATE_MAXIMIZED_HORZ;

        /// <summary>
        /// The net wm state maximized vert
        /// </summary>
        public readonly IntPtr _NET_WM_STATE_MAXIMIZED_VERT;

        /// <summary>
        /// The net wm state modal
        /// </summary>
        public readonly IntPtr _NET_WM_STATE_MODAL;

        /// <summary>
        /// The net wm state skip taskbar
        /// </summary>
        public readonly IntPtr _NET_WM_STATE_SKIP_TASKBAR;

        /// <summary>
        /// The net wm strut
        /// </summary>
        public readonly IntPtr _NET_WM_STRUT;

        /// <summary>
        /// The net wm strut partial
        /// </summary>
        public readonly IntPtr _NET_WM_STRUT_PARTIAL;

        /// <summary>
        /// The net wm synchronize request
        /// </summary>
        public readonly IntPtr _NET_WM_SYNC_REQUEST;

        /// <summary>
        /// The net wm user time
        /// </summary>
        public readonly IntPtr _NET_WM_USER_TIME;

        /// <summary>
        /// The net wm visible icon name
        /// </summary>
        public readonly IntPtr _NET_WM_VISIBLE_ICON_NAME;

        /// <summary>
        /// The net wm visible name
        /// </summary>
        public readonly IntPtr _NET_WM_VISIBLE_NAME;

        /// <summary>
        /// The net wm window opacity
        /// </summary>
        public readonly IntPtr _NET_WM_WINDOW_OPACITY;

        /// <summary>
        /// The net wm window type
        /// </summary>
        public readonly IntPtr _NET_WM_WINDOW_TYPE;

        /// <summary>
        /// The net wm window type desktop
        /// </summary>
        public readonly IntPtr _NET_WM_WINDOW_TYPE_DESKTOP;

        /// <summary>
        /// The net wm window type dialog
        /// </summary>
        public readonly IntPtr _NET_WM_WINDOW_TYPE_DIALOG;

        /// <summary>
        /// The net wm window type dock
        /// </summary>
        public readonly IntPtr _NET_WM_WINDOW_TYPE_DOCK;

        /// <summary>
        /// The net wm window type menu
        /// </summary>
        public readonly IntPtr _NET_WM_WINDOW_TYPE_MENU;

        /// <summary>
        /// The net wm window type normal
        /// </summary>
        public readonly IntPtr _NET_WM_WINDOW_TYPE_NORMAL;

        /// <summary>
        /// The net wm window type splash
        /// </summary>
        public readonly IntPtr _NET_WM_WINDOW_TYPE_SPLASH;

        /// <summary>
        /// The net wm window type toolbar
        /// </summary>
        public readonly IntPtr _NET_WM_WINDOW_TYPE_TOOLBAR;

        /// <summary>
        /// The net wm window type utility
        /// </summary>
        public readonly IntPtr _NET_WM_WINDOW_TYPE_UTILITY;

        /// <summary>
        /// The net workarea
        /// </summary>
        public readonly IntPtr _NET_WORKAREA;

        /// <summary>
        /// The xembed
        /// </summary>
        public readonly IntPtr _XEMBED;

        /// <summary>
        /// The xembed information
        /// </summary>
        public readonly IntPtr _XEMBED_INFO;

        // Our atoms
        /// <summary>
        /// Any property type
        /// </summary>
        public readonly IntPtr AnyPropertyType = (IntPtr)0;

        /// <summary>
        /// The atom pair
        /// </summary>
        public readonly IntPtr ATOM_PAIR;

        /// <summary>
        /// The clipboard
        /// </summary>
        public readonly IntPtr CLIPBOARD;

        /// <summary>
        /// The clipboard manager
        /// </summary>
        public readonly IntPtr CLIPBOARD_MANAGER;

        /// <summary>
        /// The multiple
        /// </summary>
        public readonly IntPtr MULTIPLE;

        /// <summary>
        /// The oemtext
        /// </summary>
        public readonly IntPtr OEMTEXT;

        /// <summary>
        /// The primary
        /// </summary>
        public readonly IntPtr PRIMARY;

        /// <summary>
        /// The save targets
        /// </summary>
        public readonly IntPtr SAVE_TARGETS;

        /// <summary>
        /// The targets
        /// </summary>
        public readonly IntPtr TARGETS;

        /// <summary>
        /// The unicodetext
        /// </summary>
        public readonly IntPtr UNICODETEXT;

        /// <summary>
        /// The ut F16 string
        /// </summary>
        public readonly IntPtr UTF16_STRING;

        /// <summary>
        /// The ut f8 string
        /// </summary>
        public readonly IntPtr UTF8_STRING;

        /// <summary>
        /// The wm delete window
        /// </summary>
        public readonly IntPtr WM_DELETE_WINDOW;

        /// <summary>
        /// The wm protocols
        /// </summary>
        public readonly IntPtr WM_PROTOCOLS;

        /// <summary>
        /// The wm take focus
        /// </summary>
        public readonly IntPtr WM_TAKE_FOCUS;

        /// <summary>
        /// The xa arc
        /// </summary>
        public readonly IntPtr XA_ARC = (IntPtr)3;

        /// <summary>
        /// The xa atom
        /// </summary>
        public readonly IntPtr XA_ATOM = (IntPtr)4;

        /// <summary>
        /// The xa bitmap
        /// </summary>
        public readonly IntPtr XA_BITMAP = (IntPtr)5;

        /// <summary>
        /// The xa cap height
        /// </summary>
        public readonly IntPtr XA_CAP_HEIGHT = (IntPtr)66;

        /// <summary>
        /// The xa cardinal
        /// </summary>
        public readonly IntPtr XA_CARDINAL = (IntPtr)6;

        /// <summary>
        /// The xa colormap
        /// </summary>
        public readonly IntPtr XA_COLORMAP = (IntPtr)7;

        /// <summary>
        /// The xa copyright
        /// </summary>
        public readonly IntPtr XA_COPYRIGHT = (IntPtr)61;

        /// <summary>
        /// The xa cursor
        /// </summary>
        public readonly IntPtr XA_CURSOR = (IntPtr)8;

        /// <summary>
        /// The xa cut buffe r0
        /// </summary>
        public readonly IntPtr XA_CUT_BUFFER0 = (IntPtr)9;

        /// <summary>
        /// The xa cut buffe r1
        /// </summary>
        public readonly IntPtr XA_CUT_BUFFER1 = (IntPtr)10;

        /// <summary>
        /// The xa cut buffe r2
        /// </summary>
        public readonly IntPtr XA_CUT_BUFFER2 = (IntPtr)11;

        /// <summary>
        /// The xa cut buffe r3
        /// </summary>
        public readonly IntPtr XA_CUT_BUFFER3 = (IntPtr)12;

        /// <summary>
        /// The xa cut buffe r4
        /// </summary>
        public readonly IntPtr XA_CUT_BUFFER4 = (IntPtr)13;

        /// <summary>
        /// The xa cut buffe r5
        /// </summary>
        public readonly IntPtr XA_CUT_BUFFER5 = (IntPtr)14;

        /// <summary>
        /// The xa cut buffe r6
        /// </summary>
        public readonly IntPtr XA_CUT_BUFFER6 = (IntPtr)15;

        /// <summary>
        /// The xa cut buffe r7
        /// </summary>
        public readonly IntPtr XA_CUT_BUFFER7 = (IntPtr)16;

        /// <summary>
        /// The xa drawable
        /// </summary>
        public readonly IntPtr XA_DRAWABLE = (IntPtr)17;

        /// <summary>
        /// The xa end space
        /// </summary>
        public readonly IntPtr XA_END_SPACE = (IntPtr)46;

        /// <summary>
        /// The xa family name
        /// </summary>
        public readonly IntPtr XA_FAMILY_NAME = (IntPtr)64;

        /// <summary>
        /// The xa font
        /// </summary>
        public readonly IntPtr XA_FONT = (IntPtr)18;

        /// <summary>
        /// The xa font name
        /// </summary>
        public readonly IntPtr XA_FONT_NAME = (IntPtr)63;

        /// <summary>
        /// The xa full name
        /// </summary>
        public readonly IntPtr XA_FULL_NAME = (IntPtr)65;

        /// <summary>
        /// The xa integer
        /// </summary>
        public readonly IntPtr XA_INTEGER = (IntPtr)19;

        /// <summary>
        /// The xa italic angle
        /// </summary>
        public readonly IntPtr XA_ITALIC_ANGLE = (IntPtr)55;

        /// <summary>
        /// The xa maximum space
        /// </summary>
        public readonly IntPtr XA_MAX_SPACE = (IntPtr)45;

        /// <summary>
        /// The xa minimum space
        /// </summary>
        public readonly IntPtr XA_MIN_SPACE = (IntPtr)43;

        /// <summary>
        /// The xa norm space
        /// </summary>
        public readonly IntPtr XA_NORM_SPACE = (IntPtr)44;

        /// <summary>
        /// The xa notice
        /// </summary>
        public readonly IntPtr XA_NOTICE = (IntPtr)62;

        /// <summary>
        /// The xa pixmap
        /// </summary>
        public readonly IntPtr XA_PIXMAP = (IntPtr)20;

        /// <summary>
        /// The xa point
        /// </summary>
        public readonly IntPtr XA_POINT = (IntPtr)21;

        /// <summary>
        /// The xa point size
        /// </summary>
        public readonly IntPtr XA_POINT_SIZE = (IntPtr)59;

        /// <summary>
        /// The xa primary
        /// </summary>
        public readonly IntPtr XA_PRIMARY = (IntPtr)1;

        /// <summary>
        /// The xa quad width
        /// </summary>
        public readonly IntPtr XA_QUAD_WIDTH = (IntPtr)57;

        /// <summary>
        /// The xa rectangle
        /// </summary>
        public readonly IntPtr XA_RECTANGLE = (IntPtr)22;

        /// <summary>
        /// The xa resolution
        /// </summary>
        public readonly IntPtr XA_RESOLUTION = (IntPtr)60;

        /// <summary>
        /// The xa resource manager
        /// </summary>
        public readonly IntPtr XA_RESOURCE_MANAGER = (IntPtr)23;

        /// <summary>
        /// The xa RGB best map
        /// </summary>
        public readonly IntPtr XA_RGB_BEST_MAP = (IntPtr)25;

        /// <summary>
        /// The xa RGB blue map
        /// </summary>
        public readonly IntPtr XA_RGB_BLUE_MAP = (IntPtr)26;

        /// <summary>
        /// The xa RGB color map
        /// </summary>
        public readonly IntPtr XA_RGB_COLOR_MAP = (IntPtr)24;

        /// <summary>
        /// The xa RGB default map
        /// </summary>
        public readonly IntPtr XA_RGB_DEFAULT_MAP = (IntPtr)27;

        /// <summary>
        /// The xa RGB gray map
        /// </summary>
        public readonly IntPtr XA_RGB_GRAY_MAP = (IntPtr)28;

        /// <summary>
        /// The xa RGB green map
        /// </summary>
        public readonly IntPtr XA_RGB_GREEN_MAP = (IntPtr)29;

        /// <summary>
        /// The xa RGB red map
        /// </summary>
        public readonly IntPtr XA_RGB_RED_MAP = (IntPtr)30;

        /// <summary>
        /// The xa secondary
        /// </summary>
        public readonly IntPtr XA_SECONDARY = (IntPtr)2;

        /// <summary>
        /// The xa strikeout ascent
        /// </summary>
        public readonly IntPtr XA_STRIKEOUT_ASCENT = (IntPtr)53;

        /// <summary>
        /// The xa strikeout descent
        /// </summary>
        public readonly IntPtr XA_STRIKEOUT_DESCENT = (IntPtr)54;

        /// <summary>
        /// The xa string
        /// </summary>
        public readonly IntPtr XA_STRING = (IntPtr)31;

        /// <summary>
        /// The xa subscript x
        /// </summary>
        public readonly IntPtr XA_SUBSCRIPT_X = (IntPtr)49;

        /// <summary>
        /// The xa subscript y
        /// </summary>
        public readonly IntPtr XA_SUBSCRIPT_Y = (IntPtr)50;

        /// <summary>
        /// The xa superscript x
        /// </summary>
        public readonly IntPtr XA_SUPERSCRIPT_X = (IntPtr)47;

        /// <summary>
        /// The xa superscript y
        /// </summary>
        public readonly IntPtr XA_SUPERSCRIPT_Y = (IntPtr)48;

        /// <summary>
        /// The xa underline position
        /// </summary>
        public readonly IntPtr XA_UNDERLINE_POSITION = (IntPtr)51;

        /// <summary>
        /// The xa underline thickness
        /// </summary>
        public readonly IntPtr XA_UNDERLINE_THICKNESS = (IntPtr)52;

        /// <summary>
        /// The xa visualid
        /// </summary>
        public readonly IntPtr XA_VISUALID = (IntPtr)32;

        /// <summary>
        /// The xa weight
        /// </summary>
        public readonly IntPtr XA_WEIGHT = (IntPtr)58;

        /// <summary>
        /// The xa window
        /// </summary>
        public readonly IntPtr XA_WINDOW = (IntPtr)33;

        /// <summary>
        /// The xa wm class
        /// </summary>
        public readonly IntPtr XA_WM_CLASS = (IntPtr)67;

        /// <summary>
        /// The xa wm client machine
        /// </summary>
        public readonly IntPtr XA_WM_CLIENT_MACHINE = (IntPtr)36;

        /// <summary>
        /// The xa wm command
        /// </summary>
        public readonly IntPtr XA_WM_COMMAND = (IntPtr)34;

        /// <summary>
        /// The xa wm hints
        /// </summary>
        public readonly IntPtr XA_WM_HINTS = (IntPtr)35;

        /// <summary>
        /// The xa wm icon name
        /// </summary>
        public readonly IntPtr XA_WM_ICON_NAME = (IntPtr)37;

        /// <summary>
        /// The xa wm icon size
        /// </summary>
        public readonly IntPtr XA_WM_ICON_SIZE = (IntPtr)38;

        /// <summary>
        /// The xa wm name
        /// </summary>
        public readonly IntPtr XA_WM_NAME = (IntPtr)39;

        /// <summary>
        /// The xa wm normal hints
        /// </summary>
        public readonly IntPtr XA_WM_NORMAL_HINTS = (IntPtr)40;

        /// <summary>
        /// The xa wm size hints
        /// </summary>
        public readonly IntPtr XA_WM_SIZE_HINTS = (IntPtr)41;

        /// <summary>
        /// The xa wm transient for
        /// </summary>
        public readonly IntPtr XA_WM_TRANSIENT_FOR = (IntPtr)68;

        /// <summary>
        /// The xa wm zoom hints
        /// </summary>
        public readonly IntPtr XA_WM_ZOOM_HINTS = (IntPtr)42;

        /// <summary>
        /// The xa x height
        /// </summary>
        public readonly IntPtr XA_X_HEIGHT = (IntPtr)56;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="X11Atoms" /> class.
        /// </summary>
        /// <param name="display">The display.</param>
        public X11Atoms(IntPtr display)
        {
            // make sure this array stays in sync with the statements below

            var fields = typeof(X11Atoms).GetFields()
                .Where(f => f.FieldType == typeof(IntPtr) && (IntPtr)f.GetValue(this) == IntPtr.Zero).ToArray();
            var atomNames = fields.Select(f => f.Name).ToArray();

            IntPtr[] atoms = new IntPtr[atomNames.Length];
            ;

            XInternAtoms(display, atomNames, atomNames.Length, true, atoms);

            for (var c = 0; c < fields.Length; c++)
                fields[c].SetValue(this, atoms[c]);
        }

        #endregion Constructors
    }
}
