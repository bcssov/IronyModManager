// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 07-18-2022
//
// Last Modified By : Mario
// Last Modified On : 02-07-2025
// ***********************************************************************
// <copyright file="HOI4DefinitionInfoProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.IO.Mods.InfoProviders
{
    /// <summary>
    /// Class HOI4DefinitionInfoProvider.
    /// Implements the <see cref="IronyModManager.IO.Mods.InfoProviders.BaseDefinitionInfoProvider" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Mods.InfoProviders.BaseDefinitionInfoProvider" />
    public class HOI4DefinitionInfoProvider : BaseDefinitionInfoProvider
    {
        private static readonly string[] ShallowComparisonExcludedDirectories =
        [
            "common\\on_actions".StandardizeDirectorySeparator(),
            "common\\scripted_effects".StandardizeDirectorySeparator(),
            "common\\scripted_localisation".StandardizeDirectorySeparator(),
            "common\\scripted_triggers".StandardizeDirectorySeparator()
        ];

        #region Properties

        /// <summary>
        /// Gets the fios paths.
        /// </summary>
        /// <value>The fios paths.</value>
        /// <exception cref="System.NotImplementedException"></exception>
        public override IReadOnlyCollection<string> FIOSPaths => throw new NotImplementedException();

        /// <summary>
        /// Gets the global variables path.
        /// </summary>
        /// <value>The global variables path.</value>
        public override string GlobalVariablesPath => string.Empty;

        /// <summary>
        /// Gets the inline scripts path.
        /// </summary>
        /// <value>The inline scripts path.</value>
        public override string InlineScriptsPath => string.Empty;

        /// <summary>
        /// Gets a value indicating whether this instance is fully implemented.
        /// </summary>
        /// <value><c>true</c> if this instance is fully implemented; otherwise, <c>false</c>.</value>
        public override bool IsFullyImplemented => false;

        /// <summary>
        /// Gets the merge types.
        /// </summary>
        /// <value>The merge types.</value>
        public override IDictionary<MergeType, List<string>> MergeTypes => null;

        /// <summary>
        /// Gets a value indicating whether [supports inline scripts].
        /// </summary>
        /// <value><c>true</c> if [supports inline scripts]; otherwise, <c>false</c>.</value>
        public override bool SupportsInlineScripts => false;

        /// <summary>
        /// Gets a value indicating whether [supports script merge].
        /// </summary>
        /// <value><c>true</c> if [supports script merge]; otherwise, <c>false</c>.</value>
        public override bool SupportsScriptMerge => false;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Values the tuple.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>CanProcess.</returns>
        public override bool CanProcess(string game)
        {
            return game.Equals(Shared.Constants.GamesTypes.HeartsOfIron4.Id);
        }

        /// <inheritdoc />
        public override bool CanUseShallowComparison(IDefinition definition)
        {
            if (definition == null || definition.ValueType != ValueType.Object)
            {
                return false;
            }

            var directory = (definition.ParentDirectoryCI ?? string.Empty).StandardizeDirectorySeparator().TrimEnd(Path.DirectorySeparatorChar);
            if (!directory.Equals("common", StringComparison.OrdinalIgnoreCase) &&
                !directory.StartsWith("common" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return ShallowComparisonExcludedDirectories.All(excluded =>
                !directory.Equals(excluded, StringComparison.OrdinalIgnoreCase) &&
                !directory.StartsWith(excluded + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Methods
    }
}
