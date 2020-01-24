// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-24-2020
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
using IronyModManager.Models.Common;
using IronyModManager.Storage.Common;
using Jot.Configuration.Attributes;

namespace IronyModManager.Storage
{
    /// <summary>
    /// Class Database.
    /// Implements the <see cref="IronyModManager.Storage.Common.IDatabase" />
    /// </summary>
    /// <seealso cref="IronyModManager.Storage.Common.IDatabase" />
    public class Database : IDatabase
    {
        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets the preferences.
        /// </summary>
        /// <value>The preferences.</value>
        [Trackable]
        public virtual IPreferences Preferences { get; set; } = DIResolver.Get<IPreferences>();

        /// <summary>
        /// Gets or sets the state of the window.
        /// </summary>
        /// <value>The state of the window.</value>
        [Trackable]
        public IWindowState WindowState { get; set; } = DIResolver.Get<IWindowState>();

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        public void OnPropertyChanged(string methodName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(methodName));
        }

        /// <summary>
        /// Called when [property changing].
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        public void OnPropertyChanging(string methodName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(methodName));
        }

        #endregion Methods
    }
}
