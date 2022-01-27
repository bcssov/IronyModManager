// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 01-27-2022
// ***********************************************************************
// <copyright file="Extensions.Char.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    public static partial class Extensions
    {
        #region Fields

        /// <summary>
        /// The invalid file name characters
        /// </summary>
        private static readonly char[] invalidFileNameCharactersExtension = new char[] { ':' };

        #endregion Fields

#nullable enable

        #region Methods

        /// <summary>
        /// Gets the invalid file name chars.
        /// </summary>
        /// <returns>System.Collections.Generic.IEnumerable&lt;char&gt;.</returns>
        private static IEnumerable<char> GetInvalidFileNameChars()
        {
            if (invalidFileNameCharacters == null)
            {
                invalidFileNameCharacters = Path.GetInvalidFileNameChars().Concat(invalidFileNameCharactersExtension).Distinct().ToList();
            }
            return invalidFileNameCharacters;
        }

        #endregion Methods

#nullable disable
    }
}
