// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 07-24-2022
//
// Last Modified By : Mario
// Last Modified On : 12-04-2025
// ***********************************************************************
// <copyright file="DomainConfigurationOptions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Shared.Configuration
{
    /// <summary>
    /// Class ConflictSolver.
    /// </summary>
    public class ConflictSolver
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [compress indexed definitions].
        /// </summary>
        /// <value><c>true</c> if [compress indexed definitions]; otherwise, <c>false</c>.</value>
        public bool CompressIndexedDefinitions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use disk search].
        /// </summary>
        /// <value><c>true</c> if [use disk search]; otherwise, <c>false</c>.</value>
        public bool UseDiskSearch { get; set; }

        /// <summary>
        /// Gets or sets the use sub menus.
        /// </summary>
        /// <value>The use sub menus.</value>
        public bool UseHybridMemory { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class DomainConfigurationOptions.
    /// </summary>
    public class DomainConfigurationOptions
    {
        #region Properties

        /// <summary>
        /// Gets the conflict solver.
        /// </summary>
        /// <value>The conflict solver.</value>
        public ConflictSolver ConflictSolver { get; } = new();

        /// <summary>
        /// Gets the formatting.
        /// </summary>
        /// <value>The formatting.</value>
        public Formatting Formatting { get; } = new();

        /// <summary>
        /// Gets the osx options.
        /// </summary>
        /// <value>The osx options.</value>
        public OSXOptions OSXOptions { get; } = new();

        /// <summary>
        /// Gets the steam.
        /// </summary>
        /// <value>The steam.</value>
        public Steam Steam { get; } = new();

        #endregion Properties
    }

    /// <summary>
    /// Class Formatting.
    /// </summary>
    public class Formatting
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [use system culture].
        /// </summary>
        /// <value><c>true</c> if [use system culture]; otherwise, <c>false</c>.</value>
        public bool UseSystemCulture { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class OSXOptions.
    /// </summary>
    public class OSXOptions
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [use file streams].
        /// </summary>
        /// <value><c>true</c> if [use file streams]; otherwise, <c>false</c>.</value>
        public bool UseFileStreams { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class Steam.
    /// </summary>
    public class Steam
    {
        #region Properties

        /// <summary>
        /// Gets or sets the game handler path.
        /// </summary>
        /// <value>The game handler path.</value>
        public string GameHandlerPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [generate steam application identifier file].
        /// </summary>
        /// <value><c>true</c> if [generate steam application identifier file]; otherwise, <c>false</c>.</value>
        public bool GenerateSteamAppIdFile { get; set; }

        /// <summary>
        /// Gets or sets the installation location override.
        /// </summary>
        /// <value>The installation location override.</value>
        public string InstallLocationOverride { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use game handler].
        /// </summary>
        /// <value><c>true</c> if [use game handler]; otherwise, <c>false</c>.</value>
        public bool UseGameHandler { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use legacy launch method].
        /// </summary>
        /// <value><c>true</c> if [use legacy launch method]; otherwise, <c>false</c>.</value>
        public bool UseLegacyLaunchMethod { get; set; }

        #endregion Properties
    }
}
