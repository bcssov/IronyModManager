// ***********************************************************************
// Assembly         : IronyModManager.Storage.Common
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 01-27-2022
// ***********************************************************************
// <copyright file="IGameType.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Models.Common;

namespace IronyModManager.Storage.Common
{
    /// <summary>
    /// Class IGameType.
    /// </summary>
    public interface IGameType
    {
        #region Properties

        /// <summary>
        /// Gets or sets the abrv.
        /// </summary>
        /// <value>The abrv.</value>
        string Abrv { get; set; }

        /// <summary>
        /// Gets or sets the advanced features.
        /// </summary>
        /// <value>The advanced features.</value>
        GameAdvancedFeatures AdvancedFeatures { get; set; }

        /// <summary>
        /// Gets or sets the base steam game directory.
        /// </summary>
        /// <value>The base steam game directory.</value>
        string BaseSteamGameDirectory { get; set; }

        /// <summary>
        /// Gets or sets the checksum folders.
        /// </summary>
        /// <value>The checksum folders.</value>
        IEnumerable<string> ChecksumFolders { get; set; }

        /// <summary>
        /// Gets or sets the DLC container.
        /// </summary>
        /// <value>The DLC container.</value>
        string DLCContainer { get; set; }

        /// <summary>
        /// Gets or sets the executable arguments.
        /// </summary>
        /// <value>The executable arguments.</value>
        string ExecutableArgs { get; set; }

        /// <summary>
        /// Gets or sets the executable path.
        /// </summary>
        /// <value>The executable path.</value>
        string ExecutablePath { get; set; }

        /// <summary>
        /// Gets or sets the game folders.
        /// </summary>
        /// <value>The game folders.</value>
        IEnumerable<string> GameFolders { get; set; }

        /// <summary>
        /// Gets or sets the name of the launcher settings file.
        /// </summary>
        /// <value>The name of the launcher settings file.</value>
        string LauncherSettingsFileName { get; set; }

        /// <summary>
        /// Gets or sets the launcher settings prefix.
        /// </summary>
        /// <value>The launcher settings prefix.</value>
        string LauncherSettingsPrefix { get; set; }

        /// <summary>
        /// Gets or sets the log location.
        /// </summary>
        /// <value>The log location.</value>
        string LogLocation { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the paradox game identifier.
        /// </summary>
        /// <value>The paradox game identifier.</value>
        string ParadoxGameId { get; set; }

        /// <summary>
        /// Gets or sets the remote steam user directory.
        /// </summary>
        /// <value>The remote steam user directory.</value>
        IEnumerable<string> RemoteSteamUserDirectory { get; set; }

        /// <summary>
        /// Gets or sets the steam application identifier.
        /// </summary>
        /// <value>The steam application identifier.</value>
        int SteamAppId { get; set; }

        /// <summary>
        /// Gets or sets the user directory.
        /// </summary>
        /// <value>The user directory.</value>
        string UserDirectory { get; set; }

        /// <summary>
        /// Gets or sets the workshop directory.
        /// </summary>
        /// <value>The workshop directory.</value>
        IEnumerable<string> WorkshopDirectory { get; set; }

        #endregion Properties
    }
}
