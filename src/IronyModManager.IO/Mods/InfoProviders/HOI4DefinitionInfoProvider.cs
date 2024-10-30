// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 07-18-2022
//
// Last Modified By : Mario
// Last Modified On : 10-30-2024
// ***********************************************************************
// <copyright file="HOI4DefinitionInfoProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Shared.Models;

namespace IronyModManager.IO.Mods.InfoProviders
{
    /// <summary>
    /// Class HOI4DefinitionInfoProvider.
    /// Implements the <see cref="IronyModManager.IO.Mods.InfoProviders.BaseDefinitionInfoProvider" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Mods.InfoProviders.BaseDefinitionInfoProvider" />
    public class HOI4DefinitionInfoProvider : BaseDefinitionInfoProvider
    {
        #region Properties

        /// <summary>
        /// Gets the fios paths.
        /// </summary>
        /// <value>The fios paths.</value>
        /// <exception cref="System.NotImplementedException"></exception>
        public override IReadOnlyCollection<string> FIOSPaths => throw new NotImplementedException();

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

        #endregion Methods
    }
}
