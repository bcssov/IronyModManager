// ***********************************************************************
// Assembly         : IronyModManager.Tests
// ***********************************************************************
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using AutoMapper;
using AwesomeAssertions;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.FileSystem;
using IronyModManager.IO.Mods;
using IronyModManager.IO.Readers;
using IronyModManager.Implementation.Actions;
using IronyModManager.Models.Common;
using IronyModManager.Models;
using IronyModManager.Parser.Common.Mod.Search;
using IronyModManager.Parser.Mod.Search;
using IronyModManager.Parser.Mod;
using IronyModManager.Parser;
using IronyModManager.Services.Common;
using IronyModManager.Services;
using IronyModManager.Shared.Cache;
using IronyModManager.Shared.MessageBus;
using IronyModManager.Shared.Models;
using IronyModManager.Shared;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using IronyModManager.ViewModels.Controls;
using Moq;
using Xunit;

namespace IronyModManager.Tests.ViewModels.Controls
{
    public class InstalledModsControlViewModelTests
    {
        [Fact]
        public async Task Invalid_null_path_mod_should_reach_existing_warning_and_remain_when_deletion_is_declined()
        {
            DISetup.SetupContainer();
            var game = new Game { Type = "invalid-descriptor" };
            var invalid = new Mod { Name = "Invalid", DescriptorFile = "mod/invalid.mod", FullPath = null, IsValid = false };
            var valid = new Mod { Name = "Valid", DescriptorFile = "mod/valid.mod", FullPath = "valid-path", IsValid = true };
            var service = new Mock<IModService>();
            service.Setup(p => p.RefreshInstalledModsAsync(game)).ReturnsAsync(
                new InstalledModsResult { IsAuthoritative = true, Mods = [invalid, valid] });
            service.Setup(p => p.FilterMods(It.IsAny<IEnumerable<IMod>>(), It.IsAny<string>()))
                .Returns((IEnumerable<IMod> mods, string _) => mods);
            var interaction = new Mock<ModControlInteraction>(MockBehavior.Loose, new object[] { null, null, null, null });
            interaction.Setup(p => p.GetText(LocalizationResources.Installed_Mods.InvalidMods.Title)).Returns("Invalid Mods Detected");
            interaction.Setup(p => p.GetText(LocalizationResources.Installed_Mods.InvalidMods.Message)).Returns("{Mods}");
            interaction.Setup(p => p.PromptAsync("Invalid Mods Detected", "Invalid Mods Detected", "Invalid (mod/invalid.mod)",
                NotificationType.Warning, PromptType.YesNo)).ReturnsAsync(false);
            var view = CreateViewModel(service, interaction: interaction);

            (await view.BindForTestAsync(game)).Should().BeTrue();

            interaction.Verify(p => p.PromptAsync("Invalid Mods Detected", "Invalid Mods Detected", "Invalid (mod/invalid.mod)",
                NotificationType.Warning, PromptType.YesNo), Times.Once);
            service.Verify(p => p.DeleteDescriptorsAsync(It.IsAny<IEnumerable<IMod>>()), Times.Never);
            view.Mods.Should().Equal(invalid, valid);
            view.AllMods.Should().Contain(invalid).And.Contain(valid);
            invalid.IsValid.Should().BeFalse();
            invalid.IsVirtual.Should().BeFalse();
            invalid.FullPath.Should().BeNull();
        }

        [Fact]
        public async Task Achievement_evaluation_should_ignore_virtual_mods()
        {
            DISetup.SetupContainer();
            var modService = new Mock<IModService>();
            var viewModel = CreateViewModel(modService);

            await viewModel.EvalModAchievementForTestAsync([new Mod { IsVirtual = true }]);

            modService.Verify(service => service.PopulateModFilesAsync(It.IsAny<IEnumerable<IMod>>()), Times.Never);
            modService.Verify(service => service.EvalAchievementCompatibility(It.IsAny<IEnumerable<IMod>>()), Times.Never);
        }

