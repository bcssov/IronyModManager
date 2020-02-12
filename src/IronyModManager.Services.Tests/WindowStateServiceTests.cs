// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 02-04-2020
//
// Last Modified By : Mario
// Last Modified On : 02-12-2020
// ***********************************************************************
// <copyright file="WindowStateServiceTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using FluentAssertions;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Storage.Common;
using Moq;
using Xunit;

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class WindowStateServiceTests.
    /// </summary>
    public class WindowStateServiceTests
    {
        /// <summary>
        /// Defines the test method Should_return_preferences_object.
        /// </summary>
        [Fact]
        public void Should_return_window_state_object()
        {
            var state = new WindowState()
            {
                Height = 300
            };
            var storage = new Mock<IStorageProvider>();
            storage.Setup(s => s.GetWindowState()).Returns(state);

            var service = new WindowStateService(storage.Object, new Mock<IMapper>().Object);
            var result = service.Get();
            result.Should().Be(state);
        }

        /// <summary>
        /// Defines the test method Window_state_should_be_defined.
        /// </summary>
        [Fact]
        public void Window_state_should_be_defined()
        {
            var state = new WindowState()
            {
                Height = 300,
                LocationX = 1,
                LocationY = 1,
                Width = 300
            };
            var storage = new Mock<IStorageProvider>();
            storage.Setup(s => s.GetWindowState()).Returns(state);
            var service = new WindowStateService(storage.Object, new Mock<IMapper>().Object);
            service.IsDefined().Should().Be(true);
        }

        /// <summary>
        /// Defines the test method Window_state_should_not_be_defined.
        /// </summary>
        [Fact]
        public void Window_state_should_not_be_defined()
        {
            var state = new WindowState()
            {
                Height = 300,
                LocationY = 1,
                Width = 300
            };
            var storage = new Mock<IStorageProvider>();
            storage.Setup(s => s.GetWindowState()).Returns(state);
            var service = new WindowStateService(storage.Object, new Mock<IMapper>().Object);
            service.IsDefined().Should().Be(false);
        }

        /// <summary>
        /// Defines the test method Window_state_should_be_maximized.
        /// </summary>
        [Fact]
        public void Window_state_should_be_maximized()
        {
            var state = new WindowState()
            {
                IsMaximized = true
            };
            var storage = new Mock<IStorageProvider>();
            storage.Setup(s => s.GetWindowState()).Returns(state);
            var service = new WindowStateService(storage.Object, new Mock<IMapper>().Object);
            service.IsMaximized().Should().Be(true);
        }

        /// <summary>
        /// Defines the test method Window_state_should_not_be_maximized.
        /// </summary>
        [Fact]
        public void Window_state_should_not_be_maximized()
        {
            var state = new WindowState();            
            var storage = new Mock<IStorageProvider>();
            storage.Setup(s => s.GetWindowState()).Returns(state);
            var service = new WindowStateService(storage.Object, new Mock<IMapper>().Object);
            service.IsMaximized().Should().Be(false);
        }

        /// <summary>
        /// Defines the test method Should_save_window_state.
        /// </summary>
        [Fact]
        public void Should_save_window_state()
        {            
            var storage = new Mock<IStorageProvider>();
            storage.Setup(s => s.SetWindowState(It.IsAny<IWindowState>())).Returns(true);
            var service = new WindowStateService(storage.Object, new Mock<IMapper>().Object);
            service.Save(new WindowState()).Should().Be(true);
        }
    }
}
