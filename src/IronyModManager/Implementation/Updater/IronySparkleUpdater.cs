// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-17-2020
//
// Last Modified By : Mario
// Last Modified On : 01-30-2021
// ***********************************************************************
// <copyright file="IronySparkleUpdater.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using IronyModManager.Implementation.Actions;
using IronyModManager.Services.Common;
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
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is installer version.
        /// </summary>
        /// <value><c>true</c> if this instance is installer version; otherwise, <c>false</c>.</value>
        public bool IsInstallerVersion { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [update installing].
        /// </summary>
        /// <value><c>true</c> if [update installing]; otherwise, <c>false</c>.</value>
        public bool UpdateInstalling { get; private set; }

        #endregion Properties

        #region Methods

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
                        if (files.Count() > 0)
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
                    EnsurePermissions(GetUpdaterExeFileNameParam(extractPath));
                    if (await appAction.RunAsync(GetUpdaterExeFileName(extractPath)))
                    {
                        await appAction.ExitAppAsync();
                    }
                }
            }
            UpdateInstalling = false;
        }

        /// <summary>
        /// Ensures the permissions.
        /// </summary>
        /// <param name="path">The path.</param>
        private void EnsurePermissions(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var sanitizedCmd = path.Replace("\"", "\\\"");
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"chmod +x {sanitizedCmd}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit();
            }
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

        #endregion Methods
    }
}
