﻿// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 05-05-2025
// ***********************************************************************
// <copyright file="Constants.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IronyModManager.Parser.Common
{
    /// <summary>
    /// Class Constants.
    /// </summary>
    public static class Constants
    {
        #region Fields

        /// <summary>
        /// The asset extension
        /// </summary>
        public const string AssetExtension = ".asset";

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
        /// The store cache root folder
        /// </summary>
        public const string StoreCacheRootFolder = "StoreCache";

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
            /// The GFX
            /// </summary>
            // ReSharper disable once InconsistentNaming
            public const string GFX = "gfx";

            /// <summary>
            /// The history
            /// </summary>
            public const string History = "history";

            /// <summary>
            /// The map
            /// </summary>
            public const string Map = "map";

            /// <summary>
            /// The music
            /// </summary>
            public const string Music = "music";

            /// <summary>
            /// The script
            /// </summary>
            public const string Script = "script";

            /// <summary>
            /// The tests
            /// </summary>
            public const string Tests = "tests";

            /// <summary>
            /// The tutorial
            /// </summary>
            public const string Tutorial = "tutorial";

            /// <summary>
            /// The abilities
            /// </summary>
            public static readonly string Abilities = MergePath(CommonPath, "abilities");

            /// <summary>
            /// The aces
            /// </summary>
            public static readonly string Aces = MergePath(CommonPath, "aces");

            /// <summary>
            /// The ai areas
            /// </summary>
            public static readonly string AIAreas = MergePath(CommonPath, "ai_areas");

            /// <summary>
            /// The AI strategy
            /// </summary>
            public static readonly string AIStrategy = MergePath(CommonPath, "ai_strategy");

            /// <summary>
            /// The AI strategy planes
            /// </summary>
            public static readonly string AIStrategyPlanes = MergePath(CommonPath, "ai_strategy_plans");

            /// <summary>
            /// The bookmark
            /// </summary>
            public static readonly string Bookmark = MergePath(CommonPath, "bookmarks");

            /// <summary>
            /// The buildings
            /// </summary>
            public static readonly string Buildings = MergePath(CommonPath, "buildings");

            /// <summary>
            /// The characters
            /// </summary>
            public static readonly string Characters = MergePath(CommonPath, "characters");

            /// <summary>
            /// The countries
            /// </summary>
            public static readonly string Countries = MergePath(CommonPath, "countries");

            /// <summary>
            /// The country leader
            /// </summary>
            public static readonly string CountryLeader = MergePath(CommonPath, "country_leader");

            /// <summary>
            /// The country tags
            /// </summary>
            public static readonly string CountryTags = MergePath(CommonPath, "country_tags");

            /// <summary>
            /// The decisions
            /// </summary>
            public static readonly string Decisions = MergePath(CommonPath, "decisions");

            /// <summary>
            /// The difficulty settings
            /// </summary>
            public static readonly string DifficultySettings = MergePath(CommonPath, "difficulty_settings");

            /// <summary>
            /// The focus inlay windows
            /// </summary>
            public static readonly string FocusInlayWindows = MergePath(CommonPath, "focus_inlay_windows");

            /// <summary>
            /// The generation
            /// </summary>
            public static readonly string Generation = MergePath(CommonPath, "generation");

            /// <summary>
            /// The graphical culture type
            /// </summary>
            // ReSharper disable once StringLiteralTypo -- Please tell me resharper how should I rename a filename that is not under my control
            public static readonly string GraphicalCultureType = MergePath(CommonPath, "graphicalculturetype.txt");

            /// <summary>
            /// The ideas
            /// </summary>
            public static readonly string Ideas = MergePath(CommonPath, "ideas");

            /// <summary>
            /// The idea tags
            /// </summary>
            public static readonly string IdeaTags = MergePath(CommonPath, "idea_tags");

            /// <summary>
            /// The ideologies
            /// </summary>
            public static readonly string Ideologies = MergePath(CommonPath, "ideologies");

            /// <summary>
            /// The intelligence agencies
            /// </summary>
            public static readonly string IntelligenceAgencies = MergePath(CommonPath, "intelligence_agencies");

            /// <summary>
            /// The map modes
            /// </summary>
            public static readonly string MapModes = MergePath(CommonPath, "map_modes");

            /// <summary>
            /// The medals
            /// </summary>
            public static readonly string Medals = MergePath(CommonPath, "medals");

            /// <summary>
            /// The mio
            /// </summary>
            public static readonly string MIO = MergePath(CommonPath, "military_industrial_organization");

            /// <summary>
            /// The opinion modifiers
            /// </summary>
            public static readonly string OpinionModifiers = MergePath(CommonPath, "opinion_modifiers");

            /// <summary>
            /// The peace conference
            /// </summary>
            public static readonly string PeaceConference = MergePath(CommonPath, "peace_conference");

            /// <summary>
            /// The portraits
            /// </summary>
            public static readonly string Portraits = MergePath("portraits");

            /// <summary>
            /// The profile backgrounds
            /// </summary>
            public static readonly string ProfileBackgrounds = MergePath(CommonPath, "profile_backgrounds");

            /// <summary>
            /// The profile pictures
            /// </summary>
            public static readonly string ProfilePictures = MergePath(CommonPath, "profile_pictures");

            /// <summary>
            /// The raid categories
            /// </summary>
            public static readonly string Raids = MergePath(CommonPath, "raids");

            /// <summary>
            /// The resources
            /// </summary>
            public static readonly string Resources = MergePath(CommonPath, "resources");

            /// <summary>
            /// The ribbons
            /// </summary>
            public static readonly string Ribbons = MergePath(CommonPath, "ribbons");

            /// <summary>
            /// The scripted diplomatic actions
            /// </summary>
            public static readonly string ScriptedDiplomaticActions = MergePath(CommonPath, "scripted_diplomatic_actions");

            /// <summary>
            /// The scripted GUI
            /// </summary>
            public static readonly string ScriptedGui = MergePath(CommonPath, "scripted_guis");

            /// <summary>
            /// The special projects
            /// </summary>
            public static readonly string SpecialProjects = MergePath(CommonPath, "special_projects");

            /// <summary>
            /// The state categories
            /// </summary>
            public static readonly string StateCategories = MergePath(CommonPath, "state_category");

            /// <summary>
            /// The technologies
            /// </summary>
            public static readonly string Technologies = MergePath(CommonPath, "technologies");

            /// <summary>
            /// The terrain
            /// </summary>
            public static readonly string Terrain = MergePath(CommonPath, "terrain");

            /// <summary>
            /// The unit leader
            /// </summary>
            public static readonly string UnitLeader = MergePath(CommonPath, "unit_leader");

            /// <summary>
            /// The unit medals
            /// </summary>
            public static readonly string UnitMedals = MergePath(CommonPath, "unit_medals");

            /// <summary>
            /// The units
            /// </summary>
            public static readonly string Units = MergePath(CommonPath, "units");

            /// <summary>
            /// The wargoals
            /// </summary>
            public static readonly string Wargoals = MergePath(CommonPath, "wargoals");

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
            public static readonly string[] LocaleFolders = ["default", "english", "braz_por", "french", "german", "polish", "russian", "simp_chinese", "spanish", "chinese", "traditional_chinese", "japanese", "korean"];

            /// <summary>
            /// The locales
            /// </summary>
            public static readonly string[] Locales =
            [
                "l_default", "l_english", "l_braz_por", "l_french", "l_german", "l_polish", "l_russian", "l_simp_chinese", "l_spanish", "l_chinese", "l_traditional_chinese", "l_japanese", "l_korean"
            ];

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
            /// The placeholder file comment
            /// </summary>
            public const string PlaceholderFileComment = "# Irony this is a placeholder file please ignore it";

            /// <summary>
            /// The placeholder objects comment
            /// </summary>
            public const string PlaceholderObjectsComment = "# Irony these are placeholder objects please ignore them:";

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
            public static readonly char[] CodeTerminators = { OpenObject, CloseObject };

            /// <summary>
            /// The generic key ids
            /// </summary>
            public static readonly string[] GenericKeyIds = { "id=", "name=", "key=", "format=", "world=", "localization=" };

            /// <summary>
            /// The generic keys
            /// </summary>
            public static readonly string[] GenericKeys = { "id", "name", "key", "format", "world", "localization" };

            /// <summary>
            /// The inline operators
            /// </summary>
            public static readonly string[] InlineOperators = { "hsv", "rgb" };

            /// <summary>
            /// The namespace
            /// </summary>
            public static readonly string[] Namespaces = { "namespace", "add_namespace" };

            /// <summary>
            /// The operators
            /// </summary>
            public static readonly char[] Operators = { EqualsOperator, GreaterThanOperator, LowerThanOperator, NotEqualOperator };

            #endregion Fields
        }

        /// <summary>
        /// Class Stellaris.
        /// </summary>
        public static class Stellaris
        {
            #region Fields

            // ReSharper disable once IdentifierTypo,CommentTypo -- shut up
            /// <summary>
            /// The animsm extension
            /// </summary>
            public const string AnimsmExtension = ".animsm";

            /// <summary>
            /// The editor data extension
            /// </summary>
            public const string EditorDataExtension = ".editordata";

            /// <summary>
            /// The flags
            /// </summary>
            public const string Flags = "flags";

            /// <summary>
            /// The inline script identifier
            /// </summary>
            public const string InlineScriptId = "inline_script";

            /// <summary>
            /// The prescripted countries
            /// </summary>
            public const string PrescriptedCountries = "prescripted_countries";

            /// <summary>
            /// The sound
            /// </summary>
            public const string Sound = "sound";

            /// <summary>
            /// The unchecked defines
            /// </summary>
            public const string UncheckedDefines = "unchecked_defines";

            /// <summary>
            /// The buildings
            /// </summary>
            public static readonly string Buildings = MergePath(CommonPath, "buildings");

            /// <summary>
            /// The component tags
            /// </summary>
            public static readonly string ComponentTags = MergePath(CommonPath, "component_tags");

            /// <summary>
            /// The country container
            /// </summary>
            public static readonly string CountryContainer = MergePath(CommonPath, "country_container");

            /// <summary>
            /// The country types
            /// </summary>
            public static readonly string CountryTypes = MergePath(CommonPath, "country_types");

            /// <summary>
            /// The diplomacy economy
            /// </summary>
            public static readonly string DiplomacyEconomy = MergePath(CommonPath, "diplomacy_economy");

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
            /// The economic plans
            /// </summary>
            public static readonly string EconomicPlans = MergePath(CommonPath, "economic_plans");

            /// <summary>
            /// The ethics
            /// </summary>
            public static readonly string Ethics = MergePath(CommonPath, "ethics");

            /// <summary>
            /// The gamesetup settings
            /// </summary>
            public static readonly string GamesetupSettings = MergePath(CommonPath, "gamesetup_settings");

            /// <summary>
            /// The government authorities
            /// </summary>
            public static readonly string GovernmentAuthorities = MergePath(CommonPath, "governments", "authorities");

            /// <summary>
            /// The inline scripts
            /// </summary>
            public static readonly string InlineScripts = MergePath(CommonPath, "inline_scripts");

            /// <summary>
            /// The job tags
            /// </summary>
            public static readonly string JobTags = MergePath(CommonPath, "job_tags");

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
            /// The special projects
            /// </summary>
            public static readonly string SpecialProjects = MergePath(CommonPath, "special_projects");

            /// <summary>
            /// The species archetypes
            /// </summary>
            public static readonly string SpeciesArchetypes = MergePath(CommonPath, "species_archetypes");

            /// <summary>
            /// The species classes
            /// </summary>
            public static readonly string SpeciesClasses = MergePath(CommonPath, "species_classes");

            /// <summary>
            /// The species names
            /// </summary>
            public static readonly string SpeciesNames = MergePath(CommonPath, "species_names");

            /// <summary>
            /// The species rights
            /// </summary>
            public static readonly string SpeciesRights = MergePath(CommonPath, "species_rights");

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

            /// <summary>
            /// The trait tags
            /// </summary>
            public static readonly string TraitTags = MergePath(CommonPath, "trait_tags");

            #endregion Fields
        }

        #endregion Classes
    }
}
