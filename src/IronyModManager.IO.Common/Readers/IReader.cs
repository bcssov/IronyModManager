// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 03-19-2021
// ***********************************************************************
// <copyright file="IReader.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace IronyModManager.IO.Common.Readers
{
    /// <summary>
    /// Interface IReader
    /// </summary>
    public interface IReader
    {
        #region Methods

        /// <summary>
        /// Gets the file information.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="file">The file.</param>
        /// <returns>IFileInfo.</returns>
        IFileInfo GetFileInfo(string rootPath, string file);

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        IEnumerable<string> GetFiles(string path);

        /// <summary>
        /// Gets the image stream asynchronous.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="file">The file.</param>
        /// <returns>Task&lt;MemoryStream&gt;.</returns>
        Task<MemoryStream> GetImageStreamAsync(string rootPath, string file);

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="file">The file.</param>
        /// <returns>Stream.</returns>
        Stream GetStream(string rootPath, string file);

        /// <summary>
        /// Gets the total size.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.Int64.</returns>
        long GetTotalSize(string path);

        /// <summary>
        /// Reads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="allowedPaths">The allowed paths.</param>
        /// <returns>IEnumerable&lt;IFileInfo&gt;.</returns>
        IEnumerable<IFileInfo> Read(string path, IEnumerable<string> allowedPaths = null);

        #endregion Methods
    }
}
