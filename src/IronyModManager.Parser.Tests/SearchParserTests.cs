// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 10-26-2021
//
// Last Modified By : Mario
// Last Modified On : 07-12-2022
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
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.OrStatementSeparator, "||");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.StatementSeparator, "&&");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.ValueSeparator, ":");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.Negate, "--");

            var parser = new Mod.Search.Parser(new Cache(), new Logger(), new List<ITypeConverter<object>> { new VersionConverter(registry), new BoolConverter(registry), new SourceTypeConverter(registry) }, registry);
            var line = "test test &&EN-VER:2.0 &&fr-src:steam &&en-ach:yest &&en-sel:true";
            var result = parser.Parse("en", line);
            result.Should().NotBeNull();
            result.AchievementCompatible.Result.Should().BeTrue();
            result.IsSelected.Result.Should().BeTrue();
            result.Source.Count.Should().Be(1);
            result.Source.FirstOrDefault().Result.Should().Be(Common.Mod.Search.SourceType.Steam);
            result.Version.Count.Should().Be(1);
            result.Version.First().Version.Equals(new Shared.Version(2, 0)).Should().BeTrue();
            result.Name.Count.Should().Be(1);
            result.Name.FirstOrDefault().Negate.Should().BeFalse();
            result.Name.FirstOrDefault().Text.Should().Be("test test");
        }

        /// <summary>
        /// Defines the test method Should_be_able_to_recognize_text_search.
        /// </summary>
        [Fact]
        public void Should_be_able_to_recognize_text_search()
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
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.OrStatementSeparator, "||");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.StatementSeparator, "&&");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.ValueSeparator, ":");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.Negate, "--");

            var parser = new Mod.Search.Parser(new Cache(), new Logger(), new List<ITypeConverter<object>> { new VersionConverter(registry), new BoolConverter(registry), new SourceTypeConverter(registry) }, registry);
            var line = "test test";
            var result = parser.Parse("en", line);
            result.Should().NotBeNull();
            result.AchievementCompatible.Result.Should().BeNull();
            result.IsSelected.Result.Should().BeNull();
            result.Source.Count.Should().Be(0);            
            result.Version.Count.Should().Be(0);
            result.Name.Count.Should().Be(1);
            result.Name.FirstOrDefault().Negate.Should().BeFalse();
            result.Name.FirstOrDefault().Text.Should().Be("test test");
        }

        /// <summary>
        /// Defines the test method Should_trim_search_text.
        /// </summary>
        [Fact]
        public void Should_trim_search_text()
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
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.OrStatementSeparator, "||");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.StatementSeparator, "&&");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.ValueSeparator, ":");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.Negate, "--");

            var parser = new Mod.Search.Parser(new Cache(), new Logger(), new List<ITypeConverter<object>> { new VersionConverter(registry), new BoolConverter(registry), new SourceTypeConverter(registry) }, registry);
            var line = "test test ";
            var result = parser.Parse("en", line);
            result.Should().NotBeNull();
            result.AchievementCompatible.Result.Should().BeNull();
            result.IsSelected.Result.Should().BeNull();
            result.Source.Count.Should().Be(0);
            result.Version.Count.Should().Be(0);
            result.Name.Count.Should().Be(1);
            result.Name.FirstOrDefault().Negate.Should().BeFalse();
            result.Name.FirstOrDefault().Text.Should().Be("test test");
        }

        /// <summary>
        /// Defines the test method Should_filter_out_duplicates.
        /// </summary>
        [Fact]
        public void Should_filter_out_duplicates()
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
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.OrStatementSeparator, "||");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.StatementSeparator, "&&");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.ValueSeparator, ":");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.Negate, "--");

            var parser = new Mod.Search.Parser(new Cache(), new Logger(), new List<ITypeConverter<object>> { new VersionConverter(registry), new BoolConverter(registry), new SourceTypeConverter(registry) }, registry);
            var line = "en-ach:yes && en-ach:no";
            var result = parser.Parse("en", line);
            result.Should().NotBeNull();
            result.AchievementCompatible.Result.GetValueOrDefault().Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_treat_close_matches_as_search_text.
        /// </summary>
        [Fact]
        public void Should_treat_close_matches_as_search_text()
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
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.OrStatementSeparator, "||");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.StatementSeparator, "&&");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.ValueSeparator, ":");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.Negate, "--");

            var parser = new Mod.Search.Parser(new Cache(), new Logger(), new List<ITypeConverter<object>> { new VersionConverter(registry), new BoolConverter(registry), new SourceTypeConverter(registry) }, registry);
            var line = "en-ach";
            var result = parser.Parse("en", line);
            result.Should().NotBeNull();
            result.AchievementCompatible.Result.Should().BeNull();
            result.Name.Count.Should().Be(1);
            result.Name.FirstOrDefault().Negate.Should().BeFalse();            
            result.Name.FirstOrDefault().Text.Should().Be("en-ach");
        }

        /// <summary>
        /// Defines the test method Should_treat_close_separator_matches_as_search_text.
        /// </summary>
        [Fact]
        public void Should_treat_close_separator_matches_as_search_text()
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
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.OrStatementSeparator, "||");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.StatementSeparator, "&&");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.ValueSeparator, ":");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.Negate, "--");

            var parser = new Mod.Search.Parser(new Cache(), new Logger(), new List<ITypeConverter<object>> { new VersionConverter(registry), new BoolConverter(registry), new SourceTypeConverter(registry) }, registry);
            var line = "test:test";
            var result = parser.Parse("en", line);
            result.Should().NotBeNull();
            result.Name.Count.Should().Be(1);
            result.Name.FirstOrDefault().Negate.Should().BeFalse();
            result.Name.FirstOrDefault().Text.Should().Be("test:test");

            line = "test:test:test";
            result = parser.Parse("en", line);
            result.Should().NotBeNull();
            result.Name.Count.Should().Be(1);
            result.Name.FirstOrDefault().Negate.Should().BeFalse();
            result.Name.FirstOrDefault().Text.Should().Be("test:test:test");
        }

        /// <summary>
        /// Defines the test method Should_be_able_to_parse_or_operator.
        /// </summary>
        [Fact]
        public void Should_be_able_to_parse_or_operator()
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
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.OrStatementSeparator, "||");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.StatementSeparator, "&&");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.ValueSeparator, ":");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.Negate, "--");

            var parser = new Mod.Search.Parser(new Cache(), new Logger(), new List<ITypeConverter<object>> { new VersionConverter(registry), new BoolConverter(registry), new SourceTypeConverter(registry) }, registry);
            var line = "test test &&EN-VER:2.0||3.0 &&fr-src:steam &&en-ach:yest &&en-sel:true";
            var result = parser.Parse("en", line);
            result.Should().NotBeNull();
            result.AchievementCompatible.Result.Should().BeTrue();
            result.IsSelected.Result.Should().BeTrue();
            result.Source.Count.Should().Be(1);
            result.Source.FirstOrDefault().Result.Should().Be(Common.Mod.Search.SourceType.Steam);
            result.Version.Count.Should().Be(2);
            result.Version.First().Version.Equals(new Shared.Version(2, 0)).Should().BeTrue();
            result.Version.Last().Version.Equals(new Shared.Version(3, 0)).Should().BeTrue();
            result.Name.Count.Should().Be(1);
            result.Name.FirstOrDefault().Negate.Should().BeFalse();
            result.Name.FirstOrDefault().Text.Should().Be("test test");
        }

        /// <summary>
        /// Defines the test method Should_be_able_to_parse_negate_operator.
        /// </summary>
        [Fact]
        public void Should_be_able_to_parse_negate_operator()
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
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.OrStatementSeparator, "||");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.StatementSeparator, "&&");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.ValueSeparator, ":");
            registry.RegisterTranslation("en", LocalizationResources.FilterOperators.Negate, "--");

            var parser = new Mod.Search.Parser(new Cache(), new Logger(), new List<ITypeConverter<object>> { new VersionConverter(registry), new BoolConverter(registry), new SourceTypeConverter(registry) }, registry);
            var line = "--test test &&EN-VER:--2.0||3.0 &&fr-src:steam &&en-ach:yest &&en-sel:true";
            var result = parser.Parse("en", line);
            result.Should().NotBeNull();
            result.AchievementCompatible.Result.Should().BeTrue();
            result.IsSelected.Result.Should().BeTrue();
            result.Source.Count.Should().Be(1);
            result.Source.FirstOrDefault().Result.Should().Be(Common.Mod.Search.SourceType.Steam);
            result.Version.Count.Should().Be(2);
            result.Version.First().Negate.Should().BeTrue();
            result.Version.First().Version.Equals(new Shared.Version(2, 0)).Should().BeTrue();
            result.Version.Last().Version.Equals(new Shared.Version(3, 0)).Should().BeTrue();
            result.Version.Last().Negate.Should().BeFalse();
            result.Name.Count.Should().Be(1);
            result.Name.FirstOrDefault().Negate.Should().BeTrue();
            result.Name.FirstOrDefault().Text.Should().Be("test test");
        }
    }
}
