// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 09-12-2026
//
// Last Modified By : Mario
// Last Modified On : 09-12-2026
// ***********************************************************************
// <copyright file="UpdaterSignatureTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AwesomeAssertions;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.AppState;
using IronyModManager.Implementation.MessageBus;
using IronyModManager.Implementation.Updater;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Tests.Common;
using Moq;
using NetSparkleUpdater;
using NetSparkleUpdater.Enums;
using NetSparkleUpdater.Interfaces;
using NetSparkleUpdater.SignatureVerifiers;
using Xunit;

namespace IronyModManager.Tests
{
    /// <summary>
    /// Tests Irony's software artifact signature policy.
    /// </summary>
    public class UpdaterSignatureTests
    {
        private const string MissingSignatureMessage = "Update signature is missing.";

        /// <summary>
        /// A missing signature fails the download through the existing updater error path.
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task Missing_signature_should_fail_download_without_stuck_state(string signature)
        {
            var (sut, updaterService, appAction, _) = CreateUpdater(signature);
            var errors = new List<Exception>();
            using var subscription = sut.Error.Subscribe(errors.Add);

            var result = await sut.DownloadUpdateAsync();

            result.Should().BeFalse();
            errors.Should().ContainSingle().Which.Message.Should().Be(MissingSignatureMessage);
            GetField<bool>(sut, "busy").Should().BeFalse();
            GetField<IronySparkleUpdater>(sut, "updater").UpdateDownloading.Should().BeFalse();
            updaterService.Verify(p => p.UnpackUpdateAsync(It.IsAny<string>()), Times.Never);
            appAction.Verify(p => p.RunAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// A missing signature fails installation before extraction or execution.
        /// </summary>
        [Fact]
        public async Task Missing_signature_should_fail_install_without_stuck_state()
        {
            var (sut, updaterService, appAction, shutDownState) = CreateUpdater(null);
            var errors = new List<Exception>();
            using var subscription = sut.Error.Subscribe(errors.Add);
            shutDownState.Setup(p => p.WaitUntilFreeAsync()).Returns(Task.CompletedTask);

            var result = await sut.InstallUpdateAsync();

            result.Should().BeFalse();
            errors.Should().ContainSingle().Which.Message.Should().Be(MissingSignatureMessage);
            GetField<bool>(sut, "busy").Should().BeFalse();
            GetField<IronySparkleUpdater>(sut, "updater").UpdateInstalling.Should().BeFalse();
            updaterService.Verify(p => p.UnpackUpdateAsync(It.IsAny<string>()), Times.Never);
            appAction.Verify(p => p.RunAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// A present signature reaches NetSparkle's downloader and remains its responsibility to verify.
        /// </summary>
        [Fact]
        public async Task Present_signature_should_reach_netsparkle_download_flow()
        {
            var updaterService = new Mock<IUpdaterService>();
            var appAction = new Mock<IAppAction>();
            var downloader = new Mock<IUpdateDownloader>();
            var downloadStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            downloader.Setup(p => p.StartFileDownload(It.IsAny<Uri>(), It.IsAny<string>())).Callback(() => downloadStarted.SetResult());
            var downloadDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            using var sut = new IronySparkleUpdater(false, updaterService.Object, appAction.Object)
            {
                CheckServerFileName = false,
                TmpDownloadFilePath = downloadDirectory,
                UpdateDownloader = downloader.Object
            };
            var item = CreateItem("present-signature");

            try
            {
                await sut.InitAndBeginDownload(item);
                await downloadStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));

                downloader.Verify(p => p.StartFileDownload(It.IsAny<Uri>(), It.IsAny<string>()), Times.Once);
                updaterService.Verify(p => p.UnpackUpdateAsync(It.IsAny<string>()), Times.Never);
                appAction.Verify(p => p.RunAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            }
            finally
            {
                if (Directory.Exists(downloadDirectory))
                {
                    Directory.Delete(downloadDirectory, true);
                }
            }
        }

        /// <summary>
        /// Unsigned AppCast XML remains valid when the software enclosure carries a signature.
        /// </summary>
        [Fact]
        public void Unsigned_appcast_with_signed_enclosure_should_remain_available()
        {
            const string signature = "present-signature";
            var os = OperatingSystem.IsWindows() ? "windows" : OperatingSystem.IsMacOS() ? "osx" : "linux";
            var fileName = OperatingSystem.IsWindows() ? "IronyModManager-portable.zip" : "IronyModManager.zip";
            var xml = $"""
                <?xml version="1.0" encoding="utf-8"?>
                <rss version="2.0" xmlns:sparkle="http://www.andymatuschak.org/xml-namespaces/sparkle">
                  <channel>
                    <title>Irony Mod Manager</title>
                    <item>
                      <title>999.0.0</title>
                      <enclosure url="https://example.test/{fileName}" sparkle:version="999.0.0" sparkle:os="{os}" sparkle:edSignature="{signature}" length="1" type="application/octet-stream" />
                    </item>
                  </channel>
                </rss>
                """;
            var dataDownloader = new Mock<IAppCastDataDownloader>();
            dataDownloader.Setup(p => p.DownloadAndGetAppCastData(It.IsAny<string>())).Returns(xml);
            dataDownloader.Setup(p => p.GetAppCastEncoding()).Returns(Encoding.UTF8);
            var updateSettings = new Mock<IUpdateSettings>();
            updateSettings.SetupGet(p => p.CheckForPrerelease).Returns(true);
            var updaterService = new Mock<IUpdaterService>();
            updaterService.Setup(p => p.Get()).Returns(updateSettings.Object);
            var appCast = new IronyAppCast(false, updaterService.Object);
            var configuration = new UpdaterConfiguration(new EntryAssemblyAccessor());
            var verifier = new Ed25519Checker(SecurityMode.OnlyVerifySoftwareDownloads, Constants.PublicUpdateKey);
            appCast.SetupAppCastHandler(dataDownloader.Object, "https://example.test/appcast.xml", configuration, verifier);

            var parsed = appCast.DownloadAndParse();
            var updates = appCast.GetAvailableUpdates();

            parsed.Should().BeTrue();
            updates.Should().ContainSingle().Which.DownloadSignature.Should().Be(signature);
            dataDownloader.Verify(p => p.DownloadAndGetAppCastData("https://example.test/appcast.xml"), Times.Once);
        }

        /// <summary>
        /// The manual update path opens the exact offered release without downloading, unpacking, or executing an updater.
        /// </summary>
        /// <param name="title">The offered update title and canonical release tag.</param>
        [Theory]
        [InlineData("v1.28.95-alpha")]
        [InlineData("v1.28.100-rc")]
        [InlineData("v1.28.120")]
        public async Task Manual_update_should_only_open_exact_offered_release_page(string title)
        {
            var releasePage = $"https://github.com/bcssov/IronyModManager/releases/tag/{title}";
            var (sut, updaterService, appAction, shutDownState) = CreateUpdater("present-signature", title);
            appAction.Setup(p => p.OpenAsync(releasePage)).ReturnsAsync(true);

            var result = await sut.OpenReleasePageAsync();

            result.Should().BeTrue();
            appAction.Verify(p => p.OpenAsync(releasePage), Times.Once);
            appAction.Verify(p => p.OpenAsync(It.Is<string>(url => url.Contains("/releases/latest", StringComparison.OrdinalIgnoreCase))), Times.Never);
            updaterService.Verify(p => p.UnpackUpdateAsync(It.IsAny<string>()), Times.Never);
            appAction.Verify(p => p.RunAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            shutDownState.Verify(p => p.WaitUntilFreeAsync(), Times.Never);
            GetField<IronySparkleUpdater>(sut, "updater").UpdateDownloading.Should().BeFalse();
            GetField<IronySparkleUpdater>(sut, "updater").UpdateInstalling.Should().BeFalse();
        }

        /// <summary>
        /// The offered title is encoded as release-tag path data.
        /// </summary>
        [Fact]
        public async Task Manual_update_should_encode_offered_release_tag()
        {
            const string title = "v1.28.95-alpha+build";
            const string releasePage = "https://github.com/bcssov/IronyModManager/releases/tag/v1.28.95-alpha%2Bbuild";
            var (sut, _, appAction, _) = CreateUpdater("present-signature", title);
            appAction.Setup(p => p.OpenAsync(releasePage)).ReturnsAsync(true);

            var result = await sut.OpenReleasePageAsync();

            result.Should().BeTrue();
            appAction.Verify(p => p.OpenAsync(releasePage), Times.Once);
        }

        /// <summary>
        /// Incomplete offered-update metadata does not open an unrelated release page.
        /// </summary>
        /// <param name="title">The unusable offered update title.</param>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task Manual_update_without_title_should_not_open_release_page(string title)
        {
            var (sut, _, appAction, _) = CreateUpdater("present-signature", title);

            var result = await sut.OpenReleasePageAsync();

            result.Should().BeFalse();
            appAction.Verify(p => p.OpenAsync(It.IsAny<string>()), Times.Never);
        }

        private static (Updater Updater, Mock<IUpdaterService> UpdaterService, Mock<IAppAction> AppAction, Mock<IShutDownState> ShutDownState) CreateUpdater(string signature, string title = "999.0.0")
        {
            DISetup.SetupContainer();
            var updaterService = new Mock<IUpdaterService>();
            var appAction = new Mock<IAppAction>();
            var shutDownState = new Mock<IShutDownState>();
            var updater = new Updater(new UpdateUnpackProgressHandler(), updaterService.Object, appAction.Object, shutDownState.Object);
            SetField(updater, "updateInfo", new UpdateInfo(UpdateStatus.UpdateAvailable, [CreateItem(signature, title)]));
            return (updater, updaterService, appAction, shutDownState);
        }

        private static AppCastItem CreateItem(string signature, string title = "999.0.0")
        {
            return new AppCastItem
            {
                DownloadLink = "https://example.test/IronyModManager.zip",
                DownloadSignature = signature,
                Title = title,
                Version = "999.0.0"
            };
        }

        private static T GetField<T>(object instance, string name)
        {
            return (T)instance.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(instance);
        }

        private static void SetField(object instance, string name, object value)
        {
            instance.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(instance, value);
        }
    }
}
