// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 01-31-2022
//
// Last Modified By : Mario
// Last Modified On : 02-02-2022
// ***********************************************************************
// <copyright file="DefinitionResetConverterTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using DiffPlex.DiffBuilder.Model;
using AwesomeAssertions;
using IronyModManager.Converters;
using IronyModManager.Parser.Definitions;
using Xunit;
namespace IronyModManager.Tests
{
    /// <summary>
    /// Class DefinitionResetConverterTests.
    /// </summary>
    public class DefinitionResetConverterTests
    {
        /// <summary>
        /// Defines the test method Class_should_be_unchanged.
        /// </summary>
        [Fact]
        public void Class_should_be_reset_resolved_conflict()
        {
            var converter = new DefinitionResetConverter();
            var result = converter.Convert(new HierarchicalDefinitions() { ResetType = Shared.Models.ResetType.Resolved }, null, null, null);
            result.Should().Be("ResolvedResetMod");
        }

        /// <summary>
        /// Defines the test method Class_should_be_reset_ignored_conflict.
        /// </summary>
        [Fact]
        public void Class_should_be_reset_ignored_conflict()
        {
            var converter = new DefinitionResetConverter();
            var result = converter.Convert(new HierarchicalDefinitions() { ResetType = Shared.Models.ResetType.Ignored }, null, null, null);
            result.Should().Be("IgnoredResetMod");
        }

        /// <summary>
        /// Defines the test method Class_should_be_empty.
        /// </summary>
        [Fact]
        public void Class_should_be_empty()
        {
            var converter = new DefinitionResetConverter();
            var result = converter.Convert(new HierarchicalDefinitions(), null, null, null);
            result.Should().Be(string.Empty);

            result = converter.Convert(new HierarchicalDefinitions() {ResetType = Shared.Models.ResetType.Any } , null, null, null);
            result.Should().Be(string.Empty);

            result = converter.Convert(null, null, null, null);
            result.Should().Be(string.Empty);
        }
    }
}
