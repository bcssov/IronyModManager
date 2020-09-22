// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-01-2020
//
// Last Modified By : Mario
// Last Modified On : 09-22-2020
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
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> CopyAsync(string text);

        /// <summary>
        /// Exits the application asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        Task ExitAppAsync();

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> GetAsync();

        /// <summary>
        /// Opens the asynchronous.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> OpenAsync(string command);

        /// <summary>
        /// Runs the asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> RunAsync(string path, string args = Shared.Constants.EmptyParam);

        #endregion Methods
    }
}
