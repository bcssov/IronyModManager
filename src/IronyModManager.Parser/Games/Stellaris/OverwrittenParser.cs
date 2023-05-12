// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 05-25-2020
//
// Last Modified By : Mario
// Last Modified On : 10-14-2022
// ***********************************************************************
// <copyright file="OverwrittenParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Common.Parsers.Models;
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
    public class OverwrittenParser : BaseParser, IGameParser
    {
        #region Fields

        /// <summary>
        /// The starts with checks
        /// </summary>
        private static readonly string[] directoryNames = new string[]
        {
            Common.Constants.Stellaris.PopJobs, Common.Constants.Stellaris.Traits,
            Common.Constants.Stellaris.Districts, Common.Constants.Stellaris.PlanetClasses,
            Common.Constants.Stellaris.PrescriptedCountries, Common.Constants.Stellaris.SpeciesArchetypes,
            Common.Constants.Stellaris.Buildings, Common.Constants.Stellaris.DiplomaticActions,
            Common.Constants.Stellaris.Technology,
            Common.Constants.Stellaris.CountryTypes, Common.Constants.Stellaris.Terraform,
            Common.Constants.Stellaris.Relics, Common.Constants.Stellaris.OpinionModifiers,
            Common.Constants.Stellaris.SectionTemplates
        };

        /// <summary>
        /// The partial directory names
        /// </summary>
        private static readonly string[] partialDirectoryNames = new string[]
        {
            Common.Constants.Stellaris.SpeciesRights
        };

        /// <summary>
        /// The key type
        /// </summary>
        private bool keyType = false;

        /// <summary>
        /// The terraform type
        /// </summary>
        private bool terraformType = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlagsParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public OverwrittenParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Stellaris" + nameof(OverwrittenParser);

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
            keyType = false;
            if (args.File.StartsWith(Common.Constants.Stellaris.PlanetClasses) || args.File.StartsWith(Common.Constants.Stellaris.SectionTemplates))
            {
                keyType = true;
            }
            else if (args.File.StartsWith(Common.Constants.Stellaris.Terraform))
            {
                terraformType = true;
            }
            var results = ParseRoot(args);
            if (results?.Count() > 0)
            {
                foreach (var item in results)
                {
                    if (item.ValueType == ValueType.Object)
                    {
                        item.ValueType = ValueType.OverwrittenObject;
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
            return directoryNames.Any(s => directoryName.Equals(s, StringComparison.OrdinalIgnoreCase)) || partialDirectoryNames.Any(s => directoryName.StartsWith(s, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Evals the definition identifier.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="defaultId">The default identifier.</param>
        /// <returns>System.String.</returns>
        protected override string EvalDefinitionId(IEnumerable<IScriptElement> values, string defaultId)
        {
            if (terraformType)
            {
                if (values?.Count() > 0)
                {
                    string from = string.Empty;
                    string to = string.Empty;
                    foreach (var item in values)
                    {
                        if (item.Key.Equals("from", StringComparison.OrdinalIgnoreCase))
                        {
                            from = item.Value;
                        }
                        else if (item.Key.Equals("to", StringComparison.OrdinalIgnoreCase))
                        {
                            to = item.Value;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to))
                    {
                        return $"{from}-{to}";
                    }
                    else if (!string.IsNullOrWhiteSpace(from))
                    {
                        return from;
                    }
                    else if (!string.IsNullOrWhiteSpace(to))
                    {
                        return to;
                    }
                }
                return defaultId;
            }
            return base.EvalDefinitionId(values, defaultId);
        }

        /// <summary>
        /// Evals the element for identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        protected override string EvalElementForId(IScriptElement value)
        {
            if (keyType)
            {
                if (Common.Constants.Scripts.GenericKeys.Any(s => s.Equals(value.Key, StringComparison.OrdinalIgnoreCase)))
                {
                    return value.Value;
                }
            }
            return base.EvalElementForId(value);
        }

        #endregion Methods
    }
}
