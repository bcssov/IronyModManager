// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 09-03-2021
//
// Last Modified By : Mario
// Last Modified On : 09-03-2021
// ***********************************************************************
// <copyright file="LocalModConverterTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using IronyModManager.Converters;
using IronyModManager.Models.Common;
using Xunit;

namespace IronyModManager.Tests
{
    /// <summary>
    /// Class LocalModConverterTests.
    /// </summary>
    public class LocalModConverterTests
    {
        /// <summary>
        /// Defines the test method Should_be_false.
        /// </summary>
        [Fact]
        public void Should_be_false()
        {
            var converter = new LocalModConverter();
            var result = converter.Convert(ModSource.Paradox, null, null, null);
            Convert.ToBoolean(result).Should().BeFalse();
        }

        /// <summary>
        /// Shoulds the be true.
        /// </summary>
        [Fact]
        public void Should_be_true()
        {
            var converter = new LocalModConverter();
            var result = converter.Convert(ModSource.Local, null, null, null);
            Convert.ToBoolean(result).Should().BeTrue();
        }
    }
}
