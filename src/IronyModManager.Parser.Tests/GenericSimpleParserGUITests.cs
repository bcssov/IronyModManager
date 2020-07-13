// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 04-25-2020
// ***********************************************************************
// <copyright file="GenericSimpleParserGUITests.cs" company="Mario">
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
    /// Class GenericGraphicsParserTests.
    /// </summary>
    public class GenericSimpleParserGUITests
    {
        /// <summary>
        /// Defines the test method CanParse_should_be_false_then_true.
        /// </summary>
        [Fact]
        public void CanParse_should_be_false_then_true()
        {
            var lines = new HashSet<string>();
            for (int i = 0; i < 21000; i++)
            {
                lines.Add(i.ToString());
            }
            var args = new CanParseArgs()
            {
                File = "common\\gamerules\\test.txt",
                Lines = lines
            };
            var parser = new SimpleGUIParser(new CodeParser(), null);
            parser.CanParse(args).Should().BeFalse();
            args.File = "gui\\gui.gui";
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Parse_edge_case_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_edge_case_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"guiTypes = {	");
            sb.AppendLine(@"	containerWindowType = { ");
            sb.AppendLine(@"		name = ""test""");
            sb.AppendLine(@"	}		");
            sb.AppendLine(@"}");
            sb.AppendLine(@"guiTypes = {	");
            sb.AppendLine(@"	containerWindowType = { ");
            sb.AppendLine(@"		name = ""test2""");
            sb.AppendLine(@"	}		");
            sb.AppendLine(@"}");

            var sb2 = new StringBuilder();
            sb2.AppendLine(@"guiTypes = {");
            sb2.AppendLine(@"	containerWindowType = {");
            sb2.AppendLine(@"		name = ""test""");
            sb2.AppendLine(@"	}");
            sb2.AppendLine(@"}");

            var sb3 = new StringBuilder();
            sb3.AppendLine(@"guiTypes = {");
            sb3.AppendLine(@"	containerWindowType = {");
            sb3.AppendLine(@"		name = ""test2""");
            sb3.AppendLine(@"	}");
            sb3.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gui\\gui.gui",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new SimpleGUIParser(new CodeParser(), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(2);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gui\\gui.gui");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("test");
                        result[i].Code.Should().Be(sb2.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(Common.ValueType.Object);
                        break;
                    case 1:
                        result[i].Id.Should().Be("test2");
                        result[i].Code.Should().Be(sb3.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(Common.ValueType.Object);
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gui\\gui");
            }
        }

        /// <summary>
        /// Defines the test method Parse_inline_edge_case_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_ending_edge_case_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"guiTypes = {	");
            sb.AppendLine(@"	containerWindowType = { ");
            sb.AppendLine(@"		name = ""test"" }}");

            var sb2 = new StringBuilder();
            sb2.AppendLine(@"guiTypes = {");
            sb2.AppendLine(@"    containerWindowType = {");
            sb2.AppendLine(@"        name = ""test"" }");            
            sb2.AppendLine(@"}");



            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gui\\gui.gui",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new SimpleGUIParser(new CodeParser(), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gui\\gui.gui");
                switch (i)
                {

                    case 0:
                        result[i].Id.Should().Be("test");
                        result[i].Code.Should().Be(sb2.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(Common.ValueType.Object);
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gui\\gui");
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
            sb.AppendLine(@"guiTypes = { containerWindowType = { name = ""test"" } }");

            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gui\\gui.gui",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new SimpleGUIParser(new CodeParser(), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gui\\gui.gui");
                switch (i)
                {

                    case 0:
                        result[i].Id.Should().Be("test");
                        result[i].Code.Should().Be(sb.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(Common.ValueType.Object);
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gui\\gui");
            }
        }

        /// <summary>
        /// Defines the test method Parse_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"guiTypes = {	");
            sb.AppendLine(@"	containerWindowType = {");
            sb.AppendLine(@"		name = ""test""");
            sb.AppendLine(@"		x = 1");
            sb.AppendLine(@"	}		");
            sb.AppendLine(@"	");
            sb.AppendLine(@"	containerWindowType = {");
            sb.AppendLine(@"		name = ""test2"" ");
            sb.AppendLine(@"	}			");
            sb.AppendLine(@"}");


            var sb2 = new StringBuilder();
            sb2.AppendLine(@"guiTypes = {");
            sb2.AppendLine(@"	containerWindowType = {");
            sb2.AppendLine(@"		name = ""test""");
            sb2.AppendLine(@"		x = 1");
            sb2.AppendLine(@"	}");
            sb2.AppendLine(@"}");

            var sb3 = new StringBuilder();
            sb3.AppendLine(@"guiTypes = {");
            sb3.AppendLine(@"	containerWindowType = {");
            sb3.AppendLine(@"		name = ""test2""");
            sb3.AppendLine(@"	}");
            sb3.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gui\\gui.gui",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new SimpleGUIParser(new CodeParser(), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(2);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gui\\gui.gui");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("test");
                        result[i].Code.Should().Be(sb2.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(Common.ValueType.Object);
                        break;
                    case 1:
                        result[i].Id.Should().Be("test2");
                        result[i].Code.Should().Be(sb3.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(Common.ValueType.Object);
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gui\\gui");
            }
        }

        /// <summary>
        /// Defines the test method Parse_variable_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_variable_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"@sort_button_height = 80");
            sb.AppendLine(@"guiTypes = {");
            sb.AppendLine(@"	@entry_info_height = 17");
            sb.AppendLine(@"	# Button in the lower right of the main view, opening the Alliance View.");
            sb.AppendLine(@"	containerWindowType = {");
            sb.AppendLine(@"	    @why_am_i_here = 17");
            sb.AppendLine(@"		name = ""alliance_button_window""");
            sb.AppendLine(@"		position = { x = -458 y = 43 }");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gui\\gui.gui",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new SimpleGUIParser(new CodeParser(), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(4);
            for (int i = 0; i < 3; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gui\\gui.gui");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("@sort_button_height");
                        result[i].CodeTag.Should().BeNullOrWhiteSpace();
                        result[i].ValueType.Should().Be(Common.ValueType.Variable);
                        break;
                    case 1:
                        result[i].Id.Should().Be("@entry_info_height");
                        result[i].CodeTag.Should().Be("guiTypes");
                        result[i].ValueType.Should().Be(Common.ValueType.Variable);
                        break;
                    case 2:
                        result[i].Id.Should().Be("@why_am_i_here");
                        result[i].CodeTag.Should().Be("guiTypes");
                        result[i].ValueType.Should().Be(Common.ValueType.Variable);
                        break;
                    case 3:
                        result[i].Id.Should().Be("alliance_button_window");
                        result[i].ValueType.Should().Be(Common.ValueType.Object);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gui\\gui");
            }
        }
    }
}
