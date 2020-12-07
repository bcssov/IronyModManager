// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 12-07-2020
//
// Last Modified By : Mario
// Last Modified On : 12-07-2020
// ***********************************************************************
// <copyright file="ExternalEditorService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using System;
using AutoMapper;
using IronyModManager.DI;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Storage.Common;
using SmartFormat;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class ExternalEditorService.
    /// Implements the <see cref="IronyModManager.Services.BaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.IExternalEditorService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IExternalEditorService" />
    public class ExternalEditorService : BaseService, IExternalEditorService
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalEditorService" /> class.
        /// </summary>
        /// <param name="preferenceService">The preference service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ExternalEditorService(IPreferencesService preferenceService, IStorageProvider storageProvider, IMapper mapper) : base(storageProvider, mapper)
        {
            PreferencesService = preferenceService;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the preferences service.
        /// </summary>
        /// <value>The preferences service.</value>
        protected IPreferencesService PreferencesService { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IExternalEditor.</returns>
        public IExternalEditor Get()
        {
            var preferences = PreferencesService.Get();
            return Mapper.Map<IExternalEditor>(preferences);
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <returns>IExternalEditorFiles.</returns>
        public IExternalEditorFiles GetFiles()
        {
            var model = GetModelInstance<IExternalEditorFiles>();
            model.LeftDiff = DIResolver.Get<ITempFile>();
            model.LeftDiff.Create();
            model.RightDiff = DIResolver.Get<ITempFile>();
            model.RightDiff.Create();
            return model;
        }

        /// <summary>
        /// Gets the launch arguments.
        /// </summary>
        /// <param name="leftLocation">The left location.</param>
        /// <param name="rightLocation">The right location.</param>
        /// <returns>System.String.</returns>
        public string GetLaunchArguments(string leftLocation, string rightLocation)
        {
            var opts = Get();
            if (!string.IsNullOrWhiteSpace(opts.ExternalEditorLocation) && !string.IsNullOrWhiteSpace(opts.ExternalEditorParameters))
            {
                var launchArgs = Smart.Format($"{opts.ExternalEditorLocation} {opts.ExternalEditorParameters}", new { Left = $"\"{leftLocation}\"", Right = $"\"{rightLocation}\"" });
                return launchArgs;
            }
            return string.Empty;
        }

        /// <summary>
        /// Saves the specified external editor.
        /// </summary>
        /// <param name="externalEditor">The external editor.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Save(IExternalEditor externalEditor)
        {
            var preferences = PreferencesService.Get();
            return PreferencesService.Save(Mapper.Map(externalEditor, preferences));
        }

        #endregion Methods
    }
}
