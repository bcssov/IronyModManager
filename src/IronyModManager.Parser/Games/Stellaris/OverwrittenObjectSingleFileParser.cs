﻿// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 10-25-2021
//
// Last Modified By : Mario
// Last Modified On : 02-21-2025
// ***********************************************************************
// <copyright file="OverwrittenObjectSingleFileParser.cs" company="Mario">
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
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.Parser.Games.Stellaris
{
    /// <summary>
    /// Class OverwrittenParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    public class OverwrittenObjectSingleFileParser : BaseParser, IGameParser
    {
        #region Fields

        /// <summary>
        /// The merge type ids
        /// </summary>
        private static readonly Dictionary<string, string[]> allowDuplicateIds = new() { { Common.Constants.Stellaris.Ethics.ToLowerInvariant(), ["ethic_categories"] } };

        /// <summary>
        /// The starts with checks
        /// </summary>
        private static readonly string[] directoryNames =
            [Common.Constants.Stellaris.Ethics, Common.Constants.Stellaris.StarbaseModules, Common.Constants.Stellaris.ShipSizes, Common.Constants.Stellaris.StrategicResources, Common.Constants.Stellaris.GovernmentAuthorities];

        /// <summary>
        /// The merge ids
        /// </summary>
        private static readonly Dictionary<string, string[]> mergeIds = new() { { Common.Constants.Stellaris.GovernmentAuthorities.ToLowerInvariant(), ["advanced_authority_swap"] } };

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlagsParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        /// <seealso cref="T:IronyModManager.Parser.Common.Parsers.IDefaultParser" />
        /// <remarks>Initializes a new instance of the <see cref="T:IronyModManager.Parser.Common.Parsers.BaseParser" /> class.</remarks>
        public OverwrittenObjectSingleFileParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Stellaris" + nameof(OverwrittenObjectSingleFileParser);

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
            if (results != null && results.Any())
            {
                foreach (var item in results)
                {
                    item.IsSpecialFolder = true;
                    if (item.ValueType == ValueType.Object)
                    {
                        item.ValueType = ValueType.OverwrittenObjectSingleFile;
                        if (allowDuplicateIds.ContainsKey(Path.GetDirectoryName(args.File)!))
                        {
                            var items = allowDuplicateIds[Path.GetDirectoryName(args.File)!];
                            if (items.Any(p => p.Equals(item.Id, StringComparison.OrdinalIgnoreCase)))
                            {
                                item.AllowDuplicate = true;
                            }
                        }
                        else if (mergeIds.ContainsKey(Path.GetDirectoryName(args.File)!))
                        {
                            // One has to ask themselves what the actual fuck?!? Who thought this was a good idea over at paradox to implement in such a manner?!?
                            var items = mergeIds[Path.GetDirectoryName(args.File)!];
                            var mergeSegments = TryParse(new ParserArgs(args) { Lines = item.Code.SplitOnNewLine() });
                            if (mergeSegments != null && mergeSegments.Values.Count() == 1)
                            {
                                foreach (var block in items)
                                {
                                    if (mergeSegments.Values.FirstOrDefault() != null && mergeSegments.Values.FirstOrDefault()!.Values != null &&
                                        mergeSegments.Values.FirstOrDefault()!.Values.All(p => (p.Key ?? string.Empty).Equals(block, StringComparison.OrdinalIgnoreCase)))
                                    {
                                        item.MergeType = MergeType.FlatMerge;
                                    }
                                }
                            }
                        }
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
            var directoryName = Path.GetDirectoryName(args.File);
            return directoryNames.Any(s => directoryName != null && directoryName.Equals(s, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Methods
    }
}
