// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 04-02-2020
//
// Last Modified By : Mario
// Last Modified On : 04-03-2020
// ***********************************************************************
// <copyright file="StellarisDefinitionMerger.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.IO.Mods.Mergers
{
    /// <summary>
    /// Class StellarisDefinitionMerger.
    /// Implements the <see cref="IronyModManager.IO.Mods.Mergers.BaseDefinitionMerger" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Mods.Mergers.BaseDefinitionMerger" />
    public class StellarisDefinitionMerger : BaseDefinitionMerger
    {
        #region Properties

        /// <summary>
        /// Gets the fios paths.
        /// </summary>
        /// <value>The fios paths.</value>
        public override IReadOnlyCollection<string> FIOSPaths => new List<string> { "component_sets", "component_templates", "event_chains", "global_ship_designs",
            "scripted_variables", "section_templates", "ship_behaviors", "special_projects", "star_classes", "static_modifiers", "strategic_resources", "events" };

        #endregion Properties

        #region Methods

        /// <summary>
        /// Values the tuple.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>CanProcess.</returns>
        public override bool CanProcess(string game)
        {
            return game.Equals(Shared.Constants.GamesTypes.Stellaris.Name);
        }

        #endregion Methods
    }
}
