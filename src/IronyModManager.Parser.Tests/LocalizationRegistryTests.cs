// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 10-24-2021
//
// Last Modified By : Mario
// Last Modified On : 10-25-2021
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
using IronyModManager.Parser.Mod.Search;
using IronyModManager.Shared.Cache;
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
            var registry = new LocalizationRegistry(new Cache());
            registry.GetTranslationKeys().Length.Should().Be(8);
        }

        /// <summary>
        /// Shoulds the contain registered keys.
        /// </summary>
        [Fact]
        public void Should_contain_registered_keys()
        {
            var registry = new LocalizationRegistry(new Cache());
            registry.RegisterTranslation("en", registry.GetTranslationKeys().FirstOrDefault(), "test1");
            registry.RegisterTranslation("fr", registry.GetTranslationKeys().FirstOrDefault(), "test2");
            registry.GetTranslation("en", registry.GetTranslationKeys().FirstOrDefault()).Should().Be("test1");
            registry.GetTranslation("fr", registry.GetTranslationKeys().FirstOrDefault()).Should().Be("test2");
            registry.GetTranslations(registry.GetTranslationKeys().FirstOrDefault()).Count.Should().Be(2);
        }
    }
}
