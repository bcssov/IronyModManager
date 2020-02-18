// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 02-18-2020
// ***********************************************************************
// <copyright file="Constants.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Class Constants.
    /// </summary>
    public static class Constants
    {
        #region Fields

        /// <summary>
        /// The GFX extension
        /// </summary>
        public const string GfxExtension = ".gfx";

        /// <summary>
        /// The GUI extension
        /// </summary>
        public const string GuiExtension = ".gui";

        /// <summary>
        /// The localization extension
        /// </summary>
        public const string LocalizationExtension = ".yml";

        /// <summary>
        /// The text extensions
        /// </summary>
        public static readonly string[] TextExtensions = new string[] { ".txt", ".asset", ".gui", ".gfx", ".yml", ".csv", ".shader", ".fxh" };

        #endregion Fields

        #region Classes

        /// <summary>
        /// Class Localization.
        /// </summary>
        public static class Localization
        {
            #region Fields

            /// <summary>
            /// The yml separator
            /// </summary>
            public const char YmlSeparator = ':';

            /// <summary>
            /// The locales
            /// </summary>
            public static readonly string[] Locales = new string[] { "l_english", "l_braz_por", "l_french", "l_german", "l_polish", "l_russian", "l_simp_chinese", "l_spanish" };

            #endregion Fields
        }

        /// <summary>
        /// Class Scripts.
        /// </summary>
        public static class Scripts
        {
            #region Fields

            /// <summary>
            /// The closing bracket
            /// </summary>
            public const char ClosingBracket = '}';

            /// <summary>
            /// The definition separator identifier
            /// </summary>
            public const string DefinitionSeparatorId = VariableSeparatorId + "{";

            /// <summary>
            /// The graphics type name identifier
            /// </summary>
            public const string GraphicsTypeNameId = "name=";

            /// <summary>
            /// The GUI types
            /// </summary>
            public const string GuiTypes = "guiTypes";

            /// <summary>
            /// The GUI types identifier
            /// </summary>
            public const string GuiTypesId = GuiTypes + VariableSeparatorId;

            /// <summary>
            /// The namespace identifier
            /// </summary>
            public const string NamespaceId = "namespace=";

            /// <summary>
            /// The opening bracket
            /// </summary>
            public const char OpeningBracket = '{';

            /// <summary>
            /// The script comment identifier
            /// </summary>
            public const string ScriptCommentId = "#";

            /// <summary>
            /// The sprite type identifier
            /// </summary>
            public const string SpriteTypeId = "spriteType=";

            /// <summary>
            /// The sprite types
            /// </summary>
            public const string SpriteTypes = "spriteTypes =";

            /// <summary>
            /// The variable separator identifier
            /// </summary>
            public const string VariableSeparatorId = "=";

            /// <summary>
            /// The generic key flags
            /// </summary>
            public static readonly string[] GenericKeyFlags = new string[] { "id=", "name=", "key=" };

            /// <summary>
            /// The path trim parameters
            /// </summary>
            public static readonly char[] PathTrimParameters = new char[] { '\\', '/' };

            /// <summary>
            /// The separator operators
            /// </summary>
            public static readonly string[] SeparatorOperators = new string[] { VariableSeparatorId };

            #endregion Fields
        }

        /// <summary>
        /// Class Stellaris.
        /// </summary>
        public static class Stellaris
        {
            #region Fields

            /// <summary>
            /// The flags
            /// </summary>
            public const string Flags = "flags";

            /// <summary>
            /// The common root files
            /// </summary>
            public static readonly string[] CommonRootFiles = new string[] { MergePath("common", "message_types.txt"), MergePath("common", "alerts.txt") };

            /// <summary>
            /// The component tags
            /// </summary>
            public static readonly string ComponentTags = MergePath("common", "component_tags");

            /// <summary>
            /// The diplo phrases
            /// </summary>
            public static readonly string DiploPhrases = MergePath("common", "diplo_phrases");

            /// <summary>
            /// The map galaxy
            /// </summary>
            public static readonly string MapGalaxy = MergePath("map", "galaxy");

            /// <summary>
            /// The on actions flag
            /// </summary>
            public static readonly string OnActions = MergePath("common", "on_actions");

            /// <summary>
            /// The shader extensions
            /// </summary>
            public static readonly string[] ShaderExtensions = new string[] { ".shader", ".fxh" };

            /// <summary>
            /// The species names
            /// </summary>
            public static readonly string SpeciesNames = MergePath("common", "species_names");

            /// <summary>
            /// The start screen messages
            /// </summary>
            public static readonly string StartScreenMessages = MergePath("common", "start_screen_messages");

            /// <summary>
            /// The terraform
            /// </summary>
            public static readonly string Terraform = MergePath("common", "terraform");

            /// <summary>
            /// The weapon components
            /// </summary>
            public static readonly string WeaponComponents = MergePath("common", "component_templates", "weapon_components.csv");

            #endregion Fields

            #region Methods

            /// <summary>
            /// Merges the path.
            /// </summary>
            /// <param name="paths">The paths.</param>
            /// <returns>System.String.</returns>
            private static string MergePath(params string[] paths)
            {
                return string.Join(Path.DirectorySeparatorChar, paths);
            }

            #endregion Methods
        }

        #endregion Classes
    }
}
