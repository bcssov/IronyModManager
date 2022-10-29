// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2022
// ***********************************************************************
// <copyright file="Constants.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using IronyModManager.Shared;

namespace IronyModManager.IO.Common
{
    /// <summary>
    /// Class Constants.
    /// </summary>
    [ExcludeFromCoverage("Parameters.")]
    public static class Constants
    {
        #region Fields

        /// <summary>
        /// The exported mod content identifier
        /// </summary>
        public const string ExportedModContentId = "exported.json";

        /// <summary>
        /// The mod export path
        /// </summary>
        public const string ModExportPath = "irony_exported_mods";

        /// <summary>
        /// The built in DLC directory
        /// </summary>
        public static readonly string BuiltInDLCDirectory = Path.DirectorySeparatorChar + "builtin_dlc";

        /// <summary>
        /// The DLC directory
        /// </summary>
        public static readonly string DLCDirectory = Path.DirectorySeparatorChar + "dlc";

        /// <summary>
        /// The json mod directoy
        /// </summary>
        public static readonly string JsonModDirectoy = Path.DirectorySeparatorChar + "irony-mod";

        /// <summary>
        /// The mod directory
        /// </summary>
        public static readonly string ModDirectory = Path.DirectorySeparatorChar + "mod";

        #endregion Fields
    }
}
