// ***********************************************************************
// Assembly         : IronyModManager.IO.Tests
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AwesomeAssertions;
using IronyModManager.IO.Mods;
using Xunit;

namespace IronyModManager.IO.Tests
{
    /// <summary>
    /// Tests the Merge Compress archive overwrite probe.
    /// </summary>
    public class ModMergeCompressExporterTests
    {
        /// <summary>
        /// A missing output archive is available and is not created by the probe.
        /// </summary>
        [Fact]
        public void Should_allow_missing_archive_without_creating_it()
        {
            var exporter = new ModMergeCompressExporter();
            var path = GetTemporaryArchivePath();

            exporter.GetUnavailableArchivePaths([path]).Should().BeEmpty();
            File.Exists(path).Should().BeFalse();
        }

        /// <summary>
        /// A writable output archive is available and is not modified by the probe.
        /// </summary>
        [Fact]
        public void Should_allow_writable_archive_without_modifying_it()
        {
            var exporter = new ModMergeCompressExporter();
            var path = GetTemporaryArchivePath();
            var timestamp = new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            try
            {
                File.WriteAllText(path, "existing archive");
                File.SetLastWriteTimeUtc(path, timestamp);

                exporter.GetUnavailableArchivePaths([path]).Should().BeEmpty();
                File.ReadAllText(path).Should().Be("existing archive");
                File.GetLastWriteTimeUtc(path).Should().Be(timestamp);
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// A locked output archive is unavailable without preventing other paths from being checked.
        /// </summary>
        [Fact]
        public void Should_report_locked_archive_from_multiple_targets()
        {
            var exporter = new ModMergeCompressExporter();
            var lockedPath = GetTemporaryArchivePath();
            var availablePath = GetTemporaryArchivePath();
            try
            {
                File.WriteAllText(lockedPath, "locked archive");
                File.WriteAllText(availablePath, "available archive");
                using (var stream = new FileStream(lockedPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    exporter.GetUnavailableArchivePaths([availablePath, lockedPath])
                        .Should().BeEquivalentTo([lockedPath]);
                }
            }
            finally
            {
                File.Delete(lockedPath);
                File.Delete(availablePath);
            }
        }

        private string GetTemporaryArchivePath()
        {
            return Path.Combine(Path.GetTempPath(), $"irony-{Guid.NewGuid()}.zip");
        }
    }
}
