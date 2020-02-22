// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-22-2020
// ***********************************************************************
// <copyright file="GenericBinaryParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Default;

namespace IronyModManager.Parser.Generic
{
    /// <summary>
    /// Class GenericBinaryParser.
    /// Implements the <see cref="IronyModManager.Parser.Default.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.Generic.IGenericParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Default.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.Generic.IGenericParser" />
    public class GenericBinaryParser : BaseParser, IGenericParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericBinaryParser"/> class.
        /// </summary>
        /// <param name="textParser">The text parser.</param>
        public GenericBinaryParser(ITextParser textParser) : base(textParser)
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public bool CanParse(CanParseArgs args)
        {
            return !Constants.TextExtensions.Any(s => args.File.EndsWith(s, StringComparison.OrdinalIgnoreCase));
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
            def.Code = string.Empty;
            def.Id = args.File.Split(Constants.Scripts.PathTrimParameters, StringSplitOptions.RemoveEmptyEntries).Last();
            def.ValueType = ValueType.Binary;
            def.Type = FormatType(args.File, Constants.BinaryType);
            return new List<IDefinition> { def };
        }

        #endregion Methods
    }
}
