// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 04-04-2020
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
