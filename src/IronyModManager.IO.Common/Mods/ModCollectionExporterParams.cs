// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 04-02-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2022
// ***********************************************************************
// <copyright file="ModCollectionExporterParams.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Models.Common;
using IronyModManager.Shared;

namespace IronyModManager.IO.Common.Mods
{
    /// <summary>
    /// Class ModCollectionExporterParams.
    /// </summary>
    [ExcludeFromCoverage("Parameters.")]
    public class ModCollectionExporterParams
    {
        #region Properties

        /// <summary>
        /// Gets or sets the type of the descriptor.
        /// </summary>
        /// <value>The type of the descriptor.</value>
        public DescriptorType DescriptorType { get; set; }

        /// <summary>
        /// Gets or sets the export mod directory.
        /// </summary>
        /// <value>The export mod directory.</value>
        public string ExportModDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [export mod order only].
        /// </summary>
        /// <value><c>true</c> if [export mod order only]; otherwise, <c>false</c>.</value>
        public bool ExportModOrderOnly { get; set; }

        /// <summary>
        /// Gets or sets the export mods.
        /// </summary>
        /// <value>The export mods.</value>
        public IEnumerable<IMod> ExportMods { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>The file.</value>
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        /// <value>The game.</value>
        public IGame Game { get; set; }

        /// <summary>
        /// Gets or sets the mod.
        /// </summary>
        /// <value>The mod.</value>
        public IModCollection Mod { get; set; }

        /// <summary>
        /// Gets or sets the mod directory.
        /// </summary>
        /// <value>The mod directory.</value>
        public string ModDirectory { get; set; }

        /// <summary>
        /// Gets or sets the mod name override.
        /// </summary>
        /// <value>The mod name override.</value>
        public string ModNameOverride { get; set; }

        #endregion Properties
    }
}
