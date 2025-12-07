// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 03-16-2021
//
// Last Modified By : Mario
// Last Modified On : 03-16-2021
// ***********************************************************************
// <copyright file="NotificationPositionSettingsServiceTests.cs" company="Mario">
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
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Storage;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class NotificationPositionSettingsServiceTests.
    /// </summary>
    public class NotificationPositionSettingsServiceTests
    {
        /// <summary>
        /// Setups the mock case.
        /// </summary>
        /// <param name="preferencesService">The preferences service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        private static void SetupMockCase(Mock<IPreferencesService> preferencesService, Mock<IStorageProvider> storageProvider)
        {
            DISetup.SetupContainer();
            preferencesService.Setup(p => p.Get()).Returns(() =>
            {
                return new Preferences()
                {
                    NotificationPosition = IronyModManager.Models.Common.NotificationPosition.TopRight
                };
            });
            preferencesService.Setup(p => p.Save(It.IsAny<IPreferences>())).Returns(true);
            var notifications = new List<INotificationPositionType>
            {
                new NotificationPositionType() { Position = IronyModManager.Models.Common.NotificationPosition.BottomLeft, IsDefault = false },
                new NotificationPositionType() { Position = IronyModManager.Models.Common.NotificationPosition.BottomRight, IsDefault = true },
                new NotificationPositionType() { Position = IronyModManager.Models.Common.NotificationPosition.TopLeft, IsDefault = false },
                new NotificationPositionType() { Position = IronyModManager.Models.Common.NotificationPosition.TopRight, IsDefault = false },
            };
            storageProvider.Setup(p => p.GetNotificationPositions()).Returns(notifications);
        }

        /// <summary>
        /// Defines the test method Should_contain_all_notifications.
        /// </summary>
        [Fact]
        public void Should_contain_all_notifications()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new NotificationPositionSettingsService(preferencesService.Object, storageProvider.Object, new Mock<IMapper>().Object);
            var result = service.Get();
            result.Count().Should().Be(4);
            result.GroupBy(p => p.Type).Select(p => p.First()).Count().Should().Be(4);
        }

        /// <summary>
        /// Defines the test method Should_contain_selected_notification.
        /// </summary>
        [Fact]
        public void Should_contain_selected_notification()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new NotificationPositionSettingsService(preferencesService.Object, storageProvider.Object, new Mock<IMapper>().Object);
            var result = service.Get();
            result.FirstOrDefault(p => p.IsSelected).Should().NotBeNull();
            result.FirstOrDefault(p => p.IsSelected).Type.Should().Be(IronyModManager.Models.Common.NotificationPosition.TopRight);
        }

        /// <summary>
        /// Defines the test method Should_save_selected_notification.
        /// </summary>
        [Fact]
        public void Should_save_selected_notification()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new NotificationPositionSettingsService(preferencesService.Object, storageProvider.Object, new Mock<IMapper>().Object);
            var result = service.Save(new IronyModManager.Models.NotificationPosition()
            {
                IsSelected = true,
                Type = IronyModManager.Models.Common.NotificationPosition.TopRight
            });
            result.Should().Be(true);
        }

        /// <summary>
        /// Defines the test method Should_throw_exception_when_saving_non_selected_notification.
        /// </summary>
        [Fact]
        public void Should_throw_exception_when_saving_non_selected_notification()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            bool exceptionThrown = false;
            var service = new NotificationPositionSettingsService(preferencesService.Object, storageProvider.Object, new Mock<IMapper>().Object);
            try
            {
                service.Save(new IronyModManager.Models.NotificationPosition()
                {
                    IsSelected = false,
                    Type = IronyModManager.Models.Common.NotificationPosition.TopRight
                });
            }
            catch (Exception ex)
            {
                ex.GetType().Should().Be(typeof(InvalidOperationException));
                exceptionThrown = true;
            }
            exceptionThrown.Should().BeTrue();
        }
    }
}
