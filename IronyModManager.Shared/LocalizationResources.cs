// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 01-24-2020
//
// Last Modified By : Mario
// Last Modified On : 02-03-2020
// ***********************************************************************
// <copyright file="LocalizationResources.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class LocalizationResources.
    /// </summary>
    public static class LocalizationResources
    {
        #region Classes

        /// <summary>
        /// Class App.
        /// </summary>
        public static class App
        {
            #region Fields

            /// <summary>
            /// The prefix
            /// </summary>
            public const string Prefix = nameof(App) + ".";

            /// <summary>
            /// The title
            /// </summary>
            public const string Title = Prefix + "Title";

            #endregion Fields
        }

        /// <summary>
        /// Class FatalError.
        /// </summary>
        public static class FatalError
        {
            #region Fields

            /// <summary>
            /// The header
            /// </summary>
            public const string Header = Prefix + "Header";

            /// <summary>
            /// The message
            /// </summary>
            public const string Message = Prefix + "Message";

            /// <summary>
            /// The prefix
            /// </summary>
            public const string Prefix = nameof(FatalError) + ".";

            /// <summary>
            /// The title
            /// </summary>
            public const string Title = Prefix + "Title";

            #endregion Fields
        }

        /// <summary>
        /// Class Languages.
        /// </summary>
        public static class Languages
        {
            #region Fields

            /// <summary>
            /// The name
            /// </summary>
            public const string Name = Prefix + "Name";

            /// <summary>
            /// The prefix
            /// </summary>
            public const string Prefix = nameof(Languages) + ".";

            #endregion Fields
        }

        /// <summary>
        /// Class Themes.
        /// </summary>
        public static class Themes
        {
            #region Fields

            /// <summary>
            /// The dark
            /// </summary>
            public const string Dark = Prefix + "Dark";

            /// <summary>
            /// The light
            /// </summary>
            public const string Light = Prefix + "Light";

            /// <summary>
            /// The material dark
            /// </summary>
            public const string MaterialDark = Prefix + "MaterialDark";

            /// <summary>
            /// The material deep purple
            /// </summary>
            public const string MaterialDeepPurple = Prefix + "MaterialDarkPurple";

            /// <summary>
            /// The material light green
            /// </summary>
            public const string MaterialLightGreen = Prefix + "MaterialLightGreen";

            /// <summary>
            /// The name
            /// </summary>
            public const string Name = Prefix + "Name";

            /// <summary>
            /// The prefix
            /// </summary>
            public const string Prefix = nameof(Themes) + ".";

            /// <summary>
            /// The restart header
            /// </summary>
            public const string Restart_Header = Prefix + "Restart_Header";

            /// <summary>
            /// The restart message
            /// </summary>
            public const string Restart_Message = Prefix + "Restart_Message";

            /// <summary>
            /// The restart title
            /// </summary>
            public const string Restart_Title = Prefix + "Restart_Title";

            #endregion Fields
        }

        #endregion Classes
    }
}
