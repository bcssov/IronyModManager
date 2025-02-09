// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-06-2025
// ***********************************************************************
// <copyright file="CurrentLocale.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using IronyModManager.Shared;
using IronyModManager.Shared.Expressions;

namespace IronyModManager.Localization
{
    /// <summary>
    /// Class CurrentLocale.
    /// </summary>
    [ExcludeFromCoverage("Locale shouldn't be unit tested.")]
    public static class CurrentLocale
    {
        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="CurrentLocale" /> class.
        /// </summary>
        static CurrentLocale()
        {
            InitialCulture = Thread.CurrentThread.CurrentCulture;
        }

        #endregion Constructors

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

        /// <summary>
        /// Gets the initial culture.
        /// </summary>
        /// <value>The initial culture.</value>
        public static CultureInfo InitialCulture { get; private set; }

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

            MathExpression.Culture = CurrentCulture;

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
