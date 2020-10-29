// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="GlxPlatformFeature.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ***********************************************************************
using System;
using Avalonia;
using Avalonia.Logging;
using Avalonia.OpenGL;

namespace IronyModManager.Platform.x11.Glx
{
    /// <summary>
    /// Class GlxGlPlatformFeature.
    /// Implements the <see cref="Avalonia.OpenGL.IWindowingPlatformGlFeature" />
    /// </summary>
    /// <seealso cref="Avalonia.OpenGL.IWindowingPlatformGlFeature" />
    internal class GlxGlPlatformFeature : IWindowingPlatformGlFeature
    {
        #region Properties

        /// <summary>
        /// Gets the deferred context.
        /// </summary>
        /// <value>The deferred context.</value>
        public GlxContext DeferredContext { get; private set; }

        /// <summary>
        /// Gets the display.
        /// </summary>
        /// <value>The display.</value>
        public GlxDisplay Display { get; private set; }

        /// <summary>
        /// Gets the immediate context.
        /// </summary>
        /// <value>The immediate context.</value>
        public IGlContext ImmediateContext { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Tries the create.
        /// </summary>
        /// <param name="x11">The X11.</param>
        /// <returns>GlxGlPlatformFeature.</returns>
        public static GlxGlPlatformFeature TryCreate(X11Info x11)
        {
            try
            {
                var disp = new GlxDisplay(x11);
                return new GlxGlPlatformFeature
                {
                    Display = disp,
                    ImmediateContext = disp.ImmediateContext,
                    DeferredContext = disp.DeferredContext
                };
            }
            catch (Exception e)
            {
                Logger.TryGet(LogEventLevel.Error)?.Log("OpenGL", null, "Unable to initialize GLX-based rendering: {0}", e);
                return null;
            }
        }

        /// <summary>
        /// Tries the initialize.
        /// </summary>
        /// <param name="x11">The X11.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool TryInitialize(X11Info x11)
        {
            var feature = TryCreate(x11);
            if (feature != null)
            {
                AvaloniaLocator.CurrentMutable.Bind<IWindowingPlatformGlFeature>().ToConstant(feature);
                return true;
            }

            return false;
        }

        #endregion Methods
    }
}
