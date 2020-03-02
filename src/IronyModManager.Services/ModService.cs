// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 03-02-2020
// ***********************************************************************
// <copyright file="ModService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using IronyModManager.DI;
using IronyModManager.IO.Common;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Services.Common;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class ModService.
    /// Implements the <see cref="IronyModManager.Services.BaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.IModService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IModService" />
    public class ModService : BaseService, IModService
    {
        #region Fields

        /// <summary>
        /// The parser manager
        /// </summary>
        private readonly IParserManager parserManager;

        /// <summary>
        /// The reader
        /// </summary>
        private readonly IReader reader;

        /// <summary>
        /// The mod parser
        /// </summary>
        private IModParser modParser;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModService" /> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="parserManager">The parser manager.</param>
        /// <param name="modParser">The mod parser.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ModService(IReader reader, IParserManager parserManager, IModParser modParser, IStorageProvider storageProvider, IMapper mapper) : base(storageProvider, mapper)
        {
            this.reader = reader;
            this.parserManager = parserManager;
            this.modParser = modParser;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Builds the mod URL.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns>System.String.</returns>
        public string BuildModUrl(IMod mod)
        {
            switch (mod.Source)
            {
                case ModSource.Steam:
                    return string.Format(Constants.Steam_Url, mod.RemoteId);

                case ModSource.Paradox:
                    return string.Format(Constants.Paradox_Url, mod.RemoteId);

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the installed mods.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>IEnumerable&lt;IModObject&gt;.</returns>
        /// <exception cref="ArgumentNullException">game</exception>
        public virtual IEnumerable<IMod> GetInstalledMods(IGame game)
        {
            if (game == null)
            {
                throw new ArgumentNullException("game");
            }
            var result = new List<IMod>();
            var installedMods = reader.Read(Path.Combine(game.UserDirectory, Constants.ModDirectory));
            if (installedMods?.Count() > 0)
            {
                foreach (var installedMod in installedMods)
                {
                    var mod = Mapper.Map<IMod>(modParser.Parse(installedMod.Content));
                    mod.Source = GetModSource(installedMod);
                    if (mod.Source == ModSource.Paradox)
                    {
                        mod.RemoteId = GetPdxModId(installedMod.FileName);
                    }
                    result.Add(mod);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the mod objects.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="mods">The mods.</param>
        /// <returns>IIndexedDefinitions.</returns>
        public virtual IIndexedDefinitions GetModObjects(IGame game, IEnumerable<IMod> mods)
        {
            if (game == null || mods == null || mods.Count() == 0)
            {
                return null;
            }
            var definitions = new ConcurrentBag<IDefinition>();

            mods.AsParallel().ForAll((m) =>
            {
                IEnumerable<IDefinition> result = null;
                if (Path.IsPathFullyQualified(m.FileName))
                {
                    result = ParseModFiles(game, reader.Read(m.FileName), m);
                }
                else
                {
                    // Check user directory and workshop directory.
                    // Technically we don't need this since newer pdx mod launchers use absolute paths.
                    // IronyModManager will always require that a user runs the PDX mod launcher first when new mods are installed.
                    // This program will not be a replacement for mod installation only for mod management.
                    var userDirectoryMod = Path.Combine(game.UserDirectory, m.FileName);
                    var workshopDirectoryMod = Path.Combine(game.WorkshopDirectory, m.FileName);
                    if (File.Exists(userDirectoryMod) || Directory.Exists(userDirectoryMod))
                    {
                        result = ParseModFiles(game, reader.Read(userDirectoryMod), m);
                    }
                    else if (File.Exists(workshopDirectoryMod) || Directory.Exists(workshopDirectoryMod))
                    {
                        result = ParseModFiles(game, reader.Read(workshopDirectoryMod), m);
                    }
                }
                if (result?.Count() > 0)
                {
                    foreach (var item in result)
                    {
                        definitions.Add(item);
                    }
                }
            });

            var indexed = DIResolver.Get<IIndexedDefinitions>();
            indexed.InitMap(definitions);
            return indexed;
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
        /// <param name="filename">The filename.</param>
        /// <returns>System.Int32.</returns>
        protected virtual int GetPdxModId(string filename)
        {
            var name = Path.GetFileNameWithoutExtension(filename);
            int.TryParse(name.Replace(Constants.Paradox_mod_id, string.Empty), out var id);
            return id;
        }

        /// <summary>
        /// Parses the mod files.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="fileInfos">The file infos.</param>
        /// <param name="modObject">The mod object.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> ParseModFiles(IGame game, IEnumerable<IFileInfo> fileInfos, IModObject modObject)
        {
            var definitions = new List<IDefinition>();
            foreach (var fileInfo in fileInfos)
            {
                definitions.AddRange(parserManager.Parse(new ParserManagerArgs()
                {
                    ContentSHA = fileInfo.ContentSHA,
                    File = fileInfo.FileName,
                    GameType = game.Type,
                    Lines = fileInfo.Content,
                    ModDependencies = modObject.Dependencies,
                    ModName = modObject.Name
                }));
            }
            return definitions;
        }

        #endregion Methods
    }
}
