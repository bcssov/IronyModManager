// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 02-06-2020
//
// Last Modified By : Mario
// Last Modified On : 02-06-2020
// ***********************************************************************
// <copyright file="LocalizationConverterTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using AwesomeAssertions;
using IronyModManager.Converters;
using IronyModManager.Localization;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Tests
{
    /// <summary>
    /// Class LocalizationConverterTests.
    /// </summary>
    public class LocalizationConverterTests
    {
        /// <summary>
        /// Defines the test method Data_without_prefix_should_not_be_empty.
        /// </summary>
        [Fact]
        public void Data_without_prefix_should_not_be_empty()
        {
            DISetup.SetupContainer();
            var locManager = new Mock<ILocalizationManager>();
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s == "test"))).Returns("test localization");
            DISetup.Container.RegisterInstance(locManager.Object);
            var converter = new LocalizationConverter();
            var result = converter.Convert("test", null, null, null);
            result.Should().Be("test localization");
        }

        /// <summary>
        /// Defines the test method Data_without_prefix_should_be_empty.
        /// </summary>
        [Fact]
        public void Data_without_prefix_should_be_empty()
        {
            DISetup.SetupContainer();
            var locManager = new Mock<ILocalizationManager>();
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s == "test"))).Returns("test localization");
            DISetup.Container.RegisterInstance(locManager.Object);
            var converter = new LocalizationConverter();
            var result = converter.Convert("test2", null, null, null);
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Data_with_prefix_should_not_be_empty.
        /// </summary>
        [Fact]
        public void Data_with_prefix_should_not_be_empty()
        {
            DISetup.SetupContainer();
            var locManager = new Mock<ILocalizationManager>();
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s == "prefix.test"))).Returns("test localization");
            DISetup.Container.RegisterInstance(locManager.Object);
            var converter = new LocalizationConverter();
            var result = converter.Convert("test", null, "prefix.", null);
            result.Should().Be("test localization");
        }

        /// <summary>
        /// Defines the test method Data_with_prefix_should_be_empty.
        /// </summary>
        [Fact]
        public void Data_with_prefix_should_be_empty()
        {
            DISetup.SetupContainer();
            var locManager = new Mock<ILocalizationManager>();
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s == "prefix.test"))).Returns("test localization");
            DISetup.Container.RegisterInstance(locManager.Object);
            var converter = new LocalizationConverter();
            var result = converter.Convert("test 2", null, "prefix", null);
            result.Should().BeNull();
        }
    }
}
