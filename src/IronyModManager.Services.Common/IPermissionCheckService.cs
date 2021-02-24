// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 02-13-2021
//
// Last Modified By : Mario
// Last Modified On : 02-13-2021
// ***********************************************************************
// <copyright file="IPermissionCheckService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Linq;
using System;
using System.Collections.Generic;
using IronyModManager.Models.Common;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Interface IPermissionCheckService
    /// Implements the <see cref="IronyModManager.Services.Common.IBaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.IBaseService" />
    public interface IPermissionCheckService : IBaseService
    {
        #region Methods

        /// <summary>
        /// Verifies the permissions.
        /// </summary>
        /// <returns>IReadOnlyCollection&lt;IPermissionCheckResult&gt;.</returns>
        IReadOnlyCollection<IPermissionCheckResult> VerifyPermissions();

        #endregion Methods
    }
}
