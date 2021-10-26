// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 10-26-2021
//
// Last Modified By : Mario
// Last Modified On : 10-26-2021
// ***********************************************************************
// <copyright file="SearchParserTests.cs" company="Mario">
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
using IronyModManager.Parser.Common.Mod.Search.Converter;
using IronyModManager.Parser.Mod.Search;
using IronyModManager.Parser.Mod.Search.Converter;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;
using IronyModManager.Tests.Common;
using Xunit;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class SearchParserTests.
    /// </summary>
    public class SearchParserTests
    {
        /// <summary>
        /// Defines the test method Should_be_able_to_convert.
        /// </summary>
        [Fact]
        public void Should_be_able_to_convert()
        {
            DISetup.SetupContainer();
            var registry = new LocalizationRegistry(new Cache());
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Achievements, "en-ach");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Achievements, "fr-ach");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Selected, "en-sel");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Selected, "fr-sel");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Yes, "yes");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Yes, "fr-yes");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.True, "true");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.True, "fr-true");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.No, "no");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.No, "fr-no");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.False, "false");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.False, "fr-false");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Version, "en-ver");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Version, "fr-ver");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Source, "en-src");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Source, "fr-src");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Paradox, "pdx");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Paradox, "fr-pdx");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Steam, "steam");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Steam, "fr-steam");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Local, "local");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Local, "fr-local");

            var parser = new Mod.Search.Parser(new Logger(), new List<ITypeConverter<object>> { new VersionConverter(registry), new BoolConverter(registry), new SourceTypeConverter(registry) });
            var line = "test test &&EN-VER:2.0 &&fr-src:steam &&en-ach:yest &&en-sel:true";
            var result = parser.Parse("en", line);
            result.Should().NotBeNull();
            result.AchievementCompatible.Result.Should().BeTrue();
            result.IsSelected.Result.Should().BeTrue();
            result.Source.Result.Should().Be(Common.Mod.Search.SourceType.Steam);
            result.Version.Equals(new Version(2, 0)).Should().BeTrue();
            result.Name.Should().Be("test test");
        }

        /// <summary>
        /// Defines the test method Should_be_able_to_convert.
        /// </summary>
        [Fact]
        public void Should_be_able_to_convert_partially()
        {
            DISetup.SetupContainer();
            var registry = new LocalizationRegistry(new Cache());
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Achievements, "en-ach");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Achievements, "fr-ach");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Selected, "en-sel");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Selected, "fr-sel");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Yes, "yes");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Yes, "fr-yes");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.True, "true");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.True, "fr-true");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.No, "no");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.No, "fr-no");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.False, "false");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.False, "fr-false");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Version, "en-ver");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Version, "fr-ver");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Source, "en-src");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Source, "fr-src");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Paradox, "pdx");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Paradox, "fr-pdx");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Steam, "steam");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Steam, "fr-steam");
            registry.RegisterTranslation("en", LocalizationResources.FilterCommands.Local, "local");
            registry.RegisterTranslation("fr", LocalizationResources.FilterCommands.Local, "fr-local");

            var parser = new Mod.Search.Parser(new Logger(), new List<ITypeConverter<object>> { new VersionConverter(registry), new BoolConverter(registry), new SourceTypeConverter(registry) });
            var line = "test test";
            var result = parser.Parse("en", line);
            result.Should().NotBeNull();
            result.AchievementCompatible.Result.Should().BeNull();
            result.IsSelected.Result.Should().BeNull();
            result.Source.Result.Should().Be(Common.Mod.Search.SourceType.None);
            result.Version.Should().BeNull();
            result.Name.Should().Be("test test");
        }
    }
}
