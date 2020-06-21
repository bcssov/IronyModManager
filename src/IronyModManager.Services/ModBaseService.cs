// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 04-07-2020
//
// Last Modified By : Mario
// Last Modified On : 06-21-2020
// ***********************************************************************
// <copyright file="ModBaseService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IronyModManager.DI;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class ModBaseService. Implements the <see cref="IronyModManager.Services.BaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    public abstract class ModBaseService : BaseService
    {
        #region Fields

        /// <summary>
        /// The patch collection name
        /// </summary>
        protected const string PatchCollectionName = nameof(IronyModManager) + "_";

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModBaseService" /> class.
        /// </summary>
        /// <param name="definitionInfoProviders">The definition information providers.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="modWriter">The mod writer.</param>
        /// <param name="modParser">The mod parser.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ModBaseService(IEnumerable<IDefinitionInfoProvider> definitionInfoProviders, IReader reader, IModWriter modWriter,
            IModParser modParser, IGameService gameService,
            IStorageProvider storageProvider, IMapper mapper) : base(storageProvider, mapper)
        {
            DefinitionInfoProviders = definitionInfoProviders;
            GameService = gameService;
            Reader = reader;
            ModParser = modParser;
            ModWriter = modWriter;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the definition information providers.
        /// </summary>
        /// <value>The definition information providers.</value>
        protected IEnumerable<IDefinitionInfoProvider> DefinitionInfoProviders { get; private set; }

        /// <summary>
        /// Gets the game service.
        /// </summary>
        /// <value>The game service.</value>
        protected IGameService GameService { get; private set; }

        /// <summary>
        /// Gets the mod parser.
        /// </summary>
        /// <value>The mod parser.</value>
        protected IModParser ModParser { get; private set; }

        /// <summary>
        /// Gets the mod writer.
        /// </summary>
        /// <value>The mod writer.</value>
        protected IModWriter ModWriter { get; private set; }

        /// <summary>
        /// Gets the reader.
        /// </summary>
        /// <value>The reader.</value>
        protected IReader Reader { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Copies the definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>IDefinition.</returns>
        protected virtual IDefinition CopyDefinition(IDefinition definition)
        {
            var newDefinition = DIResolver.Get<IDefinition>();
            newDefinition.Code = definition.Code;
            newDefinition.ContentSHA = definition.ContentSHA;
            newDefinition.DefinitionSHA = definition.DefinitionSHA;
            newDefinition.Dependencies = definition.Dependencies;
            newDefinition.ErrorColumn = definition.ErrorColumn;
            newDefinition.ErrorLine = definition.ErrorLine;
            newDefinition.ErrorMessage = definition.ErrorMessage;
            newDefinition.File = definition.File;
            newDefinition.GeneratedFileNames = definition.GeneratedFileNames;
            newDefinition.OverwrittenFileNames = definition.OverwrittenFileNames;
            newDefinition.AdditionalFileNames = definition.AdditionalFileNames;
            newDefinition.Id = definition.Id;
            newDefinition.IsFirstLevel = definition.IsFirstLevel;
            newDefinition.ModName = definition.ModName;
            newDefinition.Type = definition.Type;
            newDefinition.UsedParser = definition.UsedParser;
            newDefinition.ValueType = definition.ValueType;
            newDefinition.Tags = definition.Tags;
            newDefinition.OriginalCode = definition.OriginalCode;
            newDefinition.CodeSeparator = definition.CodeSeparator;
            newDefinition.CodeTag = definition.CodeTag;
            return newDefinition;
        }

        /// <summary>
        /// delete descriptors internal as an asynchronous operation.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual async Task<bool> DeleteDescriptorsInternalAsync(IEnumerable<IMod> mods)
        {
            var game = GameService.GetSelected();
            if (game != null && mods?.Count() > 0)
            {
                var tasks = new List<Task>();
                foreach (var item in mods)
                {
                    var task = ModWriter.DeleteDescriptorAsync(new ModWriterParameters()
                    {
                        Mod = item,
                        RootDirectory = game.UserDirectory
                    });
                    tasks.Add(task);
                }
                await Task.WhenAll(tasks);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Evals the definition priority internal.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <returns>IPriorityDefinitionResult.</returns>
        protected virtual IPriorityDefinitionResult EvalDefinitionPriorityInternal(IEnumerable<IDefinition> definitions)
        {
            var game = GameService.GetSelected();
            var result = GetModelInstance<IPriorityDefinitionResult>();
            if (game != null && definitions?.Count() > 1)
            {
                var definitionEvals = new List<DefinitionEval>();
                var provider = DefinitionInfoProviders.FirstOrDefault(p => p.CanProcess(game.Type));
                bool isFios = false;
                if (provider != null)
                {
                    bool overrideSkipped = false;
                    isFios = provider.DefinitionUsesFIOSRules(definitions.First());
                    foreach (var item in definitions)
                    {
                        var hasOverrides = definitions.Any(p => (p.Dependencies?.Any(p => p.Equals(item.ModName))).GetValueOrDefault());
                        if (hasOverrides)
                        {
                            overrideSkipped = true;
                            continue;
                        }
                        if (isFios)
                        {
                            definitionEvals.Add(new DefinitionEval()
                            {
                                Definition = item,
                                FileName = item.AdditionalFileNames.OrderBy(p => Path.GetFileNameWithoutExtension(p), StringComparer.Ordinal).First()
                            });
                        }
                        else
                        {
                            definitionEvals.Add(new DefinitionEval()
                            {
                                Definition = item,
                                FileName = item.AdditionalFileNames.OrderBy(p => Path.GetFileNameWithoutExtension(p), StringComparer.Ordinal).Last()
                            });
                        }
                    }
                    var uniqueDefinitions = definitionEvals.GroupBy(p => p.Definition.ModName).Select(p => p.First());
                    if (uniqueDefinitions.Count() == 1 && overrideSkipped)
                    {
                        result.Definition = definitionEvals.First().Definition;
                        result.PriorityType = DefinitionPriorityType.ModOverride;
                    }
                    else if (uniqueDefinitions.Count() > 1)
                    {
                        // Has same filenames?
                        if (uniqueDefinitions.GroupBy(p => p.FileNameCI).Count() == 1)
                        {
                            result.Definition = uniqueDefinitions.Last().Definition;
                            result.PriorityType = DefinitionPriorityType.ModOrder;
                        }
                        else
                        {
                            // Using FIOS or LIOS?
                            if (isFios)
                            {
                                result.Definition = uniqueDefinitions.OrderBy(p => Path.GetFileNameWithoutExtension(p.FileName), StringComparer.Ordinal).First().Definition;
                                result.PriorityType = DefinitionPriorityType.FIOS;
                            }
                            else
                            {
                                result.Definition = uniqueDefinitions.OrderBy(p => Path.GetFileNameWithoutExtension(p.FileName), StringComparer.Ordinal).Last().Definition;
                                result.PriorityType = DefinitionPriorityType.LIOS;
                            }
                        }
                    }
                }
            }
            if (result.Definition == null)
            {
                result.Definition = definitions?.FirstOrDefault();
            }
            return result;
        }

        /// <summary>
        /// Generates the name of the collection patch.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>System.String.</returns>
        protected virtual string GenerateCollectionPatchName(string collectionName)
        {
            var fileName = $"{PatchCollectionName}{collectionName}";
            return fileName.GenerateValidFileName();
        }

        /// <summary>
        /// Generates the patch mod descriptor.
        /// </summary>
        /// <param name="allMods">All mods.</param>
        /// <param name="game">The game.</param>
        /// <param name="patchName">Name of the patch.</param>
        /// <returns>IMod.</returns>
        protected virtual IMod GeneratePatchModDescriptor(IEnumerable<IMod> allMods, IGame game, string patchName)
        {
            var mod = DIResolver.Get<IMod>();
            var dependencies = allMods.Where(p => p.Dependencies?.Count() > 0 && !IsPatchModInternal(p)).Select(p => p.Name).Distinct().ToList();
            if (dependencies.Count > 0)
            {
                mod.Dependencies = dependencies;
            }
            mod.DescriptorFile = $"{Shared.Constants.ModDirectory}/{patchName}{Shared.Constants.ModExtension}";
            mod.FileName = GetModDirectory(game, patchName).Replace("\\", "/");
            mod.Name = patchName;
            mod.Source = ModSource.Local;
            mod.Version = allMods.OrderByDescending(p => p.Version).FirstOrDefault() != null ? allMods.OrderByDescending(p => p.Version).FirstOrDefault().Version : string.Empty;
            mod.Tags = new List<string>() { "Fixes" };
            return mod;
        }

        /// <summary>
        /// Gets all mod collections internal.
        /// </summary>
        /// <returns>IEnumerable&lt;IModCollection&gt;.</returns>
        protected virtual IEnumerable<IModCollection> GetAllModCollectionsInternal()
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return new List<IModCollection>();
            }
            var collections = StorageProvider.GetModCollections().Where(s => s.Game.Equals(game.Type));
            if (collections.Count() > 0)
            {
                return collections.OrderBy(p => p.Name);
            }
            return new List<IModCollection>();
        }

        /// <summary>
        /// Gets the collection mods.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <returns>IEnumerable&lt;IMod&gt;.</returns>
        protected virtual IEnumerable<IMod> GetCollectionMods(IEnumerable<IMod> mods = null)
        {
            if (mods == null)
            {
                mods = GetInstalledModsInternal(GameService.GetSelected(), false);
            }
            var collectionMods = new List<IMod>();
            var collections = GetAllModCollectionsInternal();
            if (collections?.Count() > 0)
            {
                var collection = collections.FirstOrDefault(p => p.IsSelected);
                foreach (var item in collection.Mods)
                {
                    var mod = mods.FirstOrDefault(p => p.DescriptorFile.Equals(item, StringComparison.OrdinalIgnoreCase));
                    if (mod != null)
                    {
                        collectionMods.Add(mod);
                    }
                }
            }
            return collectionMods;
        }

        /// <summary>
        /// Gets the installed mods internal.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="ignorePatchMods">if set to <c>true</c> [ignore patch mods].</param>
        /// <returns>IEnumerable&lt;IMod&gt;.</returns>
        /// <exception cref="ArgumentNullException">game</exception>
        protected virtual IEnumerable<IMod> GetInstalledModsInternal(IGame game, bool ignorePatchMods)
        {
            if (game == null)
            {
                throw new ArgumentNullException("game");
            }
            var result = new List<IMod>();
            var installedMods = Reader.Read(Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory));
            if (installedMods?.Count() > 0)
            {
                foreach (var installedMod in installedMods.Where(p => p.Content.Count() > 0))
                {
                    var mod = Mapper.Map<IMod>(ModParser.Parse(installedMod.Content));
                    if (ignorePatchMods && IsPatchModInternal(mod))
                    {
                        continue;
                    }
                    mod.DescriptorFile = $"{Shared.Constants.ModDirectory}/{installedMod.FileName}";
                    mod.Source = GetModSource(installedMod);
                    if (mod.Source == ModSource.Paradox)
                    {
                        mod.RemoteId = GetPdxModId(installedMod.FileName);
                    }
                    if (string.IsNullOrWhiteSpace(mod.FileName))
                    {
                        mod.FileName = string.Empty;
                        mod.FullPath = string.Empty;
                    }
                    else
                    {
                        if (Path.IsPathFullyQualified(mod.FileName))
                        {
                            mod.FullPath = mod.FileName;
                        }
                        else
                        {
                            // Check user directory and workshop directory.
                            var userDirectoryMod = Path.Combine(game.UserDirectory, mod.FileName);
                            var workshopDirectoryMod = Path.Combine(game.WorkshopDirectory, mod.FileName);
                            if (File.Exists(userDirectoryMod) || Directory.Exists(userDirectoryMod))
                            {
                                mod.FullPath = userDirectoryMod;
                            }
                            else if (File.Exists(workshopDirectoryMod) || Directory.Exists(workshopDirectoryMod))
                            {
                                mod.FullPath = workshopDirectoryMod;
                            }
                        }
                    }
                    // Validate if path exists
                    mod.IsValid = File.Exists(mod.FullPath) || Directory.Exists(mod.FullPath);
                    result.Add(mod);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the mod directory.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="modCollection">The mod collection.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetModDirectory(IGame game, IModCollection modCollection)
        {
            return GetModDirectory(game, GenerateCollectionPatchName(modCollection.Name));
        }

        /// <summary>
        /// Gets the mod directory.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="patchName">Name of the patch.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetModDirectory(IGame game, string patchName)
        {
            var path = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory, patchName);
            return path;
        }

        /// <summary>
        /// Gets the mod source.
        /// </summary>
        /// <param name="fileInfo">The file information.</param>
        /// <returns>ModSource.</returns>
        protected virtual ModSource GetModSource(IFileInfo fileInfo)
        {
            if (fileInfo.FileName.Contains(Constants.Paradox_mod_id))
            {
                return ModSource.Paradox;
            }
            else if (fileInfo.FileName.Contains(Constants.Steam_mod_id))
            {
                return ModSource.Steam;
            }
            return ModSource.Local;
        }

        /// <summary>
        /// Gets the PDX mod identifier.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="isDirectory">if set to <c>true</c> [is directory].</param>
        /// <returns>System.Int32.</returns>
        protected virtual int GetPdxModId(string path, bool isDirectory = false)
        {
            var name = !isDirectory ? Path.GetFileNameWithoutExtension(path) : path;
            int.TryParse(name.Replace(Constants.Paradox_mod_id, string.Empty), out var id);
            return id;
        }

        /// <summary>
        /// Determines whether [is patch mod] [the specified mod].
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns><c>true</c> if [is patch mod] [the specified mod]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsPatchModInternal(IMod mod)
        {
            if (mod != null)
            {
                return IsPatchModInternal(mod.Name);
            }
            return false;
        }

        /// <summary>
        /// Determines whether [is patch mod] [the specified mod name].
        /// </summary>
        /// <param name="modName">Name of the mod.</param>
        /// <returns><c>true</c> if [is patch mod] [the specified mod name]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsPatchModInternal(string modName)
        {
            if (!string.IsNullOrWhiteSpace(modName))
            {
                return modName.StartsWith(PatchCollectionName);
            }
            return false;
        }

        /// <summary>
        /// Populates the mod path.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="collectionMods">The collection mods.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> PopulateModPath(IEnumerable<IDefinition> definitions, IEnumerable<IMod> collectionMods)
        {
            if (definitions?.Count() > 0)
            {
                foreach (var item in definitions)
                {
                    if (IsPatchModInternal(item.ModName))
                    {
                        item.ModPath = GetModDirectory(GameService.GetSelected(), item.ModName);
                    }
                    else
                    {
                        item.ModPath = collectionMods.FirstOrDefault(p => p.Name.Equals(item.ModName)).FullPath;
                    }
                }
            }
            return definitions;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class DefinitionEval.
        /// </summary>
        protected class DefinitionEval
        {
            #region Properties

            /// <summary>
            /// Gets or sets the definition.
            /// </summary>
            /// <value>The definition.</value>
            public IDefinition Definition { get; set; }

            /// <summary>
            /// Gets or sets the name of the file.
            /// </summary>
            /// <value>The name of the file.</value>
            public string FileName { get; set; }

            /// <summary>
            /// Gets the file name ci.
            /// </summary>
            /// <value>The file name ci.</value>
            public string FileNameCI
            {
                get
                {
                    return FileName.ToLowerInvariant();
                }
            }

            #endregion Properties
        }

        #endregion Classes
    }
}
