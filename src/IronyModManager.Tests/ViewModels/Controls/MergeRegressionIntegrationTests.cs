using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AwesomeAssertions;
using IronyModManager.Common;
using IronyModManager.Implementation;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.AppState;
using IronyModManager.Implementation.MessageBus;
using IronyModManager.Implementation.Overlay;
using IronyModManager.IO.Common;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Localization;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Services;
using IronyModManager.Services.Common;
using IronyModManager.Shared.Cache;
using IronyModManager.Shared.Configuration;
using IronyModManager.Shared.MessageBus;
using IronyModManager.Shared.Models;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using IronyModManager.ViewModels.Controls;
using Moq;
using Xunit;

namespace IronyModManager.Tests.ViewModels.Controls
{
    public class MergeRegressionIntegrationTests
    {
        [Theory]
        [InlineData(false, 0)]
        [InlineData(true, 0)]
        [InlineData(false, 1)]
        [InlineData(true, 1)]
        [InlineData(false, 2)]
        [InlineData(true, 2)]
        public async Task Commands_should_merge_real_members_into_a_new_output_collection(bool compress, int missingCount)
        {
            using var fixture = new Fixture(missingCount);
            var membership = fixture.Collection.Mods.ToList();
            var paths = fixture.Collection.ModPaths.ToList();

            var result = await fixture.ExecuteAsync(compress);

            result.State.Should().Be(CommandState.Success);
            fixture.ViewModel.Overlays.Should().Contain(true).And.EndWith(false);
            fixture.Saved.Should().NotBeNull();
            fixture.Saved.Name.Should().NotBe(fixture.Collection.Name);
            if (compress)
                fixture.Writer.Verify(p => p.WriteDescriptorToStreamAsync(It.IsAny<ModWriterParameters>(), It.IsAny<Stream>(), true), Times.Exactly(3));
            else
                fixture.Writer.Verify(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), true), Times.Once());
            fixture.Reader.Verify(p => p.GetTotalSize(It.IsAny<string>()), Times.Exactly(3));
            fixture.Reader.Verify(p => p.GetTotalSize(It.Is<string>(s => s.Contains("missing"))), Times.Never);
            fixture.Collection.Mods.Should().Equal(membership);
            fixture.Collection.ModPaths.Should().Equal(paths);
            fixture.Members.Select(p => p.DescriptorFile).Should().Equal(membership);
            fixture.Members.Where(p => !p.IsVirtual).Should().OnlyContain(p => !p.IsSelected);
            if (compress)
            {
                fixture.Saved.Mods.Should().HaveCount(3);
                fixture.Saved.Mods.Select(Path.GetFileNameWithoutExtension).Should().Equal("Merged_Original_A", "Merged_Original_C", "Merged_Original_E");
                fixture.Compressor.Verify(p => p.Finalize(It.IsAny<long>(), It.IsAny<string>()), Times.Exactly(3));
            }
            else
            {
                fixture.ExportedSources.Should().Equal("A", "C", "E");
            }

            fixture.VerifyPrompts(missingCount);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Declining_missing_members_should_cancel_before_any_mutation(bool compress)
        {
            using var fixture = new Fixture(2, confirm: false);
            var membership = fixture.Collection.Mods.ToList();

            var result = await fixture.ExecuteAsync(compress);

            result.State.Should().Be(CommandState.NotExecuted);
            fixture.Writer.Invocations.Should().BeEmpty();
            fixture.Patch.Invocations.Should().BeEmpty();
            fixture.CollectionService.Verify(p => p.Create(), Times.Never);
            fixture.CollectionService.Verify(p => p.Save(It.IsAny<IModCollection>()), Times.Never);
            fixture.Reader.Invocations.Should().BeEmpty();
            fixture.ViewModel.Overlays.Should().BeEmpty();
            fixture.Collection.Mods.Should().Equal(membership);
            fixture.Notifications.Verify(p => p.ShowNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NotificationType>(), It.IsAny<int>(), It.IsAny<Action>()), Times.Never);
            fixture.VerifyPrompts(2);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task No_real_inputs_or_locked_game_should_report_failure_and_leave_outputs_untouched(bool compress, bool locked)
        {
            using var fixture = new Fixture(1, onlyMissing: !locked, locked: locked);

            var result = await fixture.ExecuteAsync(compress);

            result.State.Should().Be(CommandState.Failed);
            fixture.ViewModel.Overlays.Should().EndWith(false);
            fixture.Writer.Verify(p => p.PurgeModDirectoryAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>()), Times.Never);
            fixture.Writer.Verify(p => p.WriteDescriptorToStreamAsync(It.IsAny<ModWriterParameters>(), It.IsAny<Stream>(), It.IsAny<bool>()), Times.Never);
            fixture.CollectionService.Verify(p => p.Save(It.IsAny<IModCollection>()), Times.Never);
            fixture.Notifications.Verify(p => p.ShowNotification(It.IsAny<string>(), It.IsAny<string>(), NotificationType.Error, 10, null), Times.Once);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Classified_output_failure_after_progress_starts_should_report_failure_and_close_overlay(bool compress)
        {
            using var fixture = new Fixture(0, destinationFailure: true);

            var result = await fixture.ExecuteAsync(compress);

            result.State.Should().Be(CommandState.Failed);
            fixture.ViewModel.Overlays.Should().Contain(true).And.EndWith(false);
            fixture.CollectionService.Verify(p => p.Save(It.IsAny<IModCollection>()), Times.Never);
            fixture.Notifications.Verify(p => p.ShowNotification(It.IsAny<string>(), It.IsAny<string>(), NotificationType.Error, 10, null), Times.Once);
        }

