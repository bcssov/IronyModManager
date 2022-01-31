// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 01-31-2022
//
// Last Modified By : Mario
// Last Modified On : 01-31-2022
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
using FluentAssertions;
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
        public void Class_should_be_reset_mod()
        {
            var converter = new DefinitionResetConverter();
            var result = converter.Convert(new HierarchicalDefinitions() { WillBeReset = true }, null, null, null);
            result.Should().Be("ResetMod");
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

            result = converter.Convert(null, null, null, null);
            result.Should().Be(string.Empty);
        }
    }
}
