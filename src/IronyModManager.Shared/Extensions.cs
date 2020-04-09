// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 04-02-2020
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
        /// The tab space
        /// </summary>
        private static readonly string tabSpace = new string(' ', 4);

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
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public static IEnumerable<string> SplitOnNewLine(this string value)
        {
            return value.Contains("\r\n") ? value.Split("\r\n", StringSplitOptions.RemoveEmptyEntries) : value.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion Methods
    }
}
