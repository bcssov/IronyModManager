// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 06-14-2020
//
// Last Modified By : Mario
// Last Modified On : 06-14-2020
// ***********************************************************************
// <copyright file="DefinitionConverterTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using IronyModManager.Converters;
using IronyModManager.Parser.Definitions;
using Xunit;

namespace IronyModManager.Tests
{
    /// <summary>
    /// Class DefinitionConverterTests.
    /// </summary>
    public class DefinitionConverterTests
    {
        /// <summary>
        /// Defines the test method Definition_text_should_be_empty.
        /// </summary>
        [Fact]
        public void Definition_text_should_be_empty()
        {
            var converter = new DefinitionConverter();
            var result = converter.Convert(null, null, null, null);
            result.ToString().Should().BeNullOrWhiteSpace();
        }


        /// <summary>
        /// Defines the test method Definition_text_should_not_be_empty.
        /// </summary>
        [Fact]
        public void Definition_text_should_not_be_empty()
        {
            var converter = new DefinitionConverter();
            var result = converter.Convert(new Definition() { Id = "test", File = "test.txt", ModName = "test" }, null, null, null);
            result.ToString().Should().NotBeNullOrWhiteSpace();
        }
    }
}
