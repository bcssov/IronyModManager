// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 03-14-2021
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
    }
}
