// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-21-2020
//
// Last Modified By : Mario
// Last Modified On : 09-01-2020
// ***********************************************************************
// <copyright file="GenericDefinesParserTests.cs" company="Mario">
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
using IronyModManager.Shared;
using IronyModManager.Tests.Common;
using ValueType = IronyModManager.Shared.Models.ValueType;
using Xunit;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class StellarisDefinesParserTests.
    /// </summary>
    public class GenericDefinesParserTests
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
            var parser = new Generic.DefinesParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeFalse();
            args.File = "common\\defines\\t.txt";
            parser.CanParse(args).Should().BeTrue();
            args.File = "common\\defines.luad";
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
            sb2.AppendLine(@"NCamera = {");
            sb2.AppendLine(@"    FOV = 35");
            sb2.Append('}');

            var sb3 = new System.Text.StringBuilder();
            sb3.AppendLine(@"NCamera = {");
            sb3.AppendLine(@"    ENTITY_SPRITE_DESIGN_ENTRY_CAM_DIR = {");
            sb3.AppendLine(@"        -1");
            sb3.AppendLine(@"        -0.6");
            sb3.AppendLine(@"        0.3");
            sb3.AppendLine(@"    }");
            sb3.Append('}');

            var sb4 = new StringBuilder();
            sb4.AppendLine(@"NGraphics = {");
            sb4.AppendLine(@"    CAMERA_DISTANCE_TO_ZOOM = 10");
            sb4.Append('}');


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\defines\\t.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Generic.DefinesParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(3);
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
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Type.Should().Be("common\\defines\\NCamera-txt");
                        break;
                    case 1:
                        result[i].Code.Trim().Should().Be(sb3.ToString().Trim());
                        result[i].Id.Should().Be("ENTITY_SPRITE_DESIGN_ENTRY_CAM_DIR");
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Type.Should().Be("common\\defines\\NCamera-txt");
                        break;
                    case 2:
                        result[i].Code.Trim().Should().Be(sb4.ToString().Trim());
                        result[i].Id.Should().Be("CAMERA_DISTANCE_TO_ZOOM");
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Type.Should().Be("common\\defines\\NGraphics-txt");
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
            }
        }

        /// <summary>
        /// Defines the test method Parse_complex_type_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_complex_type_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"	NInterface = {");
            sb.AppendLine(@"");
            sb.AppendLine(@"		TOPBAR_BUTTONS_SHORTCUTS				= {");
            sb.AppendLine(@"		""contacts"" ""F1""");
            sb.AppendLine(@"		""situation"" ""F2""");
            sb.AppendLine(@"		""technology"" ""F3""");
            sb.AppendLine(@"		""empire"" ""F4""");
            sb.AppendLine(@"		""leaders"" ""F5""");
            sb.AppendLine(@"		""species"" ""F6""");
            sb.AppendLine(@"		""ship_designer"" ""F7""");
            sb.AppendLine(@"		""fleet_manager"" ""F8""");
            sb.AppendLine(@"		""edicts"" ""F9""");
            sb.AppendLine(@"		""policies"" ""F10""");
            sb.AppendLine(@"		}}");

            var sb2 = new StringBuilder();
            sb2.AppendLine(@"NInterface = {");
            sb2.AppendLine(@"	TOPBAR_BUTTONS_SHORTCUTS = {");
            sb2.AppendLine(@"		""contacts""");
            sb2.AppendLine(@"		""F1""");
            sb2.AppendLine(@"		""situation""");
            sb2.AppendLine(@"		""F2""");
            sb2.AppendLine(@"		""technology""");
            sb2.AppendLine(@"		""F3""");
            sb2.AppendLine(@"		""empire""");
            sb2.AppendLine(@"		""F4""");
            sb2.AppendLine(@"		""leaders""");
            sb2.AppendLine(@"		""F5""");
            sb2.AppendLine(@"		""species""");
            sb2.AppendLine(@"		""F6""");
            sb2.AppendLine(@"		""ship_designer""");
            sb2.AppendLine(@"		""F7""");
            sb2.AppendLine(@"		""fleet_manager""");
            sb2.AppendLine(@"		""F8""");
            sb2.AppendLine(@"		""edicts""");
            sb2.AppendLine(@"		""F9""");
            sb2.AppendLine(@"		""policies""");
            sb2.AppendLine(@"		""F10""");
            sb2.AppendLine(@"	}");
            sb2.Append('}');

            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\defines\\t.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Generic.DefinesParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\defines\\t.txt");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("TOPBAR_BUTTONS_SHORTCUTS");
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Code.Should().Be(sb2.ToString().ReplaceTabs());
                        result[i].Type.Should().Be("common\\defines\\NInterface-txt");
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
            }
        }

        /// <summary>
        /// Defines the test method Parse_complex_type_edge_case_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_complex_type_edge_case_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"	NInterface = { TOPBAR_BUTTONS_SHORTCUTS				= {");
            sb.AppendLine(@"		""contacts"" ""F1""");
            sb.AppendLine(@"		""situation"" ""F2""");
            sb.AppendLine(@"		""technology"" ""F3""");
            sb.AppendLine(@"		""empire"" ""F4""");
            sb.AppendLine(@"		""leaders"" ""F5""");
            sb.AppendLine(@"		""species"" ""F6""");
            sb.AppendLine(@"		""ship_designer"" ""F7""");
            sb.AppendLine(@"		""fleet_manager"" ""F8""");
            sb.AppendLine(@"		""edicts"" ""F9""");
            sb.AppendLine(@"		""policies"" ""F10""");
            sb.AppendLine(@"		}}");

            var sb2 = new StringBuilder();
            sb2.AppendLine(@"NInterface = {");
            sb2.AppendLine(@"	TOPBAR_BUTTONS_SHORTCUTS = {");
            sb2.AppendLine(@"		""contacts""");
            sb2.AppendLine(@"		""F1""");
            sb2.AppendLine(@"		""situation""");
            sb2.AppendLine(@"		""F2""");
            sb2.AppendLine(@"		""technology""");
            sb2.AppendLine(@"		""F3""");
            sb2.AppendLine(@"		""empire""");
            sb2.AppendLine(@"		""F4""");
            sb2.AppendLine(@"		""leaders""");
            sb2.AppendLine(@"		""F5""");
            sb2.AppendLine(@"		""species""");
            sb2.AppendLine(@"		""F6""");
            sb2.AppendLine(@"		""ship_designer""");
            sb2.AppendLine(@"		""F7""");
            sb2.AppendLine(@"		""fleet_manager""");
            sb2.AppendLine(@"		""F8""");
            sb2.AppendLine(@"		""edicts""");
            sb2.AppendLine(@"		""F9""");
            sb2.AppendLine(@"		""policies""");
            sb2.AppendLine(@"		""F10""");
            sb2.AppendLine(@"	}");
            sb2.Append('}');

            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\defines\\t.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Generic.DefinesParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\defines\\t.txt");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("TOPBAR_BUTTONS_SHORTCUTS");
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Code.Should().Be(sb2.ToString().ReplaceTabs());
                        result[i].Type.Should().Be("common\\defines\\NInterface-txt");
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
            sb2.AppendLine(@"NGraphics = {");
            sb2.AppendLine(@"    CAMERA_DISTANCE_TO_ZOOM = 10");
            sb2.Append('}');


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\defines\\t.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Generic.DefinesParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
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
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Type.Should().Be("common\\defines\\NGraphics-txt");
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
            }
        }

        /// <summary>
        /// Defines the test method Parse_multiple_types_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_multiple_types_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"NArmy = {");
            sb.AppendLine(@"	ARMY_MILITARY_POWER_EXPONENT = 0.5	# 0.65");
            sb.AppendLine(@"}");
            sb.AppendLine(@"");
            sb.AppendLine(@"NGameplay = {");
            sb.AppendLine(@"	ASCENSION_PERKS_SLOTS = 12");
            sb.AppendLine(@"	JUMP_DRIVE_COOLDOWN = 0");
            sb.AppendLine(@"}");



            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\defines\\t.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Generic.DefinesParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(3);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\defines\\t.txt");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("ARMY_MILITARY_POWER_EXPONENT");
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Type.Should().Be("common\\defines\\NArmy-txt");
                        result[i].Code.Should().Be("NArmy = {\r\n    ARMY_MILITARY_POWER_EXPONENT = 0.5\r\n}");
                        break;
                    case 1:
                        result[i].Id.Should().Be("ASCENSION_PERKS_SLOTS");
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Type.Should().Be("common\\defines\\NGameplay-txt");
                        result[i].Code.Should().Be("NGameplay = {\r\n    ASCENSION_PERKS_SLOTS = 12\r\n}");
                        break;
                    case 2:
                        result[i].Id.Should().Be("JUMP_DRIVE_COOLDOWN");
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Type.Should().Be("common\\defines\\NGameplay-txt");
                        result[i].Code.Should().Be("NGameplay = {\r\n    JUMP_DRIVE_COOLDOWN = 0\r\n}");
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
            }
        }

        /// <summary>
        /// Defines the test method Parse_inline_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_inline_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine("NTest.Path.SubPath = 10");

            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\defines\\t.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Generic.DefinesParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\defines\\t.txt");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("SubPath");
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Type.Should().Be("common\\defines\\NTest.Path-txt");
                        result[i].Code.Should().Be("NTest.Path.SubPath = 10");
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
            }
        }

        /// <summary>
        /// Defines the test method Parse_lua_with_code_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_lua_with_code_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"NDefines_Graphics = {");
            sb.AppendLine(@"");
            sb.AppendLine(@"NWiki = {");
            sb.AppendLine(@"	BASE_URL = ""https://hoi4.paradoxwikis.com/"",");
            sb.AppendLine(@"	FORUM_URL = ""https://forum.paradoxplaza.com/forum/index.php?forums/hearts-of-iron-iv.844/""");
            sb.AppendLine(@"}");
            sb.AppendLine(@"}");
            sb.AppendLine(@"");
            sb.AppendLine(@"for k,v in pairs( NDefines_Graphics ) do NDefines[k] = v end");

            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\defines\\t.lua",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Generic.DefinesParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(2);
            for (int i = 0; i < 2; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\defines\\t.lua");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("BASE_URL");
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Type.Should().Be("common\\defines\\NDefines_Graphics.NWiki-txt");
                        result[i].Code.Should().Be("NDefines_Graphics.NWiki.BASE_URL = \"https://hoi4.paradoxwikis.com/\"");
                        break;
                    case 1:
                        result[i].Id.Should().Be("FORUM_URL");
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Type.Should().Be("common\\defines\\NDefines_Graphics.NWiki-txt");
                        result[i].Code.Should().Be("NDefines_Graphics.NWiki.FORUM_URL = \"https://forum.paradoxplaza.com/forum/index.php?forums/hearts-of-iron-iv.844/\"");
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
            }
        }

        /// <summary>
        /// Defines the test method Parse_lua_complex_object_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_lua_complex_object_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"NDefines_Graphics = {");
            sb.AppendLine(@"	NInterface = {");
            sb.AppendLine(@"		NO_COMBATS_COLOR = { 0.0, 0.0, 0.8 },				-- Color for icons if all combats are successful");
            sb.AppendLine(@"		MAP_MODE_IDEOLOGY_COLOR_TRANSPARENCY = 1");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\defines\\t.lua",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Generic.DefinesParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(2);
            for (int i = 0; i < 2; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\defines\\t.lua");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("NO_COMBATS_COLOR");
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Type.Should().Be("common\\defines\\NDefines_Graphics.NInterface-txt");
                        result[i].Code.Should().Be("NDefines_Graphics.NInterface.NO_COMBATS_COLOR = {\r\n    0,\r\n    0,\r\n    0.8\r\n}");
                        break;
                    case 1:
                        result[i].Id.Should().Be("MAP_MODE_IDEOLOGY_COLOR_TRANSPARENCY");
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Type.Should().Be("common\\defines\\NDefines_Graphics.NInterface-txt");
                        result[i].Code.Should().Be("NDefines_Graphics.NInterface.MAP_MODE_IDEOLOGY_COLOR_TRANSPARENCY = 1");
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
            }
        }

        /// <summary>
        /// Defines the test method Parse_lua_complex_object_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_lua_multiple_complex_object_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"NDefines_Graphics = {");
            sb.AppendLine(@"	NInterface = {");
            sb.AppendLine(@"		NO_COMBATS_COLOR = { 0.0, 0.0, 0.8 },				-- Color for icons if all combats are successful");
            sb.AppendLine(@"		MAP_MODE_IDEOLOGY_COLOR_TRANSPARENCY = 1");
            sb.AppendLine(@"	},");
            sb.AppendLine(@"	NInterface2 = {");
            sb.AppendLine(@"		NO_COMBATS_COLOR = { 0.0, 0.0, 0.8 },				-- Color for icons if all combats are successful");
            sb.AppendLine(@"		MAP_MODE_IDEOLOGY_COLOR_TRANSPARENCY = 1");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\defines\\t.lua",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Generic.DefinesParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(4);
            for (int i = 0; i < 4; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\defines\\t.lua");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("NO_COMBATS_COLOR");
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Type.Should().Be("common\\defines\\NDefines_Graphics.NInterface-txt");
                        result[i].Code.Should().Be("NDefines_Graphics.NInterface.NO_COMBATS_COLOR = {\r\n    0,\r\n    0,\r\n    0.8\r\n}");
                        break;
                    case 1:
                        result[i].Id.Should().Be("MAP_MODE_IDEOLOGY_COLOR_TRANSPARENCY");
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Type.Should().Be("common\\defines\\NDefines_Graphics.NInterface-txt");
                        result[i].Code.Should().Be("NDefines_Graphics.NInterface.MAP_MODE_IDEOLOGY_COLOR_TRANSPARENCY = 1");
                        break;
                    case 2:
                        result[i].Id.Should().Be("NO_COMBATS_COLOR");
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Type.Should().Be("common\\defines\\NDefines_Graphics.NInterface2-txt");
                        result[i].Code.Should().Be("NDefines_Graphics.NInterface2.NO_COMBATS_COLOR = {\r\n    0,\r\n    0,\r\n    0.8\r\n}");
                        break;
                    case 3:
                        result[i].Id.Should().Be("MAP_MODE_IDEOLOGY_COLOR_TRANSPARENCY");
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Type.Should().Be("common\\defines\\NDefines_Graphics.NInterface2-txt");
                        result[i].Code.Should().Be("NDefines_Graphics.NInterface2.MAP_MODE_IDEOLOGY_COLOR_TRANSPARENCY = 1");
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
            }
        }

        /// <summary>
        /// Defines the test method Parse_lua_complex_array_object_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_lua_complex_array_object_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"NDefines_Graphics = {");
            sb.AppendLine(@"	NInterface = {");
            sb.AppendLine(@"	SUPPLY_MAP_MODE_STATUS_COLOR = {");
            sb.AppendLine(@"		0.0,   0.9, 0.0, 0.0, 1.0,			-- #E60000 red");
            sb.AppendLine(@"		0.7,   0.98, 0.4, 0.1, 1.0,			-- #FA661A orange");
            sb.AppendLine(@"		1.0,   0.8, 0.64, 0.2, 1.0,			-- #CCA333 mustard");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\defines\\t.lua",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Generic.DefinesParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\defines\\t.lua");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("SUPPLY_MAP_MODE_STATUS_COLOR");
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Type.Should().Be("common\\defines\\NDefines_Graphics.NInterface-txt");
                        result[i].Code.Should().Be("NDefines_Graphics.NInterface.SUPPLY_MAP_MODE_STATUS_COLOR = {\r\n    0,\r\n    0.9,\r\n    0,\r\n    0,\r\n    1,\r\n    0.7,\r\n    0.98,\r\n    0.4,\r\n    0.1,\r\n    1,\r\n    1,\r\n    0.8,\r\n    0.64,\r\n    0.2,\r\n    1,\r\n}");
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
            }
        }

        /// <summary>
        /// Defines the test method Parse_lua_inline_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_lua_inline_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"NDefines.NOperatives.MAX_RECRUITED_OPERATIVES = 16");
            sb.AppendLine(@"NDefines_Graphics.NMapMode.SUPPLY_MAP_MODE_REACH_COLOR = {");
            sb.AppendLine(@"    0.0,   0.6, 0.0, 0.4, 1.0, 			-- #990066 dark purple");
            sb.AppendLine(@"    0.02,  0.2, 0.17, 0.52, 1.0, 		-- #332B85 dark purple blue");
            sb.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\defines\\t.lua",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new Generic.DefinesParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(2);
            for (int i = 0; i < 2; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\defines\\t.lua");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("MAX_RECRUITED_OPERATIVES");
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Type.Should().Be("common\\defines\\NDefines.NOperatives-txt");
                        result[i].Code.Should().Be("NDefines.NOperatives.MAX_RECRUITED_OPERATIVES = 16");
                        break;

                    case 1:
                        result[i].Id.Should().Be("SUPPLY_MAP_MODE_REACH_COLOR");
                        result[i].ValueType.Should().Be(ValueType.SpecialVariable);
                        result[i].Type.Should().Be("common\\defines\\NDefines_Graphics.NMapMode-txt");
                        result[i].Code.Should().Be("NDefines_Graphics.NMapMode.SUPPLY_MAP_MODE_REACH_COLOR = {\r\n    0,\r\n    0.6,\r\n    0,\r\n    0.4,\r\n    1,\r\n    0.02,\r\n    0.2,\r\n    0.17,\r\n    0.52,\r\n    1,\r\n}");
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
            }
        }
    }
}
