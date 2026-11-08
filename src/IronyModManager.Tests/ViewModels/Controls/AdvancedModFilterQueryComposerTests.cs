// ***********************************************************************
// Assembly         : IronyModManager.Tests
// ***********************************************************************

using System;
using System.Collections.Generic;
using AwesomeAssertions;
using IronyModManager.Parser.Common.Mod.Search;
using IronyModManager.Parser.Common.Mod.Search.Converter;
using IronyModManager.Parser.Mod.Search;
using IronyModManager.Parser.Mod.Search.Converter;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;
using IronyModManager.Tests.Common;
using IronyModManager.ViewModels.Controls;
using Xunit;

namespace IronyModManager.Tests.ViewModels.Controls
{
    /// <summary>
    /// Tests the bounded query composition seam used by the shared search control.
    /// </summary>
    public class AdvancedModFilterQueryComposerTests
    {
        private readonly AdvancedModFilterQueryComposer composer = new AdvancedModFilterQueryComposer();

        [Fact]
        public void Empty_state_should_create_empty_query()
        {
            composer.Compose(new AdvancedModFilterState(), English()).Should().BeEmpty();
        }

        [Fact]
        public void Combined_state_should_use_existing_localized_query_syntax()
        {
            var state = new AdvancedModFilterState
            {
                PlainText = "frontier",
                Steam = true,
                Paradox = true,
                Selected = AdvancedFilterBooleanState.Yes,
                Achievements = AdvancedFilterBooleanState.No,
                Version = "3.14.*"
            };

            composer.Compose(state, English()).Should().Be(
                "frontier && source:steam || paradox && selected:yes && achievements:no && version:3.14.*");
        }

        [Fact]
        public void Version_include_should_compose_single_value()
        {
            composer.Compose(new AdvancedModFilterState { Version = "1.0" }, English())
                .Should().Be("version:1.0");
        }

        [Fact]
        public void Version_include_should_compose_multiple_or_values()
        {
            composer.Compose(new AdvancedModFilterState { Version = "1.0||2.0" }, English())
                .Should().Be("version:1.0 || 2.0");
        }

        [Fact]
        public void Version_exclude_should_compose_single_value()
        {
            composer.Compose(new AdvancedModFilterState { Version = "1.0", VersionExcluded = true }, English())
                .Should().Be("version:--1.0");
        }

        [Fact]
        public void Version_exclude_should_compose_multiple_values_with_localized_operators()
        {
            composer.Compose(new AdvancedModFilterState { Version = "1.0 OR 2.0", VersionExcluded = true }, AlternateSyntax())
                .Should().Be("release=NOT 1.0 OR NOT 2.0");
        }

        [Fact]
        public void Version_include_query_should_round_trip()
        {
            var syntax = AlternateSyntax();

            composer.TryParse("release=1.0 OR 2.0", syntax, out var state).Should().BeTrue();
            state.VersionExcluded.Should().BeFalse();
            state.Version.Should().Be("1.0 OR 2.0");
            composer.Compose(state, syntax).Should().Be("release=1.0 OR 2.0");
        }

        [Fact]
        public void Version_exclude_query_should_round_trip()
        {
            var syntax = AlternateSyntax();

            composer.TryParse("release=NOT 1.0 OR NOT 2.0", syntax, out var state).Should().BeTrue();
            state.VersionExcluded.Should().BeTrue();
            state.Version.Should().Be("1.0 OR 2.0");
            composer.Compose(state, syntax).Should().Be("release=NOT 1.0 OR NOT 2.0");
        }

        [Fact]
        public void Version_exclude_should_combine_with_other_fields_once_each()
        {
            var state = new AdvancedModFilterState
            {
                Steam = true,
                Local = true,
                Selected = AdvancedFilterBooleanState.Yes,
                Achievements = AdvancedFilterBooleanState.No,
                Version = "1.0||2.0",
                VersionExcluded = true
            };

            composer.Compose(state, English()).Should().Be(
                "source:steam || local && selected:yes && achievements:no && version:--1.0 || --2.0");
        }

        [Fact]
        public void Representable_query_should_initialize_all_controls()
        {
            var query = "frontier || horizon && source:steam||local && selected:no && achievements:yes && version:1.2.3";

            composer.TryParse(query, English(), out var state).Should().BeTrue();
            state.PlainText.Should().Be("frontier || horizon");
            state.Steam.Should().BeTrue();
            state.Paradox.Should().BeFalse();
            state.Local.Should().BeTrue();
            state.Selected.Should().Be(AdvancedFilterBooleanState.No);
            state.Achievements.Should().Be(AdvancedFilterBooleanState.Yes);
            state.Version.Should().Be("1.2.3");
            composer.Compose(state, English()).Should().Be(
                "frontier || horizon && source:steam || local && selected:no && achievements:yes && version:1.2.3");
        }

        [Theory]
        [InlineData("selected:true")]
        [InlineData("frontier && horizon")]
        [InlineData("source:steam && source:local")]
        [InlineData("frontier && unknown:value")]
        public void Lossy_or_unsupported_query_should_be_reported_as_custom(string query)
        {
            composer.TryParse(query, English(), out _).Should().BeFalse();
        }

