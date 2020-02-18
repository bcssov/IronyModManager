// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 02-18-2020
// ***********************************************************************
// <copyright file="Constants.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Class Constants.
    /// </summary>
    public static class Constants
    {
        #region Fields

        /// <summary>
        /// The text extensions
        /// </summary>
        public static readonly string[] TextExtensions = new string[] { ".txt", ".asset", ".gui", ".gfx", ".yml", ".csv", ".shader", ".fxh" };

        #endregion Fields

        #region Classes

        /// <summary>
        /// Class Scripts.
        /// </summary>
        public static class Scripts
        {
            #region Fields

            /// <summary>
            /// The closing bracket
            /// </summary>
            public const char ClosingBracket = '}';

            /// <summary>
            /// The definition separator
            /// </summary>
            public const string DefinitionSeparator = VariableSeparator + "{";

            /// <summary>
            /// The namespace
            /// </summary>
            public const string Namespace = "namespace=";

            /// <summary>
            /// The opening bracket
            /// </summary>
            public const char OpeningBracket = '{';

            /// <summary>
            /// The script comment
            /// </summary>
            public const string ScriptComment = "#";

            /// <summary>
            /// The variable separator
            /// </summary>
            public const string VariableSeparator = "=";

            /// <summary>
            /// The generic key flags
            /// </summary>
            public static readonly string[] GenericKeyFlags = new string[] { "id=", "name=", "key=" };

            /// <summary>
            /// The path trim parameters
            /// </summary>
            public static readonly char[] PathTrimParameters = new char[] { '\\', '/' };

            /// <summary>
            /// The separator operators
            /// </summary>
            public static readonly string[] SeparatorOperators = new string[] { VariableSeparator };

            #endregion Fields
        }

        /// <summary>
        /// Class Stellaris.
        /// </summary>
        public static class Stellaris
        {
            #region Fields

            /// <summary>
            /// The component tags
            /// </summary>
            public const string ComponentTags = "common\\component_tags";

            /// <summary>
            /// The flags
            /// </summary>
            public const string Flags = "flags";

            /// <summary>
            /// The map galaxy
            /// </summary>
            public const string MapGalaxy = "map\\galaxy";

            /// <summary>
            /// The on actions flag
            /// </summary>
            public const string OnActions = "common\\on_actions";

            /// <summary>
            /// The weapon components
            /// </summary>
            public const string WeaponComponents = "common\\component_templates\\weapon_components.csv";

            /// <summary>
            /// The common root files
            /// </summary>
            public static readonly string[] CommonRootFiles = new string[] { "common\\message_types.txt", "common\\alerts.txt" };

            #endregion Fields
        }

        #endregion Classes
    }
}
