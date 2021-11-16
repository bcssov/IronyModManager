// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 11-16-2021
// ***********************************************************************
// <copyright file="Playsets.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.IO.Mods.Models.Paradox.v2.PropertyHandlers;
using IronyModManager.IO.Mods.Models.Paradox.v3.PropertyHandlers;
using RepoDb.Attributes;

namespace IronyModManager.IO.Mods.Models.Paradox.v3
{
    /// <summary>
    /// Class Playsets.
    /// </summary>
    [Map("playsets")]
    internal class Playsets
    {
        #region Properties

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        [Map("createdOn"), PropertyHandler(typeof(LongToDateTimeHandler))]
        public DateTime CreatedOn { get; set; }

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
        /// Gets or sets a value indicating whether this instance is removed.
        /// </summary>
        /// <value><c>true</c> if this instance is removed; otherwise, <c>false</c>.</value>
        [Map("isRemoved"), PropertyHandler(typeof(ObjectToBoolHandler))]
        public bool IsRemoved { get; set; }

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
