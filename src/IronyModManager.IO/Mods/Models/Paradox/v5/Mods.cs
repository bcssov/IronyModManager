// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-23-2023
//
// Last Modified By : Mario
// Last Modified On : 03-03-2023
// ***********************************************************************
// <copyright file="Mods.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Attributes;

namespace IronyModManager.IO.Mods.Models.Paradox.v5
{
    /// <summary>
    /// Class Mods.
    /// Implements the <see cref="IronyModManager.IO.Mods.Models.Paradox.v4.Mods" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Mods.Models.Paradox.v4.Mods" />
    internal class Mods : v4.Mods
    {
        #region Fields

        /// <summary>
        /// Gets or sets the description paradox.
        /// </summary>
        /// <value>The description paradox.</value>
        private string descParadox;

        /// <summary>
        /// The desc steam
        /// </summary>
        private string descSteam;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Map("descriptionDeprecated")]
        public override string Description { get => base.Description; set => base.Description = value; }

        /// <summary>
        /// Gets or sets the description paradox.
        /// </summary>
        /// <value>The description paradox.</value>
        [Map("descriptionPdx")]
        public string DescriptionParadox { get => !string.IsNullOrWhiteSpace(descParadox) ? descParadox : base.Description; set => descParadox = value; }

        /// <summary>
        /// Gets or sets the description steam.
        /// </summary>
        /// <value>The description steam.</value>
        [Map("descriptionSteam")]
        public string DescriptionSteam { get => !string.IsNullOrWhiteSpace(descSteam) ? descSteam : base.Description; set => descSteam = value; }

        #endregion Properties
    }
}
