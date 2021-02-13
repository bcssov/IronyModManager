// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-13-2021
//
// Last Modified By : Mario
// Last Modified On : 02-13-2021
// ***********************************************************************
// <copyright file="PermissionCheckService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using IronyModManager.DI;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class PermissionCheckService.
    /// Implements the <see cref="IronyModManager.Services.BaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.IPermissionCheckService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IPermissionCheckService" />
    public class PermissionCheckService : BaseService, IPermissionCheckService
    {
        #region Fields

        /// <summary>
        /// The days check valid
        /// </summary>
        private const int DaysCheckValid = 3;

        /// <summary>
        /// The application state service
        /// </summary>
        private readonly IAppStateService appStateService;

        /// <summary>
        /// The game service
        /// </summary>
        private readonly IGameService gameService;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionCheckService" /> class.
        /// </summary>
        /// <param name="gameService">The game service.</param>
        /// <param name="appStateService">The application state service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public PermissionCheckService(IGameService gameService, IAppStateService appStateService, IStorageProvider storageProvider, IMapper mapper) : base(storageProvider, mapper)
        {
            this.appStateService = appStateService;
            this.gameService = gameService;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Verifies the permission.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool VerifyPermission()
        {
            var game = gameService.GetSelected();
            if (game == null || !Directory.Exists(game.UserDirectory))
            {
                game = gameService.Get().FirstOrDefault(p => Directory.Exists(p.UserDirectory));
            }
            if (game == null)
            {
                return true;
            }
            var result = true;
            var state = appStateService.Get();
            if (!state.LastWritableCheck.HasValue || state.LastWritableCheck.GetValueOrDefault().AddDays(DaysCheckValid) < DateTime.Now)
            {
                var tempFile = DIResolver.Get<ITempFile>();
                tempFile.TempDirectory = game.UserDirectory;
                try
                {
                    tempFile.Create(nameof(IronyModManager) + ".txt");
                    tempFile.Text = nameof(IronyModManager);
                    tempFile.Text = nameof(IronyModManager) + " Edited";
                    tempFile.Delete();
                }
                catch
                {
                    result = false;
                }
                finally
                {
                    try
                    {
                        tempFile.Dispose();
                    }
                    catch
                    {
                    }
                }
            }
            if (result)
            {
                state.LastWritableCheck = DateTime.Now;
                appStateService.Save(state);
            }
            return result;
        }

        #endregion Methods
    }
}
