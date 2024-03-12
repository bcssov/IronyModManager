// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 02-25-2024
//
// Last Modified By : Mario
// Last Modified On : 02-25-2024
// ***********************************************************************
// <copyright file="GameLanguage.cs" company="Mario">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Shared;

namespace IronyModManager.Models
{
    /// <summary>
    /// The game language.
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.BaseModel" />
    /// <seealso cref="IronyModManager.Models.Common.IGameLanguage" />
    public class GameLanguage : BaseModel, IGameLanguage
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value representing the display name.
        /// </summary>
        /// <value>The display name.</value>
        [DynamicLocalization(LocalizationResources.GameLanguages.Prefix, nameof(Type))]
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the is selected.
        /// </summary>
        /// <value><c>true</c> if is selected; otherwise, <c>false</c>.</value>
        public virtual bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets a value representing the type.
        /// </summary>
        /// <value>The type.</value>
        public virtual string Type { get; set; }

        #endregion Properties
    }
}
