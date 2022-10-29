// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2022
// ***********************************************************************
// <copyright file="GameType.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Models.Common;
using IronyModManager.Storage.Common;

namespace IronyModManager.Storage
{
    /// <summary>
    /// Class GameType.
    /// Implements the <see cref="IronyModManager.Storage.Common.IGameType" />
    /// </summary>
    /// <seealso cref="IronyModManager.Storage.Common.IGameType" />
    public class GameType : IGameType
    {
        #region Properties

        /// <summary>
        /// Gets or sets the abrv.
        /// </summary>
        /// <value>The abrv.</value>
        public virtual string Abrv { get; set; }

        /// <summary>
        /// Gets or sets the advanced features.
        /// </summary>
        /// <value>The advanced features.</value>
        public virtual GameAdvancedFeatures AdvancedFeatures { get; set; }

        /// <summary>
        /// Gets or sets the base steam game directory.
        /// </summary>
        /// <value>The base steam game directory.</value>
        public virtual string BaseSteamGameDirectory { get; set; }

        /// <summary>
        /// Gets or sets the checksum folders.
        /// </summary>
        /// <value>The checksum folders.</value>
        public virtual IEnumerable<string> ChecksumFolders { get; set; }

        /// <summary>
        /// Gets or sets the DLC container.
        /// </summary>
        /// <value>The DLC container.</value>
        public virtual string DLCContainer { get; set; }

        /// <summary>
        /// Gets or sets the executable arguments.
        /// </summary>
        /// <value>The executable arguments.</value>
        public string ExecutableArgs { get; set; }

        /// <summary>
        /// Gets or sets the executable path.
        /// </summary>
        /// <value>The executable path.</value>
        public virtual string ExecutablePath { get; set; }

        /// <summary>
        /// Gets or sets the game folders.
        /// </summary>
        /// <value>The game folders.</value>
        public virtual IEnumerable<string> GameFolders { get; set; }

        /// <summary>
        /// Gets or sets the game index cache version.
        /// </summary>
        /// <value>The game index cache version.</value>
        public virtual int GameIndexCacheVersion { get; set; }

        /// <summary>
        /// Gets or sets the gog application identifier.
        /// </summary>
        /// <value>The gog application identifier.</value>
        public virtual int? GogAppId { get; set; }

        /// <summary>
        /// Gets or sets the name of the launcher settings file.
        /// </summary>
        /// <value>The name of the launcher settings file.</value>
        public virtual string LauncherSettingsFileName { get; set; }

        /// <summary>
        /// Gets or sets the launcher settings prefix.
        /// </summary>
        /// <value>The launcher settings prefix.</value>
        public virtual string LauncherSettingsPrefix { get; set; }

        /// <summary>
        /// Gets or sets the log location.
        /// </summary>
        /// <value>The log location.</value>
        public string LogLocation { get; set; }

        /// <summary>
        /// Gets or sets the type of the mod descriptor.
        /// </summary>
        /// <value>The type of the mod descriptor.</value>
        public virtual ModDescriptorType ModDescriptorType { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the paradox game identifier.
        /// </summary>
        /// <value>The paradox game identifier.</value>
        public virtual string ParadoxGameId { get; set; }

        /// <summary>
        /// Gets or sets the remote steam user directory.
        /// </summary>
        /// <value>The remote steam user directory.</value>
        public virtual IEnumerable<string> RemoteSteamUserDirectory { get; set; }

        /// <summary>
        /// Gets or sets the steam application identifier.
        /// </summary>
        /// <value>The steam application identifier.</value>
        public virtual int SteamAppId { get; set; }

        /// <summary>
        /// Gets or sets the supported merge types.
        /// </summary>
        /// <value>The supported merge types.</value>
        public virtual SupportedMergeTypes SupportedMergeTypes { get; set; }

        /// <summary>
        /// Gets or sets the user directory.
        /// </summary>
        /// <value>The user directory.</value>
        public virtual string UserDirectory { get; set; }

        /// <summary>
        /// Gets or sets the workshop directory.
        /// </summary>
        /// <value>The workshop directory.</value>
        public virtual IEnumerable<string> WorkshopDirectory { get; set; }

        #endregion Properties
    }
}
