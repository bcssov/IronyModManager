// ***********************************************************************
// Assembly         : IronyModManager.IO
// ***********************************************************************

using System.IO;
using ImageMagick;

namespace IronyModManager.IO.Images
{
    /// <summary>
    /// Reads the narrowly supported DDS layouts which require compatibility handling.
    /// </summary>
    internal interface IDdsCompatibilityReader
    {
        /// <summary>
        /// Attempts to read a precisely supported compatibility DDS layout.
        /// </summary>
        /// <param name="stream">The DDS stream.</param>
        /// <returns>An image when the exact compatibility layout matches; otherwise, <c>null</c>.</returns>
        MagickImage TryRead(Stream stream);
    }
}
