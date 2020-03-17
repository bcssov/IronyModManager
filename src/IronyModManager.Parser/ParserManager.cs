// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-19-2020
//
// Last Modified By : Mario
// Last Modified On : 03-17-2020
// ***********************************************************************
// <copyright file="ParserManager.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// The default parser
        /// </summary>
        private readonly IDefaultParser defaultParser;

        /// <summary>
        /// The game parsers
        /// </summary>
        private readonly IEnumerable<IGameParser> gameParsers;

        /// <summary>
        /// The generic parsers
        /// </summary>
        private readonly IEnumerable<IGenericParser> genericParsers;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserManager" /> class.
        /// </summary>
        /// <param name="gameParsers">The game parsers.</param>
        /// <param name="genericParsers">The generic parsers.</param>
        /// <param name="defaultParser">The default parser.</param>
        public ParserManager(IEnumerable<IGameParser> gameParsers, IEnumerable<IGenericParser> genericParsers, IDefaultParser defaultParser)
        {
            this.gameParsers = gameParsers;
            this.genericParsers = genericParsers;
            this.defaultParser = defaultParser;
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
            IEnumerable<IDefinition> result = null;
            var gameParser = gameParsers.FirstOrDefault(p => p.CanParse(canParseArgs));
            if (gameParser != null)
            {
                result = gameParser.Parse(parseArgs);
            }
            else
            {
                var genericParser = genericParsers.FirstOrDefault(p => p.CanParse(canParseArgs));
                if (genericParser != null)
                {
                    result = genericParser.Parse(parseArgs);
                }
                else
                {
                    result = defaultParser.Parse(parseArgs);
                }
            }
            return result;
        }

        #endregion Methods
    }
}
