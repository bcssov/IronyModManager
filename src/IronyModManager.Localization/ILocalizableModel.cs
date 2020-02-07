// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-21-2020
//
// Last Modified By : Mario
// Last Modified On : 02-05-2020
// ***********************************************************************
// <copyright file="ILocalizableModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.ComponentModel;

namespace IronyModManager.Localization
{
    /// <summary>
    /// Interface ILocalizableModel
    /// Implements the <see cref="System.ComponentModel.INotifyPropertyChanging" />
    /// Implements the <see cref="System.ComponentModel.INotifyPropertyChanged" />
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanging" />
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public interface ILocalizableModel : INotifyPropertyChanging, INotifyPropertyChanged
    {
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
