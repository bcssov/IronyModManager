// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2022
// ***********************************************************************
// <copyright file="Constants.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
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
        /// The CSV extension
        /// </summary>
        public const string CsvExtension = ".csv";

        /// <summary>
        /// The empty lua overwrite comment
        /// </summary>
        public const string EmptyLuaOverwriteComment = "-- This mod contains empty code. Possibly to overwrite other mods.";

        /// <summary>
        /// The empty overwrite comment
        /// </summary>
        public const string EmptyOverwriteComment = "# This mod contains empty code. Possibly to overwrite other mods.";

        /// <summary>
        /// The empty shader comment
        /// </summary>
        public const string EmptyShaderComment = "// This mod contains empty code. Possibly to overwrite other mods.";

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
        /// The lua extension
        /// </summary>
        public const string LuaExtension = ".lua";

        /// <summary>
        /// The shader extensions
        /// </summary>
        public const string ShaderExtension = ".shader";

        /// <summary>
        /// The text extension
        /// </summary>
        public const string TxtExtension = "." + TxtType;

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
        /// Class HOI4.
        /// </summary>
        public static class HOI4
        {
            #region Fields

            /// <summary>
            /// The map
            /// </summary>
            public const string Map = "map";

            /// <summary>
            /// The country tags
            /// </summary>
            public static readonly string CountryTags = MergePath(CommonPath, "country_tags");

            /// <summary>
            /// The graphical culture type
            /// </summary>
            public static readonly string GraphicalCultureType = MergePath(CommonPath, "graphicalculturetype.txt");

            #endregion Fields
        }

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
            /// The locale folders
            /// </summary>
            public static readonly string[] LocaleFolders = new string[] { "default", "english", "braz_por", "french", "german", "polish", "russian", "simp_chinese", "spanish", "chinese", "traditional_chinese" };

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
            /// The closing bracket
            /// </summary>
            public const char CloseObject = '}';

            /// <summary>
            /// The definition separator identifier
            /// </summary>
            public const string DefinitionSeparatorId = "={";

            /// <summary>
            /// The variable separator identifier
            /// </summary>
            public const char EqualsOperator = '=';

            /// <summary>
            /// The escape character
            /// </summary>
            public const char EscapeCharacter = '\\';

            /// <summary>
            /// The fallback to simple parser comment
            /// </summary>
            public const string FallbackToSimpleParserComment = "# Dear Irony please fallback to simple parser";

            /// <summary>
            /// The graphics type name
            /// </summary>
            public const string GraphicsTypeName = "name";

            /// <summary>
            /// The greater than operator
            /// </summary>
            public const char GreaterThanOperator = '>';

            /// <summary>
            /// The languages identifier
            /// </summary>
            public const string LanguagesId = "languages";

            /// <summary>
            /// The lower than operator
            /// </summary>
            public const char LowerThanOperator = '<';

            /// <summary>
            /// The lua script comment identifier
            /// </summary>
            public const string LuaScriptCommentId = "--";

            /// <summary>
            /// The not equal operator
            /// </summary>
            public const char NotEqualOperator = '!';

            /// <summary>
            /// The opening bracket
            /// </summary>
            public const char OpenObject = '{';

            /// <summary>
            /// The quote
            /// </summary>
            public const char Quote = '"';

            /// <summary>
            /// The script comment identifier
            /// </summary>
            public const char ScriptCommentId = '#';

            /// <summary>
            /// The square close bracket
            /// </summary>
            public const char SquareCloseBracket = ']';

            /// <summary>
            /// The square open bracket
            /// </summary>
            public const char SquareOpenBracket = '[';

            /// <summary>
            /// The variable prefix
            /// </summary>
            public const char VariableId = '@';

            /// <summary>
            /// The terminators
            /// </summary>
            public static readonly char[] CodeTerminators = new char[] { OpenObject, CloseObject };

            /// <summary>
            /// The generic key ids
            /// </summary>
            public static readonly string[] GenericKeyIds = new string[] { "id=", "name=", "key=", "format=", "world=", "localization=" };

            /// <summary>
            /// The generic keys
            /// </summary>
            public static readonly string[] GenericKeys = new string[] { "id", "name", "key", "format", "world", "localization" };

            /// <summary>
            /// The inline operators
            /// </summary>
            public static readonly string[] InlineOperators = new string[] { "hsv", "rgb" };

            /// <summary>
            /// The namespace
            /// </summary>
            public static readonly string[] Namespaces = new string[] { "namespace", "add_namespace" };

            /// <summary>
            /// The operators
            /// </summary>
            public static readonly char[] Operators = new char[] { EqualsOperator, GreaterThanOperator, LowerThanOperator, NotEqualOperator };

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
            /// The prescripted countries
            /// </summary>
            public const string PrescriptedCountries = "prescripted_countries";

            /// <summary>
            /// The sound
            /// </summary>
            public const string Sound = "sound";

            /// <summary>
            /// The buildings
            /// </summary>
            public static readonly string Buildings = MergePath(CommonPath, "buildings");

            /// <summary>
            /// The component tags
            /// </summary>
            public static readonly string ComponentTags = MergePath(CommonPath, "component_tags");

            /// <summary>
            /// The country types
            /// </summary>
            public static readonly string CountryTypes = MergePath(CommonPath, "country_types");

            /// <summary>
            /// The diplomatic actions
            /// </summary>
            public static readonly string DiplomaticActions = MergePath(CommonPath, "diplomatic_actions");

            /// <summary>
            /// The diplo phrases
            /// </summary>
            public static readonly string DiploPhrases = MergePath(CommonPath, "diplo_phrases");

            /// <summary>
            /// The districts
            /// </summary>
            public static readonly string Districts = MergePath(CommonPath, "districts");

            /// <summary>
            /// The ethics
            /// </summary>
            public static readonly string Ethics = MergePath(CommonPath, "ethics");

            /// <summary>
            /// The government authorities
            /// </summary>
            public static readonly string GovernmentAuthorities = MergePath(CommonPath, "governments", "authorities");

            /// <summary>
            /// The map galaxy
            /// </summary>
            public static readonly string MapGalaxy = MergePath("map", "galaxy");

            /// <summary>
            /// The map setup scenarios
            /// </summary>
            public static readonly string MapSetupScenarios = MergePath("map", "setup_scenarios");

            /// <summary>
            /// The name lists
            /// </summary>
            public static readonly string NameLists = MergePath(CommonPath, "name_lists");

            /// <summary>
            /// The opinion modifiers
            /// </summary>
            public static readonly string OpinionModifiers = MergePath(CommonPath, "opinion_modifiers");

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
            public static readonly string RandomNames = MergePath(CommonPath, "random_names");

            /// <summary>
            /// The random names
            /// </summary>
            public static readonly string RandomNamesBase = MergePath(CommonPath, "random_names", "base");

            /// <summary>
            /// The relics
            /// </summary>
            public static readonly string Relics = MergePath(CommonPath, "relics");

            /// <summary>
            /// The scripted variables
            /// </summary>
            public static readonly string ScriptedVariables = MergePath(CommonPath, "scripted_variables");

            /// <summary>
            /// The section templates
            /// </summary>
            public static readonly string SectionTemplates = MergePath(CommonPath, "section_templates");

            /// <summary>
            /// The ship sizes
            /// </summary>
            public static readonly string ShipSizes = MergePath(CommonPath, "ship_sizes");

            /// <summary>
            /// The solar system initializers
            /// </summary>
            public static readonly string SolarSystemInitializers = MergePath(CommonPath, "solar_system_initializers");

            /// <summary>
            /// The species archetypes
            /// </summary>
            public static readonly string SpeciesArchetypes = MergePath(CommonPath, "species_archetypes");

            /// <summary>
            /// The species names
            /// </summary>
            public static readonly string SpeciesNames = MergePath(CommonPath, "species_names");

            /// <summary>
            /// The starbase modules
            /// </summary>
            public static readonly string StarbaseModules = MergePath(CommonPath, "starbase_modules");

            /// <summary>
            /// The start screen messages
            /// </summary>
            public static readonly string StartScreenMessages = MergePath(CommonPath, "start_screen_messages");

            /// <summary>
            /// The strategic resources
            /// </summary>
            public static readonly string StrategicResources = MergePath(CommonPath, "strategic_resources");

            /// <summary>
            /// The technology
            /// </summary>
            public static readonly string Technology = MergePath(CommonPath, "technology");

            /// <summary>
            /// The terraform
            /// </summary>
            public static readonly string Terraform = MergePath(CommonPath, "terraform");

            /// <summary>
            /// The traits
            /// </summary>
            public static readonly string Traits = MergePath(CommonPath, "traits");

            #endregion Fields
        }

        #endregion Classes
    }
}
