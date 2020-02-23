// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 02-23-2020
// ***********************************************************************
// <copyright file="IFileReader.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.IO
{
    /// <summary>
    /// Interface IFileReader
    /// </summary>
    public interface IFileReader
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can read the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if this instance can read the specified path; otherwise, <c>false</c>.</returns>
        bool CanRead(string path);

        /// <summary>
        /// Reads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>IReadOnlyCollection&lt;IFileInfo&gt;.</returns>
        IReadOnlyCollection<IFileInfo> Read(string path);

        #endregion Methods
    }
}
