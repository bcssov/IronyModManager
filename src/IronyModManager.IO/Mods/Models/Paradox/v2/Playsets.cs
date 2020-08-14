// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 08-14-2020
// ***********************************************************************
// <copyright file="Playsets.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.IO.Mods.Models.Paradox.v2.PropertyHandlers;
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
        [Map("id"), PropertyHandler(typeof(StringToGuidHandler))] // TODO: It's a bug in the ORM mapper gotta lowercase property names temporarily
        public Guid id { get; set; }

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
