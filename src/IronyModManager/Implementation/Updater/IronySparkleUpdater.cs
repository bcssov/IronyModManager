// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-17-2020
//
// Last Modified By : Mario
// Last Modified On : 11-14-2022
// ***********************************************************************
// <copyright file="IronySparkleUpdater.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using IronyModManager.Implementation.Actions;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using NetSparkleUpdater;
using NetSparkleUpdater.SignatureVerifiers;

namespace IronyModManager.Implementation.Updater
{
    /// <summary>
    /// Class IronySparkleUpdater.
    /// Implements the <see cref="NetSparkleUpdater.SparkleUpdater" />
    /// </summary>
    /// <seealso cref="NetSparkleUpdater.SparkleUpdater" />
    public class IronySparkleUpdater : SparkleUpdater
    {
        #region Fields

        /// <summary>
        /// The application action
        /// </summary>
        private readonly IAppAction appAction;

        /// <summary>
        /// The updater service
        /// </summary>
        private readonly IUpdaterService updaterService;

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IronySparkleUpdater" /> class.
        /// </summary>
        /// <param name="isInstallerVersion">if set to <c>true</c> [is installer version].</param>
        /// <param name="updaterService">The updater service.</param>
        /// <param name="appAction">The application action.</param>
        public IronySparkleUpdater(bool isInstallerVersion, IUpdaterService updaterService, IAppAction appAction) :
            base(Constants.AppCastAddress, new Ed25519Checker(NetSparkleUpdater.Enums.SecurityMode.OnlyVerifySoftwareDownloads, Constants.PublicUpdateKey))
        {
            CheckServerFileName = false; // Any more breaking changes?
            IsInstallerVersion = isInstallerVersion;
            this.updaterService = updaterService;
            this.appAction = appAction;
            DownloadFinished += OnDownloadFinished;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is installer version.
        /// </summary>
        /// <value><c>true</c> if this instance is installer version; otherwise, <c>false</c>.</value>
        public bool IsInstallerVersion { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [update downloading].
        /// </summary>
        /// <value><c>true</c> if [update downloading]; otherwise, <c>false</c>.</value>
        public bool UpdateDownloading { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [update installing].
        /// </summary>
        /// <value><c>true</c> if [update installing]; otherwise, <c>false</c>.</value>
        public bool UpdateInstalling { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initializes the and begin download.
        /// </summary>
        /// <param name="item">The item.</param>
        public new async Task InitAndBeginDownload(AppCastItem item)
        {
            UpdateDownloading = true;
            await base.InitAndBeginDownload(item);
        }

        /// <summary>
        /// Installs the update.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="installPath">The install path.</param>
        public new void InstallUpdate(AppCastItem item, string installPath = null)
        {
            // So what to say to this "async void"? I don't know where to even begin.
            UpdateInstalling = true;
            base.InstallUpdate(item, installPath);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                DownloadFinished -= OnDownloadFinished;
            }
            disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Runs the downloaded installer.
        /// </summary>
        /// <param name="downloadFilePath">The download file path.</param>
        protected override async Task RunDownloadedInstaller(string downloadFilePath)
        {
            var extractPath = await Task.Run(async () => await updaterService.UnpackUpdateAsync(downloadFilePath));
            if (Directory.Exists(extractPath))
            {
                if (IsInstallerVersion)
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        var files = Directory.GetFiles(extractPath, "*.exe", SearchOption.TopDirectoryOnly);
                        if (files.Length > 0)
                        {
                            if (await appAction.RunAsync(files.FirstOrDefault()))
                            {
                                await appAction.ExitAppAsync();
                            }
                        }
                    }
                }
                else
                {
                    ProcessRunner.EnsurePermissions(GetUpdaterExeFileNameParam(extractPath));
                    if (await appAction.RunAsync(GetUpdaterExeFileName(extractPath)))
                    {
                        await appAction.ExitAppAsync();
                    }
                }
            }
            UpdateInstalling = false;
        }

        /// <summary>
        /// Gets the name of the updater executable file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        private string GetUpdaterExeFileName(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Path.Combine(path, "IronyModManager.Updater.exe");
            }
            return Path.Combine(path, "IronyModManager.Updater");
        }

        /// <summary>
        /// Gets the updater executable file name parameter.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        private string GetUpdaterExeFileNameParam(string path)
        {
            return $"\"{GetUpdaterExeFileName(path)}\"";
        }

        /// <summary>
        /// Handles the <see cref="E:DownloadFinished" /> event.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="path">The path.</param>
        private void OnDownloadFinished(AppCastItem item, string path)
        {
            UpdateDownloading = false;
        }

        #endregion Methods
    }
}
