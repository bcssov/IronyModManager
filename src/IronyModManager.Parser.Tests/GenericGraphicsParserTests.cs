// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="GenericGraphicsParserTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AwesomeAssertions;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Generic;
using IronyModManager.Shared;
using IronyModManager.Tests.Common;
using ValueType = IronyModManager.Shared.Models.ValueType;
using Xunit;
using IronyModManager.Services;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class GenericGraphicsParserTests.
    /// </summary>
    public class GenericGraphicsParserTests
    {
        /// <summary>
        /// Defines the test method CanParse_gui_should_be_false_then_true.
        /// </summary>
        [Fact]
        public void CanParse_gui_should_be_false_then_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\gamerules\\test.txt",
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeFalse();
            args.File = "gui\\gui.gui";
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_gui_wrong_extension.
        /// </summary>
        [Fact]
        public void CanParse_gui_wrong_extension()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"guiTypes = {	");
            sb.AppendLine(@"	containerWindowType = {");
            sb.AppendLine(@"		name = ""test""");
            sb.AppendLine(@"		x = 1");
            sb.AppendLine(@"	}		");
            sb.AppendLine(@"	");
            sb.AppendLine(@"	containerWindowType = {");
            sb.AppendLine(@"		name = ""test2"" ");
            sb.AppendLine(@"	}			");
            sb.AppendLine(@"}");
            var args = new CanParseArgs()
            {
                File = "gui\\gui.bak",
                Lines = sb.ToString().SplitOnNewLine()
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Parse_gui_edge_case_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_gui_edge_case_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"guiTypes = {	");
            sb.AppendLine(@"	containerWindowType = { ");
            sb.AppendLine(@"		name = ""test""");
            sb.AppendLine(@"	}		");
            sb.AppendLine(@"}");
            sb.AppendLine(@"guiTypes = {	");
            sb.AppendLine(@"	containerWindowType = { ");
            sb.AppendLine(@"		name = ""test2""");
            sb.AppendLine(@"	}		");
            sb.AppendLine(@"}");

            var sb2 = new StringBuilder();
            sb2.AppendLine(@"guiTypes = {");
            sb2.AppendLine(@"	containerWindowType = {");
            sb2.AppendLine(@"		name = ""test""");
            sb2.AppendLine(@"	}");
            sb2.AppendLine(@"}");

            var sb3 = new StringBuilder();
            sb3.AppendLine(@"guiTypes = {");
            sb3.AppendLine(@"	containerWindowType = {");
            sb3.AppendLine(@"		name = ""test2""");
            sb3.AppendLine(@"	}");
            sb3.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gui\\gui.gui",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(2);
            for (int i = 0; i < 2; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gui\\gui.gui");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("test");
                        result[i].Code.Should().Be(sb2.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    case 1:
                        result[i].Id.Should().Be("test2");
                        result[i].Code.Should().Be(sb3.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gui\\guitypes\\gui");
            }
        }

        /// <summary>
        /// Defines the test method Parse_gui_ending_edge_case_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_gui_ending_edge_case_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"guiTypes = {	");
            sb.AppendLine(@"	containerWindowType = { ");
            sb.AppendLine(@"		name = ""test"" }}");

            var sb2 = new StringBuilder();
            sb2.AppendLine(@"guiTypes = {");
            sb2.AppendLine(@"    containerWindowType = {");
            sb2.AppendLine(@"        name = ""test""");
            sb2.AppendLine(@"    }");
            sb2.AppendLine(@"}");



            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gui\\gui.gui",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gui\\gui.gui");
                switch (i)
                {

                    case 0:
                        result[i].Id.Should().Be("test");
                        result[i].Code.Should().Be(sb2.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gui\\guitypes\\gui");
            }
        }

        /// <summary>
        /// Defines the test method Parse_gui_inline_edge_case_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_gui_inline_edge_case_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"guiTypes = { containerWindowType = { name = ""test"" } }");

            var sb2 = new System.Text.StringBuilder();
            sb2.AppendLine(@"guiTypes = {");
            sb2.AppendLine(@"    containerWindowType = {");
            sb2.AppendLine(@"        name = ""test""");
            sb2.AppendLine(@"    }");
            sb2.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gui\\gui.gui",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gui\\gui.gui");
                switch (i)
                {

                    case 0:
                        result[i].Id.Should().Be("test");
                        result[i].Code.Should().Be(sb2.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gui\\guitypes\\gui");
            }
        }

        /// <summary>
        /// Defines the test method Parse_gui_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_gui_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"guiTypes = {	");
            sb.AppendLine(@"	containerWindowType = {");
            sb.AppendLine(@"		name = ""test""");
            sb.AppendLine(@"		x = 1");
            sb.AppendLine(@"	}		");
            sb.AppendLine(@"	");
            sb.AppendLine(@"	containerWindowType = {");
            sb.AppendLine(@"		name = ""test2"" ");
            sb.AppendLine(@"	}			");
            sb.AppendLine(@"}");


            var sb2 = new StringBuilder();
            sb2.AppendLine(@"guiTypes = {");
            sb2.AppendLine(@"	containerWindowType = {");
            sb2.AppendLine(@"		name = ""test""");
            sb2.AppendLine(@"		x = 1");
            sb2.AppendLine(@"	}");
            sb2.AppendLine(@"}");

            var sb3 = new StringBuilder();
            sb3.AppendLine(@"guiTypes = {");
            sb3.AppendLine(@"	containerWindowType = {");
            sb3.AppendLine(@"		name = ""test2""");
            sb3.AppendLine(@"	}");
            sb3.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gui\\gui.gui",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(2);
            for (int i = 0; i < 2; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gui\\gui.gui");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("test");
                        result[i].Code.Should().Be(sb2.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    case 1:
                        result[i].Id.Should().Be("test2");
                        result[i].Code.Should().Be(sb3.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gui\\guitypes\\gui");
            }
        }

        /// <summary>
        /// Defines the test method Parse_gui_wrong_extension_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_gui_wrong_extension_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"guiTypes = {	");
            sb.AppendLine(@"	containerWindowType = {");
            sb.AppendLine(@"		name = ""test""");
            sb.AppendLine(@"		x = 1");
            sb.AppendLine(@"	}		");
            sb.AppendLine(@"	");
            sb.AppendLine(@"	containerWindowType = {");
            sb.AppendLine(@"		name = ""test2"" ");
            sb.AppendLine(@"	}			");
            sb.AppendLine(@"}");


            var sb2 = new StringBuilder();
            sb2.AppendLine(@"guiTypes = {");
            sb2.AppendLine(@"	containerWindowType = {");
            sb2.AppendLine(@"		name = ""test""");
            sb2.AppendLine(@"		x = 1");
            sb2.AppendLine(@"	}");
            sb2.AppendLine(@"}");

            var sb3 = new StringBuilder();
            sb3.AppendLine(@"guiTypes = {");
            sb3.AppendLine(@"	containerWindowType = {");
            sb3.AppendLine(@"		name = ""test2""");
            sb3.AppendLine(@"	}");
            sb3.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gui\\gui.bak",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(2);
            for (int i = 0; i < 2; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gui\\gui.bak");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("test");
                        result[i].Code.Should().Be(sb2.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    case 1:
                        result[i].Id.Should().Be("test2");
                        result[i].Code.Should().Be(sb3.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gui\\guitypes\\gui");
            }
        }

        /// <summary>
        /// Defines the test method CanParse_gfx_should_be_false_then_true.
        /// </summary>
        [Fact]
        public void CanParse_gfx_should_be_false_then_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\gamerules\\test.txt",
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeFalse();
            args.File = "gfx\\gfx.gfx";
            parser.CanParse(args).Should().BeTrue();

        }

        /// <summary>
        /// Defines the test method CanParse_gfx_wrong_extension_should_true.
        /// </summary>
        [Fact]
        public void CanParse_gfx_wrong_extension_should_true()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"bitmapfonts = {");
            sb.AppendLine(@"");
            sb.AppendLine(@"	### ship size icons ###");
            sb.AppendLine(@"");
            sb.AppendLine(@"	bitmapfont = {");
            sb.AppendLine(@"		name = ""GFX_text_military_size_1""");
            sb.AppendLine(@"		texturefile = ""gfx/interface/icons/text_icons/icon_text_military_size_1.dds""");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	");
            sb.AppendLine(@"	bitmapfont = {");
            sb.AppendLine(@"		name = ""GFX_text_military_size_2""");
            sb.AppendLine(@"		texturefile = ""gfx/interface/icons/text_icons/icon_text_military_size_2.dds""");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");
            var args = new CanParseArgs()
            {
                File = "gfx\\gfx.gfx",
                Lines = sb.ToString().SplitOnNewLine()
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);            
            parser.CanParse(args).Should().BeTrue();

        }

        /// <summary>
        /// Defines the test method Parse_gfx_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_gfx_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"bitmapfonts = {");
            sb.AppendLine(@"");
            sb.AppendLine(@"	### ship size icons ###");
            sb.AppendLine(@"");
            sb.AppendLine(@"	bitmapfont = {");
            sb.AppendLine(@"		name = ""GFX_text_military_size_1""");
            sb.AppendLine(@"		texturefile = ""gfx/interface/icons/text_icons/icon_text_military_size_1.dds""");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	");
            sb.AppendLine(@"	bitmapfont = {");
            sb.AppendLine(@"		name = ""GFX_text_military_size_2""");
            sb.AppendLine(@"		texturefile = ""gfx/interface/icons/text_icons/icon_text_military_size_2.dds""");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");


            var sb2 = new StringBuilder();
            sb2.AppendLine(@"bitmapfonts = {");
            sb2.AppendLine(@"	bitmapfont = {");
            sb2.AppendLine(@"		name = ""GFX_text_military_size_1""");
            sb2.AppendLine(@"		texturefile = ""gfx/interface/icons/text_icons/icon_text_military_size_1.dds""");
            sb2.AppendLine(@"	}");
            sb2.AppendLine(@"}");


            var sb3 = new System.Text.StringBuilder();
            sb3.AppendLine(@"bitmapfonts = {");
            sb3.AppendLine(@"	bitmapfont = {");
            sb3.AppendLine(@"		name = ""GFX_text_military_size_2""");
            sb3.AppendLine(@"		texturefile = ""gfx/interface/icons/text_icons/icon_text_military_size_2.dds""");
            sb3.AppendLine(@"	}");
            sb3.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gfx\\gfx.gfx",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(2);
            for (int i = 0; i < 2; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gfx\\gfx.gfx");
                switch (i)
                {

                    case 0:
                        result[i].Id.Should().Be("GFX_text_military_size_1");
                        result[i].Code.Should().Be(sb2.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    case 1:
                        result[i].Id.Should().Be("GFX_text_military_size_2");
                        result[i].Code.Should().Be(sb3.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gfx\\bitmapfonts\\gfx");
            }
        }

        /// <summary>
        /// Defines the test method Parse_gfx_wrong_extension_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_gfx_wrong_extension_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"bitmapfonts = {");
            sb.AppendLine(@"");
            sb.AppendLine(@"	### ship size icons ###");
            sb.AppendLine(@"");
            sb.AppendLine(@"	bitmapfont = {");
            sb.AppendLine(@"		name = ""GFX_text_military_size_1""");
            sb.AppendLine(@"		texturefile = ""gfx/interface/icons/text_icons/icon_text_military_size_1.dds""");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	");
            sb.AppendLine(@"	bitmapfont = {");
            sb.AppendLine(@"		name = ""GFX_text_military_size_2""");
            sb.AppendLine(@"		texturefile = ""gfx/interface/icons/text_icons/icon_text_military_size_2.dds""");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");


            var sb2 = new StringBuilder();
            sb2.AppendLine(@"bitmapfonts = {");
            sb2.AppendLine(@"	bitmapfont = {");
            sb2.AppendLine(@"		name = ""GFX_text_military_size_1""");
            sb2.AppendLine(@"		texturefile = ""gfx/interface/icons/text_icons/icon_text_military_size_1.dds""");
            sb2.AppendLine(@"	}");
            sb2.AppendLine(@"}");


            var sb3 = new System.Text.StringBuilder();
            sb3.AppendLine(@"bitmapfonts = {");
            sb3.AppendLine(@"	bitmapfont = {");
            sb3.AppendLine(@"		name = ""GFX_text_military_size_2""");
            sb3.AppendLine(@"		texturefile = ""gfx/interface/icons/text_icons/icon_text_military_size_2.dds""");
            sb3.AppendLine(@"	}");
            sb3.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gfx\\gfx.bak",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(2);
            for (int i = 0; i < 2; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gfx\\gfx.bak");
                switch (i)
                {

                    case 0:
                        result[i].Id.Should().Be("GFX_text_military_size_1");
                        result[i].Code.Should().Be(sb2.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    case 1:
                        result[i].Id.Should().Be("GFX_text_military_size_2");
                        result[i].Code.Should().Be(sb3.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gfx\\bitmapfonts\\gfx");
            }
        }

        /// <summary>
        /// Defines the test method Parse_gfx_ending_edge_case_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_gfx_ending_edge_case_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"bitmapfonts = {");
            sb.AppendLine(@"	bitmapfont = {");
            sb.AppendLine(@"		name = ""GFX_text_military_size_1""");
            sb.AppendLine(@"		texturefile = ""gfx/interface/icons/text_icons/icon_text_military_size_1.dds"" } }");

            var sb2 = new StringBuilder();
            sb2.AppendLine(@"bitmapfonts = {");
            sb2.AppendLine(@"	bitmapfont = {");
            sb2.AppendLine(@"		name = ""GFX_text_military_size_1""");
            sb2.AppendLine(@"		texturefile = ""gfx/interface/icons/text_icons/icon_text_military_size_1.dds""");
            sb2.AppendLine(@"    }");
            sb2.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gfx\\gfx.gfx",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gfx\\gfx.gfx");
                switch (i)
                {

                    case 0:
                        result[i].Id.Should().Be("GFX_text_military_size_1");
                        result[i].Code.Should().Be(sb2.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gfx\\bitmapfonts\\gfx");
            }
        }

        /// <summary>
        /// Defines the test method Parse_gfx_bitmap_type_override_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_gfx_bitmap_type_override_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"bitmapfonts = {");
            sb.AppendLine(@"	 bitmapfont_override = {");
            sb.AppendLine(@"		name = ""large_title_font""");
            sb.AppendLine(@"        ttf_font = ""Easter_bigger""");
            sb.AppendLine(@"        ttf_size = ""30""");
            sb.AppendLine(@"		languages = { ""l_russian"" ""l_polish"" }");
            sb.AppendLine(@"	 }	");
            sb.AppendLine(@"	 bitmapfont_override = {");
            sb.AppendLine(@"		name = ""large_title_font""");
            sb.AppendLine(@"        ttf_font = ""Easter_bigger""");
            sb.AppendLine(@"        ttf_size = ""30""");
            sb.AppendLine(@"		languages = { ""l_russian"" ""l_polish"" }");
            sb.AppendLine(@"	 }	");
            sb.AppendLine(@"}");


            var sb2 = new StringBuilder();
            sb2.AppendLine(@"bitmapfonts = {");
            sb2.AppendLine(@"	bitmapfont_override = {");
            sb2.AppendLine(@"		name = ""large_title_font""");
            sb2.AppendLine(@"		ttf_font = ""Easter_bigger""");
            sb2.AppendLine(@"		ttf_size = ""30""");
            sb2.AppendLine(@"		languages = {");
            sb2.AppendLine(@"			""l_russian""");
            sb2.AppendLine(@"			""l_polish""");
            sb2.AppendLine(@"		}");
            sb2.AppendLine(@"	}");
            sb2.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gfx\\gfx.gfx",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(2);
            for (int i = 0; i < 2; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gfx\\gfx.gfx");
                switch (i)
                {

                    case 0:
                        result[i].Id.Should().Be("l_polish-l_russian-large_title_font");
                        result[i].Code.Should().Be(sb2.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    case 1:
                        result[i].Id.Should().Be("l_polish-l_russian-large_title_font");
                        result[i].Code.Should().Be(sb2.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gfx\\bitmapfonts\\gfx");
            }
        }

        /// <summary>
        /// Defines the test method Parse_variable_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_gfx_variable_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"@test1 = 0");
            sb.AppendLine(@"spriteTypes = {");
            sb.AppendLine(@"	@test2 = 1");
            sb.AppendLine(@"	spriteType = {");
            sb.AppendLine(@"	    @test3 = 1");
            sb.AppendLine(@"		name = ""GFX_dmm_mod_1""");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");

            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gfx\\gfx.gfx",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(4);
            for (int i = 0; i < 4; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gfx\\gfx.gfx");
                switch (i)
                {

                    case 1:
                        result[i].Id.Should().Be("@test1");
                        result[i].CodeTag.Should().BeNullOrWhiteSpace();
                        result[i].ValueType.Should().Be(ValueType.Variable);
                        break;
                    case 2:
                        result[i].Id.Should().Be("@test2");
                        result[i].CodeTag.Should().Be("spriteTypes");
                        result[i].ValueType.Should().Be(ValueType.Variable);
                        break;
                    case 0:
                        result[i].Id.Should().Be("GFX_dmm_mod_1");
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;

                    case 3:
                        result[i].Id.Should().Be("@test3");
                        result[i].CodeTag.Should().Be("spriteTypes");
                        result[i].ValueType.Should().Be(ValueType.Variable);
                        break;

                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gfx\\spritetypes\\gfx");
            }
        }

        /// <summary>
        /// Defines the test method Parse_variable_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_gui_variable_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"@sort_button_height = 80");
            sb.AppendLine(@"guiTypes = {");
            sb.AppendLine(@"	@entry_info_height = 17");
            sb.AppendLine(@"	# Button in the lower right of the main view, opening the Alliance View.");
            sb.AppendLine(@"	containerWindowType = {");
            sb.AppendLine(@"	    @why_am_i_here = 17");
            sb.AppendLine(@"		name = ""alliance_button_window""");
            sb.AppendLine(@"		position = { x = -458 y = 43 }");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gui\\gui.gui",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(4);
            for (int i = 0; i < 4; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gui\\gui.gui");
                switch (i)
                {
                    case 1:
                        result[i].Id.Should().Be("@sort_button_height");
                        result[i].CodeTag.Should().BeNullOrWhiteSpace();
                        result[i].ValueType.Should().Be(ValueType.Variable);
                        break;
                    case 2:
                        result[i].Id.Should().Be("@entry_info_height");
                        result[i].CodeTag.Should().Be("guiTypes");
                        result[i].ValueType.Should().Be(ValueType.Variable);
                        break;
                    case 0:
                        result[i].Id.Should().Be("alliance_button_window");
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    case 3:
                        result[i].Id.Should().Be("@why_am_i_here");
                        result[i].CodeTag.Should().Be("guiTypes");
                        result[i].ValueType.Should().Be(ValueType.Variable);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gui\\guitypes\\gui");
            }
        }

        /// <summary>
        /// Defines the test method Parse_gfx_replace_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_gfx_replace_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"spriteTypes = {");
            sb.AppendLine(@"	spriteType = {");
            sb.AppendLine(@"		name = ""GFX_dmm_mod_1""");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");

            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "interface\\replace\\gfx.gfx",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("interface\\replace\\gfx.gfx");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("GFX_dmm_mod_1");
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("interface\\spritetypes\\gfx");
                result[i].VirtualPath.Should().Be("interface\\spriteTypes\\gfx.gfx");
            }
        }

        /// <summary>
        /// Defines the test method CanParse_asset_should_be_false_then_true.
        /// </summary>
        [Fact]
        public void CanParse_asset_should_be_false_then_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\gamerules\\test.txt",
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeFalse();
            args.File = "gfx\\gfx.asset";
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_asset_wrong_extension_should_true.
        /// </summary>
        [Fact]
        public void CanParse_asset_wrong_extension_should_true()
        {

            var sb = new StringBuilder();
            sb.AppendLine(@"entity = {");
            sb.AppendLine(@"    name = ""ai_01_blue_sponsored_colonizer_entity""");
            sb.AppendLine(@"    clone = ""ai_01_blue_colonizer_entity""");
            sb.AppendLine(@"}");
            var args = new CanParseArgs()
            {
                File = "gfx\\gfx.asset",
                Lines = sb.ToString().SplitOnNewLine()
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);            
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Parse_asset_inline_edge_case_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_asset_inline_edge_case_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"entity = { name = ""ai_01_blue_sponsored_colonizer_entity"" clone = ""ai_01_blue_colonizer_entity"" }");

            var sb2 = new StringBuilder();
            sb2.AppendLine(@"entity = {");
            sb2.AppendLine(@"    name = ""ai_01_blue_sponsored_colonizer_entity""");
            sb2.AppendLine(@"    clone = ""ai_01_blue_colonizer_entity""");
            sb2.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gfx\\fake.asset",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gfx\\fake.asset");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().Be(sb2.ToString().Trim());
                        result[i].Id.Should().Be("ai_01_blue_sponsored_colonizer_entity");
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gfx\\entity\\asset");
            }
        }

        /// <summary>
        /// Defines the test method Parse_asset_wrong_extension_inline_edge_case_should_yield_results.
        /// </summary>
        [Fact]
        public void Parse_asset_wrong_extension_inline_edge_case_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"entity = { name = ""ai_01_blue_sponsored_colonizer_entity"" clone = ""ai_01_blue_colonizer_entity"" }");

            var sb2 = new StringBuilder();
            sb2.AppendLine(@"entity = {");
            sb2.AppendLine(@"    name = ""ai_01_blue_sponsored_colonizer_entity""");
            sb2.AppendLine(@"    clone = ""ai_01_blue_colonizer_entity""");
            sb2.AppendLine(@"}");


            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "gfx\\fake.bak",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new GraphicsParser(new ObjectClone(), new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("gfx\\fake.bak");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().Be(sb2.ToString().Trim());
                        result[i].Id.Should().Be("ai_01_blue_sponsored_colonizer_entity");
                        result[i].ValueType.Should().Be(ValueType.Object);
                        break;
                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("gfx\\entity\\asset");
            }
        }
    }
}
