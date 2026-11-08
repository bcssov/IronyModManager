// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using AwesomeAssertions;
using IronyModManager.IO.Common.DLC;
using IronyModManager.IO.Common.FileSystem;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.DLC;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;
using IronyModManager.Storage.Common;
using Moq;
using SimpleInjector;
using Xunit;

namespace IronyModManager.Services.Tests
{
    public class GameStateSafetyInterceptorTests
    {
        private static readonly Type[] safetyContracts =
        [
            typeof(IDLCService),
            typeof(IModService),
            typeof(IModCollectionService),
            typeof(IModMergeService),
            typeof(IModPatchCollectionService)
        ];

        [Fact]
        public void Participating_contracts_require_an_explicit_classification_for_every_method()
        {
            foreach (var contract in safetyContracts)
            {
                contract.GetCustomAttribute<GameStateSafetyContractAttribute>().Should().NotBeNull();
                foreach (var method in contract.GetMethods())
                {
                    var safety = method.GetCustomAttribute<GameStateSafetyAttribute>();
                    var exempt = method.GetCustomAttribute<GameStateSafetyExemptAttribute>();
                    ((safety == null) ^ (exempt == null)).Should().BeTrue(
                        $"{contract.Name}.{method.Name} must have exactly one game-state safety classification");
                    if (safety != null)
                    {
                        safety.Context.Should().NotBeNullOrWhiteSpace();
                        safety.LockOnFileSystemFailure.Should().Be(safety.FailureReason.HasValue,
                            $"{contract.Name}.{method.Name} must declare a failure reason exactly when filesystem-failure locking is enabled");
                        AssertRejectionMatches(method, safety.Rejection);
                    }
                    else
                    {
                        exempt.Reason.Should().NotBeNullOrWhiteSpace();
                    }
                }
            }
        }

        [Fact]
        public void Failure_reason_metadata_matches_the_phase_audit()
        {
            var guarded = safetyContracts.SelectMany(contract => contract.GetMethods()
                .Select(method => new
                {
                    Contract = contract.Name,
                    Method = method.Name,
                    Safety = method.GetCustomAttribute<GameStateSafetyAttribute>()
                }))
                .Where(value => value.Safety != null)
                .ToList();

            guarded.Where(value => value.Safety.FailureReason.HasValue)
                .Select(value => $"{value.Contract}.{value.Method}:{value.Safety.FailureReason}")
                .Should().BeEquivalentTo(
                [
                    "IDLCService.SyncStateAsync:DiscoveryUnavailable",
                    "IModService.GetImageStreamAsync:DiscoveryUnavailable",
                    "IModService.GetImageStreamAsync:DiscoveryUnavailable",
                    "IModService.DeleteDescriptorsAsync:WriteAccessFailure",
                    "IModService.LockDescriptorsAsync:WriteAccessFailure",
                    "IModService.PurgeModDirectoryAsync:WriteAccessFailure",
                    "IModService.PurgeModPatchAsync:WriteAccessFailure",
                    "IModPatchCollectionService.CreatePatchDefinitionAsync:DiscoveryUnavailable",
                    "IModPatchCollectionService.GetModObjectsAsync:DiscoveryUnavailable",
                    "IModPatchCollectionService.CleanPatchCollectionAsync:WriteAccessFailure"
                ]);

            guarded.Where(value => !value.Safety.FailureReason.HasValue)
                .Select(value => $"{value.Contract}.{value.Method}")
                .Should().BeEquivalentTo(
                [
                    "IDLCService.ExportAsync",
                    "IModService.InstallModsAsync",
                    "IModService.InstallModsAsync",
                    "IModMergeService.MergeCollectionByFilesAsync",
                    "IModMergeService.MergeCompressCollectionAsync",
                    "IModPatchCollectionService.AddCustomModPatchAsync",
                    "IModPatchCollectionService.ApplyModPatchAsync",
                    "IModPatchCollectionService.IgnoreModPatchAsync",
                    "IModPatchCollectionService.CopyPatchCollectionAsync",
                    "IModPatchCollectionService.RenamePatchCollectionAsync",
                    "IModPatchCollectionService.SaveIgnoredPathsAsync"
                ]);
        }

        [Fact]
        public void Safety_attributes_are_passive_immutable_metadata()
        {
            var allowedPropertyTypes = new[]
            {
                typeof(string), typeof(bool), typeof(GameStateSafetyOperation), typeof(GameStateSafetyRejection),
                typeof(GameStateSafetyEnforcement), typeof(GameStateLockReason?)
            };
            foreach (var type in new[]
                     {
                         typeof(GameStateSafetyAttribute), typeof(GameStateSafetyExemptAttribute),
                         typeof(GameStateSafetyContractAttribute)
                     })
            {
                type.IsSealed.Should().BeTrue();
                type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Should().OnlyContain(property =>
                    allowedPropertyTypes.Contains(property.PropertyType) && property.SetMethod == null);
            }
        }

