// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 04-19-2020
//
// Last Modified By : Mario
// Last Modified On : 04-19-2020
// ***********************************************************************
// <copyright file="IBaseService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Interface IBaseService
    /// </summary>
    public interface IBaseService
    {
        #region Events

        /// <summary>
        /// Occurs when [shutdown state].
        /// </summary>
        event ShutdownStateDelegate ShutdownState;

        #endregion Events
    }
}
