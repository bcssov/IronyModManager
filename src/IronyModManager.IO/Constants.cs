// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 02-23-2020
// ***********************************************************************
// <copyright file="Constants.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;

namespace IronyModManager.IO
{
    /// <summary>
    /// Class Constants.
    /// </summary>
    public static class Constants
    {
        #region Fields

        /// <summary>
        /// The zip extension
        /// </summary>
        public const string ZipExtension = ".zip";

        /// <summary>
        /// The mod directory
        /// </summary>
        public static readonly string ModDirectory = Path.DirectorySeparatorChar + "mod";

        #endregion Fields
    }
}
