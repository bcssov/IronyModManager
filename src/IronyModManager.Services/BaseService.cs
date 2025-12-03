// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 12-02-2025
// ***********************************************************************
// <copyright file="BaseService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IronyModManager.DI;
using IronyModManager.IO.Common;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common;
using IronyModManager.Services.Common;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class BaseService.
    /// Implements the <see cref="IronyModManager.Services.Common.IBaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.IBaseService" />
    /// <remarks>Initializes a new instance of the <see cref="BaseService" /> class.</remarks>
    public abstract class BaseService(IStorageProvider storageProvider, IMapper mapper) : IBaseService
    {
        #region Properties

        /// <summary>
        /// Gets the mapper.
        /// </summary>
        /// <value>The mapper.</value>
        protected IMapper Mapper { get; private set; } = mapper;

        /// <summary>
        /// Gets the storage provider.
        /// </summary>
        /// <value>The storage provider.</value>
        protected IStorageProvider StorageProvider { get; private set; } = storageProvider;

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
        protected virtual DescriptorModType MapDescriptorModType(ModDescriptorType modDescriptorType)
        {
            return modDescriptorType switch
            {
                ModDescriptorType.JsonMetadata => DescriptorModType.JsonMetadata,
                ModDescriptorType.JsonMetadataV2 => DescriptorModType.JsonMetadataV2,
                _ => DescriptorModType.DescriptorMod
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
                ModDescriptorType.JsonMetadataV2 => DescriptorType.JsonMetadataV2,
                _ => DescriptorType.DescriptorMod
            };
        }

        #endregion Methods
    }
}
