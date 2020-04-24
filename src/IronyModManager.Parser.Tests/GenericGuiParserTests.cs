// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-20-2020
//
// Last Modified By : Mario
// Last Modified On : 04-07-2020
// ***********************************************************************
// <copyright file="GenericGuiParserTests.cs" company="Mario">
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
    /// Class GenericGuiParserTests.
    /// </summary>
    public class GenericGuiParserTests
    {
        /// <summary>
        /// Defines the test method CanParse_should_be_false_then_true.
        /// </summary>
        [Fact]
        public void CanParse_should_be_false_then_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\gamerules\\test.txt",
            };
            var parser = new GuiParser(new CodeParser());
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
            var parser = new GuiParser(new CodeParser());
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
            sb2.AppendLine(@"	containerWindowType = {");
            sb2.AppendLine(@"		name = ""test"" }");
            sb2.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gui\\gui.gui",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new GuiParser(new CodeParser());
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
            var parser = new GuiParser(new CodeParser());
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
            var parser = new GuiParser(new CodeParser());
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
    }
}
