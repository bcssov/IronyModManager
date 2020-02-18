// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-18-2020
// ***********************************************************************
// <copyright file="FlagsParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Parser.Stellaris
{
    /// <summary>
    /// Class FlagsParser.
    /// Implements the <see cref="IronyModManager.Parser.Stellaris.BaseStellarisParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Stellaris.BaseStellarisParser" />
    public class FlagsParser : BaseStellarisParser
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return IsStellaris(args) && args.File.StartsWith(Constants.Stellaris.Flags, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var def = GetDefinitionInstance();
            var parsingArgs = ConstructArgs(args, def, null, null, 0, null);
            MapDefinitionFromArgs(parsingArgs);
            def.Code = args.Lines != null ? string.Join(Environment.NewLine, args.Lines) : string.Empty;
            def.Id = args.File.Split(Constants.Scripts.PathTrimParameters, StringSplitOptions.RemoveEmptyEntries).Last();
            def.ValueType = !Constants.TextExtensions.Any(s => args.File.EndsWith(s, StringComparison.OrdinalIgnoreCase)) ? ValueType.Binary : ValueType.WholeTextFile;
            return new List<IDefinition> { def };
        }

        #endregion Methods
    }
}
