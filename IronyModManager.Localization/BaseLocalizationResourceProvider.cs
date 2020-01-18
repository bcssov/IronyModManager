// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-18-2020
// ***********************************************************************
// <copyright file="BaseLocalizationResourceProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;

namespace IronyModManager.Localization
{
    /// <summary>
    /// Class BaseLocalizationResourceProvider.
    /// Implements the <see cref="IronyModManager.Localization.ILocalizationResourceProvider" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.ILocalizationResourceProvider" />
    public abstract class BaseLocalizationResourceProvider : ILocalizationResourceProvider
    {
        #region Properties

        /// <summary>
        /// Gets the resource path.
        /// </summary>
        /// <value>The resource path.</value>
        public abstract string ResourcePath { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Reads the resource.
        /// </summary>
        /// <returns>System.String.</returns>
        public virtual string ReadResource()
        {
            return File.ReadAllText(ResourcePath);
        }

        #endregion Methods
    }
}
