// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-19-2020
//
// Last Modified By : Mario
// Last Modified On : 10-30-2021
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
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Shared.Models;
using ValueType = IronyModManager.Shared.Models.ValueType;

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
            static bool isValidLine(string line)
            {
                string text = line ?? string.Empty;
                return !string.IsNullOrWhiteSpace(text) && !text.Trim().StartsWith(Constants.Scripts.ScriptCommentId);
            }
            // Check if empty text file
            if (Shared.Constants.TextExtensions.Any(p => args.File.EndsWith(p, StringComparison.OrdinalIgnoreCase)) &&
                (args.Lines == null || !args.Lines.Any() || !args.Lines.Any(p => isValidLine(p))))
            {
                var definition = DIResolver.Get<IDefinition>();
                definition.OriginalCode = definition.Code = Constants.EmptyOverwriteComment;
                definition.CodeSeparator = definition.CodeTag = string.Empty;
                definition.ContentSHA = args.ContentSHA;
                definition.Dependencies = args.ModDependencies;
                definition.File = args.File;
                definition.Id = Path.GetFileName(args.File).ToLowerInvariant();
                definition.Type = GetType(args.File);
                definition.ModName = args.ModName;
                definition.OriginalModName = args.ModName;
                definition.OriginalFileName = args.File;
                definition.UsedParser = string.Empty;
                definition.ValueType = ValueType.EmptyFile;
                return new List<IDefinition>() { definition };
            }
            return InvokeParsers(args);
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetType(string file)
        {
            var formatted = Path.GetDirectoryName(file);
            var type = Path.GetExtension(file).Trim('.');
            if (!Shared.Constants.TextExtensions.Any(s => s.EndsWith(type, StringComparison.OrdinalIgnoreCase)))
            {
                type = Constants.TxtType;
            }
            return $"{formatted.ToLowerInvariant()}{Path.DirectorySeparatorChar}{type}";
        }

        /// <summary>
        /// Sets the parser.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="parserName">Name of the parser.</param>
        private static void SetAdditionalData(IEnumerable<IDefinition> definitions, string parserName)
        {
            if (definitions?.Count() > 0)
            {
                int order = 0;
                foreach (var item in definitions)
                {
                    if (item.ValueType != ValueType.Variable && item.ValueType != ValueType.Namespace)
                    {
                        order++;
                        item.Order = order;
                    }
                    item.UsedParser = parserName;
                }
            }
        }

        /// <summary>
        /// Validates the parser names.
        /// </summary>
        /// <param name="parsers">The parsers.</param>
        /// <exception cref="ArgumentOutOfRangeException">Duplicate parsers detected: {message}</exception>
        private static void ValidateParserNames(IEnumerable<IDefaultParser> parsers)
        {
            var invalid = parsers.GroupBy(p => p.ParserName).Where(s => s.Count() > 1);
            if (invalid.Any())
            {
                var message = string.Join(',', invalid.SelectMany(s => s).Select(s => s.ParserName));
                throw new ArgumentOutOfRangeException($"Duplicate parsers detected: {message}");
            }
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
        /// Invokes the parsers.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        private IEnumerable<IDefinition> InvokeParsers(ParserManagerArgs args)
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
                if (gameParsers.Any())
                {
                    preferredParser = gameParsers.FirstOrDefault(p => p.CanParse(canParseArgs));
                }
                var genericParser = genericParsers.Where(p => preferredParserNames.Any(s => s.Equals(p.ParserName)));
                if (preferredParser == null && genericParser.Any())
                {
                    preferredParser = genericParsers.FirstOrDefault(p => p.CanParse(canParseArgs));
                }
                var defaultParser = defaultParsers.Where(p => preferredParserNames.Any(s => s.Equals(p.ParserName)));
                if (preferredParser == null && defaultParsers.Any())
                {
                    preferredParser = defaultParser.FirstOrDefault(p => p.CanParse(canParseArgs));
                }
            }
            IEnumerable<IDefinition> result = null;
            // This will be auto generated when a game is scanned for the first time. It was rushed and is now generated via unit test and is no where near as completed.
            if (preferredParser != null)
            {
                result = preferredParser.Parse(parseArgs);
                SetAdditionalData(result, preferredParser.ParserName);
            }
            else
            {
                var gameParser = gameParsers.FirstOrDefault(p => p.CanParse(canParseArgs));
                if (gameParser != null)
                {
                    result = gameParser.Parse(parseArgs);
                    SetAdditionalData(result, gameParser.ParserName);
                }
                else
                {
                    var genericParser = genericParsers.FirstOrDefault(p => p.CanParse(canParseArgs));
                    if (genericParser != null)
                    {
                        result = genericParser.Parse(parseArgs);
                        SetAdditionalData(result, genericParser.ParserName);
                    }
                    else
                    {
                        var parser = defaultParsers.FirstOrDefault(p => p.CanParse(canParseArgs));
                        result = parser.Parse(parseArgs);
                        SetAdditionalData(result, parser.ParserName);
                    }
                }
            }
            return result;
        }

        #endregion Methods
    }
}
