// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 01-27-2022
// ***********************************************************************
// <copyright file="Extensions.Stream.cs" company="Mario">
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
    public static partial class Extensions
    {
        #region Fields

        /// <summary>
        /// The hash
        /// </summary>
        private static readonly IMetroHash128 hash = MetroHash128Factory.Instance.Create();

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

        #endregion Methods
    }
}
