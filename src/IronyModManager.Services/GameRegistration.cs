// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 02-23-2020
// ***********************************************************************
// <copyright file="GameRegistration.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.InteropServices;
using IronyModManager.DI;
using IronyModManager.Shared;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
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
            storage.RegisterGame(Shared.Constants.GamesTypes.Stellaris, Path.Combine(GetRootPath(), Shared.Constants.GamesTypes.Stellaris));
        }

        /// <summary>
        /// Gets the root path.
        /// </summary>
        /// <returns>System.String.</returns>
        private string GetRootPath()
        {
            var userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string rootUserDirectory;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                rootUserDirectory = Path.Combine(userDirectory, $"Documents{Path.DirectorySeparatorChar}Paradox Interactive");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                rootUserDirectory = Path.Combine(userDirectory, $".local{Path.DirectorySeparatorChar}share{Path.DirectorySeparatorChar}Paradox Interactive");
            }
            else
            {
                rootUserDirectory = Path.Combine(userDirectory, $"Documents{Path.DirectorySeparatorChar}Paradox Interactive");
            }
            return rootUserDirectory;
        }

        #endregion Methods
    }
}
