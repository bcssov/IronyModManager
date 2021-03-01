// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 03-01-2021
//
// Last Modified By : Mario
// Last Modified On : 03-01-2021
// ***********************************************************************
// <copyright file="TextTypeFontFamilyConverterTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using IronyModManager.Converters;
using IronyModManager.Fonts;
using IronyModManager.Localization;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Tests
{
    /// <summary>
    /// Class FontFamilyConverterTests.
    /// </summary>
    public class TextTypeFontFamilyConverter
    {
        /// <summary>
        /// Defines the test method Should_return_font_family.
        /// </summary>
        [Fact]
        public void Should_return_supported_name_block_font_family()
        {
            DISetup.SetupContainer();
            var fontManager = new Mock<IFontFamilyManager>();
            var languageService = new Mock<ILanguagesService>();
            var family = new NotoSansSCFontFamily();
            fontManager.Setup(s => s.ResolveFontFamily(It.Is<string>(s => s == "test"))).Returns(family);
            languageService.Setup(p => p.GetLanguageBySupportedNameBlock(It.IsAny<string>())).Returns(new Language()
            {
                Abrv = "en",
                Font = "test"
            });
            DISetup.Container.RegisterInstance(fontManager.Object);
            var converter = new FontFamilyConverter();
            var result = converter.Convert("test", null, null, null);
            result.Should().Be(family.GetFontFamily());
        }

        /// <summary>
        /// Defines the test method Should_return_selected_language_block_font_family.
        /// </summary>
        [Fact]
        public void Should_return_selected_language_block_font_family()
        {
            DISetup.SetupContainer();
            var fontManager = new Mock<IFontFamilyManager>();
            var languageService = new Mock<ILanguagesService>();
            var family = new NotoSansFontFamily();
            fontManager.Setup(s => s.ResolveFontFamily(It.Is<string>(s => s == "test"))).Returns(family);
            languageService.Setup(p => p.GetLanguageBySupportedNameBlock(It.IsAny<string>())).Returns((ILanguage)null);
            languageService.Setup(p => p.GetSelected()).Returns(new Language()
            {
                Abrv = "en",
                Font = "test"
            });
            DISetup.Container.RegisterInstance(fontManager.Object);
            var converter = new FontFamilyConverter();
            var result = converter.Convert("test", null, null, null);
            result.Should().Be(family.GetFontFamily());
        }
    }
}
