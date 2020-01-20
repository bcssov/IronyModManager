// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-18-2020
// ***********************************************************************
// <copyright file="LocalizationResourceProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using IronyModManager.Localization;

namespace IronyModManager
{
    /// <summary>
    /// Class LocalizationResourceProvider.
    /// Implements the <see cref="IronyModManager.Localization.BaseLocalizationResourceProvider" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.BaseLocalizationResourceProvider" />
    public class LocalizationResourceProvider : BaseLocalizationResourceProvider
    {
        #region Properties

        /// <summary>
        /// Gets the root path.
        /// </summary>
        /// <value>The root path.</value>
        public override string RootPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.LocalizationsPath);

        #endregion Properties
    }
}
