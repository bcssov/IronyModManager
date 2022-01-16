// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-16-2022
// ***********************************************************************
// <copyright file="PlaysetsMods.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.IO.Mods.Models.Paradox.PropertyHandlers;
using RepoDb.Attributes;

namespace IronyModManager.IO.Mods.Models.Paradox.v4
{
    /// <summary>
    /// Class PlaysetsMods.
    /// </summary>
    [Map("playsets_mods")]
    internal class PlaysetsMods
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PlaysetsMods" /> is enabled.
        /// </summary>
        /// <value><c>null</c> if [enabled] contains no value, <c>true</c> if [enabled]; otherwise, <c>false</c>.</value>
        [Map("enabled"), PropertyHandler(typeof(ObjectToBoolHandler))]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the mod identifier.
        /// </summary>
        /// <value>The mod identifier.</value>
        [Map("modId")]
        public string ModId { get; set; }

        /// <summary>
        /// Gets or sets the playset identifier.
        /// </summary>
        /// <value>The playset identifier.</value>
        [Map("playsetId")]
        public string PlaysetId { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        [Map("position")]
        public int Position { get; set; }

        #endregion Properties
    }
}
