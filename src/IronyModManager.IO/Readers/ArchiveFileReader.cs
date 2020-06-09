// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 06-08-2020
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
using IronyModManager.IO.Common.Readers;
using IronyModManager.Shared;
using SharpCompress.Readers;

namespace IronyModManager.IO.Readers
{
    /// <summary>
    /// Class ArchiveFileReader.
    /// Implements the <see cref="IronyModManager.IO.Common.Readers.IFileReader" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Readers.IFileReader" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class ArchiveFileReader : IFileReader
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can read the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if this instance can read the specified path; otherwise, <c>false</c>.</returns>
        public virtual bool CanRead(string path)
        {
            return File.Exists(path) && path.EndsWith(Constants.ZipExtension, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="file">The file.</param>
        /// <returns>Stream.</returns>
        public virtual Stream GetStream(string rootPath, string file)
        {
            static MemoryStream readStream(SharpCompress.Readers.IReader reader)
            {
                using var entryStream = reader.OpenEntryStream();
                var memoryStream = new MemoryStream();
                entryStream.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            }

            using var fileStream = File.OpenRead(rootPath);
            using var reader = ReaderFactory.Open(fileStream);
            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    var relativePath = reader.Entry.Key.Trim("\\/".ToCharArray()).Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                    var filePath = file.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                    // If using wildcard then we are going to match if it ends with and update this logic if ever needed
                    if (file.StartsWith("*"))
                    {
                        var endsWith = file.Replace("*", string.Empty);
                        if (relativePath.EndsWith(endsWith, StringComparison.OrdinalIgnoreCase))
                        {
                            return readStream(reader);
                        }
                    }
                    else if (relativePath.Equals(filePath, StringComparison.OrdinalIgnoreCase))
                    {
                        return readStream(reader);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Reads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>IReadOnlyCollection&lt;IFileInfo&gt;.</returns>
        public virtual IReadOnlyCollection<IFileInfo> Read(string path)
        {
            using var fileStream = File.OpenRead(path);
            using var reader = ReaderFactory.Open(fileStream);
            var result = new List<IFileInfo>();
            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    var relativePath = reader.Entry.Key.Trim("\\/".ToCharArray()).Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                    if (!relativePath.Contains(Path.DirectorySeparatorChar) || relativePath.StartsWith("."))
                    {
                        continue;
                    }
                    var info = DIResolver.Get<IFileInfo>();
                    using var entryStream = reader.OpenEntryStream();
                    using var memoryStream = new MemoryStream();
                    entryStream.CopyTo(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    info.FileName = relativePath;
                    if (Constants.TextExtensions.Any(s => reader.Entry.Key.EndsWith(s, StringComparison.OrdinalIgnoreCase)))
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
