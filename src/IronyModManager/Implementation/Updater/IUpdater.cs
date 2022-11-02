// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-16-2020
//
// Last Modified By : Mario
// Last Modified On : 11-02-2022
// ***********************************************************************
// <copyright file="IUpdater.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Threading.Tasks;

namespace IronyModManager.Implementation.Updater
{
    /// <summary>
    /// Interface IUpdater
    /// </summary>
    public interface IUpdater
    {
        #region Properties

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>The error.</value>
        IObservable<Exception> Error { get; }

        /// <summary>
        /// Gets the progress.
        /// </summary>
        /// <value>The progress.</value>
        IObservable<int> Progress { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Checks for updates asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> CheckForUpdatesAsync();

        /// <summary>
        /// Downloads the update asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> DownloadUpdateAsync();

        /// <summary>
        /// Gets the change log.
        /// </summary>
        /// <returns>System.String.</returns>
        string GetChangeLog();

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <returns>System.String.</returns>
        string GetVersion();

        /// <summary>
        /// Installs the update asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> InstallUpdateAsync();

        /// <summary>
        /// Sets the skipped version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool SetSkippedVersion(string version);

        #endregion Methods
    }
}
