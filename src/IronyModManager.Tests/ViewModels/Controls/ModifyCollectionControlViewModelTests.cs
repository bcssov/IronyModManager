// ***********************************************************************
// Assembly         : IronyModManager.Tests
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using IronyModManager.Common;
using IronyModManager.Implementation;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.AppState;
using IronyModManager.Implementation.MessageBus;
using IronyModManager.Implementation.Overlay;
using IronyModManager.Localization;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Tests.Common;
using IronyModManager.ViewModels.Controls;
using Moq;
using Xunit;

namespace IronyModManager.Tests.ViewModels.Controls
{
    /// <summary>
    /// Tests Merge Compress command orchestration.
    /// </summary>
    public class ModifyCollectionControlViewModelTests
    {
        /// <summary>
        /// A failed archive preflight stops the command before destructive collaborators run.
        /// </summary>
        [Fact]
        public async Task Should_stop_merge_compress_before_cleanup_when_archive_preflight_fails()
        {
            DISetup.SetupContainer();
            var messageBus = new Mock<Shared.MessageBus.IMessageBus>();
            DISetup.Container.RegisterInstance(messageBus.Object);
            var gameService = new Mock<IGameService>();
            var modService = new Mock<IModService>();
            var idGenerator = new Mock<IIDGenerator>();
            var shutDownState = new Mock<IShutDownState>();
            var modMergeService = new Mock<IModMergeService>();
            var modCollectionService = new Mock<IModCollectionService>();
            var modPatchCollectionService = new Mock<IModPatchCollectionService>();
            var localizationManager = new Mock<ILocalizationManager>();
            var notificationAction = new Mock<INotificationAction>();
            var selectedCollection = new ModCollection { IsSelected = true, Name = "Original", Mods = ["mod/one.mod"] };
            var mergedCollection = new ModCollection { IsSelected = true, Name = "Merged", Mods = ["mod/one.mod"] };
            gameService.Setup(p => p.GetSelected()).Returns(new Game { SupportedMergeTypes = SupportedMergeTypes.Zip });
            shutDownState.Setup(p => p.WaitUntilFreeAsync()).Returns(Task.CompletedTask);
            modCollectionService.Setup(p => p.GetAll()).Returns([selectedCollection]);
            modMergeService.Setup(p => p.AllowModMergeAsync(mergedCollection.Name)).Returns(ValueTask.FromResult(true));
            modMergeService.Setup(p => p.HasEnoughFreeSpaceAsync(mergedCollection.Name)).ReturnsAsync(true);
            modMergeService.Setup(p => p.PreflightMergeCompressCollection(mergedCollection.Name, It.IsAny<string>())).Returns(new MergeCompressPreflightResult
            {
                UnavailableArchiveNames = ["locked.zip"]
            });
            localizationManager.Setup(p => p.GetResource(It.IsAny<string>())).Returns((string key) => key);
            localizationManager.Setup(p => p.GetResource(Shared.LocalizationResources.Notifications.CollectionMergeArchiveUnavailable.Message)).Returns("{Archives}");

            var viewModel = new TestModifyCollectionControlViewModel(gameService.Object, new ModMergeFreeSpaceCheckHandler(), modService.Object, idGenerator.Object,
                new ModCompressMergeProgressHandler(), new ModFileMergeProgressHandler(), shutDownState.Object, modMergeService.Object, modCollectionService.Object,
                modPatchCollectionService.Object, localizationManager.Object, notificationAction.Object, mergedCollection)
            {
                ActiveCollection = selectedCollection,
                AllowModSelection = true
            };
            using var disposables = viewModel.Initialize();
            var completion = new TaskCompletionSource<CommandResult<ModifyCollectionControlViewModel.ModifyAction>>();
            using var subscription = viewModel.MergeCompressCommand.Execute().Subscribe(completion.SetResult, completion.SetException);

            var result = await completion.Task;

            result.State.Should().Be(CommandState.Failed);
            viewModel.OverlayStates.Should().EndWith(false);
            modPatchCollectionService.Verify(p => p.CleanPatchCollectionAsync(It.IsAny<string>()), Times.Never);
            modMergeService.Verify(p => p.MergeCompressCollectionAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            modCollectionService.Verify(p => p.Save(It.IsAny<IModCollection>()), Times.Never);
            notificationAction.Verify(p => p.ShowNotification(It.IsAny<string>(), "locked.zip", NotificationType.Error, 10, null), Times.Once);
        }

        private class TestModifyCollectionControlViewModel : ModifyCollectionControlViewModel
        {
            private readonly IModCollection mergedCollection;

            public TestModifyCollectionControlViewModel(IGameService gameService, ModMergeFreeSpaceCheckHandler modMergeFreeSpaceCheckHandler, IModService modService,
                IIDGenerator idGenerator, ModCompressMergeProgressHandler modCompressMergeProgressHandler, ModFileMergeProgressHandler modFileMergeProgressHandler,
                IShutDownState shutDownState, IModMergeService modMergeService, IModCollectionService modCollectionService,
                IModPatchCollectionService modPatchCollectionService, ILocalizationManager localizationManager, INotificationAction notificationAction,
                IModCollection mergedCollection)
                : base(gameService, modMergeFreeSpaceCheckHandler, modService, idGenerator, modCompressMergeProgressHandler, modFileMergeProgressHandler,
                    shutDownState, modMergeService, modCollectionService, modPatchCollectionService, localizationManager, notificationAction)
            {
                this.mergedCollection = mergedCollection;
            }

            public List<bool> OverlayStates { get; } = [];

            public CompositeDisposable Initialize()
            {
                var disposables = new CompositeDisposable();
                OnActivated(disposables);
                return disposables;
            }

            protected override Task<Tuple<string, IModCollection>> GetMergedCollectionAsync()
            {
                return Task.FromResult(new Tuple<string, IModCollection>("Original", mergedCollection));
            }

            protected override Task TriggerOverlayAsync(long id, bool isVisible, string message = Shared.Constants.EmptyParam, string progress = Shared.Constants.EmptyParam)
            {
                OverlayStates.Add(isVisible);
                return Task.CompletedTask;
            }
        }
    }
}
