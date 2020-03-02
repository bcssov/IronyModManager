// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 03-01-2020
//
// Last Modified By : Mario
// Last Modified On : 03-01-2020
// ***********************************************************************
// <copyright file="MathConverterTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using IronyModManager.Converters;
using IronyModManager.Localization;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Tests
{
    /// <summary>
    /// Class MathConverterTests.
    /// </summary>
    public class MathConverterTests
    {
        /// <summary>
        /// Defines the test method Multiplication_should_return_result.
        /// </summary>
        [Fact]
        public void Multiplication_should_return_result()
        {
            CurrentLocale.SetCurrent("en");
            var converter = new MathConverter();
            var result = converter.Convert(200, null, "x*2", null);
            result.Should().Be(400);
        }

        /// <summary>
        /// Defines the test method Division_should_return_result.
        /// </summary>
        [Fact]
        public void Division_should_return_result()
        {
            CurrentLocale.SetCurrent("en");
            var converter = new MathConverter();
            var result = converter.Convert(200, null, "x/2", null);
            result.Should().Be(100);
        }

        /// <summary>
        /// Defines the test method Increment_should_return_result.
        /// </summary>
        [Fact]
        public void Increment_should_return_result()
        {
            CurrentLocale.SetCurrent("en");
            var converter = new MathConverter();
            var result = converter.Convert(200, null, "x+2", null);
            result.Should().Be(202);
        }

        /// <summary>
        /// Defines the test method Decrement_should_return_result.
        /// </summary>
        [Fact]
        public void Decrement_should_return_result()
        {
            CurrentLocale.SetCurrent("en");
            var converter = new MathConverter();
            var result = converter.Convert(200, null, "x-2", null);
            result.Should().Be(198);
        }
    }
}
