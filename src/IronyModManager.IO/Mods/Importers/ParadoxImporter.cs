﻿// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 06-22-2020
//
// Last Modified By : Mario
// Last Modified On : 08-29-2021
// ***********************************************************************
// <copyright file="ParadoxImporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IronyModManager.DI;
using IronyModManager.IO.Common.Models;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Mods.Models.Paradox.Common;
using IronyModManager.Shared;
using Newtonsoft.Json;

namespace IronyModManager.IO.Mods.Importers
{
    /// <summary>
    /// Class ParadoxImporter.
    /// </summary>
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    internal class ParadoxImporter
    {
        #region Fields

        /// <summary>
        /// The collection name
        /// </summary>
        private const string CollectionName = "Paradox";

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParadoxImporter" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ParadoxImporter(ILogger logger)
        {
            this.logger = logger;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// import as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public async Task<ICollectionImportResult> ImportAsync(ModCollectionExporterParams parameters)
        {
            var path = Path.Combine(Path.GetDirectoryName(parameters.ModDirectory), Constants.DLC_load_path);
            if (File.Exists(path))
            {
                var result = DIResolver.Get<ICollectionImportResult>();
                var content = await File.ReadAllTextAsync(path);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    try
                    {
                        var model = JsonConvert.DeserializeObject<DLCLoad>(content);
                        if (model.EnabledMods?.Count > 0)
                        {
                            result.Name = CollectionName;
                            result.Descriptors = model.EnabledMods;
                            return result;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
            }
            return null;
        }

        #endregion Methods
    }
}
