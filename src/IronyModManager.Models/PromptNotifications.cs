// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 03-26-2021
//
// Last Modified By : Mario
// Last Modified On : 03-26-2021
// ***********************************************************************
// <copyright file="PromptNotifications.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Models.Common;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class PromptNotifications.
    /// Implements the <see cref="IronyModManager.Models.Common.BaseModel" />
    /// Implements the <see cref="IronyModManager.Models.Common.IPromptNotifications" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.BaseModel" />
    /// <seealso cref="IronyModManager.Models.Common.IPromptNotifications" />
    public class PromptNotifications : BaseModel, IPromptNotifications
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [conflict solver prompt shown].
        /// </summary>
        /// <value><c>true</c> if [conflict solver prompt shown]; otherwise, <c>false</c>.</value>
        public virtual bool ConflictSolverPromptShown { get; set; }

        #endregion Properties
    }
}
