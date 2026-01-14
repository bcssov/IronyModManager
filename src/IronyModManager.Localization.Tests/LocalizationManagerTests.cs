// ***********************************************************************
// Assembly         : IronyModManager.Localization.Tests
// Author           : Mario
// Created          : 02-06-2020
//
// Last Modified By : Mario
// Last Modified On : 02-06-2020
// ***********************************************************************
// <copyright file="LocalizationManagerTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using AwesomeAssertions;
using IronyModManager.Localization.ResourceProviders;
using IronyModManager.Shared.Cache;
using Moq;
using Xunit;

namespace IronyModManager.Localization.Tests
{
    /// <summary>
    /// Class LocalizationManagerTests.
    /// </summary>
    public class LocalizationManagerTests
    {
        /// <summary>
        /// Defines the test method Should_find_translation.
        /// </summary>
        [Fact]
        public void Should_find_translation()
        {
            var locManager = new LocalizationManager(new List<ILocalizationResourceProvider>() { GetProvider().Object }, new Cache());
            CurrentLocale.SetCurrent("en");
            var result = locManager.GetResource("App.Title");
            result.Should().Be("Irony Mod Manager");
        }

        /// <summary>
        /// Defines the test method Should_not_find_translation.
        /// </summary>
        [Fact]
        public void Should_not_find_translation()
        {
            var locManager = new LocalizationManager(new List<ILocalizationResourceProvider>() { GetProvider().Object }, new Cache());
            CurrentLocale.SetCurrent("en");
            var result = locManager.GetResource("App.Title2");
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_find_translation_after_locale_change.
        /// </summary>
        [Fact]
        public void Should_find_translation_after_locale_change()
        {
            var locManager = new LocalizationManager(new List<ILocalizationResourceProvider>() { GetProvider().Object }, new Cache());
            CurrentLocale.SetCurrent("en");
            CurrentLocale.SetCurrent("de");
            var result = locManager.GetResource("App.Title");
            result.Should().Be("Irony Mod Manager DE");
        }

        /// <summary>
        /// Defines the test method Should_not_find_translation_after_locale_change.
        /// </summary>
        [Fact]
        public void Should_not_find_translation_after_locale_change()
        {
            var locManager = new LocalizationManager(new List<ILocalizationResourceProvider>() { GetProvider().Object }, new Cache());
            CurrentLocale.SetCurrent("en");
            CurrentLocale.SetCurrent("de");
            var result = locManager.GetResource("App.Title2");
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_fallback_to_two_letter_iso_language.
        /// </summary>
        [Fact]
        public void Should_fallback_to_two_letter_iso_language()
        {
            var locManager = new LocalizationManager(new List<ILocalizationResourceProvider>() { GetProvider().Object }, new Cache());
            CurrentLocale.SetCurrent("de-DE");
            var result = locManager.GetResource("App.Title");
            result.Should().Be("Irony Mod Manager DE");
        }

        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <returns>Mock&lt;ILocalizationResourceProvider&gt;.</returns>
        private Mock<ILocalizationResourceProvider> GetProvider()
        {
            var provider = new Mock<ILocalizationResourceProvider>();
            provider.Setup(s => s.ReadResource(It.Is<string>(t => t == "en"))).Returns(GetEnLocale());
            provider.Setup(s => s.ReadResource(It.Is<string>(t => t == "de"))).Returns(GetDeLocale());
            provider.Setup(s => s.ReadResource(It.Is<string>(t => t == "de-DE"))).Returns(string.Empty);
            return provider;
        }

        /// <summary>
        /// Gets the en locale.
        /// </summary>
        /// <returns>System.String.</returns>
        private string GetEnLocale()
        {
            return @"{""App"": {""Title"": ""Irony Mod Manager""}}";
        }
        /// <summary>
        /// Gets the de locale.
        /// </summary>
        /// <returns>System.String.</returns>
        private string GetDeLocale()
        {
            return @"{""App"": {""Title"": ""Irony Mod Manager DE""}}";
        }
    }
}
