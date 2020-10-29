// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Peter Bartok
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="X11Structs.cs" company="Peter Bartok">
//     Peter Bartok
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ************************************************************************

// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software",, to deal in the Software without restriction, including
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
// Copyright (c) 2004 Novell, Inc.
//
// Authors:
//	Peter Bartok	pbartok@novell.com
//

// NOT COMPLETE

using System;
using System.Reflection;
using System.Runtime.InteropServices;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable IdentifierTypo
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable CommentTypo
// ReSharper disable ArrangeThisQualifier
// ReSharper disable NotAccessedField.Global
#pragma warning disable 649

namespace IronyModManager.Platform.x11
{
    //
    // In the structures below, fields of type long are mapped to IntPtr.
    // This will work on all platforms where sizeof(long)==sizeof(void*), which
    // is almost all platforms except WIN64.
    //

    /// <summary>
    /// Delegate XErrorHandler
    /// </summary>
    /// <param name="DisplayHandle">The display handle.</param>
    /// <param name="error_event">The error event.</param>
    /// <returns>System.Int32.</returns>
    internal delegate int XErrorHandler(IntPtr DisplayHandle, ref XErrorEvent error_event);

    // only PreeditStartCallback requires return value though.
    /// <summary>
    /// Delegate XIMProc
    /// </summary>
    /// <param name="xim">The xim.</param>
    /// <param name="clientData">The client data.</param>
    /// <param name="callData">The call data.</param>
    /// <returns>System.Int32.</returns>
    internal delegate int XIMProc(IntPtr xim, IntPtr clientData, IntPtr callData);

    /// <summary>
    /// Enum Atom
    /// </summary>
    internal enum Atom
    {
        /// <summary>
        /// Any property type
        /// </summary>
        AnyPropertyType = 0,

        /// <summary>
        /// The xa primary
        /// </summary>
        XA_PRIMARY = 1,

        /// <summary>
        /// The xa secondary
        /// </summary>
        XA_SECONDARY = 2,

        /// <summary>
        /// The xa arc
        /// </summary>
        XA_ARC = 3,

        /// <summary>
        /// The xa atom
        /// </summary>
        XA_ATOM = 4,

        /// <summary>
        /// The xa bitmap
        /// </summary>
        XA_BITMAP = 5,

        /// <summary>
        /// The xa cardinal
        /// </summary>
        XA_CARDINAL = 6,

        /// <summary>
        /// The xa colormap
        /// </summary>
        XA_COLORMAP = 7,

        /// <summary>
        /// The xa cursor
        /// </summary>
        XA_CURSOR = 8,

        /// <summary>
        /// The xa cut buffe r0
        /// </summary>
        XA_CUT_BUFFER0 = 9,

        /// <summary>
        /// The xa cut buffe r1
        /// </summary>
        XA_CUT_BUFFER1 = 10,

        /// <summary>
        /// The xa cut buffe r2
        /// </summary>
        XA_CUT_BUFFER2 = 11,

        /// <summary>
        /// The xa cut buffe r3
        /// </summary>
        XA_CUT_BUFFER3 = 12,

        /// <summary>
        /// The xa cut buffe r4
        /// </summary>
        XA_CUT_BUFFER4 = 13,

        /// <summary>
        /// The xa cut buffe r5
        /// </summary>
        XA_CUT_BUFFER5 = 14,

        /// <summary>
        /// The xa cut buffe r6
        /// </summary>
        XA_CUT_BUFFER6 = 15,

        /// <summary>
        /// The xa cut buffe r7
        /// </summary>
        XA_CUT_BUFFER7 = 16,

        /// <summary>
        /// The xa drawable
        /// </summary>
        XA_DRAWABLE = 17,

        /// <summary>
        /// The xa font
        /// </summary>
        XA_FONT = 18,

        /// <summary>
        /// The xa integer
        /// </summary>
        XA_INTEGER = 19,

        /// <summary>
        /// The xa pixmap
        /// </summary>
        XA_PIXMAP = 20,

        /// <summary>
        /// The xa point
        /// </summary>
        XA_POINT = 21,

        /// <summary>
        /// The xa rectangle
        /// </summary>
        XA_RECTANGLE = 22,

        /// <summary>
        /// The xa resource manager
        /// </summary>
        XA_RESOURCE_MANAGER = 23,

        /// <summary>
        /// The xa RGB color map
        /// </summary>
        XA_RGB_COLOR_MAP = 24,

        /// <summary>
        /// The xa RGB best map
        /// </summary>
        XA_RGB_BEST_MAP = 25,

        /// <summary>
        /// The xa RGB blue map
        /// </summary>
        XA_RGB_BLUE_MAP = 26,

        /// <summary>
        /// The xa RGB default map
        /// </summary>
        XA_RGB_DEFAULT_MAP = 27,

        /// <summary>
        /// The xa RGB gray map
        /// </summary>
        XA_RGB_GRAY_MAP = 28,

        /// <summary>
        /// The xa RGB green map
        /// </summary>
        XA_RGB_GREEN_MAP = 29,

        /// <summary>
        /// The xa RGB red map
        /// </summary>
        XA_RGB_RED_MAP = 30,

        /// <summary>
        /// The xa string
        /// </summary>
        XA_STRING = 31,

        /// <summary>
        /// The xa visualid
        /// </summary>
        XA_VISUALID = 32,

        /// <summary>
        /// The xa window
        /// </summary>
        XA_WINDOW = 33,

        /// <summary>
        /// The xa wm command
        /// </summary>
        XA_WM_COMMAND = 34,

        /// <summary>
        /// The xa wm hints
        /// </summary>
        XA_WM_HINTS = 35,

        /// <summary>
        /// The xa wm client machine
        /// </summary>
        XA_WM_CLIENT_MACHINE = 36,

        /// <summary>
        /// The xa wm icon name
        /// </summary>
        XA_WM_ICON_NAME = 37,

        /// <summary>
        /// The xa wm icon size
        /// </summary>
        XA_WM_ICON_SIZE = 38,

        /// <summary>
        /// The xa wm name
        /// </summary>
        XA_WM_NAME = 39,

        /// <summary>
        /// The xa wm normal hints
        /// </summary>
        XA_WM_NORMAL_HINTS = 40,

        /// <summary>
        /// The xa wm size hints
        /// </summary>
        XA_WM_SIZE_HINTS = 41,

        /// <summary>
        /// The xa wm zoom hints
        /// </summary>
        XA_WM_ZOOM_HINTS = 42,

        /// <summary>
        /// The xa minimum space
        /// </summary>
        XA_MIN_SPACE = 43,

        /// <summary>
        /// The xa norm space
        /// </summary>
        XA_NORM_SPACE = 44,

        /// <summary>
        /// The xa maximum space
        /// </summary>
        XA_MAX_SPACE = 45,

        /// <summary>
        /// The xa end space
        /// </summary>
        XA_END_SPACE = 46,

        /// <summary>
        /// The xa superscript x
        /// </summary>
        XA_SUPERSCRIPT_X = 47,

        /// <summary>
        /// The xa superscript y
        /// </summary>
        XA_SUPERSCRIPT_Y = 48,

        /// <summary>
        /// The xa subscript x
        /// </summary>
        XA_SUBSCRIPT_X = 49,

        /// <summary>
        /// The xa subscript y
        /// </summary>
        XA_SUBSCRIPT_Y = 50,

        /// <summary>
        /// The xa underline position
        /// </summary>
        XA_UNDERLINE_POSITION = 51,

        /// <summary>
        /// The xa underline thickness
        /// </summary>
        XA_UNDERLINE_THICKNESS = 52,

        /// <summary>
        /// The xa strikeout ascent
        /// </summary>
        XA_STRIKEOUT_ASCENT = 53,

        /// <summary>
        /// The xa strikeout descent
        /// </summary>
        XA_STRIKEOUT_DESCENT = 54,

        /// <summary>
        /// The xa italic angle
        /// </summary>
        XA_ITALIC_ANGLE = 55,

        /// <summary>
        /// The xa x height
        /// </summary>
        XA_X_HEIGHT = 56,

        /// <summary>
        /// The xa quad width
        /// </summary>
        XA_QUAD_WIDTH = 57,

        /// <summary>
        /// The xa weight
        /// </summary>
        XA_WEIGHT = 58,

        /// <summary>
        /// The xa point size
        /// </summary>
        XA_POINT_SIZE = 59,

        /// <summary>
        /// The xa resolution
        /// </summary>
        XA_RESOLUTION = 60,

        /// <summary>
        /// The xa copyright
        /// </summary>
        XA_COPYRIGHT = 61,

        /// <summary>
        /// The xa notice
        /// </summary>
        XA_NOTICE = 62,

        /// <summary>
        /// The xa font name
        /// </summary>
        XA_FONT_NAME = 63,

        /// <summary>
        /// The xa family name
        /// </summary>
        XA_FAMILY_NAME = 64,

        /// <summary>
        /// The xa full name
        /// </summary>
        XA_FULL_NAME = 65,

        /// <summary>
        /// The xa cap height
        /// </summary>
        XA_CAP_HEIGHT = 66,

        /// <summary>
        /// The xa wm class
        /// </summary>
        XA_WM_CLASS = 67,

        /// <summary>
        /// The xa wm transient for
        /// </summary>
        XA_WM_TRANSIENT_FOR = 68,

        /// <summary>
        /// The xa last predefined
        /// </summary>
        XA_LAST_PREDEFINED = 68
    }

    /// <summary>
    /// Enum ChangeWindowFlags
    /// </summary>
    [Flags]
    internal enum ChangeWindowFlags
    {
        /// <summary>
        /// The CWX
        /// </summary>
        CWX = 1 << 0,

        /// <summary>
        /// The cwy
        /// </summary>
        CWY = 1 << 1,

        /// <summary>
        /// The cw width
        /// </summary>
        CWWidth = 1 << 2,

        /// <summary>
        /// The cw height
        /// </summary>
        CWHeight = 1 << 3,

        /// <summary>
        /// The cw border width
        /// </summary>
        CWBorderWidth = 1 << 4,

        /// <summary>
        /// The cw sibling
        /// </summary>
        CWSibling = 1 << 5,

        /// <summary>
        /// The cw stack mode
        /// </summary>
        CWStackMode = 1 << 6
    }

    /// <summary>
    /// Enum ColorFlags
    /// </summary>
    [Flags]
    internal enum ColorFlags
    {
        /// <summary>
        /// The do red
        /// </summary>
        DoRed = 1 << 0,

        /// <summary>
        /// The do green
        /// </summary>
        DoGreen = 1 << 1,

        /// <summary>
        /// The do blue
        /// </summary>
        DoBlue = 1 << 2
    }

    /// <summary>
    /// Enum CreateWindowArgs
    /// </summary>
    internal enum CreateWindowArgs
    {
        /// <summary>
        /// The copy from parent
        /// </summary>
        CopyFromParent = 0,

        /// <summary>
        /// The parent relative
        /// </summary>
        ParentRelative = 1,

        /// <summary>
        /// The input output
        /// </summary>
        InputOutput = 1,

        /// <summary>
        /// The input only
        /// </summary>
        InputOnly = 2
    }

    /// <summary>
    /// Enum CursorFontShape
    /// </summary>
    internal enum CursorFontShape
    {
        /// <summary>
        /// The xc x cursor
        /// </summary>
        XC_X_cursor = 0,

        /// <summary>
        /// The xc arrow
        /// </summary>
        XC_arrow = 2,

        /// <summary>
        /// The xc based arrow down
        /// </summary>
        XC_based_arrow_down = 4,

        /// <summary>
        /// The xc based arrow up
        /// </summary>
        XC_based_arrow_up = 6,

        /// <summary>
        /// The xc boat
        /// </summary>
        XC_boat = 8,

        /// <summary>
        /// The xc bogosity
        /// </summary>
        XC_bogosity = 10,

        /// <summary>
        /// The xc bottom left corner
        /// </summary>
        XC_bottom_left_corner = 12,

        /// <summary>
        /// The xc bottom right corner
        /// </summary>
        XC_bottom_right_corner = 14,

        /// <summary>
        /// The xc bottom side
        /// </summary>
        XC_bottom_side = 16,

        /// <summary>
        /// The xc bottom tee
        /// </summary>
        XC_bottom_tee = 18,

        /// <summary>
        /// The xc box spiral
        /// </summary>
        XC_box_spiral = 20,

        /// <summary>
        /// The xc center PTR
        /// </summary>
        XC_center_ptr = 22,

        /// <summary>
        /// The xc circle
        /// </summary>
        XC_circle = 24,

        /// <summary>
        /// The xc clock
        /// </summary>
        XC_clock = 26,

        /// <summary>
        /// The xc coffee mug
        /// </summary>
        XC_coffee_mug = 28,

        /// <summary>
        /// The xc cross
        /// </summary>
        XC_cross = 30,

        /// <summary>
        /// The xc cross reverse
        /// </summary>
        XC_cross_reverse = 32,

        /// <summary>
        /// The xc crosshair
        /// </summary>
        XC_crosshair = 34,

        /// <summary>
        /// The xc diamond cross
        /// </summary>
        XC_diamond_cross = 36,

        /// <summary>
        /// The xc dot
        /// </summary>
        XC_dot = 38,

        /// <summary>
        /// The xc dotbox
        /// </summary>
        XC_dotbox = 40,

        /// <summary>
        /// The xc double arrow
        /// </summary>
        XC_double_arrow = 42,

        /// <summary>
        /// The xc draft large
        /// </summary>
        XC_draft_large = 44,

        /// <summary>
        /// The xc draft small
        /// </summary>
        XC_draft_small = 46,

        /// <summary>
        /// The xc draped box
        /// </summary>
        XC_draped_box = 48,

        /// <summary>
        /// The xc exchange
        /// </summary>
        XC_exchange = 50,

        /// <summary>
        /// The xc fleur
        /// </summary>
        XC_fleur = 52,

        /// <summary>
        /// The xc gobbler
        /// </summary>
        XC_gobbler = 54,

        /// <summary>
        /// The xc gumby
        /// </summary>
        XC_gumby = 56,

        /// <summary>
        /// The xc hand1
        /// </summary>
        XC_hand1 = 58,

        /// <summary>
        /// The xc hand2
        /// </summary>
        XC_hand2 = 60,

        /// <summary>
        /// The xc heart
        /// </summary>
        XC_heart = 62,

        /// <summary>
        /// The xc icon
        /// </summary>
        XC_icon = 64,

        /// <summary>
        /// The xc iron cross
        /// </summary>
        XC_iron_cross = 66,

        /// <summary>
        /// The xc left PTR
        /// </summary>
        XC_left_ptr = 68,

        /// <summary>
        /// The xc left side
        /// </summary>
        XC_left_side = 70,

        /// <summary>
        /// The xc left tee
        /// </summary>
        XC_left_tee = 72,

        /// <summary>
        /// The xc left button
        /// </summary>
        XC_left_button = 74,

        /// <summary>
        /// The xc ll angle
        /// </summary>
        XC_ll_angle = 76,

        /// <summary>
        /// The xc lr angle
        /// </summary>
        XC_lr_angle = 78,

        /// <summary>
        /// The xc man
        /// </summary>
        XC_man = 80,

        /// <summary>
        /// The xc middlebutton
        /// </summary>
        XC_middlebutton = 82,

        /// <summary>
        /// The xc mouse
        /// </summary>
        XC_mouse = 84,

        /// <summary>
        /// The xc pencil
        /// </summary>
        XC_pencil = 86,

        /// <summary>
        /// The xc pirate
        /// </summary>
        XC_pirate = 88,

        /// <summary>
        /// The xc plus
        /// </summary>
        XC_plus = 90,

