// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-14-2020
// ***********************************************************************
// <copyright file="Preferences.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using System.ComponentModel;

/// <summary>
/// The Models namespace.
/// </summary>
namespace IronyModManager.Models
{
    /// <summary>
    /// Class Preferences.
    /// Implements the <see cref="IronyModManager.Models.IPreferences" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.IPreferences" />
    public class Preferences : IPreferences
    {
        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>The theme.</value>
        public virtual Enums.Theme Theme { get; set; }

        #endregion Properties
    }
}
