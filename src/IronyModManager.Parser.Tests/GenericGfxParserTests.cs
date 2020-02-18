// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-18-2020
// ***********************************************************************
// <copyright file="GenericGfxParserTests.cs" company="Mario">
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
    /// Class GenericGfxParserTests.
    /// </summary>
    public class GenericGfxParserTests
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
            var parser = new GenericGfxParser();
            parser.CanParse(args).Should().BeFalse();
            args.File = "gfx\\gfx.gfx";            
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
            sb.AppendLine(@"spriteTypes = {");
            sb.AppendLine(@"");
            sb.AppendLine(@"	### ship size icons ###");
            sb.AppendLine(@"");
            sb.AppendLine(@"	spriteType = {");
            sb.AppendLine(@"		name = ""GFX_text_military_size_1""");
            sb.AppendLine(@"		texturefile = ""gfx/interface/icons/text_icons/icon_text_military_size_1.dds""");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	");
            sb.AppendLine(@"	spriteType = {");
            sb.AppendLine(@"		name = ""GFX_text_military_size_2""");
            sb.AppendLine(@"		texturefile = ""gfx/interface/icons/text_icons/icon_text_military_size_2.dds""");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");


            var sb2 = new StringBuilder();
            sb2.AppendLine(@"spriteTypes = {");
            sb2.AppendLine(@"	spriteType = {");
            sb2.AppendLine(@"		name = ""GFX_text_military_size_1""");
            sb2.AppendLine(@"		texturefile = ""gfx/interface/icons/text_icons/icon_text_military_size_1.dds""");
            sb2.AppendLine(@"	}");
            sb2.AppendLine(@"}");


            var sb3 = new System.Text.StringBuilder();
            sb3.AppendLine(@"spriteTypes = {");
            sb3.AppendLine(@"	spriteType = {");
            sb3.AppendLine(@"		name = ""GFX_text_military_size_2""");
            sb3.AppendLine(@"		texturefile = ""gfx/interface/icons/text_icons/icon_text_military_size_2.dds""");
            sb3.AppendLine(@"	}");
            sb3.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                Dependencies = new List<string> { "1" },
                File = "gfx\\gfx.gfx",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new GenericGfxParser();
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(2);
            for (int i = 0; i < 2; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gfx\\gfx.gfx");
                switch (i)
                {
                    
                    case 0:
                        result[i].Id.Should().Be("GFX_text_military_size_1");
                        result[i].Code.Should().Be(sb2.ToString());
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    case 1:
                        result[i].Id.Should().Be("GFX_text_military_size_2");
                        result[i].Code.Should().Be(sb3.ToString());
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gfx");
            }
        }
    }
}
