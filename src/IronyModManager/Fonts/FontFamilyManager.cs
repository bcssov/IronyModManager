// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-30-2020
//
// Last Modified By : Mario
// Last Modified On : 09-30-2020
// ***********************************************************************
// <copyright file="FontFamilyManager.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Fonts
{
    /// <summary>
    /// Class FontFamilyManager.
    /// Implements the <see cref="IronyModManager.Fonts.IFontFamilyManager" />
    /// </summary>
    /// <seealso cref="IronyModManager.Fonts.IFontFamilyManager" />
    public class FontFamilyManager : IFontFamilyManager
    {
        #region Fields

        /// <summary>
        /// The font families
        /// </summary>
        private readonly IEnumerable<IFontFamily> fontFamilies;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FontFamilyManager" /> class.
        /// </summary>
        /// <param name="fontFamilies">The font families.</param>
        public FontFamilyManager(IEnumerable<IFontFamily> fontFamilies)
        {
            this.fontFamilies = fontFamilies;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Resolves the font family.
        /// </summary>
        /// <param name="fontName">Name of the font.</param>
        /// <returns>IFontFamily.</returns>
        public IFontFamily ResolveFontFamily(string fontName)
        {
            return fontFamilies.First(p => p.Name.Equals(fontName, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Methods
    }
}
