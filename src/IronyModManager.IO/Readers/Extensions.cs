// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 07-18-2022
//
// Last Modified By : Mario
// Last Modified On : 07-18-2022
// ***********************************************************************
// <copyright file="Extensions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.Shared;
using UtfUnknown;

namespace IronyModManager.IO.Readers
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    internal static class Extensions
    {
        #region Methods

        /// <summary>
        /// Gets the encoding information.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="file">The file.</param>
        /// <returns>Common.Readers.EncodingInfo.</returns>
        public static EncodingInfo GetEncodingInfo(this Stream stream, string file)
        {
            if (!Constants.TextExtensions.Any(s => file.EndsWith(s, StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            stream.Seek(0, SeekOrigin.Begin);
            var encoding = CharsetDetector.DetectFromStream(memoryStream);
            if (encoding == null || encoding.Detected == null)
            {
                return null;
            }
            var result = new EncodingInfo() { Encoding = encoding.Detected.Encoding, HasBOM = encoding.Detected.HasBOM };
            memoryStream.Close();
            memoryStream.Dispose();
            return result;
        }

        /// <summary>
        /// Gets the encoding information.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Common.Readers.EncodingInfo.</returns>
        public static EncodingInfo GetEncodingInfo(this string file)
        {
            if (!Constants.TextExtensions.Any(s => file.EndsWith(s, StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }
            var encoding = CharsetDetector.DetectFromFile(file);
            if (encoding == null || encoding.Detected == null)
            {
                return null;
            }
            var result = new EncodingInfo() { Encoding = encoding.Detected.Encoding, HasBOM = encoding.Detected.HasBOM };
            return result;
        }

        /// <summary>
        /// Gets the encoding information.
        /// </summary>
        /// <param name="fileInfo">The file information.</param>
        /// <returns>Common.Readers.EncodingInfo.</returns>
        public static EncodingInfo GetEncodingInfo(this System.IO.FileInfo fileInfo)
        {
            if (!Constants.TextExtensions.Any(s => fileInfo.FullName.EndsWith(s, StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }
            var encoding = CharsetDetector.DetectFromFile(fileInfo);
            if (encoding == null || encoding.Detected == null)
            {
                return null;
            }
            var result = new EncodingInfo() { Encoding = encoding.Detected.Encoding, HasBOM = encoding.Detected.HasBOM };
            return result;
        }

        #endregion Methods
    }
}
