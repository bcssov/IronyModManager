// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 02-07-2020
//
// Last Modified By : Mario
// Last Modified On : 03-16-2021
// ***********************************************************************
// <copyright file="ThemeType.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Storage.Common;

namespace IronyModManager.Storage
{
    /// <summary>
    /// Class ThemeType.
    /// Implements the <see cref="IronyModManager.Storage.Common.IThemeType" />
    /// </summary>
    /// <seealso cref="IronyModManager.Storage.Common.IThemeType" />
    public class ThemeType : IThemeType
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is default.
        /// </summary>
        /// <value><c>true</c> if this instance is default; otherwise, <c>false</c>.</value>
        public virtual bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name { get; set; }

        #endregion Properties
    }
}
