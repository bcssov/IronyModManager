// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 04-17-2020
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
using IronyModManager.Localization.ResourceProviders;

namespace IronyModManager.Implementation
{
    /// <summary>
    /// Class LocalizationResourceProvider.
    /// Implements the <see cref="IronyModManager.Localization.ResourceProviders.BaseLocalizationResourceProvider" />
    /// Implements the <see cref="IronyModManager.Localization.ResourceProviders.IDefaultLocalizationResourceProvider" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.ResourceProviders.BaseLocalizationResourceProvider" />
    /// <seealso cref="IronyModManager.Localization.ResourceProviders.IDefaultLocalizationResourceProvider" />
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
        public virtual IEnumerable<string> GetAvailableLocales()
        {
            return new DirectoryInfo(RootPath).GetFiles($"*{Shared.Constants.JsonExtension}", SearchOption.TopDirectoryOnly).Select(p => Path.GetFileNameWithoutExtension(p.Name));
        }

        #endregion Methods
    }
}
