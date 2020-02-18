// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-17-2020
//
// Last Modified By : Mario
// Last Modified On : 02-18-2020
// ***********************************************************************
// <copyright file="DetectDuplicates.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Linq;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class DetectDuplicates.
    /// </summary>
    public class DetectDuplicates
    {
        #region Methods

        //src: https://stackoverflow.com/questions/14449973/how-to-tell-if-a-file-is-text-readable-in-c-sharp
        /// <summary>
        /// Determines whether [is valid text file] [the specified path].
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if [is valid text file] [the specified path]; otherwise, <c>false</c>.</returns>
        public static bool IsValidTextFile(string path)
        {
            using (var stream = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
            using (var reader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8))
            {
                var bytesRead = reader.ReadToEnd();
                reader.Close();
                return bytesRead.All(c => c == (char)10 || c == (char)13 || c == (char)11 || !char.IsControl(c));
            }
        }

        #endregion Methods
    }
}
