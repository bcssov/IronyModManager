// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 10-28-2022
// ***********************************************************************
// <copyright file="Constants.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager
{
    /// <summary>
    /// Class Constants.
    /// </summary>
    public class Constants
    {
        #region Fields

        /// <summary>
        /// The application cast address
        /// </summary>
        public const string AppCastAddress = "https://bcssov.github.io/IronyModManager/appcast.xml";

        /// <summary>
        /// The application settings
        /// </summary>
        public const string AppSettings = "appSettings.json";

        /// <summary>
        /// The application settings override
        /// </summary>
        public const string AppSettingsOverride = "appSettings.override.json";

        /// <summary>
        /// The last run location
        /// </summary>
        public const string LastRunLocation = "last-known-location.json";

        /// <summary>
        /// The localizations path
        /// </summary>
        public const string LocalizationsPath = "Localization";

        /// <summary>
        /// The public update key
        /// </summary>
        public const string PublicUpdateKey = "Oc2c/G6WMYkKL9+owAZYNIwMAMu9YqURiKw+gkY4zEw=";

        /// <summary>
        /// The update settings
        /// </summary>
        public const string UpdateSettings = "update-settings.json";

        /// <summary>
        /// The wiki URL
        /// </summary>
        public const string WikiUrl = "https://github.com/bcssov/IronyModManager/wiki";

        #endregion Fields

        #region Classes

        /// <summary>
        /// Class Resources.
        /// </summary>
        public class Resources
        {
            #region Fields

            /// <summary>
            /// The logo icon
            /// </summary>
            public const string LogoIco = "Assets\\logo.ico";

            /// <summary>
            /// The logo PNG
            /// </summary>
            public const string LogoPng = "Assets\\logo.png";

            /// <summary>
            /// The PDX script
            /// </summary>
            public const string PDXScriptDark = "Implementation\\AvaloniaEdit\\Resources\\Dark\\PDXScript.xshd";

            /// <summary>
            /// The PDX script light
            /// </summary>
            public const string PDXScriptLight = "Implementation\\AvaloniaEdit\\Resources\\Light\\PDXScript.xshd";

            /// <summary>
            /// The yaml
            /// </summary>
            public const string YAMLDark = "Implementation\\AvaloniaEdit\\Resources\\Dark\\YAML.xshd";

            /// <summary>
            /// The yaml light
            /// </summary>
            public const string YAMLLight = "Implementation\\AvaloniaEdit\\Resources\\Light\\YAML.xshd";

            #endregion Fields
        }

        #endregion Classes
    }
}
