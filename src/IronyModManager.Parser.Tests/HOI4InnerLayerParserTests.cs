// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 01-29-2022
//
// Last Modified By : Mario
// Last Modified On : 02-04-2023
// ***********************************************************************
// <copyright file="HOI4InnerLayerParserTests.cs" company="Mario">
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
using IronyModManager.Parser.Common.Args;
using IronyModManager.Tests.Common;
using Xunit;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class HOI4KeyValuePairParserTests.
    /// </summary>
    public class HOI4InnerLayerParserTests
    {
        /// <summary>
        /// Defines the test method CanParse_should_be_false.
        /// </summary>
        [Fact]
        public void CanParse_should_be_false()
        {
            var args = new CanParseArgs()
            {
                File = "common\\country\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method CanParse_abilities_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_abilities_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\abilities\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_charactrs_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_charactrs_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\characters\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_decisions_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_decisions_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\decisions\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_decision_categories_should_be_false.
        /// </summary>
        [Fact]
        public void CanParse_decision_categories_should_be_false()
        {
            var args = new CanParseArgs()
            {
                File = "common\\decisions\\categories\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method CanParse_opinion_modifiers_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_opinion_modifiers_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\opinion_modifiers\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_state_categories_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_state_categories_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\state_category\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_technologies_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_technologies_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\technologies\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_unit_leader_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_unit_leader_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\unit_leader\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_country_leader_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_country_leader_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\country_leader\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_aces_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_aces_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\aces\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_ai_areas_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_ai_areas_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\ai_areas\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_buildings_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_buildings_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\buildings\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_ideologies_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_ideologies_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\ideologies\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_resources_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_resources_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\resources\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_wargoals_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_wargoals_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\wargoals\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_scripted_diplomatic_actions_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_scripted_diplomatic_actions_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\scripted_diplomatic_actions\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_scripted_medals_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_scripted_medals_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\medals\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_ribbons_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_ribbons_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\ribbons\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_unit_medals_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_unit_medals_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\unit_medals\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_map_modes_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_map_modes_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\map_modes\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
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
            sb.AppendLine(@"ability = {	");
            sb.AppendLine(@"	CHI_force_attack = {");
            sb.AppendLine(@"		name = ABILITY_FORCE_ATTACK");
            sb.AppendLine(@"		desc = ABILITY_FORCE_ATTACK_DESC");
            sb.AppendLine(@"		icon = GFX_ability_chi_force_attack");
            sb.AppendLine(@"		");
            sb.AppendLine(@"		sound_effect = command_power_ability_offensive");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"}");

            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\abilities\\fake.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\abilities\\fake.txt");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("CHI_force_attack");
                        result[i].CodeTag.Should().Be("ability");
                        result[i].CodeSeparator.Should().Be("{");
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;

                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("common\\abilities\\txt");
            }
        }

        /// <summary>
        /// Defines the test method Parse_decisions_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_decisions_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"political_actions = {");
            sb.AppendLine(@"	");
            sb.AppendLine(@"	sample = {");
            sb.AppendLine(@"		allowed = {");
            sb.AppendLine(@"			original_tag = TAG");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		available = {");
            sb.AppendLine(@"			");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\decisions\\fake.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\decisions\\fake.txt");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("sample");
                        result[i].CodeTag.Should().Be("political_actions");
                        result[i].CodeSeparator.Should().Be("{");
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;

                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("common\\decisions\\political_actions-txt");
            }
        }

        /// <summary>
        /// Defines the test method Parse_levels_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_levels_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"leader_attack_skills = {");
            sb.AppendLine(@"");
            sb.AppendLine(@"	### NAVY ###");
            sb.AppendLine(@"	1 = {");
            sb.AppendLine(@"		cost = 100");
            sb.AppendLine(@"		type = navy");
            sb.AppendLine(@"		modifier = {");
            sb.AppendLine(@"			naval_damage_factor = 0.05");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	2 = {");
            sb.AppendLine(@"		cost = 200");
            sb.AppendLine(@"		type = navy");
            sb.AppendLine(@"		modifier = {");
            sb.AppendLine(@"			naval_damage_factor = 0.10");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\unit_leader\\fake.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Games.HOI4.InnerLayerParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\unit_leader\\fake.txt");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("leader_attack_skills");
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;

                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("common\\unit_leader\\txt");
            }
        }
    }
}
