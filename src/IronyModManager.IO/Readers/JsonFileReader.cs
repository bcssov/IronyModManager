// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 09-20-2020
//
// Last Modified By : Mario
// Last Modified On : 09-20-2020
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
        /// Determines whether this instance can read the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if this instance can read the specified path; otherwise, <c>false</c>.</returns>
        public bool CanRead(string path)
        {
            return path.EndsWith(Constants.JsonExtension, StringComparison.OrdinalIgnoreCase);
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
        public Stream GetStream(string rootPath, string file)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="allowedPaths">The allowed paths.</param>
        /// <returns>IReadOnlyCollection&lt;IFileInfo&gt;.</returns>
        public IReadOnlyCollection<IFileInfo> Read(string path, IEnumerable<string> allowedPaths = null)
        {
            if (File.Exists(path))
            {
                var result = new List<IFileInfo>();
                var content = File.ReadAllText(path);
                var info = DIResolver.Get<IFileInfo>();
                info.FileName = path;
                info.IsBinary = false;
                info.Content = content.SplitOnNewLine();
                info.ContentSHA = content.CalculateSHA();
                result.Add(info);
                return result;
            }
            return null;
        }

        #endregion Methods
    }
}
