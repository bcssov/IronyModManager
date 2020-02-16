// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 02-16-2020
// ***********************************************************************
// <copyright file="SHAHash.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class SHAHash.
    /// </summary>
    public static class SHAHash
    {
        #region Methods

        /// <summary>
        /// Calculates the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string Calculate(string value)
        {
            using var hash = new SHA256Managed();
            var checksum = hash.ComputeHash(Encoding.UTF8.GetBytes(value));
            return BitConverter.ToString(checksum).Replace("-", String.Empty);
        }

        #endregion Methods
    }
}
