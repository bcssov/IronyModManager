// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-01-2020
//
// Last Modified By : Mario
// Last Modified On : 05-30-2020
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
        /// <param name="text">The text.</param>
        /// <returns>Task.</returns>
        Task CopyAsync(string text);

        /// <summary>
        /// Exits the application asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task ExitAppAsync();

        /// <summary>
        /// Opens the asynchronous.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Task.</returns>
        Task OpenAsync(string command);

        #endregion Methods
    }
}
