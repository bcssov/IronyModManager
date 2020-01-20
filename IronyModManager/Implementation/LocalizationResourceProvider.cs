// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-20-2020
// ***********************************************************************
// <copyright file="LocalizationResourceProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.Localization;

namespace IronyModManager
{
    /// <summary>
    /// Class LocalizationResourceProvider.
    /// Implements the <see cref="IronyModManager.Localization.BaseLocalizationResourceProvider" />
    /// Implements the <see cref="IronyModManager.Localization.IDefaultLocalizationResourceProvider" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.IDefaultLocalizationResourceProvider" />
    /// <seealso cref="IronyModManager.Localization.BaseLocalizationResourceProvider" />
    public class LocalizationResourceProvider : BaseLocalizationResourceProvider, IDefaultLocalizationResourceProvider
    {
        #region Properties

        /// <summary>
        /// Gets the root path.
        /// </summary>
        /// <value>The root path.</value>
        public override string RootPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.LocalizationsPath);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the available locales.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetAvailableLocales()
        {
            return new DirectoryInfo(RootPath).GetFiles($"*{Shared.Constants.JsonExtension}", SearchOption.TopDirectoryOnly).Select(p => Path.GetFileNameWithoutExtension(p.Name));
        }

        #endregion Methods
    }
}
