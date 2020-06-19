// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 05-28-2020
//
// Last Modified By : Mario
// Last Modified On : 06-10-2020
// ***********************************************************************
// <copyright file="ParadoxosImporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using IronyModManager.DI;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Mods.Models;
using IronyModManager.Shared;

namespace IronyModManager.IO.Mods
{
    /// <summary>
    /// Class ParadoxosImporter.
    /// </summary>
    internal class ParadoxosImporter
    {
        #region Fields

        /// <summary>
        /// The mod directory
        /// </summary>
        private const string ModDirectory = "mod/{0}";

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParadoxosImporter" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ParadoxosImporter(ILogger logger)
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
            if (File.Exists(parameters.File))
            {
                void map(ParadoxosExportedList model)
                {
                    parameters.Mod.Name = model.ExportedList.Name;
                    IOrderedEnumerable<ParadoxosMod> ordered;
                    // It got confusing a bit...
                    if (model.ExportedList.CustomOrder)
                    {
                        ordered = model.ExportedList.Mod.OrderByDescending(p => p.Order);
                    }
                    else
                    {
                        ordered = model.ExportedList.Mod.OrderByDescending(p => p.ModName, StringComparer.OrdinalIgnoreCase);
                    }
                    parameters.Mod.Mods = ordered.Select(p =>
                    {
                        if (!p.FileName.StartsWith("mod/", StringComparison.OrdinalIgnoreCase))
                        {
                            return string.Format(ModDirectory, p.FileName);
                        }
                        return p.FileName;
                    }).ToList();
                }
                bool parseXML(string content)
                {
                    try
                    {
                        var model = ParseParadoxosAsXML(content);
                        map(model);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                    return false;
                }
                bool parseJson(string content)
                {
                    try
                    {
                        var model = ParseParadoxosAsJSON(content);
                        map(model);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                    return false;
                }

                var content = await File.ReadAllTextAsync(parameters.File);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    if (parameters.File.EndsWith(Constants.XMLExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        return parseXML(content);
                    }
                    else if (parameters.File.EndsWith(Constants.JsonExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        return parseJson(content);
                    }
                    else
                    {
                        // Try to guess and parse as both
                        return parseXML(content) || parseJson(content);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Parses the paradoxos as json.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>ParadoxosExportedList.</returns>
        private ParadoxosExportedList ParseParadoxosAsJSON(string content)
        {
            return JsonDISerializer.Deserialize<ParadoxosExportedList>(content);
        }

        /// <summary>
        /// Parses the paradoxos as XML.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>ParadoxosExportedList.</returns>
        private ParadoxosExportedList ParseParadoxosAsXML(string content)
        {
            var serializer = new XmlSerializer(typeof(ParadoxosExportedList));
            var reader = new StringReader(content);
            return serializer.Deserialize(reader) as ParadoxosExportedList;
        }

        #endregion Methods
    }
}
