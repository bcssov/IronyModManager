// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-21-2020
//
// Last Modified By : Mario
// Last Modified On : 02-21-2020
// ***********************************************************************
// <copyright file="StellarisDefinesParserTests.cs" company="Mario">
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
    /// Class StellarisDefinesParserTests.
    /// </summary>
    public class StellarisDefinesParserTests
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
            var parser = new Stellaris.DefinesParser();
            parser.CanParse(args).Should().BeFalse();
            args.File = "common\\defines\\t.txt";
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
            sb.AppendLine(@"NCamera = {FOV							= 35 # Field-of-View");
            sb.AppendLine(@"		# Used for all ships");
            sb.AppendLine(@"		ENTITY_SPRITE_DESIGN_ENTRY_CAM_DIR = 	{ -1.0 -0.6 0.3 }");
            sb.AppendLine(@"}");
            sb.AppendLine(@"NGraphics = {");
            sb.AppendLine(@"		CAMERA_DISTANCE_TO_ZOOM				= 10.0");
            sb.AppendLine(@"}");


            var sb2 = new StringBuilder();
            sb2.AppendLine(@"NCamera={");
            sb2.AppendLine(@"FOV=35 # Field-of-View");
            sb2.Append(@"}");


            var sb3 = new StringBuilder();
            sb3.AppendLine(@"NCamera={");
            sb3.AppendLine(@"ENTITY_SPRITE_DESIGN_ENTRY_CAM_DIR={-1.0 -0.6 0.3}");
            sb3.Append(@"}");


            var sb4 = new StringBuilder();
            sb4.AppendLine(@"NGraphics={");
            sb4.AppendLine(@"CAMERA_DISTANCE_TO_ZOOM=10.0");
            sb4.Append(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\defines\\t.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Stellaris.DefinesParser();
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(3);
            for (int i = 0; i < 3; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\defines\\t.txt");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().Be(sb2.ToString().Trim());
                        result[i].Id.Should().Be("FOV");
                        result[i].ValueType.Should().Be(ValueType.Variable);
                        result[i].Type.Should().Be("common\\defines\\NCamera-txt");
                        break;
                    case 1:
                        result[i].Code.Trim().Should().Be(sb3.ToString().Trim());
                        result[i].Id.Should().Be("ENTITY_SPRITE_DESIGN_ENTRY_CAM_DIR");
                        result[i].ValueType.Should().Be(ValueType.Variable);
                        result[i].Type.Should().Be("common\\defines\\NCamera-txt");
                        break;
                    case 2:
                        result[i].Code.Trim().Should().Be(sb4.ToString().Trim());
                        result[i].Id.Should().Be("CAMERA_DISTANCE_TO_ZOOM");
                        result[i].ValueType.Should().Be(ValueType.Variable);
                        result[i].Type.Should().Be("common\\defines\\NGraphics-txt");
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");                
            }
        }

        /// <summary>
        /// Defines the test method Parse_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_ending_edge_case_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"NGraphics = {");
            sb.AppendLine(@"		CAMERA_DISTANCE_TO_ZOOM				= 10.0 }");            

            
            var sb2 = new StringBuilder();
            sb2.AppendLine(@"NGraphics={");
            sb2.AppendLine(@"CAMERA_DISTANCE_TO_ZOOM=10.0");
            sb2.Append(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\defines\\t.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Stellaris.DefinesParser();
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\defines\\t.txt");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().Be(sb2.ToString().Trim());
                        result[i].Id.Should().Be("CAMERA_DISTANCE_TO_ZOOM");
                        result[i].ValueType.Should().Be(ValueType.Variable);
                        result[i].Type.Should().Be("common\\defines\\NGraphics-txt");
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
            }
        }
    }
}
