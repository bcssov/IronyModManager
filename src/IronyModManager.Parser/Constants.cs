// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 02-17-2020
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
        /// The generic event flag
        /// </summary>
        public const string GenericEventFlag = "events";

        /// <summary>
        /// The generic parser flag
        /// </summary>
        public const string GenericParserFlag = "*";

        /// <summary>
        /// The stellaris on actions flag
        /// </summary>
        public const string StellarisOnActionsFlag = "common\\on_actions";

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
            /// The event identifier
            /// </summary>
            public const string EventId = "id" + VariableSeparator;

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
            /// The path trim parameters
            /// </summary>
            public static readonly char[] PathTrimParameters = new char[] { '\\', '/' };

            /// <summary>
            /// The separator operators
            /// </summary>
            public static readonly string[] SeparatorOperators = new string[] { VariableSeparator };

            #endregion Fields
        }

        #endregion Classes
    }
}
