// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 01-29-2022
//
// Last Modified By : Mario
// Last Modified On : 01-29-2022
// ***********************************************************************
// <copyright file="HOI4KeyValuePairParserTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class HOI4KeyValuePairParserTests
    {
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
            var parser = new Games.HOI4.KeyValuePairParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeFalse();
            args.File = "common\\country_tags\\test.txt";
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
            sb.AppendLine(@"GER	= ""countries/Germany.txt""");
            sb.AppendLine(@"ENG = ""countries/United Kingdom.txt""");
            sb.AppendLine(@"SOV = ""countries/Soviet Union.txt""");

            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\country_tags\\fake.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Games.HOI4.KeyValuePairParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(3);
            for (int i = 0; i < 3; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\country_tags\\fake.txt");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().Be("GER = \"countries/Germany.txt\"");
                        result[i].Id.Should().Be("GER");
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;

                    case 1:
                        result[i].Code.Trim().Should().Be("ENG = \"countries/United Kingdom.txt\"");
                        result[i].Id.Should().Be("ENG");
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;

                    case 2:
                        result[i].Code.Trim().Should().Be("SOV = \"countries/Soviet Union.txt\"");
                        result[i].Id.Should().Be("SOV");
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;

                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("common\\country_tags\\txt");
            }
        }
    }
}
