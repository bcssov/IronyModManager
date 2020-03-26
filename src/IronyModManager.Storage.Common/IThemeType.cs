// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 02-07-2020
//
// Last Modified By : Mario
// Last Modified On : 03-25-2020
// ***********************************************************************
// <copyright file="IThemeType.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Storage.Common
{
    /// <summary>
    /// Interface IThemeType
    /// </summary>
    public interface IThemeType
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is default.
        /// </summary>
        /// <value><c>true</c> if this instance is default; otherwise, <c>false</c>.</value>
        bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the styles.
        /// </summary>
        /// <value>The styles.</value>
        IEnumerable<string> Styles { get; set; }

        #endregion Properties
    }
}
