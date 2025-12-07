// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 03-20-2020
//
// Last Modified By : Mario
// Last Modified On : 03-20-2020
// ***********************************************************************
// <copyright file="CollectionNullOrEmptyConverterTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using AwesomeAssertions;
using IronyModManager.Converters;
using Xunit;

namespace IronyModManager.Tests
{
    /// <summary>
    /// Class CollectionNullOrEmptyConverterTests.
    /// </summary>
    public class CollectionNullOrEmptyConverterTests
    {
        /// <summary>
        /// Defines the test method Collection_should_be_null.
        /// </summary>
        [Fact]
        public void Collection_should_be_null()
        {
            var converter = new CollectionNullOrEmptyConverter();
            var result = converter.Convert(null, null, null, null);
            result.Should().Be(true);
        }

        /// <summary>
        /// Defines the test method Collection_should_not_be_null_inverted.
        /// </summary>
        [Fact]
        public void Collection_should_not_be_null_inverted()
        {
            var converter = new CollectionNullOrEmptyConverter();
            var result = converter.Convert(null, null, true, null);
            result.Should().Be(false);
        }

        /// <summary>
        /// Defines the test method Collection_should_be_empty.
        /// </summary>
        [Fact]
        public void Collection_should_be_empty()
        {
            var converter = new CollectionNullOrEmptyConverter();
            var result = converter.Convert(new List<string>(), null, null, null);
            result.Should().Be(true);
        }

        /// <summary>
        /// Defines the test method Collection_should_not_be_empty_inverted.
        /// </summary>
        [Fact]
        public void Collection_should_not_be_empty_inverted()
        {
            var converter = new CollectionNullOrEmptyConverter();
            var result = converter.Convert(new List<string>(), null, true, null);
            result.Should().Be(false);
        }

        /// <summary>
        /// Defines the test method Collection_should_not_be_null_or_empty.
        /// </summary>
        [Fact]
        public void Collection_should_not_be_null_or_empty()
        {
            var converter = new CollectionNullOrEmptyConverter();
            var result = converter.Convert(new List<string>() { "test" }, null, null, null);
            result.Should().Be(false);
        }

        /// <summary>
        /// Defines the test method Collection_should_be_null_or_empty_inverted.
        /// </summary>
        [Fact]
        public void Collection_should_be_null_or_empty_inverted()
        {
            var converter = new CollectionNullOrEmptyConverter();            
            var result = converter.Convert(new List<string>() { "test" }, null, true, null);
            result.Should().Be(true);
        }
    }
}
