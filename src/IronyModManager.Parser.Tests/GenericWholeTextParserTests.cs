// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 03-28-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2022
// ***********************************************************************
// <copyright file="GenericWholeTextParserTests.cs" company="Mario">
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
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class GenericWholeTextParserTests.
    /// </summary>
    public class GenericWholeTextParserTests
    {
        /// <summary>
        /// Defines the test method CanParse_common_root_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_common_root_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\alerts.txt",
                GameType = "Stellaris"
            };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
            args.File = "common\\message_types.txt";
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_on_actions_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_on_actions_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\on_actions\\test.txt"
            };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_shader_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_shader_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\test.shader"
            };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_text_sound_file_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_text_sound_file_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "sound\\test.fxh"
            };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }


        /// <summary>
        /// Defines the test method CanParse_name_list_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_fxh_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\name_lists\\t.txt"
            };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeFalse();
            args.File = "common\\test.fxh";
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_should_be_false.
        /// </summary>
        [Fact]
        public void CanParse_should_be_false()
        {
            var args = new CanParseArgs()
            {
                File = "common\\ship_designs\\test.txt"
            };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method CanParse_csv_fils_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_csv_fils_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\csv.csv",
                GameType = "HeartsofIronIV"
            };
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

            var args = new ParserArgs()
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
            for (int i = 0; i < 1; i++)
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

                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("common\\txt");
            }
        }

        /// <summary>
        /// Defines the test method Parse_should_properly_format_shaders.
        /// </summary>
        [Fact]
        public void Parse_should_properly_format_shaders()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"");
            sb.AppendLine(@"VertexStruct VS_DEFAULT_TEXT_INPUT");
            sb.AppendLine(@"{");
            sb.AppendLine(@"	float4	vPosition	: POSITION;");
            sb.AppendLine(@"	float2	vTexCoord	: TEXCOORD0;");
            sb.AppendLine(@"	float4	vColor		: COLOR;");
            sb.AppendLine(@"};");
            sb.AppendLine(@"");
            sb.AppendLine(@"VertexStruct VS_DEFAULT_TEXT_OUTPUT");
            sb.AppendLine(@"{");
            sb.AppendLine(@"	float4	vPosition	: PDX_POSITION;");
            sb.AppendLine(@"	float2	vTexCoord	: TEXCOORD0;");
            sb.AppendLine(@"	float4	vColor		: TEXCOORD1;");
            sb.AppendLine(@"};");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gfx\\fx\\test.fxh",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gfx\\fx\\test.fxh");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().Be(string.Join(Environment.NewLine, sb.ToString().ReplaceTabs().Trim().SplitOnNewLine()));
                        result[i].Id.Should().Be("test.fxh");
                        result[i].ValueType.Should().Be(ValueType.WholeTextFile);
                        break;

                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gfx\\fx\\fxh");
            }
        }
    }
}
