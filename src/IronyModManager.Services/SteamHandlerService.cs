// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 07-11-2022
//
// Last Modified By : Mario
// Last Modified On : 07-11-2022
// ***********************************************************************
// <copyright file="SteamHandlerService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IronyModManager.IO.Common.Platforms;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class SteamHandlerService.
    /// Implements the <see cref="IronyModManager.Services.BaseService" />
    /// Implements the <see cref="ISteamHandlerService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    /// <seealso cref="ISteamHandlerService" />
    public class SteamHandlerService : BaseService, ISteamHandlerService
    {
        #region Fields

        /// <summary>
        /// The steam
        /// </summary>
        private readonly ISteam steam;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SteamHandlerService" /> class.
        /// </summary>
        /// <param name="steam">The steam.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public SteamHandlerService(ISteam steam, IStorageProvider storageProvider, IMapper mapper) : base(storageProvider, mapper)
        {
            this.steam = steam;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Launches the steam asynchronous.
        /// </summary>
        /// <param name="useLegacyMethod">if set to <c>true</c> [use legacy method].</param>
        /// <param name="game">The game.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<bool> LaunchSteamAsync(bool useLegacyMethod, IGame game)
        {
            if (game is null)
            {
                return false;
            }
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

        #endregion Methods
    }
}
