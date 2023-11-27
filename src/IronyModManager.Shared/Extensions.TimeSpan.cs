
// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 11-27-2023
//
// Last Modified By : Mario
// Last Modified On : 11-27-2023
// ***********************************************************************
// <copyright file="Extensions.TimeSpan.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronyModManager.Shared
{

    /// <summary>
    /// Class Extensions.
    /// </summary>
    public static partial class Extensions
    {
        #region Methods

        /// <summary>
        /// Formats the elapsed.
        /// </summary>
        /// <param name="ts">The ts.</param>
        /// <returns>System.String.</returns>
        public static string FormatElapsed(this TimeSpan ts)
        {
            return string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        }

        #endregion Methods
    }
}
