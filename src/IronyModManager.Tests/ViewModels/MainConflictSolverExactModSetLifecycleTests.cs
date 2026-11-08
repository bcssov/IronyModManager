// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 09-19-2026
//
// Last Modified By : Mario
// Last Modified On : 09-19-2026
// ***********************************************************************
// <copyright file="MainConflictSolverExactModSetLifecycleTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using Castle.DynamicProxy;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.AppState;
using IronyModManager.Implementation.Hotkey;
using IronyModManager.Implementation.Overlay;
using IronyModManager.Localization;
using IronyModManager.Localization.Attributes.Handlers;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using IronyModManager.Tests.Common;
using IronyModManager.ViewModels;
using IronyModManager.ViewModels.Controls;
using Moq;
using ReactiveUI;
using Xunit;

namespace IronyModManager.Tests.ViewModels
{
    /// <summary>
    /// Exercises the production Main Conflict Solver ordering used by MainWindow navigation.
    /// </summary>
    public class MainConflictSolverExactModSetLifecycleTests
    {
        public MainConflictSolverExactModSetLifecycleTests()
        {
            DISetup.SetupContainer();
        }

        [Fact]
        public async Task Analysis_back_advanced_should_rebuild_exact_set_availability_after_initialize()
        {
            var fixture = CreateFixture();
            fixture.Solver.SelectedModCollection = fixture.Collection;
            using var exactActivation = fixture.ExactModSet.Activator.Activate();
            using var solverActivation = fixture.Solver.Activator.Activate();
            var firstResult = CreateConflictResult();
            var firstSelection = CreateSelection("A", "B");

            // MainWindow assigns collection/result first. On the first visible entry Avalonia then
            // establishes the list selections after InitializeAsync.
            SetNavigationInput(fixture.Solver, fixture.Collection, firstResult);
            await fixture.Solver.InitializeAsync(true);
            SelectConflict(fixture.Solver, firstSelection.Parent, firstSelection.Child);
            fixture.ExactModSet.CanOpen.Should().BeTrue();
            fixture.ExactModSet.Open();
            fixture.ExactModSet.ClearSurface();
            fixture.Solver.SelectedParentConflict.Should().BeSameAs(firstSelection.Parent);
            fixture.Solver.SelectedConflict.Should().BeSameAs(firstSelection.Child);

            await fixture.Solver.BackCommand.Execute().LastAsync();

            var secondResult = CreateConflictResult();
            var secondSelection = CreateSelection("Conflict A", "Conflict B");

            // On re-entry the already-created controls restore/default their selection while the
            // Conflicts assignment is being processed, before MainWindow invokes InitializeAsync.
            SetNavigationInput(fixture.Solver, fixture.Collection, secondResult);
            SelectConflict(fixture.Solver, secondSelection.Parent, secondSelection.Child);
            await fixture.Solver.InitializeAsync(false);

            fixture.Solver.ReadOnly.Should().BeFalse();
            fixture.Solver.IsConflictSolverAvailable.Should().BeTrue();
            fixture.Solver.ResolvingConflict.Should().BeFalse();
            fixture.Solver.SelectedParentConflict.Should().BeSameAs(secondSelection.Parent);
            fixture.Solver.SelectedConflict.Should().BeSameAs(secondSelection.Child);
            fixture.Solver.SelectedConflict.Mods.Should().Equal("Conflict A", "Conflict B");
            fixture.ExactModSet.AvailableMods.Should().Equal("A", "B");
            fixture.ExactModSet.IsOpen.Should().BeFalse();
            fixture.ExactModSet.CanOpen.Should().BeTrue();
        }

        [Fact]
        public async Task Advanced_back_advanced_should_rebuild_exact_set_availability_after_initialize()
        {
            var fixture = CreateFixture();
            fixture.Solver.SelectedModCollection = fixture.Collection;
            using var exactActivation = fixture.ExactModSet.Activator.Activate();
            using var solverActivation = fixture.Solver.Activator.Activate();

            await EnterFirstSessionAsync(fixture, false);
            await fixture.Solver.BackCommand.Execute().LastAsync();
            await EnterRestoredSessionAsync(fixture, false);

            fixture.ExactModSet.CanOpen.Should().BeTrue();
        }

