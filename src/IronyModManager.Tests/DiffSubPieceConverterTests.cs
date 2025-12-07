// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 03-21-2020
//
// Last Modified By : Mario
// Last Modified On : 03-21-2020
// ***********************************************************************
// <copyright file="DiffSubPieceConverterTests.cs" company="Mario">
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
using Xunit;
namespace IronyModManager.Tests
{
    /// <summary>
    /// Class DiffSubPieceConverterTests.
    /// </summary>
    public class DiffSubPieceConverterTests
    {
        /// <summary>
        /// Defines the test method Class_should_be_unchanged.
        /// </summary>
        [Fact]
        public void Class_should_be_unchanged()
        {
            var converter = new DiffSubPieceConverter();
            var result = converter.Convert(new DiffPiece(string.Empty, ChangeType.Unchanged), null, null, null);
            result.Should().Be("DiffUnchangedPieces");
        }

        /// <summary>
        /// Defines the test method Class_should_be_inserted.
        /// </summary>
        [Fact]
        public void Class_should_be_inserted()
        {
            var converter = new DiffSubPieceConverter();
            var result = converter.Convert(new DiffPiece(string.Empty, ChangeType.Inserted), null, null, null);
            result.Should().Be("DiffInsertedPieces");
        }

        /// <summary>
        /// Defines the test method Class_should_be_deleted.
        /// </summary>
        [Fact]
        public void Class_should_be_deleted()
        {
            var converter = new DiffSubPieceConverter();
            var result = converter.Convert(new DiffPiece(string.Empty, ChangeType.Deleted), null, null, null);
            result.Should().Be("DiffDeletedPieces");
        }

        /// <summary>
        /// Defines the test method Class_should_be_modified.
        /// </summary>
        [Fact]
        public void Class_should_be_modified()
        {
            var converter = new DiffSubPieceConverter();
            var result = converter.Convert(new DiffPiece(string.Empty, ChangeType.Modified), null, null, null);
            result.Should().Be("DiffModifiedPieces");
        }

        /// <summary>
        /// Defines the test method Class_should_be_empty.
        /// </summary>
        [Fact]
        public void Class_should_be_empty()
        {
            var converter = new DiffSubPieceConverter();
            var result = converter.Convert(new DiffPiece(string.Empty, ChangeType.Imaginary), null, null, null);
            result.Should().Be(string.Empty);

            result = converter.Convert(null, null, null, null);
            result.Should().Be(string.Empty);
        }
    }
}
