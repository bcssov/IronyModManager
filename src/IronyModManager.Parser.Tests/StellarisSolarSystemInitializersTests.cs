// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-20-2020
//
// Last Modified By : Mario
// Last Modified On : 03-24-2020
// ***********************************************************************
// <copyright file="StellarisSolarSystemInitializersTests.cs" company="Mario">
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
using IronyModManager.Parser.Games.Stellaris;
using IronyModManager.Shared;
using IronyModManager.Tests.Common;
using Xunit;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class StellarisSolarSystemInitializersTests.
    /// </summary>
    public class StellarisSolarSystemInitializersTests
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
            var parser = new SolarSystemInitializersParser(new CodeParser());
            parser.CanParse(args).Should().BeFalse();
            args.File = "common\\solar_system_initializers\\test.txt";
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
            sb.AppendLine(@"@test = 1");
            sb.AppendLine(@"");
            sb.AppendLine(@"sol_system_initializer = {");
            sb.AppendLine(@"	name = ""NAME_Sol""");
            sb.AppendLine(@"	a = 1");
            sb.AppendLine(@"	b = 2");
            sb.AppendLine(@"}");
            sb.AppendLine(@"");
            sb.AppendLine(@"sol_system_initializer2 = {");
            sb.AppendLine(@"	name = ""NAME_Sol""");
            sb.AppendLine(@"	a = 1");
            sb.AppendLine(@"	b = 2");
            sb.AppendLine(@"}");
            sb.AppendLine(@"");
            sb.AppendLine(@"sol_system_initializer3 = {");
            sb.AppendLine(@"	name = ""NAME_Sol2""");
            sb.AppendLine(@"	a = 1");
            sb.AppendLine(@"	b = 2");
            sb.AppendLine(@"}");


            var sb2 = new StringBuilder();
            sb2.AppendLine(@"sol_system_initializer = {");
            sb2.AppendLine(@"	name = ""NAME_Sol""");
            sb2.AppendLine(@"	a = 1");
            sb2.AppendLine(@"	b = 2");
            sb2.AppendLine(@"}");

            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\solar_system_initializers\\fake.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new SolarSystemInitializersParser(new CodeParser());
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(4);
            for (int i = 0; i < 4; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\solar_system_initializers\\fake.txt");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().Be("@test = 1");
                        result[i].Id.Should().Be("@test");
                        result[i].ValueType.Should().Be(Common.ValueType.Variable);
                        break;
                    case 1:
                        result[i].Id.Should().Be("sol_system_initializer");
                        result[i].Code.Should().Be(sb2.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(Common.ValueType.Object);
                        break;
                    case 2:
                        result[i].Id.Should().Be("sol_system_initializer2");
                        result[i].ValueType.Should().Be(Common.ValueType.Object);
                        break;
                    case 3:
                        result[i].Id.Should().Be("sol_system_initializer3");
                        result[i].ValueType.Should().Be(Common.ValueType.Object);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("common\\solar_system_initializers\\txt");
            }
        }
    }
}
