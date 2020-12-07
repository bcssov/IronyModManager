// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 04-02-2020
//
// Last Modified By : Mario
// Last Modified On : 12-07-2020
// ***********************************************************************
// <copyright file="StellarisDefinitionInfoProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using IronyModManager.Shared.Models;

namespace IronyModManager.IO.Mods.InfoProviders
{
    /// <summary>
    /// Class StellarisDefinitionInfoProvider.
    /// Implements the <see cref="IronyModManager.IO.Mods.InfoProviders.BaseDefinitionInfoProvider" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Mods.InfoProviders.BaseDefinitionInfoProvider" />
    public class StellarisDefinitionInfoProvider : BaseDefinitionInfoProvider
    {
        #region Fields

        /// <summary>
        /// The name lists
        /// </summary>
        private const string NameLists = "name_lists";

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the fios paths.
        /// </summary>
        /// <value>The fios paths.</value>
        public override IReadOnlyCollection<string> FIOSPaths => new List<string> { "component_sets", "component_templates", "event_chains", "global_ship_designs",
            "scripted_variables", "section_templates", "ship_behaviors", "special_projects", "star_classes", "static_modifiers", "strategic_resources", "events",
            "solar_system_initializers", "relics" };

        #endregion Properties

        #region Methods

        /// <summary>
        /// Values the tuple.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>CanProcess.</returns>
        public override bool CanProcess(string game)
        {
            return game.Equals(Shared.Constants.GamesTypes.Stellaris.Id);
        }

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>Encoding.</returns>
        public override Encoding GetEncoding(IDefinition definition)
        {
            EnsureValidType(definition);
            if (definition.ParentDirectory.EndsWith(NameLists, StringComparison.OrdinalIgnoreCase))
            {
                return new UTF8Encoding(true);
            }
            return base.GetEncoding(definition);
        }

        #endregion Methods
    }
}
