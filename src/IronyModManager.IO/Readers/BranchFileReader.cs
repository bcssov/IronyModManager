// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 12-03-2025
//
// Last Modified By : Mario
// Last Modified On : 12-03-2025
// ***********************************************************************
// <copyright file="BranchFileReader.cs" company="Mario">
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
using EncodingInfo = IronyModManager.Shared.EncodingInfo;

namespace IronyModManager.IO.Readers
{
    /// <summary>
    /// Class BranchFileReader.
    /// Implements the <see cref="IFileReader" />
    /// </summary>
    /// <seealso cref="IFileReader" />
    internal class BranchFileReader : IFileReader
    {
        #region Fields

        /// <summary>
        /// The allowed files
        /// </summary>
        private readonly string[] allowedFiles = ["caesar_branch.txt", "clausewitz_branch.txt"];

        #endregion Fields

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
            return allowedFiles.Any(p => path.EndsWith(p, StringComparison.OrdinalIgnoreCase));
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
        /// <exception cref="System.NotSupportedException"></exception>
        public IEnumerable<string> GetFiles(string path)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="file">The file.</param>
        /// <returns>System.ValueTuple&lt;Stream, System.Boolean, System.Nullable&lt;DateTime&gt;, EncodingInfo&gt;.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public (Stream, bool, DateTime?, EncodingInfo encoding) GetStream(string rootPath, string file)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the total size.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="extensions">The extensions.</param>
        /// <returns>System.Int64.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public long GetTotalSize(string path, string[] extensions = null)
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
                info.LastModified = fileInfo.LastWriteTime;
                info.Encoding = fileInfo.GetEncodingInfo();
                result.Add(info);
                return result;
            }

            return null;
        }

        #endregion Methods
    }
}
