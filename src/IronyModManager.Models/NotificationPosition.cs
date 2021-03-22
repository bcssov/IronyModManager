// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 03-16-2021
//
// Last Modified By : Mario
// Last Modified On : 03-16-2021
// ***********************************************************************
// <copyright file="NotificationPosition.cs" company="Mario">
//     Mario
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
    /// Class NotificationPosition.
    /// Implements the <see cref="IronyModManager.Models.Common.BaseModel" />
    /// Implements the <see cref="IronyModManager.Models.Common.INotificationPosition" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.BaseModel" />
    /// <seealso cref="IronyModManager.Models.Common.INotificationPosition" />
    public class NotificationPosition : BaseModel, INotificationPosition
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
        public virtual bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DynamicLocalization(LocalizationResources.NotificationPosition.Prefix, nameof(Type))]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public virtual Common.NotificationPosition Type { get; set; }

        #endregion Properties
    }
}
