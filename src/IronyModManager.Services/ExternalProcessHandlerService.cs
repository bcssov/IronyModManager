// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 07-11-2022
//
// Last Modified By : Mario
// Last Modified On : 12-06-2025
// ***********************************************************************
// <copyright file="ExternalProcessHandlerService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IronyModManager.DI;
using IronyModManager.IO.Common.Platforms;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.Configuration;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class ExternalProcessHandlerService.
    /// Implements the <see cref="IronyModManager.Services.BaseService" />
    /// Implements the <see cref="IExternalProcessHandlerService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    /// <seealso cref="IExternalProcessHandlerService" />
    public class ExternalProcessHandlerService : BaseService, IExternalProcessHandlerService
    {
        #region Fields

        /// <summary>
        /// The paradox launcher
        /// </summary>
        private readonly IParadoxLauncher paradoxLauncher;

        /// <summary>
        /// The steam
        /// </summary>
        private readonly ISteam steam;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SteamHandlerService" /> class.
        /// </summary>
        /// <param name="paradoxLauncher">The paradox launcher.</param>
        /// <param name="steam">The steam.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ExternalProcessHandlerService(IParadoxLauncher paradoxLauncher, ISteam steam, IStorageProvider storageProvider, IMapper mapper) : base(storageProvider, mapper)
        {
            this.steam = steam;
            this.paradoxLauncher = paradoxLauncher;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Determines whether Paradox Launcher is running.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> IsParadoxLauncherRunningAsync()
        {
            return paradoxLauncher.IsRunningAsync();
        }

        /// <summary>
        /// Launches the steam asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="forceLegacyMode">if set to <c>true</c> forces legacy mode typically for flatpak binaries.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<bool> LaunchSteamAsync(IGame game, bool forceLegacyMode = false)
        {
            if (game is null)
            {
                return false;
            }

            var options = DIResolver.Get<IDomainConfiguration>().GetOptions();
            if (!options.Steam.UseGameHandler)
            {
                var useLegacyMethod = options.Steam.UseLegacyLaunchMethod || forceLegacyMode;
                if (useLegacyMethod)
                {
                    return await steam.InitAlternateAsync();
                }
                else
                {
                    var result = await steam.InitAsync(game.SteamAppId);
                    await steam.ShutdownAPIAsync();
                    return result;
                }
            }
            else
            {
                ProcessRunner.EnsurePermissions(options.Steam.GameHandlerPath);
                ProcessRunner.RunExternalProcess(options.Steam.GameHandlerPath, options.Steam.UseLegacyLaunchMethod || forceLegacyMode ? $"-a -i {game.SteamAppId}" : $"-i {game.SteamAppId}", true);
                return true;
            }
        }

        #endregion Methods
    }
}
