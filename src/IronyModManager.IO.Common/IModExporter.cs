// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 03-09-2020
//
// Last Modified By : Mario
// Last Modified On : 03-09-2020
// ***********************************************************************
// <copyright file="IModExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Models.Common;

namespace IronyModManager.IO.Common
{
    /// <summary>
    /// Interface IModExporter
    /// </summary>
    public interface IModExporter
    {
        #region Methods

        /// <summary>
        /// Exports the specified mod directory.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exportPath">The export path.</param>
        /// <param name="mod">The mod.</param>
        /// <param name="modDirectory">The mod directory.</param>
        void Export<T>(string exportPath, T mod, string modDirectory) where T : IModel;

        /// <summary>
        /// Imports the specified file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file">The file.</param>
        /// <param name="mod">The mod.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Import<T>(string file, T mod) where T : IModel;

        #endregion Methods
    }
}
