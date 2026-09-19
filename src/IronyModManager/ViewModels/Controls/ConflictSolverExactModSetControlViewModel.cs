// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-19-2026
//
// Last Modified By : Mario
// Last Modified On : 09-19-2026
// ***********************************************************************
// <copyright file="ConflictSolverExactModSetControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Collections;
using IronyModManager.Common.ViewModels;
using IronyModManager.Localization;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Browses and creates persisted exact participating-mod set rules.
    /// </summary>
    public class ConflictSolverExactModSetControlViewModel(IModPatchCollectionService modPatchCollectionService) : BaseViewModel
    {
        private IConflictResult conflictResult;
        private string collectionName;
        private ExactModSetRuleItem selectedRule;

        /// <summary>
        /// Occurs after an exact-set rule has been persisted.
        /// </summary>
        public event Action RuleSaved;

        /// <summary>
        /// Gets the add label.
        /// </summary>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ExactModSet.Add)]
        public virtual string Add { get; protected set; }

        /// <summary>
        /// Gets the command that enters rule construction mode.
        /// </summary>
        public virtual ReactiveCommand<Unit, Unit> AddCommand { get; protected set; }

        /// <summary>
        /// Gets the available canonical mod names for rule construction.
        /// </summary>
        public virtual AvaloniaList<string> AvailableMods { get; protected set; } = [];

        /// <summary>
        /// Gets the available mods label.
        /// </summary>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ExactModSet.AvailableMods)]
        public virtual string AvailableModsTitle { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether rule construction may be entered.
        /// </summary>
        public virtual bool CanAdd { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the selected persisted rule may be deleted.
        /// </summary>
        public virtual bool CanDelete { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the rule manager may be opened.
        /// </summary>
        public virtual bool CanOpen { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the transient selection may be saved.
        /// </summary>
        public virtual bool CanSave { get; protected set; }

        /// <summary>
        /// Gets the cancel label.
        /// </summary>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ExactModSet.Cancel)]
        public virtual string Cancel { get; protected set; }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public virtual ReactiveCommand<Unit, Unit> CancelCommand { get; protected set; }

        /// <summary>
        /// Gets the close label.
        /// </summary>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ExactModSet.Close)]
        public virtual string Close { get; protected set; }

        /// <summary>
        /// Gets the close command.
        /// </summary>
        public virtual ReactiveCommand<Unit, Unit> CloseCommand { get; protected set; }

        /// <summary>
        /// Gets the delete label.
        /// </summary>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ExactModSet.Delete)]
        public virtual string Delete { get; protected set; }

        /// <summary>
        /// Gets the delete command.
        /// </summary>
        public virtual ReactiveCommand<Unit, Unit> DeleteCommand { get; protected set; }

        /// <summary>
        /// Gets the decoded valid persisted exact-set rules.
        /// </summary>
        public virtual IReadOnlyList<ExactModSetRuleItem> ExactRules { get; protected set; } = Array.Empty<ExactModSetRuleItem>();

        /// <summary>
        /// Gets the existing rules label.
        /// </summary>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ExactModSet.ExistingRules)]
        public virtual string ExistingRulesTitle { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the rule construction state is active.
        /// </summary>
        public virtual bool IsAdding { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the popup is open.
        /// </summary>
        public virtual bool IsOpen { get; protected set; }

        /// <summary>
        /// Gets the command that opens the rule manager.
        /// </summary>
        public virtual ReactiveCommand<Unit, Unit> OpenCommand { get; protected set; }

        /// <summary>
        /// Gets the preview-only label.
        /// </summary>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ExactModSet.PreviewOnly)]
        public virtual string PreviewOnly { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the solver is read-only.
        /// </summary>
        public virtual bool ReadOnly { get; protected set; }

        /// <summary>
        /// Gets the save label.
        /// </summary>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ExactModSet.Save)]
        public virtual string Save { get; protected set; }

        /// <summary>
        /// Gets the save command.
        /// </summary>
        public virtual ReactiveCommand<Unit, Unit> SaveCommand { get; protected set; }

        /// <summary>
        /// Gets the transient canonical mod selection used to construct a rule.
        /// </summary>
        public virtual AvaloniaList<string> SelectedMods { get; protected set; } = [];

        /// <summary>
        /// Gets or sets the selected persisted exact-set rule.
        /// </summary>
        public virtual ExactModSetRuleItem SelectedRule
        {
            get => selectedRule;
            set
            {
                selectedRule = value;
                UpdateAvailability();
            }
        }

        /// <summary>
        /// Gets the surface title.
        /// </summary>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ExactModSet.Title)]
        public virtual string Title { get; protected set; }

        /// <summary>
        /// Closes and clears this feature's transient presentation state.
        /// </summary>
        public virtual void ClearSurface()
        {
            IsOpen = false;
            IsAdding = false;
            SelectedRule = null;
            SelectedMods.Clear();
            ExactRules = Array.Empty<ExactModSetRuleItem>();
            UpdateAvailability();
        }

        /// <summary>
        /// Initializes this feature for a new Conflict Solver session.
        /// </summary>
        /// <param name="readOnly">Whether the session is read-only.</param>
        public virtual void Initialize(bool readOnly)
        {
            ReadOnly = readOnly;
            conflictResult = null;
            collectionName = null;
            AvailableMods.Clear();
            CanOpen = false;
            ClearSurface();
        }

        /// <summary>
        /// Opens the manager and rebuilds persisted rule presentation from the source text.
        /// </summary>
        public virtual void Open()
        {
            if (!CanOpen)
            {
                return;
            }

            IsAdding = false;
            SelectedMods.Clear();
            RefreshRules();
            IsOpen = true;
            UpdateAvailability();
        }

        /// <summary>
        /// Sets the current Conflict Solver source context.
        /// </summary>
        /// <param name="result">The current conflict result.</param>
        /// <param name="activeMods">The active canonical mod names available for rule construction.</param>
        /// <param name="currentCollectionName">The current collection name.</param>
        /// <param name="solverAvailable">Whether the rule manager may use the current solver result.</param>
        public virtual void SetContext(IConflictResult result, IEnumerable<string> activeMods, string currentCollectionName, bool solverAvailable)
        {
            if (IsOpen && (!ReferenceEquals(conflictResult, result) ||
                           !string.Equals(collectionName, currentCollectionName, StringComparison.Ordinal) || !solverAvailable))
            {
                ClearSurface();
            }

            conflictResult = result;
            collectionName = currentCollectionName;
            AvailableMods.Clear();
            AvailableMods.AddRange((activeMods ?? []).Where(name => !string.IsNullOrEmpty(name)).Distinct(StringComparer.Ordinal));
            CanOpen = solverAvailable && conflictResult != null && !string.IsNullOrWhiteSpace(collectionName);
            UpdateAvailability();
        }

        /// <summary>
        /// Enters rule construction mode.
        /// </summary>
        public virtual void BeginAdd()
        {
            if (!CanAdd)
            {
                return;
            }

            SelectedMods.Clear();
            IsAdding = true;
            UpdateAvailability();
        }

        /// <summary>
        /// Cancels rule construction without changing persisted text.
        /// </summary>
        public virtual void CancelAdd()
        {
            SelectedMods.Clear();
            IsAdding = false;
            UpdateAvailability();
        }

        /// <summary>
        /// Saves the transient exact-set selection.
        /// </summary>
        /// <returns><c>true</c> when a new rule was persisted; otherwise, <c>false</c>.</returns>
        public virtual async Task<bool> SaveAsync()
        {
            var selectedNames = SelectedMods.Distinct(StringComparer.Ordinal).ToArray();
            if (ReadOnly || !IsOpen || !IsAdding || conflictResult == null || selectedNames.Length == 0 || string.IsNullOrWhiteSpace(collectionName))
            {
                return false;
            }

            var previousIgnoredPaths = conflictResult.IgnoredPaths ?? string.Empty;
            modPatchCollectionService.AddExactModSetToIgnoreList(conflictResult, selectedNames);
            if (!previousIgnoredPaths.Equals(conflictResult.IgnoredPaths ?? string.Empty, StringComparison.Ordinal) &&
                !await modPatchCollectionService.SaveIgnoredPathsAsync(conflictResult, collectionName))
            {
                conflictResult.IgnoredPaths = previousIgnoredPaths;
                return false;
            }

            var added = !previousIgnoredPaths.Equals(conflictResult.IgnoredPaths ?? string.Empty, StringComparison.Ordinal);
            CancelAdd();
            RefreshRules();
            if (added)
            {
                RuleSaved?.Invoke();
            }
            return added;
        }

        /// <summary>
        /// Deletes the selected persisted exact-set rule.
        /// </summary>
        /// <returns><c>true</c> when the rule was removed and persisted; otherwise, <c>false</c>.</returns>
        public virtual async Task<bool> DeleteAsync()
        {
            var rule = SelectedRule;
            if (ReadOnly || !IsOpen || IsAdding || conflictResult == null || rule == null || string.IsNullOrWhiteSpace(collectionName))
            {
                return false;
            }

            var previousIgnoredPaths = conflictResult.IgnoredPaths ?? string.Empty;
            if (!modPatchCollectionService.RemoveExactModSetIgnoreRule(conflictResult, rule.ModNames))
            {
                RefreshRules();
                return false;
            }

            if (!await modPatchCollectionService.SaveIgnoredPathsAsync(conflictResult, collectionName))
            {
                conflictResult.IgnoredPaths = previousIgnoredPaths;
                RefreshRules();
                return false;
            }

            RefreshRules();
            RuleSaved?.Invoke();
            return true;
        }

        /// <inheritdoc />
        protected override void OnActivated(CompositeDisposable disposables)
        {
            // Commands intentionally have no reactive CanExecute chain. UI-thread-owned properties
            // express presentation availability without raising Button.CanExecuteChanged off-thread.
            OpenCommand = ReactiveCommand.Create(Open).DisposeWith(disposables);
            AddCommand = ReactiveCommand.Create(BeginAdd).DisposeWith(disposables);
            DeleteCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await DeleteAsync();
            }).DisposeWith(disposables);
            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await SaveAsync();
            }).DisposeWith(disposables);
            CancelCommand = ReactiveCommand.Create(CancelAdd).DisposeWith(disposables);
            CloseCommand = ReactiveCommand.Create(ClearSurface).DisposeWith(disposables);
            SelectedMods.CollectionChanged += SelectedModsCollectionChanged;
            Disposable.Create(() => SelectedMods.CollectionChanged -= SelectedModsCollectionChanged).DisposeWith(disposables);
            base.OnActivated(disposables);
        }

        private void RefreshRules()
        {
            SelectedRule = null;
            ExactRules = (modPatchCollectionService.GetExactModSetIgnoreRules(conflictResult) ?? Array.Empty<IReadOnlyList<string>>())
                .Select(rule => new ExactModSetRuleItem(rule)).ToArray();
        }

        private void SelectedModsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateAvailability();
        }

        private void UpdateAvailability()
        {
            CanAdd = IsOpen && !IsAdding && !ReadOnly && AvailableMods.Count > 0;
            CanDelete = IsOpen && !IsAdding && !ReadOnly && SelectedRule != null;
            CanSave = IsOpen && IsAdding && !ReadOnly && SelectedMods.Count > 0;
        }
    }

    /// <summary>
    /// Presents one decoded logical exact-mod-set rule.
    /// </summary>
    public sealed class ExactModSetRuleItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExactModSetRuleItem"/> class.
        /// </summary>
        /// <param name="modNames">The decoded canonical mod names.</param>
        public ExactModSetRuleItem(IReadOnlyList<string> modNames)
        {
            ModNames = modNames ?? Array.Empty<string>();
            DisplayName = string.Join(" + ", ModNames);
        }

        /// <summary>
        /// Gets the readable rule representation.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Gets the decoded canonical mod names.
        /// </summary>
        public IReadOnlyList<string> ModNames { get; }
    }
}
