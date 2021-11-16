// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 11-16-2021
//
// Last Modified By : Mario
// Last Modified On : 11-16-2021
// ***********************************************************************
// <copyright file="KnoxMigrations.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Attributes;

namespace IronyModManager.IO.Mods.Models.Paradox.v2
{
    /// <summary>
    /// Class KnoxMigrations.
    /// </summary>
    [Map("knex_migrations")]
    internal class KnoxMigrations
    {
        #region Properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [Map("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Map("name")]
        public string Name { get; set; }

        #endregion Properties
    }
}
