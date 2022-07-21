// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 07-20-2022
// ***********************************************************************
// <copyright file="FlagsParser.cs" company="Mario">
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
    /// Class FlagsParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    public class FlagsParser : BaseParser, IGameParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlagsParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public FlagsParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Stellaris" + nameof(FlagsParser);

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
            return args.IsStellaris() && args.File.StartsWith(Common.Constants.Stellaris.Flags, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var valType = args.IsBinary ? ValueType.Binary : ValueType.WholeTextFile;
            if (valType == ValueType.WholeTextFile)
            {
                var errors = EvalForErrorsOnly(args);
                if (errors != null)
                {
                    return errors;
                }
            }

            var def = GetDefinitionInstance();
            def.OriginalCode = def.Code = args.Lines != null ? string.Join(Environment.NewLine, args.Lines.Where(p => !string.IsNullOrWhiteSpace(p))) : string.Empty;
            if (FileSignatureUtility.IsImageFile(args.File))
            {
                def.Id = Path.GetFileNameWithoutExtension(args.File).ToLowerInvariant();
            }
            else
            {
                def.Id = Path.GetFileName(args.File).ToLowerInvariant();
            }
            def.ValueType = valType;
            if (valType == ValueType.WholeTextFile)
            {
                // Get tags only
                var definitions = ParseRoot(args);
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
            else
            {
                def.Tags.Add(def.Id);
            }
            MapDefinitionFromArgs(ConstructArgs(args, def, typeOverride: def.ValueType == ValueType.Binary ? Common.Constants.BinaryType : Common.Constants.TxtType));
            return new List<IDefinition> { def };
        }

        #endregion Methods
    }
}
