// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 03-26-2021
//
// Last Modified By : Mario
// Last Modified On : 03-26-2021
// ***********************************************************************
// <copyright file="PromptNotificationsService.cs" company="Mario">
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
    /// Class PromptNotificationsService.
    /// Implements the <see cref="IronyModManager.Services.BaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.IPromptNotificationsService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IPromptNotificationsService" />
    public class PromptNotificationsService : BaseService, IPromptNotificationsService
    {
        #region Fields

        /// <summary>
        /// The preferences service
        /// </summary>
        private readonly IPreferencesService preferencesService;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PromptNotificationsService" /> class.
        /// </summary>
        /// <param name="preferencesService">The preferences service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public PromptNotificationsService(IPreferencesService preferencesService, IStorageProvider storageProvider, IMapper mapper) : base(storageProvider, mapper)
        {
            this.preferencesService = preferencesService;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IPromptNotifications.</returns>
        public IPromptNotifications Get()
        {
            var preferences = preferencesService.Get();
            return Mapper.Map<IPromptNotifications>(preferences);
        }

        /// <summary>
        /// Saves the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Save(IPromptNotifications model)
        {
            var preferences = preferencesService.Get();
            return preferencesService.Save(Mapper.Map(model, preferences));
        }

        #endregion Methods
    }
}
