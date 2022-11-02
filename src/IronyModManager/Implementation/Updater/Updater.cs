// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-16-2020
//
// Last Modified By : Mario
// Last Modified On : 11-02-2022
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
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.AppState;
using IronyModManager.Implementation.MessageBus;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using NetSparkleUpdater;
using Newtonsoft.Json;

namespace IronyModManager.Implementation.Updater
{
    /// <summary>
    /// Class Updater.
    /// Implements the <see cref="IronyModManager.Implementation.Updater.IUpdater" />
    /// </summary>
    /// <seealso cref="IronyModManager.Implementation.Updater.IUpdater" />
    public class Updater : IUpdater
    {
        #region Fields

        /// <summary>
        /// The error
        /// </summary>
        private readonly Subject<Exception> error;

        /// <summary>
        /// The is installer version
        /// </summary>
        private readonly bool isInstallerVersion;

        /// <summary>
        /// The progress
        /// </summary>
        private readonly Subject<int> progress;

        /// <summary>
        /// The shut down state
        /// </summary>
        private readonly IShutDownState shutDownState;

        /// <summary>
        /// The update check threshold
        /// </summary>
        private readonly TimeSpan updateCheckThreshold = TimeSpan.FromMinutes(30);

        /// <summary>
        /// The updater
        /// </summary>
        private readonly IronySparkleUpdater updater;

        /// <summary>
        /// The updater configuration
        /// </summary>
        private readonly UpdaterConfiguration updaterConfiguration;

        /// <summary>
        /// The updater service
        /// </summary>
        private readonly IUpdaterService updaterService;

        /// <summary>
        /// The allow prerelease
        /// </summary>
        private bool allowPrerelease;

        /// <summary>
        /// The busy
        /// </summary>
        private bool busy;

        /// <summary>
        /// The last exception
        /// </summary>
        private Exception lastException;

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
        /// <param name="updateProgressHandler">The update progress handler.</param>
        /// <param name="updaterService">The updater service.</param>
        /// <param name="appAction">The application action.</param>
        /// <param name="shutDownState">State of the shut down.</param>
        public Updater(UpdateUnpackProgressHandler updateProgressHandler, IUpdaterService updaterService, IAppAction appAction, IShutDownState shutDownState)
        {
            this.shutDownState = shutDownState;
            isInstallerVersion = IsInstallerVersion();
            this.updaterService = updaterService;
            progress = new Subject<int>();
            error = new Subject<Exception>();
            updaterConfiguration = new UpdaterConfiguration(new EntryAssemblyAccessor());
            updater = new IronySparkleUpdater(isInstallerVersion, updaterService, appAction)
            {
                SecurityProtocolType = System.Net.SecurityProtocolType.Tls12,
                AppCastHandler = new IronyAppCast(isInstallerVersion, updaterService),
                Configuration = updaterConfiguration,
                TmpDownloadFilePath = StaticResources.GetUpdaterPath().FirstOrDefault()
            };
            updater.DownloadStarted += (sender, path) =>
            {
                progress.OnNext(0);
            };
            updater.DownloadHadError += (sender, path, exception) =>
            {
                if (exception.Message != lastException?.Message)
                {
                    error.OnNext(exception);
                    lastException = exception;
                }
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
            updateProgressHandler.Subscribe(s =>
            {
                progress.OnNext(s.Progress);
            });
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
            var settings = updaterService.Get();
            var elapsedTime = DateTime.Now - updater.Configuration.LastCheckTime;
            if (elapsedTime >= updateCheckThreshold || updateInfo == null ||
                updateInfo.Status == NetSparkleUpdater.Enums.UpdateStatus.CouldNotDetermine || updateInfo.Status == NetSparkleUpdater.Enums.UpdateStatus.UserSkipped ||
                settings.CheckForPrerelease != allowPrerelease)
            {
                updateInfo = await updater.CheckForUpdatesQuietly(true);
                allowPrerelease = settings.CheckForPrerelease;
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
                lastException = null;
                updatePath = string.Empty;
                await updater.InitAndBeginDownload(updateInfo.Updates.FirstOrDefault());
                while (updater.UpdateDownloading)
                {
                    // Sigh
                    await Task.Delay(25);
                }
                // Slight safety delay
                await Task.Delay(250);
                busy = false;
                var updateSettings = new UpdateSettings()
                {
                    IsInstaller = isInstallerVersion,
                    Path = AppDomain.CurrentDomain.BaseDirectory
                };
                await File.WriteAllTextAsync(Path.Combine(StaticResources.GetUpdaterPath().FirstOrDefault(), Constants.UpdateSettings), JsonConvert.SerializeObject(updateSettings));
                return !string.IsNullOrWhiteSpace(updatePath) || lastException == null; // In case file is already downloaded
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
                return string.Join(Environment.NewLine, updateInfo.Updates.FirstOrDefault().Description.SplitOnNewLine(false).Select(p => p.Trim()));
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetVersion()
        {
            if (updateInfo != null && updateInfo.Updates.Any())
            {
                return updateInfo.Updates.FirstOrDefault().Title;
            }
            return string.Empty;
        }

        /// <summary>
        /// Installs the update asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> InstallUpdateAsync()
        {
            if (busy)
            {
                return false;
            }
            busy = true;
            await shutDownState.WaitUntilFreeAsync();
            updater.InstallUpdate(updateInfo.Updates.FirstOrDefault(), updatePath);
            while (updater.UpdateInstalling)
            {
                await Task.Delay(25);
            }
            busy = false;
            return true;
        }

        /// <summary>
        /// Sets the skipped version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool SetSkippedVersion(string version)
        {
            updaterConfiguration.SetVersionToSkip(version);
            return true;
        }

        /// <summary>
        /// Determines whether [is installer version].
        /// </summary>
        /// <returns><c>true</c> if [is installer version]; otherwise, <c>false</c>.</returns>
        private bool IsInstallerVersion()
        {
            var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.exe");
            return files.Any(p => Path.GetFileName(p).StartsWith("unins", StringComparison.OrdinalIgnoreCase));
        }

        #endregion Methods
    }
}
