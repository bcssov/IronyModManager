// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-30-2020
//
// Last Modified By : Mario
// Last Modified On : 09-30-2020
// ***********************************************************************
// <copyright file="IFontFamilyManager.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Fonts
{
    /// <summary>
    /// Interface IFontFamilyManager
    /// </summary>
    public interface IFontFamilyManager
    {
        #region Methods

        /// <summary>
        /// Resolves the font family.
        /// </summary>
        /// <param name="fontName">Name of the font.</param>
        /// <returns>IFontFamily.</returns>
        IFontFamily ResolveFontFamily(string fontName);

        #endregion Methods
    }
}
