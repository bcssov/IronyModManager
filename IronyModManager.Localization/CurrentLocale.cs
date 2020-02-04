// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-04-2020
// ***********************************************************************
// <copyright file="CurrentLocale.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Threading;
using IronyModManager.Shared;

namespace IronyModManager.Localization
{
    /// <summary>
    /// Class CurrentLocale.
    /// </summary>
    [ExcludeFromCoverage("Locale shouldn't be unit tested.")]
    public static class CurrentLocale
    {
        #region Properties

        /// <summary>
        /// Gets the name of the culture.
        /// </summary>
        /// <value>The name of the culture.</value>
        public static string CultureName => CurrentCulture.Name;

        /// <summary>
        /// Gets or sets the current culture.
        /// </summary>
        /// <value>The current culture.</value>
        public static CultureInfo CurrentCulture { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Sets the current.
        /// </summary>
        /// <param name="cultureName">Name of the culture.</param>
        public static void SetCurrent(string cultureName)
        {
            var culture = new CultureInfo(cultureName);
            CurrentCulture = culture;

            SetThreadCulture();
        }

        /// <summary>
        /// Sets the current.
        /// </summary>
        /// <param name="culture">The culture.</param>
        public static void SetCurrent(CultureInfo culture)
        {
            CurrentCulture = culture;

            SetThreadCulture();
        }

        /// <summary>
        /// Sets the thread culture.
        /// </summary>
        private static void SetThreadCulture()
        {
            Thread.CurrentThread.CurrentCulture = CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = CurrentCulture;
        }

        #endregion Methods
    }
}
