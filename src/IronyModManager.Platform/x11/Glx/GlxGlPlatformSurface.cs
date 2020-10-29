// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="GlxGlPlatformSurface.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ***********************************************************************
using System;
using Avalonia;
using Avalonia.OpenGL;
using IronyModManager.Shared;

namespace IronyModManager.Platform.x11.Glx
{
    /// <summary>
    /// Class GlxGlPlatformSurface.
    /// Implements the <see cref="Avalonia.OpenGL.IGlPlatformSurface" />
    /// </summary>
    /// <seealso cref="Avalonia.OpenGL.IGlPlatformSurface" />
    [ExcludeFromCoverage("External component.")]
    internal class GlxGlPlatformSurface : IGlPlatformSurface
    {
        #region Fields

        /// <summary>
        /// The context
        /// </summary>
        private readonly GlxContext _context;

        /// <summary>
        /// The display
        /// </summary>
#pragma warning disable IDE0052 // Remove unread private members
        private readonly GlxDisplay _display;
#pragma warning restore IDE0052 // Remove unread private members

        /// <summary>
        /// The information
        /// </summary>
        private readonly EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo _info;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GlxGlPlatformSurface" /> class.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="context">The context.</param>
        /// <param name="info">The information.</param>
        public GlxGlPlatformSurface(GlxDisplay display, GlxContext context, EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo info)
        {
            _display = display;
            _context = context;
            _info = info;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Creates the gl render target.
        /// </summary>
        /// <returns>IGlPlatformSurfaceRenderTarget.</returns>
        public IGlPlatformSurfaceRenderTarget CreateGlRenderTarget()
        {
            return new RenderTarget(_context, _info);
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class RenderTarget.
        /// Implements the <see cref="Avalonia.OpenGL.IGlPlatformSurfaceRenderTarget" />
        /// </summary>
        /// <seealso cref="Avalonia.OpenGL.IGlPlatformSurfaceRenderTarget" />
        private class RenderTarget : IGlPlatformSurfaceRenderTarget
        {
            #region Fields

            /// <summary>
            /// The context
            /// </summary>
            private readonly GlxContext _context;

            /// <summary>
            /// The information
            /// </summary>
            private readonly EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo _info;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="RenderTarget" /> class.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="info">The information.</param>
            public RenderTarget(GlxContext context, EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo info)
            {
                _context = context;
                _info = info;
            }

            #endregion Constructors

            #region Methods

            /// <summary>
            /// Begins the draw.
            /// </summary>
            /// <returns>IGlPlatformSurfaceRenderingSession.</returns>
            public IGlPlatformSurfaceRenderingSession BeginDraw()
            {
                var l = _context.Lock();
                try
                {
                    _context.MakeCurrent(_info.Handle);
                    return new Session(_context, _info, l);
                }
                catch
                {
                    l.Dispose();
                    throw;
                }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                // No-op
            }

            #endregion Methods

            #region Classes

            /// <summary>
            /// Class Session.
            /// Implements the <see cref="Avalonia.OpenGL.IGlPlatformSurfaceRenderingSession" />
            /// </summary>
            /// <seealso cref="Avalonia.OpenGL.IGlPlatformSurfaceRenderingSession" />
            private class Session : IGlPlatformSurfaceRenderingSession
            {
                #region Fields

                /// <summary>
                /// The context
                /// </summary>
                private readonly GlxContext _context;

                /// <summary>
                /// The information
                /// </summary>
                private readonly EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo _info;

                /// <summary>
                /// The lock
                /// </summary>
                private readonly IDisposable _lock;

                #endregion Fields

                #region Constructors

                /// <summary>
                /// Initializes a new instance of the <see cref="Session" /> class.
                /// </summary>
                /// <param name="context">The context.</param>
                /// <param name="info">The information.</param>
                /// <param name="lock">The lock.</param>
                public Session(GlxContext context, EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo info,
                    IDisposable @lock)
                {
                    _context = context;
                    _info = info;
                    _lock = @lock;
                }

                #endregion Constructors

                #region Properties

                /// <summary>
                /// Gets the display.
                /// </summary>
                /// <value>The display.</value>
                public IGlDisplay Display => _context.Display;

                /// <summary>
                /// Gets a value indicating whether this instance is y flipped.
                /// </summary>
                /// <value><c>true</c> if this instance is y flipped; otherwise, <c>false</c>.</value>
                public bool IsYFlipped { get; }

                /// <summary>
                /// Gets the scaling.
                /// </summary>
                /// <value>The scaling.</value>
                public double Scaling => _info.Scaling;

                /// <summary>
                /// Gets the size.
                /// </summary>
                /// <value>The size.</value>
                public PixelSize Size => _info.Size;

                #endregion Properties

                #region Methods

                /// <summary>
                /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
                /// </summary>
                public void Dispose()
                {
                    _context.Display.GlInterface.Flush();
                    _context.Glx.WaitGL();
                    _context.Display.SwapBuffers(_info.Handle);
                    _context.Glx.WaitX();
                    _context.Display.ClearContext();
                    _lock.Dispose();
                }

                #endregion Methods
            }

            #endregion Classes
        }

        #endregion Classes
    }
}
