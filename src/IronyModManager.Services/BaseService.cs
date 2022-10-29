// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2022
// ***********************************************************************
// <copyright file="BaseService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using AutoMapper;
using IronyModManager.DI;
using IronyModManager.IO.Common;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class BaseService.
    /// Implements the <see cref="IronyModManager.Services.Common.IBaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.IBaseService" />
    public abstract class BaseService : IBaseService
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService" /> class.
        /// </summary>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public BaseService(IStorageProvider storageProvider, IMapper mapper)
        {
            StorageProvider = storageProvider;
            Mapper = mapper;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the mapper.
        /// </summary>
        /// <value>The mapper.</value>
        protected IMapper Mapper { get; private set; }

        /// <summary>
        /// Gets the storage provider.
        /// </summary>
        /// <value>The storage provider.</value>
        protected IStorageProvider StorageProvider { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the model instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        protected virtual T GetModelInstance<T>() where T : class, IModel
        {
            return DIResolver.Get<T>();
        }

        /// <summary>
        /// Maps the type of the descriptor mod.
        /// </summary>
        /// <param name="modDescriptorType">Type of the mod descriptor.</param>
        /// <returns>Parser.Common.DescriptorModType.</returns>
        protected virtual Parser.Common.DescriptorModType MapDescriptorModType(ModDescriptorType modDescriptorType)
        {
            return modDescriptorType switch
            {
                ModDescriptorType.JsonMetadata => Parser.Common.DescriptorModType.JsonMetadata,
                _ => Parser.Common.DescriptorModType.DescriptorMod,
            };
        }

        /// <summary>
        /// Maps the type of the descriptor.
        /// </summary>
        /// <param name="modDescriptorType">Type of the mod descriptor.</param>
        /// <returns>DescriptorType.</returns>
        protected virtual DescriptorType MapDescriptorType(ModDescriptorType modDescriptorType)
        {
            return modDescriptorType switch
            {
                ModDescriptorType.JsonMetadata => DescriptorType.JsonMetadata,
                _ => DescriptorType.DescriptorMod,
            };
        }

        #endregion Methods
    }
}
