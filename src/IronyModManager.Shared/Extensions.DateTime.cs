// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-17-2024
//
// Last Modified By : Mario
// Last Modified On : 02-17-2024
// ***********************************************************************
// <copyright file="Extensions.DateTime.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Shared
{
    /// <summary>
    /// The extensions.
    /// </summary>
    public partial class Extensions
    {
        #region Methods

        /// <summary>
        /// Is date same.
        /// </summary>
        /// <param name="date1">The date 1.</param>
        /// <param name="date2">The date 2.</param>
        /// <returns>A bool.</returns>
        public static bool IsDateSame(this DateTime date1, DateTime date2)
        {
            return date1.Date == date2.Date;
        }

        #endregion Methods
    }
}
