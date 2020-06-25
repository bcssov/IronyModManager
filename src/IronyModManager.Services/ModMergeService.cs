// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 06-19-2020
//
// Last Modified By : Mario
// Last Modified On : 06-25-2020
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
using IronyModManager.DI;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Services.Common;
using IronyModManager.Services.Common.MessageBus;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;
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
        /// <param name="cache">The cache.</param>
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
        public ModMergeService(ICache cache, IMessageBus messageBus, IModPatchExporter modPatchExporter,
            IModMergeExporter modMergeExporter, IEnumerable<IDefinitionInfoProvider> definitionInfoProviders,
            IReader reader, IModWriter modWriter,
            IModParser modParser, IGameService gameService,
            IStorageProvider storageProvider, IMapper mapper) : base(cache, definitionInfoProviders, reader, modWriter, modParser, gameService, storageProvider, mapper)
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
        /// <returns>Task&lt;IMod&gt;.</returns>
        public virtual async Task<IMod> MergeCollectionAsync(IConflictResult conflictResult, IList<string> modOrder, string collectionName)
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return null;
            }
            var total = conflictResult.AllConflicts.GetAllFileKeys().Count();
            if (conflictResult.AllConflicts.GetAll().Count() > 0)
            {
                var allMods = GetInstalledModsInternal(game, false).ToList();
                var mergeCollectionPath = collectionName.GenerateValidFileName();
                var collectionMods = GetCollectionMods(allMods);

                await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters()
                {
                    RootDirectory = game.UserDirectory,
                    Path = Path.Combine(Shared.Constants.ModDirectory, mergeCollectionPath)
                }, true);
                await ModWriter.CreateModDirectoryAsync(new ModWriterParameters()
                {
                    RootDirectory = game.UserDirectory,
                    Path = Shared.Constants.ModDirectory
                });
                await ModWriter.CreateModDirectoryAsync(new ModWriterParameters()
                {
                    RootDirectory = game.UserDirectory,
                    Path = Path.Combine(Shared.Constants.ModDirectory, mergeCollectionPath)
                });

                var mod = DIResolver.Get<IMod>();
                mod.DescriptorFile = $"{Shared.Constants.ModDirectory}/{mergeCollectionPath}{Shared.Constants.ModExtension}";
                mod.FileName = GetModDirectory(game, mergeCollectionPath).Replace("\\", "/");
                mod.Name = collectionName;
                mod.Source = ModSource.Local;
                mod.Version = allMods.OrderByDescending(p => p.Version).FirstOrDefault() != null ? allMods.OrderByDescending(p => p.Version).FirstOrDefault().Version : string.Empty;
                await ModWriter.WriteDescriptorAsync(new ModWriterParameters()
                {
                    Mod = mod,
                    RootDirectory = game.UserDirectory,
                    Path = mod.DescriptorFile
                }, true);
                Cache.Invalidate(ModsCachePrefix, ConstructModsCacheKey(game, true), ConstructModsCacheKey(game, false));

                var exportPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory, mergeCollectionPath);

                var collection = GetAllModCollectionsInternal().FirstOrDefault(p => p.IsSelected);
                var patchName = GenerateCollectionPatchName(collection.Name);
                var state = await modPatchExporter.GetPatchStateAsync(new ModPatchExporterParameters()
                {
                    RootPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                    PatchName = patchName
                }, true);

                var resolvedConflicts = state?.ResolvedConflicts ?? new List<IDefinition>();
                var ignoredConflicts = state?.IgnoredConflicts ?? new List<IDefinition>();
                var resolvedIndex = DIResolver.Get<IIndexedDefinitions>();
                resolvedIndex.InitMap(resolvedConflicts, true);
                conflictResult.ResolvedConflicts = resolvedIndex;
                var ignoredIndex = DIResolver.Get<IIndexedDefinitions>();
                ignoredIndex.InitMap(ignoredConflicts, true);
                conflictResult.IgnoredConflicts = ignoredIndex;

                var lastPercentage = 0;
                int processed = 0;

                foreach (var file in conflictResult.AllConflicts.GetAllFileKeys())
                {
                    var definitions = conflictResult.AllConflicts.GetByFile(file).Where(p => p.ValueType != Parser.Common.ValueType.EmptyFile);
                    if (definitions.Count() > 0)
                    {
                        var exportDefinitions = new List<IDefinition>();
                        var exportSingleDefinitions = new List<IDefinition>();
                        foreach (var definitionGroup in definitions.GroupBy(p => p.TypeAndId))
                        {
                            // Orphans are placed under resolved items during analysis so no need to check them
                            var resolved = conflictResult.ResolvedConflicts.GetByTypeAndId(definitionGroup.FirstOrDefault().TypeAndId);
                            var overwritten = conflictResult.OverwrittenConflicts.GetByTypeAndId(definitionGroup.FirstOrDefault().TypeAndId);
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
                                var conflicted = conflictResult.Conflicts.GetByTypeAndId(definitionGroup.FirstOrDefault().TypeAndId);
                                if (conflicted.Count() > 0)
                                {
                                    exportDefinitions.Add(EvalDefinitionPriorityInternal(conflicted.OrderBy(p => modOrder.IndexOf(p.ModName))).Definition);
                                }
                                else
                                {
                                    exportDefinitions.Add(definitionGroup.FirstOrDefault());
                                }
                            }
                        }

                        // Prevent exporting only namespaces
                        if (exportDefinitions.All(p => p.ValueType == Parser.Common.ValueType.Namespace))
                        {
                            exportDefinitions.Clear();
                        }
                        // Something to export?
                        if (exportDefinitions.Count > 0 || exportSingleDefinitions.Count > 0)
                        {
                            await modMergeExporter.ExportDefinitionsAsync(new ModMergeExporterParameters()
                            {
                                ExportPath = exportPath,
                                Definitions = exportDefinitions.Count > 0 ? PopulateModPath(new List<IDefinition>() { MergeDefinitions(exportDefinitions.OrderBy(p => p.Id, StringComparer.OrdinalIgnoreCase)) }, collectionMods) : null,
                                PatchDefinitions = PopulateModPath(exportSingleDefinitions, collectionMods),
                                Game = game.Type
                            });
                        }
                        processed++;
                        var percentage = GetProgressPercentage(total, processed, 100);
                        if (lastPercentage != percentage)
                        {
                            await messageBus.PublishAsync(new ModMergeProgressEvent(percentage));
                        }
                        lastPercentage = percentage;
                    }
                }
                return mod;
            }
            return null;
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
            static void appendLine(StringBuilder sb, IEnumerable<string> lines, int indent = 0)
            {
                foreach (var item in lines)
                {
                    if (indent == 0)
                    {
                        sb.AppendLine(item);
                    }
                    else
                    {
                        sb.AppendLine($"{new string(' ', indent)}{item}");
                    }
                }
            }
            static void mergeCode(StringBuilder sb, string codeTag, string separator, IEnumerable<string> variables, IEnumerable<string> lines)
            {
                if (Shared.Constants.CodeSeparators.ClosingSeparators.Map.ContainsKey(separator))
                {
                    var closingTag = Shared.Constants.CodeSeparators.ClosingSeparators.Map[separator];
                    sb.AppendLine($"{codeTag} = {separator}");
                    if (lines.Count() == 0)
                    {
                        foreach (var item in variables)
                        {
                            var splitLines = item.SplitOnNewLine();
                            appendLine(sb, splitLines, 4);
                        }
                    }
                    else
                    {
                        bool varsInserted = false;
                        foreach (var item in lines)
                        {
                            var splitLines = item.SplitOnNewLine();
                            foreach (var split in splitLines)
                            {
                                sb.AppendLine($"{new string(' ', 4)}{split}");
                                var key = split.Trim().Split("=:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0].ToLowerInvariant();
                                if (!varsInserted && split.Contains(Shared.Constants.CodeSeparators.ClosingSeparators.CurlyBracket))
                                {
                                    varsInserted = true;
                                    foreach (var var in variables)
                                    {
                                        var splitVars = var.SplitOnNewLine();
                                        appendLine(sb, splitVars, 8);
                                    }
                                }
                            }
                        }
                    }
                    sb.AppendLine(closingTag);
                }
                else
                {
                    sb.AppendLine($"{codeTag}{separator}");
                    foreach (var item in variables)
                    {
                        var splitLines = item.SplitOnNewLine();
                        appendLine(sb, splitLines, 4);
                    }
                    foreach (var item in lines)
                    {
                        var splitLines = item.SplitOnNewLine();
                        appendLine(sb, splitLines, 4);
                    }
                }
            }

            var sb = new StringBuilder();
            var copy = CopyDefinition(definitions.FirstOrDefault());
            if (copy.ValueType == Parser.Common.ValueType.Namespace || copy.ValueType == Parser.Common.ValueType.Variable)
            {
                copy.ValueType = Parser.Common.ValueType.Object;
            }
            var groups = definitions.GroupBy(p => p.CodeTag, StringComparer.OrdinalIgnoreCase);
            foreach (var group in groups.OrderBy(p => p.FirstOrDefault().CodeTag, StringComparer.OrdinalIgnoreCase))
            {
                bool hasCodeTag = !string.IsNullOrWhiteSpace(group.FirstOrDefault().CodeTag);
                if (!hasCodeTag)
                {
                    var namespaces = group.Where(p => p.ValueType == Parser.Common.ValueType.Namespace);
                    var variables = group.Where(p => p.ValueType == Parser.Common.ValueType.Variable);
                    var other = group.Where(p => p.ValueType != Parser.Common.ValueType.Variable && p.ValueType != Parser.Common.ValueType.Namespace);
                    var code = namespaces.Select(p => p.OriginalCode).Concat(variables.Select(p => p.OriginalCode)).Concat(other.Select(p => p.OriginalCode));
                    appendLine(sb, code);
                }
                else
                {
                    var namespaces = group.Where(p => p.ValueType == Parser.Common.ValueType.Namespace);
                    var variables = definitions.Where(p => p.ValueType == Parser.Common.ValueType.Variable && !string.IsNullOrWhiteSpace(p.CodeTag));
                    var other = group.Where(p => p.ValueType != Parser.Common.ValueType.Variable && p.ValueType != Parser.Common.ValueType.Namespace);
                    var vars = namespaces.Select(p => p.OriginalCode).Concat(variables.Select(p => p.OriginalCode));
                    var code = other.Select(p => p.OriginalCode);
                    mergeCode(sb, group.FirstOrDefault().CodeTag, group.FirstOrDefault().CodeSeparator, vars, code);
                }
            }

            copy.Code = sb.ToString();
            return copy;
        }

        #endregion Methods
    }
}
