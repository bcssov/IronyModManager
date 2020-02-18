// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-18-2020
// ***********************************************************************
// <copyright file="ComponentTagsParser.cs" company="Mario">
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
    /// Class ComponentTagsParser.
    /// Implements the <see cref="IronyModManager.Parser.Stellaris.BaseStellarisParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Stellaris.BaseStellarisParser" />
    public class ComponentTagsParser : BaseStellarisParser
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return IsStellaris(args) && args.File.StartsWith(Constants.Stellaris.ComponentTags, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var col = new List<IDefinition>();
            foreach (var line in args.Lines)
            {
                if (line.Trim().StartsWith(Constants.Scripts.ScriptCommentId) || line.Trim().Length == 0)
                {
                    continue;
                }
                var def = GetDefinitionInstance();
                var parsingArgs = ConstructArgs(args, def, null, null, 0, null);
                MapDefinitionFromArgs(parsingArgs);
                def.Code = line;
                def.Id = line;
                def.ValueType = ValueType.Variable;
                col.Add(def);
            }
            return col;
        }

        #endregion Methods
    }
}