        /// <summary>
        /// The xc question arrow
        /// </summary>
        XC_question_arrow = 92,

        /// <summary>
        /// The xc right PTR
        /// </summary>
        XC_right_ptr = 94,

        /// <summary>
        /// The xc right side
        /// </summary>
        XC_right_side = 96,

        /// <summary>
        /// The xc right tee
        /// </summary>
        XC_right_tee = 98,

        /// <summary>
        /// The xc rightbutton
        /// </summary>
        XC_rightbutton = 100,

        /// <summary>
        /// The xc RTL logo
        /// </summary>
        XC_rtl_logo = 102,

        /// <summary>
        /// The xc sailboat
        /// </summary>
        XC_sailboat = 104,

        /// <summary>
        /// The xc sb down arrow
        /// </summary>
        XC_sb_down_arrow = 106,

        /// <summary>
        /// The xc sb h double arrow
        /// </summary>
        XC_sb_h_double_arrow = 108,

        /// <summary>
        /// The xc sb left arrow
        /// </summary>
        XC_sb_left_arrow = 110,

        /// <summary>
        /// The xc sb right arrow
        /// </summary>
        XC_sb_right_arrow = 112,

        /// <summary>
        /// The xc sb up arrow
        /// </summary>
        XC_sb_up_arrow = 114,

        /// <summary>
        /// The xc sb v double arrow
        /// </summary>
        XC_sb_v_double_arrow = 116,

        /// <summary>
        /// The xc sb shuttle
        /// </summary>
        XC_sb_shuttle = 118,

        /// <summary>
        /// The xc sizing
        /// </summary>
        XC_sizing = 120,

        /// <summary>
        /// The xc spider
        /// </summary>
        XC_spider = 122,

        /// <summary>
        /// The xc spraycan
        /// </summary>
        XC_spraycan = 124,

        /// <summary>
        /// The xc star
        /// </summary>
        XC_star = 126,

        /// <summary>
        /// The xc target
        /// </summary>
        XC_target = 128,

        /// <summary>
        /// The xc tcross
        /// </summary>
        XC_tcross = 130,

        /// <summary>
        /// The xc top left arrow
        /// </summary>
        XC_top_left_arrow = 132,

        /// <summary>
        /// The xc top left corner
        /// </summary>
        XC_top_left_corner = 134,

        /// <summary>
        /// The xc top right corner
        /// </summary>
        XC_top_right_corner = 136,

        /// <summary>
        /// The xc top side
        /// </summary>
        XC_top_side = 138,

        /// <summary>
        /// The xc top tee
        /// </summary>
        XC_top_tee = 140,

        /// <summary>
        /// The xc trek
        /// </summary>
        XC_trek = 142,

        /// <summary>
        /// The xc ul angle
        /// </summary>
        XC_ul_angle = 144,

        /// <summary>
        /// The xc umbrella
        /// </summary>
        XC_umbrella = 146,

        /// <summary>
        /// The xc ur angle
        /// </summary>
        XC_ur_angle = 148,

        /// <summary>
        /// The xc watch
        /// </summary>
        XC_watch = 150,

        /// <summary>
        /// The xc xterm
        /// </summary>
        XC_xterm = 152,

        /// <summary>
        /// The xc number glyphs
        /// </summary>
        XC_num_glyphs = 154
    }

    /// <summary>
    /// Enum EventMask
    /// </summary>
    [Flags]
    internal enum EventMask
    {
        /// <summary>
        /// The no event mask
        /// </summary>
        NoEventMask = 0,

        /// <summary>
        /// The key press mask
        /// </summary>
        KeyPressMask = 1 << 0,

        /// <summary>
        /// The key release mask
        /// </summary>
        KeyReleaseMask = 1 << 1,

        /// <summary>
        /// The button press mask
        /// </summary>
        ButtonPressMask = 1 << 2,

        /// <summary>
        /// The button release mask
        /// </summary>
        ButtonReleaseMask = 1 << 3,

        /// <summary>
        /// The enter window mask
        /// </summary>
        EnterWindowMask = 1 << 4,

        /// <summary>
        /// The leave window mask
        /// </summary>
        LeaveWindowMask = 1 << 5,

        /// <summary>
        /// The pointer motion mask
        /// </summary>
        PointerMotionMask = 1 << 6,

        /// <summary>
        /// The pointer motion hint mask
        /// </summary>
        PointerMotionHintMask = 1 << 7,

        /// <summary>
        /// The button1 motion mask
        /// </summary>
        Button1MotionMask = 1 << 8,

        /// <summary>
        /// The button2 motion mask
        /// </summary>
        Button2MotionMask = 1 << 9,

        /// <summary>
        /// The button3 motion mask
        /// </summary>
        Button3MotionMask = 1 << 10,

        /// <summary>
        /// The button4 motion mask
        /// </summary>
        Button4MotionMask = 1 << 11,

        /// <summary>
        /// The button5 motion mask
        /// </summary>
        Button5MotionMask = 1 << 12,

        /// <summary>
        /// The button motion mask
        /// </summary>
        ButtonMotionMask = 1 << 13,

        /// <summary>
        /// The keymap state mask
        /// </summary>
        KeymapStateMask = 1 << 14,

        /// <summary>
        /// The exposure mask
        /// </summary>
        ExposureMask = 1 << 15,

        /// <summary>
        /// The visibility change mask
        /// </summary>
        VisibilityChangeMask = 1 << 16,

        /// <summary>
        /// The structure notify mask
        /// </summary>
        StructureNotifyMask = 1 << 17,

        /// <summary>
        /// The resize redirect mask
        /// </summary>
        ResizeRedirectMask = 1 << 18,

        /// <summary>
        /// The substructure notify mask
        /// </summary>
        SubstructureNotifyMask = 1 << 19,

        /// <summary>
        /// The substructure redirect mask
        /// </summary>
        SubstructureRedirectMask = 1 << 20,

        /// <summary>
        /// The focus change mask
        /// </summary>
        FocusChangeMask = 1 << 21,

        /// <summary>
        /// The property change mask
        /// </summary>
        PropertyChangeMask = 1 << 22,

        /// <summary>
        /// The colormap change mask
        /// </summary>
        ColormapChangeMask = 1 << 23,

        /// <summary>
        /// The owner grab button mask
        /// </summary>
        OwnerGrabButtonMask = 1 << 24
    }

    /// <summary>
    /// Enum GCArcMode
    /// </summary>
    internal enum GCArcMode
    {
        /// <summary>
        /// The arc chord
        /// </summary>
        ArcChord = 0,

        /// <summary>
        /// The arc pie slice
        /// </summary>
        ArcPieSlice = 1
    }

    /// <summary>
    /// Enum GCCapStyle
    /// </summary>
    internal enum GCCapStyle
    {
        /// <summary>
        /// The cap not last
        /// </summary>
        CapNotLast = 0,

        /// <summary>
        /// The cap butt
        /// </summary>
        CapButt = 1,

        /// <summary>
        /// The cap round
        /// </summary>
        CapRound = 2,

        /// <summary>
        /// The cap projecting
        /// </summary>
        CapProjecting = 3
    }

    /// <summary>
    /// Enum GCFillRule
    /// </summary>
    internal enum GCFillRule
    {
        /// <summary>
        /// The even odd rule
        /// </summary>
        EvenOddRule = 0,

        /// <summary>
        /// The winding rule
        /// </summary>
        WindingRule = 1
    }

    /// <summary>
    /// Enum GCFillStyle
    /// </summary>
    internal enum GCFillStyle
    {
        /// <summary>
        /// The fill solid
        /// </summary>
        FillSolid = 0,

        /// <summary>
        /// The fill tiled
        /// </summary>
        FillTiled = 1,

        /// <summary>
        /// The fill stippled
        /// </summary>
        FillStippled = 2,

        /// <summary>
        /// The fill opaque stppled
        /// </summary>
        FillOpaqueStppled = 3
    }

    /// <summary>
    /// Enum GCFunction
    /// </summary>
    [Flags]
    internal enum GCFunction
    {
        /// <summary>
        /// The gc function
        /// </summary>
        GCFunction = 1 << 0,

        /// <summary>
        /// The gc plane mask
        /// </summary>
        GCPlaneMask = 1 << 1,

        /// <summary>
        /// The gc foreground
        /// </summary>
        GCForeground = 1 << 2,

        /// <summary>
        /// The gc background
        /// </summary>
        GCBackground = 1 << 3,

        /// <summary>
        /// The gc line width
        /// </summary>
        GCLineWidth = 1 << 4,

        /// <summary>
        /// The gc line style
        /// </summary>
        GCLineStyle = 1 << 5,

        /// <summary>
        /// The gc cap style
        /// </summary>
        GCCapStyle = 1 << 6,

        /// <summary>
        /// The gc join style
        /// </summary>
        GCJoinStyle = 1 << 7,

        /// <summary>
        /// The gc fill style
        /// </summary>
        GCFillStyle = 1 << 8,

        /// <summary>
        /// The gc fill rule
        /// </summary>
        GCFillRule = 1 << 9,

        /// <summary>
        /// The gc tile
        /// </summary>
        GCTile = 1 << 10,

        /// <summary>
        /// The gc stipple
        /// </summary>
        GCStipple = 1 << 11,

        /// <summary>
        /// The gc tile stip x origin
        /// </summary>
        GCTileStipXOrigin = 1 << 12,

        /// <summary>
        /// The gc tile stip y origin
        /// </summary>
        GCTileStipYOrigin = 1 << 13,

        /// <summary>
        /// The gc font
        /// </summary>
        GCFont = 1 << 14,

        /// <summary>
        /// The gc subwindow mode
        /// </summary>
        GCSubwindowMode = 1 << 15,

        /// <summary>
        /// The gc graphics exposures
        /// </summary>
        GCGraphicsExposures = 1 << 16,

        /// <summary>
        /// The gc clip x origin
        /// </summary>
        GCClipXOrigin = 1 << 17,

        /// <summary>
        /// The gc clip y origin
        /// </summary>
        GCClipYOrigin = 1 << 18,

        /// <summary>
        /// The gc clip mask
        /// </summary>
        GCClipMask = 1 << 19,

        /// <summary>
        /// The gc dash offset
        /// </summary>
        GCDashOffset = 1 << 20,

        /// <summary>
        /// The gc dash list
        /// </summary>
        GCDashList = 1 << 21,

        /// <summary>
        /// The gc arc mode
        /// </summary>
        GCArcMode = 1 << 22
    }

    /// <summary>
    /// Enum GCJoinStyle
    /// </summary>
    internal enum GCJoinStyle
    {
        /// <summary>
        /// The join miter
        /// </summary>
        JoinMiter = 0,

        /// <summary>
        /// The join round
        /// </summary>
        JoinRound = 1,

        /// <summary>
        /// The join bevel
        /// </summary>
        JoinBevel = 2
    }

    /// <summary>
    /// Enum GCLineStyle
    /// </summary>
    internal enum GCLineStyle
    {
        /// <summary>
        /// The line solid
        /// </summary>
        LineSolid = 0,

        /// <summary>
        /// The line on off dash
        /// </summary>
        LineOnOffDash = 1,

        /// <summary>
        /// The line double dash
        /// </summary>
        LineDoubleDash = 2
    }

    /// <summary>
    /// Enum GCSubwindowMode
    /// </summary>
    internal enum GCSubwindowMode
    {
        /// <summary>
        /// The clip by children
        /// </summary>
        ClipByChildren = 0,

        /// <summary>
        /// The include inferiors
        /// </summary>
        IncludeInferiors = 1
    }

    /// <summary>
    /// Enum GrabMode
    /// </summary>
    internal enum GrabMode
    {
        /// <summary>
        /// The grab mode synchronize
        /// </summary>
        GrabModeSync = 0,

        /// <summary>
        /// The grab mode asynchronous
        /// </summary>
        GrabModeAsync = 1
    }

    /// <summary>
    /// Enum Gravity
    /// </summary>
    internal enum Gravity
    {
        /// <summary>
        /// The forget gravity
        /// </summary>
        ForgetGravity = 0,

        /// <summary>
        /// The north west gravity
        /// </summary>
        NorthWestGravity = 1,

        /// <summary>
        /// The north gravity
        /// </summary>
        NorthGravity = 2,

        /// <summary>
        /// The north east gravity
        /// </summary>
        NorthEastGravity = 3,

        /// <summary>
        /// The west gravity
        /// </summary>
        WestGravity = 4,

        /// <summary>
        /// The center gravity
        /// </summary>
        CenterGravity = 5,

        /// <summary>
        /// The east gravity
        /// </summary>
        EastGravity = 6,

        /// <summary>
        /// The south west gravity
        /// </summary>
        SouthWestGravity = 7,

        /// <summary>
        /// The south gravity
        /// </summary>
        SouthGravity = 8,

        /// <summary>
        /// The south east gravity
        /// </summary>
        SouthEastGravity = 9,

        /// <summary>
        /// The static gravity
        /// </summary>
        StaticGravity = 10
    }

    /// <summary>
    /// Enum GXFunction
    /// </summary>
    internal enum GXFunction
    {
        /// <summary>
        /// The g xclear
        /// </summary>
        GXclear = 0x0,      /* 0 */

        /// <summary>
        /// The g xand
        /// </summary>
        GXand = 0x1,      /* src AND dst */

        /// <summary>
        /// The g xand reverse
        /// </summary>
        GXandReverse = 0x2,      /* src AND NOT dst */

        /// <summary>
        /// The g xcopy
        /// </summary>
        GXcopy = 0x3,      /* src */

        /// <summary>
        /// The g xand inverted
        /// </summary>
        GXandInverted = 0x4,      /* NOT src AND dst */

        /// <summary>
        /// The g xnoop
        /// </summary>
        GXnoop = 0x5,      /* dst */

        /// <summary>
        /// The g xxor
        /// </summary>
        GXxor = 0x6,      /* src XOR dst */

        /// <summary>
        /// The g xor
        /// </summary>
        GXor = 0x7,      /* src OR dst */

        /// <summary>
        /// The g xnor
        /// </summary>
        GXnor = 0x8,      /* NOT src AND NOT dst */

        /// <summary>
        /// The g xequiv
        /// </summary>
        GXequiv = 0x9,      /* NOT src XOR dst */

        /// <summary>
        /// The g xinvert
        /// </summary>
        GXinvert = 0xa,      /* NOT dst */

        /// <summary>
        /// The g xor reverse
        /// </summary>
        GXorReverse = 0xb,      /* src OR NOT dst */

        /// <summary>
        /// The g xcopy inverted
        /// </summary>
        GXcopyInverted = 0xc,      /* NOT src */

        /// <summary>
        /// The g xor inverted
        /// </summary>
        GXorInverted = 0xd,      /* NOT src OR dst */

        /// <summary>
        /// The g xnand
        /// </summary>
        GXnand = 0xe,      /* NOT src OR NOT dst */

        /// <summary>
        /// The g xset
        /// </summary>
        GXset = 0xf     /* 1 */
    }

    /// <summary>
    /// Enum KeyMasks
    /// </summary>
    [Flags]
    internal enum KeyMasks
    {
        /// <summary>
        /// The shift mask
        /// </summary>
        ShiftMask = (1 << 0),

        /// <summary>
        /// The lock mask
        /// </summary>
        LockMask = (1 << 1),

        /// <summary>
        /// The control mask
        /// </summary>
        ControlMask = (1 << 2),

        /// <summary>
        /// The mod1 mask
        /// </summary>
        Mod1Mask = (1 << 3),

        /// <summary>
        /// The mod2 mask
        /// </summary>
        Mod2Mask = (1 << 4),

