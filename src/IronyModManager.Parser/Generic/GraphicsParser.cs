// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2022
// ***********************************************************************
// <copyright file="GraphicsParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;

namespace IronyModManager.Parser.Generic
{
    /// <summary>
    /// Class GraphicsParser.
    /// Implements the <see cref="IronyModManager.Parser.Generic.BaseGraphicsParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.Generic.BaseGraphicsParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    public class GraphicsParser : BaseParser, IGenericParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public GraphicsParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Generic" + nameof(GraphicsParser);

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
            return args.File.EndsWith(Common.Constants.GuiExtension, StringComparison.OrdinalIgnoreCase) || args.File.EndsWith(Common.Constants.GfxExtension, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var result = ParseSecondLevel(args);
            var replaceFolder = Path.DirectorySeparatorChar + "replace";
            if (Path.GetDirectoryName(args.File).EndsWith(replaceFolder))
            {
                foreach (var item in result)
                {
                    item.VirtualPath = Path.Combine(Path.GetDirectoryName(args.File).Replace(replaceFolder, string.Empty), Path.GetFileName(args.File));
                    item.Type = FormatType(item.VirtualPath);
                }
            }
            return result;
        }

        /// <summary>
        /// Evals the element for identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        protected override string EvalElementForId(IScriptElement value)
        {
            if (Common.Constants.Scripts.GraphicsTypeName.Equals(value.Key, StringComparison.OrdinalIgnoreCase))
            {
                return value.Value;
            }
            return base.EvalElementForId(value);
        }

        #endregion Methods
    }
}
