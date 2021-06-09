// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 06-09-2021
//
// Last Modified By : Mario
// Last Modified On : 06-09-2021
// ***********************************************************************
// <copyright file="OverWrittenObjectWithPreserveFileNameParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.Parser.Games.Stellaris
{
    /// <summary>
    /// Class OverWrittenObjectWithPreserveFileName.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    public class OverWrittenObjectWithPreserveFileNameParser : BaseParser, IGameParser
    {
        #region Fields

        /// <summary>
        /// The directory names
        /// </summary>
        private static readonly string[] directoryNames = new string[]
        {
            Common.Constants.Stellaris.StrategicResources
        };

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OverWrittenObjectWithPreserveFileNameParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public OverWrittenObjectWithPreserveFileNameParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Stellaris" + nameof(OverWrittenObjectWithPreserveFileNameParser);

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority => 1;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return args.IsStellaris() && CanParseStartsWith(args);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var results = ParseRoot(args);
            if (results?.Count() > 0)
            {
                foreach (var item in results)
                {
                    if (item.ValueType == ValueType.Object)
                    {
                        item.ValueType = ValueType.OverWrittenObjectWithPreserveFileName;
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Determines whether this instance [can parse starts with] the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance [can parse starts with] the specified arguments; otherwise, <c>false</c>.</returns>
        protected virtual bool CanParseStartsWith(CanParseArgs args)
        {
            var directoryName = System.IO.Path.GetDirectoryName(args.File);
            return directoryNames.Any(s => directoryName.Equals(s, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Methods
    }
}
