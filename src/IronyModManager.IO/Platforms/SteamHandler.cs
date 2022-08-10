// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 07-11-2022
//
// Last Modified By : Mario
// Last Modified On : 08-04-2022
// ***********************************************************************
// <copyright file="SteamHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class SteamHandler : ISteam
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
        private readonly ILogger logger;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SteamHandler" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public SteamHandler(ILogger logger)
        {
            this.logger = logger;
        }

        #endregion Constructors

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
            SetAppId(appId);

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
                        break;
                    }
                    await Task.Delay(Delay);
                    runCheckAttempts++;
                }
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
            return Task.FromResult(true);
        }

        /// <summary>
        /// Initialize and validate as an asynchronous operation.
        /// </summary>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        private async Task<bool> InitializeAndValidateAsync()
        {
            var result = SteamAPI.Init();

            if (!Packsize.Test())
            {
                await ShutdownAPIAsync();
                return false;
            }

            if (!DllCheck.Test())
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
        private void SetAppId(long appId)
        {
            var appIdValue = appId.ToString();
            Environment.SetEnvironmentVariable("SteamAppId", appIdValue);
            Environment.SetEnvironmentVariable("SteamOverlayGameId", appIdValue);
            Environment.SetEnvironmentVariable("SteamGameId", appIdValue);
        }

        #endregion Methods
    }
}
