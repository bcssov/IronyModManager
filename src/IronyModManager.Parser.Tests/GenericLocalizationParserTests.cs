// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-21-2020
//
// Last Modified By : Mario
// Last Modified On : 02-22-2020
// ***********************************************************************
// <copyright file="GenericLocalizationParserTests.cs" company="Mario">
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
using IronyModManager.Tests.Common;
using Xunit;


namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class GenericLocalizationParserTests.
    /// </summary>
    public class GenericLocalizationParserTests
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
                GameType = "Stellaris"
            };
            var parser = new  LocalizationParser(new CodeParser(), null);
            parser.CanParse(args).Should().BeFalse();
            args.File = "loc\\loc.yml";
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
            sb.AppendLine(@"l_english:");
            sb.AppendLine(@" NEW_ACHIEVEMENT_2_0_NAME:0 ""Brave New World""");
            sb.AppendLine(@" NEW_ACHIEVEMENT_2_0_DESC:0 ""Colonize a planet""");
            sb.AppendLine(@" NEW_ACHIEVEMENT_2_1_NAME:0 ""Digging Deep""");

            var sb2 = new StringBuilder();
            sb2.AppendLine(@"l_english:");
            sb2.AppendLine(@" NEW_ACHIEVEMENT_2_0_NAME:0 ""Brave New World""");                        

            var sb3 = new StringBuilder();
            sb3.AppendLine(@"l_english:");            
            sb3.AppendLine(@" NEW_ACHIEVEMENT_2_0_DESC:0 ""Colonize a planet""");            

            var sb4 = new StringBuilder();
            sb4.AppendLine(@"l_english:");            
            sb4.AppendLine(@" NEW_ACHIEVEMENT_2_1_NAME:0 ""Digging Deep""");

            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "loc\\loc.yml",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new LocalizationParser(new CodeParser(), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(3);
            for (int i = 0; i < 3; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("loc\\loc.yml");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().Be(sb2.ToString().Trim());
                        result[i].Id.Should().Be("NEW_ACHIEVEMENT_2_0_NAME");
                        result[i].ValueType.Should().Be(Common.ValueType.SpecialVariable);
                        break;
                    case 1:
                        result[i].Code.Trim().Should().Be(sb3.ToString().Trim());
                        result[i].Id.Should().Be("NEW_ACHIEVEMENT_2_0_DESC");
                        result[i].ValueType.Should().Be(Common.ValueType.SpecialVariable);
                        break;
                    case 2:
                        result[i].Code.Trim().Should().Be(sb4.ToString().Trim());
                        result[i].Id.Should().Be("NEW_ACHIEVEMENT_2_1_NAME");                        
                        result[i].ValueType.Should().Be(Common.ValueType.SpecialVariable);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("loc\\l_english-yml");
            }
        }

        /// <summary>
        /// Defines the test method Parse_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_no_whitespace_at_end_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"l_english:");
            sb.AppendLine(@"NEW_ACHIEVEMENT_2_0_NAME:0 ""Brave New World""");
            sb.AppendLine(@"NEW_ACHIEVEMENT_2_0_DESC:0 ""Colonize a planet""");
            sb.AppendLine(@"NEW_ACHIEVEMENT_2_1_NAME:0 ""Digging Deep""");

            var sb2 = new StringBuilder();
            sb2.AppendLine(@"l_english:");
            sb2.AppendLine(@"NEW_ACHIEVEMENT_2_0_NAME:0 ""Brave New World""");

            var sb3 = new StringBuilder();
            sb3.AppendLine(@"l_english:");
            sb3.AppendLine(@"NEW_ACHIEVEMENT_2_0_DESC:0 ""Colonize a planet""");

            var sb4 = new StringBuilder();
            sb4.AppendLine(@"l_english:");
            sb4.AppendLine(@"NEW_ACHIEVEMENT_2_1_NAME:0 ""Digging Deep""");

            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "loc\\loc.yml",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new LocalizationParser(new CodeParser(), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(3);
            for (int i = 0; i < 3; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("loc\\loc.yml");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().Be(sb2.ToString().Trim());
                        result[i].Id.Should().Be("NEW_ACHIEVEMENT_2_0_NAME");
                        result[i].ValueType.Should().Be(Common.ValueType.SpecialVariable);
                        break;
                    case 1:
                        result[i].Code.Trim().Should().Be(sb3.ToString().Trim());
                        result[i].Id.Should().Be("NEW_ACHIEVEMENT_2_0_DESC");
                        result[i].ValueType.Should().Be(Common.ValueType.SpecialVariable);
                        break;
                    case 2:
                        result[i].Code.Trim().Should().Be(sb4.ToString().Trim());
                        result[i].Id.Should().Be("NEW_ACHIEVEMENT_2_1_NAME");
                        result[i].ValueType.Should().Be(Common.ValueType.SpecialVariable);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("loc\\l_english-yml");
            }
        }
    }
}
