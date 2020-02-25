// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 02-25-2020
// ***********************************************************************
// <copyright file="ArchiveFileReader.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.IO.Common;
using IronyModManager.Shared;
using SharpCompress.Readers;

namespace IronyModManager.IO
{
    /// <summary>
    /// Class ArchiveFileReader.
    /// Implements the <see cref="IronyModManager.IO.Common.IFileReader" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.IFileReader" />
    [ExcludeFromCoverage("Shloud be covered by Unit tests from source project.")]
    public class ArchiveFileReader : IFileReader
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can read the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if this instance can read the specified path; otherwise, <c>false</c>.</returns>
        public bool CanRead(string path)
        {
            return File.Exists(path) && path.EndsWith(Common.Constants.ZipExtension, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Reads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>IReadOnlyCollection&lt;IFileInfo&gt;.</returns>
        public IReadOnlyCollection<IFileInfo> Read(string path)
        {
            using var fileStream = File.OpenRead(path);
            using var reader = ReaderFactory.Open(fileStream);
            var result = new List<IFileInfo>();
            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    var relativePath = reader.Entry.Key.Trim("\\/".ToCharArray()).Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                    if (!relativePath.Contains(Path.DirectorySeparatorChar))
                    {
                        continue;
                    }
                    var info = DIResolver.Get<IFileInfo>();
                    using var entryStream = reader.OpenEntryStream();
                    using var memoryStream = new MemoryStream();
                    entryStream.CopyTo(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    info.FileName = relativePath;
                    if (Shared.Constants.TextExtensions.Any(s => reader.Entry.Key.EndsWith(s, StringComparison.OrdinalIgnoreCase)))
                    {
                        using var streamReader = new StreamReader(memoryStream, true);
                        var text = streamReader.ReadToEnd();
                        streamReader.Close();
                        info.IsBinary = false;
                        info.Content = text.SplitOnNewLine();
                        info.ContentSHA = text.CalculateSHA();
                    }
                    else
                    {
                        info.IsBinary = true;
                        info.ContentSHA = memoryStream.CalculateSHA();
                    }
                    result.Add(info);
                }
            }
            return result.Count != 0 ? result : null;
        }

        #endregion Methods
    }
}
