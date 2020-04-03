// ***********************************************************************
// Assembly         : IronyModManager.IO.Tests
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 04-01-2020
// ***********************************************************************
// <copyright file="ReaderTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using IronyModManager.DI;
using IronyModManager.IO.Common;
using IronyModManager.IO.Common.Readers;
using IronyModManager.IO.Readers;
using IronyModManager.Tests.Common;
using Xunit;

namespace IronyModManager.IO.Tests
{
    /// <summary>
    /// Class ReaderTests.
    /// </summary>
    public class ReaderTests
    {
        /// <summary>
        /// The test path
        /// </summary>
        private const string TestPath = "..\\..\\..\\..\\..\\..\\ai_species_limit";
        /// <summary>
        /// The archive test path
        /// </summary>
        private const string ArchiveTestPath = "..\\..\\..\\..\\..\\..\\ai_species_limit.zip";

        /// <summary>
        /// Selectses the correct reader.
        /// </summary>

        [Fact]
        public void Selects_correct_reader()
        {
            var reader = new Reader(new List<IFileReader>() { new Reader1(), new Reader2() });
            var result = reader.Read("fake1");
            result.Count().Should().Be(1);
            result.First().FileName.Should().Be("fake1_result");
            result = reader.Read("fake2");
            result.Count().Should().Be(1);
            result.First().FileName.Should().Be("fake2_result");
        }

        /// <summary>
        /// Defines the test method Disk_read_test.
        /// </summary>
#if FUNCTIONAL_TEST
        [Fact]
#else
        [Fact(Skip = "This is for functional testing only")]
#endif
        public void Disk_read_test()
        {
            DISetup.SetupContainer();

            var reader = DIResolver.Get<IReader>();
            var result = reader.Read(TestPath);
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().NotBe(0);
        }

        /// <summary>
        /// Defines the test method Disk_read_test.
        /// </summary>
#if FUNCTIONAL_TEST
        [Fact]
#else
        [Fact(Skip = "This is for functional testing only")]
#endif
        public void Archive_read_test()
        {
            DISetup.SetupContainer();

            var reader = DIResolver.Get<IReader>();
            var result = reader.Read(ArchiveTestPath);
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().NotBe(0);
        }

#if FUNCTIONAL_TEST
        [Fact]
#else
        /// <summary>
        /// Defines the test method Mod_file_read_test.
        /// </summary>
        [Fact(Skip = "This is for functional testing only")]
#endif
        public void Mod_file_read_test()
        {
            DISetup.SetupContainer();

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $"Documents{Path.DirectorySeparatorChar}Paradox Interactive{Path.DirectorySeparatorChar}Stellaris{Path.DirectorySeparatorChar}mod");
            var reader = DIResolver.Get<IReader>();
            var result = reader.Read(path);
            result.Should().NotBeNullOrEmpty();
            result.Count().Should().NotBe(0);
        }

        /// <summary>
        /// Class Reader1.
        /// Implements the <see cref="IronyModManager.IO.IFileReader" />
        /// Implements the <see cref="IronyModManager.IO.Common.Readers.IFileReader" />
        /// </summary>
        /// <seealso cref="IronyModManager.IO.Common.Readers.IFileReader" />
        /// <seealso cref="IronyModManager.IO.IFileReader" />
        private class Reader1 : IFileReader
        {
            /// <summary>
            /// Determines whether this instance can read the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns><c>true</c> if this instance can read the specified path; otherwise, <c>false</c>.</returns>
            public bool CanRead(string path)
            {
                return path.Equals("fake1");
            }

            /// <summary>
            /// Reads the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns>IReadOnlyCollection&lt;IFileInfo&gt;.</returns>
            public IReadOnlyCollection<IFileInfo> Read(string path)
            {
                return new List<IFileInfo>() { new FileInfo()
                {
                    FileName = "fake1_result"
                } };
            }
        }

        /// <summary>
        /// Class Reader2.
        /// Implements the <see cref="IronyModManager.IO.IFileReader" />
        /// Implements the <see cref="IronyModManager.IO.Common.Readers.IFileReader" />
        /// </summary>
        /// <seealso cref="IronyModManager.IO.Common.Readers.IFileReader" />
        /// <seealso cref="IronyModManager.IO.IFileReader" />
        private class Reader2 : IFileReader
        {
            /// <summary>
            /// Determines whether this instance can read the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns><c>true</c> if this instance can read the specified path; otherwise, <c>false</c>.</returns>
            public bool CanRead(string path)
            {
                return path.Equals("fake2");
            }

            /// <summary>
            /// Reads the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns>IReadOnlyCollection&lt;IFileInfo&gt;.</returns>
            public IReadOnlyCollection<IFileInfo> Read(string path)
            {
                return new List<IFileInfo>() { new FileInfo()
                {
                    FileName = "fake2_result"
                } };
            }
        }
    }
}