        [Fact]
        public async Task Archive_becoming_unavailable_after_preflight_should_report_archive_error_and_close_overlay()
        {
            using var fixture = new Fixture(0);
            fixture.Compressor.SetupSequence(p => p.GetUnavailableArchivePaths(It.IsAny<IEnumerable<string>>()))
                .Returns([]).Returns(["locked.zip"]);

            var result = await fixture.ExecuteAsync(true);

            result.State.Should().Be(CommandState.Failed);
            fixture.ViewModel.Overlays.Should().EndWith(false);
            fixture.Writer.Verify(p => p.PurgeModDirectoryAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>()), Times.Never);
            fixture.Writer.Verify(p => p.WriteDescriptorToStreamAsync(It.IsAny<ModWriterParameters>(), It.IsAny<Stream>(), It.IsAny<bool>()), Times.Never);
            fixture.CollectionService.Verify(p => p.Save(It.IsAny<IModCollection>()), Times.Never);
            fixture.Notifications.Verify(p => p.ShowNotification(It.IsAny<string>(), It.Is<string>(s => s.Contains("locked.zip")), NotificationType.Error, 10, null), Times.Once);
        }

        private sealed class Fixture : IDisposable
        {
            private readonly CompositeDisposable disposables;
            public readonly Mock<IModWriter> Writer = new();
            public readonly Mock<IReader> Reader = new();
            public readonly Mock<IModMergeCompressExporter> Compressor = new();
            public readonly Mock<IModCollectionService> CollectionService = new();
            public readonly Mock<IModPatchCollectionService> Patch = new();
            public readonly Mock<INotificationAction> Notifications = new();
            public readonly List<string> ExportedSources = [];
            public readonly List<IMod> Members = [];
            public readonly ModCollection Collection;
            public readonly TestViewModel ViewModel;
            public IModCollection Saved;
            private string prompt;

            public Fixture(int missingCount, bool confirm = true, bool onlyMissing = false, bool locked = false, bool destinationFailure = false)
            {
                DISetup.SetupContainer();
                var bus = new Mock<IMessageBus>();
                DISetup.Container.RegisterInstance(bus.Object);
                var config = new Mock<IDomainConfiguration>();
                config.Setup(p => p.GetOptions()).Returns(new DomainConfigurationOptions());
                DISetup.Container.RegisterInstance(config.Object);
                var game = new Game { Type = "merge-regression", UserDirectory = "user", GameFolders = ["common"], SupportedMergeTypes = SupportedMergeTypes.Basic | SupportedMergeTypes.Zip };
                var gameService = new Mock<IGameService>();
                gameService.Setup(p => p.GetSelected()).Returns(game);
                foreach (var name in new[] { "A", "B", "C", "D", "E" })
                {
                    var missing = name == "B" || name == "D";
                    if ((missing && missingCount-- <= 0) || (!missing && onlyMissing)) continue;
                    Members.Add(new Mod { Name = name, DescriptorFile = $"mod/{name}.mod", FullPath = missing ? $"missing/{name}" : name,
                        Game = game.Type, IsValid = true, IsVirtual = missing, Dependencies = [], Files = ["common/test.txt"] });
                }

                Collection = new ModCollection { Name = "Original", Game = game.Type, IsSelected = true,
                    Mods = Members.Select(p => p.DescriptorFile).ToList(), ModPaths = Members.Select(p => p.FullPath).ToList() };
                var storage = new Mock<IStorageProvider>();
                storage.Setup(p => p.GetModCollections()).Returns([Collection]);
                CollectionService.Setup(p => p.GetAll()).Returns([Collection]);
                CollectionService.Setup(p => p.Create()).Returns(() => new ModCollection());
                CollectionService.Setup(p => p.Save(It.IsAny<IModCollection>())).Callback((IModCollection c) => Saved = c).Returns(true);
                var cache = new Mock<ICache>();
                cache.Setup(p => p.Get<IEnumerable<IMod>>(It.IsAny<CacheGetParameters>())).Returns(Members);
                var safety = new Mock<IGameStateSafetyService>();
                safety.Setup(p => p.IsLocked(game)).Returns(locked);
                if (destinationFailure)
                {
                    Writer.Setup(p => p.PurgeModDirectoryAsync(It.IsAny<ModWriterParameters>(), true)).ThrowsAsync(new UnauthorizedAccessException());
                    safety.Setup(p => p.LockIfFileSystemAccessFailure(game, GameStateLockReason.WriteAccessFailure, It.IsAny<string>(), It.IsAny<Exception>())).Returns(true);
                }
                var drive = new Mock<IDriveInfoProvider>();
                drive.Setup(p => p.HasFreeSpace(It.IsAny<string>(), 30)).Returns(true);
                Reader.Setup(p => p.GetTotalSize(It.IsAny<string>())).Returns(10);
                Reader.Setup(p => p.GetStream(It.IsAny<string>(), It.IsAny<string>())).Returns(() => new MemoryStream([1, 2, 3]));
                var exporter = new Mock<IModMergeExporter>();
                exporter.Setup(p => p.ExportFilesAsync(It.IsAny<ModMergeFileExporterParameters>())).Callback((ModMergeFileExporterParameters p) => ExportedSources.Add(p.RootModPath)).ReturnsAsync(true);
                Compressor.Setup(p => p.GetUnavailableArchivePaths(It.IsAny<IEnumerable<string>>())).Returns([]);
                Compressor.Setup(p => p.Start()).Returns(1);
                Compressor.Setup(p => p.Finalize(It.IsAny<long>(), It.IsAny<string>())).Returns(true);
                var preferences = new Mock<IPreferencesService>();
                preferences.Setup(p => p.Get()).Returns(new Preferences());
                var merge = new ModMergeService(preferences.Object, drive.Object, Compressor.Object, cache.Object, bus.Object,
                    Mock.Of<IModPatchExporter>(), exporter.Object, [Mock.Of<IDefinitionInfoProvider>()], Reader.Object, Writer.Object,
                    Mock.Of<IModParser>(), gameService.Object, storage.Object, Mock.Of<IMapper>(), safety.Object);
                var localization = new Mock<ILocalizationManager>();
                localization.Setup(p => p.GetResource(It.IsAny<string>())).Returns((string key) => key);
                localization.Setup(p => p.GetResource(Shared.LocalizationResources.Collection_Mods.MergeCollection.MergedCollectionPrefix)).Returns("Merged");
                localization.Setup(p => p.GetResource(Shared.LocalizationResources.Collection_Mods.MergeCollection.MergeCompressModPrefix)).Returns("Merged_{Name}");
                localization.Setup(p => p.GetResource(Shared.LocalizationResources.Collection_Mods.MergeCollection.MissingModsPrompt.Message)).Returns("Unavailable:{NewLine}{Mods}{NewLine}Continue?");
                localization.Setup(p => p.GetResource(Shared.LocalizationResources.Notifications.CollectionMergeArchiveUnavailable.Message)).Returns("Unavailable: {Archives}");
                Notifications.Setup(p => p.ShowPromptAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), NotificationType.Warning, It.IsAny<PromptType>()))
                    .Callback((string _, string _, string message, NotificationType _, PromptType _) => prompt = message).ReturnsAsync(confirm);
                var shutdown = new Mock<IShutDownState>();
                shutdown.Setup(p => p.WaitUntilFreeAsync()).Returns(Task.CompletedTask);
                ViewModel = new TestViewModel(gameService.Object, merge, CollectionService.Object, Patch.Object, localization.Object, Notifications.Object, shutdown.Object)
                    { ActiveCollection = Collection, SelectedMods = Members, AllowModSelection = true };
                disposables = ViewModel.Initialize();
            }

            public async Task<CommandResult<ModifyCollectionControlViewModel.ModifyAction>> ExecuteAsync(bool compress)
            {
                return await (compress ? ViewModel.MergeCompressCommand : ViewModel.MergeBasicCommand).Execute().FirstAsync();
            }

            public void VerifyPrompts(int count)
            {
                Notifications.Verify(p => p.ShowPromptAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), NotificationType.Warning, It.IsAny<PromptType>()), count == 0 ? Times.Never() : Times.Once());
                if (count > 0) prompt.Should().Contain("B (mod/B.mod)");
                if (count > 1) prompt.Should().Contain("D (mod/D.mod)");
            }

            public void Dispose() => disposables.Dispose();
        }

        private sealed class TestViewModel(IGameService game, IModMergeService merge, IModCollectionService collections,
            IModPatchCollectionService patch, ILocalizationManager localization, INotificationAction notifications, IShutDownState shutdown)
            : ModifyCollectionControlViewModel(game, new ModMergeFreeSpaceCheckHandler(), Mock.Of<IModService>(), Mock.Of<IIDGenerator>(),
                new ModCompressMergeProgressHandler(), new ModFileMergeProgressHandler(), shutdown, merge, collections, patch, localization, notifications)
        {
            public List<bool> Overlays { get; } = [];
            public CompositeDisposable Initialize() { var result = new CompositeDisposable(); OnActivated(result); return result; }
            protected override Task TriggerOverlayAsync(long id, bool isVisible, string message = Shared.Constants.EmptyParam, string progress = Shared.Constants.EmptyParam)
            { Overlays.Add(isVisible); return Task.CompletedTask; }
        }
    }
}
