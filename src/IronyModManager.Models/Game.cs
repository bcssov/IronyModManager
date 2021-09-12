// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 09-12-2021
// ***********************************************************************
// <copyright file="Game.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Shared;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class Game.
    /// Implements the <see cref="IronyModManager.Models.Common.BaseModel" />
    /// Implements the <see cref="IronyModManager.Models.Common.IGame" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.BaseModel" />
    /// <seealso cref="IronyModManager.Models.Common.IGame" />
    public class Game : BaseModel, IGame
    {
        #region Properties

        /// <summary>
        /// Gets or sets the abrv.
        /// </summary>
        /// <value>The abrv.</value>
        public virtual string Abrv { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [advanced features supported].
        /// </summary>
        /// <value><c>true</c> if [advanced features supported]; otherwise, <c>false</c>.</value>
        public virtual bool AdvancedFeaturesSupported { get; set; }

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
        /// Gets or sets a value indicating whether [close application after game launch].
        /// </summary>
        /// <value><c>true</c> if [close application after game launch]; otherwise, <c>false</c>.</value>
        public virtual bool CloseAppAfterGameLaunch { get; set; }

        /// <summary>
        /// Gets or sets the custom mod directory.
        /// </summary>
        /// <value>The custom mod directory.</value>
        public virtual string CustomModDirectory { get; set; }

        /// <summary>
        /// Gets or sets the DLC container.
        /// </summary>
        /// <value>The DLC container.</value>
        public virtual string DLCContainer { get; set; }

        /// <summary>
        /// Gets or sets the executable location.
        /// </summary>
        /// <value>The executable location.</value>
        public virtual string ExecutableLocation { get; set; }

        /// <summary>
        /// Gets or sets the game folders.
        /// </summary>
        /// <value>The game folders.</value>
        public virtual IEnumerable<string> GameFolders { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
        public virtual bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets the launch arguments.
        /// </summary>
        /// <value>The launch arguments.</value>
        public virtual string LaunchArguments { get; set; }

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
        public virtual string LogLocation { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DynamicLocalization(LocalizationResources.Games.Prefix, nameof(Type))]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the paradox game identifier.
        /// </summary>
        /// <value>The paradox game identifier.</value>
        public virtual string ParadoxGameId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [refresh descriptors].
        /// </summary>
        /// <value><c>true</c> if [refresh descriptors]; otherwise, <c>false</c>.</value>
        public virtual bool RefreshDescriptors { get; set; }

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
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public virtual string Type { get; set; }

        /// <summary>
        /// Gets or sets the mod directory.
        /// </summary>
        /// <value>The mod directory.</value>
        public virtual string UserDirectory { get; set; }

        /// <summary>
        /// Gets or sets the workshop directory.
        /// </summary>
        /// <value>The workshop directory.</value>
        public virtual IEnumerable<string> WorkshopDirectory { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether the specified term is match.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns><c>true</c> if the specified term is match; otherwise, <c>false</c>.</returns>
        public bool IsMatch(string term)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return false;
            }
            term ??= string.Empty;
            return Name.StartsWith(term, StringComparison.OrdinalIgnoreCase);
        }

        #endregion Methods
    }
}
