// ***********************************************************************
// Assembly         : IronyModManager.Storage.Common
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-20-2020
// ***********************************************************************
// <copyright file="IDatabase.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.ComponentModel;
using IronyModManager.Models.Common;

namespace IronyModManager.Storage.Common
{
    /// <summary>
    /// Interface IDatabase
    /// Implements the <see cref="System.ComponentModel.INotifyPropertyChanged" />
    /// Implements the <see cref="System.ComponentModel.INotifyPropertyChanging" />
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanging" />
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public interface IDatabase : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Properties

        /// <summary>
        /// Gets or sets the preferences.
        /// </summary>
        /// <value>The preferences.</value>
        IPreferences Preferences { get; set; }

        /// <summary>
        /// Gets or sets the state of the window.
        /// </summary>
        /// <value>The state of the window.</value>
        IWindowState WindowState { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        void OnPropertyChanged(string methodName);

        /// <summary>
        /// Called when [property changing].
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        void OnPropertyChanging(string methodName);

        #endregion Methods
    }
}
