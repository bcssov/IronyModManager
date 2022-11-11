// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 10-28-2022
//
// Last Modified By : Mario
// Last Modified On : 10-28-2022
// ***********************************************************************
// <copyright file="Extensions.Enums.cs" company="Mario">
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
    /// Class Extensions.
    /// </summary>
    public static partial class Extensions
    {
        #region Methods

        /// <summary>
        /// Determines whether [has any flag] [the specified enum].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum">The enum.</param>
        /// <returns><c>true</c> if [has any flag] [the specified enum]; otherwise, <c>false</c>.</returns>
        public static bool HasAnyFlag<T>(this T @enum) where T : Enum
        {
            return Convert.ToInt32(@enum) != 0;
        }

        /// <summary>
        /// Determines whether [has multiple flags] [the specified enum].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum">The enum.</param>
        /// <returns><c>true</c> if [has multiple flags] [the specified enum]; otherwise, <c>false</c>.</returns>
        public static bool HasMultipleFlags<T>(this T @enum) where T : Enum
        {
            return (Convert.ToInt32(@enum) & (Convert.ToInt32(@enum) - 1)) != 0;
        }

        #endregion Methods
    }
}
