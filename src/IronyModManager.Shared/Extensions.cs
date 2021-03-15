// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 03-15-2021
// ***********************************************************************
// <copyright file="Extensions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data.HashFunction.MetroHash;
using System.IO;
using System.Linq;
using System.Text;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    [ExcludeFromCoverage("Hash calculation is excluded.")]
    public static class Extensions
    {
        #region Fields

        /// <summary>
        /// The empty string characters
        /// </summary>
        private static readonly string[] emptyStringCharacters = new string[] { " " };

        /// <summary>
        /// The hash
        /// </summary>
        private static readonly IMetroHash128 hash = MetroHash128Factory.Instance.Create();

        /// <summary>
        /// The tab space
        /// </summary>
        private static readonly string tabSpace = new(' ', 4);

        #endregion Fields

        #region Methods

        /// <summary>
        /// Calculates the sha.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string CalculateSHA(this string value)
        {
            var checksum = hash.ComputeHash(Encoding.UTF8.GetBytes(value));
            return checksum.AsHexString();
        }

        /// <summary>
        /// Calculates the sha.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>System.String.</returns>
        public static string CalculateSHA(this Stream stream)
        {
            using var bufferedStream = new BufferedStream(stream, 1024 * 32);
            var checksum = hash.ComputeHash(bufferedStream);
            return checksum.AsHexString();
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
            if (hash.Length > maxLength)
            {
                return hash.Substring(0, maxLength);
            }
            return hash;
        }

        /// <summary>
        /// Generates the name of the valid file.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string GenerateValidFileName(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
            var fileName = Path.GetInvalidFileNameChars().Aggregate(value, (current, character) => current.Replace(character.ToString(), string.Empty));
            fileName = emptyStringCharacters.Aggregate(fileName, (a, b) => a.Replace(b, "_"));
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
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
            return value.Replace("\t", tabSpace);
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
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }
            return value.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        }

        #endregion Methods
    }
}
