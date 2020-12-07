// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 12-07-2020
//
// Last Modified By : Mario
// Last Modified On : 12-07-2020
// ***********************************************************************
// <copyright file="ExternalEditorServiceTests.cs" company="Mario">
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
    /// Class ExternalEditorServiceTests.
    /// </summary>
    public class ExternalEditorServiceTests
    {
        /// <summary>
        /// Defines the test method Should_return_external_editor.
        /// </summary>
        [Fact]
        public void Should_return_external_editor()
        {
            var pref = new Preferences()
            {
                ExternalEditorLocation = "test",
                ExternalEditorParameters = "{Left} {Right}"
            };
            var preferencesService = new Mock<IPreferencesService>();
            preferencesService.Setup(p => p.Get()).Returns(() =>
            {
                return pref;
            });
            var storageProvider = new Mock<IStorageProvider>();


            var service = new ExternalEditorService(preferencesService.Object, storageProvider.Object, new Mock<IMapper>().Object);
            var result = service.Get();
            result.Should().NotBeNull();
            result.ExternalEditorParameters.Should().Be(pref.ExternalEditorParameters);
            result.ExternalEditorLocation.Should().Be(pref.ExternalEditorLocation);
        }

        /// <summary>
        /// Defines the test method Should_save_external_editor.
        /// </summary>
        [Fact]
        public void Should_save_external_editor()
        {
            DISetup.SetupContainer();
            IPreferences saved = null;
            var preferencesService = new Mock<IPreferencesService>();
            preferencesService.Setup(p => p.Save(It.IsAny<IPreferences>())).Returns((IPreferences pref) =>
            {
                saved = pref;
                return true;
            });
            var storageProvider = new Mock<IStorageProvider>();

            var service = new ExternalEditorService(preferencesService.Object, storageProvider.Object, new Mock<IMapper>().Object);
            service.Save(new ExternalEditor() { ExternalEditorLocation = "test" }).Should().BeTrue();
            saved.ExternalEditorLocation.Should().Be("test");
        }

        /// <summary>
        /// Defines the test method Should_return_launch_args.
        /// </summary>
        [Fact]
        public void Should_return_launch_args()
        {
            var pref = new Preferences()
            {
                ExternalEditorLocation = "test",
                ExternalEditorParameters = "{Left} {Right}"
            };
            var preferencesService = new Mock<IPreferencesService>();
            preferencesService.Setup(p => p.Get()).Returns(() =>
            {
                return pref;
            });
            var storageProvider = new Mock<IStorageProvider>();


            var service = new ExternalEditorService(preferencesService.Object, storageProvider.Object, new Mock<IMapper>().Object);
            var result = service.GetLaunchArguments("test1", "test2");
            result.Should().Be("test test1 test2");
        }

        /// <summary>
        /// Defines the test method Should_not_return_launch_args.
        /// </summary>
        [Fact]
        public void Should_not_return_launch_args()
        {
            var pref = new Preferences()
            {
                ExternalEditorLocation = "test",
            };
            var preferencesService = new Mock<IPreferencesService>();
            preferencesService.Setup(p => p.Get()).Returns(() =>
            {
                return pref;
            });
            var storageProvider = new Mock<IStorageProvider>();


            var service = new ExternalEditorService(preferencesService.Object, storageProvider.Object, new Mock<IMapper>().Object);
            var result = service.GetLaunchArguments("test1", "test2");
            result.Should().BeNullOrWhiteSpace();
        }
    }
}
