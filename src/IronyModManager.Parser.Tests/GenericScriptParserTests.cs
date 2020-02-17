// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-17-2020
//
// Last Modified By : Mario
// Last Modified On : 02-17-2020
// ***********************************************************************
// <copyright file="GenericScriptParserTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using IronyModManager.Tests.Common;
using Xunit;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class GenericScriptParserTests.
    /// </summary>
    public class GenericScriptParserTests
    {
        /// <summary>
        /// Defines the test method Has_valid_parser_type.
        /// </summary>
        [Fact]
        public void Has_valid_parser_type()
        {
            new GenericScriptParser().ParserType.Should().Be(Constants.GenericParserFlag);
        }

        /// <summary>
        /// Defines the test method CanParse_should_be_false.
        /// </summary>
        [Fact]
        public void CanParse_should_be_false()
        {
            var args = new CanParseArgs()
            {
                File = "common\\gamerules\\test.txt",             
            };
            var parser = new GenericScriptParser();
            parser.CanParse(args).Should().BeFalse();
            args.File = "events\\test.txt";
            parser.CanParse(args).Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Parse_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_should_yield_results()
        {
            DISetup.SetupContainer();
            var sb = new StringBuilder();
            sb.AppendLine(@"@test = 1");
            sb.AppendLine(@"");
            sb.AppendLine(@"asl_mode_options_1 = {");
            sb.AppendLine(@"	potential = {");
            sb.AppendLine(@"		NOT = {");
            sb.AppendLine(@"			has_country_flag = asl_modify_3");
            sb.AppendLine(@"			has_country_flag = asl_modify_5");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	allow = {");
            sb.AppendLine(@"		always = yes");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	effect = {");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");
            sb.AppendLine(@"");
            sb.AppendLine(@"asl_mode_options_3 = {");
            sb.AppendLine(@"	potential = {");
            sb.AppendLine(@"		has_country_flag = asl_modify_3");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	allow = {");
            sb.AppendLine(@"		always = yes");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	effect = {");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");
            sb.AppendLine(@"");
            sb.AppendLine(@"asl_mode_options_5 = {");
            sb.AppendLine(@"	potential = {");
            sb.AppendLine(@"		has_country_flag = asl_modify_5");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	allow = {");
            sb.AppendLine(@"		always = yes");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	effect = {");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");
            sb.AppendLine(@"");
            sb.AppendLine(@"### END TEMPLATE:effects ###");

            var sb2 = new StringBuilder();
            sb2.AppendLine(@"asl_mode_options_1 = {");
            sb2.AppendLine(@"	potential = {");
            sb2.AppendLine(@"		NOT = {");
            sb2.AppendLine(@"			has_country_flag = asl_modify_3");
            sb2.AppendLine(@"			has_country_flag = asl_modify_5");
            sb2.AppendLine(@"		}");
            sb2.AppendLine(@"	}");
            sb2.AppendLine(@"	allow = {");
            sb2.AppendLine(@"		always = yes");
            sb2.AppendLine(@"	}");
            sb2.AppendLine(@"	effect = {");
            sb2.AppendLine(@"	}");
            sb2.AppendLine(@"}");

            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                Dependencies = new List<string> { "1"},
                File = "common\\fake\\fake.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new GenericScriptParser();
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(4);
            for (int i = 0; i < 4; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\fake\\fake.txt");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().Be("@test = 1");
                        result[i].Id.Should().Be("@test");
                        break;
                    case 1:
                        result[i].Id.Should().Be("asl_mode_options_1");
                        result[i].Code.Should().Be(sb2.ToString());
                        break;
                    case 2:
                        result[i].Id.Should().Be("asl_mode_options_3");
                        break;
                    case 3:
                        result[i].Id.Should().Be("asl_mode_options_5");
                        break;
                    default:
                        break;
                }                
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("common\\fake");
            }
        }
    }
}