        [Fact]
        public async Task Repeated_analysis_and_advanced_sessions_should_keep_exact_set_preview_available()
        {
            var fixture = CreateFixture();
            fixture.Solver.SelectedModCollection = fixture.Collection;
            using var exactActivation = fixture.ExactModSet.Activator.Activate();
            using var solverActivation = fixture.Solver.Activator.Activate();

            await EnterFirstSessionAsync(fixture, true);
            foreach (var readOnly in new[] { false, true, false })
            {
                await fixture.Solver.BackCommand.Execute().LastAsync();
                await EnterRestoredSessionAsync(fixture, readOnly);
                fixture.ExactModSet.CanOpen.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Deleting_an_exact_set_rule_should_request_normal_conflict_reevaluation()
        {
            var fixture = CreateFixture();
            fixture.Solver.SelectedModCollection = fixture.Collection;
            using var exactActivation = fixture.ExactModSet.Activator.Activate();
            using var solverActivation = fixture.Solver.Activator.Activate();
            var result = CreateConflictResult();
            result.IgnoredPaths = "modSet:\"A\",\"B\"";

            SetNavigationInput(fixture.Solver, fixture.Collection, result);
            await fixture.Solver.InitializeAsync(false);
            fixture.ExactModSet.Open();
            fixture.ExactModSet.SelectedRule = fixture.ExactModSet.ExactRules.Single();
            var previousFilterCalls = fixture.FilterInterceptor.FilterCalls;

            (await fixture.ExactModSet.DeleteAsync()).Should().BeTrue();

            result.IgnoredPaths.Should().BeEmpty();
            fixture.ExactModSet.ExactRules.Should().BeEmpty();
            fixture.FilterInterceptor.FilterCalls.Should().BeGreaterThan(previousFilterCalls);
        }

        private static Fixture CreateFixture()
        {
            var patchService = new Mock<IModPatchCollectionService>();
            patchService.Setup(p => p.GetExactModSetIgnoreRules(It.IsAny<IConflictResult>())).Returns<IConflictResult>(result =>
                result?.IgnoredPaths?.Contains("modSet:", StringComparison.Ordinal) == true
                    ? [new[] { "A", "B" }]
                    : Array.Empty<IReadOnlyList<string>>());
            patchService.Setup(p => p.RemoveExactModSetIgnoreRule(It.IsAny<IConflictResult>(), It.IsAny<IEnumerable<string>>()))
                .Callback<IConflictResult, IEnumerable<string>>((result, _) => result.IgnoredPaths = string.Empty).Returns(true);
            patchService.Setup(p => p.SaveIgnoredPathsAsync(It.IsAny<IConflictResult>(), It.IsAny<string>())).ReturnsAsync(true);
            var localizationManager = new Mock<ILocalizationManager>();
            var logger = Mock.Of<ILogger>();
            var notificationAction = Mock.Of<INotificationAction>();
            var appAction = Mock.Of<IAppAction>();
            var hotkeyHandler = new ConflictSolverViewHotkeyPressedHandler();
            var idGenerator = new Mock<IIDGenerator>();
            idGenerator.Setup(p => p.GetNextId()).Returns(1);

            var exactModSet = CreateLocalizedProxy<ConflictSolverExactModSetControlViewModel>([patchService.Object]);
            var mergeViewer = new Mock<MergeViewerControlViewModel>(MockBehavior.Loose,
                Mock.Of<IAppStateService>(), Mock.Of<IScrollState>(), patchService.Object, hotkeyHandler, appAction,
                Mock.Of<IExternalEditorService>(), notificationAction, localizationManager.Object).Object;
            var binaryMergeViewer = new Mock<MergeViewerBinaryControlViewModel>(MockBehavior.Loose,
                logger, Mock.Of<IModService>(), localizationManager.Object).Object;
            var modCompareSelector = new Mock<ModCompareSelectorControlViewModel>(MockBehavior.Loose,
                hotkeyHandler, appAction, patchService.Object).Object;
            var ignoreRules = new Mock<ModConflictIgnoreControlViewModel>(MockBehavior.Loose, patchService.Object).Object;
            var modFilter = new Mock<ConflictSolverModFilterControlViewModel>(MockBehavior.Loose, patchService.Object).Object;
            var resetConflicts = new Mock<ConflictSolverResetConflictsControlViewModel>(MockBehavior.Loose,
                localizationManager.Object, idGenerator.Object, patchService.Object).Object;
            var databaseSearch = new Mock<ConflictSolverDBSearchControlViewModel>(MockBehavior.Loose).Object;
            var customConflicts = new Mock<ConflictSolverCustomConflictsControlViewModel>(MockBehavior.Loose,
                notificationAction, localizationManager.Object, patchService.Object).Object;

            var constructorArguments = new object[]
            {
                Mock.Of<IExternalProcessHandlerService>(), hotkeyHandler, idGenerator.Object, patchService.Object,
                localizationManager.Object, mergeViewer, binaryMergeViewer, modCompareSelector, ignoreRules, modFilter,
                exactModSet, resetConflicts, databaseSearch, customConflicts, logger, notificationAction, appAction
            };
            var filterInterceptor = new SuppressConflictFilteringInterceptor();
            var solver = new ProxyGenerator().CreateClassProxy<MainConflictSolverControlViewModel>(constructorArguments,
                filterInterceptor, CreateLocalizationInterceptor<MainConflictSolverControlViewModel>());
            var collection = new Mock<IModCollection>();
            collection.SetupGet(p => p.Name).Returns("Collection");
            return new Fixture(solver, exactModSet, collection.Object, filterInterceptor);
        }

        private static T CreateLocalizedProxy<T>(object[] constructorArguments)
            where T : class, ILocalizableModel
        {
            return new ProxyGenerator().CreateClassProxy<T>(constructorArguments, CreateLocalizationInterceptor<T>());
        }

        private static LocalizationInterceptor<T> CreateLocalizationInterceptor<T>()
            where T : class, ILocalizableModel
        {
            return new LocalizationInterceptor<T>(Array.Empty<ILocalizationAttributeHandler>(), Array.Empty<ILocalizationRefreshHandler>());
        }

        private static IConflictResult CreateConflictResult()
        {
            var definitions = new Mock<IIndexedDefinitions>();
            definitions.Setup(p => p.HasResetDefinitionsAsync()).ReturnsAsync(false);
            definitions.Setup(p => p.GetByTypeAndIdAsync(It.IsAny<string>())).ReturnsAsync(Array.Empty<IDefinition>());
            var result = new Mock<IConflictResult>();
            result.SetupProperty(p => p.IgnoredPaths, string.Empty);
            result.SetupGet(p => p.Conflicts).Returns(definitions.Object);
            return result.Object;
        }

        private static (IHierarchicalDefinitions Parent, IHierarchicalDefinitions Child) CreateSelection(params string[] participants)
        {
            var child = new Mock<IHierarchicalDefinitions>();
            child.SetupGet(p => p.Key).Returns("type:id");
            child.SetupGet(p => p.Mods).Returns(participants.ToList());
            var parent = new Mock<IHierarchicalDefinitions>();
            parent.SetupGet(p => p.Key).Returns("parent");
            parent.SetupGet(p => p.Name).Returns("parent");
            parent.SetupGet(p => p.Children).Returns(new List<IHierarchicalDefinitions> { child.Object });
            return (parent.Object, child.Object);
        }

        private static void SetNavigationInput(MainConflictSolverControlViewModel solver, IModCollection collection, IConflictResult result)
        {
            solver.SelectedModCollection = collection;
            solver.SelectedModsOrder = ["A", "B"];
            solver.Conflicts = result;
        }

        private static void SelectConflict(MainConflictSolverControlViewModel solver, IHierarchicalDefinitions parent, IHierarchicalDefinitions child)
        {
            solver.SelectedParentConflict = parent;
            solver.SelectedConflict = child;
        }

        private static async Task EnterFirstSessionAsync(Fixture fixture, bool readOnly)
        {
            var selection = CreateSelection("A", "B");
            SetNavigationInput(fixture.Solver, fixture.Collection, CreateConflictResult());
            await fixture.Solver.InitializeAsync(readOnly);
            SelectConflict(fixture.Solver, selection.Parent, selection.Child);
            fixture.ExactModSet.CanOpen.Should().BeTrue();
        }

        private static async Task EnterRestoredSessionAsync(Fixture fixture, bool readOnly)
        {
            var selection = CreateSelection("A", "B");
            SetNavigationInput(fixture.Solver, fixture.Collection, CreateConflictResult());
            SelectConflict(fixture.Solver, selection.Parent, selection.Child);
            await fixture.Solver.InitializeAsync(readOnly);
        }

        private sealed record Fixture(MainConflictSolverControlViewModel Solver,
            ConflictSolverExactModSetControlViewModel ExactModSet, IModCollection Collection,
            SuppressConflictFilteringInterceptor FilterInterceptor);

        private sealed class SuppressConflictFilteringInterceptor : IInterceptor
        {
            public int FilterCalls { get; private set; }

            public void Intercept(Castle.DynamicProxy.IInvocation invocation)
            {
                if (invocation.Method.Name.Equals("FilterHierarchicalConflictsAsync", StringComparison.Ordinal))
                {
                    FilterCalls++;
                    invocation.ReturnValue = Task.CompletedTask;
                    return;
                }

                invocation.Proceed();
            }
        }
    }
}
