// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 09-19-2026
//
// Last Modified By : Mario
// Last Modified On : 09-19-2026
// ***********************************************************************
// <copyright file="ConflictSolverExactModSetControlViewModelTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using Castle.DynamicProxy;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Tests.Common;
using IronyModManager.ViewModels.Controls;
using Moq;
using ReactiveUI;
using Xunit;

namespace IronyModManager.Tests.ViewModels.Controls
{
    /// <summary>
    /// Tests the exact-set rule manager's presentation and lifecycle boundary.
    /// </summary>
    public class ConflictSolverExactModSetControlViewModelTests
    {
        public ConflictSolverExactModSetControlViewModelTests()
        {
            DISetup.SetupContainer();
        }

        [Fact]
        public async Task Should_browse_decoded_persisted_rules_in_read_only_without_mutation()
        {
            var service = new Mock<IModPatchCollectionService>();
            service.Setup(p => p.GetExactModSetIgnoreRules(It.IsAny<IConflictResult>())).Returns(
            [
                new[] { "A", "B" },
                new[] { "Some \"Quoted\" Mod", "A \\ Mod" }
            ]);
            var viewModel = new ConflictSolverExactModSetControlViewModel(service.Object);
            var result = new ConflictResult { IgnoredPaths = "original" };

            viewModel.Initialize(true);
            viewModel.SetContext(result, ["A", "B"], "Collection", true);
            viewModel.Open();

            viewModel.IsOpen.Should().BeTrue();
            viewModel.ExactRules.Select(rule => rule.DisplayName).Should().Equal("A + B", "Some \"Quoted\" Mod + A \\ Mod");
            viewModel.CanAdd.Should().BeFalse();
            viewModel.BeginAdd();
            viewModel.IsAdding.Should().BeFalse();
            (await viewModel.SaveAsync()).Should().BeFalse();
            result.IgnoredPaths.Should().Be("original");
            service.Verify(p => p.AddExactModSetToIgnoreList(It.IsAny<IConflictResult>(), It.IsAny<IEnumerable<string>>()), Times.Never);
            service.Verify(p => p.SaveIgnoredPathsAsync(It.IsAny<IConflictResult>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Should_add_selected_canonical_mods_save_and_immediately_refresh_rules()
        {
            var service = new Mock<IModPatchCollectionService>();
            IReadOnlyList<IReadOnlyList<string>> rules = Array.Empty<IReadOnlyList<string>>();
            service.Setup(p => p.GetExactModSetIgnoreRules(It.IsAny<IConflictResult>())).Returns(() => rules);
            service.Setup(p => p.AddExactModSetToIgnoreList(It.IsAny<IConflictResult>(), It.IsAny<IEnumerable<string>>()))
                .Callback<IConflictResult, IEnumerable<string>>((result, names) =>
                {
                    var selected = names.ToArray();
                    result.IgnoredPaths = "modSet-generated-by-service";
                    rules = [selected];
                });
            service.Setup(p => p.SaveIgnoredPathsAsync(It.IsAny<IConflictResult>(), "Collection")).ReturnsAsync(true);
            var viewModel = CreateProxiedViewModel(service.Object);
            using var activation = viewModel.Activator.Activate();
            var result = new ConflictResult();
            var saved = false;
            viewModel.RuleSaved += () => saved = true;

            viewModel.Initialize(false);
            viewModel.SetContext(result, ["Canonical A", "Canonical B"], "Collection", true);
            viewModel.Open();
            viewModel.BeginAdd();
            viewModel.SelectedMods.AddRange(["Canonical A", "Canonical B"]);

            viewModel.IsAdding.Should().BeTrue();
            viewModel.CanSave.Should().BeTrue();
            (await viewModel.SaveAsync()).Should().BeTrue();
            viewModel.IsAdding.Should().BeFalse();
            viewModel.ExactRules.Should().ContainSingle().Which.DisplayName.Should().Be("Canonical A + Canonical B");
            saved.Should().BeTrue();
            service.Verify(p => p.AddExactModSetToIgnoreList(result,
                It.Is<IEnumerable<string>>(names => names.SequenceEqual(new[] { "Canonical A", "Canonical B" }))), Times.Once);
            service.Verify(p => p.SaveIgnoredPathsAsync(result, "Collection"), Times.Once);
        }

        [Fact]
        public async Task Should_return_to_one_existing_rule_when_reversed_selection_is_duplicate()
        {
            var service = new Mock<IModPatchCollectionService>();
            service.Setup(p => p.GetExactModSetIgnoreRules(It.IsAny<IConflictResult>())).Returns([new[] { "A", "B" }]);
            var viewModel = new ConflictSolverExactModSetControlViewModel(service.Object);
            var result = new ConflictResult { IgnoredPaths = "modSet:\"A\",\"B\"" };

            viewModel.Initialize(false);
            viewModel.SetContext(result, ["A", "B"], "Collection", true);
            viewModel.Open();
            viewModel.BeginAdd();
            viewModel.SelectedMods.AddRange(["B", "A"]);

            (await viewModel.SaveAsync()).Should().BeFalse();
            viewModel.IsAdding.Should().BeFalse();
            viewModel.ExactRules.Should().ContainSingle().Which.ModNames.Should().Equal("A", "B");
            result.IgnoredPaths.Should().Be("modSet:\"A\",\"B\"");
            service.Verify(p => p.SaveIgnoredPathsAsync(It.IsAny<IConflictResult>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Should_cancel_add_without_mutating_persisted_rules()
        {
            var service = new Mock<IModPatchCollectionService>();
            service.Setup(p => p.GetExactModSetIgnoreRules(It.IsAny<IConflictResult>())).Returns(Array.Empty<IReadOnlyList<string>>());
            var viewModel = new ConflictSolverExactModSetControlViewModel(service.Object);
            var result = new ConflictResult { IgnoredPaths = "unchanged" };

            viewModel.Initialize(false);
            viewModel.SetContext(result, ["A", "B"], "Collection", true);
            viewModel.Open();
            viewModel.BeginAdd();
            viewModel.SelectedMods.Add("A");
            viewModel.CancelAdd();

            viewModel.IsAdding.Should().BeFalse();
            viewModel.SelectedMods.Should().BeEmpty();
            result.IgnoredPaths.Should().Be("unchanged");
            service.Verify(p => p.AddExactModSetToIgnoreList(It.IsAny<IConflictResult>(), It.IsAny<IEnumerable<string>>()), Times.Never);
        }

        [Fact]
        public async Task Should_delete_selected_rule_refresh_list_clear_selection_and_raise_reevaluation()
        {
            var service = new Mock<IModPatchCollectionService>();
            IReadOnlyList<IReadOnlyList<string>> rules = [new[] { "A", "B" }, new[] { "C", "D" }];
            service.Setup(p => p.GetExactModSetIgnoreRules(It.IsAny<IConflictResult>())).Returns(() => rules);
            service.Setup(p => p.RemoveExactModSetIgnoreRule(It.IsAny<IConflictResult>(), It.IsAny<IEnumerable<string>>()))
                .Callback<IConflictResult, IEnumerable<string>>((result, _) =>
                {
                    result.IgnoredPaths = "modSet:\"C\",\"D\"";
                    rules = [new[] { "C", "D" }];
                }).Returns(true);
            service.Setup(p => p.SaveIgnoredPathsAsync(It.IsAny<IConflictResult>(), "Collection")).ReturnsAsync(true);
            var viewModel = new ConflictSolverExactModSetControlViewModel(service.Object);
            var result = new ConflictResult { IgnoredPaths = "modSet:\"B\",\"A\"" + Environment.NewLine + "modSet:\"C\",\"D\"" };
            var reevaluate = false;
            viewModel.RuleSaved += () => reevaluate = true;

            viewModel.Initialize(false);
            viewModel.SetContext(result, ["A", "B", "C", "D"], "Collection", true);
            viewModel.Open();
            viewModel.SelectedRule = viewModel.ExactRules[0];

            viewModel.CanDelete.Should().BeTrue();
            (await viewModel.DeleteAsync()).Should().BeTrue();
            viewModel.ExactRules.Should().ContainSingle().Which.ModNames.Should().Equal("C", "D");
            viewModel.SelectedRule.Should().BeNull();
            viewModel.CanDelete.Should().BeFalse();
            reevaluate.Should().BeTrue();
            service.Verify(p => p.RemoveExactModSetIgnoreRule(result,
                It.Is<IEnumerable<string>>(names => names.SequenceEqual(new[] { "A", "B" }))), Times.Once);
            service.Verify(p => p.SaveIgnoredPathsAsync(result, "Collection"), Times.Once);
        }

        [Fact]
        public async Task Should_disable_delete_without_selection_and_defensively_block_it_in_read_only_mode()
        {
            var service = new Mock<IModPatchCollectionService>();
            service.Setup(p => p.GetExactModSetIgnoreRules(It.IsAny<IConflictResult>())).Returns([new[] { "A", "B" }]);
            var viewModel = new ConflictSolverExactModSetControlViewModel(service.Object);
            var result = new ConflictResult { IgnoredPaths = "modSet:\"A\",\"B\"" };

            viewModel.Initialize(false);
            viewModel.SetContext(result, ["A", "B"], "Collection", true);
            viewModel.Open();
            viewModel.CanDelete.Should().BeFalse();

            viewModel.Initialize(true);
            viewModel.SetContext(result, ["A", "B"], "Collection", true);
            viewModel.Open();
            viewModel.CanDelete.Should().BeFalse();
            viewModel.SelectedRule = viewModel.ExactRules[0];
            viewModel.CanDelete.Should().BeFalse();

            (await viewModel.DeleteAsync()).Should().BeFalse();
            result.IgnoredPaths.Should().Be("modSet:\"A\",\"B\"");
            service.Verify(p => p.RemoveExactModSetIgnoreRule(It.IsAny<IConflictResult>(), It.IsAny<IEnumerable<string>>()), Times.Never);
            service.Verify(p => p.SaveIgnoredPathsAsync(It.IsAny<IConflictResult>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Should_remain_reentrant_and_clear_only_transient_state()
        {
            var service = new Mock<IModPatchCollectionService>();
            service.Setup(p => p.GetExactModSetIgnoreRules(It.IsAny<IConflictResult>())).Returns([new[] { "A", "B" }]);
            var viewModel = CreateProxiedViewModel(service.Object);
            using var activation = viewModel.Activator.Activate();
            var result = new ConflictResult { IgnoredPaths = "persisted" };

            foreach (var readOnly in new[] { true, false, true, false })
            {
                viewModel.Initialize(readOnly);
                viewModel.SetContext(result, ["A", "B"], "Collection", true);
                viewModel.CanOpen.Should().BeTrue();
                viewModel.Open();
                viewModel.ExactRules.Should().ContainSingle();
                if (!readOnly)
                {
                    viewModel.BeginAdd();
                    viewModel.SelectedMods.Add("A");
                }
                viewModel.ClearSurface();
                viewModel.IsOpen.Should().BeFalse();
                viewModel.IsAdding.Should().BeFalse();
                viewModel.SelectedMods.Should().BeEmpty();
                result.IgnoredPaths.Should().Be("persisted");
            }
        }

        [Fact]
        public async Task Should_not_bind_command_availability_to_worker_thread_read_only_changes()
        {
            var viewModel = CreateProxiedViewModel(Mock.Of<IModPatchCollectionService>());
            using var activation = viewModel.Activator.Activate();

            await Task.Run(() => viewModel.Initialize(true));

            ((System.Windows.Input.ICommand)viewModel.OpenCommand).CanExecute(null).Should().BeTrue();
            ((System.Windows.Input.ICommand)viewModel.AddCommand).CanExecute(null).Should().BeTrue();
            ((System.Windows.Input.ICommand)viewModel.DeleteCommand).CanExecute(null).Should().BeTrue();
            ((System.Windows.Input.ICommand)viewModel.SaveCommand).CanExecute(null).Should().BeTrue();
            ((System.Windows.Input.ICommand)viewModel.CancelCommand).CanExecute(null).Should().BeTrue();
            ((System.Windows.Input.ICommand)viewModel.CloseCommand).CanExecute(null).Should().BeTrue();
        }

        private static ConflictSolverExactModSetControlViewModel CreateProxiedViewModel(IModPatchCollectionService service)
        {
            return new ProxyGenerator().CreateClassProxy<ConflictSolverExactModSetControlViewModel>([service], new ProceedingInterceptor());
        }

        private sealed class ProceedingInterceptor : IInterceptor
        {
            public void Intercept(Castle.DynamicProxy.IInvocation invocation)
            {
                invocation.Proceed();
            }
        }
    }
}