        /// <summary>
        /// The mod3 mask
        /// </summary>
        Mod3Mask = (1 << 5),

        /// <summary>
        /// The mod4 mask
        /// </summary>
        Mod4Mask = (1 << 6),

        /// <summary>
        /// The mod5 mask
        /// </summary>
        Mod5Mask = (1 << 7),

        /// <summary>
        /// The mod masks
        /// </summary>
        ModMasks = Mod1Mask | Mod2Mask | Mod3Mask | Mod4Mask | Mod5Mask
    }

    /// <summary>
    /// Enum MapState
    /// </summary>
    internal enum MapState
    {
        /// <summary>
        /// The is unmapped
        /// </summary>
        IsUnmapped = 0,

        /// <summary>
        /// The is unviewable
        /// </summary>
        IsUnviewable = 1,

        /// <summary>
        /// The is viewable
        /// </summary>
        IsViewable = 2
    }

    /// <summary>
    /// Enum MotifDecorations
    /// </summary>
    [Flags]
    internal enum MotifDecorations
    {
        /// <summary>
        /// All
        /// </summary>
        All = 0x01,

        /// <summary>
        /// The border
        /// </summary>
        Border = 0x02,

        /// <summary>
        /// The resize h
        /// </summary>
        ResizeH = 0x04,

        /// <summary>
        /// The title
        /// </summary>
        Title = 0x08,

        /// <summary>
        /// The menu
        /// </summary>
        Menu = 0x10,

        /// <summary>
        /// The minimize
        /// </summary>
        Minimize = 0x20,

        /// <summary>
        /// The maximize
        /// </summary>
        Maximize = 0x40,
    }

    /// <summary>
    /// Enum MotifFlags
    /// </summary>
    [Flags]
    internal enum MotifFlags
    {
        /// <summary>
        /// The functions
        /// </summary>
        Functions = 1,

        /// <summary>
        /// The decorations
        /// </summary>
        Decorations = 2,

        /// <summary>
        /// The input mode
        /// </summary>
        InputMode = 4,

        /// <summary>
        /// The status
        /// </summary>
        Status = 8
    }

    /// <summary>
    /// Enum MotifFunctions
    /// </summary>
    [Flags]
    internal enum MotifFunctions
    {
        /// <summary>
        /// All
        /// </summary>
        All = 0x01,

        /// <summary>
        /// The resize
        /// </summary>
        Resize = 0x02,

        /// <summary>
        /// The move
        /// </summary>
        Move = 0x04,

        /// <summary>
        /// The minimize
        /// </summary>
        Minimize = 0x08,

        /// <summary>
        /// The maximize
        /// </summary>
        Maximize = 0x10,

        /// <summary>
        /// The close
        /// </summary>
        Close = 0x20
    }

    /// <summary>
    /// Enum MotifInputMode
    /// </summary>
    [Flags]
    internal enum MotifInputMode
    {
        /// <summary>
        /// The modeless
        /// </summary>
        Modeless = 0,

        /// <summary>
        /// The application modal
        /// </summary>
        ApplicationModal = 1,

        /// <summary>
        /// The system modal
        /// </summary>
        SystemModal = 2,

        /// <summary>
        /// The full application modal
        /// </summary>
        FullApplicationModal = 3
    }

    /// <summary>
    /// Enum MouseKeyMasks
    /// </summary>
    [Flags]
    internal enum MouseKeyMasks
    {
        /// <summary>
        /// The button1 mask
        /// </summary>
        Button1Mask = (1 << 8),

        /// <summary>
        /// The button2 mask
        /// </summary>
        Button2Mask = (1 << 9),

        /// <summary>
        /// The button3 mask
        /// </summary>
        Button3Mask = (1 << 10),

        /// <summary>
        /// The button4 mask
        /// </summary>
        Button4Mask = (1 << 11),

        /// <summary>
        /// The button5 mask
        /// </summary>
        Button5Mask = (1 << 12),
    }

    /// <summary>
    /// Enum NetWindowManagerState
    /// </summary>
    internal enum NetWindowManagerState
    {
        /// <summary>
        /// The remove
        /// </summary>
        Remove = 0,

        /// <summary>
        /// The add
        /// </summary>
        Add = 1,

        /// <summary>
        /// The toggle
        /// </summary>
        Toggle = 2
    }

    /// <summary>
    /// Enum NetWmMoveResize
    /// </summary>
    internal enum NetWmMoveResize
    {
        /// <summary>
        /// The net wm moveresize size topleft
        /// </summary>
        _NET_WM_MOVERESIZE_SIZE_TOPLEFT = 0,

        /// <summary>
        /// The net wm moveresize size top
        /// </summary>
        _NET_WM_MOVERESIZE_SIZE_TOP = 1,

        /// <summary>
        /// The net wm moveresize size topright
        /// </summary>
        _NET_WM_MOVERESIZE_SIZE_TOPRIGHT = 2,

        /// <summary>
        /// The net wm moveresize size right
        /// </summary>
        _NET_WM_MOVERESIZE_SIZE_RIGHT = 3,

        /// <summary>
        /// The net wm moveresize size bottomright
        /// </summary>
        _NET_WM_MOVERESIZE_SIZE_BOTTOMRIGHT = 4,

        /// <summary>
        /// The net wm moveresize size bottom
        /// </summary>
        _NET_WM_MOVERESIZE_SIZE_BOTTOM = 5,

        /// <summary>
        /// The net wm moveresize size bottomleft
        /// </summary>
        _NET_WM_MOVERESIZE_SIZE_BOTTOMLEFT = 6,

        /// <summary>
        /// The net wm moveresize size left
        /// </summary>
        _NET_WM_MOVERESIZE_SIZE_LEFT = 7,

        /// <summary>
        /// The net wm moveresize move
        /// </summary>
        _NET_WM_MOVERESIZE_MOVE = 8,

        /// <summary>
        /// The net wm moveresize size keyboard
        /// </summary>
        _NET_WM_MOVERESIZE_SIZE_KEYBOARD = 9,

        /// <summary>
        /// The net wm moveresize move keyboard
        /// </summary>
        _NET_WM_MOVERESIZE_MOVE_KEYBOARD = 10,

        /// <summary>
        /// The net wm moveresize cancel
        /// </summary>
        _NET_WM_MOVERESIZE_CANCEL = 11
    }

    /// <summary>
    /// Enum NetWmStateRequest
    /// </summary>
    internal enum NetWmStateRequest
    {
        /// <summary>
        /// The net wm state remove
        /// </summary>
        _NET_WM_STATE_REMOVE = 0,

        /// <summary>
        /// The net wm state add
        /// </summary>
        _NET_WM_STATE_ADD = 1,

        /// <summary>
        /// The net wm state toggle
        /// </summary>
        _NET_WM_STATE_TOGGLE = 2
    }

    /// <summary>
    /// Enum NotifyDetail
    /// </summary>
    internal enum NotifyDetail
    {
        /// <summary>
        /// The notify ancestor
        /// </summary>
        NotifyAncestor = 0,

        /// <summary>
        /// The notify virtual
        /// </summary>
        NotifyVirtual = 1,

        /// <summary>
        /// The notify inferior
        /// </summary>
        NotifyInferior = 2,

        /// <summary>
        /// The notify nonlinear
        /// </summary>
        NotifyNonlinear = 3,

        /// <summary>
        /// The notify nonlinear virtual
        /// </summary>
        NotifyNonlinearVirtual = 4,

        /// <summary>
        /// The notify pointer
        /// </summary>
        NotifyPointer = 5,

        /// <summary>
        /// The notify pointer root
        /// </summary>
        NotifyPointerRoot = 6,

        /// <summary>
        /// The notify detail none
        /// </summary>
        NotifyDetailNone = 7
    }

    /// <summary>
    /// Enum NotifyMode
    /// </summary>
    internal enum NotifyMode
    {
        /// <summary>
        /// The notify normal
        /// </summary>
        NotifyNormal = 0,

        /// <summary>
        /// The notify grab
        /// </summary>
        NotifyGrab = 1,

        /// <summary>
        /// The notify ungrab
        /// </summary>
        NotifyUngrab = 2
    }

    /// <summary>
    /// Enum PropertyMode
    /// </summary>
    internal enum PropertyMode
    {
        /// <summary>
        /// The replace
        /// </summary>
        Replace = 0,

        /// <summary>
        /// The prepend
        /// </summary>
        Prepend = 1,

        /// <summary>
        /// The append
        /// </summary>
        Append = 2
    }

    /// <summary>
    /// Enum RandrEvent
    /// </summary>
    internal enum RandrEvent
    {
        /// <summary>
        /// The rr screen change notify
        /// </summary>
        RRScreenChangeNotify = 0,

        /* V1.2 additions */

        /// <summary>
        /// The rr notify
        /// </summary>
        RRNotify = 1
    }

    /// <summary>
    /// Enum RandrEventMask
    /// </summary>
    [Flags]
    internal enum RandrEventMask
    {
        /// <summary>
        /// The rr screen change notify
        /// </summary>
        RRScreenChangeNotify = 1 << 0,

        /* V1.2 additions */

        /// <summary>
        /// The rr CRTC change notify mask
        /// </summary>
        RRCrtcChangeNotifyMask = 1 << 1,

        /// <summary>
        /// The rr output change notify mask
        /// </summary>
        RROutputChangeNotifyMask = 1 << 2,

        /// <summary>
        /// The rr output property notify mask
        /// </summary>
        RROutputPropertyNotifyMask = 1 << 3,

        /* V1.4 additions */

        /// <summary>
        /// The rr provider change notify mask
        /// </summary>
        RRProviderChangeNotifyMask = 1 << 4,

        /// <summary>
        /// The rr provider property notify mask
        /// </summary>
        RRProviderPropertyNotifyMask = 1 << 5,

        /// <summary>
        /// The rr resource change notify mask
        /// </summary>
        RRResourceChangeNotifyMask = 1 << 6,

        /* V1.6 additions */

        /// <summary>
        /// The rr lease notify mask
        /// </summary>
        RRLeaseNotifyMask = 1 << 7
    }

    /// <summary>
    /// Enum RandrRotate
    /// </summary>
    internal enum RandrRotate
    {
        /* used in the rotation field; rotation and reflection in 0.1 proto. */

        /// <summary>
        /// The rr rotate 0
        /// </summary>
        RR_Rotate_0 = 1,

        /// <summary>
        /// The rr rotate 90
        /// </summary>
        RR_Rotate_90 = 2,

        /// <summary>
        /// The rr rotate 180
        /// </summary>
        RR_Rotate_180 = 4,

        /// <summary>
        /// The rr rotate 270
        /// </summary>
        RR_Rotate_270 = 8
    }

    /// <summary>
    /// Enum RevertTo
    /// </summary>
    internal enum RevertTo
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,

        /// <summary>
        /// The pointer root
        /// </summary>
        PointerRoot = 1,

