// ***********************************************************************
// Assembly         : IronyModManager.IO.Tests
// Author           : Mario
// Created          : 09-09-2026
//
// Last Modified By : Mario
// Last Modified On : 09-09-2026
// ***********************************************************************
// <copyright file="ArchiveCompatibilityTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AwesomeAssertions;
using Ionic.Zip;
using IronyModManager.IO.Common;
using IronyModManager.IO.Common.MessageBus;
using IronyModManager.IO.Common.Models;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Mods;
using IronyModManager.IO.Readers;
using IronyModManager.IO.Updater;
using IronyModManager.Shared;
using IronyModManager.Shared.MessageBus;
using Moq;
using Xunit;

namespace IronyModManager.IO.Tests
{
    /// <summary>
    /// Characterizes Irony's supported ZIP behavior independently of the archive implementation.
    /// </summary>
    public sealed class ArchiveCompatibilityTests : IDisposable
    {
        private readonly string testDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveCompatibilityTests" /> class.
        /// </summary>
        public ArchiveCompatibilityTests()
        {
            testDirectory = Path.Combine(Path.GetTempPath(), $"IronyArchiveTests-{Guid.NewGuid():N}");
            Directory.CreateDirectory(testDirectory);
        }

        /// <summary>
        /// Verifies that both supported filename conventions route to the ZIP reader.
        /// </summary>
        [Fact]
        public void Should_recognize_zip_and_legacy_bin_filenames_only()
        {
            var zipPath = CreateArchive("archive.zip", zip => zip.AddEntry("folder/file.txt", "content"));
            var binPath = Path.Combine(testDirectory, "archive.bin");
            var sevenZipPath = Path.Combine(testDirectory, "archive.7z");
            File.Copy(zipPath, binPath);
            File.Copy(zipPath, sevenZipPath);
            var reader = new ArchiveFileReader(Mock.Of<ILogger>());

            reader.CanRead(zipPath).Should().BeTrue();
            reader.CanRead(binPath).Should().BeTrue();
            reader.CanRead(sevenZipPath).Should().BeFalse();
            reader.GetFiles(binPath).Should().ContainSingle()
                .Which.Should().Be("folder/file.txt".StandardizeDirectorySeparator());

            var (stream, _, _, _) = reader.GetStream(binPath, "folder/file.txt");
            using (stream)
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                streamReader.ReadToEnd().Should().Be("content");
            }
        }

        /// <summary>
        /// Verifies entry enumeration, wildcard lookup, Unicode names, directories, and returned stream ownership.
        /// </summary>
        [Fact]
        public void Should_enumerate_and_read_supported_zip_entries()
        {
            const string unicodePath = "common/naïve/日本語.txt";
            var archivePath = CreateArchive("entries.zip", zip =>
            {
                zip.AddDirectoryByName("common/");
                zip.AddEntry(unicodePath, "unicode-content");
                var bzipEntry = zip.AddEntry("common/data/descriptor.mod", "descriptor-content");
                bzipEntry.CompressionMethod = CompressionMethod.BZip2;
            });
            var reader = new ArchiveFileReader(Mock.Of<ILogger>());

            reader.GetFiles(archivePath).Should().BeEquivalentTo(
                unicodePath.StandardizeDirectorySeparator(),
                "common/data/descriptor.mod".StandardizeDirectorySeparator());

            var (stream, isReadOnly, modified, _) = reader.GetStream(archivePath, "*descriptor.mod");
            using (stream)
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                streamReader.ReadToEnd().Should().Be("descriptor-content");
            }

