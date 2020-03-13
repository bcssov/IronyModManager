// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 03-03-2020
//
// Last Modified By : Mario
// Last Modified On : 03-03-2020
// ***********************************************************************
// <copyright file="AppStateServiceTests.cs" company="Mario">
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
using IronyModManager.Services;
using IronyModManager.Storage;
using IronyModManager.Storage.Common;
using Moq;
using Xunit;

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class AppStateServiceTests.
    /// </summary>
    public class AppStateServiceTests
    {
        /// <summary>
        /// Defines the test method Should_return_preferences_object.
        /// </summary>
        [Fact]
        public void Should_return_preferences_object()
        {
            var state = new AppState()
            {
                CollectionModsSearchTerm = "test"                
            };
            var storage = new Mock<IStorageProvider>();
            storage.Setup(s => s.GetAppState()).Returns(state);

            var service = new AppStateService(storage.Object, new Mock<IMapper>().Object);
            var result = service.Get();
            result.Should().Be(state);
        }

        /// <summary>
        /// Defines the test method Should_set_preferences_object.
        /// </summary>
        [Fact]
        public void Should_set_preferences_object()
        {
            IAppState setState = null;
            var storage = new Mock<IStorageProvider>();
            storage.Setup(s => s.SetAppState(It.IsAny<IAppState>())).Returns<IAppState>((p) =>
            {
                setState = p;
                return true;
            });

            var state = new AppState();
            var service = new AppStateService(storage.Object, new Mock<IMapper>().Object);
            service.Save(state);
            setState.Should().Be(state);
        }
    }
}
