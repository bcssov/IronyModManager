// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 10-25-2021
//
// Last Modified By : Mario
// Last Modified On : 10-26-2021
// ***********************************************************************
// <copyright file="BoolConverterTests.cs" company="Mario">
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
using IronyModManager.Parser.Mod.Search;
using IronyModManager.Parser.Mod.Search.Converter;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;
using Xunit;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class BoolConverterTests.
    /// </summary>
    public class BoolConverterTests
    {
        /// <summary>
        /// Defines the test method Should_be_able_to_convert.
        /// </summary>
        [Fact]
        public void Should_be_able_to_convert()
        {
            var registry = new LocalizationRegistry(new Cache());
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Achievements, "en-ach");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Achievements, "fr-ach");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Yes, "yes");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Yes, "fr-yes");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.True, "true");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.True, "fr-true");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.No, "no");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.No, "fr-no");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.False, "false");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.False, "fr-false");

            var converter = new BoolConverter(registry);
            converter.CanConvert("en", "en-ach").Should().BeTrue();
            converter.CanConvert("en", "fr-ach").Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_be_able_to_convert.
        /// </summary>
        [Fact]
        public void Should_not_be_able_to_convert()
        {
            var registry = new LocalizationRegistry(new Cache());
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Achievements, "en-ach");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Achievements, "fr-ach");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Yes, "yes");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Yes, "fr-yes");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.True, "true");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.True, "fr-true");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.No, "no");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.No, "fr-no");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.False, "false");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.False, "fr-false");

            var converter = new BoolConverter(registry);
            converter.CanConvert("en", "en-fake").Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_convert_to_true.
        /// </summary>
        [Fact]
        public void Should_convert_to_true()
        {
            var registry = new LocalizationRegistry(new Cache());
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Achievements, "en-ach");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Achievements, "fr-ach");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Yes, "yes");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Yes, "fr-yes");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.True, "true");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.True, "fr-true");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.No, "no");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.No, "fr-no");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.False, "false");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.False, "fr-false");

            var converter = new BoolConverter(registry);
            converter.Convert("en", "yes").Result.Should().BeTrue();
            converter.Convert("en", "fr-yes").Result.Should().BeTrue();
        }

        /// <summary>
        /// Shoulds the convert to false.
        /// </summary>
        [Fact]
        public void Should_convert_to_false()
        {
            var registry = new LocalizationRegistry(new Cache());
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Achievements, "en-ach");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Achievements, "fr-ach");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Yes, "yes");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Yes, "fr-yes");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.True, "true");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.True, "fr-true");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.No, "no");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.No, "fr-no");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.False, "false");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.False, "fr-false");

            var converter = new BoolConverter(registry);
            converter.Convert("en", "no").Result.Should().BeFalse();
            converter.Convert("en", "fr-no").Result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_convert_to_null.
        /// </summary>
        [Fact]
        public void Should_convert_to_null()
        {
            var registry = new LocalizationRegistry(new Cache());
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Achievements, "en-ach");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Achievements, "fr-ach");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Yes, "yes");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Yes, "fr-yes");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.True, "true");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.True, "fr-true");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.No, "no");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.No, "fr-no");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.False, "false");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.False, "fr-false");

            var converter = new BoolConverter(registry);
            converter.Convert("en", "empty").Result.Should().BeNull(); 
            converter.Convert("en", "fr-empty").Result.Should().BeNull();
        }
    }
}
