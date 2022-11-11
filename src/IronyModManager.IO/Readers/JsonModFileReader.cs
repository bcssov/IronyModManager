// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 10-29-2022
//
// Last Modified By : Mario
// Last Modified On : 10-29-2022
// ***********************************************************************
// <copyright file="JsonModFileReader.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.Shared;

namespace IronyModManager.IO.Readers
{
    /// <summary>
    /// Class JsonModFileReader.
    /// Implements the <see cref="IronyModManager.IO.Readers.BaseSpecializedDiskReader" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Readers.BaseSpecializedDiskReader" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class JsonModFileReader : BaseSpecializedDiskReader
    {
        #region Properties

        /// <summary>
        /// Gets the search extension.
        /// </summary>
        /// <value>The search extension.</value>
        public override string SearchExtension => Common.Constants.JsonModDirectoy;

        /// <summary>
        /// Gets the search option.
        /// </summary>
        /// <value>The search option.</value>
        public override SearchOption SearchOption => SearchOption.TopDirectoryOnly;

        /// <summary>
        /// Gets the search pattern.
        /// </summary>
        /// <value>The search pattern.</value>
        public override string SearchPattern => "*.json";

        #endregion Properties
    }
}