        [Fact]
        public void Non_english_syntax_should_be_composed_and_recognized()
        {
            var syntax = German();
            var state = new AdvancedModFilterState
            {
                Local = true,
                Selected = AdvancedFilterBooleanState.Yes,
                Achievements = AdvancedFilterBooleanState.No
            };

            var query = composer.Compose(state, syntax);

            query.Should().Be("quelle:local && ausgewählt:ja && erfolge:nein");
            composer.TryParse(query, syntax, out var parsed).Should().BeTrue();
            parsed.Local.Should().BeTrue();
            parsed.Selected.Should().Be(AdvancedFilterBooleanState.Yes);
            parsed.Achievements.Should().Be(AdvancedFilterBooleanState.No);
        }

        [Fact]
        public void Configured_operators_should_compose_and_round_trip_without_default_literals()
        {
            var syntax = AlternateSyntax();
            var state = new AdvancedModFilterState
            {
                PlainText = "nebula",
                Steam = true,
                Paradox = true,
                Selected = AdvancedFilterBooleanState.Yes,
                Achievements = AdvancedFilterBooleanState.No,
                Version = "4.2.*"
            };

            var query = composer.Compose(state, syntax);

            query.Should().Be("nebula AND origin=workshop OR publisher AND chosen=on AND medals=off AND release=4.2.*");
            query.Should().NotContain(":").And.NotContain("&&").And.NotContain("||");
            composer.TryParse(query, syntax, out var parsed).Should().BeTrue();
            parsed.PlainText.Should().Be("nebula");
            parsed.Steam.Should().BeTrue();
            parsed.Paradox.Should().BeTrue();
            parsed.Local.Should().BeFalse();
            parsed.Selected.Should().Be(AdvancedFilterBooleanState.Yes);
            parsed.Achievements.Should().Be(AdvancedFilterBooleanState.No);
            parsed.Version.Should().Be("4.2.*");
        }

        [Fact]
        public void Configured_structured_negation_should_be_custom_while_plain_negation_remains_representable()
        {
            var syntax = AlternateSyntax();

            composer.TryParse("chosen=NOT on", syntax, out _).Should().BeFalse();
            composer.TryParse("NOT legacy", syntax, out var plain).Should().BeTrue();
            plain.PlainText.Should().Be("NOT legacy");
            composer.Compose(plain, syntax).Should().Be("NOT legacy");
        }

        [Fact]
        public void Generated_localized_query_should_be_accepted_by_existing_parser()
        {
            DISetup.SetupContainer();
            var registry = new LocalizationRegistry(new Cache());
            RegisterGermanParserSyntax(registry);
            var parser = new IronyModManager.Parser.Mod.Search.Parser(new Cache(), new Logger(),
                new List<ITypeConverter<object>>
                {
                    new VersionConverter(registry), new BoolConverter(registry), new SourceTypeConverter(registry)
                }, registry);
            var query = composer.Compose(new AdvancedModFilterState
            {
                PlainText = "frontier",
                Steam = true,
                Local = true,
                Selected = AdvancedFilterBooleanState.Yes,
                Achievements = AdvancedFilterBooleanState.No,
                Version = "3.14"
            }, German());

            var result = parser.Parse("de", query);

            result.Name.Should().ContainSingle(x => x.Text == "frontier" && !x.Negate);
            result.Source.Should().HaveCount(2);
            result.Source.Should().Contain(x => x.Result == SourceType.Steam);
            result.Source.Should().Contain(x => x.Result == SourceType.Local);
            result.IsSelected.Result.Should().BeTrue();
            result.AchievementCompatible.Result.Should().BeFalse();
            result.Version.Should().ContainSingle(x => x.Version.Equals(new Shared.Version(3, 14)));
        }

        [Fact]
        public void Opening_and_cancelling_custom_query_should_not_change_canonical_text()
        {
            DISetup.SetupContainer();
            var viewModel = new TestSearchModsControlViewModel();
            viewModel.SetAlternateSyntax();
            viewModel.Text = "chosen=NOT on";
            using var activation = viewModel.Activator.Activate();

            viewModel.OpenAdvancedFilterCommand.Execute().Subscribe();

            viewModel.IsCustomAdvancedQuery.Should().BeTrue();
            viewModel.Text.Should().Be("chosen=NOT on");
            viewModel.AdvancedCancelCommand.Execute().Subscribe();
            viewModel.Text.Should().Be("chosen=NOT on");
        }

        [Fact]
        public void Mixed_version_query_should_remain_custom_and_be_preserved_on_cancel()
        {
            DISetup.SetupContainer();
            var viewModel = new TestSearchModsControlViewModel();
            viewModel.SetEnglishSyntax();
            viewModel.Text = "version:1.0 || --2.0";
            using var activation = viewModel.Activator.Activate();

            viewModel.OpenAdvancedFilterCommand.Execute().Subscribe();

            viewModel.IsCustomAdvancedQuery.Should().BeTrue();
            viewModel.Text.Should().Be("version:1.0 || --2.0");
            viewModel.AdvancedCancelCommand.Execute().Subscribe();
            viewModel.Text.Should().Be("version:1.0 || --2.0");
        }

