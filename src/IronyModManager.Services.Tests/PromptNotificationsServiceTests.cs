// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 03-26-2021
//
// Last Modified By : Mario
// Last Modified On : 03-26-2021
// ***********************************************************************
// <copyright file="PromptNotificationsServiceTests.cs" company="Mario">
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
using IronyModManager.Localization;
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
    /// Class PromptNotificationsServiceTests.
    /// </summary>
    public class PromptNotificationsServiceTests
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
                    ConflictSolverPromptShown = false
                };
            });
            preferencesService.Setup(p => p.Save(It.IsAny<IPreferences>())).Returns(true);
        }



        /// <summary>
        /// Defines the test method Should_return_object.
        /// </summary>
        [Fact]
        public void Should_return_object()
        {
            var mapper = new Mock<IMapper>();
            var preferencesService = new Mock<IPreferencesService>();
            mapper.Setup(s => s.Map<IPromptNotifications>(It.IsAny<IPreferences>())).Returns((IPreferences o) =>
            {
                return new PromptNotifications()
                {
                    ConflictSolverPromptShown = false
                };
            });
            SetupMocks(preferencesService);

            var service = new PromptNotificationsService(preferencesService.Object, new Mock<IStorageProvider>().Object, mapper.Object);
            var result = service.Get();
            result.ConflictSolverPromptShown.Should().BeFalse();
        }



        /// <summary>
        /// Defines the test method Should_save_object.
        /// </summary>
        [Fact]
        public void Should_save_object()
        {
            var preferencesService = new Mock<IPreferencesService>();
            SetupMocks(preferencesService);

            var service = new PromptNotificationsService(preferencesService.Object, new Mock<IStorageProvider>().Object, new Mock<IMapper>().Object);
            var result = service.Save(new PromptNotifications()
            {
                ConflictSolverPromptShown = true
            });
            result.Should().Be(true);
        }
    }
}
