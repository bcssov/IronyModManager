// ***********************************************************************
// Assembly         : IronyModManager.Common
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-19-2020
// ***********************************************************************
// <copyright file="IViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Common.ViewModels
{
    /// <summary>
    /// Interface IViewModel
    /// </summary>
    public interface IViewModel
    {
        #region Properties

        /// <summary>
        /// Gets the actual type.
        /// </summary>
        /// <value>The actual type.</value>
        Type ActualType { get; }

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
