// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 07-11-2022
//
// Last Modified By : Mario
// Last Modified On : 12-04-2025
// ***********************************************************************
// <copyright file="SteamHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using IronyModManager.IO.Common.Platforms;
using IronyModManager.Shared;
using Steamworks;

namespace IronyModManager.IO.Platforms
{
    /// <summary>
    /// Class SteamHandler.
    /// Implements the <see cref="ISteam" />
    /// </summary>
    /// <seealso cref="ISteam" />
    /// <remarks>Initializes a new instance of the <see cref="SteamHandler" /> class.</remarks>
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class SteamHandler(ILogger logger) : ISteam
    {
        #region Fields

        /// <summary>
        /// The alternate delay
        /// </summary>
        private const int AlternateDelay = 3000;

        /// <summary>
        /// The alternate maximum attempts
        /// </summary>
        private const int AlternateMaxAttempts = 20;

        /// <summary>
        /// The delay
        /// </summary>
        private const int Delay = 250; // 4 per second

        /// <summary>
        /// The maximum attempts
        /// </summary>
        private const int MaxAttempts = 60 * 4;

        /// <summary>
        /// The steam application identifier file
        /// </summary>
        private const string SteamAppIdFile = "steam_appid.txt";

        /// <summary>
        /// The steam launch
        /// </summary>
        private const string SteamLaunch = SteamProcess + "://open/main";

        // 1 minute 3 seconds delay
        /// <summary>
        /// The steam process
        /// </summary>
        private const string SteamProcess = "steam";

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger = logger;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Initialize alternate as an asynchronous operation.
        /// </summary>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        public async Task<bool> InitAlternateAsync()
        {
            var processes = Process.GetProcesses();
            if (!processes.Any(p => p.ProcessName.Equals(SteamProcess, StringComparison.OrdinalIgnoreCase)))
            {
                var result = await OpenAsync(SteamLaunch);
                var attempts = 0;
                while (!processes.Any(p => p.ProcessName.Equals(SteamProcess, StringComparison.OrdinalIgnoreCase)))
                {
                    if (attempts > AlternateMaxAttempts)
                    {
                        break;
                    }

                    await Task.Delay(AlternateDelay);
                    processes = Process.GetProcesses();
                    attempts++;
                }

                return result;
            }

            return true;
        }

        /// <summary>
        /// Initialize as an asynchronous operation.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        public async Task<bool> InitAsync(long appId)
        {
            await SetAppId(appId);

            var canProceed = true;
            if (!SteamAPI.IsSteamRunning())
            {
                if (!await OpenAsync(SteamLaunch))
                {
                    return false;
                }

                var runCheckAttempts = 0;
                while (!SteamAPI.IsSteamRunning())
                {
                    if (runCheckAttempts > MaxAttempts)
                    {
                        canProceed = false;
                        break;
                    }

                    await Task.Delay(Delay);
                    runCheckAttempts++;
                }
            }

            if (!canProceed)
            {
                return false;
            }

            var result = await InitializeAndValidateAsync();
            var initCheckAttempts = 0;

            // Will keep trying until steam starts
            while (!result)
            {
                if (initCheckAttempts > MaxAttempts)
                {
                    break;
                }

                result = await InitializeAndValidateAsync();
                await Task.Delay(Delay);
                initCheckAttempts++;
            }

            return result;
        }

        /// <summary>
        /// Shutdowns the asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ShutdownAPIAsync()
        {
            SteamAPI.Shutdown();
            CleanupAppId();
            return Task.FromResult(true);
        }

        /// <summary>
        /// SetEnvironment variable native Linux call.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <returns>System.Int32.</returns>
        [DllImport("libc", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        private static extern int setenv(string name, string value, bool overwrite);

        /// <summary>
        /// Cleanups the application identifier.
        /// </summary>
        private void CleanupAppId()
        {
            try
            {
                if (File.Exists(SteamAppIdFile))
                {
                    // ReSharper disable once RedundantNameQualifier
                    var fileInfo = new System.IO.FileInfo(SteamAppIdFile) { Attributes = FileAttributes.Normal };
                    fileInfo.Delete();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Initialize and validate as an asynchronous operation.
        /// </summary>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        private async Task<bool> InitializeAndValidateAsync()
        {
            var result = SteamAPI.Init();

            if (!Packsize.Test() || !DllCheck.Test())
            {
                await ShutdownAPIAsync();
                return false;
            }

            return result;
        }

        /// <summary>
        /// Opens the asynchronous.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        private Task<bool> OpenAsync(string command)
        {
            try
            {
                ProcessRunner.LaunchExternalCommand(command);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Sets the application identifier.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        private async Task SetAppId(long appId)
        {
            var appIdValue = appId.ToString();
            SetEnvironmentVariable("SteamAppId", appIdValue);
            SetEnvironmentVariable("SteamOverlayGameId", appIdValue);
            SetEnvironmentVariable("SteamGameId", appIdValue);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                try
                {
                    await File.WriteAllTextAsync(SteamAppIdFile, appIdValue);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Sets the environment variable.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        private void SetEnvironmentVariable(string name, string value)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Way to go Microsoft
                _ = setenv(name, value, true);
            }
            else
            {
                Environment.SetEnvironmentVariable(name, value);
            }
        }

        #endregion Methods
    }
}
