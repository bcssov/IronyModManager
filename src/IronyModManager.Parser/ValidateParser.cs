// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 09-02-2021
//
// Last Modified By : Mario
// Last Modified On : 10-17-2024
// ***********************************************************************
// <copyright file="ValidateParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Class ValidateParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IValidateParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IValidateParser" />
    public class ValidateParser : BaseParser, IValidateParser
    {
        #region Fields

        /// <summary>
        /// The skip validation for types
        /// </summary>
        private static readonly string[] skipValidationForTypes = { Common.Constants.ShaderExtension, Common.Constants.FxhExtension };

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public ValidateParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        /// <exception cref="System.NotSupportedException"></exception>
        public override string ParserName => throw new NotSupportedException();

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public override bool CanParse(CanParseArgs args)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the bracket count.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="text">The text.</param>
        /// <returns>IBracketValidateResult.</returns>
        public IBracketValidateResult GetBracketCount(string file, string text)
        {
            var bracketCount = DIResolver.Get<IBracketValidateResult>();
            var cleanText = CleanCode(file, text);

            bracketCount.CloseBracketCount = cleanText.Count(s => s == Common.Constants.Scripts.CloseObject);
            bracketCount.OpenBracketCount = cleanText.Count(s => s == Common.Constants.Scripts.OpenObject);

            return bracketCount;
        }

        /// <summary>
        /// Determines whether the specified text has code.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="text">The text.</param>
        /// <returns><c>true</c> if the specified text has code; otherwise, <c>false</c>.</returns>
        public bool HasCode(string file, string text)
        {
            return !string.IsNullOrWhiteSpace(CleanCode(file, text));
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Validates the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public IEnumerable<IDefinition> Validate(ParserArgs args)
        {
            if (!skipValidationForTypes.Any(p => args.File.EndsWith(p, StringComparison.OrdinalIgnoreCase)))
            {
                return EvalForErrorsOnly(args);
            }

            return null;
        }

        /// <summary>
        /// Cleans the code.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        private string CleanCode(string file, string text)
        {
            return string.Join(Environment.NewLine, CodeParser.CleanCode(file, text.SplitOnNewLine()));
        }

        #endregion Methods
    }
}
