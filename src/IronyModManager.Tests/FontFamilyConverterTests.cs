// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 02-06-2020
//
// Last Modified By : Mario
// Last Modified On : 03-13-2021
// ***********************************************************************
// <copyright file="FontFamilyConverterTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using AwesomeAssertions;
using IronyModManager.Converters;
using IronyModManager.Fonts;
using IronyModManager.Localization;
using IronyModManager.Platform.Fonts;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Tests
{
    /// <summary>
    /// Class FontFamilyConverterTests.
    /// </summary>
    public class FontFamilyConverterTests
    {
        /// <summary>
        /// Defines the test method Should_return_font_family.
        /// </summary>
        [Fact]
        public void Should_return_font_family()
        {
            DISetup.SetupContainer();
            var fontManager = new Mock<IFontFamilyManager>();
            var family = new NotoSansFontFamily();
            fontManager.Setup(s => s.ResolveFontFamily(It.Is<string>(s => s == "test"))).Returns(family);
            DISetup.Container.RegisterInstance(fontManager.Object);
            var converter = new FontFamilyConverter();
            var result = converter.Convert("test", null, null, null);
            result.Should().Be(family.GetFontFamily());
        }

        /// <summary>
        /// Defines the test method Should_not_return_font_family.
        /// </summary>
        [Fact]
        public void Should_not_return_font_family()
        {
            DISetup.SetupContainer();
            var fontManager = new Mock<ILocalizationManager>();
            DISetup.Container.RegisterInstance(fontManager.Object);
            var converter = new FontFamilyConverter();
            var result = converter.Convert(null, null, null, null);
            result.Should().BeNull();
        }
    }
}
