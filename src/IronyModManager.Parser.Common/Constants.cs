// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 06-30-2020
// ***********************************************************************
// <copyright file="Constants.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;

namespace IronyModManager.Parser.Common
{
    /// <summary>
    /// Class Constants.
    /// </summary>
    public static class Constants
    {
        #region Fields

        /// <summary>
        /// The binary type
        /// </summary>
        public const string BinaryType = "binary";

        /// <summary>
        /// The common path
        /// </summary>
        public const string CommonPath = "common";

        /// <summary>
        /// The FXH extension
        /// </summary>
        public const string FxhExtension = ".fxh";

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
        /// The shader extensions
        /// </summary>
        public const string ShaderExtension = ".shader";

        /// <summary>
        /// The text type
        /// </summary>
        public const string TxtType = "txt";

        /// <summary>
        /// The yml type
        /// </summary>
        public const string YmlType = "yml";

        /// <summary>
        /// The defines path
        /// </summary>
        public static readonly string DefinesPath = MergePath(CommonPath, "defines");

        /// <summary>
        /// The on actions path
        /// </summary>
        public static readonly string OnActionsPath = MergePath(CommonPath, "on_actions");

        /// <summary>
        /// The parser map path
        /// </summary>
        public static readonly string ParserMapPath = MergePath("Maps", "{0}ParserMap" + Shared.Constants.JsonExtension);

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
            public static readonly string[] Locales = new string[] { "l_default", "l_english", "l_braz_por", "l_french", "l_german", "l_polish", "l_russian", "l_simp_chinese", "l_spanish", "l_chinese", "l_traditional_chinese" };

            #endregion Fields
        }

        /// <summary>
        /// Class Scripts.
        /// </summary>
        public static class Scripts
        {
            #region Fields

            /// <summary>
            /// The bitmap fonts identifier
            /// </summary>
            public const string BitmapFontsId = "bitmapfonts" + VariableSeparatorId;

            /// <summary>
            /// The closing bracket
            /// </summary>
            public const char ClosingBracket = '}';

            /// <summary>
            /// The definition separator identifier
            /// </summary>
            public const string DefinitionSeparatorId = VariableSeparatorId + "{";

            /// <summary>
            /// The fallback to simple parser comment
            /// </summary>
            public const string FallbackToSimpleParserComment = "# Dear Irony please fallback to simple parser";

            /// <summary>
            /// The graphics type name
            /// </summary>
            public const string GraphicsTypeName = "name";

            /// <summary>
            /// The GUI types
            /// </summary>
            public const string GuiTypes = "guiTypes";

            /// <summary>
            /// The GUI types identifier
            /// </summary>
            public const string GuiTypesId = GuiTypes + VariableSeparatorId;

            /// <summary>
            /// The languages identifier
            /// </summary>
            public const string LanguagesId = "languages";

            /// <summary>
            /// The namespace
            /// </summary>
            public const string Namespace = "namespace";

            /// <summary>
            /// The namespace identifier
            /// </summary>
            public const string NamespaceId = Namespace + VariableSeparatorId;

            /// <summary>
            /// The object types
            /// </summary>
            public const string ObjectTypes = "objectTypes";

            /// <summary>
            /// The object types identifier
            /// </summary>
            public const string ObjectTypesId = "objectTypes" + VariableSeparatorId;

            /// <summary>
            /// The opening bracket
            /// </summary>
            public const char OpeningBracket = '{';

            /// <summary>
            /// The position type identifier
            /// </summary>
            public const string PositionTypeId = "positionType" + VariableSeparatorId;

            /// <summary>
            /// The script comment identifier
            /// </summary>
            public const string ScriptCommentId = "#";

            /// <summary>
            /// The sprite types
            /// </summary>
            public const string SpriteTypes = "spriteTypes";

            /// <summary>
            /// The sprite types identifier
            /// </summary>
            public const string SpriteTypesId = "spriteTypes" + VariableSeparatorId;

            /// <summary>
            /// The variable prefix
            /// </summary>
            public const char VariablePrefix = '@';

            /// <summary>
            /// The variable separator identifier
            /// </summary>
            public const string VariableSeparatorId = "=";

            /// <summary>
            /// The generic key ids
            /// </summary>
            public static readonly string[] GenericKeyIds = new string[] { "id=", "name=", "key=", "format=", "world=", "localization=" };

            /// <summary>
            /// The generic keys
            /// </summary>
            public static readonly string[] GenericKeys = new string[] { "id", "name", "key", "format", "world", "localization" };

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
            /// The sound
            /// </summary>
            public const string Sound = "sound";

            /// <summary>
            /// The scripted variables
            /// </summary>
            public static readonly string ScriptedVariables = MergePath(CommonPath, "scripted_variables");

            /// <summary>
            /// The component tags
            /// </summary>
            public static readonly string ComponentTags = MergePath(CommonPath, "component_tags");

            /// <summary>
            /// The diplo phrases
            /// </summary>
            public static readonly string DiploPhrases = MergePath(CommonPath, "diplo_phrases");

            /// <summary>
            /// The districts
            /// </summary>
            public static readonly string Districts = MergePath(CommonPath, "districts");

            /// <summary>
            /// The map galaxy
            /// </summary>
            public static readonly string MapGalaxy = MergePath("map", "galaxy");

            /// <summary>
            /// The name lists
            /// </summary>
            public static readonly string NameLists = MergePath(CommonPath, "name_lists");

            /// <summary>
            /// The planet classes
            /// </summary>
            public static readonly string PlanetClasses = MergePath(CommonPath, "planet_classes");

            /// <summary>
            /// The pop jobs
            /// </summary>
            public static readonly string PopJobs = MergePath(CommonPath, "pop_jobs");

            /// <summary>
            /// The portraits
            /// </summary>
            public static readonly string Portraits = MergePath("gfx", "portraits", "portraits");

            /// <summary>
            /// The random names
            /// </summary>
            public static readonly string RandomNamesBase = MergePath(CommonPath, "random_names", "base");

            /// <summary>
            /// The solar system initializers
            /// </summary>
            public static readonly string SolarSystemInitializers = MergePath(CommonPath, "solar_system_initializers");

            /// <summary>
            /// The species names
            /// </summary>
            public static readonly string SpeciesNames = MergePath(CommonPath, "species_names");

            /// <summary>
            /// The terraform
            /// </summary>
            public static readonly string Terraform = MergePath(CommonPath, "terraform");

            /// <summary>
            /// The traits
            /// </summary>
            public static readonly string Traits = MergePath(CommonPath, "traits");

            /// <summary>
            /// The weapon components
            /// </summary>
            public static readonly string WeaponComponents = MergePath(CommonPath, "component_templates", "weapon_components.csv");

            #endregion Fields
        }

        #endregion Classes
    }
}
