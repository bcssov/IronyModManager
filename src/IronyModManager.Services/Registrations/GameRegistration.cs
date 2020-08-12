// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 08-12-2020
// ***********************************************************************
// <copyright file="GameRegistration.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;
using IronyModManager.DI;
using IronyModManager.Shared;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services.Registrations
{
    /// <summary>
    /// Class GameRegistration.
    /// Implements the <see cref="IronyModManager.Shared.PostStartup" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.PostStartup" />
    [ExcludeFromCoverage("Setup module.")]
    public class GameRegistration : PostStartup
    {
        #region Methods

        /// <summary>
        /// Called when [post startup].
        /// </summary>
        public override void OnPostStartup()
        {
            var storage = DIResolver.Get<IStorageProvider>();
            storage.RegisterGame(GetStellaris());
        }

        /// <summary>
        /// Gets the stellaris.
        /// </summary>
        /// <returns>IGameType.</returns>
        private IGameType GetStellaris()
        {
            var stellaris = DIResolver.Get<IGameType>();
            stellaris.ChecksumFolders = Shared.Constants.GamesTypes.Stellaris.ChecksumFolders;
            stellaris.GameFolders = Shared.Constants.GamesTypes.Stellaris.GameFolders;
            stellaris.LogLocation = Path.Combine(Path.Combine(UserDirectory.GetDirectory(), Shared.Constants.GamesTypes.Stellaris.Name), Shared.Constants.GamesTypes.Stellaris.LogLocation);
            stellaris.Name = Shared.Constants.GamesTypes.Stellaris.Name;
            stellaris.SteamAppId = Shared.Constants.GamesTypes.Stellaris.SteamAppId;
            stellaris.UserDirectory = Path.Combine(UserDirectory.GetDirectory(), Shared.Constants.GamesTypes.Stellaris.Name);
            stellaris.WorkshopDirectory = WorkshopDirectory.GetDirectory(Shared.Constants.GamesTypes.Stellaris.SteamAppId);
            return stellaris;
        }

        #endregion Methods
    }
}
