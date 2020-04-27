// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-01-2020
//
// Last Modified By : Mario
// Last Modified On : 04-27-2020
// ***********************************************************************
// <copyright file="IAppAction.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace IronyModManager.Implementation.Actions
{
    /// <summary>
    /// Interface IAppAction
    /// </summary>
    public interface IAppAction
    {
        #region Methods

        /// <summary>
        /// Copies the asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task.</returns>
        Task CopyAsync(string url);

        /// <summary>
        /// Opens the asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Task.</returns>
        Task OpenAsync(string url);

        #endregion Methods
    }
}
