// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="X11Info.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ************************************************************************

using System;
using System.Linq;
using IronyModManager.Shared;
using static IronyModManager.Platform.x11.XLib;

// ReSharper disable UnusedAutoPropertyAccessor.Local
namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Class X11Info.
    /// </summary>
    [ExcludeFromCoverage("External component.")]
    internal unsafe class X11Info
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="X11Info"/> class.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="deferredDisplay">The deferred display.</param>
        public unsafe X11Info(IntPtr display, IntPtr deferredDisplay)
        {
            Display = display;
            DeferredDisplay = deferredDisplay;
            DefaultScreen = XDefaultScreen(display);
            BlackPixel = XBlackPixel(display, DefaultScreen);
            RootWindow = XRootWindow(display, DefaultScreen);
            DefaultCursor = XCreateFontCursor(display, CursorFontShape.XC_top_left_arrow);
            DefaultRootWindow = XDefaultRootWindow(display);
            Atoms = new X11Atoms(display);
            //TODO: Open an actual XIM once we get support for preedit in our textbox
            XSetLocaleModifiers("@im=none");
            Xim = XOpenIM(display, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            XMatchVisualInfo(Display, DefaultScreen, 32, 4, out var visual);
            if (visual.depth == 32)
                TransparentVisualInfo = visual;

            try
            {
                if (XRRQueryExtension(display, out int randrEventBase, out var randrErrorBase) != 0)
                {
                    RandrEventBase = randrEventBase;
                    RandrErrorBase = randrErrorBase;
                    if (XRRQueryVersion(display, out var major, out var minor) != 0)
                        RandrVersion = new Version(major, minor);
                }
            }
            catch
            {
                //Ignore, randr is not supported
            }

            try
            {
                if (XQueryExtension(display, "XInputExtension",
                        out var xiopcode, out var xievent, out var xierror))
                {
                    int major = 2, minor = 2;
                    if (XIQueryVersion(display, ref major, ref minor) == Status.Success)
                    {
                        XInputVersion = new Version(major, minor);
                        XInputOpcode = xiopcode;
                        XInputEventBase = xievent;
                        XInputErrorBase = xierror;
                    }
                }
            }
            catch
            {
                //Ignore, XI is not supported
            }
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the atoms.
        /// </summary>
        /// <value>The atoms.</value>
        public X11Atoms Atoms { get; }

        /// <summary>
        /// Gets the black pixel.
        /// </summary>
        /// <value>The black pixel.</value>
        public IntPtr BlackPixel { get; }

        /// <summary>
        /// Gets the default cursor.
        /// </summary>
        /// <value>The default cursor.</value>
        public IntPtr DefaultCursor { get; }

        /// <summary>
        /// Gets the default root window.
        /// </summary>
        /// <value>The default root window.</value>
        public IntPtr DefaultRootWindow { get; }

        /// <summary>
        /// Gets the default screen.
        /// </summary>
        /// <value>The default screen.</value>
        public int DefaultScreen { get; }

        /// <summary>
        /// Gets the deferred display.
        /// </summary>
        /// <value>The deferred display.</value>
        public IntPtr DeferredDisplay { get; }

        /// <summary>
        /// Gets the display.
        /// </summary>
        /// <value>The display.</value>
        public IntPtr Display { get; }

        /// <summary>
        /// Gets or sets the last activity timestamp.
        /// </summary>
        /// <value>The last activity timestamp.</value>
        public IntPtr LastActivityTimestamp { get; set; }

        /// <summary>
        /// Gets the randr error base.
        /// </summary>
        /// <value>The randr error base.</value>
        public int RandrErrorBase { get; }

        /// <summary>
        /// Gets the randr event base.
        /// </summary>
        /// <value>The randr event base.</value>
        public int RandrEventBase { get; }

        /// <summary>
        /// Gets the randr version.
        /// </summary>
        /// <value>The randr version.</value>
        public Version RandrVersion { get; }

        /// <summary>
        /// Gets the root window.
        /// </summary>
        /// <value>The root window.</value>
        public IntPtr RootWindow { get; }

        /// <summary>
        /// Gets or sets the transparent visual information.
        /// </summary>
        /// <value>The transparent visual information.</value>
        public XVisualInfo? TransparentVisualInfo { get; set; }

        /// <summary>
        /// Gets the xim.
        /// </summary>
        /// <value>The xim.</value>
        public IntPtr Xim { get; }

        /// <summary>
        /// Gets the x input error base.
        /// </summary>
        /// <value>The x input error base.</value>
        public int XInputErrorBase { get; }

        /// <summary>
        /// Gets the x input event base.
        /// </summary>
        /// <value>The x input event base.</value>
        public int XInputEventBase { get; }

        /// <summary>
        /// Gets the x input opcode.
        /// </summary>
        /// <value>The x input opcode.</value>
        public int XInputOpcode { get; }

        /// <summary>
        /// Gets the x input version.
        /// </summary>
        /// <value>The x input version.</value>
        public Version XInputVersion { get; }

        #endregion Properties
    }
}
