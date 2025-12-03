// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 01-17-2020
//
// Last Modified By : Mario
// Last Modified On : 12-03-2025
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

// ReSharper disable InconsistentNaming
// ReSharper disable MemberHidesStaticFromOuterClass

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class Constants.
    /// </summary>
    public class Constants
    {
        #region Fields

        /// <summary>
        /// The bin extension
        /// </summary>
        public const string BinExtension = "." + BinExtensionWithoutDot;

        /// <summary>
        /// The bin extension without dot
        /// </summary>
        public const string BinExtensionWithoutDot = "bin";

        /// <summary>
        /// The default application culture
        /// </summary>
        public const string DefaultAppCulture = "en";

        /// <summary>
        /// The descriptor file
        /// </summary>
        public const string DescriptorFile = "descriptor" + ModExtension;

        /// <summary>
        /// The user dir
        /// </summary>
        public const string DescriptorUserDir = "user_dir";

        /// <summary>
        /// The empty parameter
        /// </summary>
        public const string EmptyParam = "";

        /// <summary>
        /// The json extension
        /// </summary>
        public const string JsonExtension = "." + JsonExtensionWithoutDot;

        /// <summary>
        /// The json extension without dot
        /// </summary>
        public const string JsonExtensionWithoutDot = "json";

        /// <summary>
        /// The replacement paths constant.
        /// </summary>
        public const string JsonMetadataReplacePaths = "replace_paths";

        /// <summary>
        /// The json mod directory
        /// </summary>
        public const string JsonModDirectory = "irony-mod";

        /// <summary>
        /// The localization directory
        /// </summary>
        public const string LocalizationDirectory = "localisation";

        /// <summary>
        /// The localization replace directory
        /// </summary>
        public const string LocalizationReplaceDirectory = "replace";

        /// <summary>
        /// The mod directory
        /// </summary>
        public const string ModDirectory = "mod";

        /// <summary>
        /// The mod extension
        /// </summary>
        public const string ModExtension = "." + ModDirectory;

        /// <summary>
        /// The mod name prefix override
        /// </summary>
        public const string ModNamePrefixOverride = "mod_name_prefix.txt";

        /// <summary>
        /// The plugins path and name
        /// </summary>
        public const string PluginsPathAndName = "Plugins";

        /// <summary>
        /// The proxy namespace
        /// </summary>
        public const string ProxyNamespace = "Castle.Proxies";

        /// <summary>
        /// The XML extension
        /// </summary>
        public const string XMLExtension = "." + XMLExtensionWithoutDot;

        /// <summary>
        /// The XML extension without dot
        /// </summary>
        public const string XMLExtensionWithoutDot = "xml";

        /// <summary>
        /// The zip extension
        /// </summary>
        public const string ZipExtension = "." + ZipExtensionWithoutDot;

        /// <summary>
        /// The zip extension without dot
        /// </summary>
        public const string ZipExtensionWithoutDot = "zip";

        /// <summary>
        /// The descriptor json metadata
        /// </summary>
        public static readonly string DescriptorJsonMetadata = ".metadata" + Path.DirectorySeparatorChar + "metadata.json";

        /// <summary>
        /// The image extensions
        /// </summary>
        public static readonly string[] ImageExtensions = [".gif", ".jpg", ".jpeg", ".png", ".dds", ".tga", ".bmp", ".tiff", ".tif"];

        /// <summary>
        /// The text extensions
        /// </summary>
        public static readonly string[] TextExtensions = [".lua", ".txt", ".asset", ".gui", ".gfx", ".yml", ".csv", ".shader", ".fxh", ".mod", ".sfx", ".json"];

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
        /// Class CodeSeparators.
        /// </summary>
        public static class CodeSeparators
        {
            #region Classes

            /// <summary>
            /// Class ClosingSeparators.
            /// </summary>
            public static class ClosingSeparators
            {
                #region Fields

                /// <summary>
                /// The curly bracket
                /// </summary>
                public const string CurlyBracket = "{";

                /// <summary>
                /// The map
                /// </summary>
                public static readonly Dictionary<string, string> Map = new() { { CurlyBracket, "}" } };

                #endregion Fields
            }

            /// <summary>
            /// Class NonClosingSeparators.
            /// </summary>
            public static class NonClosingSeparators
            {
                #region Fields

                /// <summary>
                /// The colon
                /// </summary>
                public const string ColonSign = ":";

                /// <summary>
                /// The map
                /// </summary>
                public static readonly string[] Map = { ColonSign };

                #endregion Fields
            }

            #endregion Classes
        }

        /// <summary>
        /// Class GamesTypes.
        /// </summary>
        public static class GamesTypes
        {
            #region Fields

            /// <summary>
            /// The default game binary path
            /// </summary>
            public static readonly string DefaultGameBinaryPath = string.Empty;

            /// <summary>
            /// The DLC container
            /// </summary>
            public static readonly string DLCContainer = string.Empty;

            /// <summary>
            /// The launcher settings file name
            /// </summary>
            public static readonly string LauncherSettingsFileName = "launcher-settings.json";

            /// <summary>
            /// The log location
            /// </summary>
            public static readonly string LogLocation = "logs" + Path.DirectorySeparatorChar + "error.log";

            /// <summary>
            /// The signature files
            /// </summary>
            public static readonly string[] SignatureFiles = [];

            #endregion Fields

            #region Classes

            /// <summary>
            /// Class CrusaderKings3.
            /// </summary>
            public static class CrusaderKings3
            {
                #region Fields

                /// <summary>
                /// The abrv
                /// </summary>
                public const string Abrv = "CK3";

                /// <summary>
                /// The docs path
                /// </summary>
                public const string DocsPath = "Crusader Kings III";

                /// <summary>
                /// The identifier
                /// </summary>
                public const string Id = "CrusaderKings3";

                /// <summary>
                /// The steam application identifier
                /// </summary>
                public const int SteamAppId = 1158310;

                /// <summary>
                /// The checksum folders
                /// </summary>
                public static readonly string[] ChecksumFolders = ["common", "events", "history", "map_data", "gui", "localization", "data_binding"];

                /// <summary>
                /// The DLC container
                /// </summary>
                public static readonly string DLCContainer = "game";

                /// <summary>
                /// The game folders
                /// </summary>
                public static readonly string[] GameFolders =
                [
                    "common", "content_source", "data_binding", "dlc", "dlc_metadata", "events", "fonts", "fx", "gfx", "gui", "history", "licenses", "localization", "map_data", "music", "notifications", "sound", "tests", "tools",
                    "tweakergui_assets", "reader_export"
                ];

                /// <summary>
                /// The launcher settings file name
                /// </summary>
                public static readonly string LauncherSettingsFileName = "launcher" + Path.DirectorySeparatorChar + "launcher-settings.json";

                /// <summary>
                /// The launcher settings prefix
                /// </summary>
                public static readonly string LauncherSettingsPrefix = ".." + Path.DirectorySeparatorChar;

                /// <summary>
                /// The paradox game identifier
                /// </summary>
                public static readonly string ParadoxGameId = "ck3";

                #endregion Fields
            }

            /// <summary>
            /// Class EuropaUniversalis4.
            /// </summary>
            public static class EuropaUniversalis4
            {
                #region Fields

                /// <summary>
                /// The abrv
                /// </summary>
                public const string Abrv = "EU4";

                /// <summary>
                /// The docs path
                /// </summary>
                public const string DocsPath = "Europa Universalis IV";

                /// <summary>
                /// The identifier
                /// </summary>
                public const string Id = "EuropaUniversalisIV";

                /// <summary>
                /// The steam application identifier
                /// </summary>
                public const int SteamAppId = 236850;

                /// <summary>
                /// The checksum folders
                /// </summary>
                public static readonly string[] ChecksumFolders = ["common", "events", "missions", "decisions", "history", "map"];

                /// <summary>
                /// The game folders
                /// </summary>
                public static readonly string[] GameFolders =
                [
                    "common", "customizable_localization", "decisions", "events", "gfx", "hints", "history", "interface", "localisation", "map", "missions", "music", "sound", "tutorial", "tweakergui_assets"
                ];

                /// <summary>
                /// The paradox game identifier
                /// </summary>
                public static readonly string ParadoxGameId = "eu4";

                #endregion Fields
            }

            /// <summary>
            /// Class EuropaUniversalis5.
            /// </summary>
            public static class EuropaUniversalis5
            {
                #region Fields

                /// <summary>
                /// The abrv
                /// </summary>
                public const string Abrv = "EU5";

                /// <summary>
                /// The docs path
                /// </summary>
                public const string DocsPath = "Europa Universalis V";

                /// <summary>
                /// The identifier
                /// </summary>
                public const string Id = "EuropaUniversalisV";

                /// <summary>
                /// The steam application identifier
                /// </summary>
                public const int SteamAppId = 3450310;

                /// <summary>
                /// The checksum folders
                /// </summary>
                public static readonly string[] ChecksumFolders =
                [
                    MergePath("in_game", "common"), MergePath("in_game", "events"),
                    MergePath("in_game", "map_data"), MergePath("in_game", "setup"),
                    MergePath("in_game", "gui"), MergePath("main_menu", "localization")
                ];

                /// <summary>
                /// The default Windows game binary path
                /// </summary>
                public static readonly string DefaultWindowsGameBinaryPath = MergePath("binaries", "eu5.exe");

                /// <summary>
                /// The DLC container
                /// </summary>
                public static readonly string DLCContainer = "game";

                /// <summary>
                /// The game folders
                /// </summary>
                public static readonly string[] GameFolders =
                [
                    "mod", "in_game", "loading_screen", "main_menu"
                ];

                /// <summary>
                /// The launcher settings file name
                /// </summary>
                public static readonly string LauncherSettingsFileName = string.Empty;

                /// <summary>
                /// The launcher settings prefix
                /// </summary>
                public static readonly string LauncherSettingsPrefix = string.Empty;

                /// <summary>
                /// The paradox game identifier
                /// </summary>
                public static readonly string ParadoxGameId = string.Empty;

                /// <summary>
                /// The signature files
                /// </summary>
                public static readonly string[] SignatureFiles = ["caesar_branch.txt", "clausewitz_branch.txt"];

                #endregion Fields
            }

            /// <summary>
            /// Class HeartsOfIron4.
            /// </summary>
            public static class HeartsOfIron4
            {
                #region Fields

                /// <summary>
                /// The abrv
                /// </summary>
                public const string Abrv = "HOI4";

                /// <summary>
                /// The docs path
                /// </summary>
                public const string DocsPath = "Hearts of Iron IV";

                /// <summary>
                /// The identifier
                /// </summary>
                public const string Id = "HeartsofIronIV";

                /// <summary>
                /// The steam application identifier
                /// </summary>
                public const int SteamAppId = 394360;

                /// <summary>
                /// The checksum folders
                /// </summary>
                public static readonly string[] ChecksumFolders = ["common", "events", "history", "map"];

                /// <summary>
                /// The game folders
                /// </summary>
                public static readonly string[] GameFolders = ["common", "events", "gfx", "history", "interface", "localisation", "map", "music", "portraits", "previewer_assets", "script", "sound", "tutorial", "tweakergui_assets"];

                /// <summary>
                /// The paradox game identifier
                /// </summary>
                public static readonly string ParadoxGameId = "hoi4";

                #endregion Fields
            }

            /// <summary>
            /// Class ImperatorRome.
            /// </summary>
            public static class ImperatorRome
            {
                #region Fields

                /// <summary>
                /// The abrv
                /// </summary>
                public const string Abrv = "IR";

                /// <summary>
                /// The docs path
                /// </summary>
                public const string DocsPath = "Imperator";

                /// <summary>
                /// The gog identifier
                /// </summary>
                public const int GogId = 2131232214;

                /// <summary>
                /// The identifier
                /// </summary>
                public const string Id = "ImperatorRome";

                /// <summary>
                /// The steam application identifier
                /// </summary>
                public const int SteamAppId = 859580;

                /// <summary>
                /// The checksum folders
                /// </summary>
                public static readonly string[] ChecksumFolders = ["common", "events", "decisions", "gui", "localization", "map_data", "setup"];

                /// <summary>
                /// The DLC container
                /// </summary>
                public static readonly string DLCContainer = "game";

                /// <summary>
                /// The game folders
                /// </summary>
                public static readonly string[] GameFolders = ["common", "content_source", "decisions", "events", "fonts", "gfx", "gui", "localization", "map_data", "music", "setup", "sound", "tutorial", "tweakergui_assets"];

                /// <summary>
                /// The launcher settings file name
                /// </summary>
                public static readonly string LauncherSettingsFileName = "launcher" + Path.DirectorySeparatorChar + "launcher-settings.json";

                /// <summary>
                /// The launcher settings prefix
                /// </summary>
                public static readonly string LauncherSettingsPrefix = ".." + Path.DirectorySeparatorChar;

                /// <summary>
                /// The paradox game identifier
                /// </summary>
                public static readonly string ParadoxGameId = "imperator_rome";

                #endregion Fields
            }

            /// <summary>
            /// Class Stellaris.
            /// </summary>
            public static class Stellaris
            {
                #region Fields

                /// <summary>
                /// The abrv
                /// </summary>
                public const string Abrv = "Stellaris";

                /// <summary>
                /// The docs path
                /// </summary>
                public const string DocsPath = Id;

                /// <summary>
                /// The gog identifier
                /// </summary>
                public const int GogId = 1508702879;

                /// <summary>
                /// The identifier
                /// </summary>
                public const string Id = "Stellaris";

                /// <summary>
                /// The steam application identifier
                /// </summary>
                public const int SteamAppId = 281990;

                /// <summary>
                /// The checksum folders
                /// </summary>
                public static readonly string[] ChecksumFolders = ["common", "events", "map", "localisation_synced"];

                /// <summary>
                /// The game folders
                /// </summary>
                public static readonly string[] GameFolders =
                [
                    "pdx_launcher", "pdx_online_assets", "prescripted_countries", "previewer_assets", "sound", "tools", "tweakergui_assets", "unchecked_defines", "common", "crash_reporter", "curated_save_games", "dlc", "dlc_metadata",
                    "events", "flags", "fonts", "gfx", "interface", "launcher-assets", "licenses", "locales", "localisation", "map", "music", "pdx_browser"
                ];

                /// <summary>
                /// The paradox game identifier
                /// </summary>
                public static readonly string ParadoxGameId = "stellaris";

                #endregion Fields
            }

            /// <summary>
            /// Class STInfinite.
            /// </summary>
            public static class STInfinite
            {
                #region Fields

                /// <summary>
                /// The abrv
                /// </summary>
                public const string Abrv = "STInfinite";

                /// <summary>
                /// The docs path
                /// </summary>
                public const string DocsPath = "Infinite";

                /// <summary>
                /// The identifier
                /// </summary>
                public const string Id = "STInfinite";

                /// <summary>
                /// The steam application identifier
                /// </summary>
                public const int SteamAppId = 1622900;

                /// <summary>
                /// The checksum folders
                /// </summary>
                public static readonly string[] ChecksumFolders = ["common", "events", "map", "localisation_synced"];

                /// <summary>
                /// The game folders
                /// </summary>
                public static readonly string[] GameFolders =
                [
                    "common", "crash_reporter", "dlc", "dlc_metadata", "events", "flags", "fonts", "gfx", "interface", "launcher-assets", "licenses", "locales", "localisation", "localisation_synced", "map", "music", "pdx_launcher",
                    "pdx_online_assets", "prescripted_countries", "previewer_assets", "sound", "tools", "tweakergui_assets"
                ];

                /// <summary>
                /// The paradox game identifier
                /// </summary>
                public static readonly string ParadoxGameId = "infinite";

                #endregion Fields
            }

            /// <summary>
            /// Class Victoria3.
            /// </summary>
            public static class Victoria3
            {
                #region Fields

                /// <summary>
                /// The abrv
                /// </summary>
                public const string Abrv = "Vic3";

                /// <summary>
                /// The docs path
                /// </summary>
                public const string DocsPath = "Victoria 3";

                /// <summary>
                /// The identifier
                /// </summary>
                public const string Id = "Victoria3";

                /// <summary>
                /// The steam application identifier
                /// </summary>
                public const int SteamAppId = 529340;

                /// <summary>
                /// The checksum folders
                /// </summary>
                public static readonly string[] ChecksumFolders = ["common", "events", "map_data", "gui", "localization"];

                /// <summary>
                /// The DLC container
                /// </summary>
                public static readonly string DLCContainer = "game";

                /// <summary>
                /// The game folders
                /// </summary>
                public static readonly string[] GameFolders =
                [
                    "common", "content_source", "data_binding", "dlc", "dlc_metadata", "events", "fonts", "gfx", "gui", "input_profile", "interface", "licenses", "localization", "map_data", "music", "notifications", "sound", "soundtrack",
                    "tools"
                ];

                /// <summary>
                /// The launcher settings file name
                /// </summary>
                public static readonly string LauncherSettingsFileName = "launcher" + Path.DirectorySeparatorChar + "launcher-settings.json";

                /// <summary>
                /// The launcher settings prefix
                /// </summary>
                public static readonly string LauncherSettingsPrefix = ".." + Path.DirectorySeparatorChar;

                /// <summary>
                /// The paradox game identifier
                /// </summary>
                public static readonly string ParadoxGameId = "victoria3";

                #endregion Fields
            }

            #endregion Classes
        }

        #endregion Classes
    }
}
