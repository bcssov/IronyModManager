// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-15-2020
// ***********************************************************************
// <copyright file="Database.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.ComponentModel;
using IronyModManager.DI;
using IronyModManager.Models;
using Jot.Configuration.Attributes;

namespace IronyModManager.Storage
{
    /// <summary>
    /// Class Database.
    /// Implements the <see cref="IronyModManager.Storage.IDatabase" />
    /// Implements the <see cref="System.ComponentModel.INotifyPropertyChanged" />
    /// </summary>
    /// <seealso cref="IronyModManager.Storage.IDatabase" />
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class Database : IDatabase
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
        /// Gets or sets the preferences.
        /// </summary>
        /// <value>The preferences.</value>
        [Trackable]
        public virtual IPreferences Preferences { get; set; } = DIResolver.Get<IPreferences>();

        #endregion Properties
    }
}
