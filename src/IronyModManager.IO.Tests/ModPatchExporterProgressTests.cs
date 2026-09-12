// ***********************************************************************
// Assembly         : IronyModManager.IO.Tests
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwesomeAssertions;
using IronyModManager.IO.Common.MessageBus;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Mods.Models;
using IronyModManager.IO.Common.Readers;
using IronyModManager.IO.Mods;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;
using IronyModManager.Shared.MessageBus;
using Moq;
using Xunit;

namespace IronyModManager.IO.Tests
{
    /// <summary>
    /// Tests patch mod rename progress and filesystem semantics.
    /// </summary>
    public sealed class ModPatchExporterProgressTests : IDisposable
    {
        private readonly string testDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModPatchExporterProgressTests"/> class.
        /// </summary>
        public ModPatchExporterProgressTests()
        {
            testDirectory = Path.Combine(Path.GetTempPath(), $"IronyPatchRenameTests-{Guid.NewGuid():N}");
            Directory.CreateDirectory(testDirectory);
        }

        /// <summary>
        /// A successful rename reports completed files monotonically and finishes at 100 percent.
        /// </summary>
        [Fact]
        public async Task Should_report_truthful_monotonic_progress_and_preserve_rename_semantics()
        {
            var source = Path.Combine(testDirectory, "old");
            Directory.CreateDirectory(Path.Combine(source, "nested"));
            File.WriteAllText(Path.Combine(source, "first.txt"), "first");
            File.WriteAllText(Path.Combine(source, "nested", "second.txt"), "second");
            File.WriteAllText(Path.Combine(source, "state.json"), "\"old\"");
            var progress = new List<double>();
            var exporter = CreateExporter(progress);

            var result = await exporter.RenamePatchModAsync(CreateParameters("old", "new"));

            result.Should().BeTrue();
            progress.Should().Equal(33.33, 66.67, 100);
            progress.Should().BeInAscendingOrder();
            Directory.Exists(source).Should().BeFalse();
            File.ReadAllText(Path.Combine(testDirectory, "new", "first.txt")).Should().Be("first");
            File.ReadAllText(Path.Combine(testDirectory, "new", "nested", "second.txt")).Should().Be("second");
            (await ReadCompressedStateAsync(Path.Combine(testDirectory, "new", "state.irony"))).Should().Be("\"new\"");
            File.Exists(Path.Combine(testDirectory, "new", "state.json")).Should().BeFalse();
        }

        /// <summary>
        /// A failed rename never reports successful completion or deletes its source.
        /// </summary>
        [Fact]
        public async Task Should_not_report_completion_or_delete_source_when_copy_fails()
        {
            var source = Path.Combine(testDirectory, "same");
            Directory.CreateDirectory(source);
            File.WriteAllText(Path.Combine(source, "state.irony"), "state");
            var progress = new List<double>();
            var exporter = CreateExporter(progress);

            await Assert.ThrowsAsync<AggregateException>(() => exporter.RenamePatchModAsync(CreateParameters("same", "same")));

            progress.Should().NotContain(100);
            Directory.Exists(source).Should().BeTrue();
        }

        private ModPatchExporter CreateExporter(List<double> progress)
        {
            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.Publish(It.IsAny<PatchModRenameProgressEvent>()))
                .Callback<PatchModRenameProgressEvent>(e => progress.Add(e.Percentage));
            return new ModPatchExporter(Mock.Of<IObjectClone>(), Mock.Of<ICache>(), Mock.Of<IReader>(), Array.Empty<IDefinitionInfoProvider>(), messageBus.Object);
        }

        private ModPatchExporterParameters CreateParameters(string source, string destination)
        {
            return new ModPatchExporterParameters
            {
                RootPath = testDirectory,
                ModPath = source,
                PatchPath = destination,
                RenamePairs = [new KeyValuePair<string, string>(source, destination)]
            };
        }

        private static async Task<string> ReadCompressedStateAsync(string path)
        {
            await using var source = File.OpenRead(path);
            await using var gzip = new GZipStream(source, CompressionMode.Decompress);
            using var destination = new MemoryStream();
            await gzip.CopyToAsync(destination);
            return Encoding.UTF8.GetString(destination.ToArray());
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }
        }
    }
}
