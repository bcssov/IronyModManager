// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 09-10-2024
//
// Last Modified By : Mario
// Last Modified On : 09-10-2024
// ***********************************************************************
// <copyright file="StellarisKeyParserTests.cs" company="Mario">
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
using IronyModManager.Tests.Common;
using Xunit;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class StellarisKeyParserTests.
    /// </summary>
    public class StellarisKeyParserTests
    {
        /// <summary>
        /// Defines the test method CanParse_should_be_false_then_true.
        /// </summary>
        [Fact]
        public void CanParse_should_be_false_then_true()
        {
            var args = new CanParseArgs { File = "common\\fake\\test.txt", GameType = "Stellaris" };
            var parser = new Games.Stellaris.KeyParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeFalse();
            args.File = "common\\special_projects\\test.txt";
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Parse_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine(@"special_project = {");
            sb.AppendLine(@"    key = ""INETIAN_PROJECTS_01""");
            sb.AppendLine(@"    event_chain = cs_inetian_chain");
            sb.AppendLine(@"    icon = ""gfx/interface/icons/situation_log/situation_log_radar.dds""");
            sb.AppendLine(@"    picture = GFX_evt_poor_stormshelter");
            sb.AppendLine(@"    sound = yes");
            sb.AppendLine(@"    cost = 500");
            sb.AppendLine(@"    days_to_research = 0");
            sb.AppendLine(@"    timelimit = -1");
            sb.AppendLine(@"    location = no");
            sb.AppendLine(@"");
            sb.AppendLine(@"	event_scope = ship_event");
            sb.AppendLine(@"");
            sb.AppendLine(@"	on_success = {");
            sb.AppendLine(@"		from = {");
            sb.AppendLine(@"			spawn_system = {");
            sb.AppendLine(@"				min_distance = 10");
            sb.AppendLine(@"				max_distance = 15");
            sb.AppendLine(@"				initializer = ""weathermanipulators_home_system""");
            sb.AppendLine(@"				hyperlane = yes");
            sb.AppendLine(@"				is_discovered = yes");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				set_visited = last_created_system");
            sb.AppendLine(@"				set_country_flag = wm_digsite_coordinates_found");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"");
            sb.AppendLine(@"			fleet_event = { id = cstorms.135 }");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	on_fail = {");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\special_projects\\fake.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Games.Stellaris.KeyParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\special_projects\\fake.txt");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("INETIAN_PROJECTS_01");
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("common\\special_projects\\txt");
            }
        }

        /// <summary>
        /// Defines the test method Parse_should_yield_results_with_inline.
        /// </summary>
        [Fact]
        public void Parse_should_yield_results_with_inline()
        {
            DISetup.SetupContainer();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine(@"special_project = {");
            sb.AppendLine(@"	inline_script = {");
            sb.AppendLine(@"		script = cosmic_storms/EyeOfTheStormSpecialProjects");
            sb.AppendLine(@"		TYPE = nexus_storm");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");



            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\special_projects\\fake.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Games.Stellaris.KeyParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\special_projects\\fake.txt");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("nexus_storm");
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("common\\special_projects\\txt");
            }
        }
    }
}
