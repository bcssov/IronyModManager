// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 08-14-2020
//
// Last Modified By : Mario
// Last Modified On : 08-14-2020
// ***********************************************************************
// <copyright file="ModMergeFileExporterParameters.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.IO.Common.Mods
{
    /// <summary>
    /// Class ModMergeFileExporterParameters.
    /// </summary>
    public class ModMergeFileExporterParameters
    {
        #region Properties

        /// <summary>
        /// Gets or sets the export file.
        /// </summary>
        /// <value>The export file.</value>
        public string ExportFile { get; set; }

        /// <summary>
        /// Gets or sets the export path.
        /// </summary>
        /// <value>The export path.</value>
        public string ExportPath { get; set; }

        /// <summary>
        /// Gets or sets the root mod path.
        /// </summary>
        /// <value>The root mod path.</value>
        public string RootModPath { get; set; }

        #endregion Properties
    }
}
