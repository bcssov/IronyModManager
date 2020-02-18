// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-18-2020
// ***********************************************************************
// <copyright file="GenericLocalizationParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Class GenericLocalizationParser.
    /// Implements the <see cref="IronyModManager.Parser.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.IGenericParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.IGenericParser" />
    public class GenericLocalizationParser : BaseParser, IGenericParser
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public bool CanParse(CanParseArgs args)
        {
            return args.File.EndsWith(Constants.LocalizationExtension, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var result = new List<IDefinition>();
            string selectedLanguage = string.Empty;
            foreach (var line in args.Lines)
            {
                if (line.Trim().StartsWith(Constants.Scripts.ScriptCommentId))
                {
                    continue;
                }
                var lang = GetLanguageId(line);
                if (!string.IsNullOrWhiteSpace(lang))
                {
                    selectedLanguage = lang;
                }
                if (!string.IsNullOrWhiteSpace(selectedLanguage) && !string.IsNullOrWhiteSpace(line))
                {
                    if (char.IsWhiteSpace(line[0]))
                    {
                        var def = GetDefinitionInstance();
                        var parsingArgs = ConstructArgs(args, def, null, null, 0, null);
                        MapDefinitionFromArgs(parsingArgs);
                        def.Code = $"{selectedLanguage}:{Environment.NewLine}{line}";
                        def.Id = GetOperationKey(line, Constants.Localization.YmlSeparator);
                        def.ValueType = ValueType.Variable;
                        result.Add(def);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the language identifier.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetLanguageId(string line)
        {
            var lang = Constants.Localization.Locales.FirstOrDefault(s => line.StartsWith(s, StringComparison.OrdinalIgnoreCase));
            return lang;
        }

        #endregion Methods
    }
}
