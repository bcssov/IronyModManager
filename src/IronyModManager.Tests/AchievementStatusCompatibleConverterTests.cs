// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 06-16-2020
//
// Last Modified By : Mario
// Last Modified On : 06-16-2020
// ***********************************************************************
// <copyright file="AchievementStatusCompatibleConverterTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using AwesomeAssertions;
using IronyModManager.Converters;
using IronyModManager.Models.Common;
using Xunit;

namespace IronyModManager.Tests
{
    /// <summary>
    /// Class AchievementStatusCompatibleConverterTests.
    /// </summary>
    public class AchievementStatusCompatibleConverterTests
    {
        /// <summary>
        /// Defines the test method Should_be_false.
        /// </summary>
        [Fact]
        public void Should_be_false()
        {
            var converter = new AchievementStatusCompatibleConverter();
            var result = converter.Convert(AchievementStatus.NotEvaluated, null, null, null);
            Convert.ToBoolean(result).Should().BeFalse();
        }

        /// <summary>
        /// Shoulds the be true.
        /// </summary>
        [Fact]
        public void Should_be_true()
        {
            var converter = new AchievementStatusNotCompatibleConverter();
            var result = converter.Convert(AchievementStatus.NotCompatible, null, null, null);
            Convert.ToBoolean(result).Should().BeTrue();
        }
    }
}
