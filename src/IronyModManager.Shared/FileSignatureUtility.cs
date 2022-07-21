// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 07-20-2022
//
// Last Modified By : Mario
// Last Modified On : 07-20-2022
// ***********************************************************************
// <copyright file="FileSignatureUtility.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class FileSignatureUtility.
    /// </summary>
    public static class FileSignatureUtility
    {
        #region Fields

        /// <summary>
        /// The image identifier
        /// </summary>
        private const string ImageId = "image";

        /// <summary>
        /// The text identifier
        /// </summary>
        private const string TextId = "text";

        #endregion Fields

        #region Methods

        /// <summary>
        /// Determines whether [is image file] [the specified filename].
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns><c>true</c> if [is image file] [the specified filename]; otherwise, <c>false</c>.</returns>
        public static bool IsImageFile(string filename)
        {
            return IsOfType(filename, ImageId, Constants.ImageExtensions);
        }

        /// <summary>
        /// Determines whether [is text file] [the specified filename].
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="stream">The stream.</param>
        /// <returns><c>true</c> if [is text file] [the specified filename]; otherwise, <c>false</c>.</returns>
        public static bool IsTextFile(string filename, Stream stream)
        {
            return IsOfType(filename, stream, TextId, Constants.TextExtensions);
        }

        /// <summary>
        /// Determines whether [is text file] [the specified filename].
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns><c>true</c> if [is text file] [the specified filename]; otherwise, <c>false</c>.</returns>
        public static bool IsTextFile(string filename)
        {
            return IsOfType(filename, TextId, Constants.TextExtensions);
        }

        /// <summary>
        /// Determines whether [is of type] [the specified filename].
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="extensions">The extensions.</param>
        /// <returns><c>true</c> if [is of type] [the specified filename]; otherwise, <c>false</c>.</returns>
        private static bool IsOfType(string filename, Stream stream, string id, string[] extensions)
        {
            if (stream == null)
            {
                return false;
            }
            var ms = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(ms);
            stream.Seek(0, SeekOrigin.Begin);
            ms.Seek(0, SeekOrigin.Begin);
            if (PommaLabs.MimeTypes.MimeTypeMap.TryGetMimeType(ms, Path.GetFileName(filename), out var mimeType))
            {
                ms.Close();
                ms.Dispose();
                return mimeType.StartsWith(id, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                mimeType = MimeTypes.GetMimeType(filename);
                if (mimeType != MimeTypes.FallbackMimeType)
                {
                    ms.Close();
                    ms.Dispose();
                    return mimeType.StartsWith(id, StringComparison.OrdinalIgnoreCase);
                }
            }
            ms.Close();
            ms.Dispose();
            return extensions.Any(s => s.EndsWith(filename, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is of type] [the specified filename].
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="extensions">The extensions.</param>
        /// <returns><c>true</c> if [is of type] [the specified filename]; otherwise, <c>false</c>.</returns>
        private static bool IsOfType(string filename, string id, string[] extensions)
        {
            if (PommaLabs.MimeTypes.MimeTypeMap.TryGetMimeType(Path.GetFileName(filename), out var mimeType))
            {
                return mimeType.StartsWith(id, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                mimeType = MimeTypes.GetMimeType(filename);
                if (mimeType != MimeTypes.FallbackMimeType)
                {
                    return mimeType.StartsWith(id, StringComparison.OrdinalIgnoreCase);
                }
            }
            return extensions.Any(s => s.EndsWith(filename, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Methods
    }
}
