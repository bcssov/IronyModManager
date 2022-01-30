// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 01-29-2022
//
// Last Modified By : Mario
// Last Modified On : 01-29-2022
// ***********************************************************************
// <copyright file="HOI4KeyParserTests.cs" company="Mario">
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
    /// Class HOI4KeyValuePairParserTests.
    /// </summary>
    public class HOI4KeyParserTests
    {
        #region Methods

        /// <summary>
        /// Defines the test method CanParse_should_be_false_then_true.
        /// </summary>
        [Fact]
        public void CanParse_should_be_false_then_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\country\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.KeyParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeFalse();
            args.File = "common\\bookmarks\\test.txt";
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_difficulty_settings_shoul_be_true.
        /// </summary>
        [Fact]
        public void CanParse_difficulty_settings_shoul_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\difficulty_settings\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.KeyParser(new CodeParser(new Logger()), null);
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
            sb.AppendLine(@"bookmarks = {");
            sb.AppendLine(@"	bookmark = {");
            sb.AppendLine(@"		name = ""GATHERING_STORM_NAME""");
            sb.AppendLine(@"		desc = ""GATHERING_STORM_DESC""");
            sb.AppendLine(@"		date = 1936.1.1.12");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\bookmarks\\fake.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Games.HOI4.KeyParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\bookmarks\\fake.txt");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("GATHERING_STORM_NAME");
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("common\\bookmarks\\txt");
            }
        }

        /// <summary>
        /// Defines the test method Parse_difficulty_settings_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_difficulty_settings_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"difficulty_settings = {");
            sb.AppendLine(@"	difficulty_setting = {");
            sb.AppendLine(@"		key = ""custom_diff_strong_ger""");
            sb.AppendLine(@"		modifier = diff_strong_ai_generic");
            sb.AppendLine(@"		countries = { GER }");
            sb.AppendLine(@"		multiplier = 2.0");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\difficulty_settings\\fake.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Games.HOI4.KeyParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\difficulty_settings\\fake.txt");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("custom_diff_strong_ger");
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("common\\difficulty_settings\\txt");
            }

        }

        #endregion Methods
    }
}
