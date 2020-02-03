// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 02-03-2020
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
        /// The localizations path
        /// </summary>
        public const string LocalizationsPath = "Localization";

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
            public const string LogoIco = "Assets\\avalonia-logo.ico";

            /// <summary>
            /// The logo PNG
            /// </summary>
            public const string LogoPng = "Assets\\avalonia-logo.png";

            #endregion Fields
        }

        #endregion Classes
    }
}
