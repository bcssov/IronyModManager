// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 09-20-2020
//
// Last Modified By : Mario
// Last Modified On : 09-20-2020
// ***********************************************************************
// <copyright file="ContinueGame.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace IronyModManager.Services.Models
{
    /// <summary>
    /// Class ContinueGame.
    /// </summary>
    internal class ContinueGame
    {
        #region Properties

        /// <summary>
        /// Gets or sets the desc.
        /// </summary>
        /// <value>The desc.</value>
        [JsonProperty("filename")]
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [JsonProperty("title")]
        public string Title { get; set; }

        #endregion Properties
    }
}
