// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 04-18-2020
// ***********************************************************************
// <copyright file="FlagsParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Parsers;

namespace IronyModManager.Parser.Games.Stellaris
{
    /// <summary>
    /// Class FlagsParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    public class FlagsParser : BaseParser, IGameParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlagsParser" /> class.
        /// </summary>
        /// <param name="textParser">The text parser.</param>
        public FlagsParser(ITextParser textParser) : base(textParser)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Stellaris" + nameof(FlagsParser);

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
        public bool CanParse(CanParseArgs args)
        {
            return args.IsStellaris() && args.File.StartsWith(Common.Constants.Stellaris.Flags, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var def = GetDefinitionInstance();
            var parsingArgs = ConstructArgs(args, def);
            MapDefinitionFromArgs(parsingArgs);
            def.Code = args.Lines != null ? string.Join(Environment.NewLine, args.Lines) : string.Empty;
            def.Id = args.File.Split(Common.Constants.Scripts.PathTrimParameters, StringSplitOptions.RemoveEmptyEntries).Last();
            def.ValueType = !Shared.Constants.TextExtensions.Any(s => args.File.EndsWith(s, StringComparison.OrdinalIgnoreCase)) ? Common.ValueType.Binary : Common.ValueType.WholeTextFile;
            def.Type = FormatType(args.File, def.ValueType == Common.ValueType.Binary ? Common.Constants.BinaryType : Common.Constants.TxtType);
            return new List<IDefinition> { def };
        }

        #endregion Methods
    }
}
