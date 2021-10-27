// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 03-13-2021
//
// Last Modified By : Mario
// Last Modified On : 10-27-2021
// ***********************************************************************
// <copyright file="SKTypefaceCollection.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using SkiaSharp;

namespace IronyModManager.Platform.Fonts
{
    /// <summary>
    /// Class SKTypefaceCollection.
    /// </summary>
    internal class SKTypefaceCollection
    {
        #region Fields

        /// <summary>
        /// The typefaces
        /// </summary>
        private readonly ConcurrentDictionary<Typeface, SKTypeface> typefaces = new();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Adds the typeface.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="typeface">The typeface.</param>
        public void AddTypeface(Typeface key, SKTypeface typeface)
        {
            typefaces.TryAdd(key, typeface);
        }

        /// <summary>
        /// Gets the specified typeface.
        /// </summary>
        /// <param name="typeface">The typeface.</param>
        /// <returns>SKTypeface.</returns>
        public SKTypeface Get(Typeface typeface)
        {
            return GetNearestMatch(typefaces, typeface);
        }

        /// <summary>
        /// Gets the nearest match.
        /// </summary>
        /// <param name="typefaces">The typefaces.</param>
        /// <param name="key">The key.</param>
        /// <returns>SKTypeface.</returns>
        private static SKTypeface GetNearestMatch(IDictionary<Typeface, SKTypeface> typefaces, Typeface key)
        {
            if (typefaces.TryGetValue(key, out var typeface))
            {
                return typeface;
            }

            var initialWeight = (int)key.Weight;

            var weight = (int)key.Weight;

            weight -= weight % 50; // make sure we start at a full weight

            for (var i = (int)key.Style; i < 2; i++)
            {
                // only try 2 font weights in each direction
                for (var j = 0; j < initialWeight; j += 50)
                {
                    if (weight - j >= 100)
                    {
                        if (typefaces.TryGetValue(new Typeface(key.FontFamily, (FontStyle)i, (FontWeight)(weight - j)), out typeface))
                        {
                            return typeface;
                        }
                    }

                    if (weight + j > 900)
                    {
                        continue;
                    }

                    if (typefaces.TryGetValue(new Typeface(key.FontFamily, (FontStyle)i, (FontWeight)(weight + j)), out typeface))
                    {
                        return typeface;
                    }
                }
            }

            //Nothing was found so we use the first typeface we can get.
            return typefaces.Values.FirstOrDefault();
        }

        #endregion Methods
    }
}
