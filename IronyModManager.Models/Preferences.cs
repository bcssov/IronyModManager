// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-15-2020
// ***********************************************************************
// <copyright file="Preferences.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using System.ComponentModel;
using IronyModManager.Models.Common;

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
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
#pragma warning disable 67 // False detection

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

#pragma warning restore 67

        #region Properties

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>The theme.</value>
        public virtual Common.Enums.Theme Theme { get; set; }

        #endregion Properties
    }
}
