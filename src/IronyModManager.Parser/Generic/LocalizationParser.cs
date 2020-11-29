﻿// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 11-03-2020
// ***********************************************************************
// <copyright file="LocalizationParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using IronyModManager.DI;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Shared;

namespace IronyModManager.Parser.Generic
{
    /// <summary>
    /// Class LocalizationParser.
    /// Implements the <see cref="IronyModManager.Parser.Generic.BaseLineParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Generic.BaseLineParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    public class LocalizationParser : BaseLineParser, IGenericParser
    {
        #region Fields

        /// <summary>
        /// The key regex
        /// </summary>
        protected static readonly Regex keyRegex = new Regex(@"^[\w_.-]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public LocalizationParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Generic" + nameof(LocalizationParser);

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority => 2;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return args.File.EndsWith(Common.Constants.LocalizationExtension, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var result = new List<IDefinition>();
            var errors = new List<string>();
            string selectedLanguage = string.Empty;
            string prevId = string.Empty;
            foreach (var line in args.Lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                var cleaned = line.Trim().Trim('\t');
                if (cleaned.Trim().StartsWith(Common.Constants.Scripts.ScriptCommentId))
                {
                    continue;
                }
                var lang = GetLanguageId(cleaned);
                if (!string.IsNullOrWhiteSpace(lang))
                {
                    // Fix mods that have one language id in file and other in filename... Why?
                    selectedLanguage = lang;
                    var fileLanguage = GetLanguageIdFromFileName(args.File);
                    if (!string.IsNullOrWhiteSpace(fileLanguage) && selectedLanguage != fileLanguage)
                    {
                        selectedLanguage = fileLanguage;
                    }
                }
                if (!string.IsNullOrWhiteSpace(selectedLanguage))
                {
                    if (string.IsNullOrWhiteSpace(lang))
                    {
                        var message = ValidateKey(cleaned, prevId);
                        if (string.IsNullOrWhiteSpace(message))
                        {
                            var def = GetDefinitionInstance();
                            MapDefinitionFromArgs(ConstructArgs(args, def, typeOverride: $"{selectedLanguage}-{Common.Constants.YmlType}"));
                            def.Code = $"{selectedLanguage}:{Environment.NewLine} {cleaned}";
                            def.OriginalCode = cleaned;
                            def.CodeSeparator = Constants.CodeSeparators.NonClosingSeparators.ColonSign;
                            def.CodeTag = selectedLanguage.Split("=:{".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
                            def.Id = GetKey(cleaned, Common.Constants.Localization.YmlSeparator.ToString());
                            prevId = def.Id;
                            def.ValueType = Common.ValueType.SpecialVariable;
                            def.Tags.Add(def.Id.ToLowerInvariant());
                            result.Add(def);
                        }
                        else
                        {
                            errors.Add(message);
                        }
                    }
                }
            }
            if (errors.Count > 0)
            {
                var error = DIResolver.Get<IScriptError>();
                error.Message = string.Join(Environment.NewLine, errors);
                return new List<IDefinition>() { TranslateScriptError(error, args, $"{selectedLanguage}-{Common.Constants.YmlType}") };
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
            var lang = Common.Constants.Localization.Locales.FirstOrDefault(s => line.StartsWith(s, StringComparison.OrdinalIgnoreCase));
            return lang;
        }

        /// <summary>
        /// Gets the name of the language identifier from file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetLanguageIdFromFileName(string filename)
        {
            var locale = Common.Constants.Localization.Locales.FirstOrDefault(p => filename.EndsWith($"{p}.yml", StringComparison.OrdinalIgnoreCase));
            if (locale == null)
            {
                locale = Common.Constants.Localization.LocaleFolders.FirstOrDefault(p => filename.Contains($"{Path.DirectorySeparatorChar}{p}{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase));
            }
            return locale;
        }

        /// <summary>
        /// Validates the key.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="previousKey">The previous key.</param>
        /// <returns>System.String.</returns>
        protected virtual string ValidateKey(string line, string previousKey)
        {
            var cleaned = codeParser.CleanWhitespace(line);
            if (!cleaned.Contains(Common.Constants.Localization.YmlSeparator.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return $"Missing separator near key: {previousKey}.";
            }
            else
            {
                var key = cleaned.Split(Common.Constants.Localization.YmlSeparator)[0].Trim();
                if (!keyRegex.IsMatch(key))
                {
                    return $"Line contains invalid characters in key: {key}.";
                }
            }
            return string.Empty;
        }

        #endregion Methods
    }
}