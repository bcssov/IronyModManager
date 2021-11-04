// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-30-2020
//
// Last Modified By : Mario
// Last Modified On : 11-04-2021
// ***********************************************************************
// <copyright file="FontFamilyManager.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Platform.Fonts;

namespace IronyModManager.Fonts
{
    /// <summary>
    /// Class FontFamilyManager.
    /// Implements the <see cref="IronyModManager.Platform.Fonts.IFontFamilyManager" />
    /// </summary>
    /// <seealso cref="IronyModManager.Platform.Fonts.IFontFamilyManager" />
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
        /// Gets all font names.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetAllFontNames()
        {
            return fontFamilies.Select(p => p.Name).ToList();
        }

        /// <summary>
        /// Gets the default font family.
        /// </summary>
        /// <returns>IFontFamily.</returns>
        public IFontFamily GetDefaultFontFamily()
        {
            return ResolveFontFamily(Constants.DefaultFont);
        }

        /// <summary>
        /// Determines whether [is irony font] [the specified font name].
        /// </summary>
        /// <param name="fontName">Name of the font.</param>
        /// <returns><c>true</c> if [is irony font] [the specified font name]; otherwise, <c>false</c>.</returns>
        public bool IsIronyFont(string fontName)
        {
            return fontFamilies.Any(p => p.Name.Equals(fontName ?? string.Empty, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Resolves the font family.
        /// </summary>
        /// <param name="fontName">Name of the font.</param>
        /// <returns>IFontFamily.</returns>
        public IFontFamily ResolveFontFamily(string fontName)
        {
            var font = fontFamilies.FirstOrDefault(p => p.Name.Equals(fontName ?? string.Empty, StringComparison.OrdinalIgnoreCase));
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
