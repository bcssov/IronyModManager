// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 07-03-2024
// ***********************************************************************
// <copyright file="Extensions.Version.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    public static partial class Extensions
    {
#nullable enable


        #region Fields

        /// <summary>
        /// The version extract
        /// </summary>
        private static readonly Regex versionExtract = new(@"(?:\*|\d+)(?:\.\d+|\.\*)+");

        #endregion Fields

        #region Methods

        /// <summary>
        /// Converts to version.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Nullable&lt;Shared.Version&gt;.</returns>
        public static Version? ToVersion(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            // Detect PDX mess
            if (value.Any(char.IsLetter))
            {
                var result = versionExtract.Match(value);
                if (result.Success)
                {
                    value = result.Value;
                }
            }

            var sb = new StringBuilder();
            var count = 0;
            foreach (var item in value.Split("."))
            {
                var parsed = item.Replace("*", string.Empty);
                if (string.IsNullOrWhiteSpace(parsed))
                {
                    parsed = "*";
                }

                if (int.TryParse(parsed, out var part))
                {
                    sb.Append($"{part}.");
                }
                else if (parsed.Equals("*"))
                {
                    sb.Append($"{(count > 1 ? int.MaxValue : 0)}.");
                }

                count++;
            }

            return Version.TryParse(sb.ToString().Trim().Trim('.'), out var parsedVersion) ? parsedVersion : null;
        }

        #endregion Methods
    }
}
