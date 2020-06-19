// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 06-19-2020
//
// Last Modified By : Mario
// Last Modified On : 06-19-2020
// ***********************************************************************
// <copyright file="ModMergeService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Services.Common;
using IronyModManager.Services.Common.MessageBus;
using IronyModManager.Shared;
using IronyModManager.Shared.MessageBus;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class ModMergeService.
    /// Implements the <see cref="IronyModManager.Services.ModBaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.IModMergeService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.ModBaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IModMergeService" />
    public class ModMergeService : ModBaseService, IModMergeService
    {
        #region Fields

        /// <summary>
        /// The message bus
        /// </summary>
        private readonly IMessageBus messageBus;

        /// <summary>
        /// The mod merge exporter
        /// </summary>
        private readonly IModMergeExporter modMergeExporter;

        /// <summary>
        /// The mod patch exporter
        /// </summary>
        private readonly IModPatchExporter modPatchExporter;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModMergeService" /> class.
        /// </summary>
        /// <param name="messageBus">The message bus.</param>
        /// <param name="modPatchExporter">The mod patch exporter.</param>
        /// <param name="modMergeExporter">The mod merge exporter.</param>
        /// <param name="definitionInfoProviders">The definition information providers.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="modWriter">The mod writer.</param>
        /// <param name="modParser">The mod parser.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ModMergeService(IMessageBus messageBus, IModPatchExporter modPatchExporter, IModMergeExporter modMergeExporter, IEnumerable<IDefinitionInfoProvider> definitionInfoProviders,
            IReader reader, IModWriter modWriter,
            IModParser modParser, IGameService gameService,
            IStorageProvider storageProvider, IMapper mapper) : base(definitionInfoProviders, reader, modWriter, modParser, gameService, storageProvider, mapper)
        {
            this.messageBus = messageBus;
            this.modMergeExporter = modMergeExporter;
            this.modPatchExporter = modPatchExporter;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// merge collection as an asynchronous operation.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="modOrder">The mod order.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> MergeCollectionAsync(IConflictResult conflictResult, IList<string> modOrder, string collectionName)
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return false;
            }
            var total = conflictResult.AllConflicts.GetAll().Count();
            if (total > 0)
            {
                var exportPath = Path.Combine(game.UserDirectory, collectionName.GenerateValidFileName());

                var collection = GetAllModCollectionsInternal().FirstOrDefault(p => p.IsSelected);
                var patchName = GenerateCollectionPatchName(collection.Name);
                var state = await modPatchExporter.GetPatchStateAsync(new ModPatchExporterParameters()
                {
                    RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                    PatchName = patchName
                }, true);

                foreach (var file in conflictResult.AllConflicts.GetAllFileKeys())
                {
                    var definitions = conflictResult.AllConflicts.GetByFile(file).Where(p => p.ValueType != Parser.Common.ValueType.Variable &&
                                                                                            p.ValueType != Parser.Common.ValueType.Namespace &&
                                                                                            p.ValueType != Parser.Common.ValueType.EmptyFile);
                    var lastPercentage = 0;
                    int processed = 0;
                    if (definitions.Count() > 0)
                    {
                        var exportDefinitions = new List<IDefinition>();
                        var exportSingleDefinitions = new List<IDefinition>();
                        foreach (var definition in definitions)
                        {
                            // Orphans are placed under resolved items during analysis so no need to check them
                            var resolved = conflictResult.ResolvedConflicts.GetByTypeAndId(definition.TypeAndId);
                            var overwritten = conflictResult.OverwrittenConflicts.GetByTypeAndId(definition.TypeAndId);
                            if (resolved.Count() > 0 || overwritten.Count() > 0)
                            {
                                // Resolved takes priority, since if an item was resolved no need to use the overwritten code
                                // Also fetch the code from the patch state.json object to get the latest version of the code to dump
                                if (resolved.Count() > 0)
                                {
                                    foreach (var item in resolved)
                                    {
                                        var copy = CopyDefinition(item);
                                        copy.Code = state.ConflictHistory.FirstOrDefault(p => p.TypeAndId.Equals(item.TypeAndId)).Code;
                                        exportSingleDefinitions.Add(copy);
                                    }
                                }
                                else
                                {
                                    foreach (var item in overwritten)
                                    {
                                        var copy = CopyDefinition(item);
                                        exportSingleDefinitions.Add(copy);
                                    }
                                }
                            }
                            else
                            {
                                // Check if this is a conflict so we can then perform evaluation of which definition would win based on current order
                                var conflicted = conflictResult.Conflicts.GetByTypeAndId(definition.TypeAndId);
                                if (conflicted.Count() > 0)
                                {
                                    exportDefinitions.Add(EvalDefinitionPriorityInternal(conflicted.OrderBy(p => modOrder.IndexOf(p.ModName))).Definition);
                                }
                                else
                                {
                                    exportDefinitions.Add(definition);
                                }
                            }
                        }

                        await modMergeExporter.ExportDefinitionsAsync(new ModMergeExporterParameters()
                        {
                            ExportPath = exportPath,
                            Definitions = exportDefinitions.Count > 0 ? new List<IDefinition>() { MergeDefinitions(exportDefinitions) } : null,
                            PatchDefinitions = exportSingleDefinitions,
                            Game = game.Type
                        });
                        processed = exportDefinitions.Count + exportSingleDefinitions.Count;
                        var percentage = GetProgressPercentage(total, processed, 100);
                        if (lastPercentage != percentage)
                        {
                            await messageBus.PublishAsync(new ModMergeProgressEvent(percentage));
                        }
                        lastPercentage = percentage;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the progress percentage.
        /// </summary>
        /// <param name="total">The total.</param>
        /// <param name="processed">The processed.</param>
        /// <param name="maxPerc">The maximum perc.</param>
        /// <returns>System.Int32.</returns>
        protected virtual int GetProgressPercentage(double total, double processed, int maxPerc = 100)
        {
            var perc = Convert.ToInt32(processed / total * 100);
            if (perc < 1)
            {
                perc = 1;
            }
            else if (perc > maxPerc)
            {
                perc = maxPerc;
            }
            return perc;
        }

        /// <summary>
        /// Merges the definitions.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <returns>IDefinition.</returns>
        protected virtual IDefinition MergeDefinitions(IEnumerable<IDefinition> definitions)
        {
            static void appendLine(StringBuilder sb, string code)
            {
                if (!string.IsNullOrWhiteSpace(code))
                {
                    sb.AppendLine(code);
                }
            }
            static void mergeCode(StringBuilder sb, string code)
            {
                if (!string.IsNullOrWhiteSpace(code))
                {
                    var filtered = code.Substring(code.IndexOf("{") + 1);
                    filtered = filtered.Substring(0, filtered.LastIndexOf("}")).Trim();
                    sb.AppendLine(filtered);
                }
            }

            var sb = new StringBuilder();
            var copy = CopyDefinition(definitions.FirstOrDefault());
            if (!copy.IsFirstLevel)
            {
                sb.AppendLine(copy.Code.Substring(0, copy.Code.IndexOf("{") + 1));
            }
            foreach (var definition in definitions)
            {
                if (definition.IsFirstLevel)
                {
                    appendLine(sb, definition.Code);
                }
                else
                {
                    mergeCode(sb, definition.Code);
                }
            }
            if (!copy.IsFirstLevel)
            {
                sb.Append("}");
            }
            copy.Code = sb.ToString();
            return copy;
        }

        #endregion Methods
    }
}
