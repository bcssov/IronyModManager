// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-11-2020
// ***********************************************************************
// <copyright file="Database.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.DI;
using IronyModManager.Models;
using Jot.Configuration.Attributes;
using PropertyChanged;

namespace IronyModManager.Storage
{
    /// <summary>
    /// Class Database.
    /// Implements the <see cref="IronyModManager.Storage.IDatabase" />
    /// </summary>
    /// <seealso cref="IronyModManager.Storage.IDatabase" />
    [AddINotifyPropertyChangedInterface]
    internal class Database : IDatabase
    {
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
