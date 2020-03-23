// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 03-21-2020
//
// Last Modified By : Mario
// Last Modified On : 03-21-2020
// ***********************************************************************
// <copyright file="DiffLineConverterTests.cs" company="Mario">
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
using Xunit;

namespace IronyModManager.Tests
{
    /// <summary>
    /// Class DiffLineConverterTests.
    /// </summary>
    public class DiffLineConverterTests
    {
        /// <summary>
        /// Defines the test method Class_should_be_deleted.
        /// </summary>
        [Fact]
        public void Class_should_be_deleted()
        {
            var converter = new DiffLineConverter();
            var result = converter.Convert(new DiffPiece(string.Empty, ChangeType.Deleted), null, null, null);
            result.Should().Be("DiffDeletedLine");
        }

        /// <summary>
        /// Defines the test method Class_should_be_imaginary.
        /// </summary>
        [Fact]
        public void Class_should_be_imaginary()
        {
            var converter = new DiffLineConverter();
            var result = converter.Convert(new DiffPiece(string.Empty, ChangeType.Imaginary), null, null, null);
            result.Should().Be("DiffImaginaryLine");
        }

        /// <summary>
        /// Defines the test method Class_should_be_inserted.
        /// </summary>
        [Fact]
        public void Class_should_be_inserted()
        {
            var converter = new DiffLineConverter();
            var result = converter.Convert(new DiffPiece(string.Empty, ChangeType.Inserted), null, null, null);
            result.Should().Be("DiffInsertedLine");
        }

        /// <summary>
        /// Defines the test method Class_should_be_empty.
        /// </summary>
        [Fact]
        public void Class_should_be_empty()
        {
            var converter = new DiffLineConverter();
            var result = converter.Convert(new DiffPiece(string.Empty, ChangeType.Modified), null, null, null);
            result.Should().Be(string.Empty);

            result = converter.Convert(null, null, null, null);
            result.Should().Be(string.Empty);
        }
    }
}
