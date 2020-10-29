// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="X11Enums.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ***********************************************************************
using System;

namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Enum Status
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// The success
        /// </summary>
        Success = 0, /* everything's okay */

        /// <summary>
        /// The bad request
        /// </summary>
        BadRequest = 1, /* bad request code */

        /// <summary>
        /// The bad value
        /// </summary>
        BadValue = 2, /* int parameter out of range */

        /// <summary>
        /// The bad window
        /// </summary>
        BadWindow = 3, /* parameter not a Window */

        /// <summary>
        /// The bad pixmap
        /// </summary>
        BadPixmap = 4, /* parameter not a Pixmap */

        /// <summary>
        /// The bad atom
        /// </summary>
        BadAtom = 5, /* parameter not an Atom */

        /// <summary>
        /// The bad cursor
        /// </summary>
        BadCursor = 6, /* parameter not a Cursor */

        /// <summary>
        /// The bad font
        /// </summary>
        BadFont = 7, /* parameter not a Font */

        /// <summary>
        /// The bad match
        /// </summary>
        BadMatch = 8, /* parameter mismatch */

        /// <summary>
        /// The bad drawable
        /// </summary>
        BadDrawable = 9, /* parameter not a Pixmap or Window */

        /// <summary>
        /// The bad access
        /// </summary>
        BadAccess = 10, /* depending on context:

                                 - key/button already grabbed
                                 - attempt to free an illegal
                                   cmap entry
                                - attempt to store into a read-only
                                   color map entry.
                                - attempt to modify the access control
                                   list from other than the local host.
                                */

        /// <summary>
        /// The bad alloc
        /// </summary>
        BadAlloc = 11, /* insufficient resources */

        /// <summary>
        /// The bad color
        /// </summary>
        BadColor = 12, /* no such colormap */

        /// <summary>
        /// The bad gc
        /// </summary>
        BadGC = 13, /* parameter not a GC */

        /// <summary>
        /// The bad identifier choice
        /// </summary>
        BadIDChoice = 14, /* choice not in range or already used */

        /// <summary>
        /// The bad name
        /// </summary>
        BadName = 15, /* font or color name doesn't exist */

        /// <summary>
        /// The bad length
        /// </summary>
        BadLength = 16, /* Request length incorrect */

        /// <summary>
        /// The bad implementation
        /// </summary>
        BadImplementation = 17, /* server is defective */

        /// <summary>
        /// The first extension error
        /// </summary>
        FirstExtensionError = 128,

        /// <summary>
        /// The last extension error
        /// </summary>
        LastExtensionError = 255,
    }

    /// <summary>
    /// Enum XCreateWindowFlags
    /// </summary>
    [Flags]
    public enum XCreateWindowFlags
    {
        /// <summary>
        /// The cw back pixmap
        /// </summary>
        CWBackPixmap = (1 << 0),

        /// <summary>
        /// The cw back pixel
        /// </summary>
        CWBackPixel = (1 << 1),

        /// <summary>
        /// The cw border pixmap
        /// </summary>
        CWBorderPixmap = (1 << 2),

        /// <summary>
        /// The cw border pixel
        /// </summary>
        CWBorderPixel = (1 << 3),

        /// <summary>
        /// The cw bit gravity
        /// </summary>
        CWBitGravity = (1 << 4),

        /// <summary>
        /// The cw win gravity
        /// </summary>
        CWWinGravity = (1 << 5),

        /// <summary>
        /// The cw backing store
        /// </summary>
        CWBackingStore = (1 << 6),

        /// <summary>
        /// The cw backing planes
        /// </summary>
        CWBackingPlanes = (1 << 7),

        /// <summary>
        /// The cw backing pixel
        /// </summary>
        CWBackingPixel = (1 << 8),

        /// <summary>
        /// The cw override redirect
        /// </summary>
        CWOverrideRedirect = (1 << 9),

        /// <summary>
        /// The cw save under
        /// </summary>
        CWSaveUnder = (1 << 10),

        /// <summary>
        /// The cw event mask
        /// </summary>
        CWEventMask = (1 << 11),

        /// <summary>
        /// The cw dont propagate
        /// </summary>
        CWDontPropagate = (1 << 12),

        /// <summary>
        /// The cw colormap
        /// </summary>
        CWColormap = (1 << 13),

        /// <summary>
        /// The cw cursor
        /// </summary>
        CWCursor = (1 << 14),
    }

    /// <summary>
    /// Enum XEventMask
    /// </summary>
    [Flags]
    public enum XEventMask : int
    {
        /// <summary>
        /// The no event mask
        /// </summary>
        NoEventMask = 0,

        /// <summary>
        /// The key press mask
        /// </summary>
        KeyPressMask = (1 << 0),

        /// <summary>
        /// The key release mask
        /// </summary>
        KeyReleaseMask = (1 << 1),

        /// <summary>
        /// The button press mask
        /// </summary>
        ButtonPressMask = (1 << 2),

        /// <summary>
        /// The button release mask
        /// </summary>
        ButtonReleaseMask = (1 << 3),

        /// <summary>
        /// The enter window mask
        /// </summary>
        EnterWindowMask = (1 << 4),

        /// <summary>
        /// The leave window mask
        /// </summary>
        LeaveWindowMask = (1 << 5),

        /// <summary>
        /// The pointer motion mask
        /// </summary>
        PointerMotionMask = (1 << 6),

        /// <summary>
        /// The pointer motion hint mask
        /// </summary>
        PointerMotionHintMask = (1 << 7),

        /// <summary>
        /// The button1 motion mask
        /// </summary>
        Button1MotionMask = (1 << 8),

        /// <summary>
        /// The button2 motion mask
        /// </summary>
        Button2MotionMask = (1 << 9),

        /// <summary>
        /// The button3 motion mask
        /// </summary>
        Button3MotionMask = (1 << 10),

        /// <summary>
        /// The button4 motion mask
        /// </summary>
        Button4MotionMask = (1 << 11),

        /// <summary>
        /// The button5 motion mask
        /// </summary>
        Button5MotionMask = (1 << 12),

        /// <summary>
        /// The button motion mask
        /// </summary>
        ButtonMotionMask = (1 << 13),

        /// <summary>
        /// The keymap state mask
        /// </summary>
        KeymapStateMask = (1 << 14),

        /// <summary>
        /// The exposure mask
        /// </summary>
        ExposureMask = (1 << 15),

        /// <summary>
        /// The visibility change mask
        /// </summary>
        VisibilityChangeMask = (1 << 16),

        /// <summary>
        /// The structure notify mask
        /// </summary>
        StructureNotifyMask = (1 << 17),

        /// <summary>
        /// The resize redirect mask
        /// </summary>
        ResizeRedirectMask = (1 << 18),

        /// <summary>
        /// The substructure notify mask
        /// </summary>
        SubstructureNotifyMask = (1 << 19),

        /// <summary>
        /// The substructure redirect mask
        /// </summary>
        SubstructureRedirectMask = (1 << 20),

        /// <summary>
        /// The focus change mask
        /// </summary>
        FocusChangeMask = (1 << 21),

        /// <summary>
        /// The property change mask
        /// </summary>
        PropertyChangeMask = (1 << 22),

        /// <summary>
        /// The colormap change mask
        /// </summary>
        ColormapChangeMask = (1 << 23),

        /// <summary>
        /// The owner grab button mask
        /// </summary>
        OwnerGrabButtonMask = (1 << 24)
    }

    /// <summary>
    /// Enum XModifierMask
    /// </summary>
    [Flags]
    public enum XModifierMask
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

        /// <summary>
        /// Any modifier
        /// </summary>
        AnyModifier = (1 << 15)
    }
}
