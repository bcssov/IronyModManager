// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 01-16-2022
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
using System.Linq;
using IronyModManager.IO.Mods.Models.Paradox.PropertyHandlers;
using RepoDb.Attributes;

namespace IronyModManager.IO.Mods.Models.Paradox.v3
{
    /// <summary>
    /// Class Playsets.
    /// </summary>
    [Map("playsets")]
    internal class Playsets : Paradox.v2.Playsets
    {
        #region Properties

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        [Map("createdOn"), PropertyHandler(typeof(LongToDateTimeHandler))]
        public DateTime CreatedOn { get; set; }

        #endregion Properties
    }
}
