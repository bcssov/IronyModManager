// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 03-13-2021
//
// Last Modified By : Mario
// Last Modified On : 07-10-2022
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
            return GetNearestMatch(typeface);
        }

        /// <summary>
        /// Gets the nearest match.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>SKTypeface.</returns>
        private SKTypeface GetNearestMatch(Typeface key)
        {
            if (typefaces.Count == 0)
            {
                return null;
            }

            if (typefaces.TryGetValue(key, out var typeface))
            {
                return typeface;
            }

            if (key.Style != FontStyle.Normal)
            {
                key = new Typeface(key.FontFamily, FontStyle.Normal, key.Weight);
            }

            if (TryFindWeightFallback(key, out typeface))
            {
                return typeface;
            }

            //Nothing was found so we try some regular typeface.
            if (typefaces.TryGetValue(new Typeface(key.FontFamily), out typeface))
            {
                return typeface;
            }

            SKTypeface skTypeface = null;

            foreach (var pair in typefaces)
            {
                skTypeface = pair.Value;

                var typefaceFamilyName = skTypeface.FamilyName ?? string.Empty;
                var familyName = key.FontFamily.Name ?? string.Empty;

                if (typefaceFamilyName.Contains(familyName))
                {
                    return skTypeface;
                }
            }

            //Nothing was found so we use the first typeface we can get.
            return skTypeface ?? typefaces.Values.FirstOrDefault();
        }

        /// <summary>
        /// Tries the find weight fallback.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="typeface">The typeface.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool TryFindWeightFallback(Typeface key, out SKTypeface typeface)
        {
            typeface = null;
            var weight = (int)key.Weight;

            //If the target weight given is between 400 and 500 inclusive
            if (weight >= 400 && weight <= 500)
            {
                //Look for available weights between the target and 500, in ascending order.
                for (var i = 0; weight + i <= 500; i += 50)
                {
                    if (typefaces.TryGetValue(new Typeface(key.FontFamily, key.Style, (FontWeight)(weight + i)), out typeface))
                    {
                        return true;
                    }
                }

                //If no match is found, look for available weights less than the target, in descending order.
                for (var i = 0; weight - i >= 100; i += 50)
                {
                    if (typefaces.TryGetValue(new Typeface(key.FontFamily, key.Style, (FontWeight)(weight - i)), out typeface))
                    {
                        return true;
                    }
                }

                //If no match is found, look for available weights greater than 500, in ascending order.
                for (var i = 0; weight + i <= 900; i += 50)
                {
                    if (typefaces.TryGetValue(new Typeface(key.FontFamily, key.Style, (FontWeight)(weight + i)), out typeface))
                    {
                        return true;
                    }
                }
            }

            //If a weight less than 400 is given, look for available weights less than the target, in descending order.
            if (weight < 400)
            {
                for (var i = 0; weight - i >= 100; i += 50)
                {
                    if (typefaces.TryGetValue(new Typeface(key.FontFamily, key.Style, (FontWeight)(weight - i)), out typeface))
                    {
                        return true;
                    }
                }

                //If no match is found, look for available weights less than the target, in descending order.
                for (var i = 0; weight + i <= 900; i += 50)
                {
                    if (typefaces.TryGetValue(new Typeface(key.FontFamily, key.Style, (FontWeight)(weight + i)), out typeface))
                    {
                        return true;
                    }
                }
            }

            //If a weight greater than 500 is given, look for available weights greater than the target, in ascending order.
            if (weight > 500)
            {
                for (var i = 0; weight + i <= 900; i += 50)
                {
                    if (typefaces.TryGetValue(new Typeface(key.FontFamily, key.Style, (FontWeight)(weight + i)), out typeface))
                    {
                        return true;
                    }
                }

                //If no match is found, look for available weights less than the target, in descending order.
                for (var i = 0; weight - i >= 100; i += 50)
                {
                    if (typefaces.TryGetValue(new Typeface(key.FontFamily, key.Style, (FontWeight)(weight - i)), out typeface))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion Methods
    }
}
