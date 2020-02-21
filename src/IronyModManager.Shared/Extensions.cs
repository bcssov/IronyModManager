// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 02-18-2020
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
using System.Security.Cryptography;
using System.Text;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    [ExcludeFromCoverage("Hash calculation is excluded.")]
    public static class Extensions
    {
        #region Methods

        /// <summary>
        /// Calculates the sha.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string CalculateSHA(this string value)
        {
            using var hash = new SHA256Managed();
            var checksum = hash.ComputeHash(Encoding.UTF8.GetBytes(value));
            return BitConverter.ToString(checksum).Replace("-", String.Empty);
        }

        //Source: https://stackoverflow.com/questions/14449973/how-to-tell-if-a-file-is-text-readable-in-c-sharp
        /// <summary>
        /// Determines whether [is text file] [the specified stream].
        /// Source: https://stackoverflow.com/questions/14449973/how-to-tell-if-a-file-is-text-readable-in-c-sharp
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns><c>true</c> if [is text file] [the specified stream]; otherwise, <c>false</c>.</returns>
        public static bool IsTextFile(this Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var bytesRead = reader.ReadToEnd();
            reader.Close();
            return bytesRead.All(c => c == (char)10 || c == (char)13 || c == (char)11 || !char.IsControl(c));
        }

        #endregion Methods
    }
}
