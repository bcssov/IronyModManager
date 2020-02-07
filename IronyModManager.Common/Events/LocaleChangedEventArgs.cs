// ***********************************************************************
// Assembly         : IronyModManager.Common
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 02-06-2020
// ***********************************************************************
// <copyright file="LocaleChangedEventArgs.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Shared;

namespace IronyModManager.Common.Events
{
    /// <summary>
    /// Class LocaleChangedEventArgs.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    [ExcludeFromCoverage("Excluding external message bus.")]
    public class LocaleChangedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        /// <value>The locale.</value>
        public string Locale { get; set; }

        /// <summary>
        /// Gets or sets the old locale.
        /// </summary>
        /// <value>The old locale.</value>
        public string OldLocale { get; set; }

        #endregion Properties
    }
}
