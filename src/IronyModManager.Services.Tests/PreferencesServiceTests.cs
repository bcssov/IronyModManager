// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 01-31-2020
//
// Last Modified By : Mario
// Last Modified On : 02-12-2020
// ***********************************************************************
// <copyright file="PreferencesServiceTests.cs" company="Mario">
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
    /// Class PreferencesServiceTests.
    /// </summary>
    public class PreferencesServiceTests
    {
        /// <summary>
        /// Defines the test method Should_return_preferences_object.
        /// </summary>
        [Fact]
        public void Should_return_preferences_object()
        {
            var pref = new Preferences()
            {
                Locale = "test-locale"
            };            
            var storage = new Mock<IStorageProvider>();
            storage.Setup(s => s.GetPreferences()).Returns(pref);

            var service = new PreferencesService(storage.Object, new Mock<IMapper>().Object);
            var result = service.Get();
            result.Should().Be(pref);
        }

        /// <summary>
        /// Defines the test method Should_set_preferences_object.
        /// </summary>
        [Fact]
        public  void Should_set_preferences_object()
        {            
            IPreferences setPref = null;
            var storage = new Mock<IStorageProvider>();            
            storage.Setup(s => s.SetPreferences(It.IsAny<IPreferences>())).Returns<IPreferences>((p) =>
            {
                setPref = p;
                return true;
            });

            var pref = new Preferences();
            var service = new PreferencesService(storage.Object, new Mock<IMapper>().Object);
            service.Save(pref);
            setPref.Should().Be(pref);
        }
    }
}