        [Fact]
        public void Apply_and_reset_should_update_only_canonical_text()
        {
            DISetup.SetupContainer();
            var viewModel = new TestSearchModsControlViewModel();
            viewModel.SetEnglishSyntax();
            using var activation = viewModel.Activator.Activate();
            viewModel.AdvancedPlainText = "frontier";
            viewModel.SteamSource = true;

            viewModel.AdvancedApplyCommand.Execute().Subscribe();

            viewModel.Text.Should().Be("frontier && source:steam");
            viewModel.AdvancedClearCommand.Execute().Subscribe();
            viewModel.Text.Should().BeEmpty();
        }

        private static AdvancedModFilterQuerySyntax English()
        {
            return new AdvancedModFilterQuerySyntax
            {
                Achievements = "achievements",
                Local = "local",
                Negate = "--",
                No = "no",
                OrSeparator = "||",
                Paradox = "paradox",
                Selected = "selected",
                Source = "source",
                StatementSeparator = "&&",
                Steam = "steam",
                ValueSeparator = ":",
                Version = "version",
                Yes = "yes"
            };
        }

        private static AdvancedModFilterQuerySyntax German()
        {
            var syntax = English();
            syntax.Achievements = "erfolge";
            syntax.No = "nein";
            syntax.Selected = "ausgewählt";
            syntax.Source = "quelle";
            syntax.Yes = "ja";
            return syntax;
        }

        private static AdvancedModFilterQuerySyntax AlternateSyntax()
        {
            return new AdvancedModFilterQuerySyntax
            {
                Achievements = "medals",
                Local = "disk",
                Negate = "NOT ",
                No = "off",
                OrSeparator = "OR",
                Paradox = "publisher",
                Selected = "chosen",
                Source = "origin",
                StatementSeparator = "AND",
                Steam = "workshop",
                ValueSeparator = "=",
                Version = "release",
                Yes = "on"
            };
        }

        private static void RegisterGermanParserSyntax(LocalizationRegistry registry)
        {
            registry.RegisterTranslation("de", LocalizationResources.FilterCommands.Achievements, "erfolge");
            registry.RegisterTranslation("de", LocalizationResources.FilterCommands.Selected, "ausgewählt");
            registry.RegisterTranslation("de", LocalizationResources.FilterCommands.Yes, "ja");
            registry.RegisterTranslation("de", LocalizationResources.FilterCommands.True, "wahr");
            registry.RegisterTranslation("de", LocalizationResources.FilterCommands.No, "nein");
            registry.RegisterTranslation("de", LocalizationResources.FilterCommands.False, "falsch");
            registry.RegisterTranslation("de", LocalizationResources.FilterCommands.Version, "version");
            registry.RegisterTranslation("de", LocalizationResources.FilterCommands.Source, "quelle");
            registry.RegisterTranslation("de", LocalizationResources.FilterCommands.Paradox, "paradox");
            registry.RegisterTranslation("de", LocalizationResources.FilterCommands.Steam, "steam");
            registry.RegisterTranslation("de", LocalizationResources.FilterCommands.Local, "local");
            registry.RegisterTranslation("de", LocalizationResources.FilterOperators.OrStatementSeparator, "||");
            registry.RegisterTranslation("de", LocalizationResources.FilterOperators.StatementSeparator, "&&");
            registry.RegisterTranslation("de", LocalizationResources.FilterOperators.ValueSeparator, ":");
            registry.RegisterTranslation("de", LocalizationResources.FilterOperators.Negate, "--");
        }

        private class TestSearchModsControlViewModel : SearchModsControlViewModel
        {
            public void SetEnglishSyntax()
            {
                AchievementsCommand = "achievements";
                LocalCommand = "local";
                NoCommand = "no";
                NegateOperator = "--";
                OrSeparator = "||";
                ParadoxCommand = "paradox";
                SelectedCommand = "selected";
                SourceCommand = "source";
                StatementSeparator = "&&";
                SteamCommand = "steam";
                ValueSeparator = ":";
                VersionCommand = "version";
                YesCommand = "yes";
            }

            public void SetAlternateSyntax()
            {
                var syntax = AlternateSyntax();
                AchievementsCommand = syntax.Achievements;
                LocalCommand = syntax.Local;
                NegateOperator = syntax.Negate;
                NoCommand = syntax.No;
                OrSeparator = syntax.OrSeparator;
                ParadoxCommand = syntax.Paradox;
                SelectedCommand = syntax.Selected;
                SourceCommand = syntax.Source;
                StatementSeparator = syntax.StatementSeparator;
                SteamCommand = syntax.Steam;
                ValueSeparator = syntax.ValueSeparator;
                VersionCommand = syntax.Version;
                YesCommand = syntax.Yes;
            }
        }
    }
}
