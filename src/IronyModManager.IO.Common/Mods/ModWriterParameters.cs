// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 04-02-2020
//
// Last Modified By : Mario
// Last Modified On : 05-01-2020
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
        /// Gets or sets the enabled mods.
        /// </summary>
        /// <value>The enabled mods.</value>
        public IReadOnlyCollection<IMod> EnabledMods { get; set; }

        /// <summary>
        /// Gets or sets the mod.
        /// </summary>
        /// <value>The mod.</value>
        public IMod Mod { get; set; }

        /// <summary>
        /// Gets or sets the other mods.
        /// </summary>
        /// <value>The other mods.</value>
        public IReadOnlyCollection<IMod> OtherMods { get; set; }

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

        /// <summary>
        /// Gets or sets the top priority mods.
        /// </summary>
        /// <value>The top priority mods.</value>
        public IReadOnlyCollection<IMod> TopPriorityMods { get; set; }

        #endregion Properties
    }
}