        /// <summary>
        /// The parent
        /// </summary>
        Parent = 2
    }

    /// <summary>
    /// Enum SendEventValues
    /// </summary>
    internal enum SendEventValues
    {
        /// <summary>
        /// The pointer window
        /// </summary>
        PointerWindow = 0,

        /// <summary>
        /// The input focus
        /// </summary>
        InputFocus = 1
    }

    /// <summary>
    /// Enum SetWindowValuemask
    /// </summary>
    [Flags]
    internal enum SetWindowValuemask
    {
        /// <summary>
        /// The nothing
        /// </summary>
        Nothing = 0,

        /// <summary>
        /// The back pixmap
        /// </summary>
        BackPixmap = 1,

        /// <summary>
        /// The back pixel
        /// </summary>
        BackPixel = 2,

        /// <summary>
        /// The border pixmap
        /// </summary>
        BorderPixmap = 4,

        /// <summary>
        /// The border pixel
        /// </summary>
        BorderPixel = 8,

        /// <summary>
        /// The bit gravity
        /// </summary>
        BitGravity = 16,

        /// <summary>
        /// The win gravity
        /// </summary>
        WinGravity = 32,

        /// <summary>
        /// The backing store
        /// </summary>
        BackingStore = 64,

        /// <summary>
        /// The backing planes
        /// </summary>
        BackingPlanes = 128,

        /// <summary>
        /// The backing pixel
        /// </summary>
        BackingPixel = 256,

        /// <summary>
        /// The override redirect
        /// </summary>
        OverrideRedirect = 512,

        /// <summary>
        /// The save under
        /// </summary>
        SaveUnder = 1024,

        /// <summary>
        /// The event mask
        /// </summary>
        EventMask = 2048,

        /// <summary>
        /// The dont propagate
        /// </summary>
        DontPropagate = 4096,

        /// <summary>
        /// The color map
        /// </summary>
        ColorMap = 8192,

        /// <summary>
        /// The cursor
        /// </summary>
        Cursor = 16384
    }

    /// <summary>
    /// Enum StackMode
    /// </summary>
    internal enum StackMode
    {
        /// <summary>
        /// The above
        /// </summary>
        Above = 0,

        /// <summary>
        /// The below
        /// </summary>
        Below = 1,

        /// <summary>
        /// The top if
        /// </summary>
        TopIf = 2,

        /// <summary>
        /// The bottom if
        /// </summary>
        BottomIf = 3,

        /// <summary>
        /// The opposite
        /// </summary>
        Opposite = 4
    }

    /// <summary>
    /// Enum SystrayRequest
    /// </summary>
    internal enum SystrayRequest
    {
        /// <summary>
        /// The system tray request dock
        /// </summary>
        SYSTEM_TRAY_REQUEST_DOCK = 0,

        /// <summary>
        /// The system tray begin message
        /// </summary>
        SYSTEM_TRAY_BEGIN_MESSAGE = 1,

        /// <summary>
        /// The system tray cancel message
        /// </summary>
        SYSTEM_TRAY_CANCEL_MESSAGE = 2
    }

    /// <summary>
    /// Enum WindowType
    /// </summary>
    [Flags]
    internal enum WindowType
    {
        /// <summary>
        /// The client
        /// </summary>
        Client = 1,

        /// <summary>
        /// The whole
        /// </summary>
        Whole = 2,

        /// <summary>
        /// The both
        /// </summary>
        Both = 3
    }

    /// <summary>
    /// Enum XEmbedMessage
    /// </summary>
    internal enum XEmbedMessage
    {
        /// <summary>
        /// The embedded notify
        /// </summary>
        EmbeddedNotify = 0,

        /// <summary>
        /// The window activate
        /// </summary>
        WindowActivate = 1,

        /// <summary>
        /// The window deactivate
        /// </summary>
        WindowDeactivate = 2,

        /// <summary>
        /// The request focus
        /// </summary>
        RequestFocus = 3,

        /// <summary>
        /// The focus in
        /// </summary>
        FocusIn = 4,

        /// <summary>
        /// The focus out
        /// </summary>
        FocusOut = 5,

        /// <summary>
        /// The focus next
        /// </summary>
        FocusNext = 6,

        /// <summary>
        /// The focus previous
        /// </summary>
        FocusPrev = 7,

        /* 8-9 were used for XEMBED_GRAB_KEY/XEMBED_UNGRAB_KEY */

        /// <summary>
        /// The modality on
        /// </summary>
        ModalityOn = 10,

        /// <summary>
        /// The modality off
        /// </summary>
        ModalityOff = 11,

        /// <summary>
        /// The register accelerator
        /// </summary>
        RegisterAccelerator = 12,

        /// <summary>
        /// The unregister accelerator
        /// </summary>
        UnregisterAccelerator = 13,

        /// <summary>
        /// The activate accelerator
        /// </summary>
        ActivateAccelerator = 14
    }

    /// <summary>
    /// Enum XEventName
    /// </summary>
    internal enum XEventName
    {
        /// <summary>
        /// The key press
        /// </summary>
        KeyPress = 2,

        /// <summary>
        /// The key release
        /// </summary>
        KeyRelease = 3,

        /// <summary>
        /// The button press
        /// </summary>
        ButtonPress = 4,

        /// <summary>
        /// The button release
        /// </summary>
        ButtonRelease = 5,

        /// <summary>
        /// The motion notify
        /// </summary>
        MotionNotify = 6,

        /// <summary>
        /// The enter notify
        /// </summary>
        EnterNotify = 7,

        /// <summary>
        /// The leave notify
        /// </summary>
        LeaveNotify = 8,

        /// <summary>
        /// The focus in
        /// </summary>
        FocusIn = 9,

        /// <summary>
        /// The focus out
        /// </summary>
        FocusOut = 10,

        /// <summary>
        /// The keymap notify
        /// </summary>
        KeymapNotify = 11,

        /// <summary>
        /// The expose
        /// </summary>
        Expose = 12,

        /// <summary>
        /// The graphics expose
        /// </summary>
        GraphicsExpose = 13,

        /// <summary>
        /// The no expose
        /// </summary>
        NoExpose = 14,

        /// <summary>
        /// The visibility notify
        /// </summary>
        VisibilityNotify = 15,

        /// <summary>
        /// The create notify
        /// </summary>
        CreateNotify = 16,

        /// <summary>
        /// The destroy notify
        /// </summary>
        DestroyNotify = 17,

        /// <summary>
        /// The unmap notify
        /// </summary>
        UnmapNotify = 18,

        /// <summary>
        /// The map notify
        /// </summary>
        MapNotify = 19,

        /// <summary>
        /// The map request
        /// </summary>
        MapRequest = 20,

        /// <summary>
        /// The reparent notify
        /// </summary>
        ReparentNotify = 21,

        /// <summary>
        /// The configure notify
        /// </summary>
        ConfigureNotify = 22,

        /// <summary>
        /// The configure request
        /// </summary>
        ConfigureRequest = 23,

        /// <summary>
        /// The gravity notify
        /// </summary>
        GravityNotify = 24,

        /// <summary>
        /// The resize request
        /// </summary>
        ResizeRequest = 25,

        /// <summary>
        /// The circulate notify
        /// </summary>
        CirculateNotify = 26,

        /// <summary>
        /// The circulate request
        /// </summary>
        CirculateRequest = 27,

        /// <summary>
        /// The property notify
        /// </summary>
        PropertyNotify = 28,

        /// <summary>
        /// The selection clear
        /// </summary>
        SelectionClear = 29,

        /// <summary>
        /// The selection request
        /// </summary>
        SelectionRequest = 30,

        /// <summary>
        /// The selection notify
        /// </summary>
        SelectionNotify = 31,

        /// <summary>
        /// The colormap notify
        /// </summary>
        ColormapNotify = 32,

        /// <summary>
        /// The client message
        /// </summary>
        ClientMessage = 33,

        /// <summary>
        /// The mapping notify
        /// </summary>
        MappingNotify = 34,

        /// <summary>
        /// The generic event
        /// </summary>
        GenericEvent = 35,

        /// <summary>
        /// The last event
        /// </summary>
        LASTEvent
    }

    /// <summary>
    /// Enum XIMCaretDirection
    /// </summary>
    internal enum XIMCaretDirection
    {
        /// <summary>
        /// The xim forward character
        /// </summary>
        XIMForwardChar,

        /// <summary>
        /// The xim backward character
        /// </summary>
        XIMBackwardChar,

        /// <summary>
        /// The xim forward word
        /// </summary>
        XIMForwardWord,

        /// <summary>
        /// The xim backward word
        /// </summary>
        XIMBackwardWord,

        /// <summary>
        /// The xim caret up
        /// </summary>
        XIMCaretUp,

        /// <summary>
        /// The xim caret down
        /// </summary>
        XIMCaretDown,

        /// <summary>
        /// The xim next line
        /// </summary>
        XIMNextLine,

        /// <summary>
        /// The xim previous line
        /// </summary>
        XIMPreviousLine,

        /// <summary>
        /// The xim line start
        /// </summary>
        XIMLineStart,

        /// <summary>
        /// The xim line end
        /// </summary>
        XIMLineEnd,

        /// <summary>
        /// The xim absolute position
        /// </summary>
        XIMAbsolutePosition,

        /// <summary>
        /// The xim dont change
        /// </summary>
        XIMDontChange
    }

    /// <summary>
    /// Enum XIMCaretStyle
    /// </summary>
    internal enum XIMCaretStyle
    {
        /// <summary>
        /// The is invisible
        /// </summary>
        IsInvisible,

        /// <summary>
        /// The is primary
        /// </summary>
        IsPrimary,

        /// <summary>
        /// The is secondary
        /// </summary>
        IsSecondary
    }

    /// <summary>
    /// Enum XIMFeedback
    /// </summary>
    internal enum XIMFeedback
    {
        /// <summary>
        /// The reverse
        /// </summary>
        Reverse = 1,

        /// <summary>
        /// The underline
        /// </summary>
        Underline = 2,

        /// <summary>
        /// The highlight
        /// </summary>
        Highlight = 4,

        /// <summary>
        /// The primary
        /// </summary>
        Primary = 32,

        /// <summary>
        /// The secondary
        /// </summary>
        Secondary = 64,

        /// <summary>
        /// The tertiary
        /// </summary>
        Tertiary = 128,
    }

    /// <summary>
    /// Enum XIMProperties
    /// </summary>
    [Flags]
    internal enum XIMProperties
    {
        /// <summary>
        /// The xim preedit area
        /// </summary>
        XIMPreeditArea = 0x0001,

        /// <summary>
        /// The xim preedit callbacks
        /// </summary>
        XIMPreeditCallbacks = 0x0002,

        /// <summary>
        /// The xim preedit position
        /// </summary>
        XIMPreeditPosition = 0x0004,

        /// <summary>
        /// The xim preedit nothing
        /// </summary>
        XIMPreeditNothing = 0x0008,

        /// <summary>
        /// The xim preedit none
        /// </summary>
        XIMPreeditNone = 0x0010,

        /// <summary>
        /// The xim status area
        /// </summary>
        XIMStatusArea = 0x0100,

        /// <summary>
        /// The xim status callbacks
        /// </summary>
        XIMStatusCallbacks = 0x0200,

        /// <summary>
        /// The xim status nothing
        /// </summary>
        XIMStatusNothing = 0x0400,

        /// <summary>
        /// The xim status none
        /// </summary>
        XIMStatusNone = 0x0800,
    }

    /// <summary>
    /// Enum XInitialState
    /// </summary>
    internal enum XInitialState
    {
        /// <summary>
        /// The dont care state
        /// </summary>
        DontCareState = 0,

        /// <summary>
        /// The normal state
        /// </summary>
        NormalState = 1,

        /// <summary>
        /// The zoom state
        /// </summary>
        ZoomState = 2,

        /// <summary>
        /// The iconic state
        /// </summary>
        IconicState = 3,

        /// <summary>
        /// The inactive state
        /// </summary>
        InactiveState = 4
    }

    /// <summary>
    /// Enum XKeySym
    /// </summary>
    internal enum XKeySym : uint
    {
        /// <summary>
        /// The xk back space
        /// </summary>
        XK_BackSpace = 0xFF08,

        /// <summary>
        /// The xk tab
        /// </summary>
        XK_Tab = 0xFF09,

        /// <summary>
        /// The xk clear
        /// </summary>
        XK_Clear = 0xFF0B,

        /// <summary>
        /// The xk return
        /// </summary>
        XK_Return = 0xFF0D,

        /// <summary>
        /// The xk home
        /// </summary>
        XK_Home = 0xFF50,

        /// <summary>
        /// The xk left
        /// </summary>
        XK_Left = 0xFF51,

        /// <summary>
        /// The xk up
        /// </summary>
        XK_Up = 0xFF52,

        /// <summary>
        /// The xk right
        /// </summary>
        XK_Right = 0xFF53,

        /// <summary>
        /// The xk down
        /// </summary>
        XK_Down = 0xFF54,

        /// <summary>
        /// The xk page up
        /// </summary>
        XK_Page_Up = 0xFF55,

        /// <summary>
        /// The xk page down
        /// </summary>
        XK_Page_Down = 0xFF56,

        /// <summary>
        /// The xk end
        /// </summary>
        XK_End = 0xFF57,

        /// <summary>
        /// The xk begin
        /// </summary>
        XK_Begin = 0xFF58,

        /// <summary>
        /// The xk menu
        /// </summary>
        XK_Menu = 0xFF67,

        /// <summary>
        /// The xk shift l
        /// </summary>
        XK_Shift_L = 0xFFE1,

        /// <summary>
        /// The xk shift r
        /// </summary>
        XK_Shift_R = 0xFFE2,

        /// <summary>
        /// The xk control l
        /// </summary>
        XK_Control_L = 0xFFE3,

        /// <summary>
        /// The xk control r
        /// </summary>
        XK_Control_R = 0xFFE4,

        /// <summary>
        /// The xk caps lock
        /// </summary>
        XK_Caps_Lock = 0xFFE5,

        /// <summary>
        /// The xk shift lock
        /// </summary>
        XK_Shift_Lock = 0xFFE6,

        /// <summary>
        /// The xk meta l
        /// </summary>
        XK_Meta_L = 0xFFE7,

        /// <summary>
        /// The xk meta r
        /// </summary>
        XK_Meta_R = 0xFFE8,

        /// <summary>
        /// The xk alt l
        /// </summary>
        XK_Alt_L = 0xFFE9,

        /// <summary>
        /// The xk alt r
        /// </summary>
        XK_Alt_R = 0xFFEA,

        /// <summary>
        /// The xk super l
        /// </summary>
        XK_Super_L = 0xFFEB,

        /// <summary>
        /// The xk super r
        /// </summary>
        XK_Super_R = 0xFFEC,

        /// <summary>
        /// The xk hyper l
        /// </summary>
        XK_Hyper_L = 0xFFED,

        /// <summary>
        /// The xk hyper r
        /// </summary>
        XK_Hyper_R = 0xFFEE,
    }

    /// <summary>
    /// Enum XRequest
    /// </summary>
    internal enum XRequest : byte
    {
        /// <summary>
        /// The x create window
        /// </summary>
        X_CreateWindow = 1,

        /// <summary>
        /// The x change window attributes
        /// </summary>
        X_ChangeWindowAttributes = 2,

        /// <summary>
        /// The x get window attributes
        /// </summary>
        X_GetWindowAttributes = 3,

        /// <summary>
        /// The x destroy window
        /// </summary>
        X_DestroyWindow = 4,

        /// <summary>
        /// The x destroy subwindows
        /// </summary>
        X_DestroySubwindows = 5,

        /// <summary>
        /// The x change save set
        /// </summary>
        X_ChangeSaveSet = 6,

        /// <summary>
        /// The x reparent window
        /// </summary>
        X_ReparentWindow = 7,

        /// <summary>
        /// The x map window
        /// </summary>
        X_MapWindow = 8,

        /// <summary>
        /// The x map subwindows
        /// </summary>
        X_MapSubwindows = 9,

        /// <summary>
        /// The x unmap window
        /// </summary>
        X_UnmapWindow = 10,

        /// <summary>
        /// The x unmap subwindows
        /// </summary>
        X_UnmapSubwindows = 11,

        /// <summary>
        /// The x configure window
        /// </summary>
        X_ConfigureWindow = 12,

        /// <summary>
        /// The x circulate window
        /// </summary>
        X_CirculateWindow = 13,

        /// <summary>
        /// The x get geometry
        /// </summary>
        X_GetGeometry = 14,

        /// <summary>
        /// The x query tree
        /// </summary>
        X_QueryTree = 15,

        /// <summary>
        /// The x intern atom
        /// </summary>
        X_InternAtom = 16,

        /// <summary>
        /// The x get atom name
        /// </summary>
        X_GetAtomName = 17,

        /// <summary>
        /// The x change property
        /// </summary>
        X_ChangeProperty = 18,

        /// <summary>
        /// The x delete property
        /// </summary>
        X_DeleteProperty = 19,

        /// <summary>
        /// The x get property
        /// </summary>
        X_GetProperty = 20,

        /// <summary>
        /// The x list properties
        /// </summary>
        X_ListProperties = 21,

        /// <summary>
        /// The x set selection owner
        /// </summary>
        X_SetSelectionOwner = 22,

        /// <summary>
        /// The x get selection owner
        /// </summary>
        X_GetSelectionOwner = 23,

        /// <summary>
        /// The x convert selection
        /// </summary>
        X_ConvertSelection = 24,

        /// <summary>
        /// The x send event
        /// </summary>
        X_SendEvent = 25,

        /// <summary>
        /// The x grab pointer
        /// </summary>
        X_GrabPointer = 26,

        /// <summary>
        /// The x ungrab pointer
        /// </summary>
        X_UngrabPointer = 27,

        /// <summary>
        /// The x grab button
        /// </summary>
        X_GrabButton = 28,

        /// <summary>
        /// The x ungrab button
        /// </summary>
        X_UngrabButton = 29,

        /// <summary>
        /// The x change active pointer grab
        /// </summary>
        X_ChangeActivePointerGrab = 30,

        /// <summary>
        /// The x grab keyboard
        /// </summary>
        X_GrabKeyboard = 31,

        /// <summary>
        /// The x ungrab keyboard
        /// </summary>
        X_UngrabKeyboard = 32,

        /// <summary>
        /// The x grab key
        /// </summary>
        X_GrabKey = 33,

        /// <summary>
        /// The x ungrab key
        /// </summary>
        X_UngrabKey = 34,

        /// <summary>
        /// The x allow events
        /// </summary>
        X_AllowEvents = 35,

        /// <summary>
        /// The x grab server
        /// </summary>
        X_GrabServer = 36,

        /// <summary>
        /// The x ungrab server
        /// </summary>
        X_UngrabServer = 37,

        /// <summary>
        /// The x query pointer
        /// </summary>
        X_QueryPointer = 38,

        /// <summary>
        /// The x get motion events
        /// </summary>
        X_GetMotionEvents = 39,

        /// <summary>
        /// The x translate coords
        /// </summary>
        X_TranslateCoords = 40,

        /// <summary>
        /// The x warp pointer
        /// </summary>
        X_WarpPointer = 41,

        /// <summary>
        /// The x set input focus
        /// </summary>
        X_SetInputFocus = 42,

        /// <summary>
        /// The x get input focus
        /// </summary>
        X_GetInputFocus = 43,

        /// <summary>
        /// The x query keymap
        /// </summary>
        X_QueryKeymap = 44,

        /// <summary>
        /// The x open font
        /// </summary>
        X_OpenFont = 45,

        /// <summary>
        /// The x close font
        /// </summary>
        X_CloseFont = 46,

        /// <summary>
        /// The x query font
        /// </summary>
        X_QueryFont = 47,

        /// <summary>
        /// The x query text extents
        /// </summary>
        X_QueryTextExtents = 48,

        /// <summary>
        /// The x list fonts
        /// </summary>
        X_ListFonts = 49,

        /// <summary>
        /// The x list fonts with information
        /// </summary>
        X_ListFontsWithInfo = 50,

        /// <summary>
        /// The x set font path
        /// </summary>
        X_SetFontPath = 51,

        /// <summary>
        /// The x get font path
        /// </summary>
        X_GetFontPath = 52,

        /// <summary>
        /// The x create pixmap
        /// </summary>
        X_CreatePixmap = 53,

        /// <summary>
        /// The x free pixmap
        /// </summary>
        X_FreePixmap = 54,

        /// <summary>
        /// The x create gc
        /// </summary>
        X_CreateGC = 55,

        /// <summary>
        /// The x change gc
        /// </summary>
        X_ChangeGC = 56,

        /// <summary>
        /// The x copy gc
        /// </summary>
        X_CopyGC = 57,

        /// <summary>
        /// The x set dashes
        /// </summary>
        X_SetDashes = 58,

        /// <summary>
        /// The x set clip rectangles
        /// </summary>
        X_SetClipRectangles = 59,

        /// <summary>
        /// The x free gc
        /// </summary>
        X_FreeGC = 60,

        /// <summary>
        /// The x clear area
        /// </summary>
        X_ClearArea = 61,

        /// <summary>
        /// The x copy area
        /// </summary>
        X_CopyArea = 62,

        /// <summary>
        /// The x copy plane
        /// </summary>
        X_CopyPlane = 63,

        /// <summary>
        /// The x poly point
        /// </summary>
        X_PolyPoint = 64,

        /// <summary>
        /// The x poly line
        /// </summary>
        X_PolyLine = 65,

        /// <summary>
        /// The x poly segment
        /// </summary>
        X_PolySegment = 66,

        /// <summary>
        /// The x poly rectangle
        /// </summary>
        X_PolyRectangle = 67,

        /// <summary>
        /// The x poly arc
        /// </summary>
        X_PolyArc = 68,

        /// <summary>
        /// The x fill poly
        /// </summary>
        X_FillPoly = 69,

        /// <summary>
        /// The x poly fill rectangle
        /// </summary>
        X_PolyFillRectangle = 70,

        /// <summary>
        /// The x poly fill arc
        /// </summary>
        X_PolyFillArc = 71,

        /// <summary>
        /// The x put image
        /// </summary>
        X_PutImage = 72,

        /// <summary>
        /// The x get image
        /// </summary>
        X_GetImage = 73,

        /// <summary>
        /// The x poly text8
        /// </summary>
        X_PolyText8 = 74,

        /// <summary>
        /// The x poly text16
        /// </summary>
        X_PolyText16 = 75,

        /// <summary>
        /// The x image text8
        /// </summary>
        X_ImageText8 = 76,

        /// <summary>
        /// The x image text16
        /// </summary>
        X_ImageText16 = 77,

        /// <summary>
        /// The x create colormap
        /// </summary>
        X_CreateColormap = 78,

        /// <summary>
        /// The x free colormap
        /// </summary>
        X_FreeColormap = 79,

        /// <summary>
        /// The x copy colormap and free
        /// </summary>
        X_CopyColormapAndFree = 80,

        /// <summary>
        /// The x install colormap
        /// </summary>
        X_InstallColormap = 81,

        /// <summary>
        /// The x uninstall colormap
        /// </summary>
        X_UninstallColormap = 82,

        /// <summary>
        /// The x list installed colormaps
        /// </summary>
        X_ListInstalledColormaps = 83,

        /// <summary>
        /// The x alloc color
        /// </summary>
        X_AllocColor = 84,

        /// <summary>
        /// The x alloc named color
        /// </summary>
        X_AllocNamedColor = 85,

        /// <summary>
        /// The x alloc color cells
        /// </summary>
        X_AllocColorCells = 86,

        /// <summary>
        /// The x alloc color planes
        /// </summary>
        X_AllocColorPlanes = 87,

        /// <summary>
        /// The x free colors
        /// </summary>
        X_FreeColors = 88,

        /// <summary>
        /// The x store colors
        /// </summary>
        X_StoreColors = 89,

        /// <summary>
        /// The x store named color
        /// </summary>
        X_StoreNamedColor = 90,

        /// <summary>
        /// The x query colors
        /// </summary>
        X_QueryColors = 91,

        /// <summary>
        /// The x lookup color
        /// </summary>
        X_LookupColor = 92,

        /// <summary>
        /// The x create cursor
        /// </summary>
        X_CreateCursor = 93,

        /// <summary>
        /// The x create glyph cursor
        /// </summary>
        X_CreateGlyphCursor = 94,

        /// <summary>
        /// The x free cursor
        /// </summary>
        X_FreeCursor = 95,

        /// <summary>
        /// The x recolor cursor
        /// </summary>
        X_RecolorCursor = 96,

        /// <summary>
        /// The x query best size
        /// </summary>
        X_QueryBestSize = 97,

        /// <summary>
        /// The x query extension
        /// </summary>
        X_QueryExtension = 98,

        /// <summary>
        /// The x list extensions
        /// </summary>
        X_ListExtensions = 99,

        /// <summary>
        /// The x change keyboard mapping
        /// </summary>
        X_ChangeKeyboardMapping = 100,

        /// <summary>
        /// The x get keyboard mapping
        /// </summary>
        X_GetKeyboardMapping = 101,

        /// <summary>
        /// The x change keyboard control
        /// </summary>
        X_ChangeKeyboardControl = 102,

        /// <summary>
        /// The x get keyboard control
        /// </summary>
        X_GetKeyboardControl = 103,

        /// <summary>
        /// The x bell
        /// </summary>
        X_Bell = 104,

        /// <summary>
        /// The x change pointer control
        /// </summary>
        X_ChangePointerControl = 105,

        /// <summary>
        /// The x get pointer control
        /// </summary>
        X_GetPointerControl = 106,

        /// <summary>
        /// The x set screen saver
        /// </summary>
        X_SetScreenSaver = 107,

        /// <summary>
        /// The x get screen saver
        /// </summary>
        X_GetScreenSaver = 108,

        /// <summary>
        /// The x change hosts
        /// </summary>
        X_ChangeHosts = 109,

        /// <summary>
        /// The x list hosts
        /// </summary>
        X_ListHosts = 110,

        /// <summary>
        /// The x set access control
        /// </summary>
        X_SetAccessControl = 111,

        /// <summary>
        /// The x set close down mode
        /// </summary>
        X_SetCloseDownMode = 112,

        /// <summary>
        /// The x kill client
        /// </summary>
        X_KillClient = 113,

        /// <summary>
        /// The x rotate properties
        /// </summary>
        X_RotateProperties = 114,

        /// <summary>
        /// The x force screen saver
        /// </summary>
        X_ForceScreenSaver = 115,

        /// <summary>
        /// The x set pointer mapping
        /// </summary>
        X_SetPointerMapping = 116,

        /// <summary>
        /// The x get pointer mapping
        /// </summary>
        X_GetPointerMapping = 117,

        /// <summary>
        /// The x set modifier mapping
        /// </summary>
        X_SetModifierMapping = 118,

        /// <summary>
        /// The x get modifier mapping
        /// </summary>
        X_GetModifierMapping = 119,

        /// <summary>
        /// The x no operation
        /// </summary>
        X_NoOperation = 127
    }

    /// <summary>
    /// Enum XSizeHintsFlags
    /// </summary>
    [Flags]
    internal enum XSizeHintsFlags
    {
        /// <summary>
        /// The us position
        /// </summary>
        USPosition = (1 << 0),

        /// <summary>
        /// The us size
        /// </summary>
        USSize = (1 << 1),

        /// <summary>
        /// The p position
        /// </summary>
        PPosition = (1 << 2),

        /// <summary>
        /// The p size
        /// </summary>
        PSize = (1 << 3),

        /// <summary>
        /// The p minimum size
        /// </summary>
        PMinSize = (1 << 4),

        /// <summary>
        /// The p maximum size
        /// </summary>
        PMaxSize = (1 << 5),

        /// <summary>
        /// The p resize inc
        /// </summary>
        PResizeInc = (1 << 6),

        /// <summary>
        /// The p aspect
        /// </summary>
        PAspect = (1 << 7),

        /// <summary>
        /// The p all hints
        /// </summary>
        PAllHints = (PPosition | PSize | PMinSize | PMaxSize | PResizeInc | PAspect),

        /// <summary>
        /// The p base size
        /// </summary>
        PBaseSize = (1 << 8),

        /// <summary>
        /// The p win gravity
        /// </summary>
        PWinGravity = (1 << 9),
    }

    /// <summary>
    /// Enum XWindowClass
    /// </summary>
    internal enum XWindowClass
    {
        /// <summary>
        /// The input output
        /// </summary>
        InputOutput = 1,

        /// <summary>
        /// The input only
        /// </summary>
        InputOnly = 2
    }

    /// <summary>
    /// Enum XWMHintsFlags
    /// </summary>
    [Flags]
    internal enum XWMHintsFlags
    {
        /// <summary>
        /// The input hint
        /// </summary>
        InputHint = (1 << 0),

        /// <summary>
        /// The state hint
        /// </summary>
        StateHint = (1 << 1),

        /// <summary>
        /// The icon pixmap hint
        /// </summary>
        IconPixmapHint = (1 << 2),

        /// <summary>
        /// The icon window hint
        /// </summary>
        IconWindowHint = (1 << 3),

        /// <summary>
        /// The icon position hint
        /// </summary>
        IconPositionHint = (1 << 4),

        /// <summary>
        /// The icon mask hint
        /// </summary>
        IconMaskHint = (1 << 5),

        /// <summary>
        /// The window group hint
        /// </summary>
        WindowGroupHint = (1 << 6),

        /// <summary>
        /// All hints
        /// </summary>
        AllHints = (InputHint | StateHint | IconPixmapHint | IconWindowHint | IconPositionHint | IconMaskHint | WindowGroupHint)
    }

    /// <summary>
    /// Struct XImage
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct XImage
    {
        /// <summary>
        /// The width
        /// </summary>
        public int width, height; /* size of image */

        /// <summary>
        /// The xoffset
        /// </summary>
        public int xoffset; /* number of pixels offset in X direction */

        /// <summary>
        /// The format
        /// </summary>
        public int format; /* XYBitmap, XYPixmap, ZPixmap */

        /// <summary>
        /// The data
        /// </summary>
        public IntPtr data; /* pointer to image data */

        /// <summary>
        /// The byte order
        /// </summary>
        public int byte_order; /* data byte order, LSBFirst, MSBFirst */

        /// <summary>
        /// The bitmap unit
        /// </summary>
        public int bitmap_unit; /* quant. of scanline 8, 16, 32 */

        /// <summary>
        /// The bitmap bit order
        /// </summary>
        public int bitmap_bit_order; /* LSBFirst, MSBFirst */

        /// <summary>
        /// The bitmap pad
        /// </summary>
        public int bitmap_pad; /* 8, 16, 32 either XY or ZPixmap */

        /// <summary>
        /// The depth
        /// </summary>
        public int depth; /* depth of image */

        /// <summary>
        /// The bytes per line
        /// </summary>
        public int bytes_per_line; /* accelerator to next scanline */

        /// <summary>
        /// The bits per pixel
        /// </summary>
        public int bits_per_pixel; /* bits per pixel (ZPixmap) */

        /// <summary>
        /// The red mask
        /// </summary>
        public ulong red_mask; /* bits in z arrangement */

        /// <summary>
        /// The green mask
        /// </summary>
        public ulong green_mask;

        /// <summary>
        /// The blue mask
        /// </summary>
        public ulong blue_mask;

        /// <summary>
        /// The funcs
        /// </summary>
        private fixed byte funcs[128];
    }

    /// <summary>
    /// Struct MotifWmHints
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MotifWmHints
    {
        /// <summary>
        /// The flags
        /// </summary>
        internal IntPtr flags;

        /// <summary>
        /// The functions
        /// </summary>
        internal IntPtr functions;

        /// <summary>
        /// The decorations
        /// </summary>
        internal IntPtr decorations;

        /// <summary>
        /// The input mode
        /// </summary>
        internal IntPtr input_mode;

        /// <summary>
        /// The status
        /// </summary>
        internal IntPtr status;

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("MotifWmHints <flags={0}, functions={1}, decorations={2}, input_mode={3}, status={4}", (MotifFlags)flags.ToInt32(), (MotifFunctions)functions.ToInt32(), (MotifDecorations)decorations.ToInt32(), (MotifInputMode)input_mode.ToInt32(), status.ToInt32());
        }
    }

    /// <summary>
    /// Struct XAnyEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XAnyEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;
    }

    /// <summary>
    /// Struct XButtonEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XButtonEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The root
        /// </summary>
        internal IntPtr root;

        /// <summary>
        /// The subwindow
        /// </summary>
        internal IntPtr subwindow;

        /// <summary>
        /// The time
        /// </summary>
        internal IntPtr time;

        /// <summary>
        /// The x
        /// </summary>
        internal int x;

        /// <summary>
        /// The y
        /// </summary>
        internal int y;

        /// <summary>
        /// The x root
        /// </summary>
        internal int x_root;

        /// <summary>
        /// The y root
        /// </summary>
        internal int y_root;

        /// <summary>
        /// The state
        /// </summary>
        internal XModifierMask state;

        /// <summary>
        /// The button
        /// </summary>
        internal int button;

        /// <summary>
        /// The same screen
        /// </summary>
        internal bool same_screen;
    }

    /// <summary>
    /// Struct XCirculateEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XCirculateEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The xevent
        /// </summary>
        internal IntPtr xevent;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The place
        /// </summary>
        internal int place;
    }

    /// <summary>
    /// Struct XCirculateRequestEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XCirculateRequestEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The parent
        /// </summary>
        internal IntPtr parent;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The place
        /// </summary>
        internal int place;
    }

    /// <summary>
    /// Struct XClientMessageEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XClientMessageEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The message type
        /// </summary>
        internal IntPtr message_type;

        /// <summary>
        /// The format
        /// </summary>
        internal int format;

        /// <summary>
        /// The PTR1
        /// </summary>
        internal IntPtr ptr1;

        /// <summary>
        /// The PTR2
        /// </summary>
        internal IntPtr ptr2;

        /// <summary>
        /// The PTR3
        /// </summary>
        internal IntPtr ptr3;

        /// <summary>
        /// The PTR4
        /// </summary>
        internal IntPtr ptr4;

        /// <summary>
        /// The PTR5
        /// </summary>
        internal IntPtr ptr5;
    }

    /// <summary>
    /// Struct XColor
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct XColor
    {
        /// <summary>
        /// The pixel
        /// </summary>
        internal IntPtr pixel;

        /// <summary>
        /// The red
        /// </summary>
        internal ushort red;

        /// <summary>
        /// The green
        /// </summary>
        internal ushort green;

        /// <summary>
        /// The blue
        /// </summary>
        internal ushort blue;

        /// <summary>
        /// The flags
        /// </summary>
        internal byte flags;

        /// <summary>
        /// The pad
        /// </summary>
        internal byte pad;
    }

    /// <summary>
    /// Struct XColormapEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XColormapEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The colormap
        /// </summary>
        internal IntPtr colormap;

        /// <summary>
        /// The c new
        /// </summary>
        internal bool c_new;

        /// <summary>
        /// The state
        /// </summary>
        internal int state;
    }

    /// <summary>
    /// Struct XConfigureEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XConfigureEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The xevent
        /// </summary>
        internal IntPtr xevent;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The x
        /// </summary>
        internal int x;

        /// <summary>
        /// The y
        /// </summary>
        internal int y;

        /// <summary>
        /// The width
        /// </summary>
        internal int width;

        /// <summary>
        /// The height
        /// </summary>
        internal int height;

        /// <summary>
        /// The border width
        /// </summary>
        internal int border_width;

        /// <summary>
        /// The above
        /// </summary>
        internal IntPtr above;

        /// <summary>
        /// The override redirect
        /// </summary>
        internal bool override_redirect;
    }

    /// <summary>
    /// Struct XConfigureRequestEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XConfigureRequestEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The parent
        /// </summary>
        internal IntPtr parent;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The x
        /// </summary>
        internal int x;

        /// <summary>
        /// The y
        /// </summary>
        internal int y;

        /// <summary>
        /// The width
        /// </summary>
        internal int width;

        /// <summary>
        /// The height
        /// </summary>
        internal int height;

        /// <summary>
        /// The border width
        /// </summary>
        internal int border_width;

        /// <summary>
        /// The above
        /// </summary>
        internal IntPtr above;

        /// <summary>
        /// The detail
        /// </summary>
        internal int detail;

        /// <summary>
        /// The value mask
        /// </summary>
        internal IntPtr value_mask;
    }

    /// <summary>
    /// Struct XCreateWindowEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XCreateWindowEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The parent
        /// </summary>
        internal IntPtr parent;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The x
        /// </summary>
        internal int x;

        /// <summary>
        /// The y
        /// </summary>
        internal int y;

        /// <summary>
        /// The width
        /// </summary>
        internal int width;

        /// <summary>
        /// The height
        /// </summary>
        internal int height;

        /// <summary>
        /// The border width
        /// </summary>
        internal int border_width;

        /// <summary>
        /// The override redirect
        /// </summary>
        internal bool override_redirect;
    }

    /// <summary>
    /// Struct XCrossingEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XCrossingEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The root
        /// </summary>
        internal IntPtr root;

        /// <summary>
        /// The subwindow
        /// </summary>
        internal IntPtr subwindow;

        /// <summary>
        /// The time
        /// </summary>
        internal IntPtr time;

        /// <summary>
        /// The x
        /// </summary>
        internal int x;

        /// <summary>
        /// The y
        /// </summary>
        internal int y;

        /// <summary>
        /// The x root
        /// </summary>
        internal int x_root;

        /// <summary>
        /// The y root
        /// </summary>
        internal int y_root;

        /// <summary>
        /// The mode
        /// </summary>
        internal NotifyMode mode;

        /// <summary>
        /// The detail
        /// </summary>
        internal NotifyDetail detail;

        /// <summary>
        /// The same screen
        /// </summary>
        internal bool same_screen;

        /// <summary>
        /// The focus
        /// </summary>
        internal bool focus;

        /// <summary>
        /// The state
        /// </summary>
        internal XModifierMask state;
    }

    /// <summary>
    /// Struct XcursorImage
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XcursorImage
    {
        /// <summary>
        /// The version
        /// </summary>
        private readonly int version;

        /// <summary>
        /// The size
        /// </summary>
        public int size;       /* nominal size for matching */

        /// <summary>
        /// The width
        /// </summary>
        public int width;      /* actual width */

        /// <summary>
        /// The height
        /// </summary>
        public int height;     /* actual height */

        /// <summary>
        /// The xhot
        /// </summary>
        public int xhot;       /* hot spot x (must be inside image) */

        /// <summary>
        /// The yhot
        /// </summary>
        public int yhot;       /* hot spot y (must be inside image) */

        /// <summary>
        /// The delay
        /// </summary>
        public int delay;       /* hot spot y (must be inside image) */

        /// <summary>
        /// The pixels
        /// </summary>
        public IntPtr pixels;    /* pointer to pixels */

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("XCursorImage (version: {0}, size: {1}, width: {2}, height: {3}, xhot: {4}, yhot: {5}, delay: {6}, pixels: {7}",
                version, size, width, height, xhot, yhot, delay, pixels);
        }
    };

    /// <summary>
    /// Struct XcursorImages
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XcursorImages
    {
        /// <summary>
        /// The nimage
        /// </summary>
        public int nimage;     /* number of images */

        /// <summary>
        /// The images
        /// </summary>
        public IntPtr images;   /* array of XcursorImage pointers */
    }

    /// <summary>
    /// Struct XDestroyWindowEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XDestroyWindowEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The xevent
        /// </summary>
        internal IntPtr xevent;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;
    }

    /// <summary>
    /// Struct XErrorEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XErrorEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The resourceid
        /// </summary>
        internal IntPtr resourceid;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The error code
        /// </summary>
        internal byte error_code;

        /// <summary>
        /// The request code
        /// </summary>
        internal XRequest request_code;

        /// <summary>
        /// The minor code
        /// </summary>
        internal byte minor_code;
    }

    /// <summary>
    /// Struct XEvent
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct XEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        [FieldOffset(0)] internal XEventName type;

        /// <summary>
        /// Any event
        /// </summary>
        [FieldOffset(0)] internal XAnyEvent AnyEvent;

        /// <summary>
        /// The key event
        /// </summary>
        [FieldOffset(0)] internal XKeyEvent KeyEvent;

        /// <summary>
        /// The button event
        /// </summary>
        [FieldOffset(0)] internal XButtonEvent ButtonEvent;

        /// <summary>
        /// The motion event
        /// </summary>
        [FieldOffset(0)] internal XMotionEvent MotionEvent;

        /// <summary>
        /// The crossing event
        /// </summary>
        [FieldOffset(0)] internal XCrossingEvent CrossingEvent;

        /// <summary>
        /// The focus change event
        /// </summary>
        [FieldOffset(0)] internal XFocusChangeEvent FocusChangeEvent;

        /// <summary>
        /// The expose event
        /// </summary>
        [FieldOffset(0)] internal XExposeEvent ExposeEvent;

        /// <summary>
        /// The graphics expose event
        /// </summary>
        [FieldOffset(0)] internal XGraphicsExposeEvent GraphicsExposeEvent;

        /// <summary>
        /// The no expose event
        /// </summary>
        [FieldOffset(0)] internal XNoExposeEvent NoExposeEvent;

        /// <summary>
        /// The visibility event
        /// </summary>
        [FieldOffset(0)] internal XVisibilityEvent VisibilityEvent;

        /// <summary>
        /// The create window event
        /// </summary>
        [FieldOffset(0)] internal XCreateWindowEvent CreateWindowEvent;

        /// <summary>
        /// The destroy window event
        /// </summary>
        [FieldOffset(0)] internal XDestroyWindowEvent DestroyWindowEvent;

        /// <summary>
        /// The unmap event
        /// </summary>
        [FieldOffset(0)] internal XUnmapEvent UnmapEvent;

        /// <summary>
        /// The map event
        /// </summary>
        [FieldOffset(0)] internal XMapEvent MapEvent;

        /// <summary>
        /// The map request event
        /// </summary>
        [FieldOffset(0)] internal XMapRequestEvent MapRequestEvent;

        /// <summary>
        /// The reparent event
        /// </summary>
        [FieldOffset(0)] internal XReparentEvent ReparentEvent;

        /// <summary>
        /// The configure event
        /// </summary>
        [FieldOffset(0)] internal XConfigureEvent ConfigureEvent;

        /// <summary>
        /// The gravity event
        /// </summary>
        [FieldOffset(0)] internal XGravityEvent GravityEvent;

        /// <summary>
        /// The resize request event
        /// </summary>
        [FieldOffset(0)] internal XResizeRequestEvent ResizeRequestEvent;

        /// <summary>
        /// The configure request event
        /// </summary>
        [FieldOffset(0)] internal XConfigureRequestEvent ConfigureRequestEvent;

        /// <summary>
        /// The circulate event
        /// </summary>
        [FieldOffset(0)] internal XCirculateEvent CirculateEvent;

        /// <summary>
        /// The circulate request event
        /// </summary>
        [FieldOffset(0)] internal XCirculateRequestEvent CirculateRequestEvent;

        /// <summary>
        /// The property event
        /// </summary>
        [FieldOffset(0)] internal XPropertyEvent PropertyEvent;

        /// <summary>
        /// The selection clear event
        /// </summary>
        [FieldOffset(0)] internal XSelectionClearEvent SelectionClearEvent;

        /// <summary>
        /// The selection request event
        /// </summary>
        [FieldOffset(0)] internal XSelectionRequestEvent SelectionRequestEvent;

        /// <summary>
        /// The selection event
        /// </summary>
        [FieldOffset(0)] internal XSelectionEvent SelectionEvent;

        /// <summary>
        /// The colormap event
        /// </summary>
        [FieldOffset(0)] internal XColormapEvent ColormapEvent;

        /// <summary>
        /// The client message event
        /// </summary>
        [FieldOffset(0)] internal XClientMessageEvent ClientMessageEvent;

        /// <summary>
        /// The mapping event
        /// </summary>
        [FieldOffset(0)] internal XMappingEvent MappingEvent;

        /// <summary>
        /// The error event
        /// </summary>
        [FieldOffset(0)] internal XErrorEvent ErrorEvent;

        /// <summary>
        /// The keymap event
        /// </summary>
        [FieldOffset(0)] internal XKeymapEvent KeymapEvent;

        /// <summary>
        /// The generic event cookie
        /// </summary>
        [FieldOffset(0)] internal XGenericEventCookie GenericEventCookie;

        //[MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst=24)]
        //[ FieldOffset(0) ] internal int[] pad;
        /// <summary>
        /// The pad
        /// </summary>
        [FieldOffset(0)] internal XEventPad Pad;

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            switch (type)
            {
                case XEventName.ButtonPress:
                case XEventName.ButtonRelease:
                    return ToString(ButtonEvent);

                case XEventName.CirculateNotify:
                case XEventName.CirculateRequest:
                    return ToString(CirculateEvent);

                case XEventName.ClientMessage:
                    return ToString(ClientMessageEvent);

                case XEventName.ColormapNotify:
                    return ToString(ColormapEvent);

                case XEventName.ConfigureNotify:
                    return ToString(ConfigureEvent);

                case XEventName.ConfigureRequest:
                    return ToString(ConfigureRequestEvent);

                case XEventName.CreateNotify:
                    return ToString(CreateWindowEvent);

                case XEventName.DestroyNotify:
                    return ToString(DestroyWindowEvent);

                case XEventName.Expose:
                    return ToString(ExposeEvent);

                case XEventName.FocusIn:
                case XEventName.FocusOut:
                    return ToString(FocusChangeEvent);

                case XEventName.GraphicsExpose:
                    return ToString(GraphicsExposeEvent);

                case XEventName.GravityNotify:
                    return ToString(GravityEvent);

                case XEventName.KeymapNotify:
                    return ToString(KeymapEvent);

                case XEventName.MapNotify:
                    return ToString(MapEvent);

                case XEventName.MappingNotify:
                    return ToString(MappingEvent);

                case XEventName.MapRequest:
                    return ToString(MapRequestEvent);

                case XEventName.MotionNotify:
                    return ToString(MotionEvent);

                case XEventName.NoExpose:
                    return ToString(NoExposeEvent);

                case XEventName.PropertyNotify:
                    return ToString(PropertyEvent);

                case XEventName.ReparentNotify:
                    return ToString(ReparentEvent);

                case XEventName.ResizeRequest:
                    return ToString(ResizeRequestEvent);

                case XEventName.SelectionClear:
                    return ToString(SelectionClearEvent);

                case XEventName.SelectionNotify:
                    return ToString(SelectionEvent);

                case XEventName.SelectionRequest:
                    return ToString(SelectionRequestEvent);

                case XEventName.UnmapNotify:
                    return ToString(UnmapEvent);

                case XEventName.VisibilityNotify:
                    return ToString(VisibilityEvent);

                case XEventName.EnterNotify:
                case XEventName.LeaveNotify:
                    return ToString(CrossingEvent);

                default:
                    return type.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="ev">The ev.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public static string ToString(object ev)
        {
            string result = string.Empty;
            Type type = ev.GetType();
            FieldInfo[] fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++)
            {
                if (result != string.Empty)
                {
                    result += ", ";
                }
                object value = fields[i].GetValue(ev);
                result += fields[i].Name + "=" + (value == null ? "<null>" : value.ToString());
            }
            return type.Name + " (" + result + ")";
        }
    }

    /// <summary>
    /// Struct XEventPad
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XEventPad
    {
        /// <summary>
        /// The pad0
        /// </summary>
        internal IntPtr pad0;

        /// <summary>
        /// The pad1
        /// </summary>
        internal IntPtr pad1;

        /// <summary>
        /// The pad2
        /// </summary>
        internal IntPtr pad2;

        /// <summary>
        /// The pad3
        /// </summary>
        internal IntPtr pad3;

        /// <summary>
        /// The pad4
        /// </summary>
        internal IntPtr pad4;

        /// <summary>
        /// The pad5
        /// </summary>
        internal IntPtr pad5;

        /// <summary>
        /// The pad6
        /// </summary>
        internal IntPtr pad6;

        /// <summary>
        /// The pad7
        /// </summary>
        internal IntPtr pad7;

        /// <summary>
        /// The pad8
        /// </summary>
        internal IntPtr pad8;

        /// <summary>
        /// The pad9
        /// </summary>
        internal IntPtr pad9;

        /// <summary>
        /// The pad10
        /// </summary>
        internal IntPtr pad10;

        /// <summary>
        /// The pad11
        /// </summary>
        internal IntPtr pad11;

        /// <summary>
        /// The pad12
        /// </summary>
        internal IntPtr pad12;

        /// <summary>
        /// The pad13
        /// </summary>
        internal IntPtr pad13;

        /// <summary>
        /// The pad14
        /// </summary>
        internal IntPtr pad14;

        /// <summary>
        /// The pad15
        /// </summary>
        internal IntPtr pad15;

        /// <summary>
        /// The pad16
        /// </summary>
        internal IntPtr pad16;

        /// <summary>
        /// The pad17
        /// </summary>
        internal IntPtr pad17;

        /// <summary>
        /// The pad18
        /// </summary>
        internal IntPtr pad18;

        /// <summary>
        /// The pad19
        /// </summary>
        internal IntPtr pad19;

        /// <summary>
        /// The pad20
        /// </summary>
        internal IntPtr pad20;

        /// <summary>
        /// The pad21
        /// </summary>
        internal IntPtr pad21;

        /// <summary>
        /// The pad22
        /// </summary>
        internal IntPtr pad22;

        /// <summary>
        /// The pad23
        /// </summary>
        internal IntPtr pad23;
    }

    /// <summary>
    /// Struct XExposeEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XExposeEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The x
        /// </summary>
        internal int x;

        /// <summary>
        /// The y
        /// </summary>
        internal int y;

        /// <summary>
        /// The width
        /// </summary>
        internal int width;

        /// <summary>
        /// The height
        /// </summary>
        internal int height;

        /// <summary>
        /// The count
        /// </summary>
        internal int count;
    }

    /// <summary>
    /// Struct XFocusChangeEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XFocusChangeEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The mode
        /// </summary>
        internal int mode;

        /// <summary>
        /// The detail
        /// </summary>
        internal NotifyDetail detail;
    }

    /// <summary>
    /// Struct XGCValues
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XGCValues
    {
        /// <summary>
        /// The function
        /// </summary>
        internal GXFunction function;

        /// <summary>
        /// The plane mask
        /// </summary>
        internal IntPtr plane_mask;

        /// <summary>
        /// The foreground
        /// </summary>
        internal IntPtr foreground;

        /// <summary>
        /// The background
        /// </summary>
        internal IntPtr background;

        /// <summary>
        /// The line width
        /// </summary>
        internal int line_width;

        /// <summary>
        /// The line style
        /// </summary>
        internal GCLineStyle line_style;

        /// <summary>
        /// The cap style
        /// </summary>
        internal GCCapStyle cap_style;

        /// <summary>
        /// The join style
        /// </summary>
        internal GCJoinStyle join_style;

        /// <summary>
        /// The fill style
        /// </summary>
        internal GCFillStyle fill_style;

        /// <summary>
        /// The fill rule
        /// </summary>
        internal GCFillRule fill_rule;

        /// <summary>
        /// The arc mode
        /// </summary>
        internal GCArcMode arc_mode;

        /// <summary>
        /// The tile
        /// </summary>
        internal IntPtr tile;

        /// <summary>
        /// The stipple
        /// </summary>
        internal IntPtr stipple;

        /// <summary>
        /// The ts x origin
        /// </summary>
        internal int ts_x_origin;

        /// <summary>
        /// The ts y origin
        /// </summary>
        internal int ts_y_origin;

        /// <summary>
        /// The font
        /// </summary>
        internal IntPtr font;

        /// <summary>
        /// The subwindow mode
        /// </summary>
        internal GCSubwindowMode subwindow_mode;

        /// <summary>
        /// The graphics exposures
        /// </summary>
        internal bool graphics_exposures;

        /// <summary>
        /// The clip x origin
        /// </summary>
        internal int clip_x_origin;

        /// <summary>
        /// The clib y origin
        /// </summary>
        internal int clib_y_origin;

        /// <summary>
        /// The clip mask
        /// </summary>
        internal IntPtr clip_mask;

        /// <summary>
        /// The dash offset
        /// </summary>
        internal int dash_offset;

        /// <summary>
        /// The dashes
        /// </summary>
        internal byte dashes;
    }

    /// <summary>
    /// Struct XGenericEventCookie
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct XGenericEventCookie
    {
        /// <summary>
        /// The type
        /// </summary>
        internal int type; /* of event. Always GenericEvent */

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial; /* # of last request processed */

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event; /* true if from SendEvent request */

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display; /* Display the event was read from */

        /// <summary>
        /// The extension
        /// </summary>
        internal int extension; /* major opcode of extension that caused the event */

        /// <summary>
        /// The evtype
        /// </summary>
        internal int evtype; /* actual event type. */

        /// <summary>
        /// The cookie
        /// </summary>
        internal uint cookie;

        /// <summary>
        /// The data
        /// </summary>
        internal void* data;

        /// <summary>
        /// Gets the event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T GetEvent<T>() where T : unmanaged
        {
            if (data == null)
                throw new InvalidOperationException();
            return *(T*)data;
        }
    }

    /// <summary>
    /// Struct XGraphicsExposeEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XGraphicsExposeEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The drawable
        /// </summary>
        internal IntPtr drawable;

        /// <summary>
        /// The x
        /// </summary>
        internal int x;

        /// <summary>
        /// The y
        /// </summary>
        internal int y;

        /// <summary>
        /// The width
        /// </summary>
        internal int width;

        /// <summary>
        /// The height
        /// </summary>
        internal int height;

        /// <summary>
        /// The count
        /// </summary>
        internal int count;

        /// <summary>
        /// The major code
        /// </summary>
        internal int major_code;

        /// <summary>
        /// The minor code
        /// </summary>
        internal int minor_code;
    }

    /// <summary>
    /// Struct XGravityEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XGravityEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The xevent
        /// </summary>
        internal IntPtr xevent;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The x
        /// </summary>
        internal int x;

        /// <summary>
        /// The y
        /// </summary>
        internal int y;
    }

    /// <summary>
    /// Struct XIconSize
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XIconSize
    {
        /// <summary>
        /// The minimum width
        /// </summary>
        internal int min_width;

        /// <summary>
        /// The minimum height
        /// </summary>
        internal int min_height;

        /// <summary>
        /// The maximum width
        /// </summary>
        internal int max_width;

        /// <summary>
        /// The maximum height
        /// </summary>
        internal int max_height;

        /// <summary>
        /// The width inc
        /// </summary>
        internal int width_inc;

        /// <summary>
        /// The height inc
        /// </summary>
        internal int height_inc;
    }

    /// <summary>
    /// Struct XIMFeedbackStruct
    /// </summary>
    internal struct XIMFeedbackStruct
    {
        #region Fields

        /// <summary>
        /// The feedback mask
        /// </summary>
        public byte FeedbackMask;

        #endregion Fields

        // one or more of XIMFeedback enum
    }

    /// <summary>
    /// Struct XIMPreeditCaretCallbackStruct
    /// </summary>
    internal struct XIMPreeditCaretCallbackStruct
    {
        #region Fields

        /// <summary>
        /// The direction
        /// </summary>
        public XIMCaretDirection Direction;

        /// <summary>
        /// The position
        /// </summary>
        public int Position;

        /// <summary>
        /// The style
        /// </summary>
        public XIMCaretStyle Style;

        #endregion Fields
    }

    /// <summary>
    /// Struct XIMPreeditDrawCallbackStruct
    /// </summary>
    internal struct XIMPreeditDrawCallbackStruct
    {
        #region Fields

        /// <summary>
        /// The caret
        /// </summary>
        public int Caret;

        /// <summary>
        /// The change first
        /// </summary>
        public int ChangeFirst;

        /// <summary>
        /// The change length
        /// </summary>
        public int ChangeLength;

        /// <summary>
        /// The text
        /// </summary>
        public IntPtr Text;

        #endregion Fields

        // to XIMText
    }

    /// <summary>
    /// Struct XIMStyles
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XIMStyles
    {
        /// <summary>
        /// The count styles
        /// </summary>
        public ushort count_styles;

        /// <summary>
        /// The supported styles
        /// </summary>
        public IntPtr supported_styles;
    }

    /// <summary>
    /// Struct XIMText
    /// </summary>
    internal struct XIMText
    {
        #region Fields

        /// <summary>
        /// The encoding is w character
        /// </summary>
        public bool EncodingIsWChar;

        /// <summary>
        /// The feedback
        /// </summary>
        public IntPtr Feedback;

        /// <summary>
        /// The length
        /// </summary>
        public ushort Length;

        // to XIMFeedbackStruct
        /// <summary>
        /// The string
        /// </summary>
        public IntPtr String;

        #endregion Fields

        // it could be either char* or wchar_t*
    }

    /// <summary>
    /// Struct XKeyBoardState
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XKeyBoardState
    {
        /// <summary>
        /// The key click percent
        /// </summary>
        public int key_click_percent;

        /// <summary>
        /// The bell percent
        /// </summary>
        public int bell_percent;

        /// <summary>
        /// The bell pitch
        /// </summary>
        public uint bell_pitch, bell_duration;

        /// <summary>
        /// The led mask
        /// </summary>
        public IntPtr led_mask;

        /// <summary>
        /// The global automatic repeat
        /// </summary>
        public int global_auto_repeat;

        /// <summary>
        /// The automatic repeats
        /// </summary>
        public AutoRepeats auto_repeats;

        /// <summary>
        /// Struct AutoRepeats
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct AutoRepeats
        {
            /// <summary>
            /// The first
            /// </summary>
            [FieldOffset(0)]
            public byte first;

            /// <summary>
            /// The last
            /// </summary>
            [FieldOffset(31)]
            public byte last;
        }
    }

    /// <summary>
    /// Struct XKeyEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XKeyEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The root
        /// </summary>
        internal IntPtr root;

        /// <summary>
        /// The subwindow
        /// </summary>
        internal IntPtr subwindow;

        /// <summary>
        /// The time
        /// </summary>
        internal IntPtr time;

        /// <summary>
        /// The x
        /// </summary>
        internal int x;

        /// <summary>
        /// The y
        /// </summary>
        internal int y;

        /// <summary>
        /// The x root
        /// </summary>
        internal int x_root;

        /// <summary>
        /// The y root
        /// </summary>
        internal int y_root;

        /// <summary>
        /// The state
        /// </summary>
        internal XModifierMask state;

        /// <summary>
        /// The keycode
        /// </summary>
        internal int keycode;

        /// <summary>
        /// The same screen
        /// </summary>
        internal bool same_screen;
    }

    /// <summary>
    /// Struct XKeymapEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XKeymapEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The key vector0
        /// </summary>
        internal byte key_vector0;

        /// <summary>
        /// The key vector1
        /// </summary>
        internal byte key_vector1;

        /// <summary>
        /// The key vector2
        /// </summary>
        internal byte key_vector2;

        /// <summary>
        /// The key vector3
        /// </summary>
        internal byte key_vector3;

        /// <summary>
        /// The key vector4
        /// </summary>
        internal byte key_vector4;

        /// <summary>
        /// The key vector5
        /// </summary>
        internal byte key_vector5;

        /// <summary>
        /// The key vector6
        /// </summary>
        internal byte key_vector6;

        /// <summary>
        /// The key vector7
        /// </summary>
        internal byte key_vector7;

        /// <summary>
        /// The key vector8
        /// </summary>
        internal byte key_vector8;

        /// <summary>
        /// The key vector9
        /// </summary>
        internal byte key_vector9;

        /// <summary>
        /// The key vector10
        /// </summary>
        internal byte key_vector10;

        /// <summary>
        /// The key vector11
        /// </summary>
        internal byte key_vector11;

        /// <summary>
        /// The key vector12
        /// </summary>
        internal byte key_vector12;

        /// <summary>
        /// The key vector13
        /// </summary>
        internal byte key_vector13;

        /// <summary>
        /// The key vector14
        /// </summary>
        internal byte key_vector14;

        /// <summary>
        /// The key vector15
        /// </summary>
        internal byte key_vector15;

        /// <summary>
        /// The key vector16
        /// </summary>
        internal byte key_vector16;

        /// <summary>
        /// The key vector17
        /// </summary>
        internal byte key_vector17;

        /// <summary>
        /// The key vector18
        /// </summary>
        internal byte key_vector18;

        /// <summary>
        /// The key vector19
        /// </summary>
        internal byte key_vector19;

        /// <summary>
        /// The key vector20
        /// </summary>
        internal byte key_vector20;

        /// <summary>
        /// The key vector21
        /// </summary>
        internal byte key_vector21;

        /// <summary>
        /// The key vector22
        /// </summary>
        internal byte key_vector22;

        /// <summary>
        /// The key vector23
        /// </summary>
        internal byte key_vector23;

        /// <summary>
        /// The key vector24
        /// </summary>
        internal byte key_vector24;

        /// <summary>
        /// The key vector25
        /// </summary>
        internal byte key_vector25;

        /// <summary>
        /// The key vector26
        /// </summary>
        internal byte key_vector26;

        /// <summary>
        /// The key vector27
        /// </summary>
        internal byte key_vector27;

        /// <summary>
        /// The key vector28
        /// </summary>
        internal byte key_vector28;

        /// <summary>
        /// The key vector29
        /// </summary>
        internal byte key_vector29;

        /// <summary>
        /// The key vector30
        /// </summary>
        internal byte key_vector30;

        /// <summary>
        /// The key vector31
        /// </summary>
        internal byte key_vector31;
    }

    /// <summary>
    /// Struct XMapEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XMapEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The xevent
        /// </summary>
        internal IntPtr xevent;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The override redirect
        /// </summary>
        internal bool override_redirect;
    }

    /// <summary>
    /// Struct XMappingEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XMappingEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The request
        /// </summary>
        internal int request;

        /// <summary>
        /// The first keycode
        /// </summary>
        internal int first_keycode;

        /// <summary>
        /// The count
        /// </summary>
        internal int count;
    }

    /// <summary>
    /// Struct XMapRequestEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XMapRequestEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The parent
        /// </summary>
        internal IntPtr parent;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;
    }

    /// <summary>
    /// Struct XModifierKeymap
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XModifierKeymap
    {
        /// <summary>
        /// The maximum keypermod
        /// </summary>
        public int max_keypermod;

        /// <summary>
        /// The modifiermap
        /// </summary>
        public IntPtr modifiermap;
    }

    /// <summary>
    /// Struct XMotionEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XMotionEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The root
        /// </summary>
        internal IntPtr root;

        /// <summary>
        /// The subwindow
        /// </summary>
        internal IntPtr subwindow;

        /// <summary>
        /// The time
        /// </summary>
        internal IntPtr time;

        /// <summary>
        /// The x
        /// </summary>
        internal int x;

        /// <summary>
        /// The y
        /// </summary>
        internal int y;

        /// <summary>
        /// The x root
        /// </summary>
        internal int x_root;

        /// <summary>
        /// The y root
        /// </summary>
        internal int y_root;

        /// <summary>
        /// The state
        /// </summary>
        internal XModifierMask state;

        /// <summary>
        /// The is hint
        /// </summary>
        internal byte is_hint;

        /// <summary>
        /// The same screen
        /// </summary>
        internal bool same_screen;
    }

    /// <summary>
    /// Struct XNoExposeEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XNoExposeEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The drawable
        /// </summary>
        internal IntPtr drawable;

        /// <summary>
        /// The major code
        /// </summary>
        internal int major_code;

        /// <summary>
        /// The minor code
        /// </summary>
        internal int minor_code;
    }

    /// <summary>
    /// Struct XPropertyEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XPropertyEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The atom
        /// </summary>
        internal IntPtr atom;

        /// <summary>
        /// The time
        /// </summary>
        internal IntPtr time;

        /// <summary>
        /// The state
        /// </summary>
        internal int state;
    }

    /// <summary>
    /// Struct XReparentEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XReparentEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The xevent
        /// </summary>
        internal IntPtr xevent;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The parent
        /// </summary>
        internal IntPtr parent;

        /// <summary>
        /// The x
        /// </summary>
        internal int x;

        /// <summary>
        /// The y
        /// </summary>
        internal int y;

        /// <summary>
        /// The override redirect
        /// </summary>
        internal bool override_redirect;
    }

    /// <summary>
    /// Struct XResizeRequestEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XResizeRequestEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The width
        /// </summary>
        internal int width;

        /// <summary>
        /// The height
        /// </summary>
        internal int height;
    }

    /// <summary>
    /// Struct XRRMonitorInfo
    /// </summary>
    internal struct XRRMonitorInfo
    {
        #region Fields

        /// <summary>
        /// The automatic
        /// </summary>
        public int Automatic;

        /// <summary>
        /// The height
        /// </summary>
        public int Height;

        /// <summary>
        /// The m height
        /// </summary>
        public int MHeight;

        /// <summary>
        /// The m width
        /// </summary>
        public int MWidth;

        /// <summary>
        /// The name
        /// </summary>
        public IntPtr Name;

        /// <summary>
        /// The n output
        /// </summary>
        public int NOutput;

        /// <summary>
        /// The outputs
        /// </summary>
        public IntPtr Outputs;

        /// <summary>
        /// The primary
        /// </summary>
        public int Primary;

        /// <summary>
        /// The width
        /// </summary>
        public int Width;

        /// <summary>
        /// The x
        /// </summary>
        public int X;

        /// <summary>
        /// The y
        /// </summary>
        public int Y;

        #endregion Fields
    }

    /// <summary>
    /// Struct XScreen
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XScreen
    {
        /// <summary>
        /// The ext data
        /// </summary>
        internal IntPtr ext_data;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The root
        /// </summary>
        internal IntPtr root;

        /// <summary>
        /// The width
        /// </summary>
        internal int width;

        /// <summary>
        /// The height
        /// </summary>
        internal int height;

        /// <summary>
        /// The mwidth
        /// </summary>
        internal int mwidth;

        /// <summary>
        /// The mheight
        /// </summary>
        internal int mheight;

        /// <summary>
        /// The ndepths
        /// </summary>
        internal int ndepths;

        /// <summary>
        /// The depths
        /// </summary>
        internal IntPtr depths;

        /// <summary>
        /// The root depth
        /// </summary>
        internal int root_depth;

        /// <summary>
        /// The root visual
        /// </summary>
        internal IntPtr root_visual;

        /// <summary>
        /// The default gc
        /// </summary>
        internal IntPtr default_gc;

        /// <summary>
        /// The cmap
        /// </summary>
        internal IntPtr cmap;

        /// <summary>
        /// The white pixel
        /// </summary>
        internal IntPtr white_pixel;

        /// <summary>
        /// The black pixel
        /// </summary>
        internal IntPtr black_pixel;

        /// <summary>
        /// The maximum maps
        /// </summary>
        internal int max_maps;

        /// <summary>
        /// The minimum maps
        /// </summary>
        internal int min_maps;

        /// <summary>
        /// The backing store
        /// </summary>
        internal int backing_store;

        /// <summary>
        /// The save unders
        /// </summary>
        internal bool save_unders;

        /// <summary>
        /// The root input mask
        /// </summary>
        internal IntPtr root_input_mask;
    }

    /// <summary>
    /// Struct XSelectionClearEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XSelectionClearEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The selection
        /// </summary>
        internal IntPtr selection;

        /// <summary>
        /// The time
        /// </summary>
        internal IntPtr time;
    }

    /// <summary>
    /// Struct XSelectionEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XSelectionEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The requestor
        /// </summary>
        internal IntPtr requestor;

        /// <summary>
        /// The selection
        /// </summary>
        internal IntPtr selection;

        /// <summary>
        /// The target
        /// </summary>
        internal IntPtr target;

        /// <summary>
        /// The property
        /// </summary>
        internal IntPtr property;

        /// <summary>
        /// The time
        /// </summary>
        internal IntPtr time;
    }

    /// <summary>
    /// Struct XSelectionRequestEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XSelectionRequestEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The owner
        /// </summary>
        internal IntPtr owner;

        /// <summary>
        /// The requestor
        /// </summary>
        internal IntPtr requestor;

        /// <summary>
        /// The selection
        /// </summary>
        internal IntPtr selection;

        /// <summary>
        /// The target
        /// </summary>
        internal IntPtr target;

        /// <summary>
        /// The property
        /// </summary>
        internal IntPtr property;

        /// <summary>
        /// The time
        /// </summary>
        internal IntPtr time;
    }

    /// <summary>
    /// Struct XSetWindowAttributes
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XSetWindowAttributes
    {
        /// <summary>
        /// The background pixmap
        /// </summary>
        internal IntPtr background_pixmap;

        /// <summary>
        /// The background pixel
        /// </summary>
        internal IntPtr background_pixel;

        /// <summary>
        /// The border pixmap
        /// </summary>
        internal IntPtr border_pixmap;

        /// <summary>
        /// The border pixel
        /// </summary>
        internal IntPtr border_pixel;

        /// <summary>
        /// The bit gravity
        /// </summary>
        internal Gravity bit_gravity;

        /// <summary>
        /// The win gravity
        /// </summary>
        internal Gravity win_gravity;

        /// <summary>
        /// The backing store
        /// </summary>
        internal int backing_store;

        /// <summary>
        /// The backing planes
        /// </summary>
        internal IntPtr backing_planes;

        /// <summary>
        /// The backing pixel
        /// </summary>
        internal IntPtr backing_pixel;

        /// <summary>
        /// The save under
        /// </summary>
        internal bool save_under;

        /// <summary>
        /// The event mask
        /// </summary>
        internal IntPtr event_mask;

        /// <summary>
        /// The do not propagate mask
        /// </summary>
        internal IntPtr do_not_propagate_mask;

        /// <summary>
        /// The override redirect
        /// </summary>
        internal bool override_redirect;

        /// <summary>
        /// The colormap
        /// </summary>
        internal IntPtr colormap;

        /// <summary>
        /// The cursor
        /// </summary>
        internal IntPtr cursor;
    }

    /// <summary>
    /// Struct XSizeHints
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XSizeHints
    {
        /// <summary>
        /// The flags
        /// </summary>
        internal IntPtr flags;

        /// <summary>
        /// The x
        /// </summary>
        internal int x;

        /// <summary>
        /// The y
        /// </summary>
        internal int y;

        /// <summary>
        /// The width
        /// </summary>
        internal int width;

        /// <summary>
        /// The height
        /// </summary>
        internal int height;

        /// <summary>
        /// The minimum width
        /// </summary>
        internal int min_width;

        /// <summary>
        /// The minimum height
        /// </summary>
        internal int min_height;

        /// <summary>
        /// The maximum width
        /// </summary>
        internal int max_width;

        /// <summary>
        /// The maximum height
        /// </summary>
        internal int max_height;

        /// <summary>
        /// The width inc
        /// </summary>
        internal int width_inc;

        /// <summary>
        /// The height inc
        /// </summary>
        internal int height_inc;

        /// <summary>
        /// The minimum aspect x
        /// </summary>
        internal int min_aspect_x;

        /// <summary>
        /// The minimum aspect y
        /// </summary>
        internal int min_aspect_y;

        /// <summary>
        /// The maximum aspect x
        /// </summary>
        internal int max_aspect_x;

        /// <summary>
        /// The maximum aspect y
        /// </summary>
        internal int max_aspect_y;

        /// <summary>
        /// The base width
        /// </summary>
        internal int base_width;

        /// <summary>
        /// The base height
        /// </summary>
        internal int base_height;

        /// <summary>
        /// The win gravity
        /// </summary>
        internal int win_gravity;
    }

    /// <summary>
    /// Struct XStandardColormap
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XStandardColormap
    {
        /// <summary>
        /// The colormap
        /// </summary>
        internal IntPtr colormap;

        /// <summary>
        /// The red maximum
        /// </summary>
        internal IntPtr red_max;

        /// <summary>
        /// The red mult
        /// </summary>
        internal IntPtr red_mult;

        /// <summary>
        /// The green maximum
        /// </summary>
        internal IntPtr green_max;

        /// <summary>
        /// The green mult
        /// </summary>
        internal IntPtr green_mult;

        /// <summary>
        /// The blue maximum
        /// </summary>
        internal IntPtr blue_max;

        /// <summary>
        /// The blue mult
        /// </summary>
        internal IntPtr blue_mult;

        /// <summary>
        /// The base pixel
        /// </summary>
        internal IntPtr base_pixel;

        /// <summary>
        /// The visualid
        /// </summary>
        internal IntPtr visualid;

        /// <summary>
        /// The killid
        /// </summary>
        internal IntPtr killid;
    }

    /// <summary>
    /// Struct XTextProperty
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XTextProperty
    {
        /// <summary>
        /// The value
        /// </summary>
        internal string value;

        /// <summary>
        /// The encoding
        /// </summary>
        internal IntPtr encoding;

        /// <summary>
        /// The format
        /// </summary>
        internal int format;

        /// <summary>
        /// The nitems
        /// </summary>
        internal IntPtr nitems;
    }

    /// <summary>
    /// Struct XUnmapEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XUnmapEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The xevent
        /// </summary>
        internal IntPtr xevent;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// From configure
        /// </summary>
        internal bool from_configure;
    }

    /// <summary>
    /// Struct XVisibilityEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XVisibilityEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        internal XEventName type;

        /// <summary>
        /// The serial
        /// </summary>
        internal IntPtr serial;

        /// <summary>
        /// The send event
        /// </summary>
        internal bool send_event;

        /// <summary>
        /// The display
        /// </summary>
        internal IntPtr display;

        /// <summary>
        /// The window
        /// </summary>
        internal IntPtr window;

        /// <summary>
        /// The state
        /// </summary>
        internal int state;
    }

    /// <summary>
    /// Struct XVisualInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XVisualInfo
    {
        /// <summary>
        /// The visual
        /// </summary>
        internal IntPtr visual;

        /// <summary>
        /// The visualid
        /// </summary>
        internal IntPtr visualid;

        /// <summary>
        /// The screen
        /// </summary>
        internal int screen;

        /// <summary>
        /// The depth
        /// </summary>
        internal uint depth;

        /// <summary>
        /// The klass
        /// </summary>
        internal int klass;

        /// <summary>
        /// The red mask
        /// </summary>
        internal IntPtr red_mask;

        /// <summary>
        /// The green mask
        /// </summary>
        internal IntPtr green_mask;

        /// <summary>
        /// The blue mask
        /// </summary>
        internal IntPtr blue_mask;

        /// <summary>
        /// The colormap size
        /// </summary>
        internal int colormap_size;

        /// <summary>
        /// The bits per RGB
        /// </summary>
        internal int bits_per_rgb;
    }

    /// <summary>
    /// Struct XWindowAttributes
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XWindowAttributes
    {
        /// <summary>
        /// The x
        /// </summary>
        internal int x;

        /// <summary>
        /// The y
        /// </summary>
        internal int y;

        /// <summary>
        /// The width
        /// </summary>
        internal int width;

        /// <summary>
        /// The height
        /// </summary>
        internal int height;

        /// <summary>
        /// The border width
        /// </summary>
        internal int border_width;

        /// <summary>
        /// The depth
        /// </summary>
        internal int depth;

        /// <summary>
        /// The visual
        /// </summary>
        internal IntPtr visual;

        /// <summary>
        /// The root
        /// </summary>
        internal IntPtr root;

        /// <summary>
        /// The c class
        /// </summary>
        internal int c_class;

        /// <summary>
        /// The bit gravity
        /// </summary>
        internal Gravity bit_gravity;

        /// <summary>
        /// The win gravity
        /// </summary>
        internal Gravity win_gravity;

        /// <summary>
        /// The backing store
        /// </summary>
        internal int backing_store;

        /// <summary>
        /// The backing planes
        /// </summary>
        internal IntPtr backing_planes;

        /// <summary>
        /// The backing pixel
        /// </summary>
        internal IntPtr backing_pixel;

        /// <summary>
        /// The save under
        /// </summary>
        internal bool save_under;

        /// <summary>
        /// The colormap
        /// </summary>
        internal IntPtr colormap;

        /// <summary>
        /// The map installed
        /// </summary>
        internal bool map_installed;

        /// <summary>
        /// The map state
        /// </summary>
        internal MapState map_state;

        /// <summary>
        /// All event masks
        /// </summary>
        internal IntPtr all_event_masks;

        /// <summary>
        /// Your event mask
        /// </summary>
        internal IntPtr your_event_mask;

        /// <summary>
        /// The do not propagate mask
        /// </summary>
        internal IntPtr do_not_propagate_mask;

        /// <summary>
        /// The override direct
        /// </summary>
        internal bool override_direct;

        /// <summary>
        /// The screen
        /// </summary>
        internal IntPtr screen;

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return XEvent.ToString(this);
        }
    }

    /// <summary>
    /// Struct XWindowChanges
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XWindowChanges
    {
        /// <summary>
        /// The x
        /// </summary>
        internal int x;

        /// <summary>
        /// The y
        /// </summary>
        internal int y;

        /// <summary>
        /// The width
        /// </summary>
        internal int width;

        /// <summary>
        /// The height
        /// </summary>
        internal int height;

        /// <summary>
        /// The border width
        /// </summary>
        internal int border_width;

        /// <summary>
        /// The sibling
        /// </summary>
        internal IntPtr sibling;

        /// <summary>
        /// The stack mode
        /// </summary>
        internal StackMode stack_mode;
    }

    /// <summary>
    /// Struct XWMHints
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XWMHints
    {
        /// <summary>
        /// The flags
        /// </summary>
        internal IntPtr flags;

        /// <summary>
        /// The input
        /// </summary>
        internal bool input;

        /// <summary>
        /// The initial state
        /// </summary>
        internal XInitialState initial_state;

        /// <summary>
        /// The icon pixmap
        /// </summary>
        internal IntPtr icon_pixmap;

        /// <summary>
        /// The icon window
        /// </summary>
        internal IntPtr icon_window;

        /// <summary>
        /// The icon x
        /// </summary>
        internal int icon_x;

        /// <summary>
        /// The icon y
        /// </summary>
        internal int icon_y;

        /// <summary>
        /// The icon mask
        /// </summary>
        internal IntPtr icon_mask;

        /// <summary>
        /// The window group
        /// </summary>
        internal IntPtr window_group;
    }

    /// <summary>
    /// Class XNames.
    /// </summary>
    internal static class XNames
    {
        #region Fields

        /// <summary>
        /// The xn area
        /// </summary>
        public const string XNArea = "area";

        /// <summary>
        /// The xn area needed
        /// </summary>
        public const string XNAreaNeeded = "areaNeeded";

        /// <summary>
        /// The xn client window
        /// </summary>
        public const string XNClientWindow = "clientWindow";

        /// <summary>
        /// The xn focus window
        /// </summary>
        public const string XNFocusWindow = "focusWindow";

        /// <summary>
        /// The xn font set
        /// </summary>
        public const string XNFontSet = "fontSet";

        /// <summary>
        /// The xn input style
        /// </summary>
        public const string XNInputStyle = "inputStyle";

        /// <summary>
        /// The xn preedit attributes
        /// </summary>
        public const string XNPreeditAttributes = "preeditAttributes";

        /// <summary>
        /// The xn preedit caret callback
        /// </summary>
        public const string XNPreeditCaretCallback = "preeditCaretCallback";

        /// <summary>
        /// The xn preedit done callback
        /// </summary>
        public const string XNPreeditDoneCallback = "preeditDoneCallback";

        /// <summary>
        /// The xn preedit draw callback
        /// </summary>
        public const string XNPreeditDrawCallback = "preeditDrawCallback";

        // XIMPreeditCallbacks delegate names.
        /// <summary>
        /// The xn preedit start callback
        /// </summary>
        public const string XNPreeditStartCallback = "preeditStartCallback";

        /// <summary>
        /// The xn preedit state notify callback
        /// </summary>
        public const string XNPreeditStateNotifyCallback = "preeditStateNotifyCallback";

        /// <summary>
        /// The xn query input style
        /// </summary>
        public const string XNQueryInputStyle = "queryInputStyle";

        /// <summary>
        /// The xn spot location
        /// </summary>
        public const string XNSpotLocation = "spotLocation";

        /// <summary>
        /// The xn status attributes
        /// </summary>
        public const string XNStatusAttributes = "statusAttributes";

        /// <summary>
        /// The xn status done callback
        /// </summary>
        public const string XNStatusDoneCallback = "statusDoneCallback";

        /// <summary>
        /// The xn status draw callback
        /// </summary>
        public const string XNStatusDrawCallback = "statusDrawCallback";

        // XIMStatusCallbacks delegate names.
        /// <summary>
        /// The xn status start callback
        /// </summary>
        public const string XNStatusStartCallback = "statusStartCallback";

        /// <summary>
        /// The xn va nested list
        /// </summary>
        public const string XNVaNestedList = "XNVaNestedList";

        #endregion Fields
    }

    /// <summary>
    /// Class XIMCallback.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    internal class XIMCallback
    {
        /// <summary>
        /// The client data
        /// </summary>
        public IntPtr client_data;

        /// <summary>
        /// The callback
        /// </summary>
        public XIMProc callback;

        /// <summary>
        /// The GCH
        /// </summary>
        [NonSerialized]
        private GCHandle gch;

        /// <summary>
        /// Initializes a new instance of the <see cref="XIMCallback"/> class.
        /// </summary>
        /// <param name="clientData">The client data.</param>
        /// <param name="proc">The proc.</param>
        public XIMCallback(IntPtr clientData, XIMProc proc)
        {
            this.client_data = clientData;
            this.gch = GCHandle.Alloc(proc);
            this.callback = proc;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="XIMCallback"/> class.
        /// </summary>
        ~XIMCallback()
        {
            gch.Free();
        }
    }

    /// <summary>
    /// Class XPoint.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    internal class XPoint
    {
        /// <summary>
        /// The x
        /// </summary>
        public short X;

        /// <summary>
        /// The y
        /// </summary>
        public short Y;
    }
}
