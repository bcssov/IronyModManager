// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="X11Window.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.FreeDesktop;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.OpenGL;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Threading;
using IronyModManager.Platform.x11.Glx;
using IronyModManager.Shared;
using static IronyModManager.Platform.x11.XLib;

// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo
namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Class X11Window.
    /// Implements the <see cref="Avalonia.Platform.IWindowImpl" />
    /// Implements the <see cref="Avalonia.Platform.IPopupImpl" />
    /// Implements the <see cref="IronyModManager.Platform.x11.IXI2Client" />
    /// Implements the <see cref="Avalonia.Controls.Platform.ITopLevelImplWithNativeMenuExporter" />
    /// </summary>
    /// <seealso cref="Avalonia.Platform.IWindowImpl" />
    /// <seealso cref="Avalonia.Platform.IPopupImpl" />
    /// <seealso cref="IronyModManager.Platform.x11.IXI2Client" />
    /// <seealso cref="Avalonia.Controls.Platform.ITopLevelImplWithNativeMenuExporter" />
    [ExcludeFromCoverage("External component.")]
    internal unsafe class X11Window : IWindowImpl, IPopupImpl, IXI2Client, ITopLevelImplWithNativeMenuExporter
    {
        #region Fields

        /// <summary>
        /// The maximum window dimension
        /// </summary>
        private const int MaxWindowDimension = 100000;

        /// <summary>
        /// The input queue
        /// </summary>
        private readonly Queue<InputEventContainer> _inputQueue = new Queue<InputEventContainer>();

        /// <summary>
        /// The keyboard
        /// </summary>
        private readonly IKeyboardDevice _keyboard;

        /// <summary>
        /// The mouse
        /// </summary>
        private readonly MouseDevice _mouse;

        /// <summary>
        /// The platform
        /// </summary>
        private readonly AvaloniaX11Platform _platform;

        /// <summary>
        /// The popup
        /// </summary>
        private readonly bool _popup;

        /// <summary>
        /// The popup parent
        /// </summary>
        private readonly IWindowImpl _popupParent;

        /// <summary>
        /// The touch
        /// </summary>
        private readonly TouchDevice _touch;

        /// <summary>
        /// The transient children
        /// </summary>
        private readonly HashSet<X11Window> _transientChildren = new HashSet<X11Window>();

        /// <summary>
        /// The use render window
        /// </summary>
        private readonly bool _useRenderWindow = false;

        /// <summary>
        /// The X11
        /// </summary>
        private readonly X11Info _x11;

        /// <summary>
        /// The can resize
        /// </summary>
        private bool _canResize = true;

        /// <summary>
        /// The configure
        /// </summary>
        private XConfigureEvent? _configure;

        /// <summary>
        /// The configure point
        /// </summary>
        private PixelPoint? _configurePoint;

        /// <summary>
        /// The handle
        /// </summary>
        private IntPtr _handle;

        /// <summary>
        /// The input root
        /// </summary>
        private IInputRoot _inputRoot;

        /// <summary>
        /// The last event
        /// </summary>
        private InputEventContainer _lastEvent;

        /// <summary>
        /// The last window state
        /// </summary>
        private WindowState _lastWindowState;

        /// <summary>
        /// The mapped
        /// </summary>
        private bool _mapped;

        /// <summary>
        /// The minimum maximum size
        /// </summary>
        private (PixelSize minSize, PixelSize maxSize) _minMaxSize = (new PixelSize(1, 1),
            new PixelSize(MaxWindowDimension, MaxWindowDimension));

        /// <summary>
        /// The position
        /// </summary>
        private PixelPoint? _position;

        /// <summary>
        /// The real size
        /// </summary>
        private PixelSize _realSize;

        /// <summary>
        /// The render handle
        /// </summary>
        private IntPtr _renderHandle;

        /// <summary>
        /// The scaled minimum maximum size
        /// </summary>
        private (Size minSize, Size maxSize) _scaledMinMaxSize =
            (new Size(1, 1), new Size(double.PositiveInfinity, double.PositiveInfinity));

        /// <summary>
        /// The scaling
        /// </summary>
        private double _scaling = 1;

        /// <summary>
        /// The scaling override
        /// </summary>
        private double? _scalingOverride;

        /// <summary>
        /// The system decorations
        /// </summary>
        private bool _systemDecorations = true;

        /// <summary>
        /// The transient parent
        /// </summary>
        private X11Window _transientParent;

        /// <summary>
        /// The triggered expose
        /// </summary>
        private bool _triggeredExpose;

        /// <summary>
        /// The xic
        /// </summary>
        private IntPtr _xic;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="X11Window" /> class.
        /// </summary>
        /// <param name="platform">The platform.</param>
        /// <param name="popupParent">The popup parent.</param>
        public X11Window(AvaloniaX11Platform platform, IWindowImpl popupParent)
        {
            _platform = platform;
            _popup = popupParent != null;
            _x11 = platform.Info;
            _mouse = new MouseDevice();
            _touch = new TouchDevice();
            _keyboard = platform.KeyboardDevice;

            var glfeature = AvaloniaLocator.Current.GetService<IWindowingPlatformGlFeature>();
            XSetWindowAttributes attr = new XSetWindowAttributes();
            var valueMask = default(SetWindowValuemask);

            attr.backing_store = 1;
            attr.bit_gravity = Gravity.NorthWestGravity;
            attr.win_gravity = Gravity.NorthWestGravity;
            valueMask |= SetWindowValuemask.BackPixel | SetWindowValuemask.BorderPixel
                         | SetWindowValuemask.BackPixmap | SetWindowValuemask.BackingStore
                         | SetWindowValuemask.BitGravity | SetWindowValuemask.WinGravity;

            if (_popup)
            {
                attr.override_redirect = true;
                valueMask |= SetWindowValuemask.OverrideRedirect;
            }

            XVisualInfo? visualInfo = null;

            // OpenGL seems to be do weird things to it's current window which breaks resize sometimes
            _useRenderWindow = glfeature != null;

            var glx = glfeature as GlxGlPlatformFeature;
            if (glx != null)
                visualInfo = *glx.Display.VisualInfo;
            else if (glfeature == null)
                visualInfo = _x11.TransparentVisualInfo;

            var visual = IntPtr.Zero;
            var depth = 24;
            if (visualInfo != null)
            {
                visual = visualInfo.Value.visual;
                depth = (int)visualInfo.Value.depth;
                attr.colormap = XCreateColormap(_x11.Display, _x11.RootWindow, visualInfo.Value.visual, 0);
                valueMask |= SetWindowValuemask.ColorMap;
            }

            int defaultWidth = 0, defaultHeight = 0;

            if (!_popup && Screen != null)
            {
                var monitor = Screen.AllScreens.OrderBy(x => x.PixelDensity)
                   .FirstOrDefault(m => m.Bounds.Contains(Position));

                if (monitor != null)
                {
                    // Emulate Window 7+'s default window size behavior.
                    defaultWidth = (int)(monitor.WorkingArea.Width * 0.75d);
                    defaultHeight = (int)(monitor.WorkingArea.Height * 0.7d);
                }
            }

            // check if the calculated size is zero then compensate to hardcoded resolution
            defaultWidth = Math.Max(defaultWidth, 300);
            defaultHeight = Math.Max(defaultHeight, 200);

            _handle = XCreateWindow(_x11.Display, _x11.RootWindow, 10, 10, defaultWidth, defaultHeight, 0,
                depth,
                (int)CreateWindowArgs.InputOutput,
                visual,
                new UIntPtr((uint)valueMask), ref attr);

            if (_useRenderWindow)
                _renderHandle = XCreateWindow(_x11.Display, _handle, 0, 0, defaultWidth, defaultHeight, 0, depth,
                    (int)CreateWindowArgs.InputOutput,
                    visual,
                    new UIntPtr((uint)(SetWindowValuemask.BorderPixel | SetWindowValuemask.BitGravity |
                                       SetWindowValuemask.WinGravity | SetWindowValuemask.BackingStore)), ref attr);
            else
                _renderHandle = _handle;

            Handle = new PlatformHandle(_handle, "XID");
            _realSize = new PixelSize(defaultWidth, defaultHeight);
            platform.Windows[_handle] = OnEvent;
            XEventMask ignoredMask = XEventMask.SubstructureRedirectMask
                                     | XEventMask.ResizeRedirectMask
                                     | XEventMask.PointerMotionHintMask;
            if (platform.XI2 != null)
                ignoredMask |= platform.XI2.AddWindow(_handle, this);
            var mask = new IntPtr(0xffffff ^ (int)ignoredMask);
            XSelectInput(_x11.Display, _handle, mask);
            var protocols = new[]
            {
                _x11.Atoms.WM_DELETE_WINDOW
            };
            XSetWMProtocols(_x11.Display, _handle, protocols, protocols.Length);
            XChangeProperty(_x11.Display, _handle, _x11.Atoms._NET_WM_WINDOW_TYPE, _x11.Atoms.XA_ATOM,
                32, PropertyMode.Replace, new[] { _x11.Atoms._NET_WM_WINDOW_TYPE_NORMAL }, 1);

            if (platform.Options.WmClass != null)
                SetWmClass(platform.Options.WmClass);

            var surfaces = new List<object>
            {
                new X11FramebufferSurface(_x11.DeferredDisplay, _renderHandle,
                   depth, () => Scaling)
            };

            if (glfeature is EglGlPlatformFeature egl)
                surfaces.Insert(0,
                    new EglGlPlatformSurface((EglDisplay)egl.Display, egl.DeferredContext,
                        new SurfaceInfo(this, _x11.DeferredDisplay, _handle, _renderHandle)));
            if (glx != null)
                surfaces.Insert(0, new GlxGlPlatformSurface(glx.Display, glx.DeferredContext,
                    new SurfaceInfo(this, _x11.Display, _handle, _renderHandle)));

            Surfaces = surfaces.ToArray();
            UpdateMotifHints();
            _xic = XCreateIC(_x11.Xim, XNames.XNInputStyle, XIMProperties.XIMPreeditNothing | XIMProperties.XIMStatusNothing,
                XNames.XNClientWindow, _handle, IntPtr.Zero);
            XFlush(_x11.Display);
            if (_popup)
                PopupPositioner = new ManagedPopupPositioner(new ManagedPopupPositionerPopupImplHelper(popupParent, MoveResize));
            if (platform.Options.UseDBusMenu)
                NativeMenuExporter = DBusMenuExporter.TryCreate(_handle);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the activated.
        /// </summary>
        /// <value>The activated.</value>
        public Action Activated { get; set; }

        /// <summary>
        /// Gets the size of the client.
        /// </summary>
        /// <value>The size of the client.</value>
        public Size ClientSize => new Size(_realSize.Width / Scaling, _realSize.Height / Scaling);

        /// <summary>
        /// Gets or sets the closed.
        /// </summary>
        /// <value>The closed.</value>
        public Action Closed { get; set; }

        /// <summary>
        /// Gets or sets the closing.
        /// </summary>
        /// <value>The closing.</value>
        public Func<bool> Closing { get; set; }

        /// <summary>
        /// Gets or sets the deactivated.
        /// </summary>
        /// <value>The deactivated.</value>
        public Action Deactivated { get; set; }

        /// <summary>
        /// Gets the handle.
        /// </summary>
        /// <value>The handle.</value>
        public IPlatformHandle Handle { get; }

        /// <summary>
        /// Gets or sets the input.
        /// </summary>
        /// <value>The input.</value>
        public Action<RawInputEventArgs> Input { get; set; }

        /// <summary>
        /// Gets the input root.
        /// </summary>
        /// <value>The input root.</value>
        public IInputRoot InputRoot => _inputRoot;

        /// <summary>
        /// Gets the maximum size of the client.
        /// </summary>
        /// <value>The maximum size of the client.</value>
        public Size MaxClientSize => _platform.X11Screens.Screens.Select(s => s.Bounds.Size.ToSize(s.PixelDensity))
            .OrderByDescending(x => x.Width + x.Height).FirstOrDefault();

        /// <summary>
        /// Gets the mouse device.
        /// </summary>
        /// <value>The mouse device.</value>
        public IMouseDevice MouseDevice => _mouse;

        /// <summary>
        /// Gets the native menu exporter.
        /// </summary>
        /// <value>The native menu exporter.</value>
        public ITopLevelNativeMenuExporter NativeMenuExporter { get; }

        /// <summary>
        /// Gets or sets the paint.
        /// </summary>
        /// <value>The paint.</value>
        public Action<Rect> Paint { get; set; }

        /// <summary>
        /// Gets the popup positioner.
        /// </summary>
        /// <value>The popup positioner.</value>
        public IPopupPositioner PopupPositioner { get; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public PixelPoint Position
        {
            get => _position ?? default;
            set
            {
                var changes = new XWindowChanges
                {
                    x = (int)value.X,
                    y = (int)value.Y
                };
                XConfigureWindow(_x11.Display, _handle, ChangeWindowFlags.CWX | ChangeWindowFlags.CWY,
                    ref changes);
                XFlush(_x11.Display);
            }
        }

        /// <summary>
        /// Gets or sets the position changed.
        /// </summary>
        /// <value>The position changed.</value>
        public Action<PixelPoint> PositionChanged { get; set; }

        /// <summary>
        /// Gets or sets the resized.
        /// </summary>
        /// <value>The resized.</value>
        public Action<Size> Resized { get; set; }

        /// <summary>
        /// Gets the scaling.
        /// </summary>
        /// <value>The scaling.</value>
        public double Scaling
        {
            get
            {
                lock (SyncRoot)
                    return _scaling;
            }
            private set => _scaling = value;
        }

        //TODO
        /// <summary>
        /// Gets or sets the scaling changed.
        /// </summary>
        /// <value>The scaling changed.</value>
        public Action<double> ScalingChanged { get; set; }

        /// <summary>
        /// Gets the screen.
        /// </summary>
        /// <value>The screen.</value>
        public IScreenImpl Screen => _platform.Screens;

        /// <summary>
        /// Gets the surfaces.
        /// </summary>
        /// <value>The surfaces.</value>
        public IEnumerable<object> Surfaces { get; }

        /// <summary>
        /// Gets the synchronize root.
        /// </summary>
        /// <value>The synchronize root.</value>
        public object SyncRoot { get; } = new object();

        /// <summary>
        /// Gets the touch device.
        /// </summary>
        /// <value>The touch device.</value>
        public TouchDevice TouchDevice => _touch;

        /// <summary>
        /// Gets or sets the state of the window.
        /// </summary>
        /// <value>The state of the window.</value>
        public WindowState WindowState
        {
            get => _lastWindowState;
            set
            {
                if (_lastWindowState == value)
                    return;
                _lastWindowState = value;
                if (value == WindowState.Minimized)
                {
                    XIconifyWindow(_x11.Display, _handle, _x11.DefaultScreen);
                }
                else if (value == WindowState.Maximized)
                {
                    ChangeWMAtoms(false, _x11.Atoms._NET_WM_STATE_HIDDEN);
                    ChangeWMAtoms(true, _x11.Atoms._NET_WM_STATE_MAXIMIZED_VERT,
                        _x11.Atoms._NET_WM_STATE_MAXIMIZED_HORZ);
                }
                else
                {
                    ChangeWMAtoms(false, _x11.Atoms._NET_WM_STATE_HIDDEN);
                    ChangeWMAtoms(false, _x11.Atoms._NET_WM_STATE_MAXIMIZED_VERT,
                        _x11.Atoms._NET_WM_STATE_MAXIMIZED_HORZ);
                }
            }
        }

        /// <summary>
        /// Gets or sets the window state changed.
        /// </summary>
        /// <value>The window state changed.</value>
        public Action<WindowState> WindowStateChanged { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Activates this instance.
        /// </summary>
        public void Activate()
        {
            if (_x11.Atoms._NET_ACTIVE_WINDOW != IntPtr.Zero)
            {
                SendNetWMMessage(_x11.Atoms._NET_ACTIVE_WINDOW, (IntPtr)1, _x11.LastActivityTimestamp,
                    IntPtr.Zero);
            }
            else
            {
                XRaiseWindow(_x11.Display, _handle);
                XSetInputFocus(_x11.Display, _handle, 0, IntPtr.Zero);
            }
        }

        /// <summary>
        /// Begins the move drag.
        /// </summary>
        /// <param name="e">The <see cref="PointerPressedEventArgs" /> instance containing the event data.</param>
        public void BeginMoveDrag(PointerPressedEventArgs e)
        {
            BeginMoveResize(NetWmMoveResize._NET_WM_MOVERESIZE_MOVE, e);
        }

        /// <summary>
        /// Begins the resize drag.
        /// </summary>
        /// <param name="edge">The edge.</param>
        /// <param name="e">The <see cref="PointerPressedEventArgs" /> instance containing the event data.</param>
        public void BeginResizeDrag(WindowEdge edge, PointerPressedEventArgs e)
        {
            var side = NetWmMoveResize._NET_WM_MOVERESIZE_CANCEL;
            if (edge == WindowEdge.East)
                side = NetWmMoveResize._NET_WM_MOVERESIZE_SIZE_RIGHT;
            if (edge == WindowEdge.North)
                side = NetWmMoveResize._NET_WM_MOVERESIZE_SIZE_TOP;
            if (edge == WindowEdge.South)
                side = NetWmMoveResize._NET_WM_MOVERESIZE_SIZE_BOTTOM;
            if (edge == WindowEdge.West)
                side = NetWmMoveResize._NET_WM_MOVERESIZE_SIZE_LEFT;
            if (edge == WindowEdge.NorthEast)
                side = NetWmMoveResize._NET_WM_MOVERESIZE_SIZE_TOPRIGHT;
            if (edge == WindowEdge.NorthWest)
                side = NetWmMoveResize._NET_WM_MOVERESIZE_SIZE_TOPLEFT;
            if (edge == WindowEdge.SouthEast)
                side = NetWmMoveResize._NET_WM_MOVERESIZE_SIZE_BOTTOMRIGHT;
            if (edge == WindowEdge.SouthWest)
                side = NetWmMoveResize._NET_WM_MOVERESIZE_SIZE_BOTTOMLEFT;
            BeginMoveResize(side, e);
        }

        /// <summary>
        /// Determines whether this instance can resize the specified value.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public void CanResize(bool value)
        {
            _canResize = value;
            UpdateMotifHints();
            UpdateSizeHints(null);
        }

        /// <summary>
        /// Creates the popup.
        /// </summary>
        /// <returns>IPopupImpl.</returns>
        public IPopupImpl CreatePopup()
            => _platform.Options.OverlayPopups ? null : new X11Window(_platform, this);

        /// <summary>
        /// Creates the renderer.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <returns>IRenderer.</returns>
        public IRenderer CreateRenderer(IRenderRoot root)
        {
            var loop = AvaloniaLocator.Current.GetService<IRenderLoop>();
            return _platform.Options.UseDeferredRendering ?
                new DeferredRenderer(root, loop) :
                (IRenderer)new X11ImmediateRendererProxy(root, loop);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {
                XDestroyWindow(_x11.Display, _handle);
                Cleanup();
            }
        }

        /// <summary>
        /// Hides this instance.
        /// </summary>
        public void Hide() => XUnmapWindow(_x11.Display, _handle);

        /// <summary>
        /// Invalidates the specified rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        public void Invalidate(Rect rect)
        {
        }

        /// <summary>
        /// Moves the specified point.
        /// </summary>
        /// <param name="point">The point.</param>
        public void Move(PixelPoint point) => Position = point;

        /// <summary>
        /// Points to client.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Point.</returns>
        public Point PointToClient(PixelPoint point) => new Point((point.X - Position.X) / Scaling, (point.Y - Position.Y) / Scaling);

        /// <summary>
        /// Points to screen.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>PixelPoint.</returns>
        public PixelPoint PointToScreen(Point point) => new PixelPoint(
            (int)(point.X * Scaling + Position.X),
            (int)(point.Y * Scaling + Position.Y));

        /// <summary>
        /// Resizes the specified client size.
        /// </summary>
        /// <param name="clientSize">Size of the client.</param>
        public void Resize(Size clientSize) => Resize(clientSize, false);

        /// <summary>
        /// Schedules the x i2 input.
        /// </summary>
        /// <param name="args">The <see cref="RawInputEventArgs" /> instance containing the event data.</param>
        public void ScheduleXI2Input(RawInputEventArgs args)
        {
            if (args is RawPointerEventArgs pargs)
            {
                if ((pargs.Type == RawPointerEventType.TouchBegin
                     || pargs.Type == RawPointerEventType.TouchUpdate
                     || pargs.Type == RawPointerEventType.LeftButtonDown
                     || pargs.Type == RawPointerEventType.RightButtonDown
                     || pargs.Type == RawPointerEventType.MiddleButtonDown
                     || pargs.Type == RawPointerEventType.NonClientLeftButtonDown)
                    && ActivateTransientChildIfNeeded())
                    return;
                if (pargs.Type == RawPointerEventType.TouchEnd
                    && ActivateTransientChildIfNeeded())
                    pargs.Type = RawPointerEventType.TouchCancel;
            }

            ScheduleInput(args);
        }

        /// <summary>
        /// Sets the cursor.
        /// </summary>
        /// <param name="cursor">The cursor.</param>
        /// <exception cref="ArgumentException">Expected XCURSOR handle type</exception>
        public void SetCursor(IPlatformHandle cursor)
        {
            if (cursor == null)
                XDefineCursor(_x11.Display, _handle, _x11.DefaultCursor);
            else
            {
                if (cursor.HandleDescriptor != "XCURSOR")
                    throw new ArgumentException("Expected XCURSOR handle type");
                XDefineCursor(_x11.Display, _handle, cursor.Handle);
            }
        }

        /// <summary>
        /// Sets the icon.
        /// </summary>
        /// <param name="icon">The icon.</param>
        public void SetIcon(IWindowIconImpl icon)
        {
            if (icon != null)
            {
                var data = ((X11IconData)icon).Data;
                fixed (void* pdata = data)
                    XChangeProperty(_x11.Display, _handle, _x11.Atoms._NET_WM_ICON,
                        new IntPtr((int)Atom.XA_CARDINAL), 32, PropertyMode.Replace,
                        pdata, data.Length);
            }
            else
            {
                XDeleteProperty(_x11.Display, _handle, _x11.Atoms._NET_WM_ICON);
            }
        }

        /// <summary>
        /// Sets the input root.
        /// </summary>
        /// <param name="inputRoot">The input root.</param>
        public void SetInputRoot(IInputRoot inputRoot)
        {
            _inputRoot = inputRoot;
        }

        /// <summary>
        /// Sets the minimum size of the maximum.
        /// </summary>
        /// <param name="minSize">The minimum size.</param>
        /// <param name="maxSize">The maximum size.</param>
        public void SetMinMaxSize(Size minSize, Size maxSize)
        {
            _scaledMinMaxSize = (minSize, maxSize);
            var min = new PixelSize(
                (int)(minSize.Width < 1 ? 1 : minSize.Width * Scaling),
                (int)(minSize.Height < 1 ? 1 : minSize.Height * Scaling));

            const int maxDim = MaxWindowDimension;
            var max = new PixelSize(
                (int)(maxSize.Width > maxDim ? maxDim : Math.Max(min.Width, maxSize.Width * Scaling)),
                (int)(maxSize.Height > maxDim ? maxDim : Math.Max(min.Height, maxSize.Height * Scaling)));

            _minMaxSize = (min, max);
            UpdateSizeHints(null);
        }

        /// <summary>
        /// Sets the system decorations.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        public void SetSystemDecorations(bool enabled)
        {
            _systemDecorations = enabled;
            UpdateMotifHints();
        }

        /// <summary>
        /// Sets the title.
        /// </summary>
        /// <param name="title">The title.</param>
        public void SetTitle(string title)
        {
            var data = Encoding.UTF8.GetBytes(title);
            fixed (void* pdata = data)
            {
                XChangeProperty(_x11.Display, _handle, _x11.Atoms._NET_WM_NAME, _x11.Atoms.UTF8_STRING, 8,
                    PropertyMode.Replace, pdata, data.Length);
                XStoreName(_x11.Display, _handle, title);
            }
        }

        /// <summary>
        /// Sets the topmost.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public void SetTopmost(bool value)
        {
            ChangeWMAtoms(value, _x11.Atoms._NET_WM_STATE_ABOVE);
        }

        /// <summary>
        /// Sets the wm class.
        /// </summary>
        /// <param name="wmClass">The wm class.</param>
        public void SetWmClass(string wmClass)
        {
            var data = Encoding.ASCII.GetBytes(wmClass);
            fixed (void* pdata = data)
            {
                XChangeProperty(_x11.Display, _handle, _x11.Atoms.XA_WM_CLASS, _x11.Atoms.XA_STRING, 8,
                    PropertyMode.Replace, pdata, data.Length);
            }
        }

        /// <summary>
        /// Shows this instance.
        /// </summary>
        public void Show()
        {
            SetTransientParent(null);
            ShowCore();
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public void ShowDialog(IWindowImpl parent)
        {
            SetTransientParent((X11Window)parent);
            ShowCore();
        }

        /// <summary>
        /// Shows the taskbar icon.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public void ShowTaskbarIcon(bool value)
        {
            ChangeWMAtoms(!value, _x11.Atoms._NET_WM_STATE_SKIP_TASKBAR);
        }

        /// <summary>
        /// Activates the transient child if needed.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool ActivateTransientChildIfNeeded()
        {
            if (_transientChildren.Count == 0)
                return false;
            var child = _transientChildren.First();
            if (!child.ActivateTransientChildIfNeeded())
                child.Activate();
            return true;
        }

        /// <summary>
        /// Begins the move resize.
        /// </summary>
        /// <param name="side">The side.</param>
        /// <param name="e">The <see cref="PointerPressedEventArgs" /> instance containing the event data.</param>
        private void BeginMoveResize(NetWmMoveResize side, PointerPressedEventArgs e)
        {
            var (x, y) = GetCursorPos(_x11);
            XUngrabPointer(_x11.Display, new IntPtr(0));
            SendNetWMMessage(_x11.Atoms._NET_WM_MOVERESIZE, (IntPtr)x, (IntPtr)y,
                (IntPtr)side,
                (IntPtr)1, (IntPtr)1); // left button

            e.Pointer.Capture(null);
        }

        /// <summary>
        /// Changes the wm atoms.
        /// </summary>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <param name="atoms">The atoms.</param>
        /// <exception cref="ArgumentException"></exception>
        private void ChangeWMAtoms(bool enable, params IntPtr[] atoms)
        {
            if (atoms.Length < 1 || atoms.Length > 4)
                throw new ArgumentException();

            if (!_mapped)
            {
                XGetWindowProperty(_x11.Display, _handle, _x11.Atoms._NET_WM_STATE, IntPtr.Zero, new IntPtr(256),
                    false, (IntPtr)Atom.XA_ATOM, out _, out _, out var nitems, out _,
                    out var prop);
                var ptr = (IntPtr*)prop.ToPointer();
                var newAtoms = new HashSet<IntPtr>();
                for (var c = 0; c < nitems.ToInt64(); c++)
                    newAtoms.Add(*ptr);
                XFree(prop);
                foreach (var atom in atoms)
                    if (enable)
                        newAtoms.Add(atom);
                    else
                        newAtoms.Remove(atom);

                XChangeProperty(_x11.Display, _handle, _x11.Atoms._NET_WM_STATE, (IntPtr)Atom.XA_ATOM, 32,
                    PropertyMode.Replace, newAtoms.ToArray(), newAtoms.Count);
            }

            SendNetWMMessage(_x11.Atoms._NET_WM_STATE,
                (IntPtr)(enable ? 1 : 0),
                atoms[0],
                atoms.Length > 1 ? atoms[1] : IntPtr.Zero,
                atoms.Length > 2 ? atoms[2] : IntPtr.Zero,
                atoms.Length > 3 ? atoms[3] : IntPtr.Zero
            );
        }

        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        private void Cleanup()
        {
            SetTransientParent(null, false);
            if (_xic != IntPtr.Zero)
            {
                XDestroyIC(_xic);
                _xic = IntPtr.Zero;
            }

            if (_handle != IntPtr.Zero)
            {
                XDestroyWindow(_x11.Display, _handle);
                _platform.Windows.Remove(_handle);
                _platform.XI2?.OnWindowDestroyed(_handle);
                _handle = IntPtr.Zero;
                Closed?.Invoke();
                _mouse.Dispose();
                _touch.Dispose();
            }

            if (_useRenderWindow && _renderHandle != IntPtr.Zero)
            {
                XDestroyWindow(_x11.Display, _renderHandle);
                _renderHandle = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Does the paint.
        /// </summary>
        private void DoPaint()
        {
            Paint?.Invoke(new Rect());
        }

        /// <summary>
        /// Mouses the event.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="ev">The ev.</param>
        /// <param name="mods">The mods.</param>
        private void MouseEvent(RawPointerEventType type, ref XEvent ev, XModifierMask mods)
        {
            var mev = new RawPointerEventArgs(
                _mouse, (ulong)ev.ButtonEvent.time.ToInt64(), _inputRoot,
                type, new Point(ev.ButtonEvent.x, ev.ButtonEvent.y), TranslateModifiers(mods));
            if (type == RawPointerEventType.Move && _inputQueue.Count > 0 && _lastEvent.Event is RawPointerEventArgs ma)
                if (ma.Type == RawPointerEventType.Move)
                {
                    _lastEvent.Event = mev;
                    return;
                }
            ScheduleInput(mev, ref ev);
        }

        /// <summary>
        /// Moves the resize.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="size">The size.</param>
        /// <param name="scaling">The scaling.</param>
        private void MoveResize(PixelPoint position, Size size, double scaling)
        {
            Move(position);
            _scalingOverride = scaling;
            UpdateScaling(true);
            Resize(size, true);
        }

        /// <summary>
        /// Called when [event].
        /// </summary>
        /// <param name="ev">The ev.</param>
        private void OnEvent(XEvent ev)
        {
            lock (SyncRoot)
                OnEventSync(ev);
        }

        /// <summary>
        /// Called when [event synchronize].
        /// </summary>
        /// <param name="ev">The ev.</param>
        private void OnEventSync(XEvent ev)
        {
            if (XFilterEvent(ref ev, _handle))
                return;
            if (ev.type == XEventName.MapNotify)
            {
                _mapped = true;
                if (_useRenderWindow)
                    XMapWindow(_x11.Display, _renderHandle);
            }
            else if (ev.type == XEventName.UnmapNotify)
                _mapped = false;
            else if (ev.type == XEventName.Expose ||
                     (ev.type == XEventName.VisibilityNotify &&
                      ev.VisibilityEvent.state < 2))
            {
                if (!_triggeredExpose)
                {
                    _triggeredExpose = true;
                    Dispatcher.UIThread.Post(() =>
                    {
                        _triggeredExpose = false;
                        DoPaint();
                    }, DispatcherPriority.Render);
                }
            }
            else if (ev.type == XEventName.FocusIn)
            {
                if (ActivateTransientChildIfNeeded())
                    return;
                Activated?.Invoke();
            }
            else if (ev.type == XEventName.FocusOut)
                Deactivated?.Invoke();
            else if (ev.type == XEventName.MotionNotify)
                MouseEvent(RawPointerEventType.Move, ref ev, ev.MotionEvent.state);
            else if (ev.type == XEventName.LeaveNotify)
                MouseEvent(RawPointerEventType.LeaveWindow, ref ev, ev.CrossingEvent.state);
            else if (ev.type == XEventName.PropertyNotify)
            {
                OnPropertyChange(ev.PropertyEvent.atom, ev.PropertyEvent.state == 0);
            }
            else if (ev.type == XEventName.ButtonPress)
            {
                if (ActivateTransientChildIfNeeded())
                    return;
                if (ev.ButtonEvent.button < 4)
                    MouseEvent(ev.ButtonEvent.button == 1 ? RawPointerEventType.LeftButtonDown
                        : ev.ButtonEvent.button == 2 ? RawPointerEventType.MiddleButtonDown
                        : RawPointerEventType.RightButtonDown, ref ev, ev.ButtonEvent.state);
                else
                {
                    var delta = ev.ButtonEvent.button == 4
                        ? new Vector(0, 1)
                        : ev.ButtonEvent.button == 5
                            ? new Vector(0, -1)
                            : ev.ButtonEvent.button == 6
                                ? new Vector(1, 0)
                                : new Vector(-1, 0);
                    ScheduleInput(new RawMouseWheelEventArgs(_mouse, (ulong)ev.ButtonEvent.time.ToInt64(),
                        _inputRoot, new Point(ev.ButtonEvent.x, ev.ButtonEvent.y), delta,
                        TranslateModifiers(ev.ButtonEvent.state)), ref ev);
                }
            }
            else if (ev.type == XEventName.ButtonRelease)
            {
                if (ev.ButtonEvent.button < 4)
                    MouseEvent(ev.ButtonEvent.button == 1 ? RawPointerEventType.LeftButtonUp
                        : ev.ButtonEvent.button == 2 ? RawPointerEventType.MiddleButtonUp
                        : RawPointerEventType.RightButtonUp, ref ev, ev.ButtonEvent.state);
            }
            else if (ev.type == XEventName.ConfigureNotify)
            {
                if (ev.ConfigureEvent.window != _handle)
                    return;
                var needEnqueue = (_configure == null);
                _configure = ev.ConfigureEvent;
                if (ev.ConfigureEvent.override_redirect || ev.ConfigureEvent.send_event)
                    _configurePoint = new PixelPoint(ev.ConfigureEvent.x, ev.ConfigureEvent.y);
                else
                {
                    XTranslateCoordinates(_x11.Display, _handle, _x11.RootWindow,
                        0, 0,
                        out var tx, out var ty, out _);
                    _configurePoint = new PixelPoint(tx, ty);
                }
                if (needEnqueue)
                    Dispatcher.UIThread.Post(() =>
                    {
                        if (_configure == null)
                            return;
                        var cev = _configure.Value;
                        var npos = _configurePoint.Value;
                        _configure = null;
                        _configurePoint = null;

                        var nsize = new PixelSize(cev.width, cev.height);
                        var changedSize = _realSize != nsize;
                        var changedPos = _position == null || npos != _position;
                        _realSize = nsize;
                        _position = npos;
                        bool updatedSizeViaScaling = false;
                        if (changedPos)
                        {
                            PositionChanged?.Invoke(npos);
                            updatedSizeViaScaling = UpdateScaling();
                        }

                        if (changedSize && !updatedSizeViaScaling && !_popup)
                            Resized?.Invoke(ClientSize);

                        Dispatcher.UIThread.RunJobs(DispatcherPriority.Layout);
                    }, DispatcherPriority.Layout);
                if (_useRenderWindow)
                    XConfigureResizeWindow(_x11.Display, _renderHandle, ev.ConfigureEvent.width,
                        ev.ConfigureEvent.height);
            }
            else if (ev.type == XEventName.DestroyNotify && ev.AnyEvent.window == _handle)
            {
                Cleanup();
            }
            else if (ev.type == XEventName.ClientMessage)
            {
                if (ev.ClientMessageEvent.message_type == _x11.Atoms.WM_PROTOCOLS)
                {
                    if (ev.ClientMessageEvent.ptr1 == _x11.Atoms.WM_DELETE_WINDOW)
                    {
                        if (Closing?.Invoke() != true)
                            Dispose();
                    }
                }
            }
            else if (ev.type == XEventName.KeyPress || ev.type == XEventName.KeyRelease)
            {
                if (ActivateTransientChildIfNeeded())
                    return;
                var buffer = stackalloc byte[40];

                var index = ev.KeyEvent.state.HasFlag(XModifierMask.ShiftMask);

                // We need the latin key, since it's mainly used for hotkeys, we use a different API for text anyway
                var key = (X11Key)XKeycodeToKeysym(_x11.Display, ev.KeyEvent.keycode, index ? 1 : 0).ToInt32();

                // Manually switch the Shift index for the keypad,
                // there should be a proper way to do this
                if (ev.KeyEvent.state.HasFlag(XModifierMask.Mod2Mask)
                    && key > X11Key.Num_Lock && key <= X11Key.KP_9)
                    key = (X11Key)XKeycodeToKeysym(_x11.Display, ev.KeyEvent.keycode, index ? 0 : 1).ToInt32();

                ScheduleInput(new RawKeyEventArgs(_keyboard, (ulong)ev.KeyEvent.time.ToInt64(), _inputRoot,
                    ev.type == XEventName.KeyPress ? RawKeyEventType.KeyDown : RawKeyEventType.KeyUp,
                    X11KeyTransform.ConvertKey(key), TranslateModifiers(ev.KeyEvent.state)), ref ev);

                if (ev.type == XEventName.KeyPress)
                {
                    var len = Xutf8LookupString(_xic, ref ev, buffer, 40, out _, out _);
                    if (len != 0)
                    {
                        var text = Encoding.UTF8.GetString(buffer, len);
                        if (text.Length == 1)
                        {
                            if (text[0] < ' ' || text[0] == 0x7f) //Control codes or DEL
                                return;
                        }
                        ScheduleInput(new RawTextInputEventArgs(_keyboard, (ulong)ev.KeyEvent.time.ToInt64(), _inputRoot, text),
                            ref ev);
                    }
                }
            }
        }

        /// <summary>
        /// Called when [property change].
        /// </summary>
        /// <param name="atom">The atom.</param>
        /// <param name="hasValue">if set to <c>true</c> [has value].</param>
        private void OnPropertyChange(IntPtr atom, bool hasValue)
        {
            if (atom == _x11.Atoms._NET_WM_STATE)
            {
                WindowState state = WindowState.Normal;
                if (hasValue)
                {
                    XGetWindowProperty(_x11.Display, _handle, _x11.Atoms._NET_WM_STATE, IntPtr.Zero, new IntPtr(256),
                        false, (IntPtr)Atom.XA_ATOM, out _, out _, out var nitems, out _,
                        out var prop);
                    int maximized = 0;
                    var pitems = (IntPtr*)prop.ToPointer();
                    for (var c = 0; c < nitems.ToInt32(); c++)
                    {
                        if (pitems[c] == _x11.Atoms._NET_WM_STATE_HIDDEN)
                        {
                            state = WindowState.Minimized;
                            break;
                        }

                        if (pitems[c] == _x11.Atoms._NET_WM_STATE_MAXIMIZED_HORZ ||
                            pitems[c] == _x11.Atoms._NET_WM_STATE_MAXIMIZED_VERT)
                        {
                            maximized++;
                            if (maximized == 2)
                            {
                                state = WindowState.Maximized;
                                break;
                            }
                        }
                    }
                    XFree(prop);
                }
                if (_lastWindowState != state)
                {
                    _lastWindowState = state;
                    WindowStateChanged?.Invoke(state);
                }
            }
        }

        /// <summary>
        /// Resizes the specified client size.
        /// </summary>
        /// <param name="clientSize">Size of the client.</param>
        /// <param name="force">if set to <c>true</c> [force].</param>
        private void Resize(Size clientSize, bool force)
        {
            if (!force && clientSize == ClientSize)
                return;

            var needImmediatePopupResize = clientSize != ClientSize;

            var pixelSize = ToPixelSize(clientSize);
            UpdateSizeHints(pixelSize);
            XConfigureResizeWindow(_x11.Display, _handle, pixelSize);
            if (_useRenderWindow)
                XConfigureResizeWindow(_x11.Display, _renderHandle, pixelSize);
            XFlush(_x11.Display);

            if (force || (_popup && needImmediatePopupResize))
            {
                _realSize = pixelSize;
                Resized?.Invoke(ClientSize);
            }
        }

        /// <summary>
        /// Schedules the input.
        /// </summary>
        /// <param name="args">The <see cref="RawInputEventArgs" /> instance containing the event data.</param>
        /// <param name="xev">The xev.</param>
        private void ScheduleInput(RawInputEventArgs args, ref XEvent xev)
        {
            _x11.LastActivityTimestamp = xev.ButtonEvent.time;
            ScheduleInput(args);
        }

        /// <summary>
        /// Schedules the input.
        /// </summary>
        /// <param name="args">The <see cref="RawInputEventArgs" /> instance containing the event data.</param>
        private void ScheduleInput(RawInputEventArgs args)
        {
            if (args is RawPointerEventArgs mouse)
                mouse.Position /= Scaling;
            if (args is RawDragEvent drag)
                drag.Location /= Scaling;

            _lastEvent = new InputEventContainer() { Event = args };
            _inputQueue.Enqueue(_lastEvent);
            if (_inputQueue.Count == 1)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    while (_inputQueue.Count > 0)
                    {
                        Dispatcher.UIThread.RunJobs(DispatcherPriority.Input + 1);
                        var ev = _inputQueue.Dequeue();
                        Input?.Invoke(ev.Event);
                    }
                }, DispatcherPriority.Input);
            }
        }

        /// <summary>
        /// Sends the net wm message.
        /// </summary>
        /// <param name="message_type">Type of the message.</param>
        /// <param name="l0">The l0.</param>
        /// <param name="l1">The l1.</param>
        /// <param name="l2">The l2.</param>
        /// <param name="l3">The l3.</param>
        /// <param name="l4">The l4.</param>
        private void SendNetWMMessage(IntPtr message_type, IntPtr l0,
            IntPtr? l1 = null, IntPtr? l2 = null, IntPtr? l3 = null, IntPtr? l4 = null)
        {
            var xev = new XEvent
            {
                ClientMessageEvent =
                {
                    type = XEventName.ClientMessage,
                    send_event = true,
                    window = _handle,
                    message_type = message_type,
                    format = 32,
                    ptr1 = l0,
                    ptr2 = l1 ?? IntPtr.Zero,
                    ptr3 = l2 ?? IntPtr.Zero,
                    ptr4 = l3 ?? IntPtr.Zero
                }
            };
            xev.ClientMessageEvent.ptr4 = l4 ?? IntPtr.Zero;
            XSendEvent(_x11.Display, _x11.RootWindow, false,
                new IntPtr((int)(EventMask.SubstructureRedirectMask | EventMask.SubstructureNotifyMask)), ref xev);
        }

        /// <summary>
        /// Sets the transient for hint.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void SetTransientForHint(IntPtr? parent)
        {
            if (parent == null || parent == IntPtr.Zero)
                XDeleteProperty(_x11.Display, _handle, _x11.Atoms.XA_WM_TRANSIENT_FOR);
            else
                XSetTransientForHint(_x11.Display, _handle, parent.Value);
        }

        /// <summary>
        /// Sets the transient parent.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="informServer">if set to <c>true</c> [inform server].</param>
        private void SetTransientParent(X11Window window, bool informServer = true)
        {
            _transientParent?._transientChildren.Remove(this);
            _transientParent = window;
            _transientParent?._transientChildren.Add(this);
            if (informServer)
                SetTransientForHint(_transientParent?._handle);
        }

        /// <summary>
        /// Shows the core.
        /// </summary>
        private void ShowCore()
        {
            XMapWindow(_x11.Display, _handle);
            XFlush(_x11.Display);
        }

        /// <summary>
        /// Converts to pixelsize.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>PixelSize.</returns>
        private PixelSize ToPixelSize(Size size) => new PixelSize((int)(size.Width * Scaling), (int)(size.Height * Scaling));

        /// <summary>
        /// Translates the modifiers.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>RawInputModifiers.</returns>
        private RawInputModifiers TranslateModifiers(XModifierMask state)
        {
            var rv = default(RawInputModifiers);
            if (state.HasFlag(XModifierMask.Button1Mask))
                rv |= RawInputModifiers.LeftMouseButton;
            if (state.HasFlag(XModifierMask.Button2Mask))
                rv |= RawInputModifiers.RightMouseButton;
            if (state.HasFlag(XModifierMask.Button2Mask))
                rv |= RawInputModifiers.MiddleMouseButton;
            if (state.HasFlag(XModifierMask.ShiftMask))
                rv |= RawInputModifiers.Shift;
            if (state.HasFlag(XModifierMask.ControlMask))
                rv |= RawInputModifiers.Control;
            if (state.HasFlag(XModifierMask.Mod1Mask))
                rv |= RawInputModifiers.Alt;
            if (state.HasFlag(XModifierMask.Mod4Mask))
                rv |= RawInputModifiers.Meta;
            return rv;
        }

        /// <summary>
        /// Updates the motif hints.
        /// </summary>
        private void UpdateMotifHints()
        {
            var functions = MotifFunctions.Move | MotifFunctions.Close | MotifFunctions.Resize |
                            MotifFunctions.Minimize | MotifFunctions.Maximize;
            var decorations = MotifDecorations.Menu | MotifDecorations.Title | MotifDecorations.Border |
                              MotifDecorations.Maximize | MotifDecorations.Minimize | MotifDecorations.ResizeH;

            if (_popup || !_systemDecorations)
            {
                decorations = 0;
            }

            if (!_canResize)
            {
                functions &= ~(MotifFunctions.Resize | MotifFunctions.Maximize);
                decorations &= ~(MotifDecorations.Maximize | MotifDecorations.ResizeH);
            }

            var hints = new MotifWmHints
            {
                flags = new IntPtr((int)(MotifFlags.Decorations | MotifFlags.Functions)),
                decorations = new IntPtr((int)decorations),
                functions = new IntPtr((int)functions)
            };

            XChangeProperty(_x11.Display, _handle,
                _x11.Atoms._MOTIF_WM_HINTS, _x11.Atoms._MOTIF_WM_HINTS, 32,
                PropertyMode.Replace, ref hints, 5);
        }

        /// <summary>
        /// Updates the scaling.
        /// </summary>
        /// <param name="skipResize">if set to <c>true</c> [skip resize].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool UpdateScaling(bool skipResize = false)
        {
            lock (SyncRoot)
            {
                double newScaling;
                if (_scalingOverride.HasValue)
                    newScaling = _scalingOverride.Value;
                else
                {
                    var monitor = _platform.X11Screens.Screens.OrderBy(x => x.PixelDensity)
                        .FirstOrDefault(m => m.Bounds.Contains(Position));
                    newScaling = monitor?.PixelDensity ?? Scaling;
                }

                if (Scaling != newScaling)
                {
                    var oldScaledSize = ClientSize;
                    Scaling = newScaling;
                    ScalingChanged?.Invoke(Scaling);
                    SetMinMaxSize(_scaledMinMaxSize.minSize, _scaledMinMaxSize.maxSize);
                    if (!skipResize)
                        Resize(oldScaledSize, true);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Updates the size hints.
        /// </summary>
        /// <param name="preResize">The pre resize.</param>
        private void UpdateSizeHints(PixelSize? preResize)
        {
            var min = _minMaxSize.minSize;
            var max = _minMaxSize.maxSize;

            if (!_canResize)
                max = min = _realSize;

            if (preResize.HasValue)
            {
                var desired = preResize.Value;
                max = new PixelSize(Math.Max(desired.Width, max.Width), Math.Max(desired.Height, max.Height));
                min = new PixelSize(Math.Min(desired.Width, min.Width), Math.Min(desired.Height, min.Height));
            }

            var hints = new XSizeHints
            {
                min_width = min.Width,
                min_height = min.Height
            };
            hints.height_inc = hints.width_inc = 1;
            var flags = XSizeHintsFlags.PMinSize | XSizeHintsFlags.PResizeInc;
            // People might be passing double.MaxValue
            if (max.Width < 100000 && max.Height < 100000)
            {
                hints.max_width = max.Width;
                hints.max_height = max.Height;
                flags |= XSizeHintsFlags.PMaxSize;
            }

            hints.flags = (IntPtr)flags;

            XSetWMNormalHints(_x11.Display, _handle, ref hints);
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class InputEventContainer.
        /// </summary>
        private class InputEventContainer
        {
            #region Fields

            /// <summary>
            /// The event
            /// </summary>
            public RawInputEventArgs Event;

            #endregion Fields
        }

        /// <summary>
        /// Class SurfaceInfo.
        /// Implements the <see cref="Avalonia.OpenGL.EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo" />
        /// </summary>
        /// <seealso cref="Avalonia.OpenGL.EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo" />
        private class SurfaceInfo : EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo
        {
            #region Fields

            /// <summary>
            /// The display
            /// </summary>
            private readonly IntPtr _display;

            /// <summary>
            /// The parent
            /// </summary>
            private readonly IntPtr _parent;

            /// <summary>
            /// The window
            /// </summary>
            private readonly X11Window _window;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="SurfaceInfo" /> class.
            /// </summary>
            /// <param name="window">The window.</param>
            /// <param name="display">The display.</param>
            /// <param name="parent">The parent.</param>
            /// <param name="xid">The xid.</param>
            public SurfaceInfo(X11Window window, IntPtr display, IntPtr parent, IntPtr xid)
            {
                _window = window;
                _display = display;
                _parent = parent;
                Handle = xid;
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets the handle.
            /// </summary>
            /// <value>The handle.</value>
            public IntPtr Handle { get; }

            /// <summary>
            /// Gets the scaling.
            /// </summary>
            /// <value>The scaling.</value>
            public double Scaling => _window.Scaling;

            /// <summary>
            /// Gets the size.
            /// </summary>
            /// <value>The size.</value>
            public PixelSize Size
            {
                get
                {
                    XLockDisplay(_display);
                    XGetGeometry(_display, _parent, out var geo);
                    XResizeWindow(_display, Handle, geo.width, geo.height);
                    XUnlockDisplay(_display);
                    return new PixelSize(geo.width, geo.height);
                }
            }

            #endregion Properties
        }

        #endregion Classes
    }
}
