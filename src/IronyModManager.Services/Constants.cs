// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 03-12-2021
// ***********************************************************************
// <copyright file="Constants.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class Constants.
    /// </summary>
    public static class Constants
    {
        #region Fields

        /// <summary>
        /// The paradox mod identifier
        /// </summary>
        public const string Paradox_mod_id = "pdx_";

        /// <summary>
        /// The paradox URL
        /// </summary>
        public const string Paradox_Url = "https://mods.paradoxplaza.com/mods/{0}/Any";

        /// <summary>
        /// The steam mod identifier
        /// </summary>
        public const string Steam_mod_id = "ugc_";

        /// <summary>
        /// The steam protocol URI
        /// </summary>
        public const string Steam_protocol_uri = "steam://openurl/{0}";

        /// <summary>
        /// The steam URL
        /// </summary>
        public const string Steam_Url = "https://steamcommunity.com/sharedfiles/filedetails/?id={0}";

        #endregion Fields

        #region Classes

        /// <summary>
        /// Class Themes.
        /// </summary>
        public static class Themes
        {
            #region Classes

            /// <summary>
            /// Class Dark.
            /// </summary>
            public static class Dark
            {
                #region Fields

                /// <summary>
                /// The accent resource
                /// </summary>
                public const string AccentResource = "avares://Avalonia.Themes.Default/Accents/BaseDark.xaml";

                /// <summary>
                /// The main resource
                /// </summary>
                public const string MainResource = "avares://Avalonia.Themes.Default/DefaultTheme.xaml";

                /// <summary>
                /// The name
                /// </summary>
                public const string Name = "Dark";

                /// <summary>
                /// The override theme
                /// </summary>
                public const string OverrideTheme = "avares://IronyModManager/Controls/Themes/Dark/ThemeOverride.xaml";

                #endregion Fields
            }

            /// <summary>
            /// Class FluentDark.
            /// </summary>
            public static class FluentDark
            {
                #region Fields

                /// <summary>
                /// The compact
                /// </summary>
                public const string Compact = "avares://Avalonia.Themes.Fluent/DensityStyles/Compact.xaml";

                /// <summary>
                /// The main resource
                /// </summary>
                public const string MainResource = "avares://Avalonia.Themes.Fluent/FluentDark.xaml";

                /// <summary>
                /// The name
                /// </summary>
                public const string Name = "FluentDark";

                /// <summary>
                /// The override theme
                /// </summary>
                public const string OverrideTheme = "avares://IronyModManager/Controls/Themes/FluentDark/ThemeOverride.axaml";

                #endregion Fields
            }

            /// <summary>
            /// Class FluentLight.
            /// </summary>
            public static class FluentLight
            {
                #region Fields

                /// <summary>
                /// The compact
                /// </summary>
                public const string Compact = "avares://Avalonia.Themes.Fluent/DensityStyles/Compact.xaml";

                /// <summary>
                /// The main resource
                /// </summary>
                public const string MainResource = "avares://Avalonia.Themes.Fluent/FluentLight.xaml";

                /// <summary>
                /// The name
                /// </summary>
                public const string Name = "FluentLight";

                /// <summary>
                /// The override theme
                /// </summary>
                public const string OverrideTheme = "avares://IronyModManager/Controls/Themes/FluentLight/ThemeOverride.axaml";

                #endregion Fields
            }

            /// <summary>
            /// Class Light.
            /// </summary>
            public static class Light
            {
                #region Fields

                /// <summary>
                /// The accent resource
                /// </summary>
                public const string AccentResource = "avares://Avalonia.Themes.Default/Accents/BaseLight.xaml";

                /// <summary>
                /// The main resource
                /// </summary>
                public const string MainResource = "avares://Avalonia.Themes.Default/DefaultTheme.xaml";

                /// <summary>
                /// The name
                /// </summary>
                public const string Name = "Light";

                /// <summary>
                /// The override theme
                /// </summary>
                public const string OverrideTheme = "avares://IronyModManager/Controls/Themes/Light/ThemeOverride.xaml";

                #endregion Fields
            }

            #endregion Classes
        }

        #endregion Classes
    }
}