            isReadOnly.Should().BeFalse();
            modified.Should().BeCloseTo(File.GetLastWriteTime(archivePath), TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Preserves the historical size calculation of the primary archive path.
        /// </summary>
        [Fact]
        public void Should_preserve_historical_archive_size_calculation()
        {
            var archivePath = CreateArchive("sizes.zip", zip =>
            {
                zip.AddEntry("common/a.txt", "abc");
                zip.AddEntry("common/b.bin", "1234");
            });
            var reader = new ArchiveFileReader(Mock.Of<ILogger>());

            reader.GetTotalSize(archivePath).Should().Be(14);
            reader.GetTotalSize(archivePath, [".txt"]).Should().Be(10);
        }

        /// <summary>
        /// Verifies that Zip64 archives remain readable.
        /// </summary>
        [Fact]
        public void Should_read_zip64_archives()
        {
            var archivePath = CreateArchive("zip64.zip", zip =>
            {
                zip.UseZip64WhenSaving = Zip64Option.Always;
                zip.AddEntry("common/file.txt", "zip64-content");
            });
            var reader = new ArchiveFileReader(Mock.Of<ILogger>());

            var (stream, _, _, _) = reader.GetStream(archivePath, "common/file.txt");
            using (stream)
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                streamReader.ReadToEnd().Should().Be("zip64-content");
            }
        }

