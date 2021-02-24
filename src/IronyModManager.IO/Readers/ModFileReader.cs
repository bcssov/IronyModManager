// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 02-13-2021
// ***********************************************************************
// <copyright file="ModFileReader.cs" company="Mario">
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
    /// Class ModFileReader.
    /// Implements the <see cref="IronyModManager.IO.Common.Readers.IFileReader" />
    /// Implements the <see cref="IronyModManager.IO.Readers.BaseSpecializedDiskReader" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Readers.BaseSpecializedDiskReader" />
    /// <seealso cref="IronyModManager.IO.Common.Readers.IFileReader" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class ModFileReader : BaseSpecializedDiskReader
    {
        #region Properties

        /// <summary>
        /// Gets the search extension.
        /// </summary>
        /// <value>The search extension.</value>
        public override string SearchExtension => Common.Constants.ModDirectory;

        /// <summary>
        /// Gets the search option.
        /// </summary>
        /// <value>The search option.</value>
        public override SearchOption SearchOption => SearchOption.TopDirectoryOnly;

        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        /// <value>The search pattern.</value>
        public override string SearchPattern => "*.mod";

        #endregion Properties
    }
}
