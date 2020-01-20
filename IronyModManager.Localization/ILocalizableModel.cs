// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-21-2020
//
// Last Modified By : Mario
// Last Modified On : 01-21-2020
// ***********************************************************************
// <copyright file="ILocalizableModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Localization
{
    /// <summary>
    /// Interface ILocalizableModel
    /// </summary>
    public interface ILocalizableModel
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
        /// Called when [locale changed].
        /// </summary>
        /// <param name="newLocale">The new locale.</param>
        /// <param name="oldLocale">The old locale.</param>
        void OnLocaleChanged(string newLocale, string oldLocale);

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
