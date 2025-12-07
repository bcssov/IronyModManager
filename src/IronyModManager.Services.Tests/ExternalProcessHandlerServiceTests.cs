// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 07-11-2022
//
// Last Modified By : Mario
// Last Modified On : 12-06-2025
// ***********************************************************************
// <copyright file="ExternalProcessHandlerServiceTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AwesomeAssertions;
using IronyModManager.IO.Common.Platforms;
using IronyModManager.Models;
using IronyModManager.Shared.Configuration;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

// ReSharper disable UnusedParameter.Local

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class SteamHandlerServiceTests.
    /// </summary>
    public class ExternalProcessHandlerServiceTests
    {
        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="launcher">The launcher.</param>
        /// <param name="steam">The steam.</param>
        /// <returns>ExternalProcessHandlerService.</returns>
        private static ExternalProcessHandlerService GetService(Mock<IParadoxLauncher> launcher = null, Mock<ISteam> steam = null)
        {
            return new ExternalProcessHandlerService(launcher?.Object, steam?.Object, new Mock<IStorageProvider>().Object, new Mock<IMapper>().Object);
        }

        /// <summary>
        /// Defines the test method Should_not_launch_steam_due_to_no_game.
        /// </summary>
        [Fact]
        public async Task Should_not_launch_steam_due_to_no_game()
        {
            var steam = new Mock<ISteam>();

            DISetup.SetupContainer();
            DISetup.Container.Register<IDomainConfiguration>(() => new DomainConfigDummy(true));

            var service = GetService(steam: steam);
            var result = await service.LaunchSteamAsync(null);
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_launch_steam_alternate.
        /// </summary>
        [Fact]
        public async Task Should_not_launch_steam_alternate()
        {
            var steam = new Mock<ISteam>();
            steam.Setup(p => p.InitAlternateAsync()).Returns(Task.FromResult(false));

            DISetup.SetupContainer();
            DISetup.Container.Register<IDomainConfiguration>(() => new DomainConfigDummy(true));

            var service = GetService(steam: steam);
            var result = await service.LaunchSteamAsync(new Game());
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_launch_steam.
        /// </summary>
        [Fact]
        public async Task Should_not_launch_steam()
        {
            var steam = new Mock<ISteam>();
            steam.Setup(p => p.InitAsync(It.IsAny<long>())).Returns(Task.FromResult(false));
            steam.Setup(p => p.ShutdownAPIAsync()).Returns(Task.FromResult(true));

            DISetup.SetupContainer();
            DISetup.Container.Register<IDomainConfiguration>(() => new DomainConfigDummy(false));

            var service = GetService(steam: steam);
            var result = await service.LaunchSteamAsync(new Game());
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_launch_steam_alternate.
        /// </summary>
        [Fact]
        public async Task Should_launch_steam_alternate()
        {
            var steam = new Mock<ISteam>();
            steam.Setup(p => p.InitAlternateAsync()).Returns(Task.FromResult(true));

            DISetup.SetupContainer();
            DISetup.Container.Register<IDomainConfiguration>(() => new DomainConfigDummy(true));

            var service = GetService(steam: steam);
            var result = await service.LaunchSteamAsync(new Game());
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_launch_steam_alternate_via_override.
        /// </summary>
        [Fact]
        public async Task Should_launch_steam_alternate_via_override()
        {
            var steam = new Mock<ISteam>();
            steam.Setup(p => p.InitAlternateAsync()).Returns(Task.FromResult(true));

            DISetup.SetupContainer();
            DISetup.Container.Register<IDomainConfiguration>(() => new DomainConfigDummy(true));

            var service = GetService(steam: steam);
            var result = await service.LaunchSteamAsync(new Game(), true);
            result.Should().BeTrue();
        }


        /// <summary>
        /// Defines the test method Should_launch_steam.
        /// </summary>
        [Fact]
        public async Task Should_launch_steam()
        {
            var steam = new Mock<ISteam>();
            steam.Setup(p => p.InitAsync(It.IsAny<long>())).Returns(Task.FromResult(true));
            steam.Setup(p => p.ShutdownAPIAsync()).Returns(Task.FromResult(true));

            DISetup.SetupContainer();
            DISetup.Container.Register<IDomainConfiguration>(() => new DomainConfigDummy(false));

            var service = GetService(steam: steam);
            var result = await service.LaunchSteamAsync(new Game());
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_launch_external_steam.
        /// </summary>
        [Fact]
        public async Task Should_launch_external_steam()
        {
            var isValid = true;
            var steam = new Mock<ISteam>();
            steam.Setup(p => p.InitAlternateAsync()).Returns(() =>
            {
                isValid = false;
                return Task.FromResult(true);
            });
            steam.Setup(p => p.InitAsync(It.IsAny<long>())).Returns((long i) =>
            {
                isValid = false;
                return Task.FromResult(true);
            });
            steam.Setup(p => p.ShutdownAPIAsync()).Returns(() =>
            {
                isValid = false;
                return Task.FromResult(true);
            });

            DISetup.SetupContainer();
            DISetup.Container.Register<IDomainConfiguration>(() => new DomainConfigDummyExternal());

            var service = GetService(steam: steam);
            await service.LaunchSteamAsync(new Game());
            isValid.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_state_paradox_launcher_is_running.
        /// </summary>
        [Fact]
        public async Task Should_not_state_paradox_launcher_is_running()
        {
            var launcher = new Mock<IParadoxLauncher>();
            launcher.Setup(p => p.IsRunningAsync()).Returns(Task.FromResult(false));

            var service = GetService(launcher);
            var result = await service.IsParadoxLauncherRunningAsync();
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_state_paradox_launcher_is_running.
        /// </summary>
        [Fact]
        public async Task Should_state_paradox_launcher_is_running()
        {
            var launcher = new Mock<IParadoxLauncher>();
            launcher.Setup(p => p.IsRunningAsync()).Returns(Task.FromResult(true));

            var service = GetService(launcher);
            var result = await service.IsParadoxLauncherRunningAsync();
            result.Should().BeTrue();
        }

        /// <summary>
        /// Class DomainConfigDummy.
        /// Implements the <see cref="IDomainConfiguration" />
        /// </summary>
        /// <seealso cref="IDomainConfiguration" />
        private class DomainConfigDummy : IDomainConfiguration
        {
            /// <summary>
            /// The domain
            /// </summary>
            private readonly DomainConfigurationOptions domain = new();

            /// <summary>
            /// Initializes a new instance of the <see cref="DomainConfigDummy" /> class.
            /// </summary>
            /// <param name="useLegacySteamLaunch">if set to <c>true</c> [use legacy steam launch].</param>
            public DomainConfigDummy(bool useLegacySteamLaunch)
            {
                domain.Steam.UseLegacyLaunchMethod = useLegacySteamLaunch;
            }

            /// <summary>
            /// Gets the options.
            /// </summary>
            /// <returns>DomainConfigurationOptions.</returns>
            public DomainConfigurationOptions GetOptions()
            {
                return domain;
            }
        }

        /// <summary>
        /// Class DomainConfigDummyExternal.
        /// Implements the <see cref="IDomainConfiguration" />
        /// </summary>
        /// <seealso cref="IDomainConfiguration" />
        private class DomainConfigDummyExternal : IDomainConfiguration
        {
            /// <summary>
            /// The domain
            /// </summary>
            private readonly DomainConfigurationOptions domain = new();

            /// <summary>
            /// Initializes a new instance of the <see cref="DomainConfigDummy" /> class.
            /// </summary>
            public DomainConfigDummyExternal()
            {
                domain.Steam.UseLegacyLaunchMethod = false;
                domain.Steam.UseGameHandler = true;
            }

            /// <summary>
            /// Gets the options.
            /// </summary>
            /// <returns>DomainConfigurationOptions.</returns>
            public DomainConfigurationOptions GetOptions()
            {
                return domain;
            }
        }
    }
}