        [Fact]
        public async Task Achievement_evaluation_should_pass_only_real_mods_to_service()
        {
            DISetup.SetupContainer();
            var realMod = new Mod { Name = "Real", IsValid = true };
            var virtualMod = new Mod { Name = "Virtual", IsVirtual = true };
            var modService = new Mock<IModService>();
            modService.Setup(service => service.PopulateModFilesAsync(It.IsAny<IEnumerable<IMod>>())).ReturnsAsync(true);

            var viewModel = CreateViewModel(modService);

            await viewModel.EvalModAchievementForTestAsync([realMod, virtualMod]);

            modService.Verify(service => service.PopulateModFilesAsync(
                It.Is<IEnumerable<IMod>>(mods => mods.SequenceEqual(new[] { realMod }))), Times.Once);
            modService.Verify(service => service.EvalAchievementCompatibility(
                It.Is<IEnumerable<IMod>>(mods => mods.SequenceEqual(new[] { realMod }))), Times.Once);
        }

        [Fact]
        public async Task Controlled_authoritative_refresh_should_publish_refreshed_mods()
        {
            DISetup.SetupContainer();
            var game = new Game { Type = "publish-revalidation" };
            var revalidationLock = new GameStateLockInfo
                { GameType = game.Type, Reason = GameStateLockReason.ConfigurationChanged };
            var discovered = new Mod { Name = "New custom mod", DescriptorFile = "mod/new-custom.mod", IsValid = true };
            var modService = new Mock<IModService>();
            modService.Setup(service => service.RevalidateInstalledModsAsync(game, revalidationLock)).ReturnsAsync(
                new InstalledModsResult { IsAuthoritative = true, Mods = [discovered] });
            modService.Setup(service => service.FilterMods(It.IsAny<IEnumerable<IMod>>(), It.IsAny<string>()))
                .Returns((IEnumerable<IMod> mods, string _) => mods);

            var filter = new SearchModsControlViewModel();
            var presentation = new Mock<InstalledModsPresentationCoordinator>(MockBehavior.Loose,
                new object[] { null, null, null, null, null });
            presentation.SetupGet(value => value.FilterMods).Returns(filter);
            var events = new Mock<InstalledModsEventCoordinator>(MockBehavior.Loose, new object[] { null });
            var factory = new Mock<IInstalledModsCoordinatorFactory>();
            factory.Setup(value => value.Create()).Returns(new InstalledModsCoordinators(presentation.Object, events.Object));
            var interaction = new Mock<ModControlInteraction>(MockBehavior.Loose,
                new object[] { null, null, null, null });
            interaction.Setup(value => value.BeginOperation()).Returns(1);
            var viewModel = new InstalledModsControlViewModel(Mock.Of<IGameService>(), modService.Object,
                Mock.Of<IGameStateSafetyService>(), interaction.Object, factory.Object);

            var authoritative = await viewModel.RevalidateModsAsync(game, revalidationLock, () => true, true);

            authoritative.Should().BeTrue();
            viewModel.Mods.Should().ContainSingle().Which.Should().BeSameAs(discovered);
            viewModel.LastRefreshAuthoritative.Should().BeTrue();
            modService.Verify(service => service.RevalidateInstalledModsAsync(game, revalidationLock), Times.Once);
            modService.Verify(service => service.RefreshInstalledModsAsync(It.IsAny<IGame>()), Times.Never);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Switching_to_locked_game_should_present_only_its_own_cached_graph(bool hasTargetCache)
        {
            DISetup.SetupContainer();
            var a = new Game { Type = "A" };
            var b = new Game { Type = "B" };
            var modA = new Mod { Game = a.Type, Name = "A", DescriptorFile = "mod/shared.mod", IsValid = true };
            var modB = new Mod { Game = b.Type, Name = "B", DescriptorFile = "mod/shared.mod", IsValid = true };
            var game = new Mock<IGameService>();
            IGame selected = a;
            game.Setup(p => p.GetSelected()).Returns(() => selected);
            var safety = new Mock<IGameStateSafetyService>();
            safety.Setup(p => p.IsLocked(b)).Returns(true);
            var service = new Mock<IModService>();
            service.Setup(p => p.RefreshInstalledModsAsync(a)).ReturnsAsync(new InstalledModsResult { IsAuthoritative = true, Mods = [modA] });
            service.Setup(p => p.RefreshInstalledModsAsync(b)).ReturnsAsync(new InstalledModsResult { IsAuthoritative = false, Mods = hasTargetCache ? [modB] : [] });
            service.Setup(p => p.FilterMods(It.IsAny<IEnumerable<IMod>>(), It.IsAny<string>())).Returns((IEnumerable<IMod> mods, string _) => mods);
            var viewModel = CreateViewModel(service, game, safety);
            await viewModel.BindForTestAsync(a);
            viewModel.ContextMenuMod = modA;

            selected = b;
            var refresh = viewModel.BindForTestAsync(b);
            viewModel.Mods.Should().BeEmpty("the source projection must clear before discovery completes");
            await refresh;

            viewModel.ActiveGame.Should().BeSameAs(b);
            viewModel.LastRefreshAuthoritative.Should().BeFalse();
            viewModel.Mods.Should().OnlyContain(p => p.Game == b.Type);
            viewModel.FilteredMods.Should().OnlyContain(p => p.Game == b.Type);
            viewModel.AllMods.Should().NotContain(modA);
            viewModel.ContextMenuMod.Should().BeNull();
            viewModel.Mods.Should().HaveCount(hasTargetCache ? 1 : 0);
            if (hasTargetCache) viewModel.Mods.Single().Should().BeSameAs(modB);

            selected = a;
            await viewModel.BindForTestAsync(a);
            viewModel.Mods.Should().Equal(modA);
            safety.Verify(p => p.Lock(a, It.IsAny<GameStateLockReason>(), It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task Same_game_failed_refresh_should_keep_that_games_known_good_objects()
        {
            DISetup.SetupContainer();
            var game = new Game { Type = "B" };
            var mod = new Mod { Game = game.Type, Name = "B", IsValid = true };
            var service = new Mock<IModService>();
            service.SetupSequence(p => p.RefreshInstalledModsAsync(game))
                .ReturnsAsync(new InstalledModsResult { IsAuthoritative = true, Mods = [mod] })
                .ReturnsAsync(new InstalledModsResult { IsAuthoritative = false, Mods = [mod] });
            service.Setup(p => p.FilterMods(It.IsAny<IEnumerable<IMod>>(), It.IsAny<string>())).Returns((IEnumerable<IMod> mods, string _) => mods);
            var viewModel = CreateViewModel(service);

            await viewModel.BindForTestAsync(game);
            await viewModel.BindForTestAsync(game);

            viewModel.Mods.Single().Should().BeSameAs(mod);
            viewModel.FilteredMods.Single().Should().BeSameAs(mod);
            viewModel.LastRefreshAuthoritative.Should().BeFalse();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Rapid_switches_should_reject_old_completions_even_after_returning_to_the_same_game(bool lateFailure)
        {
            DISetup.SetupContainer();
            var a = new Game { Type = "A" };
            var b = new Game { Type = "B" };
            var stale = new Mod { Game = "A", Name = "stale", IsValid = true };
            var current = new Mod { Game = "A", Name = "current", IsValid = true };
            var firstA = new TaskCompletionSource<InstalledModsResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            var firstB = new TaskCompletionSource<InstalledModsResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            var startedA = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var startedB = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var calls = 0;
            var service = new Mock<IModService>();
            service.Setup(p => p.RefreshInstalledModsAsync(a)).Returns(() =>
            {
                if (System.Threading.Interlocked.Increment(ref calls) == 1) { startedA.TrySetResult(); return firstA.Task; }
                return Task.FromResult(new InstalledModsResult { IsAuthoritative = true, Mods = [current] });
            });
            service.Setup(p => p.RefreshInstalledModsAsync(b)).Returns(() => { startedB.TrySetResult(); return firstB.Task; });
            service.Setup(p => p.FilterMods(It.IsAny<IEnumerable<IMod>>(), It.IsAny<string>())).Returns((IEnumerable<IMod> mods, string _) => mods);
            var viewModel = CreateViewModel(service);
            var pendingA = viewModel.BindForTestAsync(a);
            await startedA.Task;
            var pendingB = viewModel.BindForTestAsync(b);
            await startedB.Task;
            await viewModel.BindForTestAsync(a);
            firstB.SetResult(new InstalledModsResult { IsAuthoritative = !lateFailure, Mods = [new Mod { Game = "B", IsValid = true }] });
            firstA.SetResult(new InstalledModsResult { IsAuthoritative = true, Mods = [stale] });
            await Task.WhenAll(pendingA, pendingB);

            viewModel.ActiveGame.Should().BeSameAs(a);
            viewModel.Mods.Should().Equal(current);
            viewModel.FilteredMods.Should().Equal(current);
            viewModel.LastRefreshAuthoritative.Should().BeTrue();
        }

        [Fact]
        public async Task Game_collection_context_should_be_loaded_before_discovery_publishes_live_mods()
        {
            DISetup.SetupContainer();
            var game = new Game { Type = "early-collection-context" };
            var mod = new Mod { Game = game.Type, DescriptorFile = "mod/live.mod", IsValid = true };
            var service = new Mock<IModService>();
            var viewModel = CreateViewModel(service);
            var contextPresented = false;
            viewModel.GameContextPresented += (target, _) => contextPresented = target == game;
            service.Setup(p => p.RefreshInstalledModsAsync(game)).Returns(() =>
            {
                contextPresented.Should().BeTrue();
                viewModel.GameChangedRefresh.Should().BeTrue("collection rebinding precedes the live mod publication, as in the known-good RC");
                viewModel.Mods.Should().BeEmpty();
                viewModel.LastRefreshAuthoritative.Should().BeFalse();
                return Task.FromResult(new InstalledModsResult { IsAuthoritative = true, Mods = [mod] });
            });
            service.Setup(p => p.FilterMods(It.IsAny<IEnumerable<IMod>>(), It.IsAny<string>()))
                .Returns((IEnumerable<IMod> mods, string _) => mods);

            await viewModel.BindForTestAsync(game);

            viewModel.Mods.Should().Equal(mod);
            viewModel.LastRefreshAuthoritative.Should().BeTrue();
        }

        [Fact]
        public async Task Game_switch_should_release_superseded_refresh_ownership()
        {
            DISetup.SetupContainer();
            var a = new Game { Type = "A" };
            var b = new Game { Type = "B" };
            var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var pending = new TaskCompletionSource<InstalledModsResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            var service = new Mock<IModService>();
            service.Setup(p => p.RefreshInstalledModsAsync(a)).Returns(() => { started.SetResult(); return pending.Task; });
            service.Setup(p => p.RefreshInstalledModsAsync(b)).ReturnsAsync(new InstalledModsResult { IsAuthoritative = true, Mods = [] });
            service.Setup(p => p.FilterMods(It.IsAny<IEnumerable<IMod>>(), It.IsAny<string>())).Returns((IEnumerable<IMod> mods, string _) => mods);
            var gameService = new Mock<IGameService>();
            gameService.Setup(p => p.GetSelected()).Returns(a);
            var viewModel = CreateViewModel(service, gameService);
            await viewModel.BindForTestAsync(a, discover: false);
            var refresh = viewModel.RefreshModsAsync(true);
            await started.Task;

            await viewModel.BindForTestAsync(b);
            pending.SetResult(new InstalledModsResult { IsAuthoritative = true, Mods = [] });
            await refresh;

            viewModel.ActiveGame.Should().BeSameAs(b);
            viewModel.RefreshingMods.Should().BeFalse();
            viewModel.LastRefreshAuthoritative.Should().BeTrue();
        }

        public enum DirectoryRecoveryCase
        {
            NewDescriptor,
            ExistingDescriptor,
            NoWorkshopMods,
            PartialWriteFailure
        }

        [Theory]
        [InlineData(ModDescriptorType.JsonMetadata, DirectoryRecoveryCase.NewDescriptor)]
        [InlineData(ModDescriptorType.DescriptorMod, DirectoryRecoveryCase.NewDescriptor)]
        [InlineData(ModDescriptorType.JsonMetadata, DirectoryRecoveryCase.ExistingDescriptor)]
        [InlineData(ModDescriptorType.DescriptorMod, DirectoryRecoveryCase.ExistingDescriptor)]
        [InlineData(ModDescriptorType.JsonMetadata, DirectoryRecoveryCase.NoWorkshopMods)]
        [InlineData(ModDescriptorType.DescriptorMod, DirectoryRecoveryCase.NoWorkshopMods)]
        [InlineData(ModDescriptorType.JsonMetadata, DirectoryRecoveryCase.PartialWriteFailure)]
        public async Task Data_directory_recovery_should_publish_synchronized_workshop_mods_before_unlocking(
            ModDescriptorType descriptorType, DirectoryRecoveryCase recovery)
        {
            DISetup.SetupContainer();
            var temp = Directory.CreateTempSubdirectory("IronyWorkshopRecovery-");
            try
            {
                const string workshopName = "Workshop fixture";
                const string workshopId = "123456";
                var json = descriptorType == ModDescriptorType.JsonMetadata;
                var data = Directory.CreateDirectory(Path.Combine(temp.FullName, "data")).FullName;
                var workshop = Directory.CreateDirectory(Path.Combine(temp.FullName, "workshop")).FullName;
                var descriptorDirectory = json ? Shared.Constants.JsonModDirectory : Shared.Constants.ModDirectory;
                var extension = json ? ".json" : ".mod";
                var output = Path.Combine(data, descriptorDirectory);
                var workshopMod = Path.Combine(workshop, workshopId);
                var expectedMods = recovery == DirectoryRecoveryCase.NoWorkshopMods ? 0 : 1;
                if (expectedMods != 0)
                {
                    writeWorkshopFixture(workshopMod, workshopName, workshopId);
                }
                var failWrite = recovery == DirectoryRecoveryCase.PartialWriteFailure;
                if (failWrite)
                {
                    writeWorkshopFixture(Path.Combine(workshop, "654321"), "Refused workshop fixture", "654321");
                }
                if (OperatingSystem.IsWindows())
                {
                    File.WriteAllText(Path.Combine(data, "IronyModManager_IntegrityCheck.txt"), string.Empty);
                }

                var game = new Game
                {
                    Type = json ? Shared.Constants.GamesTypes.Victoria3.Id : Shared.Constants.GamesTypes.Stellaris.Id,
                    SteamAppId = json ? Shared.Constants.GamesTypes.Victoria3.SteamAppId : Shared.Constants.GamesTypes.Stellaris.SteamAppId,
                    ModDescriptorType = descriptorType,
                    UserDirectory = data,
                    CustomModDirectory = string.Empty,
                    WorkshopDirectory = [workshop]
                };
                var logger = Mock.Of<ILogger>();
                var probe = new FileSystemStateProbe();
                var cache = new Cache();
                var safety = new GameStateSafetyService(probe, logger);
                var games = new Mock<IGameService>();
                games.Setup(p => p.GetSelected()).Returns(game);
                var reader = new Reader([new JsonModFileReader(), new ModFileReader(), new DiskFileReader()], logger);
                var parser = new ModParser(logger, new CodeParser(logger));
                var mapper = new Mock<IMapper>();
                mapper.Setup(p => p.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject mod) => new Mod
                {
                    Name = mod.Name,
                    FileName = mod.FileName,
                    JsonId = mod.JsonId,
                    RemoteId = mod.RemoteId,
                    Version = mod.Version,
                    Dependencies = mod.Dependencies ?? [],
                    ReplacePath = mod.ReplacePath ?? [],
                    UserDir = mod.UserDir ?? []
                });
                var diskWriter = new ModWriter(logger, mapper.Object, new IronyModManager.IO.DriveInfoProvider(logger), Mock.Of<IMessageBus>());
                var blockBootstrap = true;
                var writer = new Mock<IModWriter>();
                writer.Setup(p => p.FormatPrefixModName(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns((string prefix, string name) => diskWriter.FormatPrefixModName(prefix, name));
                writer.Setup(p => p.CanWriteToModDirectoryAsync(It.IsAny<ModWriterParameters>()))
                    .Returns((ModWriterParameters p) => diskWriter.CanWriteToModDirectoryAsync(p));
                writer.Setup(p => p.CreateModDirectoryAsync(It.IsAny<ModWriterParameters>()))
                    .Returns((ModWriterParameters p) => blockBootstrap && p.Path == descriptorDirectory
                        ? Task.FromResult(false)
                        : diskWriter.CreateModDirectoryAsync(p));
                writer.Setup(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>()))
                    .Returns((ModWriterParameters p, bool writeInMod) => failWrite && p.Mod.RemoteId == 654321
                        ? Task.FromResult(false)
                        : diskWriter.WriteDescriptorAsync(p, writeInMod));

                var languages = new Mock<ILanguagesService>();
                languages.Setup(p => p.GetSelected()).Returns(new Language { Abrv = "en" });
                var search = new Mock<IParser>();
                search.Setup(p => p.Parse("en", It.IsAny<string>())).Returns(() => new SearchParserResult());
                ModService createService(ICache modCache, IGameStateSafetyService safetyService) =>
                    new(languages.Object, search.Object, logger, modCache, null, reader, parser, writer.Object,
                        games.Object, Mock.Of<IStorageProvider>(), mapper.Object, probe, safetyService, () => new Mod(), () => Mock.Of<IModAlias>());

                var service = createService(cache, safety);
                var view = CreateViewModel(service, games.Object, safety);
                (await view.BindForTestAsync(game)).Should().BeFalse();
                safety.IsLocked(game).Should().BeTrue();
                Directory.Exists(output).Should().BeFalse();
                readRegularCache().Should().BeNull();
                view.LastRefreshAuthoritative.Should().BeFalse();
                view.Mods.Should().BeEmpty();

                blockBootstrap = false;
                if (!json || recovery != DirectoryRecoveryCase.NewDescriptor)
                {
                    Directory.CreateDirectory(output);
                }
                if (recovery == DirectoryRecoveryCase.ExistingDescriptor)
                {
                    var seedService = createService(new Cache(), new GameStateSafetyService(probe, logger));
                    var seeded = await seedService.InstallModsAsync([]);
                    seeded.Should().ContainSingle(result => result.Installed && !result.Invalid);
                    writer.Invocations.Clear();
                }

                var coordinator = new ModStateReconciliationCoordinator(safety);
                var steps = new List<string>();
                var recovered = await coordinator.RevalidateConfigurationAsync(game,
                    (revalidationLock, isCurrent) =>
                    {
                        steps.Add("refresh");
                        isCurrent().Should().BeTrue();
                        view.LastRefreshAuthoritative.Should().BeFalse();
                        readRegularCache().Should().BeNull();
                        Directory.EnumerateFiles(output, "*" + extension).Should().HaveCount(expectedMods);
                        return view.RevalidateModsAsync(game, revalidationLock, isCurrent, skipOverlay: true);
                    },
                    reset: null,
                    reconcile: revalidationLock =>
                    {
                        steps.Add("reconcile");
                        safety.IsCurrentRevalidation(game, revalidationLock).Should().BeTrue();
                        view.LastRefreshAuthoritative.Should().BeTrue();
                        view.Mods.Should().HaveCount(expectedMods);
                        readRegularCache().Should().Equal(view.Mods);
                    },
                    synchronize: (revalidationLock, isCurrent) =>
                    {
                        steps.Add("synchronize");
                        isCurrent().Should().BeTrue();
                        return view.SynchronizeNewModsAsync(game, revalidationLock);
                    });

                recovered.Should().Be(!failWrite);
                if (failWrite)
                {
                    steps.Should().Equal("synchronize");
                    safety.GetLock(game).Reason.Should().Be(GameStateLockReason.WriteAccessFailure);
                    readRegularCache().Should().BeNull();
                    view.LastRefreshAuthoritative.Should().BeFalse();
                    view.Mods.Should().BeEmpty();
                    Directory.EnumerateFiles(output, "*" + extension).Should().ContainSingle();
                    writer.Verify(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>()), Times.Exactly(2));
                    return;
                }

                steps.Should().Equal("synchronize", "refresh", "reconcile");
                safety.IsLocked(game).Should().BeFalse();
                view.LastRefreshAuthoritative.Should().BeTrue();
                view.FilteredMods.Should().Equal(view.Mods);
                readRegularCache().Should().Equal(view.Mods);
                if (expectedMods != 0)
                {
                    var installed = view.Mods.Should().ContainSingle().Which;
                    installed.Name.Should().Be(workshopName);
                    installed.FullPath.Should().Be(workshopMod);
                    installed.IsValid.Should().BeTrue();
                }
                writer.Verify(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>()),
                    recovery == DirectoryRecoveryCase.NewDescriptor ? Times.Once() : Times.Never());

                IEnumerable<IMod> readRegularCache() => cache.Get<IEnumerable<IMod>>(
                    new CacheGetParameters { Region = "Mods", Prefix = game.Type, Key = "RegularMods" });

                void writeWorkshopFixture(string modPath, string name, string id)
                {
                    var manifest = Path.Combine(modPath, json ? Shared.Constants.DescriptorJsonMetadata : Shared.Constants.DescriptorFile);
                    Directory.CreateDirectory(Path.GetDirectoryName(manifest)!);
                    var contents = json
                        ? $$"""{"id":"{{id}}","name":"{{name}}","supported_game_version":"1.0","relationships":[]}"""
                        : $"""
                           name="{name}"
                           supported_version="1.0"
                           remote_file_id="{id}"
                           """;
                    File.WriteAllText(manifest, contents);
                }
            }
            finally
            {
                temp.Delete(recursive: true);
            }
        }

        private static TestInstalledModsControlViewModel CreateViewModel(Mock<IModService> modService,
            Mock<IGameService> game = null, Mock<IGameStateSafetyService> safety = null,
            Mock<ModControlInteraction> interaction = null) =>
            CreateViewModel(modService.Object, game?.Object ?? Mock.Of<IGameService>(),
                safety?.Object ?? Mock.Of<IGameStateSafetyService>(), interaction);

        private static TestInstalledModsControlViewModel CreateViewModel(IModService modService, IGameService game,
            IGameStateSafetyService safety, Mock<ModControlInteraction> interaction = null)
        {
            var filter = new SearchModsControlViewModel();
            var presentation = new Mock<InstalledModsPresentationCoordinator>(MockBehavior.Loose,
                new object[] { null, null, null, null, null });
            presentation.SetupGet(value => value.FilterMods).Returns(filter);
            var events = new Mock<InstalledModsEventCoordinator>(MockBehavior.Loose, new object[] { null });
            var factory = new Mock<IInstalledModsCoordinatorFactory>();
            factory.Setup(value => value.Create()).Returns(new InstalledModsCoordinators(presentation.Object, events.Object));
            interaction ??= new Mock<ModControlInteraction>(MockBehavior.Loose,
                new object[] { null, null, null, null });

            return new TestInstalledModsControlViewModel(game, modService, safety, interaction.Object, factory.Object);
        }

        private sealed class TestInstalledModsControlViewModel(IGameService gameService, IModService modService,
            IGameStateSafetyService gameStateSafetyService, ModControlInteraction interaction,
            IInstalledModsCoordinatorFactory coordinatorFactory)
            : InstalledModsControlViewModel(gameService, modService, gameStateSafetyService, interaction, coordinatorFactory)
        {
            public Task<bool> BindForTestAsync(IGame game, bool discover = true)
            {
                if (!discover) { ActiveGame = game; return Task.FromResult(true); }
                return BindAsync(game, skipOverlay: true);
            }
            protected override Task TriggerOverlayAsync(long id, bool isVisible, string message, string progress) => Task.CompletedTask;

            public Task EvalModAchievementForTestAsync(IEnumerable<IMod> mods) => EvalModAchievementAsync(mods, true);
        }
    }
}
