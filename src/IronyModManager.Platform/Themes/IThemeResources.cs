// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 03-14-2021
//
// Last Modified By : Mario
// Last Modified On : 03-14-2021
// ***********************************************************************
// <copyright file="IThemeResources.cs" company="Mario">
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
    /// Interface IThemeResources
    /// </summary>
    public interface IThemeResources
    {
        #region Properties

        /// <summary>
        /// Gets the styles.
        /// </summary>
        /// <value>The styles.</value>
        IReadOnlyCollection<string> Styles { get; }

        /// <summary>
        /// Gets the name of the theme.
        /// </summary>
        /// <value>The name of the theme.</value>
        string ThemeName { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Registers this instance.
        /// </summary>
        void Register();

        #endregion Methods
    }
}
