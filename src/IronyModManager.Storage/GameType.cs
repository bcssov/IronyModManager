// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 08-12-2020
// ***********************************************************************
// <copyright file="GameType.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
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
        /// Gets or sets the base game directory.
        /// </summary>
        /// <value>The base game directory.</value>
        public virtual string BaseGameDirectory { get; set; }

        /// <summary>
        /// Gets or sets the checksum folders.
        /// </summary>
        /// <value>The checksum folders.</value>
        public virtual IEnumerable<string> ChecksumFolders { get; set; }

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
        /// Gets or sets the log location.
        /// </summary>
        /// <value>The log location.</value>
        public string LogLocation { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the steam application identifier.
        /// </summary>
        /// <value>The steam application identifier.</value>
        public virtual int SteamAppId { get; set; }

        /// <summary>
        /// Gets or sets the user directory.
        /// </summary>
        /// <value>The user directory.</value>
        public virtual string UserDirectory { get; set; }

        /// <summary>
        /// Gets or sets the workshop directory.
        /// </summary>
        /// <value>The workshop directory.</value>
        public virtual string WorkshopDirectory { get; set; }

        #endregion Properties
    }
}
