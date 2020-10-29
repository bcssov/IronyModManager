// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="X11IconLoader.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ************************************************************************

using System;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Class X11IconData.
    /// Implements the <see cref="Avalonia.Platform.IWindowIconImpl" />
    /// Implements the <see cref="Avalonia.Controls.Platform.Surfaces.IFramebufferPlatformSurface" />
    /// </summary>
    /// <seealso cref="Avalonia.Platform.IWindowIconImpl" />
    /// <seealso cref="Avalonia.Controls.Platform.Surfaces.IFramebufferPlatformSurface" />
    internal unsafe class X11IconData : IWindowIconImpl, IFramebufferPlatformSurface
    {
        #region Fields

        /// <summary>
        /// The bdata
        /// </summary>
        private readonly uint[] _bdata;

        /// <summary>
        /// The height
        /// </summary>
        private readonly int _height;

        /// <summary>
        /// The width
        /// </summary>
        private readonly int _width;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="X11IconData"/> class.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        public X11IconData(Bitmap bitmap)
        {
            _width = Math.Min(bitmap.PixelSize.Width, 128);
            _height = Math.Min(bitmap.PixelSize.Height, 128);
            _bdata = new uint[_width * _height];
            fixed (void* ptr = _bdata)
            {
                var iptr = (int*)ptr;
                iptr[0] = _width;
                iptr[1] = _height;
            }
            using (var rt = AvaloniaLocator.Current.GetService<IPlatformRenderInterface>().CreateRenderTarget(new[] { this }))
            using (var ctx = rt.CreateDrawingContext(null))
                ctx.DrawImage(bitmap.PlatformImpl, 1, new Rect(bitmap.Size),
                    new Rect(0, 0, _width, _height));
            Data = new UIntPtr[_width * _height + 2];
            Data[0] = new UIntPtr((uint)_width);
            Data[1] = new UIntPtr((uint)_height);
            for (var y = 0; y < _height; y++)
            {
                var r = y * _width;
                for (var x = 0; x < _width; x++)
                    Data[r + x] = new UIntPtr(_bdata[r + x]);
            }

            _bdata = null;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        public UIntPtr[] Data { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Locks this instance.
        /// </summary>
        /// <returns>ILockedFramebuffer.</returns>
        public ILockedFramebuffer Lock()
        {
            var h = GCHandle.Alloc(_bdata, GCHandleType.Pinned);
            return new LockedFramebuffer(h.AddrOfPinnedObject(), new PixelSize(_width, _height), _width * 4,
                new Vector(96, 96), PixelFormat.Bgra8888,
                () => h.Free());
        }

        /// <summary>
        /// Saves the specified output stream.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        public void Save(Stream outputStream)
        {
            using var wr =
                new WriteableBitmap(new PixelSize(_width, _height), new Vector(96, 96), PixelFormat.Bgra8888);
            using (var fb = wr.Lock())
            {
                var fbp = (uint*)fb.Address;
                for (var y = 0; y < _height; y++)
                {
                    var r = y * _width;
                    var fbr = y * fb.RowBytes / 4;
                    for (var x = 0; x < _width; x++)
                        fbp[fbr + x] = Data[r + x].ToUInt32();
                }
            }
            wr.Save(outputStream);
        }

        #endregion Methods
    }

    /// <summary>
    /// Class X11IconLoader.
    /// Implements the <see cref="Avalonia.Platform.IPlatformIconLoader" />
    /// </summary>
    /// <seealso cref="Avalonia.Platform.IPlatformIconLoader" />
    internal class X11IconLoader : IPlatformIconLoader
    {
        /// <summary>
        /// The X11
        /// </summary>
#pragma warning disable IDE0052 // Remove unread private members

        #region Fields

        private readonly X11Info _x11;

        #endregion Fields

#pragma warning restore IDE0052 // Remove unread private members

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="X11IconLoader"/> class.
        /// </summary>
        /// <param name="x11">The X11.</param>
        public X11IconLoader(X11Info x11)
        {
            _x11 = x11;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Loads the icon.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>IWindowIconImpl.</returns>
        public IWindowIconImpl LoadIcon(string fileName) => LoadIcon(new Bitmap(fileName));

        /// <summary>
        /// Loads the icon.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>IWindowIconImpl.</returns>
        public IWindowIconImpl LoadIcon(Stream stream) => LoadIcon(new Bitmap(stream));

        /// <summary>
        /// Loads the icon.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns>IWindowIconImpl.</returns>
        public IWindowIconImpl LoadIcon(IBitmapImpl bitmap)
        {
            var ms = new MemoryStream();
            bitmap.Save(ms);
            ms.Position = 0;
            return LoadIcon(ms);
        }

        /// <summary>
        /// Loads the icon.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns>IWindowIconImpl.</returns>
        private IWindowIconImpl LoadIcon(Bitmap bitmap)
        {
            var rv = new X11IconData(bitmap);
            bitmap.Dispose();
            return rv;
        }

        #endregion Methods
    }
}
