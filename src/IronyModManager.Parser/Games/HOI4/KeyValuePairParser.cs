// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 01-29-2022
//
// Last Modified By : Mario
// Last Modified On : 01-29-2022
// ***********************************************************************
// <copyright file="KeyValuePairParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;

namespace IronyModManager.Parser.Games.HOI4
{
    /// <summary>
    /// Class KeyValuePairParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    public class KeyValuePairParser : BaseParser, IGameParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValuePairParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public KeyValuePairParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "HOI4" + nameof(KeyValuePairParser);

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority => 1;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return args.IsHOI4() && EvalStartsWith(args);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var result = new List<IDefinition>();
            var parseResult = TryParse(args);
            if (parseResult.Error != null)
            {
                result.Add(TranslateScriptError(parseResult.Error, args));
            }
            else
            {
                foreach (var item in parseResult.Values)
                {
                    var definition = GetDefinitionInstance();
                    string id = EvalDefinitionId(item.Values, item.Key);
                    MapDefinitionFromArgs(ConstructArgs(args, definition));
                    definition.Id = TrimId(id);
                    definition.ValueType = Shared.Models.ValueType.Object;
                    definition.OriginalCode = definition.Code = FormatCode(item);
                    var tags = ParseScriptTags(item.Values, item.Key);
                    if (tags.Any())
                    {
                        foreach (var tag in tags)
                        {
                            var lower = tag.ToLowerInvariant();
                            if (!definition.Tags.Contains(lower))
                            {
                                definition.Tags.Add(lower);
                            }
                        }
                    }
                    result.Add(definition);
                }
            }
            return result;
        }

        /// <summary>
        /// Evals the starts with.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual bool EvalStartsWith(CanParseArgs args)
        {
            return args.File.StartsWith(Common.Constants.HOI4.CountryTags, StringComparison.OrdinalIgnoreCase);
        }

        #endregion Methods
    }
}
