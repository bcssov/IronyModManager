// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 05-29-2020
//
// Last Modified By : Mario
// Last Modified On : 03-17-2021
// ***********************************************************************
// <copyright file="GameSettings.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Models.Common;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class GameSettings.
    /// Implements the <see cref="IronyModManager.Models.Common.BaseModel" />
    /// Implements the <see cref="IronyModManager.Models.Common.IGameSettings" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.BaseModel" />
    /// <seealso cref="IronyModManager.Models.Common.IGameSettings" />
    public class GameSettings : BaseModel, IGameSettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [close application after game launch].
        /// </summary>
        /// <value><c>null</c> if [close application after game launch] contains no value, <c>true</c> if [close application after game launch]; otherwise, <c>false</c>.</value>
        public virtual bool? CloseAppAfterGameLaunch { get; set; }

        /// <summary>
        /// Gets or sets the custom mod directory.
        /// </summary>
        /// <value>The custom mod directory.</value>
        public virtual string CustomModDirectory { get; set; }

        /// <summary>
        /// Gets or sets the executable location.
        /// </summary>
        /// <value>The executable location.</value>
        public virtual string ExecutableLocation { get; set; }

        /// <summary>
        /// Gets or sets the launch arguments.
        /// </summary>
        /// <value>The launch arguments.</value>
        public virtual string LaunchArguments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [refresh descriptors].
        /// </summary>
        /// <value><c>true</c> if [refresh descriptors]; otherwise, <c>false</c>.</value>
        public virtual bool RefreshDescriptors { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public virtual string Type { get; set; }

        /// <summary>
        /// Gets or sets the data directory.
        /// </summary>
        /// <value>The data directory.</value>
        public virtual string UserDirectory { get; set; }

        #endregion Properties
    }
}
