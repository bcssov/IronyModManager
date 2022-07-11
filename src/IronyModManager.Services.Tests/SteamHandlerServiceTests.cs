// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 07-11-2022
//
// Last Modified By : Mario
// Last Modified On : 07-11-2022
// ***********************************************************************
// <copyright file="SteamHandlerServiceTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using IronyModManager.IO.Common.Platforms;
using IronyModManager.Storage.Common;
using Moq;
using Xunit;

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class SteamHandlerServiceTests.
    /// </summary>
    public class SteamHandlerServiceTests
    {
        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="steam">The steam.</param>
        /// <returns>SteamHandlerService.</returns>
        private static SteamHandlerService GetService(Mock<ISteam> steam)
        {
            return new SteamHandlerService(steam.Object, new Mock<IStorageProvider>().Object, new Mock<IMapper>().Object);
        }

        /// <summary>
        /// Defines the test method Should_not_launch_steam_due_to_no_game.
        /// </summary>
        [Fact]
        public async Task Should_not_launch_steam_due_to_no_game()
        {
            var steam = new Mock<ISteam>();

            var service = GetService(steam);
            var result = await service.LaunchSteamAsync(true, null);
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

            var service = GetService(steam);
            var result = await service.LaunchSteamAsync(true, null);
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_launch_steam.
        /// </summary>
        [Fact]
        public async Task Should_not_launch_steam()
        {
            var steam = new Mock<ISteam>();
            steam.Setup(p => p.InitAsync(It.IsAny<int>())).Returns(Task.FromResult(false));
            steam.Setup(p => p.ShutdownAPIAsync()).Returns(Task.FromResult(true));

            var service = GetService(steam);
            var result = await service.LaunchSteamAsync(false, null);
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

            var service = GetService(steam);
            var result = await service.LaunchSteamAsync(true, null);
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_launch_steam.
        /// </summary>
        [Fact]
        public async Task Should_launch_steam()
        {
            var steam = new Mock<ISteam>();
            steam.Setup(p => p.InitAsync(It.IsAny<int>())).Returns(Task.FromResult(true));
            steam.Setup(p => p.ShutdownAPIAsync()).Returns(Task.FromResult(true));

            var service = GetService(steam);
            var result = await service.LaunchSteamAsync(false, null);

        }
    }
}
