// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="X11FramebufferSurface.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ************************************************************************

using System;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Platform;
using static IronyModManager.Platform.x11.XLib;

namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Class X11FramebufferSurface.
    /// Implements the <see cref="Avalonia.Controls.Platform.Surfaces.IFramebufferPlatformSurface" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.Platform.Surfaces.IFramebufferPlatformSurface" />
    public class X11FramebufferSurface : IFramebufferPlatformSurface
    {
        #region Fields

        /// <summary>
        /// The depth
        /// </summary>
        private readonly int _depth;

        /// <summary>
        /// The display
        /// </summary>
        private readonly IntPtr _display;

        /// <summary>
        /// The scaling
        /// </summary>
        private readonly Func<double> _scaling;

        /// <summary>
        /// The xid
        /// </summary>
        private readonly IntPtr _xid;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="X11FramebufferSurface"/> class.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="xid">The xid.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="scaling">The scaling.</param>
        public X11FramebufferSurface(IntPtr display, IntPtr xid, int depth, Func<double> scaling)
        {
            _display = display;
            _xid = xid;
            _depth = depth;
            _scaling = scaling;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Locks this instance.
        /// </summary>
        /// <returns>ILockedFramebuffer.</returns>
        public ILockedFramebuffer Lock()
        {
            XLockDisplay(_display);
            XGetGeometry(_display, _xid, out var root, out var x, out var y, out var width, out var height,
                out var bw, out var d);
            XUnlockDisplay(_display);
            return new X11Framebuffer(_display, _xid, _depth, width, height, _scaling());
        }

        #endregion Methods
    }
}
