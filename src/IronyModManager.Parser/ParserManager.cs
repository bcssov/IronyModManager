// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-19-2020
//
// Last Modified By : Mario
// Last Modified On : 04-25-2020
// ***********************************************************************
// <copyright file="ParserManager.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Parsers;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Class ParserManager.
    /// Implements the <see cref="IronyModManager.Parser.Common.IParserManager" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.IParserManager" />
    public class ParserManager : IParserManager
    {
        #region Fields

        /// <summary>
        /// All parsers
        /// </summary>
        private readonly List<IDefaultParser> allParsers;

        /// <summary>
        /// The default parsers
        /// </summary>
        private readonly IEnumerable<IDefaultParser> defaultParsers;

        /// <summary>
        /// The game parsers
        /// </summary>
        private readonly IEnumerable<IGameParser> gameParsers;

        /// <summary>
        /// The generic parsers
        /// </summary>
        private readonly IEnumerable<IGenericParser> genericParsers;

        /// <summary>
        /// The parser maps
        /// </summary>
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, List<IParserMap>>> parserMaps;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserManager" /> class.
        /// </summary>
        /// <param name="gameParsers">The game parsers.</param>
        /// <param name="genericParsers">The generic parsers.</param>
        /// <param name="defaultParsers">The default parsers.</param>
        public ParserManager(IEnumerable<IGameParser> gameParsers, IEnumerable<IGenericParser> genericParsers, IEnumerable<IDefaultParser> defaultParsers)
        {
            parserMaps = new ConcurrentDictionary<string, ConcurrentDictionary<string, List<IParserMap>>>();
            this.gameParsers = gameParsers.OrderBy(p => p.Priority);
            this.genericParsers = genericParsers.OrderBy(p => p.Priority);
            this.defaultParsers = defaultParsers;
            allParsers = new List<IDefaultParser>(this.defaultParsers);
            allParsers.AddRange(this.gameParsers);
            allParsers.AddRange(this.genericParsers);
            ValidateParserNames(gameParsers);
            ValidateParserNames(genericParsers);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IIndexedDefinitions.</returns>
        public IEnumerable<IDefinition> Parse(ParserManagerArgs args)
        {
            var canParseArgs = new CanParseArgs()
            {
                File = args.File,
                GameType = args.GameType,
                Lines = args.Lines ?? new List<string>()
            };
            var parseArgs = new ParserArgs()
            {
                ContentSHA = args.ContentSHA,
                ModDependencies = args.ModDependencies,
                File = args.File,
                Lines = args.Lines ?? new List<string>(),
                ModName = args.ModName
            };
            var preferredParserNames = GetPreferredParsers(args.GameType, Path.GetDirectoryName(args.File));
            IDefaultParser preferredParser = null;
            if (preferredParserNames?.Count() > 0)
            {
                var gameParser = gameParsers.Where(p => preferredParserNames.Any(s => s.Equals(p.ParserName)));
                if (gameParsers.Count() > 0)
                {
                    preferredParser = gameParsers.FirstOrDefault(p => p.CanParse(canParseArgs));
                }
                var genericParser = genericParsers.Where(p => preferredParserNames.Any(s => s.Equals(p.ParserName)));
                if (preferredParser == null && genericParser.Count() > 0)
                {
                    preferredParser = genericParsers.FirstOrDefault(p => p.CanParse(canParseArgs));
                }
                var defaultParser = defaultParsers.Where(p => preferredParserNames.Any(s => s.Equals(p.ParserName)));
                if (preferredParser == null && defaultParsers.Count() > 0)
                {
                    preferredParser = defaultParser.FirstOrDefault(p => p.CanParse(canParseArgs));
                }
            }
            IEnumerable<IDefinition> result = null;
            // This will be auto generated when a game is scanned for the first time. It was rushed and is now generated via unit test and is no where near as completed.
            if (preferredParser != null)
            {
                result = preferredParser.Parse(parseArgs);
                SetParser(result, preferredParser.ParserName);
            }
            else
            {
                var gameParser = gameParsers.FirstOrDefault(p => p.CanParse(canParseArgs));
                if (gameParser != null)
                {
                    result = gameParser.Parse(parseArgs);
                    SetParser(result, gameParser.ParserName);
                }
                else
                {
                    var genericParser = genericParsers.FirstOrDefault(p => p.CanParse(canParseArgs));
                    if (genericParser != null)
                    {
                        result = genericParser.Parse(parseArgs);
                        SetParser(result, genericParser.ParserName);
                    }
                    else
                    {
                        var parser = defaultParsers.FirstOrDefault(p => p.CanParse(canParseArgs));
                        result = parser.Parse(parseArgs);
                        SetParser(result, parser.ParserName);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the preferred parser.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="location">The location.</param>
        /// <returns>System.String.</returns>
        private IEnumerable<string> GetPreferredParsers(string game, string location)
        {
            location = location.ToLowerInvariant();
            IEnumerable<string> parser = null;
            if (parserMaps.TryGetValue(game, out var maps))
            {
                if (maps.TryGetValue(location, out var value))
                {
                    parser = value.Select(p => p.PreferredParser);
                }
            }
            else
            {
                var path = string.Format(Constants.ParserMapPath, game);
                if (File.Exists(path))
                {
                    var content = File.ReadAllText(path);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        var cachedMap = JsonDISerializer.Deserialize<List<IParserMap>>(content);
                        var grouped = cachedMap.GroupBy(p => p.DirectoryPath);
                        var newMaps = new ConcurrentDictionary<string, List<IParserMap>>();
                        foreach (var item in grouped)
                        {
                            var id = item.First().DirectoryPath.ToLowerInvariant();
                            newMaps.TryAdd(id, item.Select(p => p).ToList());
                            if (id.Equals(location))
                            {
                                parser = item.Select(p => p.PreferredParser);
                            }
                        }
                        parserMaps.TryAdd(game, newMaps);
                    }
                }
            }
            return parser;
        }

        /// <summary>
        /// Sets the parser.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="parserName">Name of the parser.</param>
        private void SetParser(IEnumerable<IDefinition> definitions, string parserName)
        {
            if (definitions?.Count() > 0)
            {
                foreach (var item in definitions)
                {
                    item.UsedParser = parserName;
                }
            }
        }

        /// <summary>
        /// Validates the parser names.
        /// </summary>
        /// <param name="parsers">The parsers.</param>
        /// <exception cref="ArgumentOutOfRangeException">Duplicate parsers detected: {message}</exception>
        private void ValidateParserNames(IEnumerable<IDefaultParser> parsers)
        {
            var invalid = parsers.GroupBy(p => p.ParserName).Where(s => s.Count() > 1);
            if (invalid.Count() > 0)
            {
                var message = string.Join(',', invalid.SelectMany(s => s).Select(s => s.ParserName));
                throw new ArgumentOutOfRangeException($"Duplicate parsers detected: {message}");
            }
        }

        #endregion Methods
    }
}
