// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="X11Platform.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.FreeDesktop;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.OpenGL;
using Avalonia.Platform;
using Avalonia.Rendering;
using IronyModManager.Platform.x11.Glx;
using IronyModManager.Platform.x11.NativeDialogs;
using static IronyModManager.Platform.x11.XLib;

namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Class X11PlatformExtensions.
    /// </summary>
    public static class X11PlatformExtensions
    {
        #region Methods

        /// <summary>
        /// Initializes the irony X11 platform.
        /// </summary>
        /// <param name="options">The options.</param>
        public static void InitializeIronyX11Platform(X11PlatformOptions options = null) =>
            new AvaloniaX11Platform().Initialize(options ?? new X11PlatformOptions());

        /// <summary>
        /// Uses the irony X11.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns>T.</returns>
        public static T UseIronyX11<T>(this T builder) where T : AppBuilderBase<T>, new()
        {
            builder.UseWindowingSubsystem(() =>
                new AvaloniaX11Platform().Initialize(AvaloniaLocator.Current.GetService<X11PlatformOptions>() ??
                                                     new X11PlatformOptions()));
            return builder;
        }

        #endregion Methods
    }

    /// <summary>
    /// Class AvaloniaX11Platform.
    /// Implements the <see cref="Avalonia.Platform.IWindowingPlatform" />
    /// </summary>
    /// <seealso cref="Avalonia.Platform.IWindowingPlatform" />
    internal class AvaloniaX11Platform : IWindowingPlatform
    {
        #region Fields

        /// <summary>
        /// The windows
        /// </summary>
        public Dictionary<IntPtr, Action<XEvent>> Windows = new Dictionary<IntPtr, Action<XEvent>>();

        /// <summary>
        /// The x i2
        /// </summary>
        public XI2Manager XI2;

        /// <summary>
        /// The keyboard device
        /// </summary>
        private readonly Lazy<KeyboardDevice> _keyboardDevice = new Lazy<KeyboardDevice>(() => new KeyboardDevice());

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the deferred display.
        /// </summary>
        /// <value>The deferred display.</value>
        public IntPtr DeferredDisplay { get; set; }

        /// <summary>
        /// Gets or sets the display.
        /// </summary>
        /// <value>The display.</value>
        public IntPtr Display { get; set; }

        /// <summary>
        /// Gets the information.
        /// </summary>
        /// <value>The information.</value>
        public X11Info Info { get; private set; }

        /// <summary>
        /// Gets the keyboard device.
        /// </summary>
        /// <value>The keyboard device.</value>
        public KeyboardDevice KeyboardDevice => _keyboardDevice.Value;

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>The options.</value>
        public X11PlatformOptions Options { get; private set; }

        /// <summary>
        /// Gets the screens.
        /// </summary>
        /// <value>The screens.</value>
        public IScreenImpl Screens { get; private set; }

        /// <summary>
        /// Gets the X11 screens.
        /// </summary>
        /// <value>The X11 screens.</value>
        public IX11Screens X11Screens { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Creates the embeddable window.
        /// </summary>
        /// <returns>IEmbeddableWindowImpl.</returns>
        /// <exception cref="NotSupportedException"></exception>
        public IEmbeddableWindowImpl CreateEmbeddableWindow()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates the window.
        /// </summary>
        /// <returns>IWindowImpl.</returns>
        public IWindowImpl CreateWindow()
        {
            return new X11Window(this, null);
        }

        /// <summary>
        /// Initializes the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="Exception">XOpenDisplay failed</exception>
        public void Initialize(X11PlatformOptions options)
        {
            Options = options;
            XInitThreads();
            Display = XOpenDisplay(IntPtr.Zero);
            DeferredDisplay = XOpenDisplay(IntPtr.Zero);
            if (Display == IntPtr.Zero)
                throw new Exception("XOpenDisplay failed");
            XError.Init();
            Info = new X11Info(Display, DeferredDisplay);
            //TODO: log
            if (options.UseDBusMenu)
                DBusHelper.TryInitialize();
            AvaloniaLocator.CurrentMutable.BindToSelf(this)
                .Bind<IWindowingPlatform>().ToConstant(this)
                .Bind<IPlatformThreadingInterface>().ToConstant(new X11PlatformThreading(this))
                .Bind<IRenderTimer>().ToConstant(new DefaultRenderTimer(60))
                .Bind<IRenderLoop>().ToConstant(new RenderLoop())
#pragma warning disable CS0618 // Type or member is obsolete
                .Bind<PlatformHotkeyConfiguration>().ToConstant(new PlatformHotkeyConfiguration(InputModifiers.Control))
#pragma warning restore CS0618 // Type or member is obsolete
                .Bind<IKeyboardDevice>().ToFunc(() => KeyboardDevice)
                .Bind<IStandardCursorFactory>().ToConstant(new X11CursorFactory(Display))
                .Bind<IClipboard>().ToConstant(new X11Clipboard(this))
                .Bind<IPlatformSettings>().ToConstant(new PlatformSettingsStub())
                .Bind<IPlatformIconLoader>().ToConstant(new X11IconLoader(Info))
                .Bind<ISystemDialogImpl>().ToConstant(new GtkSystemDialog())
                .Bind<IMountedVolumeInfoProvider>().ToConstant(new LinuxMountedVolumeInfoProvider());

            X11Screens = IronyModManager.Platform.x11.X11Screens.Init(this);
            Screens = new X11Screens(X11Screens);
            if (Info.XInputVersion != null)
            {
                var xi2 = new XI2Manager();
                if (xi2.Init(this))
                    XI2 = xi2;
            }

            if (options.UseGpu)
            {
                if (options.UseEGL)
                    EglGlPlatformFeature.TryInitialize();
                else
                    GlxGlPlatformFeature.TryInitialize(Info);
            }
        }

        #endregion Methods
    }
}
