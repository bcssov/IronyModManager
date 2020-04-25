// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 04-25-2020
//
// Last Modified By : Mario
// Last Modified On : 04-25-2020
// ***********************************************************************
// <copyright file="SimpleKeyParser.cs" company="Mario">
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

namespace IronyModManager.Parser.Generic
{
    /// <summary>
    /// Class SimpleKeyParser.
    /// Implements the <see cref="IronyModManager.Parser.Generic.KeyParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Generic.KeyParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    public class SimpleKeyParser : KeyParser, IGenericParser
    {
        #region Fields

        /// <summary>
        /// The event identifier
        /// </summary>
        private string key = string.Empty;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleKeyParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        public SimpleKeyParser(ICodeParser codeParser) : base(codeParser)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Generic" + nameof(SimpleKeyParser);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return HasPassedComplexThreshold(args.Lines) && EvalContainsKeyElements(args);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var errors = EvalSimpleParseForErrorsOnly(args);
            if (errors != null)
            {
                return errors;
            }
            return ParseSimple(args);
        }

        /// <summary>
        /// Finalizes the object definition.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IDefinition.</returns>
        protected override IDefinition FinalizeSimpleParseObjectDefinition(ParsingArgs args)
        {
            var result = base.FinalizeSimpleParseObjectDefinition(args);
            if (!string.IsNullOrWhiteSpace(key))
            {
                result.Id = key;
                key = string.Empty;
            }
            return result;
        }

        /// <summary>
        /// Called when [read object line].
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected override void OnSimpleParseReadObjectLine(ParsingArgs args)
        {
            var cleaned = codeParser.CleanWhitespace(args.Line);
            if (args.OpeningBracket - args.ClosingBracket <= 1 && Common.Constants.Scripts.GenericKeyIds.Any(s => !string.IsNullOrWhiteSpace(codeParser.GetValue(cleaned, s))))
            {
                string sep = string.Empty;
                var bracketLocation = cleaned.IndexOf(Common.Constants.Scripts.OpeningBracket.ToString());
                int idLoc = -1;
                foreach (var item in Common.Constants.Scripts.GenericKeyIds)
                {
                    idLoc = cleaned.IndexOf(item);
                    if (idLoc > -1)
                    {
                        sep = item;
                        break;
                    }
                }
                if (idLoc < bracketLocation || bracketLocation == -1 || (args.Inline && cleaned.Substring(0, idLoc).Count(s => s == Common.Constants.Scripts.OpeningBracket) == 1))
                {
                    key = codeParser.GetValue(cleaned, sep);
                }
            }
            base.OnSimpleParseReadObjectLine(args);
        }

        #endregion Methods
    }
}
