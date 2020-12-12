// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-22-2020
// ***********************************************************************
// <copyright file="GenericBinaryParserTests.cs" company="Mario">
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
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class BinaryParserTests.
    /// </summary>
    public class GenericBinaryParserTests
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
            var parser = new BinaryParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeFalse();
            args.File = "common\\gfx\\test.png";            
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Parse_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_should_yield_results()
        {
            DISetup.SetupContainer();
            
            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\gfx\\test.png",            
                ModName = "fake"
            };
            var parser = new BinaryParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\gfx\\test.png");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().BeNullOrEmpty();
                        result[i].Id.Should().Be("test");
                        result[i].ValueType.Should().Be(ValueType.Binary);
                        break;

                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("common\\gfx\\binary");
            }
        }
    }
}
