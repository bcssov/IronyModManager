// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 03-04-2020
//
// Last Modified By : Mario
// Last Modified On : 03-04-2020
// ***********************************************************************
// <copyright file="PropertyChangedModelBase.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.ComponentModel;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class PropertyChangedModelBase.
    /// Implements the <see cref="IronyModManager.Shared.IPropertyChangedModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.IPropertyChangedModel" />
    public abstract class PropertyChangedModelBase : IPropertyChangedModel
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
