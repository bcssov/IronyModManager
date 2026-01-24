// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 03-13-2021
//
// Last Modified By : Mario
// Last Modified On : 01-24-2026
// ***********************************************************************
// <copyright file="SKTypefaceCollectionCache.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Fonts;
using Avalonia.Platform;
using Avalonia.Skia;
using SkiaSharp;

namespace IronyModManager.Platform.Fonts
{
    /// <summary>
    /// Class SKTypefaceCollectionCache.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static class SKTypefaceCollectionCache
    {
        #region Fields

        /// <summary>
        /// The cached collections
        /// </summary>
        private static readonly ConcurrentDictionary<FontFamily, SKTypefaceCollection> cachedCollections;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="SKTypefaceCollectionCache" /> class.
        /// </summary>
        static SKTypefaceCollectionCache()
        {
            cachedCollections = new ConcurrentDictionary<FontFamily, SKTypefaceCollection>();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the or add typeface collection.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <returns>SKTypefaceCollection.</returns>
        public static SKTypefaceCollection GetOrAddTypefaceCollection(FontFamily fontFamily)
        {
            return cachedCollections.GetOrAdd(fontFamily, _ => CreateCustomFontCollection(fontFamily));
        }

        /// <summary>
        /// Creates the custom font collection.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        /// <returns>SKTypefaceCollection.</returns>
        /// <exception cref="System.InvalidOperationException">Asset could not be loaded.</exception>
        private static SKTypefaceCollection CreateCustomFontCollection(FontFamily fontFamily)
        {
            var fontAssets = FontFamilyLoader.LoadFontAssets(fontFamily.Key);
            var typeFaceCollection = new SKTypefaceCollection();
            var assetLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();

            foreach (var asset in fontAssets)
            {
                var assetStream = assetLoader.Open(asset);
                if (assetStream == null)
                {
                    throw new InvalidOperationException("Asset could not be loaded.");
                }

                var typeface = SKTypeface.FromStream(assetStream);
                if (typeface == null)
                {
                    return null;
                }

                var typefaceFamilyName = typeface.FamilyName ?? string.Empty;
                var familyName = fontFamily.Name ?? string.Empty;
                if (!typefaceFamilyName.Contains(familyName))
                {
                    continue;
                }

                var key = new Typeface(fontFamily, typeface.FontSlant.ToAvalonia(), (FontWeight)typeface.FontWeight);
                typeFaceCollection.AddTypeface(key, typeface);
            }

            return typeFaceCollection;
        }

        #endregion Methods
    }
}
