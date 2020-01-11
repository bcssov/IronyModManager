// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-11-2020
// ***********************************************************************
// <copyright file="Preferences.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using IronyModManager.Models.Enums;
using PropertyChanged;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class Preferences.
    /// Implements the <see cref="IronyModManager.Models.IPreferences" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.IPreferences" />
    [AddINotifyPropertyChangedInterface]
    internal class Preferences : IPreferences
    {
        #region Properties

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>The theme.</value>
        public virtual Theme Theme { get; set; }

        #endregion Properties
    }
}
