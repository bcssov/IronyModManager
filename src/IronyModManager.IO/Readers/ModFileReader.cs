// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 06-16-2020
// ***********************************************************************
// <copyright file="ModFileReader.cs" company="Mario">
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
    /// Class ModFileReader.
    /// Implements the <see cref="IronyModManager.IO.Common.Readers.IFileReader" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Readers.IFileReader" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class ModFileReader : IFileReader
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can read the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if this instance can read the specified path; otherwise, <c>false</c>.</returns>
        public virtual bool CanRead(string path)
        {
            return Directory.Exists(path) && path.EndsWith(Common.Constants.ModDirectory, StringComparison.OrdinalIgnoreCase);
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
        public virtual Stream GetStream(string rootPath, string file)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>IReadOnlyCollection&lt;IFileInfo&gt;.</returns>
        public virtual IReadOnlyCollection<IFileInfo> Read(string path)
        {
            var files = Directory.GetFiles(path, "*.mod", SearchOption.TopDirectoryOnly);
            if (files?.Count() > 0)
            {
                var result = new List<IFileInfo>();
                foreach (var file in files)
                {
                    var relativePath = file.Replace(path, string.Empty).Trim(Path.DirectorySeparatorChar);
                    var info = DIResolver.Get<IFileInfo>();
                    var content = File.ReadAllText(file);
                    info.FileName = relativePath;
                    info.IsBinary = false;
                    info.Content = content.SplitOnNewLine();
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