        /// <summary>
        /// Preserves the historical reader behavior for an entry whose stored payload no longer matches its CRC.
        /// </summary>
        [Fact]
        public void Should_read_entries_with_invalid_crc()
        {
            const string content = "unique-uncompressed-payload";
            var archivePath = CreateArchive("bad-crc.zip", zip =>
            {
                var entry = zip.AddEntry("common/file.txt", content);
                entry.CompressionMethod = CompressionMethod.None;
            });
            CorruptStoredPayload(archivePath, content);
            var reader = new ArchiveFileReader(Mock.Of<ILogger>());

            var (stream, _, _, _) = reader.GetStream(archivePath, "common/file.txt");
            using (stream)
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                streamReader.ReadToEnd().Should().NotBe(content);
            }
        }

        /// <summary>
        /// Preserves the historical empty-result behavior for ZIPs with a truncated end record.
        /// </summary>
        [Fact]
        public void Should_enumerate_archives_with_truncated_end_record()
        {
            var archivePath = CreateArchive("truncated.zip", zip => zip.AddEntry("common/file.txt", "content"));
            var bytes = File.ReadAllBytes(archivePath);
            File.WriteAllBytes(archivePath, bytes[..^8]);
            var reader = new ArchiveFileReader(Mock.Of<ILogger>());

            reader.GetFiles(archivePath).Should().BeEmpty();
        }

        /// <summary>
        /// Verifies update extraction semantics and progress reporting.
        /// </summary>
        [Fact]
        public async Task Should_unpack_zip_updates_with_progress_and_timestamps()
        {
            var timestamp = new DateTime(2020, 2, 3, 4, 5, 6, DateTimeKind.Local);
            var archivePath = CreateArchive("update.zip", zip =>
            {
                zip.AddDirectoryByName("nested/");
                var first = zip.AddEntry("nested/first.txt", "first");
                first.LastModified = timestamp;
                zip.AddEntry("second.txt", "second");
            });
            var extractionPath = Path.Combine(testDirectory, "update");
            Directory.CreateDirectory(extractionPath);
            File.WriteAllText(Path.Combine(extractionPath, "stale.txt"), "stale");
            var progress = new List<int>();
            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<UpdateUnpackProgressEvent>()))
                .Callback<UpdateUnpackProgressEvent>(e => progress.Add(e.Progress))
                .Returns(Task.CompletedTask);
            var unpacker = new Unpacker(messageBus.Object);

            var result = await unpacker.UnpackUpdateAsync(archivePath);

            result.Should().Be(extractionPath);
            File.Exists(Path.Combine(extractionPath, "stale.txt")).Should().BeFalse();
            File.ReadAllText(Path.Combine(extractionPath, "nested", "first.txt")).Should().Be("first");
            File.GetLastWriteTime(Path.Combine(extractionPath, "nested", "first.txt"))
                .Should().BeCloseTo(timestamp, TimeSpan.FromSeconds(2));
            progress.Should().Equal(50, 100);
        }

        /// <summary>
        /// Verifies that update entries cannot escape their extraction root.
        /// </summary>
        [Fact]
        public async Task Should_reject_update_path_traversal()
        {
            var outsidePath = Path.Combine(testDirectory, "outside.txt");
            var archivePath = CreateArchive("unsafe-update.zip", zip => zip.AddEntry("../outside.txt", "unsafe"));
            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<UpdateUnpackProgressEvent>())).Returns(Task.CompletedTask);
            var unpacker = new Unpacker(messageBus.Object);

            Func<Task> action = () => unpacker.UnpackUpdateAsync(archivePath);

            await action.Should().ThrowAsync<InvalidOperationException>();
            File.Exists(outsidePath).Should().BeFalse();
        }

        /// <summary>
        /// Verifies whole-collection file extraction and its path-safety boundary.
        /// </summary>
        [Fact]
        public async Task Should_import_collection_files_with_unicode_paths_and_overwrite()
        {
            var archivePath = CreateArchive("collection.zip", zip =>
            {
                zip.AddEntry("nested/日本語.txt", "unicode-content");
                zip.AddEntry("collision.txt", "first");
                zip.AddEntry("nested/../collision.txt", "second");
            });
            var modDirectory = Path.Combine(testDirectory, "mod");
            Directory.CreateDirectory(modDirectory);
            File.WriteAllText(Path.Combine(modDirectory, "stale.txt"), "stale");
            var exporter = CreateCollectionExporter();

            var result = await exporter.ImportModDirectoryAsync(new ModCollectionExporterParams
            {
                File = archivePath,
                ModDirectory = modDirectory,
                ExportModDirectory = Path.Combine(testDirectory, "exported-mods")
            });

            result.Should().BeTrue();
            File.Exists(Path.Combine(modDirectory, "stale.txt")).Should().BeFalse();
            File.ReadAllText(Path.Combine(modDirectory, "nested", "日本語.txt")).Should().Be("unicode-content");
            File.ReadAllText(Path.Combine(modDirectory, "collision.txt")).Should().Be("second");
        }

        /// <summary>
        /// Verifies that collection entries cannot escape their extraction root.
        /// </summary>
        [Fact]
        public async Task Should_reject_collection_path_traversal()
        {
            var outsidePath = Path.Combine(testDirectory, "outside-collection.txt");
            var archivePath = CreateArchive("unsafe-collection.zip", zip => zip.AddEntry("../outside-collection.txt", "unsafe"));
            var exporter = CreateCollectionExporter();

            Func<Task> action = () => exporter.ImportModDirectoryAsync(new ModCollectionExporterParams
            {
                File = archivePath,
                ModDirectory = Path.Combine(testDirectory, "mod"),
                ExportModDirectory = Path.Combine(testDirectory, "exported-mods")
            });

            await action.Should().ThrowAsync<InvalidOperationException>();
            File.Exists(outsidePath).Should().BeFalse();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }
        }

        private string CreateArchive(string name, Action<ZipFile> configure)
        {
            var path = Path.Combine(testDirectory, name);
            using var zip = new ZipFile
            {
                AlternateEncoding = Encoding.UTF8,
                AlternateEncodingUsage = ZipOption.AsNecessary
            };
            configure(zip);
            zip.Save(path);
            return path;
        }

        private static ModCollectionExporter CreateCollectionExporter()
        {
            var messageBus = new Mock<IMessageBus>();
            return new ModCollectionExporter(Mock.Of<IModWriter>(), Mock.Of<ILogger>(), messageBus.Object, Mock.Of<IMapper>());
        }

        private static void CorruptStoredPayload(string path, string content)
        {
            var bytes = File.ReadAllBytes(path);
            var payload = Encoding.UTF8.GetBytes(content);
            var index = bytes.AsSpan().IndexOf(payload);
            index.Should().BeGreaterThanOrEqualTo(0);
            bytes[index] ^= 0xff;
            File.WriteAllBytes(path, bytes);
        }
    }
}
