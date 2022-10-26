// ***********************************************************************
// Assembly         : IronyModManager.GameLauncher
// Author           : Mario
// Created          : 10-26-2022
//
// Last Modified By : Mario
// Last Modified On : 10-26-2022
// ***********************************************************************
// <copyright file="CommandLineArgs.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;

namespace IronyModManager.GameLauncher
{
    /// <summary>
    /// Class CommandLineArgs.
    /// </summary>
    public class CommandLineArgs
    {
        #region Properties

        /// <summary>
        /// Gets or sets the steam application identifier.
        /// </summary>
        /// <value>The steam application identifier.</value>
        [Option('i', "id", Required = false, HelpText = "Steam app id")]
        public long? SteamAppId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use alternate launch method].
        /// </summary>
        /// <value><c>true</c> if [use alternate launch method]; otherwise, <c>false</c>.</value>
        [Option('a', "alternate", Required = false, HelpText = "Use alternate launch method")]
        public bool UseAlternateLaunchMethod { get; set; }

        #endregion Properties
    }
}
