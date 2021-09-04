// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 08-23-2021
// ***********************************************************************
// <copyright file="ITheme.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using IronyModManager.Localization;
using IronyModManager.Shared.Models;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Interface ITheme
    /// Implements the <see cref="IronyModManager.Models.Common.IModel" />
    /// Implements the <see cref="IronyModManager.Localization.ILocalizableModel" />
    /// Implements the <see cref="IronyModManager.Shared.Models.IQueryableModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.Models.IQueryableModel" />
    /// <seealso cref="IronyModManager.Localization.ILocalizableModel" />
    /// <seealso cref="IronyModManager.Models.Common.IModel" />
    public interface ITheme : IModel, ILocalizableModel, IQueryableModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
        bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        string Type { get; set; }

        #endregion Properties
    }
}
