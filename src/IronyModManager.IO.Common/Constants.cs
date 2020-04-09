// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 04-05-2020
// ***********************************************************************
// <copyright file="Constants.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
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
        /// The mod directory
        /// </summary>
        public static readonly string ModDirectory = Path.DirectorySeparatorChar + "mod";

        #endregion Fields
    }
}
