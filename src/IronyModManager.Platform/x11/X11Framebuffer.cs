// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="X11Framebuffer.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ************************************************************************

using System;
using Avalonia;
using Avalonia.Platform;
using IronyModManager.Shared;
using static IronyModManager.Platform.x11.XLib;

namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Class X11Framebuffer.
    /// Implements the <see cref="Avalonia.Platform.ILockedFramebuffer" />
    /// </summary>
    /// <seealso cref="Avalonia.Platform.ILockedFramebuffer" />
    [ExcludeFromCoverage("External component.")]
    internal class X11Framebuffer : ILockedFramebuffer
    {
        #region Fields

        /// <summary>
        /// The BLOB
        /// </summary>
        private readonly IUnmanagedBlob _blob;

        /// <summary>
        /// The depth
        /// </summary>
        private readonly int _depth;

        /// <summary>
        /// The display
        /// </summary>
        private readonly IntPtr _display;

        /// <summary>
        /// The xid
        /// </summary>
        private readonly IntPtr _xid;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="X11Framebuffer"/> class.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="xid">The xid.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="factor">The factor.</param>
        public X11Framebuffer(IntPtr display, IntPtr xid, int depth, int width, int height, double factor)
        {
            // HACK! Please fix renderer, should never ask for 0x0 bitmap.
            width = Math.Max(1, width);
            height = Math.Max(1, height);

            _display = display;
            _xid = xid;
            _depth = depth;
            Size = new PixelSize(width, height);
            RowBytes = width * 4;
            Dpi = new Vector(96, 96) * factor;
            Format = PixelFormat.Bgra8888;
            _blob = AvaloniaLocator.Current.GetService<IRuntimePlatform>().AllocBlob(RowBytes * height);
            Address = _blob.Address;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <value>The address.</value>
        public IntPtr Address { get; }

        /// <summary>
        /// Gets the dpi.
        /// </summary>
        /// <value>The dpi.</value>
        public Vector Dpi { get; }

        /// <summary>
        /// Gets the format.
        /// </summary>
        /// <value>The format.</value>
        public PixelFormat Format { get; }

        /// <summary>
        /// Gets the row bytes.
        /// </summary>
        /// <value>The row bytes.</value>
        public int RowBytes { get; }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public PixelSize Size { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            var image = new XImage();
            int bitsPerPixel = 32;
            image.width = Size.Width;
            image.height = Size.Height;
            image.format = 2; //ZPixmap;
            image.data = Address;
            image.byte_order = 0;// LSBFirst;
            image.bitmap_unit = bitsPerPixel;
            image.bitmap_bit_order = 0;// LSBFirst;
            image.bitmap_pad = bitsPerPixel;
            image.depth = _depth;
            image.bytes_per_line = RowBytes;
            image.bits_per_pixel = bitsPerPixel;
            XLockDisplay(_display);
            XInitImage(ref image);
            var gc = XCreateGC(_display, _xid, 0, IntPtr.Zero);
            XPutImage(_display, _xid, gc, ref image, 0, 0, 0, 0, (uint)Size.Width, (uint)Size.Height);
            XFreeGC(_display, gc);
            XSync(_display, true);
            XUnlockDisplay(_display);
            _blob.Dispose();
        }

        #endregion Methods
    }
}
