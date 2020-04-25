// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 04-25-2020
// ***********************************************************************
// <copyright file="FlagsParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
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
        public FlagsParser(ICodeParser textParser) : base(textParser)
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
            var valType = !Shared.Constants.TextExtensions.Any(s => args.File.EndsWith(s, StringComparison.OrdinalIgnoreCase)) ? Common.ValueType.Binary : Common.ValueType.WholeTextFile;
            if (valType == Common.ValueType.WholeTextFile && args.Lines.Count() < MaxLines)
            {
                var errors = EvalForErrorsOnly(args);
                if (errors != null)
                {
                    return errors;
                }
            }

            var def = GetDefinitionInstance();
            def.Code = args.Lines != null ? string.Join(Environment.NewLine, args.Lines) : string.Empty;
            def.Id = Path.GetFileName(args.File);
            def.ValueType = valType;
            MapDefinitionFromArgs(def, args, def.ValueType == Common.ValueType.Binary ? Common.Constants.BinaryType : Common.Constants.TxtType);
            return new List<IDefinition> { def };
        }

        #endregion Methods
    }
}
