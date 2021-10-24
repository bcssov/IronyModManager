// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 10-24-2021
//
// Last Modified By : Mario
// Last Modified On : 10-24-2021
// ***********************************************************************
// <copyright file="LocalizationRegistryTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using IronyModManager.Parser.Common.Mod.Search;
using IronyModManager.Parser.Games.Stellaris;
using Xunit;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class LocalizationRegistryTests.
    /// </summary>
    public class LocalizationRegistryTests
    {
        /// <summary>
        /// Defines the test method Should_have_all_translation_keys.
        /// </summary>
        [Fact]
        public void Should_have_all_translation_keys()
        {
            LocalizationRegistry.GetTranslationKeys().Count().Should().Be(8);
        }

        /// <summary>
        /// Shoulds the contain registered keys.
        /// </summary>
        [Fact]
        public void Should_contain_registered_keys()
        {
            LocalizationRegistry.RegisterTranslation("en", LocalizationRegistry.GetTranslationKeys().FirstOrDefault(), "test1");
            LocalizationRegistry.RegisterTranslation("fr", LocalizationRegistry.GetTranslationKeys().FirstOrDefault(), "test2");
            LocalizationRegistry.GetTranslation("en", LocalizationRegistry.GetTranslationKeys().FirstOrDefault()).Should().Be("test1");
            LocalizationRegistry.GetTranslation("fr", LocalizationRegistry.GetTranslationKeys().FirstOrDefault()).Should().Be("test2");
            LocalizationRegistry.GetTranslations(LocalizationRegistry.GetTranslationKeys().FirstOrDefault()).Count.Should().Be(2);
        }
    }
}
