// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2022
// ***********************************************************************
// <copyright file="IGame.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Localization;
using IronyModManager.Shared.Models;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Interface IGame
    /// Implements the <see cref="IronyModManager.Models.Common.IModel" />
    /// Implements the <see cref="IronyModManager.Localization.ILocalizableModel" />
    /// Implements the <see cref="IronyModManager.Shared.Models.IQueryableModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.Models.IQueryableModel" />
    /// <seealso cref="IronyModManager.Models.Common.IModel" />
    /// <seealso cref="IronyModManager.Localization.ILocalizableModel" />
    public interface IGame : IModel, ILocalizableModel, IQueryableModel
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
        /// Gets or sets a value indicating whether [close application after game launch].
        /// </summary>
        /// <value><c>true</c> if [close application after game launch]; otherwise, <c>false</c>.</value>
        bool CloseAppAfterGameLaunch { get; set; }

        /// <summary>
        /// Gets or sets the custom mod directory.
        /// </summary>
        /// <value>The custom mod directory.</value>
        string CustomModDirectory { get; set; }

        /// <summary>
        /// Gets or sets the DLC container.
        /// </summary>
        /// <value>The DLC container.</value>
        string DLCContainer { get; set; }

        /// <summary>
        /// Gets or sets the executable location.
        /// </summary>
        /// <value>The executable location.</value>
        string ExecutableLocation { get; set; }

        /// <summary>
        /// Gets or sets the game folders.
        /// </summary>
        /// <value>The game folders.</value>
        IEnumerable<string> GameFolders { get; set; }

        /// <summary>
        /// Gets or sets the game index cache version.
        /// </summary>
        /// <value>The game index cache version.</value>
        int GameIndexCacheVersion { get; set; }

        /// <summary>
        /// Gets or sets the gog application identifier.
        /// </summary>
        /// <value>The gog application identifier.</value>
        int? GogAppId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
        bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets the launch arguments.
        /// </summary>
        /// <value>The launch arguments.</value>
        string LaunchArguments { get; set; }

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
        /// Gets or sets the type of the mod descriptor.
        /// </summary>
        /// <value>The type of the mod descriptor.</value>
        ModDescriptorType ModDescriptorType { get; set; }

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
        /// Gets or sets a value indicating whether [refresh descriptors].
        /// </summary>
        /// <value><c>true</c> if [refresh descriptors]; otherwise, <c>false</c>.</value>
        bool RefreshDescriptors { get; set; }

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
        /// Gets or sets the supported merge types.
        /// </summary>
        /// <value>The supported merge types.</value>
        SupportedMergeTypes SupportedMergeTypes { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        string Type { get; set; }

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
