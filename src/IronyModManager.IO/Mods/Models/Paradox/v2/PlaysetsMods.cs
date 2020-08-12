// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 08-12-2020
// ***********************************************************************
// <copyright file="PlaysetsMods.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using RepoDb.Attributes;

namespace IronyModManager.IO.Mods.Models.Paradox.v2
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
        [Map("enabled")]
        public bool? Enabled { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [Map("modId")] // TODO: It's a bug in the ORM mapper gotta lowercase property names temporarily. Also changed the type to string which is another reported issue when updating records using composite keys and property handler.
        public string modId { get; set; }

        /// <summary>
        /// Gets or sets the playset identifier.
        /// </summary>
        /// <value>The playset identifier.</value>
        [Map("playsetId")] // TODO: It's a bug in the ORM mapper gotta lowercase property names temporarily. Also changed the type to string which is another reported issue when updating records using composite keys and property handler.
        public string playsetId { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        [Map("position")]
        public string Position { get; set; }

        #endregion Properties
    }
}
