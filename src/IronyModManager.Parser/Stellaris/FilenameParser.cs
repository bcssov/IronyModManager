// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-18-2020
// ***********************************************************************
// <copyright file="FilenameParser.cs" company="Mario">
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
    public class FilenameParser : BaseStellarisParser
    {
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
            var parsingArgs = ConstructArgs(args, def, null, null, 0, null);
            MapDefinitionFromArgs(parsingArgs);
            def.Code = string.Join(Environment.NewLine, args.Lines);
            def.Id = args.File.Split(Constants.Scripts.PathTrimParameters, StringSplitOptions.RemoveEmptyEntries).Last();
            def.ValueType = ValueType.WholeTextFile;
            return new List<IDefinition> { def };
        }

        /// <summary>
        /// Determines whether [is common root] [the specified arguments].
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if [is common root] [the specified arguments]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsCommonRoot(CanParseArgs args)
        {
            return Constants.Stellaris.CommonRootFiles.Any(s => args.File.Equals(s, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is map galaxy] [the specified arguments].
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if [is map galaxy] [the specified arguments]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsMapGalaxy(CanParseArgs args)
        {
            return args.File.StartsWith(Constants.Stellaris.MapGalaxy);
        }

        /// <summary>
        /// Determines whether [is on actions] [the specified arguments].
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if [is on actions] [the specified arguments]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsOnActions(CanParseArgs args)
        {
            return args.File.StartsWith(Constants.Stellaris.OnActions, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether [is valid type] [the specified arguments].
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if [is valid type] [the specified arguments]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsValidType(CanParseArgs args)
        {
            return IsCommonRoot(args) || IsMapGalaxy(args) || IsOnActions(args) || IsWeaponComponents(args);
        }

        /// <summary>
        /// Determines whether [is weapon components] [the specified arguments].
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if [is weapon components] [the specified arguments]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsWeaponComponents(CanParseArgs args)
        {
            return args.File.Equals(Constants.Stellaris.WeaponComponents, StringComparison.OrdinalIgnoreCase);
        }

        #endregion Methods
    }
}
