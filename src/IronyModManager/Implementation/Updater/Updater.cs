// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-16-2020
//
// Last Modified By : Mario
// Last Modified On : 09-17-2020
// ***********************************************************************
// <copyright file="Updater.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using NetSparkleUpdater;
using NetSparkleUpdater.AssemblyAccessors;
using NetSparkleUpdater.SignatureVerifiers;

namespace IronyModManager.Implementation.Updater
{
    /// <summary>
    /// Class Updater.
    /// </summary>
    public class Updater
    {
        #region Fields

        /// <summary>
        /// The error
        /// </summary>
        private readonly Subject<Exception> error;

        /// <summary>
        /// The progress
        /// </summary>
        private readonly Subject<int> progress;

        /// <summary>
        /// The update check threshold
        /// </summary>
        private readonly TimeSpan updateCheckThreshold = TimeSpan.FromMinutes(30);

        /// <summary>
        /// The updater
        /// </summary>
        private readonly SparkleUpdater updater;

        /// <summary>
        /// The busy
        /// </summary>
        private bool busy;

        /// <summary>
        /// The update information
        /// </summary>
        private UpdateInfo updateInfo;

        /// <summary>
        /// The update path
        /// </summary>
        private string updatePath;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Updater" /> class.
        /// </summary>
        public Updater()
        {
            progress = new Subject<int>();
            error = new Subject<Exception>();
            updater = new SparkleUpdater(Constants.AppCastAddress, new Ed25519Checker(NetSparkleUpdater.Enums.SecurityMode.Strict, Constants.PublicUpdateKey))
            {
                SecurityProtocolType = System.Net.SecurityProtocolType.Tls12,
                AppCastHandler = new IronyAppCast(IsInstallerVersion()),
                Configuration = new UpdaterConfiguration(new AssemblyReflectionAccessor(string.Empty)),
                TmpDownloadFilePath = StaticResources.GetUpdaterPath()
            };
            updater.DownloadStarted += (sender, path) =>
            {
                progress.OnNext(0);
            };
            updater.DownloadHadError += (sender, path, exception) =>
            {
                error.OnNext(exception);
            };
            updater.DownloadMadeProgress += (sender, item, progress) =>
            {
                this.progress.OnNext(progress.ProgressPercentage);
            };
            updater.DownloadFinished += (sender, path) =>
            {
                updatePath = path;
                progress.OnNext(100);
            };
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>The error.</value>
        public IObservable<Exception> Error => error;

        /// <summary>
        /// Gets or sets the progress.
        /// </summary>
        /// <value>The progress.</value>
        public IObservable<int> Progress => progress;

        #endregion Properties

        #region Methods

        /// <summary>
        /// check for updates as an asynchronous operation.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public async Task<bool> CheckForUpdatesAsync()
        {
            var elapsedTime = DateTime.Now - updater.Configuration.LastCheckTime;
            if (elapsedTime >= updateCheckThreshold || updateInfo == null ||
                updateInfo.Status == NetSparkleUpdater.Enums.UpdateStatus.CouldNotDetermine || updateInfo.Status == NetSparkleUpdater.Enums.UpdateStatus.UserSkipped)
            {
                updateInfo = await updater.CheckForUpdatesQuietly();
            }
            if (updateInfo == null)
            {
                return false;
            }
            else
            {
                return updateInfo.Status == NetSparkleUpdater.Enums.UpdateStatus.UpdateAvailable;
            }
        }

        /// <summary>
        /// download update as an asynchronous operation.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public async Task<bool> DownloadUpdateAsync()
        {
            if (busy)
            {
                return false;
            }
            busy = true;
            if (updateInfo != null && updateInfo.Updates.Count > 0)
            {
                updatePath = string.Empty;
                await updater.InitAndBeginDownload(updateInfo.Updates.FirstOrDefault());
                return true;
            }
            busy = false;
            return false;
        }

        /// <summary>
        /// Gets the change log.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetChangeLog()
        {
            if (updateInfo != null && updateInfo.Updates.Count > 0)
            {
                return updateInfo.Updates.FirstOrDefault().Description;
            }
            return string.Empty;
        }

        /// <summary>
        /// Determines whether [is installer version].
        /// </summary>
        /// <returns><c>true</c> if [is installer version]; otherwise, <c>false</c>.</returns>
        private bool IsInstallerVersion()
        {
            var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.exe");
            return files.Any(p => p.StartsWith("unins", StringComparison.OrdinalIgnoreCase));
        }

        #endregion Methods
    }
}
