
// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 10-03-2023
//
// Last Modified By : Mario
// Last Modified On : 10-03-2023
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
        /// Gets the object identifier.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        public IReadOnlyCollection<string> GetObjectId(string code, string parameters)
        {
            var elParams = codeParser.ParseScriptWithoutValidation(parameters.SplitOnNewLine(), string.Empty);
            if (elParams != null && elParams.Values != null && elParams.Error == null && elParams.Values.Count(p => p.Key.Equals(Common.Constants.Stellaris.InlineScriptId, StringComparison.OrdinalIgnoreCase)) == 1)
            {
                var elCode = codeParser.ParseScriptWithoutValidation(code.SplitOnNewLine(), string.Empty);
                if (elCode != null && elCode.Values != null && elCode.Error == null)
                {
                    var result = new List<string>();
                    foreach (var el in elCode.Values)
                    {
                        var terminatorCount = el.Key.Count(c => c == Terminator);
                        if (terminatorCount > 0)
                        {
                            var idElements = new List<string>();

                            // Needs to have even number
                            if (terminatorCount % 2 == 0)
                            {
                                var segments = el.Key.Split(Terminator, StringSplitOptions.RemoveEmptyEntries);
                                idElements.Add(segments[0]);
                                for (int i = 1; i < segments.Length; i++)
                                {
                                    if (elParams.Values.FirstOrDefault().Values.Any())
                                    {
                                        var match = elParams.Values.FirstOrDefault().Values.FirstOrDefault(p => p.Key.Equals(segments[i], StringComparison.OrdinalIgnoreCase));
                                        if (match != null)
                                        {
                                            idElements.Add((match.Value ?? string.Empty).Trim(Quotes));
                                        }
                                    }
                                }
                                result.Add(string.Join(string.Empty, idElements));
                            }
                        }
                        else
                        {
                            // Just take whatever we're given from the script key
                            result.Add(el.Key);
                        }
                    }
                    return result.Any() ? result.Distinct().ToList() : null;
                }
            }
            return null;
        }

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
                if (elParams.Values.FirstOrDefault().Values.Any())
                {
                    var match = elParams.Values.FirstOrDefault().Values.FirstOrDefault(p => p.Key.Equals(Script, StringComparison.OrdinalIgnoreCase));
                    if (match != null)
                    {
                        return ((match.Value ?? string.Empty).Trim(Quotes)).StandardizeDirectorySeparator();
                    }
                }
            }
            return string.Empty;
        }

        #endregion Methods
    }
}
