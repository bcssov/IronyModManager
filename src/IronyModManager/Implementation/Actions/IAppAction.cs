// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-01-2020
//
// Last Modified By : Mario
// Last Modified On : 12-04-2025
// ***********************************************************************
// <copyright file="IAppAction.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Runs the game asynchronous.
        /// </summary>
        /// <param name="createSteamFile">if set to <c>true</c> [create steam file].</param>
        /// <param name="path">The path.</param>
        /// <param name="steamRoot">The steam root.</param>
        /// <param name="steamProtonVersion">The steam proton version.</param>
        /// <param name="appId">The application identifier.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> RunGameAsync(bool createSteamFile, string path, string steamRoot, string steamProtonVersion, int appId, string args = Shared.Constants.EmptyParam);

        #endregion Methods
    }
}
