// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 09-17-2020
//
// Last Modified By : Mario
// Last Modified On : 09-17-2020
// ***********************************************************************
// <copyright file="UpdaterService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using AutoMapper;
using IronyModManager.IO.Common.Updater;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class UpdaterService.
    /// Implements the <see cref="IronyModManager.Services.BaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.IUpdaterService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IUpdaterService" />
    public class UpdaterService : BaseService, IUpdaterService
    {
        #region Fields

        /// <summary>
        /// The preferences service
        /// </summary>
        private readonly IPreferencesService preferencesService;

        /// <summary>
        /// The unpacker
        /// </summary>
        private readonly IUnpacker unpacker;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdaterService" /> class.
        /// </summary>
        /// <param name="unpacker">The unpacker.</param>
        /// <param name="preferencesService">The preferences service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public UpdaterService(IUnpacker unpacker, IPreferencesService preferencesService, IStorageProvider storageProvider, IMapper mapper) : base(storageProvider, mapper)
        {
            this.preferencesService = preferencesService;
            this.unpacker = unpacker;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IUpdateSettings.</returns>
        public virtual IUpdateSettings Get()
        {
            var preferences = preferencesService.Get();
            return Mapper.Map<IUpdateSettings>(preferences);
        }

        /// <summary>
        /// Saves the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool Save(IUpdateSettings settings)
        {
            var preferences = preferencesService.Get();
            return preferencesService.Save(Mapper.Map(settings, preferences));
        }

        /// <summary>
        /// Unpacks the update asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public virtual Task<string> UnpackUpdateAsync(string path)
        {
            return unpacker.UnpackUpdateAsync(path);
        }

        #endregion Methods
    }
}
