// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 09-17-2020
// ***********************************************************************
// <copyright file="Constants.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;

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
        /// The localizations path
        /// </summary>
        public const string LocalizationsPath = "Localization";

        /// <summary>
        /// The public update key
        /// </summary>
        public const string PublicUpdateKey = "Oc2c/G6WMYkKL9+owAZYNIwMAMu9YqURiKw+gkY4zEw=";

        /// <summary>
        /// The unhandled error header
        /// </summary>
        public const string UnhandledErrorHeader = "Fatal Error";

        /// <summary>
        /// The error message
        /// </summary>
        public const string UnhandledErrorMessage = "Unhandled error occurred. App will close automatically.";

        /// <summary>
        /// The unhandled error title
        /// </summary>
        public const string UnhandledErrorTitle = "Error";

        /// <summary>
        /// The update settings
        /// </summary>
        public const string UpdateSettings = "update-settings.json";

        /// <summary>
        /// The wiki URL
        /// </summary>
        public const string WikiUrl = "https://github.com/bcssov/IronyModManager/wiki";

        /// <summary>
        /// The logs location
        /// </summary>
        public static readonly string LogsLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

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
            public const string PDXScript = "Implementation\\AvaloniaEdit\\Resources\\PDXScript.xshd";

            /// <summary>
            /// The yaml
            /// </summary>
            public const string YAML = "Implementation\\AvaloniaEdit\\Resources\\YAML.xshd";

            #endregion Fields
        }

        #endregion Classes
    }
}
