// ***********************************************************************
// Assembly         : IronyModManager.Storage.Tests
// Author           : Mario
// Created          : 01-28-2020
//
// Last Modified By : Mario
// Last Modified On : 01-31-2020
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
using IronyModManager.Tests.Common;

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
            DISetup.SetupContainer();
            var dbMock = GetDbMock();
            var mapper = new Mock<IMapper>();
            mapper.Setup(p => p.Map<IPreferences, IPreferences>(It.IsAny<IPreferences>())).Returns(dbMock.Preferences);
            var storage = new Storage(dbMock, mapper.Object);
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
            DISetup.SetupContainer();
            var dbMock = GetDbMock();
            var mapper = new Mock<IMapper>();
            mapper.Setup(p => p.Map<IWindowState, IWindowState>(It.IsAny<IWindowState>())).Returns(dbMock.WindowState);
            var storage = new Storage(dbMock, mapper.Object);
            var state = storage.GetWindowState();
            state.IsMaximized.Should().Be(GetDbMock().WindowState.IsMaximized);
        }

        /// <summary>
        /// Defines the test method Should_overwrite_preferences_object.
        /// </summary>
        [Fact]
        public void Should_overwrite_preferences_object()
        {
            DISetup.SetupContainer();
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
            DISetup.SetupContainer();
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
        /// Defines the test method Should_overwrite_and_return_same_preferences_object.
        /// </summary>
        [Fact]
        public void Should_overwrite_and_return_same_preferences_object()
        {
            DISetup.SetupContainer();
            var newPref = new Preferences()
            {
                Locale = "test2"
            };                        
            var storage = new Storage(GetDbMock(), DIResolver.Get<IMapper>());
            storage.SetPreferences(newPref);
            var pref = storage.GetPreferences();
            pref.Locale.Should().Be(newPref.Locale);
        }

        /// <summary>
        /// Defines the test method Should_overwrite_and_return_same_window_state_object.
        /// </summary>
        [Fact]
        public void Should_overwrite_and_return_same_window_state_object()
        {
            DISetup.SetupContainer();
            var newState = new WindowState()
            {
                Height = 300
            };
            var storage = new Storage(GetDbMock(), DIResolver.Get<IMapper>());
            storage.SetWindowState(newState);
            var state = storage.GetWindowState();
            state.Height.Should().Be(newState.Height);

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