        [Fact]
        public async Task Production_service_registration_uses_interface_metadata_and_proxy_enforcement()
        {
            var game = new Game { Type = "production-proxy" };
            var probe = new Mock<IFileSystemStateProbe>();
            var safety = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            safety.Lock(game, GameStateLockReason.WriteAccessFailure, "test");
            var exporter = new Mock<IDLCExporter>();
            var gameService = new Mock<IGameService>();
            gameService.Setup(service => service.GetSelected()).Returns(game);
            var container = new Container();
            container.Options.AllowOverridingRegistrations = true;
            container.Options.EnableAutoVerification = false;
            new DIPackage().RegisterServices(container);
            container.RegisterInstance(exporter.Object);
            container.RegisterInstance(Mock.Of<ICache>());
            container.RegisterInstance(Mock.Of<IReader>());
            container.RegisterInstance(Mock.Of<IDLCParser>());
            container.RegisterInstance(Mock.Of<IStorageProvider>());
            container.RegisterInstance(Mock.Of<IMapper>());
            container.RegisterInstance<IGameService>(gameService.Object);
            container.RegisterInstance<IGameStateSafetyService>(safety);
            container.RegisterInstance(probe.Object);

            var service = container.GetInstance<IDLCService>();
            var result = await service.ExportAsync(game, [new DLC { IsEnabled = false }]);

            service.Should().NotBeOfType<DLCService>();
            result.Should().BeFalse();
            exporter.Verify(value => value.ExportDLCAsync(It.IsAny<DLCParameters>()), Times.Never);
        }

        [Fact]
        public async Task Explicit_game_argument_takes_precedence_over_selected_game()
        {
            var explicitGame = new Game { Type = "explicit" };
            var selectedGame = new Game { Type = "selected" };
            var fixture = CreateFixture(selectedGame);
            fixture.Target.ExplicitHandler = _ => throw new IOException("unavailable");

            var result = await fixture.Service.ExplicitAsync(explicitGame);

            result.Should().BeFalse();
            fixture.Safety.IsLocked(explicitGame).Should().BeTrue();
            fixture.Safety.IsLocked(selectedGame).Should().BeFalse();
            fixture.Safety.GetLock(explicitGame).Reason.Should().Be(GameStateLockReason.DiscoveryUnavailable);
            fixture.Safety.GetLock(explicitGame).Context.Should().Be("Explicit test");
        }

        [Fact]
        public async Task Null_explicit_game_does_not_fall_back_to_selected_game()
        {
            var selectedGame = new Game { Type = "selected" };
            var fixture = CreateFixture(selectedGame);

            var result = await fixture.Service.ExplicitAsync(null);

            result.Should().BeFalse();
            fixture.Target.ExplicitCalls.Should().Be(0);
            fixture.GameService.Verify(service => service.GetSelected(), Times.Never);
            fixture.Safety.IsLocked(selectedGame).Should().BeFalse();
        }

        [Fact]
        public async Task Selected_game_is_captured_once_for_an_async_invocation()
        {
            var first = new Game { Type = "first" };
            var second = new Game { Type = "second" };
            var selected = first;
            var entered = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var fixture = CreateFixture(() => selected);
            fixture.Target.SelectedHandler = async () =>
            {
                entered.SetResult(true);
                await release.Task;
                throw new IOException("unavailable");
            };

            var invocation = fixture.Service.SelectedAsync();
            await entered.Task;
            selected = second;
            release.SetResult(true);
            var result = await invocation;

            result.Should().BeFalse();
            fixture.Safety.IsLocked(first).Should().BeTrue();
            fixture.Safety.IsLocked(second).Should().BeFalse();
            fixture.GameService.Verify(service => service.GetSelected(), Times.Once);
        }

        [Fact]
        public async Task Locked_invocation_is_rejected_without_calling_target()
        {
            var game = new Game { Type = "locked" };
            var fixture = CreateFixture(game);
            fixture.Safety.Lock(game, GameStateLockReason.WriteAccessFailure, "test");

            var result = await fixture.Service.SelectedAsync();

            result.Should().BeFalse();
            fixture.Target.SelectedCalls.Should().Be(0);
        }

