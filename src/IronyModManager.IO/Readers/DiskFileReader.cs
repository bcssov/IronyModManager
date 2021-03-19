// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 03-19-2021
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
using IronyModManager.Shared;

namespace IronyModManager.IO
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
        private static readonly string[] disallowedPaths = new string[] { Common.Constants.ModDirectory, Common.Constants.DLCDirectory };

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
        /// <returns><c>true</c> if this instance can read the specified path; otherwise, <c>false</c>.</returns>
        public virtual bool CanRead(string path)
        {
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
        public virtual (Stream, bool) GetStream(string rootPath, string file)
        {
            static FileStream readStream(string path)
            {
                var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                return fs;
            }
            // If using wildcard then we are going to match if it ends with and update this logic if ever needed
            if (file.StartsWith("*"))
            {
                var files = Directory.EnumerateFiles(rootPath, file, SearchOption.TopDirectoryOnly);
                if (files.Any())
                {
                    return (readStream(files.First()), new System.IO.FileInfo(files.First()).IsReadOnly);
                }
            }
            else
            {
                var fullPath = Path.Combine(rootPath, file);
                if (File.Exists(fullPath))
                {
                    return (readStream(fullPath), new System.IO.FileInfo(fullPath).IsReadOnly);
                }
            }
            return (null, false);
        }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.Int64.</returns>
        public virtual long GetTotalSize(string path)
        {
            var files = GetFiles(path);
            long total = 0;
            if (files.Any())
            {
                foreach (var item in files)
                {
                    var info = new System.IO.FileInfo(item);
                    if (info != null)
                    {
                        total += info.Length;
                    }
                }
            }
            return total;
        }

        /// <summary>
        /// Reads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="allowedPaths">The allowed paths.</param>
        /// <returns>IReadOnlyCollection&lt;IFileInfo&gt;.</returns>
        public virtual IReadOnlyCollection<IFileInfo> Read(string path, IEnumerable<string> allowedPaths)
        {
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            if (files?.Length > 0)
            {
                var result = new List<IFileInfo>();
                foreach (var file in files)
                {
                    var relativePath = file.Replace(path, string.Empty).Trim(Path.DirectorySeparatorChar);
                    if (!relativePath.Contains(Path.DirectorySeparatorChar) ||
                        relativePath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).Any(s => s.StartsWith(".")) ||
                        (allowedPaths?.Count() > 0 && !allowedPaths.Any(p => relativePath.StartsWith(p, StringComparison.OrdinalIgnoreCase))))
                    {
                        continue;
                    }
                    var info = DIResolver.Get<IFileInfo>();
                    var fileInfo = new System.IO.FileInfo(file);
                    info.IsReadOnly = fileInfo.IsReadOnly;
                    info.Size = fileInfo.Length;
                    using var stream = File.OpenRead(file);
                    info.FileName = relativePath;
                    if (Constants.TextExtensions.Any(s => file.EndsWith(s, StringComparison.OrdinalIgnoreCase)))
                    {
                        using var streamReader = new StreamReader(stream, true);
                        var text = streamReader.ReadToEnd();
                        streamReader.Close();
                        info.IsBinary = false;
                        info.Content = text.SplitOnNewLine(false);
                        info.ContentSHA = text.CalculateSHA();
                    }
                    else
                    {
                        info.IsBinary = true;
                        using var fs = new FileStream(file, FileMode.Open, FileAccess.Read);
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
