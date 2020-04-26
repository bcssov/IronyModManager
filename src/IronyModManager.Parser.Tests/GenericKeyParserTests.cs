// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-17-2020
//
// Last Modified By : Mario
// Last Modified On : 04-18-2020
// ***********************************************************************
// <copyright file="GenericKeyParserTests.cs" company="Mario">
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
using IronyModManager.Parser.Generic;
using IronyModManager.Shared;
using IronyModManager.Tests.Common;
using Xunit;


namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class GenericEventParserTests.
    /// </summary>
    public class GenericKeyParserTests
    {
        /// <summary>
        /// Defines the test method CanParse_should_be_false_then_true.
        /// </summary>
        [Fact]
        public void CanParse_should_be_false_then_true()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"@test = 1");
            sb.AppendLine(@"");
            sb.AppendLine(@"namespace = dmm_mod");
            sb.AppendLine(@"");
            sb.AppendLine(@"country_event = {");
            sb.AppendLine(@"    id = dmm_mod.1");
            sb.AppendLine(@"    hide_window = yes");
            sb.AppendLine(@"    is_triggered_only = yes");
            sb.AppendLine(@"");
            sb.AppendLine(@"    trigger = {");
            sb.AppendLine(@"        has_global_flag = dmm_mod_1");
            sb.AppendLine(@"    }");
            sb.AppendLine(@"");
            sb.AppendLine(@"    after = {");
            sb.AppendLine(@"        remove_global_flag = dmm_mod_1_opened");
            sb.AppendLine(@"    }");
            sb.AppendLine(@"");
            sb.AppendLine(@"    immediate = {");
            sb.AppendLine(@"        country_event = {");
            sb.AppendLine(@"            id = asl_options.1");
            sb.AppendLine(@"        }");
            sb.AppendLine(@"    }");
            sb.AppendLine(@"}");

            var args = new CanParseArgs()
            {
                File = "common\\gamerules\\test.txt",
                Lines = new List<string> { "test", "test2 = {}" }
            };
            var parser = new KeyParser(new CodeParser(), null);
            parser.CanParse(args).Should().BeFalse();
            args.File = "events\\test.txt";
            args.Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
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
            sb.AppendLine(@"@test = 1");
            sb.AppendLine(@"");
            sb.AppendLine(@"namespace = dmm_mod");
            sb.AppendLine(@"");
            sb.AppendLine(@"country_event = {");
            sb.AppendLine(@"    id = dmm_mod.1");
            sb.AppendLine(@"    hide_window = yes");
            sb.AppendLine(@"    is_triggered_only = yes");
            sb.AppendLine(@"");
            sb.AppendLine(@"    trigger = {");
            sb.AppendLine(@"        has_global_flag = dmm_mod_1");
            sb.AppendLine(@"    }");
            sb.AppendLine(@"");
            sb.AppendLine(@"    after = {");
            sb.AppendLine(@"        remove_global_flag = dmm_mod_1_opened");
            sb.AppendLine(@"    }");
            sb.AppendLine(@"");
            sb.AppendLine(@"    immediate = {");
            sb.AppendLine(@"        country_event = {");
            sb.AppendLine(@"            id = asl_options.1");
            sb.AppendLine(@"        }");
            sb.AppendLine(@"    }");
            sb.AppendLine(@"}");


            var sb2 = new StringBuilder();
            sb2.AppendLine(@"country_event = {");
            sb2.AppendLine(@"    id = dmm_mod.1");
            sb2.AppendLine(@"    hide_window = yes");
            sb2.AppendLine(@"    is_triggered_only = yes");
            sb2.AppendLine(@"    trigger = {");
            sb2.AppendLine(@"        has_global_flag = dmm_mod_1");
            sb2.AppendLine(@"    }");
            sb2.AppendLine(@"    after = {");
            sb2.AppendLine(@"        remove_global_flag = dmm_mod_1_opened");
            sb2.AppendLine(@"    }");
            sb2.AppendLine(@"    immediate = {");
            sb2.AppendLine(@"        country_event = {");
            sb2.AppendLine(@"            id = asl_options.1");
            sb2.AppendLine(@"        }");
            sb2.AppendLine(@"    }");
            sb2.AppendLine(@"}");

            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "events\\fake.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new KeyParser(new CodeParser(), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(3);
            for (int i = 0; i < 3; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("events\\fake.txt");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().Be("@test = 1");
                        result[i].Id.Should().Be("@test");
                        result[i].ValueType.Should().Be(Common.ValueType.Variable);
                        break;
                    case 1:
                        result[i].Id.Should().Be("fake-namespace");
                        result[i].ValueType.Should().Be(Common.ValueType.Namespace);
                        break;
                    case 2:
                        result[i].Id.Should().Be("dmm_mod.1");
                        result[i].Code.Should().Be(sb2.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(Common.ValueType.Object);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("events\\txt");
            }
        }

        /// <summary>
        /// Defines the test method Parse_inline_edge_case_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_inline_edge_case_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"entity = { name = ""ai_01_blue_sponsored_colonizer_entity"" clone = ""ai_01_blue_colonizer_entity"" }");

            var sb2 = new StringBuilder();
            sb2.AppendLine(@"entity = {");
            sb2.AppendLine(@"    name = ""ai_01_blue_sponsored_colonizer_entity""");
            sb2.AppendLine(@"    clone = ""ai_01_blue_colonizer_entity""");
            sb2.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "events\\fake.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new KeyParser(new CodeParser(), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("events\\fake.txt");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().Be(sb2.ToString().Trim());
                        result[i].Id.Should().Be("ai_01_blue_sponsored_colonizer_entity");
                        result[i].ValueType.Should().Be(Common.ValueType.Object);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("events\\txt");
            }
        }

        /// <summary>
        /// Defines the test method Parse_inline_edge_case_should_not_yield_results.
        /// </summary>
        [Fact]
        public void Parse_inline_edge_case_should_not_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"create_envoys = {		create_ship = { name = random graphical_culture = ""ehof_01"" design = ""NAME_Compound_Envoy"" } }");

            var sb2 = new System.Text.StringBuilder();
            sb2.AppendLine(@"create_envoys = {");
            sb2.AppendLine(@"    create_ship = {");
            sb2.AppendLine(@"        name = random");
            sb2.AppendLine(@"        graphical_culture = ""ehof_01""");
            sb2.AppendLine(@"        design = ""NAME_Compound_Envoy""");
            sb2.AppendLine(@"    }");
            sb2.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "events\\fake.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new KeyParser(new CodeParser(), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("events\\fake.txt");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().Be(sb2.ToString().Trim());
                        result[i].Id.Should().Be("create_envoys");
                        result[i].ValueType.Should().Be(Common.ValueType.Object);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("events\\txt");
            }
        }

        /// <summary>
        /// Defines the test method Parse_two_line_edge_case_should_not_parse.
        /// </summary>
        [Fact]
        public void Parse_two_line_edge_case_should_not_parse()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"create_envoys = {		");
            sb.AppendLine(@"create_ship = { name = random graphical_culture = ""ehof_01"" design = ""NAME_Compound_Envoy"" } }");

            var sb2 = new System.Text.StringBuilder();
            sb2.AppendLine(@"create_envoys = {");
            sb2.AppendLine(@"    create_ship = {");
            sb2.AppendLine(@"        name = random");
            sb2.AppendLine(@"        graphical_culture = ""ehof_01""");
            sb2.AppendLine(@"        design = ""NAME_Compound_Envoy""");
            sb2.AppendLine(@"    }");
            sb2.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "events\\fake.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new KeyParser(new CodeParser(), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("events\\fake.txt");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().Be(sb2.ToString().Trim());
                        result[i].Id.Should().Be("create_envoys");
                        result[i].ValueType.Should().Be(Common.ValueType.Object);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("events\\txt");
            }
        }


        /// <summary>
        /// Defines the test method CanParse_inline_edge_case_should_parse.
        /// </summary>
        [Fact]
        public void CanParse_inline_edge_case_should_parse()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"entity = { name = ""ai_01_blue_sponsored_colonizer_entity"" clone = ""ai_01_blue_colonizer_entity"" }");

            var args = new CanParseArgs()
            {
                File = "common\\gamerules\\test.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
            };
            var parser = new KeyParser(new CodeParser(), null);
            var result = parser.CanParse(args);
            result.Should().BeTrue();
        }
        /// <summary>
        /// Defines the test method CanParse_inline_edge_case_should_not_parse.
        /// </summary>
        [Fact]
        public void CanParse_inline_edge_case_should_not_parse()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"create_envoys = {		create_ship = { name = random graphical_culture = ""ehof_01"" design = ""NAME_Compound_Envoy"" } }");

            var args = new CanParseArgs()
            {
                File = "common\\gamerules\\test.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
            };
            var parser = new KeyParser(new CodeParser(), null);
            var result = parser.CanParse(args);
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method CanParse_two_line_edge_case_should_not_parse.
        /// </summary>
        [Fact]
        public void CanParse_two_line_edge_case_should_not_parse()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"create_envoys = {		");
            sb.AppendLine(@"create_ship = { name = random graphical_culture = ""ehof_01"" design = ""NAME_Compound_Envoy"" } }");

            var args = new CanParseArgs()
            {
                File = "common\\gamerules\\test.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
            };
            var parser = new KeyParser(new CodeParser(), null);
            var result = parser.CanParse(args);
            result.Should().BeFalse();
        }
    }
}
