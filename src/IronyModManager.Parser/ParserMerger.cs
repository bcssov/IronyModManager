// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 10-18-2024
//
// Last Modified By : Mario
// Last Modified On : 10-20-2024
// ***********************************************************************
// <copyright file="ParserMerger.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Class ParserMerger.
    /// Implements the <see cref="BaseParser" />
    /// Implements the <see cref="IParserMerger" />
    /// </summary>
    /// <seealso cref="BaseParser" />
    /// <seealso cref="IParserMerger" />
    public class ParserMerger : BaseParser, IParserMerger
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserMerger" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public ParserMerger(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => nameof(ParserMerger);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return false; // Readonly, hidden or whatever you want to call it
        }

        /// <summary>
        /// Merges the top level.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="target">The target.</param>
        /// <param name="targetCode">The target code.</param>
        /// <param name="addToEnd">if set to <c>true</c> [add to end].</param>
        /// <returns>System.String.</returns>
        public string MergeTopLevel(IEnumerable<string> code, string fileName, string target, IEnumerable<string> targetCode, bool addToEnd)
        {
            var nodes = TryParse(new ParserArgs { File = fileName, Lines = code, ValidationType = ValidationType.SkipAll });
            var targetNodes = TryParse(new ParserArgs { File = fileName, Lines = targetCode, ValidationType = ValidationType.SkipAll });
            if (nodes.Error == null && targetNodes.Error == null && nodes.Values.Any() && targetNodes.Values.Any())
            {
                var values = nodes.Values.FirstOrDefault()!.Values.ToList();
                if (addToEnd)
                {
                    values.AddRange(targetNodes.Values.FirstOrDefault()!.Values);
                }
                else
                {
                    var match = values.FirstOrDefault(p => (p.Key ?? string.Empty).Equals(target, StringComparison.OrdinalIgnoreCase));
                    if (match == null)
                    {
                        values.AddRange(targetNodes.Values.FirstOrDefault()!.Values);
                    }
                    else
                    {
                        var index = values.IndexOf(match);
                        values.InsertRange(index, targetNodes.Values.FirstOrDefault()!.Values);
                    }
                }

                var newNodes = nodes.Values.ToList();
                newNodes[0].Values = values;

                var sb = new StringBuilder();
                foreach (var node in newNodes)
                {
                    sb.AppendLine(FormatCode(node));
                }

                return sb.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            throw new NotImplementedException("If this parser is being used it's a mistake.");
        }

        #endregion Methods
    }
}
