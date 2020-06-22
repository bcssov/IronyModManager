// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 06-22-2020
//
// Last Modified By : Mario
// Last Modified On : 06-22-2020
// ***********************************************************************
// <copyright file="ParadoxImporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Mods.Models;
using IronyModManager.Shared;
using Newtonsoft.Json;

namespace IronyModManager.IO.Mods
{
    /// <summary>
    /// Class ParadoxImporter.
    /// </summary>
    internal class ParadoxImporter
    {
        #region Fields

        /// <summary>
        /// The collection name
        /// </summary>
        private const string CollectionName = "Paradox";

        /// <summary>
        /// The DLC load path
        /// </summary>
        private const string DLC_load_path = "dlc_load.json";

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
        public async Task<bool> ImportAsync(ModCollectionExporterParams parameters)
        {
            var path = Path.Combine(parameters.ModDirectory, DLC_load_path);
            if (File.Exists(path))
            {
                var content = await File.ReadAllTextAsync(path);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    try
                    {
                        var model = JsonConvert.DeserializeObject<DLCLoad>(content);
                        if (model.EnabledMods?.Count > 0)
                        {
                            parameters.Mod.Name = CollectionName;
                            parameters.Mod.Mods = model.EnabledMods;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
            }
            return false;
        }

        #endregion Methods
    }
}
