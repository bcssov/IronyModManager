// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 06-16-2020
// ***********************************************************************
// <copyright file="IReader.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;

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
        /// Gets the stream.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="file">The file.</param>
        /// <returns>Stream.</returns>
        Stream GetStream(string rootPath, string file);

        /// <summary>
        /// Reads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>IEnumerable&lt;IFileInfo&gt;.</returns>
        IEnumerable<IFileInfo> Read(string path);

        #endregion Methods
    }
}
