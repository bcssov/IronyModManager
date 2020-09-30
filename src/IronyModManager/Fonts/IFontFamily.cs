// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-30-2020
//
// Last Modified By : Mario
// Last Modified On : 09-30-2020
// ***********************************************************************
// <copyright file="IFontFamily.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using Avalonia.Media;

namespace IronyModManager.Fonts
{
    /// <summary>
    /// Interface IFontFamily
    /// </summary>
    public interface IFontFamily
    {
        #region Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the resource URI.
        /// </summary>
        /// <value>The resource URI.</value>
        string ResourceUri { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the font family.
        /// </summary>
        /// <returns>FontFamily.</returns>
        FontFamily GetFontFamily();

        #endregion Methods
    }
}
