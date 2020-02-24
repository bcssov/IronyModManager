// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-22-2020
// ***********************************************************************
// <copyright file="ComponentTagsParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Parser.Games.Stellaris
{
    /// <summary>
    /// Class ComponentTagsParser.
    /// Implements the <see cref="IronyModManager.Parser.Games.Stellaris.BaseStellarisParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Games.Stellaris.BaseStellarisParser" />
    public class ComponentTagsParser : BaseStellarisParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentTagsParser"/> class.
        /// </summary>
        /// <param name="textParser">The text parser.</param>
        public ComponentTagsParser(ITextParser textParser) : base(textParser)
        {
        }

        #endregion Constructors

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
                if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith(Constants.Scripts.ScriptCommentId))
                {
                    continue;
                }
                var def = GetDefinitionInstance();
                var parsingArgs = ConstructArgs(args, def);
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
