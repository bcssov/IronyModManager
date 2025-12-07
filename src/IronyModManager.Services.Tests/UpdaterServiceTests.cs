// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 02-04-2020
//
// Last Modified By : Mario
// Last Modified On : 11-02-2022
// ***********************************************************************
// <copyright file="UpdaterServiceTests.cs" company="Mario">
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
using AwesomeAssertions;
using IronyModManager.IO.Common.Updater;
using IronyModManager.Localization;
using IronyModManager.Localization.ResourceProviders;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class LanguageServiceTests.
    /// </summary>
    public class UpdaterServiceTests
    {

        /// <summary>
        /// Setups the mocks.
        /// </summary>
        /// <param name="preferencesService">The preferences service.</param>
        private static void SetupMocks(Mock<IPreferencesService> preferencesService)
        {
            DISetup.SetupContainer();
            CurrentLocale.SetCurrent("en");
            preferencesService.Setup(p => p.Get()).Returns(() =>
            {
                return new Preferences()
                {
                    AutoUpdates = true,
                    CheckForPrerelease = false
                };
            });
            preferencesService.Setup(p => p.Save(It.IsAny<IPreferences>())).Returns(true);
        }



        /// <summary>
        /// Shoulds the contain selected language.
        /// </summary>
        [Fact]
        public void Should_return_updater_settings()
        {
            var unpacker = new Mock<IUnpacker>();
            var mapper = new Mock<IMapper>();
            var preferencesService = new Mock<IPreferencesService>();
            mapper.Setup(s => s.Map<IUpdateSettings>(It.IsAny<IPreferences>())).Returns((IPreferences o) =>
            {
                return new UpdateSettings()
                {
                    AutoUpdates = o.AutoUpdates,
                    CheckForPrerelease = o.CheckForPrerelease,
                    LastSkippedVersion = "1.2.3"
                };
            });
            SetupMocks(preferencesService);

            var service = new UpdaterService(unpacker.Object, preferencesService.Object, new Mock<IStorageProvider>().Object, mapper.Object);
            var result = service.Get();
            result.CheckForPrerelease.Should().BeFalse();
            result.AutoUpdates.Should().BeTrue();
            result.LastSkippedVersion.Should().Be("1.2.3");
        }



        /// <summary>
        /// Defines the test method Should_save_selected_language.
        /// </summary>
        [Fact]
        public void Should_save_updater_settings()
        {
            var unpacker = new Mock<IUnpacker>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMocks(preferencesService);

            var service = new UpdaterService(unpacker.Object, preferencesService.Object, new Mock<IStorageProvider>().Object, new Mock<IMapper>().Object);
            var result = service.Save(new UpdateSettings()
            {
                CheckForPrerelease = true,
                AutoUpdates = false,
                LastSkippedVersion = "3.2.1"
            });
            result.Should().Be(true);
        }

        /// <summary>
        /// Defines the test method Should_unpack_update.
        /// </summary>
        [Fact]
        public async Task Should_unpack_update()
        {
            var unpacker = new Mock<IUnpacker>();
            unpacker.Setup(p => p.UnpackUpdateAsync(It.IsAny<string>())).Returns(Task.FromResult("test"));
            var preferencesService = new Mock<IPreferencesService>();
            SetupMocks(preferencesService);

            var service = new UpdaterService(unpacker.Object, preferencesService.Object, new Mock<IStorageProvider>().Object, new Mock<IMapper>().Object);
            var result = await service.UnpackUpdateAsync("path");
            result.Should().Be("test");
        }
    }
}
