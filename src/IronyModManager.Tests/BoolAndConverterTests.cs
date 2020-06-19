// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 06-15-2020
//
// Last Modified By : Mario
// Last Modified On : 06-15-2020
// ***********************************************************************
// <copyright file="BoolAndConverterTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using IronyModManager.Converters;
using Xunit;

namespace IronyModManager.Tests
{
    /// <summary>
    /// Class BoolAndConverterTests.
    /// </summary>
    public class BoolAndConverterTests
    {
        /// <summary>
        /// Defines the test method Should_be_false.
        /// </summary>
        [Fact]
        public void Should_be_false()
        {
            var converter = new BoolAndConverter();
            var result = converter.Convert(new List<object>() { true, false }, null, null, null);
            Convert.ToBoolean(result).Should().BeFalse();
        }

        /// <summary>
        /// Shoulds the be true.
        /// </summary>
        [Fact] 
        public void Should_be_true()
        {
            var converter = new BoolAndConverter();
            var result = converter.Convert(new List<object>() { true, true, true }, null, null, null);
            Convert.ToBoolean(result).Should().BeTrue();
        }
    }
}
