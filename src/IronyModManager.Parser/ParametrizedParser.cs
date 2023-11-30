
// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 10-03-2023
//
// Last Modified By : Mario
// Last Modified On : 11-29-2023
// ***********************************************************************
// <copyright file="ParametrizedParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Shared;

namespace IronyModManager.Parser
{

    /// <summary>
    /// Class ParametrizedParser.
    /// Implements the <see cref="IParametrizedParser" />
    /// </summary>
    /// <seealso cref="IParametrizedParser" />
    public class ParametrizedParser : IParametrizedParser
    {
        #region Fields

        /// <summary>
        /// The quotes
        /// </summary>
        private const string Quotes = "\"";

        /// <summary>
        /// The script
        /// </summary>
        private const string Script = "script";

        /// <summary>
        /// The terminator
        /// </summary>
        private const char Terminator = '$'; // I'll be back
        /// <summary>
        /// The code parser
        /// </summary>
        private readonly ICodeParser codeParser;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParametrizedParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        public ParametrizedParser(ICodeParser codeParser)
        {
            this.codeParser = codeParser;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the script path.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        public string GetScriptPath(string parameters)
        {
            var elParams = codeParser.ParseScriptWithoutValidation(parameters.SplitOnNewLine(), string.Empty);
            if (elParams != null && elParams.Values != null && elParams.Error == null && elParams.Values.Count(p => p.Key.Equals(Common.Constants.Stellaris.InlineScriptId, StringComparison.OrdinalIgnoreCase)) == 1)
            {
                var elObj = elParams.Values.FirstOrDefault(p => p.Values != null);
                if (elObj != null)
                {
                    var match = elObj.Values.FirstOrDefault(p => p.Key.Equals(Script, StringComparison.OrdinalIgnoreCase));
                    if (match != null)
                    {
                        return ((match.Value ?? string.Empty).Trim(Quotes)).StandardizeDirectorySeparator();
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Processes the specified code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        public string Process(string code, string parameters)
        {
            var elParams = codeParser.ParseScriptWithoutValidation(parameters.SplitOnNewLine(), string.Empty);
            if (elParams != null && elParams.Values != null && elParams.Error == null && elParams.Values.Count(p => p.Key.Equals(Common.Constants.Stellaris.InlineScriptId, StringComparison.OrdinalIgnoreCase)) == 1)
            {
                var processed = code;
                var elObj = elParams.Values.FirstOrDefault(p => p.Values != null);
                if (elObj != null)
                {
                    foreach (var value in elObj.Values)
                    {
                        var id = (value.Key ?? string.Empty).Trim(Quotes);
                        var replacement = (value.Value ?? string.Empty).Trim(Quotes);
                        if (!id.Equals(Script, StringComparison.OrdinalIgnoreCase))
                        {
                            var key = $"{Terminator}{id}{Terminator}";
                            processed = processed.Replace(key, replacement, StringComparison.OrdinalIgnoreCase);
                        }
                    }
                }
                return processed;
            }
            return string.Empty;
        }

        #endregion Methods
    }
}
