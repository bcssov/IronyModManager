// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-30-2020
//
// Last Modified By : Mario
// Last Modified On : 04-02-2021
// ***********************************************************************
// <copyright file="IFontFamilyManager.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Platform.Fonts
{
    /// <summary>
    /// Interface IFontFamilyManager
    /// </summary>
    public interface IFontFamilyManager
    {
        #region Methods

        /// <summary>
        /// Gets all font names.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        IEnumerable<string> GetAllFontNames();

        /// <summary>
        /// Gets the default font family.
        /// </summary>
        /// <returns>IFontFamily.</returns>
        IFontFamily GetDefaultFontFamily();

        /// <summary>
        /// Determines whether [is irony font] [the specified font name].
        /// </summary>
        /// <param name="fontName">Name of the font.</param>
        /// <returns><c>true</c> if [is irony font] [the specified font name]; otherwise, <c>false</c>.</returns>
        bool IsIronyFont(string fontName);

        /// <summary>
        /// Resolves the font family.
        /// </summary>
        /// <param name="fontName">Name of the font.</param>
        /// <returns>IFontFamily.</returns>
        IFontFamily ResolveFontFamily(string fontName);

        #endregion Methods
    }
}
