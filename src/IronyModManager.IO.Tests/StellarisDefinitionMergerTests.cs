// ***********************************************************************
// Assembly         : IronyModManager.IO.Tests
// Author           : Mario
// Created          : 04-05-2020
//
// Last Modified By : Mario
// Last Modified On : 04-17-2020
// ***********************************************************************
// <copyright file="StellarisDefinitionMergerTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using IronyModManager.IO.Mods.InfoProviders;
using IronyModManager.Shared.Models;
using IronyModManager.Parser.Definitions;
using Xunit;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.IO.Tests
{
    /// <summary>
    /// Class StellarisDefinitionInfoProviderTests.
    /// </summary>
    public class StellarisDefinitionInfoProviderTests
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
            var result = service.GetEncoding(new Definition()
            {
                File = "localisation\\test.yml",
                ValueType = ValueType.SpecialVariable
            });
            result.GetPreamble().Length.Should().Be(3);
        }

        /// <summary>
        /// Defines the test method Encoding_name_lists_should_be_uft8_bom.
        /// </summary>
        [Fact]
        public void Encoding_name_lists_should_be_uft8_bom()
        {
            var service = GetService();
            var result = service.GetEncoding(new Definition()
            {
                File = "common\\name_lists\\test.txt",
                ValueType = ValueType.SpecialVariable
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
            var result = service.GetEncoding(new Definition()
            {
                File = "events\\test.txt",
                ValueType = ValueType.Object
            });
            result.GetPreamble().Length.Should().Be(0);
        }

        /// <summary>
        /// Defines the test method EnsureAllSameType_should_throw_validation_exception.
        /// </summary>
        [Fact]
        public void EnsureValidType_should_throw_validation_exception()
        {
            var service = GetService();
            Exception ex = null;
            try
            {
                var result = service.GetFileName(new Definition()
                {
                    File = "events\\test.txt",
                    ValueType = ValueType.Namespace
                });
            }
            catch (Exception e)
            {
                ex = e;
            }
            ex.GetType().Should().Be(typeof(ArgumentException));

            ex = null;
            try
            {
                var result = service.GetFileName(new Definition()
                {
                    File = "events\\test.txt",
                    ValueType = ValueType.Variable
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
            var result = service.GetFileName(new Definition()
            {
                File = "events\\test.txt",
                ValueType = ValueType.Object,
                Id = "t"
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
            var result = service.GetFileName(new Definition()
            {
                File = "common\\agendas\\test.txt",
                ValueType = ValueType.Object,
                Id = "t"
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
            var result = service.GetFileName(new Definition()
            {
                File = "localisation\\test.yml",
                ValueType = ValueType.SpecialVariable,
                Id = "t"
            });
            result.Should().Be("localisation\\replace\\zzz_t.yml");
        }

        /// <summary>
        /// Defines the test method GetFilename_localization_should_not_append_replace_path.
        /// </summary>
        [Fact]
        public void GetFilename_localization_should_not_append_replace_path()
        {
            var service = GetService();
            var result = service.GetFileName(new Definition()
            {
                File = "localisation\\replace\\test.yml",
                ValueType = ValueType.SpecialVariable,
                Id = "t"
            });
            result.Should().Be("localisation\\replace\\zzz_t.yml");
        }


        /// <summary>
        /// Defines the test method GetFilename_FIOS_Prefixed_should_append_additional_prefixes.
        /// </summary>
        [Fact]
        public void GetFilename_FIOS_Prefixed_should_append_additional_prefixes()
        {
            var service = GetService();
            var result = service.GetFileName(new Definition()
            {
                File = "common\\ship_behaviors\\!!!_a.txt",
                ValueType = ValueType.SpecialVariable,
                Id = "t"
            });
            result.Should().Be("common\\ship_behaviors\\!!!!_t.txt");
        }

        [Fact]
        public void GetFilename_LIOS_Prefixed_should_append_additional_prefixes()
        {
            var service = GetService();
            var result = service.GetFileName(new Definition()
            {
                File = "common\\anomalies\\zzz_z.txt",
                ValueType = ValueType.SpecialVariable,
                Id = "t"
            });
            result.Should().Be("common\\anomalies\\zzzz_t.txt");
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <returns>StellarisDefinitionInfoProvider.</returns>
        private static StellarisDefinitionInfoProvider GetService()
        {
            return new StellarisDefinitionInfoProvider();
        }
    }
}
