// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-13-2021
//
// Last Modified By : Mario
// Last Modified On : 03-17-2021
// ***********************************************************************
// <copyright file="BaseSpecializedDiskReader.cs" company="Mario">
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

namespace IronyModManager.IO.Readers
{
    /// <summary>
    /// Class BaseSpecializedDiskReader.
    /// Implements the <see cref="IronyModManager.IO.Common.Readers.IFileReader" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Readers.IFileReader" />
    public abstract class BaseSpecializedDiskReader : IFileReader
    {
        #region Properties

        /// <summary>
        /// Gets the search extension.
        /// </summary>
        /// <value>The search extension.</value>
        public abstract string SearchExtension { get; }

        /// <summary>
        /// Gets the search option.
        /// </summary>
        /// <value>The search option.</value>
        public abstract SearchOption SearchOption { get; }

        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        /// <value>The search pattern.</value>
        public abstract string SearchPattern { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance [can list files] the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if this instance [can list files] the specified path; otherwise, <c>false</c>.</returns>
        public virtual bool CanListFiles(string path)
        {
            return false;
        }

        /// <summary>
        /// Determines whether this instance can read the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if this instance can read the specified path; otherwise, <c>false</c>.</returns>
        public virtual bool CanRead(string path)
        {
            return Directory.Exists(path) && path.EndsWith(SearchExtension, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether this instance [can read stream] the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if this instance [can read stream] the specified path; otherwise, <c>false</c>.</returns>
        public virtual bool CanReadStream(string path)
        {
            return false;
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>IList&lt;System.String&gt;.</returns>
        /// <exception cref="NotSupportedException"></exception>
        public virtual IEnumerable<string> GetFiles(string path)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="file">The file.</param>
        /// <returns>System.Int64.</returns>
        public virtual long GetFileSize(string rootPath, string file)
        {
            var path = Path.Combine(rootPath, file);
            if (File.Exists(path))
            {
                return new System.IO.FileInfo(path).Length;
            }
            return 0;
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="file">The file.</param>
        /// <returns>Stream.</returns>
        /// <exception cref="NotSupportedException"></exception>
        public virtual (Stream, bool) GetStream(string rootPath, string file)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="allowedPaths">The allowed paths.</param>
        /// <returns>IReadOnlyCollection&lt;IFileInfo&gt;.</returns>
        public virtual IReadOnlyCollection<IFileInfo> Read(string path, IEnumerable<string> allowedPaths = null)
        {
            var files = Directory.GetFiles(path, SearchPattern, SearchOption);
            if (files?.Length > 0)
            {
                var result = new List<IFileInfo>();
                foreach (var file in files)
                {
                    var relativePath = file.Replace(path, string.Empty).Trim(Path.DirectorySeparatorChar);
                    var info = DIResolver.Get<IFileInfo>();
                    var fileInfo = new System.IO.FileInfo(file);
                    info.IsReadOnly = fileInfo.IsReadOnly;
                    info.Size = fileInfo.Length;
                    var content = File.ReadAllText(file);
                    info.FileName = relativePath;
                    info.IsBinary = false;
                    info.Content = content.SplitOnNewLine(false);
                    info.ContentSHA = content.CalculateSHA();
                    result.Add(info);
                }
                return result;
            }
            return null;
        }

        #endregion Methods
    }
}
