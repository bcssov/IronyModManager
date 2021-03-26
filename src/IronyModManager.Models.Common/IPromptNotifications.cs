// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 03-26-2021
//
// Last Modified By : Mario
// Last Modified On : 03-26-2021
// ***********************************************************************
// <copyright file="IPromptNotifications.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Interface IPromptNotifications
    /// Implements the <see cref="IronyModManager.Models.Common.IModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.IModel" />
    public interface IPromptNotifications : IModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [conflict solver prompt shown].
        /// </summary>
        /// <value><c>true</c> if [conflict solver prompt shown]; otherwise, <c>false</c>.</value>
        bool ConflictSolverPromptShown { get; set; }

        #endregion Properties
    }
}
