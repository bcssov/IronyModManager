// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 03-28-2020
//
// Last Modified By : Mario
// Last Modified On : 03-28-2020
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
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Parsers;

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
        /// The ends with check
        /// </summary>
        private static readonly string[] endsWithCheck = new string[]
        {
            Common.Constants.ShaderExtension, Common.Constants.FxhExtension
        };

        /// <summary>
        /// The starts with checks
        /// </summary>
        private static readonly string[] startsWithChecks = new string[]
        {
            Common.Constants.OnActionsPath
        };

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WholeTextParser" /> class.
        /// </summary>
        /// <param name="textParser">The text parser.</param>
        public WholeTextParser(ITextParser textParser) : base(textParser)
        {
        }

        #endregion Constructors

        #region Properties

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
        public virtual bool CanParse(CanParseArgs args)
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
            // This type is a bit different and only will conflict in filenames.
            var def = GetDefinitionInstance();
            var parsingArgs = ConstructArgs(args, def);
            MapDefinitionFromArgs(parsingArgs);
            def.Code = string.Join(Environment.NewLine, args.Lines);
            def.Id = args.File.Split(Common.Constants.Scripts.PathTrimParameters, StringSplitOptions.RemoveEmptyEntries).Last();
            def.ValueType = Common.ValueType.WholeTextFile;
            return new List<IDefinition> { def };
        }

        /// <summary>
        /// Determines whether this instance [can parse ends with] the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance [can parse ends with] the specified arguments; otherwise, <c>false</c>.</returns>
        protected virtual bool CanParseEndsWith(CanParseArgs args)
        {
            return endsWithCheck.Any(s => args.File.EndsWith(s, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether this instance [can parse root common file] the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance [can parse root common file] the specified arguments; otherwise, <c>false</c>.</returns>
        protected virtual bool CanParseRootCommonFile(CanParseArgs args)
        {
            return Path.GetDirectoryName(args.File).Equals(Common.Constants.CommonPath);
        }

        /// <summary>
        /// Determines whether this instance [can parse sound file] the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance [can parse sound file] the specified arguments; otherwise, <c>false</c>.</returns>
        protected virtual bool CanParseSoundFile(CanParseArgs args)
        {
            return args.File.StartsWith(Common.Constants.Stellaris.Sound, StringComparison.OrdinalIgnoreCase) && Shared.Constants.TextExtensions.Any(s => args.File.EndsWith(s, StringComparison.OrdinalIgnoreCase));
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
