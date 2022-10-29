// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 07-20-2022
//
// Last Modified By : Mario
// Last Modified On : 10-29-2022
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
        private static readonly string[] ImageIds = new[] { "image" };

        /// <summary>
        /// The text identifier
        /// </summary>
        private static readonly string[] TextIds = new[] { "text", "application/json" };

        #endregion Fields

        #region Methods

        /// <summary>
        /// Determines whether [is image file] [the specified filename].
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns><c>true</c> if [is image file] [the specified filename]; otherwise, <c>false</c>.</returns>
        public static bool IsImageFile(string filename)
        {
            return IsOfType(filename, ImageIds, Constants.ImageExtensions);
        }

        /// <summary>
        /// Determines whether [is text file] [the specified filename].
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="stream">The stream.</param>
        /// <returns><c>true</c> if [is text file] [the specified filename]; otherwise, <c>false</c>.</returns>
        public static bool IsTextFile(string filename, Stream stream)
        {
            return IsOfType(filename, stream, TextIds, Constants.TextExtensions);
        }

        /// <summary>
        /// Determines whether [is text file] [the specified filename].
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns><c>true</c> if [is text file] [the specified filename]; otherwise, <c>false</c>.</returns>
        public static bool IsTextFile(string filename)
        {
            return IsOfType(filename, TextIds, Constants.TextExtensions);
        }

        /// <summary>
        /// Determines whether [is of type] [the specified filename].
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="ids">The ids.</param>
        /// <param name="extensions">The extensions.</param>
        /// <returns><c>true</c> if [is of type] [the specified filename]; otherwise, <c>false</c>.</returns>
        private static bool IsOfType(string filename, Stream stream, string[] ids, string[] extensions)
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
                var result = ids.Any(p => mimeType.StartsWith(p, StringComparison.OrdinalIgnoreCase) && IsValidTextFile(ms));
                ms.Close();
                ms.Dispose();
                return result;
            }
            else
            {
                mimeType = MimeTypes.GetMimeType(filename);
                if (mimeType != MimeTypes.FallbackMimeType)
                {
                    ms.Close();
                    ms.Dispose();
                    return ids.Any(p => mimeType.StartsWith(p, StringComparison.OrdinalIgnoreCase));
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
        /// <param name="ids">The ids.</param>
        /// <param name="extensions">The extensions.</param>
        /// <returns><c>true</c> if [is of type] [the specified filename]; otherwise, <c>false</c>.</returns>
        private static bool IsOfType(string filename, string[] ids, string[] extensions)
        {
            if (PommaLabs.MimeTypes.MimeTypeMap.TryGetMimeType(Path.GetFileName(filename), out var mimeType))
            {
                return ids.Any(p => mimeType.StartsWith(p, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                mimeType = MimeTypes.GetMimeType(filename);
                if (mimeType != MimeTypes.FallbackMimeType)
                {
                    return ids.Any(p => mimeType.StartsWith(p, StringComparison.OrdinalIgnoreCase));
                }
            }
            return extensions.Any(s => s.EndsWith(filename, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is valid text file] [the specified stream].
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns><c>true</c> if [is valid text file] [the specified stream]; otherwise, <c>false</c>.</returns>
        private static bool IsValidTextFile(Stream stream)
        {
            // Same as the library does only backwards just to be sure
            try
            {
                if (stream.Length < 512)
                {
                    return true;
                }
                stream.Seek(-512, SeekOrigin.End);
                var buffer = new byte[512];
                var byteCount = stream.Read(buffer, 0, buffer.Length);

                for (var i = 1; i < byteCount; i++)
                {
                    if (buffer[i] == 0x00 && buffer[i - 1] == 0x00)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return true;
            }
        }

        #endregion Methods
    }
}
