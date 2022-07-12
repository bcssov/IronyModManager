// ***********************************************************************
// Assembly         : IronyModManager.Common
// Author           : Mario
// Created          : 07-12-2022
//
// Last Modified By : Mario
// Last Modified On : 07-12-2022
// ***********************************************************************
// <copyright file="Helpers.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.Localization;
using IronyModManager.Platform.Configuration;

namespace IronyModManager.Common
{
    /// <summary>
    /// Class Helpers.
    /// </summary>
    public static class Helpers
    {
        #region Methods

        /// <summary>
        /// Gets the format provider.
        /// </summary>
        /// <returns>IFormatProvider.</returns>
        public static IFormatProvider GetFormatProvider() => DIResolver.Get<IPlatformConfiguration>().GetOptions().Formatting.UseSystemCulture ? CurrentLocale.InitialCulture : CurrentLocale.CurrentCulture;

        #endregion Methods
    }
}
