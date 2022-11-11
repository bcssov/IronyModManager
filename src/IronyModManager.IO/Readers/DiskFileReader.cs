// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 11-06-2022
// ***********************************************************************
// <copyright file="DiskFileReader.cs" company="Mario">
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
using IronyModManager.IO.Common.Streams;
using IronyModManager.Shared;

namespace IronyModManager.IO.Readers
{
    /// <summary>
    /// Class DiskFileReader.
    /// Implements the <see cref="IronyModManager.IO.Common.Readers.IFileReader" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Readers.IFileReader" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class DiskFileReader : IFileReader
    {
        #region Fields

        /// <summary>
        /// The disallowed paths
        /// </summary>
        private static readonly string[] disallowedPaths = new string[] { Common.Constants.ModDirectory,
            Common.Constants.DLCDirectory, Common.Constants.BuiltInDLCDirectory, Common.Constants.JsonModDirectoy };

        #endregion Fields

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
            if (!searchSubFolders)
            {
                return Directory.Exists(path);
            }
            return Directory.Exists(path) && !disallowedPaths.Any(p => path.EndsWith(p, StringComparison.OrdinalIgnoreCase));
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
            var files = new List<string>();
            foreach (var item in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            {
                var relativePath = item.Replace(path, string.Empty).Trim(Path.DirectorySeparatorChar);
                files.Add(relativePath);
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
            static OnDemandFileStream readStream(string path)
            {
                var fs = new OnDemandFileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                return fs;
            }
            // If using wildcard then we are going to match if it ends with and update this logic if ever needed
            if (file.StartsWith("*"))
            {
                var files = Directory.EnumerateFiles(rootPath, file, SearchOption.TopDirectoryOnly);
                if (files.Any())
                {
                    var fInfo = new System.IO.FileInfo(files.First());
                    var isreadOnly = fInfo.IsReadOnly;
                    var modified = fInfo.LastWriteTime;
                    return (readStream(files.First()), isreadOnly, modified, fInfo.GetEncodingInfo());
                }
            }
            else
            {
                var fullPath = Path.Combine(rootPath, file);
                if (File.Exists(fullPath))
                {
                    var fInfo = new System.IO.FileInfo(fullPath);
                    var isreadOnly = fInfo.IsReadOnly;
                    var modified = fInfo.LastWriteTime;
                    return (readStream(fullPath), isreadOnly, modified, fInfo.GetEncodingInfo());
                }
            }
            return (null, false, null, null);
        }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.Int64.</returns>
        public virtual long GetTotalSize(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            if (info.Exists)
            {
                var total = info.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(fi => fi.Length);
                return total;
            }
            return 0;
        }

        /// <summary>
        /// Reads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="allowedPaths">The allowed paths.</param>
        /// <param name="searchSubFolders">if set to <c>true</c> [search sub folders].</param>
        /// <returns>IReadOnlyCollection&lt;IFileInfo&gt;.</returns>
        public virtual IReadOnlyCollection<IFileInfo> Read(string path, IEnumerable<string> allowedPaths, bool searchSubFolders = true)
        {
            var files = Directory.GetFiles(path, "*", searchSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            if (files?.Length > 0)
            {
                var result = new List<IFileInfo>();
                foreach (var file in files)
                {
                    var relativePath = file.Replace(path, string.Empty).Trim(Path.DirectorySeparatorChar);
                    if (searchSubFolders)
                    {
                        if (!relativePath.Contains(Path.DirectorySeparatorChar) ||
                            relativePath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).Any(s => s.StartsWith(".")) ||
                            (allowedPaths?.Count() > 0 && !allowedPaths.Any(p => relativePath.StartsWith(p, StringComparison.OrdinalIgnoreCase))))
                        {
                            continue;
                        }
                    }
                    var info = DIResolver.Get<IFileInfo>();
                    var fileInfo = new System.IO.FileInfo(file);
                    info.IsReadOnly = fileInfo.IsReadOnly;
                    info.Size = fileInfo.Length;
                    info.LastModified = fileInfo.LastWriteTime;
                    using var stream = File.OpenRead(file);
                    info.FileName = relativePath;
                    info.Encoding = fileInfo.GetEncodingInfo();
                    if (FileSignatureUtility.IsTextFile(file, stream))
                    {
                        using var streamReader = new StreamReader(stream, true);
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
                        using var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                        info.ContentSHA = fs.CalculateSHA();
                    }
                    result.Add(info);
                }
                return result;
            }
            return null;
        }

        #endregion Methods
    }
}
