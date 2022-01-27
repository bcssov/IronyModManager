// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 01-27-2022
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

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    public static partial class Extensions
    {
#nullable enable

        #region Methods

        /// <summary>
        /// Converts to version.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Version?.</returns>
        public static Version? ToVersion(this string value)
        {
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
            if (Version.TryParse(sb.ToString().Trim().Trim('.'), out var parsedVersion))
            {
                return parsedVersion;
            }
            return null;
        }

        #endregion Methods

#nullable disable
    }
}
