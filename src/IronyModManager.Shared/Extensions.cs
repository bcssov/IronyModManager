// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 03-22-2024
// ***********************************************************************
// <copyright file="Extensions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    [ExcludeFromCoverage("Extensions are excluded.")]
    public static partial class Extensions
    {
        #region Fields

        /// <summary>
        /// The emoji filter
        /// </summary>
        private const string EmojiFilter = @"\p{Cs}";

        /// <summary>
        /// A private static readonly Dictionary{char,string} named cyrilicMap.
        /// </summary>
        private static readonly Dictionary<char, string> cyrilicMap =
            new() // Pretty much copied various map sources. I'm not looking for 1:1 transliteration just something conherent translation into latin alphabet (also strips diacritics just in case)
            {
                { 'А', "A" },
                { 'Б', "B" },
                { 'В', "V" },
                { 'Г', "G" },
                { 'Д', "D" },
                { 'Ђ', "Đ" },
                { 'Е', "E" },
                { 'Ж', "Z" },
                { 'З', "Z" },
                { 'И', "I" },
                { 'Ј', "J" },
                { 'К', "K" },
                { 'Л', "L" },
                { 'Љ', "Lj" },
                { 'М', "M" },
                { 'Н', "N" },
                { 'Њ', "Nj" },
                { 'О', "O" },
                { 'П', "P" },
                { 'Р', "R" },
                { 'С', "S" },
                { 'Т', "T" },
                { 'Ћ', "C" },
                { 'У', "U" },
                { 'Ф', "F" },
                { 'Х', "H" },
                { 'Ц', "C" },
                { 'Ч', "C" },
                { 'Џ', "Dz" },
                { 'Ш', "S" },
                { 'а', "a" },
                { 'б', "b" },
                { 'в', "v" },
                { 'г', "g" },
                { 'д', "d" },
                { 'ђ', "đ" },
                { 'е', "e" },
                { 'ж', "z" },
                { 'з', "z" },
                { 'и', "i" },
                { 'ј', "j" },
                { 'к', "k" },
                { 'л', "l" },
                { 'љ', "lj" },
                { 'м', "m" },
                { 'н', "n" },
                { 'њ', "nj" },
                { 'о', "o" },
                { 'п', "p" },
                { 'р', "r" },
                { 'с', "s" },
                { 'т', "t" },
                { 'ћ', "c" },
                { 'у', "u" },
                { 'ф', "f" },
                { 'х', "h" },
                { 'ц', "c" },
                { 'ч', "c" },
                { 'џ', "dz" },
                { 'ш', "s" },
                { 'ё', "yo" },
                { 'й', "j" },
                { 'щ', "sch" },
                { 'ъ', "j" },
                { 'ы', "i" },
                { 'ь', "j" },
                { 'э', "e" },
                { 'ю', "yu" },
                { 'я', "ya" },
                { 'Ё', "Yo" },
                { 'Й', "J" },
                { 'Щ', "Sch" },
                { 'Ъ', "J" },
                { 'Ы', "I" },
                { 'Ь', "J" },
                { 'Э', "E" },
                { 'Ю', "Yu" },
                { 'Я', "Ya" }
            };

        /// <summary>
        /// The empty string characters
        /// </summary>
        private static readonly string[] emptyStringCharacters = [" "];

        /// <summary>
        /// The tab space
        /// </summary>
        private static readonly string tabSpace = new(' ', 4);

        /// <summary>
        /// The invalid file name characters
        /// </summary>
        private static IEnumerable<char> invalidFileNameCharacters;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Capitalizes an every first letter.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
        public static string CapitalizeEveryFirstLetter(this string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var sb = new StringBuilder();
                var split = value.Split(' ');
                foreach (var s in split)
                {
                    sb.Append($" {char.ToUpperInvariant(s.FirstOrDefault())}{s[1..]}");
                }

                return sb.ToString().Trim();
            }

            return value;
        }

        /// <summary>
        /// Generates the short file name hash identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns>System.String.</returns>
        public static string GenerateShortFileNameHashId(this string value, int maxLength = 2)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var hash = value.CalculateSHA().GenerateValidFileName();
            return hash.Length > maxLength ? hash[..maxLength] : hash;
        }

        /// <summary>
        /// Generates the name of the valid file.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="transliterateCyrilic">The transliterate cyrilic.</param>
        /// <returns>System.String.</returns>
        public static string GenerateValidFileName(this string value, bool transliterateCyrilic = true)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            if (transliterateCyrilic)
            {
                value = value.TransliterateCyrilicToLatin();
            }

            var fileName = GetInvalidFileNameChars().Aggregate(value, (current, character) => current.Replace(character.ToString(), string.Empty));
            fileName = emptyStringCharacters.Aggregate(fileName, (a, b) => a.Replace(b, "_"));

            // Fireprince don't use emojis in mod names, thanks so much
            fileName = Regex.Replace(fileName, EmojiFilter, string.Empty);
            return fileName;
        }

        /// <summary>
        /// Replaces the new line.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string ReplaceNewLine(this string value)
        {
            return value.Replace("\r", string.Empty).Replace("\n", " ");
        }

        /// <summary>
        /// Replaces the tabs.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string ReplaceTabs(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? value : value.Replace("\t", tabSpace);
        }

        /// <summary>
        /// Splits the on new line.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="ignoreEmpty">if set to <c>true</c> [ignore empty].</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public static IEnumerable<string> SplitOnNewLine(this string value, bool ignoreEmpty = true)
        {
            return value.Replace("\r", string.Empty).Split("\n", ignoreEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

        /// <summary>
        /// Standardizes the directory separator.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string StandardizeDirectorySeparator(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Convert a cyrilic to latin. It does not do 1:1 transliteration.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A string.</returns>
        public static string TransliterateCyrilicToLatin(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            var sb = new StringBuilder();
            foreach (var letter in value)
            {
                if (cyrilicMap.TryGetValue(letter, out var mapped))
                {
                    sb.Append(mapped);
                }
                else
                {
                    sb.Append(letter);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Trims the specified value.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.String.</returns>
        public static string Trim(this string input, string value, StringComparison type = StringComparison.CurrentCultureIgnoreCase)
        {
            return TrimStart(TrimEnd(input, value, type), value, type);
        }

        /// <summary>
        /// Trims the end.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.String.</returns>
        public static string TrimEnd(this string input, string value, StringComparison type = StringComparison.CurrentCultureIgnoreCase)
        {
            if (!string.IsNullOrEmpty(value))
            {
                while (!string.IsNullOrEmpty(input) && input.EndsWith(value, type))
                {
                    input = input[..^value.Length];
                }
            }

            return input;
        }

        /// <summary>
        /// Trims the start.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.String.</returns>
        public static string TrimStart(this string input, string value, StringComparison type = StringComparison.CurrentCultureIgnoreCase)
        {
            if (!string.IsNullOrEmpty(value))
            {
                while (!string.IsNullOrEmpty(input) && input.StartsWith(value, type))
                {
                    input = input[value.Length..];
                }
            }

            return input;
        }

        #endregion Methods
    }
}
