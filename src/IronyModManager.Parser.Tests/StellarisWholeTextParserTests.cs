// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 08-18-2024
// ***********************************************************************
// <copyright file="StellarisWholeTextParserTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Games.Stellaris;
using IronyModManager.Tests.Common;
using Xunit;
using ValueType = IronyModManager.Shared.Models.ValueType;


namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class StellarisWholeTextParserTests.
    /// </summary>
    public class StellarisWholeTextParserTests
    {
        /// <summary>
        /// Defines the test method CanParse_map_galaxy_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_map_galaxy_should_be_true()
        {
            var args = new CanParseArgs { File = "map\\galaxy\\test.txt", GameType = "Stellaris" };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_weapon_components_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_component_tags_should_be_true()
        {
            var args = new CanParseArgs { File = "common\\component_tags\\tags.txt", GameType = "Stellaris" };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_diplo_phrase_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_diplo_phrase_should_be_true()
        {
            var args = new CanParseArgs { File = "common\\diplo_phrases\\test.txt", GameType = "Stellaris" };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_random_names_base_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_random_names_base_should_be_true()
        {
            var args = new CanParseArgs { File = "common\\random_names\\base\\test.txt", GameType = "Stellaris" };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_random_names_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_random_names_should_be_true()
        {
            var args = new CanParseArgs { File = "common\\random_names\\test.txt", GameType = "Stellaris" };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_species_names_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_species_names_should_be_true()
        {
            var args = new CanParseArgs { File = "common\\species_names\\test.txt", GameType = "Stellaris" };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }


        /// <summary>
        /// Defines the test method CanParse_name_list_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_name_list_should_be_true()
        {
            var args = new CanParseArgs { File = "common\\name_lists\\t.txt", GameType = "Stellaris" };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_start_screen_messages_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_start_screen_messages_should_be_true()
        {
            var args = new CanParseArgs { File = "common\\start_screen_messages\\t.txt", GameType = "Stellaris" };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }


        /// <summary>
        /// Defines the test method CanParse_should_be_false.
        /// </summary>
        [Fact]
        public void CanParse_should_be_false()
        {
            var args = new CanParseArgs { File = "common\\ship_designs\\test.txt", GameType = "Stellaris" };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method CanParse_map_setup_scenarios_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_map_setup_scenarios_should_be_true()
        {
            var args = new CanParseArgs { File = "map\\setup_scenarios\\test.txt", GameType = "Stellaris" };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_country_container_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_country_container_should_be_true()
        {
            var args = new CanParseArgs { File = "common\\country_container\\test.txt", GameType = "Stellaris" };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_diplomacy_economy_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_diplomacy_economy_should_be_true()
        {
            var args = new CanParseArgs { File = "common\\diplomacy_economy\\test.txt", GameType = "Stellaris" };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_inline_scripts_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_inline_scripts_should_be_true()
        {
            var args = new CanParseArgs { File = "common\\inline_scripts\\test.txt", GameType = "Stellaris" };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_economic_plans_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_economic_plans_should_be_true()
        {
            var args = new CanParseArgs { File = "common\\economic_plans\\test.txt", GameType = "Stellaris" };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_species_classes_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_species_classes_should_be_true()
        {
            var args = new CanParseArgs { File = "common\\species_classes\\test.txt", GameType = "Stellaris" };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_terraform_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_terraform_should_be_true()
        {
            var args = new CanParseArgs { File = "common\\terraform\\test.txt", GameType = "Stellaris" };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Parse_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"on_game_start = {");
            sb.AppendLine(@"    events = {");
            sb.AppendLine(@"        dmm_mod_1_flag.1");
            sb.AppendLine(@"    }");
            sb.AppendLine(@"}");

            var args = new ParserArgs
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\alerts.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (var i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\alerts.txt");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().Be(sb.ToString().Trim());
                        result[i].Id.Should().Be("alerts.txt");
                        result[i].ValueType.Should().Be(ValueType.WholeTextFile);
                        break;
                }

                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("common\\txt");
            }
        }

        /// <summary>
        /// Defines the test method Parse_component_tags_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_component_tags_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"weapon_type_energy");
            sb.AppendLine(@"weapon_type_kinetic");
            sb.AppendLine(@"weapon_type_explosive");
            sb.AppendLine(@"weapon_type_strike_craft");
            sb.AppendLine(@"weapon_type_point_defense");
            sb.AppendLine(@"weapon_role_anti_armor");
            sb.AppendLine(@"weapon_role_anti_shield");
            sb.AppendLine(@"weapon_role_artillery");
            sb.AppendLine(@"weapon_role_anti_hull");
            sb.AppendLine(@"weapon_role_point_defense");

            var args = new ParserArgs
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\component_tags\\t.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (var i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\component_tags\\t.txt");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().Be(sb.ToString().Trim());
                        result[i].Id.Should().Be("t.txt");
                        result[i].ValueType.Should().Be(ValueType.WholeTextFile);
                        break;
                }

                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("common\\component_tags\\txt");
            }
        }
    }
}
