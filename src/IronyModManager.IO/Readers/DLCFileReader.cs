// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-13-2021
//
// Last Modified By : Mario
// Last Modified On : 02-13-2021
// ***********************************************************************
// <copyright file="DLCFileReader.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
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
    }
}
