// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 10-26-2021
//
// Last Modified By : Mario
// Last Modified On : 10-26-2021
// ***********************************************************************
// <copyright file="SourceTypeConverterTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwesomeAssertions;
using IronyModManager.Parser.Common.Mod.Search;
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
    public class SourceTypeConverterTests
    {
        /// <summary>
        /// Defines the test method Should_be_able_to_convert.
        /// </summary>
        [Fact]
        public void Should_be_able_to_convert()
        {
            var registry = new LocalizationRegistry(new Cache());
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Source, "en-ach");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Source, "fr-ach");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Paradox, "pdx");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Paradox, "fr-pdx");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Steam, "steam");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Steam, "fr-steam");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Local, "local");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Local, "fr-local");

            var converter = new SourceTypeConverter(registry);
            converter.CanConvert("en", "en-ach").Result.Should().BeTrue();
            converter.CanConvert("en", "en-ach").MappedStaticField.Should().Be(Fields.Source);
            converter.CanConvert("en", "fr-ach").Result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_be_able_to_convert.
        /// </summary>
        [Fact]
        public void Should_not_be_able_to_convert()
        {
            var registry = new LocalizationRegistry(new Cache());
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Source, "en-ach");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Source, "fr-ach");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Paradox, "pdx");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Paradox, "fr-pdx");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Steam, "steam");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Steam, "fr-steam");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Local, "local");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Local, "fr-local");

            var converter = new SourceTypeConverter(registry);
            converter.CanConvert("en", "en-fake").Result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_convert_to_steam.
        /// </summary>
        [Fact]
        public void Should_convert_to_steam()
        {
            var registry = new LocalizationRegistry(new Cache());
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Source, "en-ach");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Source, "fr-ach");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Paradox, "pdx");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Paradox, "fr-pdx");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Steam, "steam");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Steam, "fr-steam");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Local, "local");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Local, "fr-local");

            var converter = new SourceTypeConverter(registry);
            converter.Convert("en", "steam").Result.Should().Be(SourceType.Steam);
            converter.Convert("en", "fr-steam").Result.Should().Be(SourceType.Steam);
        }

        /// <summary>
        /// Defines the test method Should_convert_to_paradox.
        /// </summary>
        [Fact]
        public void Should_convert_to_paradox()
        {
            var registry = new LocalizationRegistry(new Cache());
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Source, "en-ach");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Source, "fr-ach");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Paradox, "pdx");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Paradox, "fr-pdx");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Steam, "steam");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Steam, "fr-steam");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Local, "local");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Local, "fr-local");

            var converter = new SourceTypeConverter(registry);
            converter.Convert("en", "pdx").Result.Should().Be(SourceType.Paradox);
            converter.Convert("en", "fr-pdx").Result.Should().Be(SourceType.Paradox);
        }

        /// <summary>
        /// Defines the test method Should_convert_to_local.
        /// </summary>
        [Fact]
        public void Should_convert_to_local()
        {
            var registry = new LocalizationRegistry(new Cache());
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Source, "en-ach");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Source, "fr-ach");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Paradox, "pdx");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Paradox, "fr-pdx");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Steam, "steam");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Steam, "fr-steam");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Local, "local");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Local, "fr-local");

            var converter = new SourceTypeConverter(registry);
            converter.Convert("en", "local").Result.Should().Be(SourceType.Local);
            converter.Convert("en", "fr-local").Result.Should().Be(SourceType.Local);
        }

        /// <summary>
        /// Defines the test method Should_not_convert.
        /// </summary>
        [Fact]
        public void Should_not_convert()
        {
            var registry = new LocalizationRegistry(new Cache());
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Source, "en-ach");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Source, "fr-ach");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Paradox, "pdx");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Paradox, "fr-pdx");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Steam, "steam");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Steam, "fr-steam");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Local, "local");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Local, "fr-local");

            var converter = new SourceTypeConverter(registry);
            converter.Convert("en", "dummy").Result.Should().Be(SourceType.None);
            converter.Convert("en", "fr-dummy").Result.Should().Be(SourceType.None);
        }
    }
}
