// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-13-2021
//
// Last Modified By : Mario
// Last Modified On : 10-29-2022
// ***********************************************************************
// <copyright file="DLCFileReader.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Shared;

namespace IronyModManager.IO.Readers
{
    /// <summary>
    /// Class DLCFileReader.
    /// Implements the <see cref="IronyModManager.IO.Readers.BaseSpecializedDiskReader" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Readers.BaseSpecializedDiskReader" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class DLCFileReader : BaseSpecializedDiskReader
    {
        #region Properties

        /// <summary>
        /// Gets the search extension.
        /// </summary>
        /// <value>The search extension.</value>
        public override string SearchExtension => Common.Constants.DLCDirectory;

        /// <summary>
        /// Gets the search option.
        /// </summary>
        /// <value>The search option.</value>
        public override SearchOption SearchOption => SearchOption.AllDirectories;

        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        /// <value>The search pattern.</value>
        public override string SearchPattern => "*.dlc";

        #endregion Properties

        #region Methods

        /// <summary>
        /// Reads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="allowedPaths">The allowed paths.</param>
        /// <param name="searchSubFolders">if set to <c>true</c> [search sub folders].</param>
        /// <returns>IReadOnlyCollection&lt;IFileInfo&gt;.</returns>
        public override IReadOnlyCollection<IFileInfo> Read(string path, IEnumerable<string> allowedPaths = null, bool searchSubFolders = true)
        {
            var result = ReadInternal(Directory.GetFiles(path, "*.json", SearchOption), path);
            if (result != null && result.Any())
            {
                return result;
            }
            else
            {
                return base.Read(path, allowedPaths, searchSubFolders);
            }
        }

        #endregion Methods
    }
}
