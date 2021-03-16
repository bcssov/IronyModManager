// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 03-16-2021
//
// Last Modified By : Mario
// Last Modified On : 03-16-2021
// ***********************************************************************
// <copyright file="INotificationPosition.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Localization;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Interface INotificationPosition
    /// Implements the <see cref="IronyModManager.Models.Common.IModel" />
    /// Implements the <see cref="IronyModManager.Localization.ILocalizableModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.ILocalizableModel" />
    /// <seealso cref="IronyModManager.Models.Common.IModel" />
    public interface INotificationPosition : IModel, ILocalizableModel
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
        NotificationPosition Type { get; set; }

        #endregion Properties
    }
}
