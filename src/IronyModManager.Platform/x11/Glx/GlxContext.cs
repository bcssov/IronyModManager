// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="GlxContext.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ***********************************************************************
using System;
using System.Reactive.Disposables;
using System.Threading;
using Avalonia.OpenGL;

namespace IronyModManager.Platform.x11.Glx
{
    /// <summary>
    /// Class GlxContext.
    /// Implements the <see cref="Avalonia.OpenGL.IGlContext" />
    /// </summary>
    /// <seealso cref="Avalonia.OpenGL.IGlContext" />
    internal class GlxContext : IGlContext
    {
        #region Fields

        /// <summary>
        /// The default xid
        /// </summary>
        private readonly IntPtr _defaultXid;

        /// <summary>
        /// The lock
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// The X11
        /// </summary>
        private readonly X11Info _x11;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GlxContext" /> class.
        /// </summary>
        /// <param name="glx">The GLX.</param>
        /// <param name="handle">The handle.</param>
        /// <param name="display">The display.</param>
        /// <param name="x11">The X11.</param>
        /// <param name="defaultXid">The default xid.</param>
        public GlxContext(GlxInterface glx, IntPtr handle, GlxDisplay display, X11Info x11, IntPtr defaultXid)
        {
            Handle = handle;
            Glx = glx;
            _x11 = x11;
            _defaultXid = defaultXid;
            Display = display;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the display.
        /// </summary>
        /// <value>The display.</value>
        public GlxDisplay Display { get; }

        /// <summary>
        /// Gets the display.
        /// </summary>
        /// <value>The display.</value>
        IGlDisplay IGlContext.Display => Display;

        /// <summary>
        /// Gets the GLX.
        /// </summary>
        /// <value>The GLX.</value>
        public GlxInterface Glx { get; }

        /// <summary>
        /// Gets the handle.
        /// </summary>
        /// <value>The handle.</value>
        public IntPtr Handle { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Locks this instance.
        /// </summary>
        /// <returns>IDisposable.</returns>
        public IDisposable Lock()
        {
            Monitor.Enter(_lock);
            return Disposable.Create(() => Monitor.Exit(_lock));
        }

        /// <summary>
        /// Makes the current.
        /// </summary>
        public void MakeCurrent() => MakeCurrent(_defaultXid);

        /// <summary>
        /// Makes the current.
        /// </summary>
        /// <param name="xid">The xid.</param>
        /// <exception cref="OpenGlException">glXMakeContextCurrent failed</exception>
        public void MakeCurrent(IntPtr xid)
        {
            if (!Glx.MakeContextCurrent(_x11.Display, xid, xid, Handle))
                throw new OpenGlException("glXMakeContextCurrent failed ");
        }

        #endregion Methods
    }
}
