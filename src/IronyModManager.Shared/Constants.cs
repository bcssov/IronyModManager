// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 01-17-2020
//
// Last Modified By : Mario
// Last Modified On : 02-23-2020
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
        /// The proxy identifier
        /// </summary>
        public const string ProxyIdentifier = "Proxy";

        /// <summary>
        /// The proxy namespace
        /// </summary>
        public const string ProxyNamespace = "Castle.Proxies";

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
            #region Fields

            /// <summary>
            /// The stellaris
            /// </summary>
            public const string Stellaris = "Stellaris";

            #endregion Fields
        }

        #endregion Classes
    }
}
