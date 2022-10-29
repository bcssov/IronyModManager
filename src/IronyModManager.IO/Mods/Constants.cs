// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2022
// ***********************************************************************
// <copyright file="Constants.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.IO.Mods
{
    /// <summary>
    /// Class Constants.
    /// </summary>
    internal class Constants
    {
        #region Fields

        /// <summary>
        /// The DLC load path
        /// </summary>
        public const string DLC_load_path = "dlc_load.json";

        /// <summary>
        /// The game data path
        /// </summary>
        public const string Game_data_path = "game_data.json";

        /// <summary>
        /// The mod registry path
        /// </summary>
        public const string Mod_registry_path = "mods_registry.json";

        /// <summary>
        /// The ready to play
        /// </summary>
        public const string Ready_to_play = "ready_to_play";

        /// <summary>
        /// The SQL database beta path
        /// </summary>
        public const string Sql_db_beta_path = "launcher-v2_openbeta.sqlite";

        /// <summary>
        /// The SQL database path
        /// </summary>
        public const string Sql_db_path = "launcher-v2.sqlite";

        /// <summary>
        /// The empty SQL database path
        /// </summary>
        public static readonly string Empty_sql_db_path = "Databases" + System.IO.Path.DirectorySeparatorChar + "empty_paradox_launcher.sqlite";

        #endregion Fields

        #region Classes

        /// <summary>
        /// Class SqlV5Id.
        /// </summary>
        public class SqlV3Id
        {
            #region Fields

            /// <summary>
            /// The SQL v5 identifier
            /// </summary>
            public const string Name = "addCreatedAndSubscribedDate";

            #endregion Fields
        }

        /// <summary>
        /// Class SqlV4Id.
        /// </summary>
        public class SqlV4Id
        {
            #region Fields

            /// <summary>
            /// The SQL v5 identifier
            /// </summary>
            public const string Name = "modifyPositionToInteger";

            #endregion Fields
        }

        #endregion Classes
    }
}
