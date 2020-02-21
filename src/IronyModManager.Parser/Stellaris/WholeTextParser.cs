// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-21-2020
// ***********************************************************************
// <copyright file="WholeTextParser.cs" company="Mario">
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
    /// Class FilenameParser.
    /// Implements the <see cref="IronyModManager.Parser.Stellaris.BaseStellarisParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Stellaris.BaseStellarisParser" />
    public class WholeTextParser : BaseStellarisParser
    {
        #region Fields

        /// <summary>
        /// The ends with check
        /// </summary>
        protected static readonly string[] endsWithCheck = new string[]
        {
            Constants.ShaderExtension, Constants.FxhExtension
        };

        /// <summary>
        /// The equals checks
        /// </summary>
        protected static readonly string[] equalsChecks = new string[]
        {
            Constants.Stellaris.Alerts,
            Constants.Stellaris.MessageTypes,
            Constants.Stellaris.WeaponComponents
        };

        /// <summary>
        /// The starts with checks
        /// </summary>
        protected static readonly string[] startsWithChecks = new string[]
        {
            Constants.Stellaris.StartScreenMessages, Constants.Stellaris.DiploPhrases,
            Constants.Stellaris.MapGalaxy, Constants.Stellaris.NameLists, Constants.Stellaris.OnActions,
            Constants.Stellaris.SpeciesNames, Constants.Stellaris.Terraform, Constants.Stellaris.Portraits
        };

        #endregion Fields

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return IsStellaris(args) && IsValidType(args);
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
            def.Code = string.Join(Environment.NewLine, args.Lines);
            def.Id = args.File.Split(Constants.Scripts.PathTrimParameters, StringSplitOptions.RemoveEmptyEntries).Last();
            def.ValueType = ValueType.WholeTextFile;
            return new List<IDefinition> { def };
        }

        /// <summary>
        /// Determines whether this instance [can parse ends with] the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance [can parse ends with] the specified arguments; otherwise, <c>false</c>.</returns>
        protected virtual bool CanParseEndsWith(CanParseArgs args)
        {
            return endsWithCheck.Any(s => args.File.EndsWith(s, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether this instance [can parse equals] the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance [can parse equals] the specified arguments; otherwise, <c>false</c>.</returns>
        protected virtual bool CanParseEquals(CanParseArgs args)
        {
            return equalsChecks.Any(s => args.File.Equals(s, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether this instance [can parse sound file] the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance [can parse sound file] the specified arguments; otherwise, <c>false</c>.</returns>
        protected virtual bool CanParseSoundFile(CanParseArgs args)
        {
            return args.File.StartsWith(Constants.Stellaris.Sound, StringComparison.OrdinalIgnoreCase) && Constants.TextExtensions.Any(s => args.File.EndsWith(s, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether this instance [can parse starts with] the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance [can parse starts with] the specified arguments; otherwise, <c>false</c>.</returns>
        protected virtual bool CanParseStartsWith(CanParseArgs args)
        {
            return startsWithChecks.Any(s => args.File.StartsWith(s, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is valid type] [the specified arguments].
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if [is valid type] [the specified arguments]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsValidType(CanParseArgs args)
        {
            return CanParseStartsWith(args) || CanParseEquals(args) ||
                CanParseEndsWith(args) || CanParseSoundFile(args);
        }

        #endregion Methods
    }
}
