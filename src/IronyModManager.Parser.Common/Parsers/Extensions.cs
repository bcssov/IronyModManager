// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 07-20-2022
//
// Last Modified By : Mario
// Last Modified On : 07-20-2022
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

namespace IronyModManager.Parser.Common.Parsers
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    public static class Extensions
    {
        #region Methods

        /// <summary>
        /// Formats the type of the definition.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="typeOverride">The type override.</param>
        /// <returns>System.String.</returns>
        public static string FormatDefinitionType(this string file, string typeOverride = Shared.Constants.EmptyParam)
        {
            var formatted = Path.GetDirectoryName(file);
            var type = Path.GetExtension(file).Trim('.');
            if (!Shared.Constants.TextExtensions.Any(s => s.EndsWith(type, StringComparison.OrdinalIgnoreCase)))
            {
                type = Constants.TxtType;
            }
            return $"{formatted.ToLowerInvariant()}{Path.DirectorySeparatorChar}{(string.IsNullOrWhiteSpace(typeOverride) ? type : typeOverride)}";
        }

        #endregion Methods
    }
}
