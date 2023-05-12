// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 05-25-2020
//
// Last Modified By : Mario
// Last Modified On : 10-14-2022
// ***********************************************************************
// <copyright file="StellarisOverwrittenParserTests.cs" company="Mario">
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
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class StellarisWholeTextParserTests.
    /// </summary>
    public class StellarisOverwrittenParserTests
    {
        #region Methods

        /// <summary>
        /// Defines the test method CanParse_districts_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_districts_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\districts\\t.txt",
                GameType = "Stellaris"
            };
            var parser = new OverwrittenParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_planet_classes_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_planet_classes_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\planet_classes\\t.txt",
                GameType = "Stellaris"
            };
            var parser = new OverwrittenParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_pop_jobs_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_pop_jobs_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\pop_jobs\\t.txt",
                GameType = "Stellaris"
            };
            var parser = new OverwrittenParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_should_be_false.
        /// </summary>
        [Fact]
        public void CanParse_should_be_false()
        {
            var args = new CanParseArgs()
            {
                File = "common\\ship_designs\\test.txt",
                GameType = "Stellaris"
            };
            var parser = new OverwrittenParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method CanParse_traits_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_traits_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\traits\\t.txt",
                GameType = "Stellaris"
            };
            var parser = new OverwrittenParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_prescripted_countries_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_prescripted_countries_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "prescripted_countries\\t.txt",
                GameType = "Stellaris"
            };
            var parser = new OverwrittenParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_species_archetypes_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_species_archetypes_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\species_archetypes\\t.txt",
                GameType = "Stellaris"
            };
            var parser = new OverwrittenParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_buildings_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_buildings_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\buildings\\t.txt",
                GameType = "Stellaris"
            };
            var parser = new OverwrittenParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_diplo_actions_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_diplo_actions_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\diplomatic_actions\\t.txt",
                GameType = "Stellaris"
            };
            var parser = new OverwrittenParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_technology_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_technology_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\technology\\t.txt",
                GameType = "Stellaris"
            };
            var parser = new OverwrittenParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_country_types_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_country_types_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\country_types\\t.txt",
                GameType = "Stellaris"
            };
            var parser = new OverwrittenParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_terraform_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_terraform_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\terraform\\test.txt",
                GameType = "Stellaris"
            };
            var parser = new OverwrittenParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_relics_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_relics_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\relics\\test.txt",
                GameType = "Stellaris"
            };
            var parser = new OverwrittenParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_section_templates_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_section_templates_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\section_templates\\test.txt",
                GameType = "Stellaris"
            };
            var parser = new OverwrittenParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_opinion_modifiers_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_opinion_modifiers_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\opinion_modifiers\\test.txt",
                GameType = "Stellaris"
            };
            var parser = new OverwrittenParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanParse_species_rights_should_be_true.
        /// </summary>
        [Fact]
        public void CanParse_species_rights_should_be_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\species_rights\\test\\test.txt",
                GameType = "Stellaris"
            };
            var parser = new OverwrittenParser(new CodeParser(new Logger()), null);
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
            sb.AppendLine(@"asl_mode_options_1 = {");
            sb.AppendLine(@"	potential = {");
            sb.AppendLine(@"		NOT = {");
            sb.AppendLine(@"			has_country_flag = asl_modify_3");
            sb.AppendLine(@"			has_country_flag = asl_modify_5");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	allow = {");
            sb.AppendLine(@"		always = yes");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	effect = {");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");
            sb.AppendLine(@"");
            sb.AppendLine(@"asl_mode_options_3 = {");
            sb.AppendLine(@"	potential = {");
            sb.AppendLine(@"		has_country_flag = asl_modify_3");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	allow = {");
            sb.AppendLine(@"		always = yes");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	effect = {");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");
            sb.AppendLine(@"");
            sb.AppendLine(@"asl_mode_options_5 = {");
            sb.AppendLine(@"	potential = {");
            sb.AppendLine(@"		has_country_flag = asl_modify_5");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	allow = {");
            sb.AppendLine(@"		always = yes");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	effect = {");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");
            sb.AppendLine(@"");
            sb.AppendLine(@"### END TEMPLATE:effects ###");

            var sb2 = new StringBuilder();
            sb2.AppendLine(@"asl_mode_options_1 = {");
            sb2.AppendLine(@"	potential = {");
            sb2.AppendLine(@"		NOT = {");
            sb2.AppendLine(@"			has_country_flag = asl_modify_3");
            sb2.AppendLine(@"			has_country_flag = asl_modify_5");
            sb2.AppendLine(@"		}");
            sb2.AppendLine(@"	}");
            sb2.AppendLine(@"	allow = {");
            sb2.AppendLine(@"		always = yes");
            sb2.AppendLine(@"	}");
            sb2.AppendLine(@"	effect = {");
            sb2.AppendLine(@"	}");
            sb2.AppendLine(@"}");

            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\fake\\fake.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new OverwrittenParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(4);
            for (int i = 0; i < 4; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\fake\\fake.txt");
                switch (i)
                {
                    case 0:
                        result[i].Code.Trim().Should().Be("@test = 1");
                        result[i].Id.Should().Be("@test");
                        result[i].ValueType.Should().Be(ValueType.Variable);
                        break;

                    case 1:
                        result[i].Id.Should().Be("asl_mode_options_1");
                        result[i].Code.Should().Be(sb2.ToString().Trim().ReplaceTabs());
                        result[i].ValueType.Should().Be(ValueType.OverwrittenObject);
                        break;

                    case 2:
                        result[i].Id.Should().Be("asl_mode_options_3");
                        result[i].ValueType.Should().Be(ValueType.OverwrittenObject);
                        break;

                    case 3:
                        result[i].Id.Should().Be("asl_mode_options_5");
                        result[i].ValueType.Should().Be(ValueType.OverwrittenObject);
                        break;

                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("common\\fake\\txt");
            }
        }

        /// <summary>
        /// Defines the test method Parse_should_parse_terraform_links.
        /// </summary>
        [Fact]
        public void Parse_should_parse_terraform_links()
        {
            DISetup.SetupContainer();
            var sb = new StringBuilder();
            sb.AppendLine(@"terraform_link = {");
            sb.AppendLine(@"	from = ""pc_frozen""");
            sb.AppendLine(@"	to = ""pc_arctic""");
            sb.AppendLine(@"	");
            sb.AppendLine(@"	energy = 30000");
            sb.AppendLine(@"	duration = 7200");
            sb.AppendLine(@"	");
            sb.AppendLine(@"	condition = {");
            sb.AppendLine(@"		has_technology = ""test""");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");

            var args = new ParserArgs()
            {
                ContentSHA = "sha",
                ModDependencies = new List<string> { "1" },
                File = "common\\terraform\\fake.txt",
                Lines = sb.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                ModName = "fake"
            };
            var parser = new OverwrittenParser(new CodeParser(new Logger()), null);
            var result = parser.Parse(args).ToList();
            result.Should().NotBeNullOrEmpty();
            result.Count.Should().Be(1);
            for (int i = 0; i < 1; i++)
            {
                result[i].ContentSHA.Should().Be("sha");
                result[i].Dependencies.First().Should().Be("1");
                result[i].File.Should().Be("common\\terraform\\fake.txt");
                switch (i)
                {
                    case 0:
                        result[i].Id.Should().Be("pc_frozen-pc_arctic");
                        result[i].ValueType.Should().Be(ValueType.OverwrittenObject);
                        break;

                    default:
                        break;
                }
                result[i].ModName.Should().Be("fake");
                result[i].Type.Should().Be("common\\terraform\\txt");
            }
        }

        #endregion Methods
    }
}
