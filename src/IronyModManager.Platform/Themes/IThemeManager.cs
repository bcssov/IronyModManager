// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 03-14-2021
//
// Last Modified By : Mario
// Last Modified On : 06-14-2021
// ***********************************************************************
// <copyright file="IThemeManager.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Platform.Themes
{
    /// <summary>
    /// Interface IThemeManager
    /// </summary>
    public interface IThemeManager
    {
        #region Methods

        /// <summary>
        /// Applies the theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        void ApplyTheme(string theme);

        /// <summary>
        /// Determines whether [is light theme] [the specified theme].
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns><c>true</c> if [is light theme] [the specified theme]; otherwise, <c>false</c>.</returns>
        bool IsLightTheme(string theme);

        #endregion Methods
    }
}
