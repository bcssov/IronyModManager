// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 04-02-2020
//
// Last Modified By : Mario
// Last Modified On : 04-02-2020
// ***********************************************************************
// <copyright file="ModWriterParameters.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Models.Common;

namespace IronyModManager.IO.Common.Mods
{
    /// <summary>
    /// Class ModWriterParameters.
    /// </summary>
    public class ModWriterParameters
    {
        #region Properties

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
