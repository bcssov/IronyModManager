﻿// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-16-2022
// ***********************************************************************
// <copyright file="Playsets.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.IO.Mods.Models.Paradox.PropertyHandlers;
using RepoDb.Attributes;

namespace IronyModManager.IO.Mods.Models.Paradox.v2
{
    /// <summary>
    /// Class Playsets.
    /// </summary>
    [Map("playsets")]
    internal class Playsets
    {
        #region Properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [Map("id"), Primary]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>null</c> if [is active] contains no value, <c>true</c> if [is active]; otherwise, <c>false</c>.</value>
        [Map("isActive"), PropertyHandler(typeof(ObjectToBoolHandler))]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the load order.
        /// </summary>
        /// <value>The load order.</value>
        [Map("loadOrder")]
        public string LoadOrder { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Map("name")]
        public string Name { get; set; }

        #endregion Properties
    }
}
