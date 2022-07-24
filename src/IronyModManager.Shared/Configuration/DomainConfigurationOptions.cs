// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 07-24-2022
//
// Last Modified By : Mario
// Last Modified On : 07-24-2022
// ***********************************************************************
// <copyright file="DomainConfigurationOptions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Shared.Configuration
{
    /// <summary>
    /// Class DomainConfigurationOptions.
    /// </summary>
    public class DomainConfigurationOptions
    {
        #region Properties

        /// <summary>
        /// Gets the formatting.
        /// </summary>
        /// <value>The formatting.</value>
        public Formatting Formatting { get; } = new Formatting();

        /// <summary>
        /// Gets the osx options.
        /// </summary>
        /// <value>The osx options.</value>
        public OSXOptions OSXOptions { get; } = new OSXOptions();

        /// <summary>
        /// Gets the steam.
        /// </summary>
        /// <value>The steam.</value>
        public Steam Steam { get; } = new Steam();

        #endregion Properties
    }

    /// <summary>
    /// Class Formatting.
    /// </summary>
    public class Formatting
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [use system culture].
        /// </summary>
        /// <value><c>true</c> if [use system culture]; otherwise, <c>false</c>.</value>
        public bool UseSystemCulture { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class OSXOptions.
    /// </summary>
    public class OSXOptions
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [use file streams].
        /// </summary>
        /// <value><c>true</c> if [use file streams]; otherwise, <c>false</c>.</value>
        public bool UseFileStreams { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class Steam.
    /// </summary>
    public class Steam
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [use legacy launch method].
        /// </summary>
        /// <value><c>true</c> if [use legacy launch method]; otherwise, <c>false</c>.</value>
        public bool UseLegacyLaunchMethod { get; set; }

        #endregion Properties
    }
}
