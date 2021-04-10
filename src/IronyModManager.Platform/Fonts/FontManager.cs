// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 03-13-2021
//
// Last Modified By : Mario
// Last Modified On : 04-10-2021
// ***********************************************************************
// <copyright file="FontManager.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Skia;
using IronyModManager.DI;

namespace IronyModManager.Platform.Fonts
{
    /// <summary>
    /// Class FontManager.
    /// Implements the <see cref="Avalonia.Platform.IFontManagerImpl" />
    /// </summary>
    /// <seealso cref="Avalonia.Platform.IFontManagerImpl" />
    internal class FontManager : IFontManagerImpl
    {
        #region Fields

        /// <summary>
        /// The font family manager
        /// </summary>
        private static IFontFamilyManager fontFamilyManager;

        /// <summary>
        /// The font manager
        /// </summary>
        private static IFontManagerImpl fontManager;

        /// <summary>
        /// All fonts
        /// </summary>
        private List<string> allFonts = null;

        /// <summary>
        /// The typefaces
        /// </summary>
        private List<Typeface> typefaces;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FontManager" /> class.
        /// </summary>
        /// <param name="underlyingFontManager">The underlying font manager.</param>
        public FontManager(IFontManagerImpl underlyingFontManager)
        {
            fontManager = underlyingFontManager;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Creates a glyph typeface.
        /// </summary>
        /// <param name="typeface">The typeface.</param>
        /// <returns>0
        /// The created glyph typeface. Can be <c>Null</c> if it was not possible to create a glyph typeface.</returns>
        public IGlyphTypefaceImpl CreateGlyphTypeface(Typeface typeface)
        {
            var fontFamilyManager = GetFontFamilyManager();
            if (typeface.FontFamily != null && fontFamilyManager.IsIronyFont(typeface.FontFamily.Name))
            {
                if (typeface.FontFamily.Key == null)
                {
                    typeface = new Typeface(fontFamilyManager.ResolveFontFamily(typeface.FontFamily.Name).GetFontFamily(), typeface.Style, typeface.Weight);
                }
                var fontCollection = SKTypefaceCollectionCache.GetOrAddTypefaceCollection(typeface.FontFamily);
                if (fontCollection == null)
                {
                    // Fallback to Irony default
                    typeface = new Typeface(fontFamilyManager.GetDefaultFontFamily().GetFontFamily(), typeface.Style, typeface.Weight);
                    fontCollection = SKTypefaceCollectionCache.GetOrAddTypefaceCollection(typeface.FontFamily);
                }
                var skTypeface = fontCollection.Get(typeface);
                if (skTypeface == null)
                {
                    return fontManager.CreateGlyphTypeface(typeface);
                }
                return new GlyphTypefaceImpl(skTypeface);
            }
            return fontManager.CreateGlyphTypeface(typeface);
        }

        /// <summary>
        /// Gets the system's default font family's name.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetDefaultFontFamilyName()
        {
            var fontFamilyManager = GetFontFamilyManager();
            return fontFamilyManager.GetDefaultFontFamily().Name;
        }

        /// <summary>
        /// Get all installed fonts in the system.
        /// <param name="checkForUpdates">If <c>true</c> the font collection is updated.</param>
        /// </summary>
        /// <param name="checkForUpdates">if set to <c>true</c> [check for updates].</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetInstalledFontFamilyNames(bool checkForUpdates = false)
        {
            if (allFonts == null || checkForUpdates)
            {
                var fontFamilyManager = GetFontFamilyManager();
                var fonts = new List<string>();
                fonts.AddRange(fontFamilyManager.GetAllFontNames());
                fonts.AddRange(fontManager.GetInstalledFontFamilyNames(checkForUpdates));
                allFonts = fonts.Distinct().ToList();
            }
            return allFonts;
        }

        /// <summary>
        /// Tries to match a specified character to a typeface that supports specified font properties.
        /// </summary>
        /// <param name="codepoint">The codepoint to match against.</param>
        /// <param name="fontStyle">The font style.</param>
        /// <param name="fontWeight">The font weight.</param>
        /// <param name="fontFamily">The font family. This is optional and used for fallback lookup.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="typeface">The matching typeface.</param>
        /// <returns><c>True</c>, if the <see cref="T:Avalonia.Platform.IFontManagerImpl" /> could match the character to specified parameters, <c>False</c> otherwise.</returns>
        public bool TryMatchCharacter(int codepoint, FontStyle fontStyle, FontWeight fontWeight, FontFamily fontFamily, CultureInfo culture, out Typeface typeface)
        {
            if (typefaces == null)
            {
                typefaces = new List<Typeface>();
                foreach (var item in fontFamilyManager.GetAllFontNames())
                {
                    typefaces.Add(new Typeface(fontFamilyManager.ResolveFontFamily(item).GetFontFamily()));
                }
            }
            foreach (var item in typefaces)
            {
                if (item.GlyphTypeface.GetGlyph((uint)codepoint) == 0)
                {
                    continue;
                }
                typeface = new Typeface(item.FontFamily.Name, fontStyle, fontWeight);
                return true;
            }

            return fontManager.TryMatchCharacter(codepoint, fontStyle, fontWeight, fontFamily, culture, out typeface);
        }

        /// <summary>
        /// Gets the font family manager.
        /// </summary>
        /// <returns>IFontFamilyManager.</returns>
        private IFontFamilyManager GetFontFamilyManager()
        {
            if (fontFamilyManager == null)
            {
                fontFamilyManager = DIResolver.Get<IFontFamilyManager>();
            }
            return fontFamilyManager;
        }

        #endregion Methods
    }
}
