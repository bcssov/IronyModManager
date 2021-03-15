// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-21-2021
//
// Last Modified By : Mario
// Last Modified On : 03-11-2021
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

namespace IronyModManager
{
    /// <summary>
    /// Class CommandLineArgs.
    /// </summary>
    public class CommandLineArgs
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [enable resume game button].
        /// </summary>
        /// <value><c>true</c> if [enable resume game button]; otherwise, <c>false</c>.</value>
        [Option('r', "resume", Required = false, HelpText = "Force enables Irony to show resume game button")]
        public bool EnableResumeGameButton { get; set; }

        /// <summary>
        /// Gets or sets the game abrv.
        /// </summary>
        /// <value>The game abrv.</value>
        [Option('g', "game", Required = false, HelpText = "Game:CK3,EU4,HOI4,IR,Stellaris")]
        public string GameAbrv { get; set; }

        /// <summary>
        /// Gets or sets the show fatal error notification.
        /// </summary>
        /// <value>The show fatal error notification.</value>
        [Option("fatal-error", Required = false, HelpText = "Shows Irony fatal error notification.")]
        public bool ShowFatalErrorNotification { get; set; }

        #endregion Properties
    }
}
