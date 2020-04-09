// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 04-02-2020
//
// Last Modified By : Mario
// Last Modified On : 04-09-2020
// ***********************************************************************
// <copyright file="ModWriterParameters.cs" company="Mario">
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
    /// Class ModWriterParameters.
    /// </summary>
    [ExcludeFromCoverage("Parameters.")]
    public class ModWriterParameters
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [append only].
        /// </summary>
        /// <value><c>true</c> if [append only]; otherwise, <c>false</c>.</value>
        public bool AppendOnly { get; set; }

        /// <summary>
        /// Gets or sets the hidden mods.
        /// </summary>
        /// <value>The hidden mods.</value>
        public IReadOnlyCollection<IMod> HiddenMods { get; set; }

        /// <summary>
        /// Gets or sets the mod.
        /// </summary>
        /// <value>The mod.</value>
        public IMod Mod { get; set; }

        /// <summary>
        /// Gets or sets the mods.
        /// </summary>
        /// <value>The mods.</value>
        public IReadOnlyCollection<IMod> Mods { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the root directory.
        /// </summary>
        /// <value>The root directory.</value>
        public string RootDirectory { get; set; }

        #endregion Properties
    }
}
