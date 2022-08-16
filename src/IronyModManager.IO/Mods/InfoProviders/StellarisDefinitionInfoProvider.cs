// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 04-02-2020
//
// Last Modified By : Mario
// Last Modified On : 08-15-2022
// ***********************************************************************
// <copyright file="StellarisDefinitionInfoProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IronyModManager.Shared;
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
        /// The localization synced
        /// </summary>
        private const string LocalizationSynced = "localisation_synced";

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
            "solar_system_initializers", "relics", "traits", "start_screen_messages" };

        /// <summary>
        /// Gets a value indicating whether this instance is fully implemented.
        /// </summary>
        /// <value><c>true</c> if this instance is fully implemented; otherwise, <c>false</c>.</value>
        public override bool IsFullyImplemented => true;

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

        /// <summary>
        /// Determines whether [is valid encoding] [the specified path].
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns><c>true</c> if [is valid encoding] [the specified path]; otherwise, <c>false</c>.</returns>
        public override bool IsValidEncoding(string path, Shared.EncodingInfo encoding)
        {
            var sanitizedPath = path ?? string.Empty;
            if (sanitizedPath.EndsWith(NameLists, StringComparison.OrdinalIgnoreCase))
            {
                return HasValidUTF8BOMEncoding(encoding);
            }
            return base.IsValidEncoding(path, encoding);
        }

        /// <summary>
        /// Generates the name of the localization file.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>System.String.</returns>
        protected override string GenerateLocalizationFileName(IDefinition definition, string fileName)
        {
            if (definition.File.StartsWith(LocalizationSynced))
            {
                var proposedFileName = Path.Combine(definition.ParentDirectory, $"{LIOSName}{fileName.GenerateValidFileName()}");
                return EnsureRuleEnforced(definition, proposedFileName, false);
            }
            return base.GenerateLocalizationFileName(definition, fileName);
        }

        #endregion Methods
    }
}
