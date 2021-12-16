// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 04-16-2021
//
// Last Modified By : Mario
// Last Modified On : 12-16-2021
// ***********************************************************************
// <copyright file="PlatformConfiguration.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Platform.Configuration;
using Microsoft.Extensions.Configuration;

namespace IronyModManager.Implementation.Config
{
    /// <summary>
    /// Class PlatformConfiguration.
    /// Implements the <see cref="IronyModManager.Platform.Configuration.IPlatformConfiguration" />
    /// </summary>
    /// <seealso cref="IronyModManager.Platform.Configuration.IPlatformConfiguration" />
    public class PlatformConfiguration : IPlatformConfiguration
    {
        #region Fields

        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfigurationRoot configuration;

        /// <summary>
        /// The platform configuration
        /// </summary>
        private PlatformConfigurationOptions platformConfiguration = null;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformConfiguration" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public PlatformConfiguration(IConfigurationRoot configuration)
        {
            this.configuration = configuration;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <returns>PlatformConfigurationOptions.</returns>
        public PlatformConfigurationOptions GetOptions()
        {
            if (platformConfiguration == null)
            {
                platformConfiguration = new PlatformConfigurationOptions();
                platformConfiguration.Logging.EnableAvaloniaLogger = configuration.GetSection("Logging").GetSection("EnableAvaloniaLogger").Get<bool?>().GetValueOrDefault();
                var linuxSection = configuration.GetSection("LinuxOptions");
                platformConfiguration.LinuxOptions.UseGPU = linuxSection.GetSection("UseGPU").Get<bool?>();
                platformConfiguration.LinuxOptions.UseEGL = linuxSection.GetSection("UseEGL").Get<bool?>();
                platformConfiguration.LinuxOptions.UseDBusMenu = linuxSection.GetSection("UseDBusMenu").Get<bool?>();
                platformConfiguration.LinuxOptions.UseDeferredRendering = linuxSection.GetSection("UseDeferredRendering").Get<bool?>();
                platformConfiguration.Tooltips.Disable = configuration.GetSection("Tooltips").GetSection("Disable").Get<bool>();
                platformConfiguration.Fonts.UseInbuiltFontsOnly = configuration.GetSection("Fonts").GetSection("UseInbuiltFontsOnly").Get<bool>();
                platformConfiguration.Updates.Disable = configuration.GetSection("Updates").GetSection("Disable").Get<bool>();
            }
            return platformConfiguration;
        }

        #endregion Methods
    }
}
