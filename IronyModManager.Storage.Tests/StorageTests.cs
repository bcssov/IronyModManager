// ***********************************************************************
// Assembly         : IronyModManager.Storage.Tests
// Author           : Mario
// Created          : 01-28-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2020
// ***********************************************************************
// <copyright file="StorageTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using IronyModManager.Models;
using AutoMapper;
using IronyModManager.Models.Common;
using FluentAssertions;
using SimpleInjector;
using IronyModManager.DI;
using IronyModManager.Shared;

namespace IronyModManager.Storage.Tests
{
    /// <summary>
    /// Class StorageTests.
    /// </summary>
    public class StorageTests
    {

        /// <summary>
        /// Defines the test method Should_return_same_preferences_object.
        /// </summary>
        [Fact]
        public void Should_return_same_preferences_object()
        {
            // I know totally redundant test, done just for a bit of practice
            SetupContainer();
            var mapper = new Mock<IMapper>();
            mapper.Setup(p => p.Map<IPreferences, IPreferences>(It.IsAny<IPreferences>())).Returns(GetDbMock().Preferences);
            var storage = new Storage(GetDbMock(), mapper.Object);
            var pref = storage.GetPreferences();
            pref.Locale.Should().Be(GetDbMock().Preferences.Locale);
        }


        /// <summary>
        /// Defines the test method Should_return_same_window_state_object.
        /// </summary>
        [Fact]
        public void Should_return_same_window_state_object()
        {
            // I know totally redundant test, done just for a bit of practice
            SetupContainer();
            var mapper = new Mock<IMapper>();
            mapper.Setup(p => p.Map<IWindowState, IWindowState>(It.IsAny<IWindowState>())).Returns(GetDbMock().WindowState);
            var storage = new Storage(GetDbMock(), mapper.Object);
            var state = storage.GetWindowState();
            state.IsMaximized.Should().Be(GetDbMock().WindowState.IsMaximized);
        }

        /// <summary>
        /// Defines the test method Should_overwrite_preferences_object.
        /// </summary>
        [Fact]
        public void Should_overwrite_preferences_object()
        {
            SetupContainer();
            var dbMock = GetDbMock();
            var newPref = new Preferences()
            {
                Locale = "test2"
            };
            var storage = new Storage(dbMock, new Mock<IMapper>().Object);
            storage.SetPreferences(newPref);
            dbMock.Preferences.Should().Be(newPref);
        }

        /// <summary>
        /// Defines the test method Should_overwrite_window_state_object.
        /// </summary>
        [Fact]
        public void Should_overwrite_window_state_object()
        {
            SetupContainer();
            var dbMock = GetDbMock();
            var state = new WindowState()
            {
                Height = 300
            };
            var storage = new Storage(dbMock, new Mock<IMapper>().Object);
            storage.SetWindowState(state);
            dbMock.WindowState.Should().Be(state);
        }

        /// <summary>
        /// Setups the container.
        /// </summary>
        private void SetupContainer()
        {
            var container = new Container();
            Bootstrap.Setup(
                new DIOptions()
                {
                    Container = container,
                    PluginPathAndName = Constants.PluginsPathAndName,
                    ModuleTypes = new List<Type>() { typeof(IModule) },
                    PluginTypes = new List<Type>() { typeof(IPlugin) }
                });
        }

        /// <summary>
        /// Gets the database mock.
        /// </summary>
        /// <returns>Database.</returns>
        private Database GetDbMock()
        {
            return new Database()
            {
                Preferences = new Preferences() { Locale = "test" },
                WindowState = new WindowState() { IsMaximized = true }
            };
        }
    }
}
