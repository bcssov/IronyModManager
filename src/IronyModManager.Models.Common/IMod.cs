// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 02-29-2020
// ***********************************************************************
// <copyright file="IMod.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Localization;
using IronyModManager.Parser.Common.Mod;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Interface IMod
    /// Implements the <see cref="IronyModManager.Parser.Common.Mod.IModObject" />
    /// Implements the <see cref="IronyModManager.Localization.ILocalizableModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.ILocalizableModel" />
    /// <seealso cref="IronyModManager.Parser.Common.Mod.IModObject" />
    public interface IMod : IModObject, ILocalizableModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
        bool IsSelected { get; set; }

        #endregion Properties
    }
}
