// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-15-2020
// ***********************************************************************
// <copyright file="IModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using System.ComponentModel;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Interface IModel
    /// Implements the <see cref="System.ComponentModel.INotifyPropertyChanged" />
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public interface IModel : INotifyPropertyChanged, INotifyPropertyChanging
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
