// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 04-07-2020
//
// Last Modified By : Mario
// Last Modified On : 04-26-2022
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
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using IronyModManager.DI;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Services.Common;
using IronyModManager.Services.Resolver;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;
using IronyModManager.Shared.Models;
using IronyModManager.Storage.Common;
using ValueType = IronyModManager.Shared.Models.ValueType;

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
        /// All mods cache key
        /// </summary>
        protected const string AllModsCacheKey = "AllMods";

        /// <summary>
        /// The mods cache prefix
        /// </summary>
        protected const string ModsCacheRegion = "Mods";

        /// <summary>
        /// The patch collection name
        /// </summary>
        protected const string PatchCollectionName = nameof(IronyModManager) + "_";

        /// <summary>
        /// The regular mods cache key
        /// </summary>
        protected const string RegularModsCacheKey = "RegularMods";

        /// <summary>
        /// The path resolver
        /// </summary>
        protected readonly IGameRootPathResolver pathResolver;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModBaseService" /> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="definitionInfoProviders">The definition information providers.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="modWriter">The mod writer.</param>
        /// <param name="modParser">The mod parser.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ModBaseService(ICache cache, IEnumerable<IDefinitionInfoProvider> definitionInfoProviders, IReader reader, IModWriter modWriter,
            IModParser modParser, IGameService gameService,
            IStorageProvider storageProvider, IMapper mapper) : base(storageProvider, mapper)
        {
            Cache = cache;
            DefinitionInfoProviders = definitionInfoProviders;
            GameService = gameService;
            Reader = reader;
            ModParser = modParser;
            ModWriter = modWriter;
            pathResolver = new GameRootPathResolver();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the cache.
        /// </summary>
        /// <value>The cache.</value>
        protected ICache Cache { get; private set; }

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
        /// Checks if mod should be locked.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="mod">The mod.</param>
        /// <returns>bool.</returns>
        protected virtual bool CheckIfModShouldBeLocked(IGame game, IMod mod)
        {
            if (game != null && mod != null)
            {
                var fullPath = mod.FullPath ?? string.Empty;
                return IsPatchModInternal(mod.Name) || (mod.Source == ModSource.Local &&
                    (fullPath.EndsWith(Shared.Constants.ZipExtension, StringComparison.OrdinalIgnoreCase) || fullPath.EndsWith(Shared.Constants.BinExtension, StringComparison.OrdinalIgnoreCase)) &&
                    (fullPath.StartsWith(game.UserDirectory) || fullPath.StartsWith(game.CustomModDirectory)));
            }
            return false;
        }

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
            newDefinition.ModName = definition.ModName;
            newDefinition.Type = definition.Type;
            newDefinition.UsedParser = definition.UsedParser;
            newDefinition.ValueType = definition.ValueType;
            newDefinition.Tags = definition.Tags;
            newDefinition.OriginalCode = definition.OriginalCode;
            newDefinition.CodeSeparator = definition.CodeSeparator;
            newDefinition.CodeTag = definition.CodeTag;
            newDefinition.Order = definition.Order;
            newDefinition.OriginalModName = definition.OriginalModName;
            newDefinition.OriginalFileName = definition.OriginalFileName;
            newDefinition.DiskFile = definition.DiskFile;
            newDefinition.Variables = definition.Variables;
            newDefinition.ExistsInLastFile = definition.ExistsInLastFile;
            newDefinition.VirtualPath = definition.VirtualPath;
            newDefinition.CustomPriorityOrder = definition.CustomPriorityOrder;
            newDefinition.IsCustomPatch = definition.IsCustomPatch;
            newDefinition.IsFromGame = definition.IsFromGame;
            newDefinition.AllowDuplicate = definition.AllowDuplicate;
            newDefinition.ResetType = definition.ResetType;
            newDefinition.FileNameSuffix = definition.FileNameSuffix;
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
                Cache.Invalidate(new CacheInvalidateParameters() { Region = ModsCacheRegion, Prefix = game.Type, Keys = new List<string> { GetModsCacheKey(true), GetModsCacheKey(false) } });
                return true;
            }
            return false;
        }

        /// <summary>
        /// Evals the definition priority internal.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="forceFios">if set to <c>true</c> [force fios].</param>
        /// <returns>IPriorityDefinitionResult.</returns>
        protected virtual IPriorityDefinitionResult EvalDefinitionPriorityInternal(IEnumerable<IDefinition> definitions, bool forceFios = false)
        {
            // We're expecting properly ordered definitions based on load order.
            // In case of game being included this should be done by the calling method as well,
            // though there should not be any problems since it's all based on a list of strings modOrder.IndexOf(modName).
            // And the game is never a mod. If this changes this is going to be bad for me.
            var game = GameService.GetSelected();
            var result = GetModelInstance<IPriorityDefinitionResult>();
            var noProvider = false;
            if (game != null && definitions?.Count() > 1)
            {
                var provider = DefinitionInfoProviders.FirstOrDefault(p => p.CanProcess(game.Type));
                noProvider = provider == null;
                if (!noProvider)
                {
                    // Handle localizations differently
                    var file = definitions.FirstOrDefault().File ?? string.Empty;
                    if (file.StartsWith(Shared.Constants.LocalizationDirectory))
                    {
                        IEnumerable<IDefinition> filtered = null;
                        if (definitions.Any(p => p.FileCI.Contains(Shared.Constants.LocalizationReplaceDirectory, StringComparison.OrdinalIgnoreCase)))
                        {
                            var replaceDefinitions = definitions.Where(p => p.FileCI.Contains(Shared.Constants.LocalizationReplaceDirectory, StringComparison.OrdinalIgnoreCase));
                            if (replaceDefinitions.GroupBy(p => p.CustomPriorityOrder).Count() == 1)
                            {
                                filtered = replaceDefinitions.ToList();
                            }
                            else
                            {
                                var topPriority = replaceDefinitions.OrderByDescending(p => p.CustomPriorityOrder).FirstOrDefault().CustomPriorityOrder;
                                filtered = replaceDefinitions.Where(p => p.CustomPriorityOrder == topPriority);
                            }
                        }
                        else
                        {
                            if (definitions.GroupBy(p => p.CustomPriorityOrder).Count() == 1)
                            {
                                filtered = definitions.ToList();
                            }
                            else
                            {
                                var topPriority = definitions.OrderByDescending(p => p.CustomPriorityOrder).FirstOrDefault().CustomPriorityOrder;
                                filtered = definitions.Where(p => p.CustomPriorityOrder == topPriority);
                            }
                        }
                        var uniqueDefinitions = filtered.GroupBy(p => p.ModName).Select(p => p.OrderBy(f => Path.GetFileNameWithoutExtension(f.File), StringComparer.Ordinal).Last());
                        if (uniqueDefinitions.Count() == 1)
                        {
                            var definition = uniqueDefinitions.FirstOrDefault(p => !p.IsFromGame);
                            if (definition == null)
                            {
                                definition = uniqueDefinitions.FirstOrDefault();
                            }
                            result.Definition = definition;
                            result.FileName = definition.File;
                        }
                        else if (uniqueDefinitions.Count() > 1)
                        {
                            var modDefinitions = uniqueDefinitions.Where(p => !p.IsFromGame);
                            if (!modDefinitions.Any())
                            {
                                definitions = uniqueDefinitions;
                            }
                            var definition = modDefinitions.OrderBy(p => Path.GetFileNameWithoutExtension(p.File), StringComparer.Ordinal).Last();
                            result.Definition = definition;
                            result.FileName = definition.File;
                        }
                    }
                    else
                    {
                        var validDefinitions = definitions.Where(p => p.ExistsInLastFile).ToList();
                        if (validDefinitions.Any(d => !string.IsNullOrWhiteSpace(d.VirtualPath)))
                        {
                            validDefinitions = definitions.Where(d => !string.IsNullOrWhiteSpace(d.VirtualPath)).ToList();
                        }
                        if (validDefinitions.Count == 1)
                        {
                            result.Definition = validDefinitions.FirstOrDefault();
                            // If it's the only valid one assume load order is responsible
                            result.PriorityType = DefinitionPriorityType.ModOrder;
                            result.FileName = validDefinitions.FirstOrDefault().File;
                        }
                        else if (validDefinitions.Count > 1)
                        {
                            var definitionEvals = new List<DefinitionEval>();
                            bool isFios = false;
                            if (provider != null)
                            {
                                bool overrideSkipped = false;
                                isFios = forceFios || provider.DefinitionUsesFIOSRules(validDefinitions.First());
                                foreach (var item in validDefinitions)
                                {
                                    var fileName = isFios ? item.AdditionalFileNames.OrderBy(p => Path.GetFileNameWithoutExtension(p), StringComparer.Ordinal).First() : item.AdditionalFileNames.OrderBy(p => Path.GetFileNameWithoutExtension(p), StringComparer.Ordinal).Last();
                                    var hasOverrides = validDefinitions.Any(p => !p.IsCustomPatch && p.Dependencies != null && p.Dependencies.Any(d => d.Equals(item.ModName)) &&
                                                        (isFios ? p.AdditionalFileNames.OrderBy(p => Path.GetFileNameWithoutExtension(p), StringComparer.Ordinal).First().Equals(fileName) : p.AdditionalFileNames.OrderBy(p => Path.GetFileNameWithoutExtension(p), StringComparer.Ordinal).Last().Equals(fileName)));
                                    if (hasOverrides)
                                    {
                                        overrideSkipped = true;
                                        continue;
                                    }
                                    definitionEvals.Add(new DefinitionEval()
                                    {
                                        Definition = item,
                                        FileName = fileName
                                    });
                                }
                                List<DefinitionEval> uniqueDefinitions;
                                if (isFios)
                                {
                                    uniqueDefinitions = definitionEvals.GroupBy(p => p.Definition.ModName).Select(p => p.OrderBy(f => Path.GetFileNameWithoutExtension(f.FileName), StringComparer.Ordinal).First()).ToList();
                                }
                                else
                                {
                                    uniqueDefinitions = definitionEvals.GroupBy(p => p.Definition.ModName).Select(p => p.OrderBy(f => Path.GetFileNameWithoutExtension(f.FileName), StringComparer.Ordinal).Last()).ToList();
                                }
                                // Filter out game definitions which might have the same filename
                                var filteredGameDefinitions = false;
                                var gameDefinitions = uniqueDefinitions.GroupBy(p => p.FileNameCI).Where(p => p.Any(a => a.Definition.IsFromGame)).SelectMany(p => p.Where(w => w.Definition.IsFromGame));
                                if (gameDefinitions.Any())
                                {
                                    filteredGameDefinitions = true;
                                    foreach (var gameDef in gameDefinitions)
                                    {
                                        uniqueDefinitions.Remove(gameDef);
                                    }
                                }
                                if (uniqueDefinitions.Count == 1 && (overrideSkipped || filteredGameDefinitions))
                                {
                                    var definition = definitionEvals.FirstOrDefault(p => !p.Definition.IsFromGame);
                                    if (definition == null)
                                    {
                                        definition = definitionEvals.FirstOrDefault();
                                    }
                                    result.Definition = definition.Definition;
                                    result.FileName = definition.FileName;
                                    if (overrideSkipped)
                                    {
                                        result.PriorityType = DefinitionPriorityType.ModOverride;
                                    }
                                    else if (filteredGameDefinitions)
                                    {
                                        result.PriorityType = DefinitionPriorityType.ModOrder;
                                    }
                                }
                                else if (uniqueDefinitions.Count > 1)
                                {
                                    // Has same filenames?
                                    if (uniqueDefinitions.GroupBy(p => p.FileNameCI).Count() == 1)
                                    {
                                        if (uniqueDefinitions.Any(p => p.Definition.IsCustomPatch))
                                        {
                                            var definition = uniqueDefinitions.FirstOrDefault(p => p.Definition.IsCustomPatch);
                                            result.Definition = definition.Definition;
                                            result.FileName = definition.FileName;
                                            result.PriorityType = DefinitionPriorityType.ModOrder;
                                        }
                                        else
                                        {
                                            var definition = uniqueDefinitions.Last();
                                            result.Definition = definition.Definition;
                                            result.FileName = definition.FileName;
                                            result.PriorityType = DefinitionPriorityType.ModOrder;
                                        }
                                    }
                                    else
                                    {
                                        // Using FIOS or LIOS?
                                        if (isFios)
                                        {
                                            var definition = uniqueDefinitions.OrderBy(p => Path.GetFileNameWithoutExtension(p.FileName), StringComparer.Ordinal).First();
                                            result.Definition = definition.Definition;
                                            result.FileName = definition.FileName;
                                            result.PriorityType = DefinitionPriorityType.FIOS;
                                        }
                                        else
                                        {
                                            var definition = uniqueDefinitions.OrderBy(p => Path.GetFileNameWithoutExtension(p.FileName), StringComparer.Ordinal).Last();
                                            result.Definition = definition.Definition;
                                            result.FileName = definition.FileName;
                                            result.PriorityType = DefinitionPriorityType.LIOS;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (result.Definition == null)
            {
                var definition = definitions?.FirstOrDefault(p => !p.IsFromGame);
                if (definition == null && (definitions?.Any()).GetValueOrDefault())
                {
                    definition = definitions.FirstOrDefault();
                }
                result.Definition = definition;
                result.FileName = definition?.File;
                if (noProvider)
                {
                    result.PriorityType = DefinitionPriorityType.NoProvider;
                }
            }
            return result;
        }

        /// <summary>
        /// Evaluates the patch name path.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="patchName">Name of the patch.</param>
        /// <param name="modDirectoryRootPath">The mod directory root path.</param>
        /// <returns>System.String.</returns>
        protected virtual string EvaluatePatchNamePath(IGame game, string patchName, string modDirectoryRootPath = Shared.Constants.EmptyParam)
        {
            if (string.IsNullOrWhiteSpace(modDirectoryRootPath))
            {
                modDirectoryRootPath = GetModDirectoryRootPath(game);
            }
            modDirectoryRootPath = modDirectoryRootPath.StandardizeDirectorySeparator();
            var patchNamePath = GetPatchModDirectory(game, patchName).StandardizeDirectorySeparator();
            if (Path.GetDirectoryName(patchNamePath).Equals(modDirectoryRootPath))
            {
                patchNamePath = patchName;
            }
            return patchNamePath;
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
            mod.DescriptorFile = $"{Shared.Constants.ModDirectory}/{patchName}{Shared.Constants.ModExtension}";
            mod.FileName = GetPatchModDirectory(game, patchName).Replace("\\", "/");
            mod.Name = patchName;
            mod.Source = ModSource.Local;
            var version = GameService.GetVersion(game);
            if (!string.IsNullOrWhiteSpace(version))
            {
                mod.Version = version;
            }
            else
            {
                mod.Version = allMods.OrderByDescending(p => p.VersionData).FirstOrDefault() != null ? allMods.OrderByDescending(p => p.VersionData).FirstOrDefault().Version : string.Empty;
            }
            mod.Tags = new List<string>() { "Fixes" };
            mod.IsValid = true;
            mod.FullPath = mod.FileName.StandardizeDirectorySeparator();
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
            if (collections.Any())
            {
                return collections.OrderBy(p => p.Name);
            }
            return new List<IModCollection>();
        }

        /// <summary>
        /// Gets the collection mods.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>IEnumerable&lt;IMod&gt;.</returns>
        protected virtual IEnumerable<IMod> GetCollectionMods(IEnumerable<IMod> mods = null, string collectionName = Shared.Constants.EmptyParam)
        {
            if (mods == null)
            {
                mods = GetInstalledModsInternal(GameService.GetSelected(), false);
            }
            var collectionMods = new List<IMod>();
            var collections = GetAllModCollectionsInternal();
            if (collections?.Count() > 0)
            {
                IModCollection collection;
                if (!string.IsNullOrWhiteSpace(collectionName))
                {
                    collection = collections.FirstOrDefault(p => p.Name.Equals(collectionName, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    collection = collections.FirstOrDefault(p => p.IsSelected);
                }

                if (collection != null)
                {
                    foreach (var item in collection.Mods)
                    {
                        var mod = mods.FirstOrDefault(p => p.DescriptorFile.Equals(item, StringComparison.OrdinalIgnoreCase));
                        if (mod != null)
                        {
                            collectionMods.Add(mod);
                        }
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
        protected virtual IEnumerable<IMod> GetInstalledModsInternal(string game, bool ignorePatchMods)
        {
            return GetInstalledModsInternal(GameService.Get().FirstOrDefault(p => p.Type.Equals(game)), ignorePatchMods);
        }

        /// <summary>
        /// Gets the installed mods internal.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="ignorePatchMods">if set to <c>true</c> [ignore patch mods].</param>
        /// <returns>IEnumerable&lt;IMod&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">game</exception>
        protected virtual IEnumerable<IMod> GetInstalledModsInternal(IGame game, bool ignorePatchMods)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }
            var mods = Cache.Get<IEnumerable<IMod>>(new CacheGetParameters() { Region = ModsCacheRegion, Prefix = game.Type, Key = GetModsCacheKey(ignorePatchMods) });
            if (mods != null)
            {
                return mods;
            }
            else
            {
                var result = new List<IMod>();
                var installedMods = Reader.Read(Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory));
                if (installedMods?.Count() > 0)
                {
                    foreach (var installedMod in installedMods.Where(p => p.Content.Any()))
                    {
                        var mod = Mapper.Map<IMod>(ModParser.Parse(installedMod.Content));
                        if (ignorePatchMods && IsPatchModInternal(mod))
                        {
                            continue;
                        }
                        mod.Name = string.IsNullOrWhiteSpace(mod.Name) ? string.Empty : mod.Name;
                        mod.Version = string.IsNullOrWhiteSpace(mod.Version) ? string.Empty : mod.Version;
                        mod.IsLocked = installedMod.IsReadOnly;
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
                                mod.FullPath = mod.FileName.StandardizeDirectorySeparator();
                            }
                            else
                            {
                                // Check user directory and workshop directory.
                                var userDirectoryMod = new List<string>() { Path.Combine(game.CustomModDirectory, mod.FileName), Path.Combine(game.UserDirectory, mod.FileName) }.GroupBy(p => p).Select(p => p.First());
                                var workshopDirectoryMod = game.WorkshopDirectory.Select(p => Path.Combine(p, mod.FileName)).GroupBy(p => p).Select(p => p.First());
                                if (userDirectoryMod.Any(p => File.Exists(p) || Directory.Exists(p)))
                                {
                                    mod.FullPath = userDirectoryMod.FirstOrDefault(p => File.Exists(p) || Directory.Exists(p)).StandardizeDirectorySeparator();
                                }
                                else if (workshopDirectoryMod.Any(p => File.Exists(p) || Directory.Exists(p)))
                                {
                                    mod.FullPath = workshopDirectoryMod.FirstOrDefault(p => File.Exists(p) || Directory.Exists(p)).StandardizeDirectorySeparator();
                                }
                            }
                        }
                        // Validate if path exists
                        mod.IsValid = File.Exists(mod.FullPath) || Directory.Exists(mod.FullPath);
                        mod.Game = game.Type;
                        result.Add(mod);
                    }
                }
                Cache.Set(new CacheAddParameters<IEnumerable<IMod>>() { Region = ModsCacheRegion, Prefix = game.Type, Key = GetModsCacheKey(ignorePatchMods), Value = result });
                return result;
            }
        }

        /// <summary>
        /// Gets the mod directory root path.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetModDirectoryRootPath(IGame game)
        {
            return !string.IsNullOrWhiteSpace(game.CustomModDirectory) ? game.CustomModDirectory : Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory);
        }

        /// <summary>
        /// Constructs the mods cache key.
        /// </summary>
        /// <param name="regularMods">if set to <c>true</c> [regular mods].</param>
        /// <returns>System.String.</returns>
        protected virtual string GetModsCacheKey(bool regularMods)
        {
            return regularMods ? RegularModsCacheKey : AllModsCacheKey;
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
        /// Gets the patch mod directory.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="patchOrMergeName">Name of the patch or merge.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetPatchModDirectory(IGame game, string patchOrMergeName)
        {
            var path = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory, patchOrMergeName);
            path = path.StandardizeDirectorySeparator();
            var parameters = new ModWriterParameters()
            {
                Path = path
            };
            if (!ModWriter.ModDirectoryExists(parameters) && !string.IsNullOrWhiteSpace(game.CustomModDirectory))
            {
                path = Path.Combine(game.CustomModDirectory, patchOrMergeName).StandardizeDirectorySeparator();
            }
            return path;
        }

        /// <summary>
        /// Gets the patch mod directory.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="modCollection">The mod collection.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetPatchModDirectory(IGame game, IModCollection modCollection)
        {
            return GetPatchModDirectory(game, GenerateCollectionPatchName(modCollection.Name));
        }

        /// <summary>
        /// Gets the PDX mod identifier.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="isDirectory">if set to <c>true</c> [is directory].</param>
        /// <returns>System.Int32.</returns>
        protected virtual long GetPdxModId(string path, bool isDirectory = false)
        {
            var name = !isDirectory ? Path.GetFileNameWithoutExtension(path) : path;
#pragma warning disable CA1806 // Do not ignore method results
            long.TryParse(name.Replace(Constants.Paradox_mod_id, string.Empty), out var id);
#pragma warning restore CA1806 // Do not ignore method results
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
        /// Determines whether [is valid definition type] [the specified definition].
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns><c>true</c> if [is valid definition type] [the specified definition]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsValidDefinitionType(IDefinition definition)
        {
            return definition != null && definition.ValueType != ValueType.Variable &&
                definition.ValueType != ValueType.Namespace &&
                definition.ValueType != ValueType.Invalid &&
                definition.ValueType != ValueType.EmptyFile;
        }

        /// <summary>
        /// Merges the definitions.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        protected virtual void MergeDefinitions(IEnumerable<IDefinition> definitions)
        {
            static void appendLine(StringBuilder sb, IEnumerable<string> lines)
            {
                if (lines != null && lines.Any())
                {
                    sb.AppendLine(string.Join(Environment.NewLine, lines));
                }
            }
            static string mergeCode(string codeTag, string separator, IEnumerable<string> lines)
            {
                if (Shared.Constants.CodeSeparators.ClosingSeparators.Map.ContainsKey(separator))
                {
                    var closingTag = Shared.Constants.CodeSeparators.ClosingSeparators.Map[separator];
                    var sb = new StringBuilder();
                    sb.AppendLine($"{codeTag} = {separator}");
                    foreach (var item in lines)
                    {
                        var splitLines = item.SplitOnNewLine();
                        foreach (var split in splitLines)
                        {
                            sb.AppendLine($"{new string(' ', 4)}{split}");
                        }
                    }
                    sb.Append(closingTag);
                    return sb.ToString();
                }
                else
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"{codeTag}{separator}");
                    foreach (var item in lines)
                    {
                        var splitLines = item.SplitOnNewLine();
                        foreach (var split in splitLines)
                        {
                            sb.AppendLine($"{new string(' ', 4)}{split}");
                        }
                    }
                    return sb.ToString();
                }
            }

            if (definitions != null && definitions.Any())
            {
                var otherDefinitions = definitions.Where(p => IsValidDefinitionType(p));
                var variableDefinitions = definitions.Where(p => !IsValidDefinitionType(p));
                if (variableDefinitions.Any())
                {
                    foreach (var definition in otherDefinitions)
                    {
                        var originalCode = definition.OriginalCode.ReplaceTabs().ReplaceNewLine().Split(" ", StringSplitOptions.RemoveEmptyEntries);
                        var namespaces = variableDefinitions.Where(p => p.ValueType == ValueType.Namespace);
                        var variables = variableDefinitions.Where(p => originalCode.Contains(p.Id));
                        var allVars = namespaces.Concat(variables);
                        if (allVars.Any())
                        {
                            definition.Variables = allVars.ToList();
                        }
                        if (string.IsNullOrWhiteSpace(definition.CodeTag))
                        {
                            StringBuilder sb = new StringBuilder();
                            appendLine(sb, namespaces.Select(p => p.Code));
                            appendLine(sb, variables.Select(p => p.Code));
                            appendLine(sb, new List<string> { definition.Code });
                            definition.Code = sb.ToString();
                        }
                        else
                        {
                            definition.Code = mergeCode(definition.CodeTag, definition.CodeSeparator, namespaces.Select(p => p.OriginalCode).Concat(variables.Select(p => p.OriginalCode)).Concat(new List<string>() { definition.OriginalCode }));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// populate mod files internal as an asynchronous operation.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual async Task<bool> PopulateModFilesInternalAsync(IEnumerable<IMod> mods)
        {
            var logger = DIResolver.Get<ILogger>();
            if (mods?.Count() > 0)
            {
                var tasks = new List<Task>();
                foreach (var mod in mods)
                {
                    if (mod.IsValid)
                    {
                        var task = Task.Run(() =>
                        {
                            var localMod = mod;
                            try
                            {
                                var files = Reader.GetFiles(mod.FullPath);
                                localMod.Files = files ?? new List<string>();
                            }
                            catch (Exception ex)
                            {
                                mod.Files = new List<string>();
                                logger.Error(ex);
                            }
                        });
                        tasks.Add(task);
                    }
                    else
                    {
                        mod.Files = new List<string>();
                    }
                }
                await Task.WhenAll(tasks);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Populates the mod path.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="collectionMods">The collection mods.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> PopulateModPath(IDefinition definition, IEnumerable<IMod> collectionMods)
        {
            return PopulateModPath(new List<IDefinition>() { definition }, collectionMods);
        }

        /// <summary>
        /// Populates the mod path.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="collectionMods">The collection mods.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> PopulateModPath(IEnumerable<IDefinition> definitions, IEnumerable<IMod> collectionMods)
        {
            if (definitions != null && definitions.Any())
            {
                foreach (var item in definitions)
                {
                    if (IsPatchModInternal(item.ModName))
                    {
                        item.ModPath = GetPatchModDirectory(GameService.GetSelected(), item.ModName);
                    }
                    else if (item.IsFromGame)
                    {
                        item.ModPath = pathResolver.GetPath(GameService.GetSelected());
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
