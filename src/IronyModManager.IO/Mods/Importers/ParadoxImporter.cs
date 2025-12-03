// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 06-22-2020
//
// Last Modified By : Mario
// Last Modified On : 12-03-2025
// ***********************************************************************
// <copyright file="ParadoxImporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.DI;
using IronyModManager.IO.Common;
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
    /// <remarks>Initializes a new instance of the <see cref="ParadoxImporter" /> class.</remarks>
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    internal class ParadoxImporter(ILogger logger)
    {
        #region Fields

        /// <summary>
        /// The collection name
        /// </summary>
        private const string CollectionName = "Paradox";

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger = logger;

        #endregion Fields

        #region Methods

        /// <summary>
        /// import as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public async Task<ICollectionImportResult> ImportAsync(ModCollectionExporterParams parameters)
        {
            if (parameters.DescriptorType == DescriptorType.DescriptorMod)
            {
                var path = Path.Combine(Path.GetDirectoryName(parameters.ModDirectory) ?? string.Empty, Constants.DLC_load_path);
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
            }
            else if (parameters.DescriptorType == DescriptorType.JsonMetadata)
            {
                var path = Path.Combine(Path.GetDirectoryName(parameters.ModDirectory) ?? string.Empty, Constants.Content_load_path);
                if (File.Exists(path))
                {
                    var result = DIResolver.Get<ICollectionImportResult>();
                    var content = await File.ReadAllTextAsync(path);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        try
                        {
                            var model = JsonConvert.DeserializeObject<ContentLoad>(content);
                            if (model.EnabledMods?.Count > 0)
                            {
                                result.Name = CollectionName;
                                result.FullPaths = [.. model.EnabledMods.Where(p => p != null && !string.IsNullOrWhiteSpace(p.Path)).Select(m => m.Path)];
                                return result;
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                        }
                    }
                }
            }
            else
            {
                var path = Path.Combine(Path.GetDirectoryName(parameters.ModDirectory) ?? string.Empty, Constants.Playsets_path);
                if (File.Exists(path))
                {
                    var result = DIResolver.Get<ICollectionImportResult>();
                    var content = await File.ReadAllTextAsync(path);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        try
                        {
                            var model = JsonConvert.DeserializeObject<Playsets>(content);
                            if (model is { PlaysetsCollection: not null })
                            {
                                var activeCollection = model.PlaysetsCollection.FirstOrDefault(p => p.IsActive.GetValueOrDefault());
                                if (activeCollection is { OrderedListMods: not null })
                                {
                                    result.Name = CollectionName;
                                    result.FullPaths = [.. activeCollection.OrderedListMods.Where(p => p != null && !string.IsNullOrWhiteSpace(p.Path)).Select(m => m.Path.TrimEnd("/"))];
                                    return result;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                        }
                    }
                }
            }

            return null;
        }

        #endregion Methods
    }
}
