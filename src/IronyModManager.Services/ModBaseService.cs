// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 04-07-2020
//
// Last Modified By : Mario
// Last Modified On : 04-07-2020
// ***********************************************************************
// <copyright file="ModBaseService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using AutoMapper;
using IronyModManager.Services.Common;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class ModBaseService.
    /// Implements the <see cref="IronyModManager.Services.BaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    public abstract class ModBaseService : BaseService
    {
        #region Fields

        /// <summary>
        /// The patch collection name
        /// </summary>
        protected const string PatchCollectionName = nameof(IronyModManager) + "_";

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModBaseService" /> class.
        /// </summary>
        /// <param name="gameService">The game service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ModBaseService(IGameService gameService, IStorageProvider storageProvider, IMapper mapper) : base(storageProvider, mapper)
        {
            GameService = gameService;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the game service.
        /// </summary>
        /// <value>The game service.</value>
        protected IGameService GameService { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Generates the name of the collection patch.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>System.String.</returns>
        protected virtual string GenerateCollectionPatchName(string collectionName)
        {
            var fileName = $"{PatchCollectionName}{collectionName}";
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, character) => current.Replace(character.ToString(), string.Empty));
        }

        #endregion Methods
    }
}
