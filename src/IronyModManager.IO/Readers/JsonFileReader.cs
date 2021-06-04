// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 09-20-2020
//
// Last Modified By : Mario
// Last Modified On : 05-28-2021
// ***********************************************************************
// <copyright file="JsonFileReader.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using IronyModManager.DI;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Shared;

namespace IronyModManager.IO.Readers
{
    /// <summary>
    /// Class JsonFileReader.
    /// Implements the <see cref="IronyModManager.IO.Common.Readers.IFileReader" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Readers.IFileReader" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class JsonFileReader : IFileReader
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance [can list files] the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if this instance [can list files] the specified path; otherwise, <c>false</c>.</returns>
        public bool CanListFiles(string path)
        {
            return false;
        }

        /// <summary>
        /// Determines whether this instance can read the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="searchSubFolders">if set to <c>true</c> [search sub folders].</param>
        /// <returns><c>true</c> if this instance can read the specified path; otherwise, <c>false</c>.</returns>
        public bool CanRead(string path, bool searchSubFolders = true)
        {
            return path.EndsWith(Constants.JsonExtension, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether this instance [can read stream] the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if this instance [can read stream] the specified path; otherwise, <c>false</c>.</returns>
        public bool CanReadStream(string path)
        {
            return false;
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>IList&lt;System.String&gt;.</returns>
        /// <exception cref="NotSupportedException"></exception>
        public IEnumerable<string> GetFiles(string path)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="file">The file.</param>
        /// <returns>Stream.</returns>
        /// <exception cref="NotSupportedException"></exception>
        public (Stream, bool) GetStream(string rootPath, string file)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the total size.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.Int64.</returns>
        /// <exception cref="NotSupportedException"></exception>
        public virtual long GetTotalSize(string path)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="allowedPaths">The allowed paths.</param>
        /// <param name="searchSubFolders">if set to <c>true</c> [search sub folders].</param>
        /// <returns>IReadOnlyCollection&lt;IFileInfo&gt;.</returns>
        public IReadOnlyCollection<IFileInfo> Read(string path, IEnumerable<string> allowedPaths = null, bool searchSubFolders = true)
        {
            if (File.Exists(path))
            {
                var result = new List<IFileInfo>();
                var content = File.ReadAllText(path);
                var info = DIResolver.Get<IFileInfo>();
                var fileInfo = new System.IO.FileInfo(path);
                info.IsReadOnly = fileInfo.IsReadOnly;
                info.Size = fileInfo.Length;
                info.FileName = path;
                info.IsBinary = false;
                info.Content = content.SplitOnNewLine(false);
                info.ContentSHA = content.CalculateSHA();
                result.Add(info);
                return result;
            }
            return null;
        }

        #endregion Methods
    }
}
