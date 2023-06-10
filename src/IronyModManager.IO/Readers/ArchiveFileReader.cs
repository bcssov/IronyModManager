
// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 06-10-2023
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
using SharpCompress.Archives;
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
        #region Fields

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveFileReader" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ArchiveFileReader(ILogger logger)
        {
            this.logger = logger;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Determines whether this instance [can list files] the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if this instance [can list files] the specified path; otherwise, <c>false</c>.</returns>
        public bool CanListFiles(string path)
        {
            return true;
        }

        /// <summary>
        /// Determines whether this instance can read the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="searchSubFolders">if set to <c>true</c> [search sub folders].</param>
        /// <returns><c>true</c> if this instance can read the specified path; otherwise, <c>false</c>.</returns>
        public virtual bool CanRead(string path, bool searchSubFolders = true)
        {
            return File.Exists(path) && (path.EndsWith(Constants.ZipExtension, StringComparison.OrdinalIgnoreCase) || path.EndsWith(Constants.BinExtension, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether this instance [can read stream] the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if this instance [can read stream] the specified path; otherwise, <c>false</c>.</returns>
        public bool CanReadStream(string path)
        {
            return true;
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>IList&lt;System.String&gt;.</returns>
        public virtual IEnumerable<string> GetFiles(string path)
        {
            using var zip = Ionic.Zip.ZipFile.Read(path);
            var files = new List<string>();
            foreach (var entry in zip.EntryFileNames)
            {
                if (!zip[entry].IsDirectory)
                {
                    var relativePath = entry.StandardizeDirectorySeparator().Trim(Path.DirectorySeparatorChar);
                    files.Add(relativePath);
                }
            }
            return files;
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="file">The file.</param>
        /// <returns>Stream.</returns>
        public virtual (Stream, bool, DateTime?, EncodingInfo) GetStream(string rootPath, string file)
        {
            (MemoryStream, EncodingInfo) readStream(Stream entryStream)
            {
                var memoryStream = new MemoryStream();
                entryStream.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return (memoryStream, memoryStream.GetEncodingInfo(file));
            }
            (MemoryStream, EncodingInfo) getUsingReaderFactory()
            {
                using var fileStream = File.OpenRead(rootPath);
                using var reader = ReaderFactory.Open(fileStream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        var relativePath = reader.Entry.Key.StandardizeDirectorySeparator().Trim(Path.DirectorySeparatorChar);
                        var filePath = file.StandardizeDirectorySeparator();

                        // If using wildcard then we are going to match if it ends with and update this logic if ever needed
                        if (file.StartsWith("*"))
                        {
                            var endsWith = file.Replace("*", string.Empty);
                            if (relativePath.EndsWith(endsWith, StringComparison.OrdinalIgnoreCase))
                            {
                                using var stream = reader.OpenEntryStream();
                                return readStream(stream);
                            }
                        }
                        else if (relativePath.Equals(filePath, StringComparison.OrdinalIgnoreCase))
                        {
                            using var stream = reader.OpenEntryStream();
                            return readStream(stream);
                        }
                    }
                }
                return (null, null);
            }
            (MemoryStream, EncodingInfo) getUsingArchiveFactory()
            {
                using var fileStream = File.OpenRead(rootPath);
                using var reader = ArchiveFactory.Open(fileStream);
                foreach (var entry in reader.Entries.Where(entry => !entry.IsDirectory))
                {
                    var relativePath = entry.Key.StandardizeDirectorySeparator().Trim(Path.DirectorySeparatorChar);
                    var filePath = file.StandardizeDirectorySeparator();

                    // If using wildcard then we are going to match if it ends with and update this logic if ever needed
                    if (file.StartsWith("*"))
                    {
                        var endsWith = file.Replace("*", string.Empty);
                        if (relativePath.EndsWith(endsWith, StringComparison.OrdinalIgnoreCase))
                        {
                            using var stream = entry.OpenEntryStream();
                            return readStream(stream);
                        }
                    }
                    else if (relativePath.Equals(filePath, StringComparison.OrdinalIgnoreCase))
                    {
                        using var stream = entry.OpenEntryStream();
                        return readStream(stream);
                    }
                }
                return (null, null);
            }

            // Return zip file last write time. Zip info can be unreliable if the client which created it actually didn't write the info in the first place (as far as I know)
            try
            {
                var result = getUsingArchiveFactory();
                return (result.Item1, false, new System.IO.FileInfo(rootPath).LastWriteTime, result.Item2);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                var result = getUsingReaderFactory();
                return (result.Item1, false, new System.IO.FileInfo(rootPath).LastWriteTime, result.Item2);
            }
        }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="extensions">The extensions.</param>
        /// <returns>System.Int64.</returns>
        public virtual long GetTotalSize(string path, string[] extensions = null)
        {
            long getUsingReaderFactory()
            {
                long total = 0;
                using var fileStream = File.OpenRead(path);
                using var reader = ReaderFactory.Open(fileStream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        if (extensions != null && extensions.Any())
                        {
                            if (extensions.Any(p => reader.Entry.Key.EndsWith(p, StringComparison.OrdinalIgnoreCase)))
                            {
                                total += reader.Entry.Size;
                            }
                        }
                        else
                        {
                            total += reader.Entry.Size;
                        }
                    }
                }
                return total;
            }
            long getUsingArchiveFactory()
            {
                long total = 0;
                using var fileStream = File.OpenRead(path);
                using var reader = ArchiveFactory.Open(fileStream);
                foreach (var entry in reader.Entries.Where(entry => !entry.IsDirectory))
                {
                    total += entry.Size;
                    if (extensions != null && extensions.Any())
                    {
                        if (extensions.Any(p => entry.Key.EndsWith(p, StringComparison.OrdinalIgnoreCase)))
                        {
                            total += entry.Size;
                        }
                    }
                    else
                    {
                        total += entry.Size;
                    }
                }
                return total;
            }
            try
            {
                return getUsingArchiveFactory();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return getUsingReaderFactory();
            }
        }

        /// <summary>
        /// Reads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="allowedPaths">The allowed paths.</param>
        /// <param name="searchSubFolders">if set to <c>true</c> [search sub folders].</param>
        /// <returns>IReadOnlyCollection&lt;IFileInfo&gt;.</returns>
        public virtual IReadOnlyCollection<IFileInfo> Read(string path, IEnumerable<string> allowedPaths = null, bool searchSubFolders = true)
        {
            var result = new List<IFileInfo>();

            void parseUsingReaderFactory()
            {
                using var fileStream = File.OpenRead(path);
                var modified = new System.IO.FileInfo(path).LastWriteTime;
                using var reader = ReaderFactory.Open(fileStream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        var relativePath = reader.Entry.Key.StandardizeDirectorySeparator().Trim(Path.DirectorySeparatorChar);
                        if (searchSubFolders)
                        {
                            if (!relativePath.Contains(Path.DirectorySeparatorChar) ||
                                relativePath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).Any(s => s.StartsWith(".") ||
                                (allowedPaths?.Count() > 0 && !allowedPaths.Any(p => relativePath.StartsWith(p, StringComparison.OrdinalIgnoreCase)))))
                            {
                                continue;
                            }
                        }
                        else if (!searchSubFolders && !Path.GetDirectoryName(relativePath).Equals(path))
                        {
                            continue;
                        }
                        var info = DIResolver.Get<IFileInfo>();
                        info.IsReadOnly = false;
                        info.LastModified = modified;
                        info.Size = reader.Entry.Size;
                        using var entryStream = reader.OpenEntryStream();
                        using var memoryStream = new MemoryStream();
                        entryStream.CopyTo(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        info.Encoding = memoryStream.GetEncodingInfo(relativePath);
                        info.FileName = relativePath;
                        if (FileSignatureUtility.IsTextFile(reader.Entry.Key, memoryStream))
                        {
                            using var streamReader = new StreamReader(memoryStream, true);
                            var text = streamReader.ReadToEnd();
                            streamReader.Close();
                            streamReader.Dispose();
                            info.IsBinary = false;
                            info.Content = text.SplitOnNewLine(false);
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
            }

            void parseUsingArchiveFactory()
            {
                using var fileStream = File.OpenRead(path);
                var modified = new System.IO.FileInfo(path).LastWriteTime;
                using var reader = ArchiveFactory.Open(fileStream);
                foreach (var entry in reader.Entries.Where(entry => !entry.IsDirectory))
                {
                    var relativePath = entry.Key.StandardizeDirectorySeparator().Trim(Path.DirectorySeparatorChar);
                    if (searchSubFolders)
                    {
                        if (!relativePath.Contains(Path.DirectorySeparatorChar) ||
                            relativePath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).Any(s => s.StartsWith(".") ||
                            (allowedPaths?.Count() > 0 && !allowedPaths.Any(p => relativePath.StartsWith(p, StringComparison.OrdinalIgnoreCase)))))
                        {
                            continue;
                        }
                    }
                    else if (!searchSubFolders && !Path.GetDirectoryName(relativePath).Equals(path))
                    {
                        continue;
                    }
                    var info = DIResolver.Get<IFileInfo>();
                    info.IsReadOnly = false;
                    info.LastModified = modified;
                    info.Size = entry.Size;
                    using var entryStream = entry.OpenEntryStream();
                    using var memoryStream = new MemoryStream();
                    entryStream.CopyTo(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    info.Encoding = memoryStream.GetEncodingInfo(relativePath);
                    info.FileName = relativePath;
                    if (FileSignatureUtility.IsTextFile(entry.Key, memoryStream))
                    {
                        using var streamReader = new StreamReader(memoryStream, true);
                        var text = streamReader.ReadToEnd();
                        streamReader.Close();
                        streamReader.Dispose();
                        info.IsBinary = false;
                        info.Content = text.SplitOnNewLine(false);
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
            try
            {
                parseUsingArchiveFactory();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                result = new List<IFileInfo>();
                parseUsingReaderFactory();
            }

            return result.Count != 0 ? result : null;
        }

        #endregion Methods
    }
}
