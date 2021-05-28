// ***********************************************************************
// Assembly         : IronyModManager.IO.Tests
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 05-28-2021
// ***********************************************************************
// <copyright file="ReaderTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var reader = new Reader(new List<IFileReader>() { new Reader1(), new Reader2() }, new Logger());
            var result = reader.Read("fake1");
            result.Count().Should().Be(1);
            result.First().FileName.Should().Be("fake1_result");
            result = reader.Read("fake2");
            result.Count().Should().Be(1);
            result.First().FileName.Should().Be("fake2_result");
        }

#if FUNCTIONAL_TEST
        [Fact]
#else
        /// <summary>
        /// Defines the test method Fileinfo_read_test.
        /// </summary>
        [Fact(Skip = "This is for functional testing only")]
#endif
        public void Fileinfo_read_test()
        {
            DISetup.SetupContainer();

            var reader = DIResolver.Get<IReader>();
            var result = reader.GetFileInfo(TestPath, @"gfx\interface\buttons\asl_text_button.dds");
            result.Should().NotBeNull();
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

#if FUNCTIONAL_TEST
        [Fact]
#else
        /// <summary>
        /// Defines the test method Disk_stream_read_test.
        /// </summary>
        [Fact(Skip = "This is for functional testing only")]
#endif
        public void Disk_stream_read_test()
        {
            DISetup.SetupContainer();

            var reader = DIResolver.Get<IReader>();
            var result = reader.GetStream(TestPath, @"gfx\interface\buttons\asl_text_button.dds");
            result.Should().NotBeNull();
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
        /// Defines the test method Archive_stream_read_test.
        /// </summary>
        [Fact(Skip = "This is for functional testing only")]
#endif
        public void Archive_stream_read_test()
        {
            DISetup.SetupContainer();

            var reader = DIResolver.Get<IReader>();
            var result = reader.GetStream(ArchiveTestPath, @"gfx\interface\buttons\asl_text_button.dds");
            result.Should().NotBeNull();
        }

#if FUNCTIONAL_TEST
        [Fact]
#else
        /// <summary>
        /// Defines the test method Archive_return_file_list.
        /// </summary>
        [Fact(Skip = "This is for functional testing only")]
#endif
        public void Archive_return_file_list()
        {
            DISetup.SetupContainer();

            var reader = DIResolver.Get<IReader>();
            var result = reader.GetFiles(ArchiveTestPath);
            result.Count().Should().BeGreaterThan(0);
        }

#if FUNCTIONAL_TEST
        [Fact]
#else
        /// <summary>
        /// Defines the test method Disk_return_file_list.
        /// </summary>
        [Fact(Skip = "This is for functional testing only")]
#endif
        public void Disk_return_file_list()
        {
            DISetup.SetupContainer();

            var reader = DIResolver.Get<IReader>();
            var result = reader.GetFiles(TestPath);
            result.Count().Should().BeGreaterThan(0);
        }

#if FUNCTIONAL_TEST
        [Fact]
#else
        /// <summary>
        /// Defines the test method Disk_stream_read_test.
        /// </summary>
        [Fact(Skip = "This is for functional testing only")]
#endif
        public async Task Image_stream_test()
        {
            DISetup.SetupContainer();

            var reader = DIResolver.Get<IReader>();
            var result = await reader.GetImageStreamAsync(TestPath, @"gfx\interface\buttons\asl_text_button.dds");
            result.Should().NotBeNull();

            result = await reader.GetImageStreamAsync(TestPath, @"gfx\interface\buttons\asl_text_button.tga");
            result.Should().NotBeNull();

            result = await reader.GetImageStreamAsync(TestPath, @"gfx\interface\buttons\asl_text_button.png");
            result.Should().NotBeNull();
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
            /// Determines whether this instance [can list files] the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns><c>true</c> if this instance [can list files] the specified path; otherwise, <c>false</c>.</returns>
            public bool CanListFiles(string path)
            {
                return true;
            }

            /// <summary>
            /// Determines whether this instance can read the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="searchSubfolders">if set to <c>true</c> [search subfolders].</param>
            /// <returns><c>true</c> if this instance can read the specified path; otherwise, <c>false</c>.</returns>
            public bool CanRead(string path, bool searchSubfolders = true)
            {
                return path.Equals("fake1");
            }

            /// <summary>
            /// Determines whether this instance [can read stream] the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns><c>true</c> if this instance [can read stream] the specified path; otherwise, <c>false</c>.</returns>
            public bool CanReadStream(string path)
            {
                return true;
            }

            /// <summary>
            /// Gets the files.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns>IList&lt;System.String&gt;.</returns>
            public IEnumerable<string> GetFiles(string path)
            {
                return new List<string>() { "fake1.txt" };
            }

            /// <summary>
            /// Gets the size of the file.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns>System.Int64.</returns>
            public long GetTotalSize(string path)
            {
                return 0;
            }

            /// <summary>
            /// Gets the stream.
            /// </summary>
            /// <param name="rootPath">The root path.</param>
            /// <param name="file">The file.</param>
            /// <returns>Stream.</returns>
            public (Stream, bool) GetStream(string rootPath, string file)
            {
                var ms = new MemoryStream();
                var sw = new StreamWriter(ms);
                sw.Write("fake1");
                return (ms, false);
            }

            /// <summary>
            /// Reads the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="allowedPaths">The allowed paths.</param>
            /// <param name="searchSubFolders">if set to <c>true</c> [search sub folders].</param>
            /// <returns>IReadOnlyCollection&lt;IFileInfo&gt;.</returns>
            public IReadOnlyCollection<IFileInfo> Read(string path, IEnumerable<string> allowedPaths, bool searchSubFolders = true)
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
            /// Determines whether this instance [can list files] the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns><c>true</c> if this instance [can list files] the specified path; otherwise, <c>false</c>.</returns>
            public bool CanListFiles(string path)
            {
                return true;
            }

            /// <summary>
            /// Determines whether this instance can read the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="searchSubfolders">if set to <c>true</c> [search subfolders].</param>
            /// <returns><c>true</c> if this instance can read the specified path; otherwise, <c>false</c>.</returns>
            public bool CanRead(string path, bool searchSubfolders = true)
            {
                return path.Equals("fake2");
            }

            /// <summary>
            /// Determines whether this instance [can read stream] the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns><c>true</c> if this instance [can read stream] the specified path; otherwise, <c>false</c>.</returns>
            public bool CanReadStream(string path)
            {
                return true;
            }

            /// <summary>
            /// Gets the files.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns>IList&lt;System.String&gt;.</returns>
            public IEnumerable<string> GetFiles(string path)
            {
                return new List<string>() { "fake12.txt" };
            }

            /// <summary>
            /// Gets the size of the file.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns>System.Int64.</returns>
            public long GetTotalSize(string path)
            {
                return 0;
            }

            /// <summary>
            /// Gets the stream.
            /// </summary>
            /// <param name="rootPath">The root path.</param>
            /// <param name="file">The file.</param>
            /// <returns>Stream.</returns>
            public (Stream, bool) GetStream(string rootPath, string file)
            {
                var ms = new MemoryStream();
                var sw = new StreamWriter(ms);
                sw.Write("fake1");
                return (ms, false);
            }

            /// <summary>
            /// Reads the specified path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="allowedPaths">The allowed paths.</param>
            /// <param name="searchSubFolders">if set to <c>true</c> [search sub folders].</param>
            /// <returns>IReadOnlyCollection&lt;IFileInfo&gt;.</returns>
            public IReadOnlyCollection<IFileInfo> Read(string path, IEnumerable<string> allowedPaths, bool searchSubFolders = true)
            {
                return new List<IFileInfo>() { new FileInfo()
                {
                    FileName = "fake2_result"
                } };
            }
        }
    }
}
