// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 01-17-2020
//
// Last Modified By : Mario
// Last Modified On : 03-09-2020
// ***********************************************************************
// <copyright file="Constants.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

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
        /// The empty parameter
        /// </summary>
        public const string EmptyParam = "";

        /// <summary>
        /// The json extension
        /// </summary>
        public const string JsonExtension = ".json";

        /// <summary>
        /// The plugins path and name
        /// </summary>
        public const string PluginsPathAndName = "Plugins";

        /// <summary>
        /// The proxy namespace
        /// </summary>
        public const string ProxyNamespace = "Castle.Proxies";

        /// <summary>
        /// The zip extension
        /// </summary>
        public const string ZipExtension = "." + ZipExtensionWithoutDot;

        /// <summary>
        /// The zip extension without dot
        /// </summary>
        public const string ZipExtensionWithoutDot = "zip";

        /// <summary>
        /// The text extensions
        /// </summary>
        public static readonly string[] TextExtensions = new string[] { ".lua", ".txt", ".asset", ".gui", ".gfx", ".yml", ".csv", ".shader", ".fxh" };

        #endregion Fields

        #region Classes

        /// <summary>
        /// Class GamesTypes.
        /// </summary>
        public static class GamesTypes
        {
            #region Classes

            /// <summary>
            /// Class Stellaris.
            /// </summary>
            public static class Stellaris
            {
                #region Fields

                /// <summary>
                /// The name
                /// </summary>
                public const string Name = "Stellaris";

                /// <summary>
                /// The steam application identifier
                /// </summary>
                public const int SteamAppId = 281990;

                #endregion Fields
            }

            #endregion Classes
        }

        #endregion Classes
    }
}
