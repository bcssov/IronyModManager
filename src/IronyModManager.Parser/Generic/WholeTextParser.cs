// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 03-28-2020
//
// Last Modified By : Mario
// Last Modified On : 10-17-2024
// ***********************************************************************
// <copyright file="WholeTextParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.Parser.Generic
{
    /// <summary>
    /// Class WholeTextParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    public class WholeTextParser : BaseParser, IGenericParser
    {
        #region Fields

        /// <summary>
        /// The skip validation for types
        /// </summary>
        private static readonly string[] skipValidationForTypes = [Common.Constants.ShaderExtension, Common.Constants.FxhExtension, Common.Constants.CsvExtension];

        /// <summary>
        /// The starts with checks
        /// </summary>
        private static readonly string[] startsWithChecks = [Common.Constants.OnActionsPath];

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WholeTextParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public WholeTextParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Generic" + nameof(WholeTextParser);

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public virtual int Priority => 1;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return IsValidType(args);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var fileNameTag = IsFileNameTag(args);

            // Doesn't seem to like fxh and or shader file extensions
            if (!fileNameTag)
            {
                var errors = EvalForErrorsOnly(args);
                if (errors != null)
                {
                    return errors;
                }
            }

            var def = GetDefinitionInstance();
            MapDefinitionFromArgs(ConstructArgs(args, def));
            def.Id = Path.GetFileName(args.File)!.ToLowerInvariant();
            if (fileNameTag)
            {
                def.OriginalCode = def.Code = GetFileTagCode(args.File, args.Lines);
                def.Tags.Add(def.Id.ToLowerInvariant());
            }
            else
            {
                var code = CodeParser.ParseScriptWithoutValidation(args.Lines, args.File);
                def.OriginalCode = def.Code = GetNonFileTagCode(code);

                var definitions = new List<IDefinition>();
                definitions.AddRange(ParseSimpleTypes(code.Values, args));
                definitions.AddRange(ParseComplexTypes(code.Values, args));
                definitions.AddRange(ParseTypesForVariables(code.Values, args));
                foreach (var item in definitions)
                {
                    foreach (var tag in item.Tags)
                    {
                        var lower = tag.ToLowerInvariant();
                        if (!def.Tags.Contains(lower))
                        {
                            def.Tags.Add(lower);
                        }
                    }
                }
            }

            def.ValueType = ValueType.WholeTextFile;
            return new List<IDefinition> { def };
        }

        /// <summary>
        /// Determines whether this instance [can parse ends with] the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance [can parse ends with] the specified arguments; otherwise, <c>false</c>.</returns>
        protected virtual bool CanParseEndsWith(CanParseArgs args)
        {
            return skipValidationForTypes.Any(s => args.File.EndsWith(s, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether this instance [can parse root common file] the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance [can parse root common file] the specified arguments; otherwise, <c>false</c>.</returns>
        protected virtual bool CanParseRootCommonFile(CanParseArgs args)
        {
            return Path.GetDirectoryName(args.File)!.Equals(Common.Constants.CommonPath);
        }

        /// <summary>
        /// Determines whether this instance [can parse sound file] the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance [can parse sound file] the specified arguments; otherwise, <c>false</c>.</returns>
        protected virtual bool CanParseSoundFile(CanParseArgs args)
        {
            return args.File.StartsWith(Common.Constants.Stellaris.Sound, StringComparison.OrdinalIgnoreCase) && Constants.TextExtensions.Any(s => args.File.EndsWith(s, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether this instance [can parse starts with] the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance [can parse starts with] the specified arguments; otherwise, <c>false</c>.</returns>
        protected virtual bool CanParseStartsWith(CanParseArgs args)
        {
            return startsWithChecks.Any(s => args.File.StartsWith(s, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the file tag code.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="lines">The lines.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetFileTagCode(string file, IEnumerable<string> lines)
        {
            return string.Join(Environment.NewLine, lines);
        }

        /// <summary>
        /// Gets the non file tag code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetNonFileTagCode(IParseResponse code)
        {
            var sb = new StringBuilder();
            foreach (var result in code.Values)
            {
                sb.AppendLine(FormatCode(result));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Determines whether [is file name tag] [the specified arguments].
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if [is file name tag] [the specified arguments]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsFileNameTag(ParserArgs args)
        {
            return skipValidationForTypes.Any(p => args.File.EndsWith(p, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is valid type] [the specified arguments].
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if [is valid type] [the specified arguments]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsValidType(CanParseArgs args)
        {
            return CanParseStartsWith(args) || CanParseEndsWith(args) || CanParseSoundFile(args) || CanParseRootCommonFile(args);
        }

        #endregion Methods
    }
}
