// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 04-16-2021
//
// Last Modified By : Mario
// Last Modified On : 11-24-2022
// ***********************************************************************
// <copyright file="PlatformConfiguration.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.Platform;
using IronyModManager.Platform.Configuration;
using IronyModManager.Shared.Configuration;
using Microsoft.Extensions.Configuration;

namespace IronyModManager.Implementation.Config
{
    /// <summary>
    /// Class PlatformConfiguration.
    /// Implements the <see cref="IronyModManager.Platform.Configuration.IPlatformConfiguration" />
    /// </summary>
    /// <seealso cref="IronyModManager.Platform.Configuration.IPlatformConfiguration" />
    public class PlatformConfiguration : IPlatformConfiguration, IDomainConfiguration
    {
        #region Fields

        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfigurationRoot configuration;

        /// <summary>
        /// The domain configuration
        /// </summary>
        private DomainConfigurationOptions domainConfiguration = null;

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
        /// <returns>DomainConfigurationOptions.</returns>
        DomainConfigurationOptions IDomainConfiguration.GetOptions()
        {
            return GetDomainOptions();
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <returns>PlatformConfigurationOptions.</returns>
        PlatformConfigurationOptions IPlatformConfiguration.GetOptions()
        {
            return GetPlatformOptions();
        }

        /// <summary>
        /// Gets the domain options.
        /// </summary>
        /// <returns>DomainConfigurationOptions.</returns>
        private DomainConfigurationOptions GetDomainOptions()
        {
            if (domainConfiguration == null)
            {
                var steamSection = configuration.GetSection("Steam");
                domainConfiguration = new DomainConfigurationOptions();
                domainConfiguration.OSXOptions.UseFileStreams = configuration.GetSection("OSXOptions").GetSection("UseFileStreams").Get<bool>();
                domainConfiguration.Steam.UseLegacyLaunchMethod = steamSection.GetSection("UseLegacyLaunchMethod").Get<bool>();
                domainConfiguration.Steam.UseGameHandler = steamSection.GetSection("UseGameHandler").Get<bool>();
                domainConfiguration.Steam.GameHandlerPath = steamSection.GetSection("GameHandlerPath").Get<string>();
                if (!Path.IsPathFullyQualified(domainConfiguration.Steam.GameHandlerPath))
                {
                    domainConfiguration.Steam.GameHandlerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, domainConfiguration.Steam.GameHandlerPath);
                }
                domainConfiguration.Formatting.UseSystemCulture = configuration.GetSection("Formatting").GetSection("UseSystemCulture").Get<bool>();
            }
            return domainConfiguration;
        }

        /// <summary>
        /// Gets the platform options.
        /// </summary>
        /// <returns>PlatformConfigurationOptions.</returns>
        /// <exception cref="System.ArgumentException">Invalid display server type. Valida values are: x11, wayland or auto</exception>
        private PlatformConfigurationOptions GetPlatformOptions()
        {
            if (platformConfiguration == null)
            {
                platformConfiguration = new PlatformConfigurationOptions();
                platformConfiguration.Logging.EnableAvaloniaLogger = configuration.GetSection("Logging").GetSection("EnableAvaloniaLogger").Get<bool?>().GetValueOrDefault();
                var linuxSection = configuration.GetSection("LinuxOptions");
                platformConfiguration.LinuxOptions.DisplayServer = linuxSection.GetSection("DisplayServer").Get<string>();
                var displayServerState = LinuxDisplayServer.IsWayland(platformConfiguration.LinuxOptions.DisplayServer) || LinuxDisplayServer.IsX11(platformConfiguration.LinuxOptions.DisplayServer) || LinuxDisplayServer.IsAuto(platformConfiguration.LinuxOptions.DisplayServer);
                if (!displayServerState)
                {
                    throw new ArgumentException("Invalid display server type. Valida values are: x11, wayland or auto");
                }
                platformConfiguration.LinuxOptions.UseGPU = linuxSection.GetSection("UseGPU").Get<bool?>();
                platformConfiguration.LinuxOptions.UseEGL = linuxSection.GetSection("UseEGL").Get<bool?>();
                platformConfiguration.LinuxOptions.UseDBusMenu = linuxSection.GetSection("UseDBusMenu").Get<bool?>();
                platformConfiguration.LinuxOptions.UseDeferredRendering = linuxSection.GetSection("UseDeferredRendering").Get<bool?>();
                platformConfiguration.Tooltips.Disable = configuration.GetSection("Tooltips").GetSection("Disable").Get<bool>();
                platformConfiguration.Fonts.UseInbuiltFontsOnly = configuration.GetSection("Fonts").GetSection("UseInbuiltFontsOnly").Get<bool>();
                platformConfiguration.Updates.Disable = configuration.GetSection("Updates").GetSection("Disable").Get<bool>();
                platformConfiguration.TitleBar.Native = configuration.GetSection("TitleBar").GetSection("Native").Get<bool>();
            }
            return platformConfiguration;
        }

        #endregion Methods
    }
}
