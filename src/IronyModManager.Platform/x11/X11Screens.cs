// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="X11Screens.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Platform;
using static IronyModManager.Platform.x11.XLib;

namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Interface IX11Screens
    /// </summary>
    internal interface IX11Screens
    {
        #region Properties

        /// <summary>
        /// Gets the screens.
        /// </summary>
        /// <value>The screens.</value>
        X11Screen[] Screens { get; }

        #endregion Properties
    }

    /// <summary>
    /// Class X11Screen.
    /// </summary>
    internal class X11Screen
    {
        #region Fields

        /// <summary>
        /// The full hd width
        /// </summary>
        private const int FullHDWidth = 1920;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="X11Screen" /> class.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="primary">if set to <c>true</c> [primary].</param>
        /// <param name="name">The name.</param>
        /// <param name="physicalSize">Size of the physical.</param>
        /// <param name="pixelDensity">The pixel density.</param>
        public X11Screen(PixelRect bounds, bool primary,
            string name, Size? physicalSize, double? pixelDensity)
        {
            Primary = primary;
            Name = name;
            Bounds = bounds;
            if (physicalSize == null && pixelDensity == null)
            {
                PixelDensity = 1;
            }
            else if (pixelDensity == null)
            {
                PixelDensity = GuessPixelDensity(bounds.Width, physicalSize.Value.Width);
            }
            else
            {
                PixelDensity = pixelDensity.Value;
                PhysicalSize = physicalSize;
            }
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>The bounds.</value>
        public PixelRect Bounds { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the size of the physical.
        /// </summary>
        /// <value>The size of the physical.</value>
        public Size? PhysicalSize { get; set; }

        /// <summary>
        /// Gets or sets the pixel density.
        /// </summary>
        /// <value>The pixel density.</value>
        public double PixelDensity { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="X11Screen" /> is primary.
        /// </summary>
        /// <value><c>true</c> if primary; otherwise, <c>false</c>.</value>
        public bool Primary { get; }

        /// <summary>
        /// Gets or sets the working area.
        /// </summary>
        /// <value>The working area.</value>
        public PixelRect WorkingArea { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Guesses the pixel density.
        /// </summary>
        /// <param name="pixelWidth">Width of the pixel.</param>
        /// <param name="mmWidth">Width of the mm.</param>
        /// <returns>System.Double.</returns>
        public static double GuessPixelDensity(double pixelWidth, double mmWidth)
            => pixelWidth <= FullHDWidth ? 1 : Math.Max(1, Math.Round(pixelWidth / mmWidth * 25.4 / 96));

        #endregion Methods
    }

    /// <summary>
    /// Class X11Screens.
    /// Implements the <see cref="Avalonia.Platform.IScreenImpl" />
    /// </summary>
    /// <seealso cref="Avalonia.Platform.IScreenImpl" />
    internal class X11Screens : IScreenImpl
    {
        #region Fields

        /// <summary>
        /// The implementation
        /// </summary>
        private readonly IX11Screens _impl;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="X11Screens" /> class.
        /// </summary>
        /// <param name="impl">The implementation.</param>
        public X11Screens(IX11Screens impl)
        {
            _impl = impl;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets all screens.
        /// </summary>
        /// <value>All screens.</value>
        public IReadOnlyList<Screen> AllScreens =>
            _impl.Screens.Select(s => new Screen(s.PixelDensity, s.Bounds, s.WorkingArea, s.Primary)).ToArray();

        /// <summary>
        /// Gets the screen count.
        /// </summary>
        /// <value>The screen count.</value>
        public int ScreenCount => _impl.Screens.Length;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initializes the specified platform.
        /// </summary>
        /// <param name="platform">The platform.</param>
        /// <returns>IX11Screens.</returns>
        public static IX11Screens Init(AvaloniaX11Platform platform)
        {
            var info = platform.Info;
            var settings = X11ScreensUserSettings.Detect();
            var impl = (info.RandrVersion != null && info.RandrVersion >= new Version(1, 5))
                ? new Randr15ScreensImpl(platform, settings)
                : (IX11Screens)new FallbackScreensImpl(info, settings);

            return impl;
        }

        /// <summary>
        /// Updates the work area.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="screens">The screens.</param>
        /// <returns>X11Screen[].</returns>
        private static unsafe X11Screen[] UpdateWorkArea(X11Info info, X11Screen[] screens)
        {
            var rect = default(PixelRect);
            foreach (var s in screens)
            {
                rect = rect.Union(s.Bounds);
                //Fallback value
                s.WorkingArea = s.Bounds;
            }

            var res = XGetWindowProperty(info.Display,
                info.RootWindow,
                info.Atoms._NET_WORKAREA,
                IntPtr.Zero,
                new IntPtr(128),
                false,
                info.Atoms.AnyPropertyType,
                out var type,
                out var format,
                out var count,
                out var bytesAfter,
                out var prop);

            if (res != (int)Status.Success || type == IntPtr.Zero ||
                format == 0 || bytesAfter.ToInt64() != 0 || count.ToInt64() % 4 != 0)
                return screens;

            var pwa = (IntPtr*)prop;
            var wa = new PixelRect(pwa[0].ToInt32(), pwa[1].ToInt32(), pwa[2].ToInt32(), pwa[3].ToInt32());

            foreach (var s in screens)
            {
                s.WorkingArea = s.Bounds.Intersect(wa);
                if (s.WorkingArea.Width <= 0 || s.WorkingArea.Height <= 0)
                    s.WorkingArea = s.Bounds;
            }

            XFree(prop);
            return screens;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class FallbackScreensImpl.
        /// Implements the <see cref="IronyModManager.Platform.x11.IX11Screens" />
        /// </summary>
        /// <seealso cref="IronyModManager.Platform.x11.IX11Screens" />
        private class FallbackScreensImpl : IX11Screens
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="FallbackScreensImpl" /> class.
            /// </summary>
            /// <param name="info">The information.</param>
            /// <param name="settings">The settings.</param>
            public FallbackScreensImpl(X11Info info, X11ScreensUserSettings settings)
            {
                if (XGetGeometry(info.Display, info.RootWindow, out var geo))
                {
                    Screens = UpdateWorkArea(info,
                        new[]
                        {
                            new X11Screen(new PixelRect(0, 0, geo.width, geo.height), true, "Default", null,
                                settings.GlobalScaleFactor)
                        });
                }
                else
                {
                    Screens = new[]
                    {
                        new X11Screen(new PixelRect(0, 0, 1920, 1280), true, "Default", null,
                            settings.GlobalScaleFactor)
                    };
                }
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets the screens.
            /// </summary>
            /// <value>The screens.</value>
            public X11Screen[] Screens { get; }

            #endregion Properties
        }

        /// <summary>
        /// Class Randr15ScreensImpl.
        /// Implements the <see cref="IronyModManager.Platform.x11.IX11Screens" />
        /// </summary>
        /// <seealso cref="IronyModManager.Platform.x11.IX11Screens" />
        private class Randr15ScreensImpl : IX11Screens
        {
            #region Fields

            /// <summary>
            /// The settings
            /// </summary>
            private readonly X11ScreensUserSettings _settings;

            /// <summary>
            /// The window
            /// </summary>
            private readonly IntPtr _window;

            /// <summary>
            /// The X11
            /// </summary>
            private readonly X11Info _x11;

            /// <summary>
            /// The cache
            /// </summary>
            private X11Screen[] _cache;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Randr15ScreensImpl" /> class.
            /// </summary>
            /// <param name="platform">The platform.</param>
            /// <param name="settings">The settings.</param>
            public Randr15ScreensImpl(AvaloniaX11Platform platform, X11ScreensUserSettings settings)
            {
                _settings = settings;
                _x11 = platform.Info;
                _window = CreateEventWindow(platform, OnEvent);
                XRRSelectInput(_x11.Display, _window, RandrEventMask.RRScreenChangeNotify);
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets the screens.
            /// </summary>
            /// <value>The screens.</value>
            public unsafe X11Screen[] Screens
            {
                get
                {
                    if (_cache != null)
                        return _cache;
                    var monitors = XRRGetMonitors(_x11.Display, _window, true, out var count);

                    var screens = new X11Screen[count];
                    for (var c = 0; c < count; c++)
                    {
                        var mon = monitors[c];
                        var namePtr = XGetAtomName(_x11.Display, mon.Name);
                        var name = Marshal.PtrToStringAnsi(namePtr);
                        XFree(namePtr);

                        var density = 1d;
                        if (_settings.NamedScaleFactors?.TryGetValue(name, out density) != true)
                        {
                            if (mon.MWidth == 0)
                                density = 1;
                            else
                                density = X11Screen.GuessPixelDensity(mon.Width, mon.MWidth);
                        }

                        density *= _settings.GlobalScaleFactor;

                        var bounds = new PixelRect(mon.X, mon.Y, mon.Width, mon.Height);
                        screens[c] = new X11Screen(bounds,
                            mon.Primary != 0,
                            name,
                            (mon.MWidth == 0 || mon.MHeight == 0) ? (Size?)null : new Size(mon.MWidth, mon.MHeight),
                            density);
                    }

                    XFree(new IntPtr(monitors));
                    _cache = UpdateWorkArea(_x11, screens);
                    return screens;
                }
            }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Called when [event].
            /// </summary>
            /// <param name="ev">The ev.</param>
            private void OnEvent(XEvent ev)
            {
                // Invalidate cache on RRScreenChangeNotify
                if ((int)ev.type == _x11.RandrEventBase + (int)RandrEvent.RRScreenChangeNotify)
                    _cache = null;
            }

            #endregion Methods
        }

        #endregion Classes
    }

    /// <summary>
    /// Class X11ScreensUserSettings.
    /// </summary>
    internal class X11ScreensUserSettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets the global scale factor.
        /// </summary>
        /// <value>The global scale factor.</value>
        public double GlobalScaleFactor { get; set; } = 1;

        /// <summary>
        /// Gets or sets the named scale factors.
        /// </summary>
        /// <value>The named scale factors.</value>
        public Dictionary<string, double> NamedScaleFactors { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Detects this instance.
        /// </summary>
        /// <returns>X11ScreensUserSettings.</returns>
        public static X11ScreensUserSettings Detect()
        {
            return DetectEnvironment() ?? new X11ScreensUserSettings();
        }

        /// <summary>
        /// Detects the environment.
        /// </summary>
        /// <returns>X11ScreensUserSettings.</returns>
        public static X11ScreensUserSettings DetectEnvironment()
        {
            var globalFactor = Environment.GetEnvironmentVariable("AVALONIA_GLOBAL_SCALE_FACTOR");
            var screenFactors = Environment.GetEnvironmentVariable("AVALONIA_SCREEN_SCALE_FACTORS");
            if (globalFactor == null && screenFactors == null)
                return null;

            var rv = new X11ScreensUserSettings
            {
                GlobalScaleFactor = TryParse(globalFactor) ?? 1
            };

            try
            {
                if (!string.IsNullOrWhiteSpace(screenFactors))
                {
                    rv.NamedScaleFactors = screenFactors.Split(';').Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Split('=')).ToDictionary(x => x[0],
                            x => double.Parse(x[1], CultureInfo.InvariantCulture));
                }
            }
            catch
            {
                //Ignore
            }

            return rv;
        }

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>System.Nullable&lt;System.Double&gt;.</returns>
        private static double? TryParse(string s)
        {
            if (s == null)
                return null;
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var rv))
                return rv;
            return null;
        }

        #endregion Methods
    }
}
