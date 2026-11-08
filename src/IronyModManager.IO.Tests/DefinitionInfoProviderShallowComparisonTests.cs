using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Mods.InfoProviders;
using IronyModManager.Parser.Definitions;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using IronyModManager.Tests.Common;
using Xunit;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.IO.Tests
{
    public class DefinitionInfoProviderShallowComparisonTests
    {
        private static readonly IReadOnlyCollection<IDefinitionInfoProvider> Providers =
        [
            new StellarisDefinitionInfoProvider(),
            new HOI4DefinitionInfoProvider()
        ];

        [Fact]
        public void Should_resolve_game_specific_providers_from_io_composition()
        {
            DISetup.SetupContainer();

            var providers = DISetup.Container.GetAllInstances<IDefinitionInfoProvider>().ToList();

            providers.Should().ContainSingle(provider => provider.CanProcess(Constants.GamesTypes.Stellaris.Id));
            providers.Should().ContainSingle(provider => provider.CanProcess(Constants.GamesTypes.HeartsOfIron4.Id));
        }

        [Fact]
        public void Should_resolve_stellaris_provider_for_stellaris()
        {
            var provider = Providers.FirstOrDefault(candidate => candidate.CanProcess(Constants.GamesTypes.Stellaris.Id));

            provider.Should().BeOfType<StellarisDefinitionInfoProvider>();
        }

        [Fact]
        public void Should_resolve_hoi4_provider_for_hoi4()
        {
            var provider = Providers.FirstOrDefault(candidate => candidate.CanProcess(Constants.GamesTypes.HeartsOfIron4.Id));

            provider.Should().BeOfType<HOI4DefinitionInfoProvider>();
        }

        [Fact]
        public void Should_not_resolve_provider_for_unsupported_game()
        {
            var provider = Providers.FirstOrDefault(candidate => candidate.CanProcess("unsupported"));

            provider.Should().BeNull();
        }

        [Theory]
        [InlineData("common\\armies\\00_armies.txt", true)]
        [InlineData("common\\scripted_effects\\00_scripted_effects.txt", false)]
        [InlineData("common\\inline_scripts\\example.txt", false)]
        [InlineData("common\\solar_system_initializers\\example.txt", false)]
        public void Stellaris_should_apply_its_own_shallow_comparison_policy(string file, bool expected)
        {
            var provider = new StellarisDefinitionInfoProvider();

            provider.CanUseShallowComparison(Definition(file)).Should().Be(expected);
        }

        [Theory]
        [InlineData("common\\ideas\\00_ideas.txt", true)]
        [InlineData("common\\scripted_effects\\00_scripted_effects.txt", false)]
        [InlineData("common\\scripted_triggers\\00_scripted_triggers.txt", false)]
        [InlineData("common\\scripted_localisation\\00_scripted_localisation.txt", false)]
        [InlineData("common\\on_actions\\00_on_actions.txt", false)]
        [InlineData("common\\inline_scripts\\stellaris_like_path.txt", true)]
        public void Hoi4_should_apply_its_own_live_data_policy(string file, bool expected)
        {
            var provider = new HOI4DefinitionInfoProvider();

            provider.CanUseShallowComparison(Definition(file)).Should().Be(expected);
        }

        private static IDefinition Definition(string file)
        {
            return new Definition
            {
                File = file,
                Id = "test",
                Type = "test",
                ValueType = ValueType.Object
            };
        }
    }
}
