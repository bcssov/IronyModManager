// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 05-29-2020
//
// Last Modified By : Mario
// Last Modified On : 06-01-2020
// ***********************************************************************
// <copyright file="IGameSettings.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Interface IGameSettings
    /// Implements the <see cref="IronyModManager.Models.Common.IModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.IModel" />
    public interface IGameSettings : IModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the executable location.
        /// </summary>
        /// <value>The executable location.</value>
        string ExecutableLocation { get; set; }

        /// <summary>
        /// Gets or sets the launch arguments.
        /// </summary>
        /// <value>The launch arguments.</value>
        string LaunchArguments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [refresh descriptors].
        /// </summary>
        /// <value><c>true</c> if [refresh descriptors]; otherwise, <c>false</c>.</value>
        bool RefreshDescriptors { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        string Type { get; set; }

        #endregion Properties
    }
}
