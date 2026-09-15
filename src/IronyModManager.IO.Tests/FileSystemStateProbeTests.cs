// ***********************************************************************
// Assembly         : IronyModManager.IO.Tests
// ***********************************************************************

using System;
using System.IO;
using AwesomeAssertions;
using IronyModManager.IO.Common.FileSystem;
using IronyModManager.IO.FileSystem;
using Xunit;

namespace IronyModManager.IO.Tests
{
    /// <summary>
    /// Tests deterministic filesystem state classification.
    /// </summary>
    public class FileSystemStateProbeTests
    {
        private const int CloudFileProviderNotRunningHResult = unchecked((int)0x8007016A);

        [Fact]
        public void Existing_and_missing_directories_should_be_distinguished()
        {
            var probe = new FileSystemStateProbe();
            var existing = AppDomain.CurrentDomain.BaseDirectory;
            var missing = Path.Combine(existing, Guid.NewGuid().ToString("N"));

            probe.CheckDirectory(existing).State.Should().Be(FileSystemPathState.Available);
            probe.CheckDirectory(missing).State.Should().Be(FileSystemPathState.Missing);
        }
        [Fact]
        public void Access_failure_classifier_should_be_narrow_and_follow_wrapped_exceptions()
        {
            var probe = new FileSystemStateProbe();

            probe.IsFileSystemAccessFailure(new IOException()).Should().BeTrue();
            probe.IsFileSystemAccessFailure(new UnauthorizedAccessException()).Should().BeTrue();
            probe.IsFileSystemAccessFailure(new InvalidOperationException("outer", new IOException())).Should().BeTrue();
            probe.IsFileSystemAccessFailure(new InvalidOperationException()).Should().BeFalse();
        }

        [Fact]
        public void Historical_cloud_provider_read_failure_should_be_classified_without_message_matching()
        {
            var probe = new FileSystemStateProbe();
            var exception = new IOException("Locale-independent test fixture", CloudFileProviderNotRunningHResult);

            exception.HResult.Should().Be(CloudFileProviderNotRunningHResult);
            probe.IsFileSystemAccessFailure(exception).Should().BeTrue();
        }
    }
}
