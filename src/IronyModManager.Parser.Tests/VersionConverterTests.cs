// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 10-26-2021
//
// Last Modified By : Mario
// Last Modified On : 08-12-2022
// ***********************************************************************
// <copyright file="VersionConverterTests.cs" company="Mario">
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
using IronyModManager.Parser.Mod.Search;
using IronyModManager.Parser.Mod.Search.Converter;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;
using Xunit;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class VersionConverterTests.
    /// </summary>
    public class VersionConverterTests
    {
        /// <summary>
        /// Defines the test method Should_be_able_to_convert.
        /// </summary>
        [Fact]
        public void Should_be_able_to_convert()
        {
            var registry = new LocalizationRegistry(new Cache());
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Version, "en-ach");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Version, "fr-ach");

            var converter = new VersionConverter(registry);
            converter.CanConvert("en", "en-ach").Result.Should().BeTrue();
            converter.CanConvert("en", "en-ach").MappedStaticField.Should().Be(Fields.Version);
            converter.CanConvert("en", "fr-ach").Result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_be_able_to_convert.
        /// </summary>
        [Fact]
        public void Should_not_be_able_to_convert()
        {
            var registry = new LocalizationRegistry(new Cache());
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Version, "en-ach");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Version, "fr-ach");

            var converter = new VersionConverter(registry);
            converter.CanConvert("en", "en-fake").Result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_convert.
        /// </summary>
        [Fact]
        public void Should_convert()
        {
            var registry = new LocalizationRegistry(new Cache());

            var converter = new VersionConverter(registry);
            var version = converter.Convert("en", "1.0");
            version.Version.Equals(new Shared.Version(1, 0)).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_convert.
        /// </summary>
        [Fact]
        public void Should_not_convert()
        {
            var registry = new LocalizationRegistry(new Cache());

            var converter = new VersionConverter(registry);
            var version = converter.Convert("en", "test");
            version.Version.Should().BeNull();
        }
    }
}
