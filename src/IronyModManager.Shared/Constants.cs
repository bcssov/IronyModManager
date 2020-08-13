﻿// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 01-17-2020
//
// Last Modified By : Mario
// Last Modified On : 08-13-2020
// ***********************************************************************
// <copyright file="Constants.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class Constants.
    /// </summary>
    public class Constants
    {
        #region Fields

        /// <summary>
        /// The default application culture
        /// </summary>
        public const string DefaultAppCulture = "en";

        /// <summary>
        /// The descriptor file
        /// </summary>
        public const string DescriptorFile = "descriptor" + ModExtension;

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
        /// The image extensions
        /// </summary>
        public static readonly string[] ImageExtensions = new string[] { ".gif", ".jpg", ".jpeg", ".png", ".dds", ".tga", ".bmp", ".tiff", ".tif" };

        /// <summary>
        /// The text extensions
        /// </summary>
        public static readonly string[] TextExtensions = new string[] { ".lua", ".txt", ".asset", ".gui", ".gfx", ".yml", ".csv", ".shader", ".fxh", ".mod" };

        #endregion Fields

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
                public static Dictionary<string, string> Map = new Dictionary<string, string>() { { CurlyBracket, "}" } };

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
                public static string[] Map = new string[] { ColonSign };

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
            /// The log location
            /// </summary>
            public static readonly string LogLocation = "logs" + Path.DirectorySeparatorChar + "error.log";

            /// <summary>
            /// The launcher settings file name
            /// </summary>
            public static string LauncherSettingsFileName = "launcher-settings.json";

            #endregion Fields

            #region Classes

            /// <summary>
            /// Class EuropaUniversalis4.
            /// </summary>
            public static class EuropaUniversalis4
            {
                #region Fields

                /// <summary>
                /// The identifier
                /// </summary>
                public const string Id = "EuropaUniversalisIV";

                /// <summary>
                /// The docs path
                /// </summary>
                public const string DocsPath = "Europa Universalis IV";

                /// <summary>
                /// The steam application identifier
                /// </summary>
                public const int SteamAppId = 236850;

                /// <summary>
                /// The checksum folders
                /// </summary>
                public static readonly string[] ChecksumFolders = new string[] { "common", "events", "missions", "decisions", "history", "map" };

                /// <summary>
                /// The game folders
                /// </summary>
                public static readonly string[] GameFolders = new string[] { "common", "customizable_localization", "decisions", "events", "gfx", "hints",
                    "history", "interface", "localisation", "map", "missions", "music", "sound", "tutorial", "tweakergui_assets" };

                #endregion Fields
            }

            /// <summary>
            /// Class HeartsOfIron4.
            /// </summary>
            public static class HeartsOfIron4
            {
                #region Fields

                /// <summary>
                /// The identifier
                /// </summary>
                public const string Id = "HeartsofIronIV";

                /// <summary>
                /// The docs path
                /// </summary>
                public const string DocsPath = "Hearts of Iron IV";

                /// <summary>
                /// The steam application identifier
                /// </summary>
                public const int SteamAppId = 394360;

                /// <summary>
                /// The checksum folders
                /// </summary>
                public static readonly string[] ChecksumFolders = new string[] { "common", "events", "history", "map" };

                /// <summary>
                /// The game folders
                /// </summary>
                public static readonly string[] GameFolders = new string[] { "common", "events", "gfx", "history", "interface", "localisation",
                    "map", "music", "portraits", "previewer_assets", "script", "sound", "tutorial", "tweakergui_assets" };

                #endregion Fields
            }

            /// <summary>
            /// Class Stellaris.
            /// </summary>
            public static class Stellaris
            {
                #region Fields

                /// <summary>
                /// The identifier
                /// </summary>
                public const string Id = "Stellaris";

                /// <summary>
                /// The docs path
                /// </summary>
                public const string DocsPath = Id;

                /// <summary>
                /// The steam application identifier
                /// </summary>
                public const int SteamAppId = 281990;

                /// <summary>
                /// The checksum folders
                /// </summary>
                public static readonly string[] ChecksumFolders = new string[] { "common", "events", "map", "localisation_synced" };

                /// <summary>
                /// The game folders
                /// </summary>
                public static readonly string[] GameFolders = new string[] { "sound", "tweakergui_assets", "common", "crash_reporter", "dlc", "dlc_metadata",
                    "events", "flags", "fonts", "gfx", "interface", "launcher-assets", "licenses", "locales", "localisation", "localisation_synced", "map",
                    "music", "pdx_browser", "pdx_launcher", "pdx_online_assets", "prescripted_countries", "previewer_assets" };

                #endregion Fields
            }

            #endregion Classes
        }

        #endregion Classes
    }
}