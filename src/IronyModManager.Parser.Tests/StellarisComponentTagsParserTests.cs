// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-22-2020
// ***********************************************************************
// <copyright file="StellarisComponentTagsParserTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using IronyModManager.Parser.Games.Stellaris;
using IronyModManager.Tests.Common;
using Xunit;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class ComponentTagsParserTests.
    /// </summary>
    public class StellarisComponentTagsParserTests
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
            var parser = new ComponentTagsParser(new TextParser());
            parser.CanParse(args).Should().BeFalse();
            args.File = "common\\component_tags\\test.txt";
            args.Lines = new List<string> { "test", "test2 = {}" };
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
            sb.AppendLine(@"#weapon_type_kinetic");
            sb.AppendLine(@"weapon_type_kinetic");
            sb.AppendLine(@"weapon_type_explosive");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\component_tags\\fake.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new ComponentTagsParser(new TextParser());
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(2);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\component_tags\\fake.txt");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().Be("weapon_type_kinetic");
                        result[i].Id.Should().Be("weapon_type_kinetic");
                        result[i].ValueType.Should().Be(ValueType.Variable);
                        break;
                    case 1:
                        result[i].Code.Trim().Should().Be("weapon_type_explosive");
                        result[i].Id.Should().Be("weapon_type_explosive");
                        result[i].ValueType.Should().Be(ValueType.Variable);
                        break;

                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("common\\component_tags\\txt");
            }
        }
    }
}
