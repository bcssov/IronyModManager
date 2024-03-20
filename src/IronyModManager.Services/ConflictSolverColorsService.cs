// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 03-18-2024
//
// Last Modified By : Mario
// Last Modified On : 03-18-2024
// ***********************************************************************
// <copyright file="ConflictSolverColorsService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// The conflict solver colors service.
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IConflictSolverColorsService" />
    public class ConflictSolverColorsService(IPreferencesService preferencesService, IStorageProvider storageProvider, IMapper mapper) : BaseService(storageProvider, mapper), IConflictSolverColorsService
    {
        #region Fields

        /// <summary>
        /// A private static readonly object named objLock.
        /// </summary>
        private static readonly object objLock = new();

        /// <summary>
        /// A private IPreferencesService named preferencesService.
        /// </summary>
        private readonly IPreferencesService preferencesService = preferencesService;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Get.
        /// </summary>
        /// <returns>An IConflictSolverColors.<see cref="T:IronyModManager.Models.Common.IConflictSolverColors" /></returns>
        public IConflictSolverColors Get()
        {
            return Mapper.Map<IConflictSolverColors>(preferencesService.Get());
        }

        /// <summary>
        /// Has any.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>A bool.</returns>
        public bool HasAny(IConflictSolverColors color)
        {
            return color != null && (!string.IsNullOrWhiteSpace(color.ConflictSolverDeletedLineColor) || !string.IsNullOrWhiteSpace(color.ConflictSolverImaginaryLineColor) ||
                                     !string.IsNullOrWhiteSpace(color.ConflictSolverInsertedLineColor) || !string.IsNullOrWhiteSpace(color.ConflictSolverModifiedLineColor));
        }

        /// <summary>
        /// Save.
        /// </summary>
        /// <param name="color">The color.</param>
        public void Save(IConflictSolverColors color)
        {
            lock (objLock)
            {
                if (color != null)
                {
                    var preferences = preferencesService.Get();
                    var mapped = Mapper.Map(color, preferences);
                    preferencesService.Save(mapped);
                }
            }
        }

        #endregion Methods
    }
}
