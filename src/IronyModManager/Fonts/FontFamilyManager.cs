// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-30-2020
//
// Last Modified By : Mario
// Last Modified On : 10-01-2020
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

        /// <summary>
        /// The system fonts
        /// </summary>
        private readonly List<SystemFontFamily> systemFonts;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FontFamilyManager" /> class.
        /// </summary>
        /// <param name="fontFamilies">The font families.</param>
        public FontFamilyManager(IEnumerable<IFontFamily> fontFamilies)
        {
            this.fontFamilies = fontFamilies;
            systemFonts = new List<SystemFontFamily>();
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
            var font = fontFamilies.FirstOrDefault(p => p.Name.Equals(fontName, StringComparison.OrdinalIgnoreCase));
            if (font == null)
            {
                SystemFontFamily systemFont = systemFonts.FirstOrDefault(p => p.Name.Equals(fontName));
                if (systemFont == null)
                {
                    systemFont = new SystemFontFamily(fontName);
                    systemFonts.Add(systemFont);
                }
                return systemFont;
            }
            return font;
        }

        #endregion Methods
    }
}
