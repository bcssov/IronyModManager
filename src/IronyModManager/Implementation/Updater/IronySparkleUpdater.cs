// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-17-2020
//
// Last Modified By : Mario
// Last Modified On : 09-20-2020
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
using Newtonsoft.Json;

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

        #endregion Properties

        #region Methods

        /// <summary>
        /// Runs the downloaded installer.
        /// </summary>
        /// <param name="downloadFilePath">The download file path.</param>
        protected override async Task RunDownloadedInstaller(string downloadFilePath)
        {
            var extractPath = await updaterService.UnpackUpdateAsync(downloadFilePath);
            if (Directory.Exists(extractPath))
            {
                var updateSettings = new UpdateSettings()
                {
                    IsInstaller = IsInstallerVersion,
                    Path = AppDomain.CurrentDomain.BaseDirectory
                };
                await File.WriteAllTextAsync(Path.Combine(StaticResources.GetUpdaterPath(), Constants.UpdateSettings), JsonConvert.SerializeObject(updateSettings));
                if (IsInstallerVersion)
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        var files = Directory.GetFiles(extractPath, "*.exe");
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
