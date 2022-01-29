// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2022
// ***********************************************************************
// <copyright file="HOI4WholeTextParserTests.cs" company="Mario">
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
using IronyModManager.Parser.Games.HOI4;
using IronyModManager.Tests.Common;
using Xunit;
using ValueType = IronyModManager.Shared.Models.ValueType;


namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class StellarisWholeTextParserTests.
    /// </summary>
    public class HOI4WholeTextParserTests
    {
        /// <summary>
        /// Defines the test method CanParse_map_galaxy_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_graphicalculturetype_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\graphicalculturetype.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_map_csv_file_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_map_csv_file_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "map\\fake.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_countries_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_countries_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\countries\\fake.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_ideas_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_ideas_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\ideas\\fake.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_aistrategyplans_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_aistrategyplans_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\ai_strategy_plans\\fake.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_aistrategy_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_aistrategy_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\ai_strategy\\fake.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_intelligence_agencies_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_intelligence_agencies_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\intelligence_agencies\\fake.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_scripted_gui_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_scripted_gui_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\scripted_guis\\fake.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new WholeTextParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_units_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_units_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\units\\test\\fake.txt",
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
            sb.AppendLine(@"test1");
            sb.AppendLine(@"test2");

            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\graphicalculturetype.txt",
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
                result[i].File.Should().Be("common\\graphicalculturetype.txt");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().Be(sb.ToString().Trim());
                        result[i].Id.Should().Be("graphicalculturetype.txt");
                        result[i].ValueType.Should().Be(ValueType.WholeTextFile);
                        break;

                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("common\\txt");
            }
        }
    }
}
