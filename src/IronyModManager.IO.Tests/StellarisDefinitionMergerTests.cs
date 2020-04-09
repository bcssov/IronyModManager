// ***********************************************************************
// Assembly         : IronyModManager.IO.Tests
// Author           : Mario
// Created          : 04-05-2020
//
// Last Modified By : Mario
// Last Modified On : 04-05-2020
// ***********************************************************************
// <copyright file="StellarisDefinitionMergerTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using IronyModManager.IO.Mods.Mergers;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Definitions;
using Xunit;

namespace IronyModManager.IO.Tests
{
    /// <summary>
    /// Class StellarisDefinitionMergerTests.
    /// </summary>
    public class StellarisDefinitionMergerTests
    {
        /// <summary>
        /// Defines the test method Can_process_should_be_false.
        /// </summary>
        [Fact]
        public void Can_process_should_be_false()
        {
            var service = GetService();
            service.CanProcess("test").Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Can_process_should_be_true.
        /// </summary>
        [Fact]
        public void Can_process_should_be_true()
        {
            var service = GetService();
            service.CanProcess("Stellaris").Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Encoding_should_be_uft8_bom.
        /// </summary>
        [Fact]
        public void Encoding_should_be_uft8_bom()
        {
            var service = GetService();
            var result = service.GetEncoding(new List<IDefinition>()
            {
                new Definition()
                {
                    File = "localisation\\test.yml",
                    ValueType = Parser.Common.ValueType.SpecialVariable
                }
            });
            result.GetPreamble().Length.Should().Be(3);
        }

        /// <summary>
        /// Defines the test method Encoding_should_not_be_uft8_bom.
        /// </summary>
        [Fact]
        public void Encoding_should_not_be_uft8_bom()
        {
            var service = GetService();
            var result = service.GetEncoding(new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\test.txt",
                    ValueType = Parser.Common.ValueType.Object
                }
            });
            result.GetPreamble().Length.Should().Be(0);
        }

        /// <summary>
        /// Defines the test method EnsureAllSameType_should_throw_validation_exception.
        /// </summary>
        [Fact]
        public void EnsureAllSameType_should_throw_validation_exception()
        {
            var service = GetService();
            Exception ex = null;
            try
            {
                var result = service.GetFileName(new List<IDefinition>()
                {
                    new Definition()
                    {
                        File = "events\\test.txt",
                        ValueType = Parser.Common.ValueType.Object
                    },
                    new Definition()
                    {
                        File = "events\\test.txt",
                        ValueType = Parser.Common.ValueType.Object
                    }
                });
            }
            catch (Exception e)
            {
                ex = e;
            }
            ex.GetType().Should().Be(typeof(ArgumentException));
        }

        /// <summary>
        /// Defines the test method GetFilename_should_use_fios_rule.
        /// </summary>
        [Fact]
        public void GetFilename_should_use_fios_rule()
        {
            var service = GetService();
            var result = service.GetFileName(new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\test.txt",
                    ValueType = Parser.Common.ValueType.Object,
                    Id = "t"
                }
            });
            result.Should().Be("events\\!!!_t.txt");
        }

        /// <summary>
        /// Defines the test method GetFilename_should_use_lios_rule.
        /// </summary>
        [Fact]
        public void GetFilename_should_use_lios_rule()
        {
            var service = GetService();
            var result = service.GetFileName(new List<IDefinition>()
            {
                new Definition()
                {
                    File = "common\\agendas\\test.txt",
                    ValueType = Parser.Common.ValueType.Object,
                    Id = "t"
                }
            });
            result.Should().Be("common\\agendas\\zzz_t.txt");
        }

        /// <summary>
        /// Defines the test method GetFilename_localization_should_append_replace_path.
        /// </summary>
        [Fact]
        public void GetFilename_localization_should_append_replace_path()
        {
            var service = GetService();
            var result = service.GetFileName(new List<IDefinition>()
            {
                new Definition()
                {
                    File = "localisation\\test.yml",
                    ValueType = Parser.Common.ValueType.SpecialVariable,
                    Id = "t"
                }
            });
            result.Should().Be("localisation\\replace\\t.yml");
        }

        /// <summary>
        /// Defines the test method GetFilename_localization_should_not_append_replace_path.
        /// </summary>
        [Fact]
        public void GetFilename_localization_should_not_append_replace_path()
        {
            var service = GetService();
            var result = service.GetFileName(new List<IDefinition>()
            {
                new Definition()
                {
                    File = "localisation\\replace\\test.yml",
                    ValueType = Parser.Common.ValueType.SpecialVariable,
                    Id = "t"
                }
            });
            result.Should().Be("localisation\\replace\\t.yml");
        }

        /// <summary>
        /// Defines the test method MergeContent_can_merge_all_variables_and_namespaces_into_one.
        /// </summary>
        [Fact]
        public void MergeContent_can_merge_all_variables_and_namespaces_into_one()
        {
            var service = GetService();
            var result = service.MergeContent(new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\t.txt",
                    ValueType = Parser.Common.ValueType.Namespace,
                    Code = "namespace = test",
                    Id = "t"
                },
                new Definition()
                {
                    File = "events\\t.txt",
                    ValueType = Parser.Common.ValueType.Variable,
                    Code = "t = test",
                    Id = "t2"
                },
                new Definition()
                {
                    File = "events\\t.txt",
                    ValueType = Parser.Common.ValueType.Object,
                    Code = "o = {}",
                    Id = "t3"
                }
            });
            var sb = new StringBuilder();
            sb.AppendLine("namespace = test");
            sb.AppendLine("t = test");
            sb.AppendLine("o = {}");
            result.Should().Be(sb.ToString());
        }



        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <returns>StellarisDefinitionMerger.</returns>
        private StellarisDefinitionMerger GetService()
        {
            return new StellarisDefinitionMerger();
        }
    }
}