        [Fact]
        public async Task Missing_selected_game_uses_the_declared_rejection_without_calling_target()
        {
            var fixture = CreateFixture((IGame)null);

            var result = await fixture.Service.SelectedAsync();

            result.Should().BeFalse();
            fixture.Target.SelectedCalls.Should().Be(0);
        }

        [Fact]
        public async Task Standardized_methods_on_each_migrated_contract_are_enforced_by_interface_proxying()
        {
            var game = new Game { Type = "contract-proxies" };
            var fixture = CreateContainerFixture(game);
            var modService = new Mock<IModService>();
            var mergeService = new Mock<IModMergeService>();
            var patchService = new Mock<IModPatchCollectionService>();
            fixture.Container.RegisterInstance(modService.Object);
            fixture.Container.RegisterInstance(mergeService.Object);
            fixture.Container.RegisterInstance(patchService.Object);
            fixture.Container.InterceptGameStateSafetyContracts();
            fixture.Safety.Lock(game, GameStateLockReason.WriteAccessFailure, "test");
            fixture.Container.Verify();

            (await fixture.Container.GetInstance<IModService>().DeleteDescriptorsAsync([])).Should().BeFalse();
            (await fixture.Container.GetInstance<IModMergeService>().MergeCollectionByFilesAsync("collection")).Should().BeNull();
            (await fixture.Container.GetInstance<IModPatchCollectionService>().CopyPatchCollectionAsync("one", "two")).Should().BeFalse();
            modService.Verify(service => service.DeleteDescriptorsAsync(It.IsAny<IEnumerable<IMod>>()), Times.Never);
            mergeService.Verify(service => service.MergeCollectionByFilesAsync(It.IsAny<string>()), Times.Never);
            patchService.Verify(service => service.CopyPatchCollectionAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Owned_install_is_allowed_only_for_the_current_revalidation_generation()
        {
            var game = new Game { Type = "owned-install-proxy" };
            var fixture = CreateContainerFixture(game);
            var modService = new Mock<IModService>();
            var current = fixture.Safety.BeginRevalidation(game, "custom directory changed");
            modService.Setup(service => service.InstallModsAsync(game, It.IsAny<IEnumerable<IMod>>(), current))
                .ReturnsAsync(true);
            fixture.Container.RegisterInstance(modService.Object);
            fixture.Container.InterceptGameStateSafetyContracts();
            fixture.Container.Verify();
            var proxied = fixture.Container.GetInstance<IModService>();

            (await proxied.InstallModsAsync(game, [], current)).Should().BeTrue();
            (await proxied.InstallModsAsync([])).Should().BeNull();

            var stale = current;
            fixture.Safety.BeginRevalidation(game, "newer custom directory");
            (await proxied.InstallModsAsync(game, [], stale)).Should().BeFalse();
            modService.Verify(service => service.InstallModsAsync(game, It.IsAny<IEnumerable<IMod>>(), current), Times.Once);
            modService.Verify(service => service.InstallModsAsync(It.IsAny<IEnumerable<IMod>>()), Times.Never);
        }

        [Fact]
        public async Task Unexpected_async_exception_propagates_without_locking()
        {
            var game = new Game { Type = "unexpected" };
            var fixture = CreateFixture(game, classifyFileSystemFailure: false);
            fixture.Target.SelectedHandler = () => throw new InvalidOperationException("programming defect");

            var action = async () => await fixture.Service.SelectedAsync();

            await action.Should().ThrowAsync<InvalidOperationException>();
            fixture.Safety.IsLocked(game).Should().BeFalse();
        }

        [Fact]
        public async Task Null_and_completed_task_rejections_are_explicitly_supported()
        {
            var game = new Game { Type = "shapes" };
            var fixture = CreateFixture(game);
            fixture.Safety.Lock(game, GameStateLockReason.WriteAccessFailure, "test");

            (await fixture.Service.NullableAsync()).Should().BeNull();
            await fixture.Service.CompletesAsync();
            fixture.Target.NullableCalls.Should().Be(0);
            fixture.Target.CompletedCalls.Should().Be(0);
        }

        [Fact]
        public void Synchronous_rejection_is_supported_for_explicitly_declared_shapes()
        {
            var game = new Game { Type = "sync" };
            var fixture = CreateFixture(game);
            fixture.Safety.Lock(game, GameStateLockReason.WriteAccessFailure, "test");

            fixture.Service.Synchronous().Should().BeFalse();
            fixture.Target.SynchronousCalls.Should().Be(0);
        }

        [Fact]
        public void Synchronous_classified_failure_locks_as_the_explicit_failure_reason()
        {
            var game = new Game { Type = "sync-failure" };
            var fixture = CreateFixture(game);
            fixture.Target.SynchronousHandler = () => throw new UnauthorizedAccessException();

            fixture.Service.Synchronous().Should().BeFalse();
            fixture.Safety.GetLock(game).Reason.Should().Be(GameStateLockReason.DiscoveryUnavailable);
            fixture.Safety.GetLock(game).Context.Should().Be("Synchronous test");
        }

        [Fact]
        public async Task Pre_lock_rejection_and_failure_locking_are_independent_capabilities()
        {
            var game = new Game { Type = "capabilities" };
            var fixture = CreateFixture(game);
            fixture.Target.PreLockOnlyHandler = () => throw new IOException("not intercepted");

            var action = async () => await fixture.Service.PreLockOnlyAsync();
            await action.Should().ThrowAsync<IOException>();
            fixture.Safety.IsLocked(game).Should().BeFalse();

            fixture.Safety.Lock(game, GameStateLockReason.DiscoveryUnavailable, "test");
            (await fixture.Service.FailureOnlyAsync()).Should().BeTrue();
            fixture.Target.FailureOnlyCalls.Should().Be(1);
        }

        [Fact]
        public async Task Invalid_failure_reason_and_enforcement_combinations_are_rejected()
        {
            var fixture = CreateFixture(new Game { Type = "invalid-metadata" });

            var missingReason = async () => await fixture.Service.MissingFailureReasonAsync();
            var unusedReason = async () => await fixture.Service.UnusedFailureReasonAsync();

            await missingReason.Should().ThrowAsync<InvalidOperationException>();
            await unusedReason.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public void Non_virtual_implementation_methods_remain_supported_by_interface_proxying()
        {
            typeof(ModPatchCollectionService).GetMethod(nameof(IModPatchCollectionService.CopyPatchCollectionAsync))
                .IsFinal.Should().BeTrue("the implementation method is not overridable even though CLR interface implementations are virtual-final");
            typeof(IModPatchCollectionService).GetMethod(nameof(IModPatchCollectionService.CopyPatchCollectionAsync))
                .GetCustomAttribute<GameStateSafetyAttribute>().Should().NotBeNull();
        }

        private static SafetyFixture CreateFixture(IGame selectedGame, bool classifyFileSystemFailure = true)
        {
            return CreateFixture(() => selectedGame, classifyFileSystemFailure);
        }

        private static SafetyFixture CreateFixture(Func<IGame> selectedGame, bool classifyFileSystemFailure = true)
        {
            var containerFixture = CreateContainerFixture(selectedGame, classifyFileSystemFailure);
            var target = new InterceptedSafetyTestService();
            containerFixture.Container.RegisterInstance<IInterceptedSafetyTestService>(target);
            containerFixture.Container.InterceptGameStateSafetyContracts();
            containerFixture.Container.Verify();
            return new SafetyFixture(containerFixture.Container.GetInstance<IInterceptedSafetyTestService>(), target,
                containerFixture.Safety, containerFixture.GameService);
        }

        private static ContainerFixture CreateContainerFixture(IGame selectedGame, bool classifyFileSystemFailure = true)
        {
            return CreateContainerFixture(() => selectedGame, classifyFileSystemFailure);
        }

        private static ContainerFixture CreateContainerFixture(Func<IGame> selectedGame, bool classifyFileSystemFailure = true)
        {
            var gameService = new Mock<IGameService>();
            gameService.Setup(service => service.GetSelected()).Returns(selectedGame);
            var probe = new Mock<IFileSystemStateProbe>();
            probe.Setup(value => value.IsFileSystemAccessFailure(It.IsAny<Exception>())).Returns(classifyFileSystemFailure);
            var safety = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var container = new Container();
            container.RegisterInstance<IGameService>(gameService.Object);
            container.RegisterInstance<IGameStateSafetyService>(safety);
            container.RegisterInstance(probe.Object);
            return new ContainerFixture(container, safety, gameService);
        }

        private static void AssertRejectionMatches(MethodInfo method, GameStateSafetyRejection rejection)
        {
            var returnType = method.ReturnType;
            if (returnType == typeof(Task))
            {
                rejection.Should().Be(GameStateSafetyRejection.CompletedTask);
                return;
            }
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                returnType = returnType.GenericTypeArguments[0];
            }
            if (rejection == GameStateSafetyRejection.False)
            {
                returnType.Should().Be(typeof(bool));
            }
            else if (rejection == GameStateSafetyRejection.Null)
            {
                (!returnType.IsValueType || Nullable.GetUnderlyingType(returnType) != null).Should().BeTrue();
            }
        }

        private sealed record SafetyFixture(IInterceptedSafetyTestService Service, InterceptedSafetyTestService Target,
            IGameStateSafetyService Safety, Mock<IGameService> GameService);

        private sealed record ContainerFixture(Container Container, IGameStateSafetyService Safety, Mock<IGameService> GameService);
    }

    [GameStateSafetyContract]
    public interface IInterceptedSafetyTestService
    {
        [GameStateSafety(GameStateSafetyOperation.TrustedRead, GameStateSafetyRejection.False, "Explicit test", GameStateLockReason.DiscoveryUnavailable)]
        Task<bool> ExplicitAsync(IGame game);

        [GameStateSafety(GameStateSafetyOperation.TrustedRead, GameStateSafetyRejection.False, "Selected test", GameStateLockReason.DiscoveryUnavailable)]
        Task<bool> SelectedAsync();

        [GameStateSafety(GameStateSafetyOperation.TrustedRead, GameStateSafetyRejection.Null, "Nullable test", GameStateLockReason.DiscoveryUnavailable)]
        Task<object> NullableAsync();

        [GameStateSafety(GameStateSafetyOperation.Mutation, GameStateSafetyRejection.CompletedTask, "Completed test", GameStateLockReason.WriteAccessFailure)]
        Task CompletesAsync();

        [GameStateSafety(GameStateSafetyOperation.Mutation, GameStateSafetyRejection.False, "Synchronous test", GameStateLockReason.DiscoveryUnavailable)]
        bool Synchronous();

        [GameStateSafety(GameStateSafetyOperation.TrustedRead, GameStateSafetyRejection.False, "Pre-lock-only test",
            GameStateSafetyEnforcement.RejectWhenLocked | GameStateSafetyEnforcement.RejectWhenGameUnavailable)]
        Task<bool> PreLockOnlyAsync();

        [GameStateSafety(GameStateSafetyOperation.TrustedRead, GameStateSafetyRejection.False, "Failure-only test",
            GameStateLockReason.DiscoveryUnavailable,
            GameStateSafetyEnforcement.LockOnFileSystemFailure | GameStateSafetyEnforcement.RejectWhenGameUnavailable)]
        Task<bool> FailureOnlyAsync();

        [GameStateSafety(GameStateSafetyOperation.Mutation, GameStateSafetyRejection.False, "Missing failure reason",
            GameStateSafetyEnforcement.Standard)]
        Task<bool> MissingFailureReasonAsync();

        [GameStateSafety(GameStateSafetyOperation.Mutation, GameStateSafetyRejection.False, "Unused failure reason",
            GameStateLockReason.WriteAccessFailure, GameStateSafetyEnforcement.EntryOnly)]
        Task<bool> UnusedFailureReasonAsync();
    }

    public class InterceptedSafetyTestService : IInterceptedSafetyTestService
    {
        public Func<IGame, Task<bool>> ExplicitHandler { get; set; } = _ => Task.FromResult(true);

        public int ExplicitCalls { get; private set; }

        public Func<Task<bool>> SelectedHandler { get; set; } = () => Task.FromResult(true);

        public int SelectedCalls { get; private set; }

        public int NullableCalls { get; private set; }

        public int CompletedCalls { get; private set; }

        public int SynchronousCalls { get; private set; }

        public Func<bool> SynchronousHandler { get; set; } = () => true;

        public Func<Task<bool>> PreLockOnlyHandler { get; set; } = () => Task.FromResult(true);

        public int FailureOnlyCalls { get; private set; }

        public Task<bool> ExplicitAsync(IGame game)
        {
            ExplicitCalls++;
            return ExplicitHandler(game);
        }

        public Task<bool> SelectedAsync()
        {
            SelectedCalls++;
            return SelectedHandler();
        }

        public Task<object> NullableAsync()
        {
            NullableCalls++;
            return Task.FromResult<object>(new object());
        }

        public Task CompletesAsync()
        {
            CompletedCalls++;
            return Task.CompletedTask;
        }

        public bool Synchronous()
        {
            SynchronousCalls++;
            return SynchronousHandler();
        }

        public Task<bool> PreLockOnlyAsync() => PreLockOnlyHandler();

        public Task<bool> FailureOnlyAsync()
        {
            FailureOnlyCalls++;
            return Task.FromResult(true);
        }

        public Task<bool> MissingFailureReasonAsync() => Task.FromResult(true);

        public Task<bool> UnusedFailureReasonAsync() => Task.FromResult(true);
    }
}
