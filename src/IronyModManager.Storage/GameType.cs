// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 02-24-2020
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
