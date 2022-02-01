// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 06-08-2020
//
// Last Modified By : Mario
// Last Modified On : 02-01-2022
// ***********************************************************************
// <copyright file="ConflictSolverModFilterControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Threading;
using IronyModManager.Common;
using IronyModManager.Common.ViewModels;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class ConflictSolverModFilterControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ConflictSolverModFilterControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The mod patch collection service
        /// </summary>
        private readonly IModPatchCollectionService modPatchCollectionService;

        /// <summary>
        /// The collection name
        /// </summary>
        private string collectionName;

        /// <summary>
        /// The ignored paths
        /// </summary>
        private IDisposable isOpen;

        /// <summary>
        /// The previous ignored path
        /// </summary>
        private string previousIgnoredPath;

        /// <summary>
        /// The previous ignore game mods
        /// </summary>
        private bool previousIgnoreGameMods;

        /// <summary>
        /// The previous show reset conflicts
        /// </summary>
        private bool previousShowResetConflicts;

        /// <summary>
        /// The previous show self conflicts
        /// </summary>
        private bool previousShowSelfConflicts;

        /// <summary>
        /// The setting values
        /// </summary>
        private bool settingValues = false;

        /// <summary>
        /// The should toggle game mods
        /// </summary>
        private bool shouldToggleGameMods;

        /// <summary>
        /// The should toggle self conflicts
        /// </summary>
        private bool shouldToggleSelfConflicts;

        /// <summary>
        /// The should toggle show reset conflicts
        /// </summary>
        private bool shouldToggleShowResetConflicts;

        /// <summary>
        /// The syncing selected mods
        /// </summary>
        private CancellationTokenSource syncingToken;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictSolverModFilterControlViewModel" /> class.
        /// </summary>
        /// <param name="modPatchCollectionService">The mod patch collection service.</param>
        public ConflictSolverModFilterControlViewModel(IModPatchCollectionService modPatchCollectionService)
        {
            this.modPatchCollectionService = modPatchCollectionService;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the close.
        /// </summary>
        /// <value>The close.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ModFilter.Close)]
        public virtual string Close { get; protected set; }

        /// <summary>
        /// Gets or sets the close command.
        /// </summary>
        /// <value>The close command.</value>
        public virtual ReactiveCommand<Unit, Unit> CloseCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the conflict result.
        /// </summary>
        /// <value>The conflict result.</value>
        public virtual IConflictResult ConflictResult { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has saved state.
        /// </summary>
        /// <value><c>true</c> if this instance has saved state; otherwise, <c>false</c>.</value>
        public virtual bool HasSavedState { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [ignore game mods].
        /// </summary>
        /// <value><c>true</c> if [ignore game mods]; otherwise, <c>false</c>.</value>
        public virtual bool IgnoreGameMods { get; protected set; }

        /// <summary>
        /// Gets or sets the ignore game mods title.
        /// </summary>
        /// <value>The ignore game mods title.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ModFilter.IgnoreGameConflicts)]
        public virtual string IgnoreGameModsTitle { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public virtual bool IsOpen { get; protected set; }

        /// <summary>
        /// Gets or sets the mod filter.
        /// </summary>
        /// <value>The mod filter.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ModFilter.Title)]
        public virtual string ModFilter { get; protected set; }

        /// <summary>
        /// Gets or sets the mod filter command.
        /// </summary>
        /// <value>The mod filter command.</value>
        public virtual ReactiveCommand<Unit, Unit> ModFilterCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the mods.
        /// </summary>
        /// <value>The mods.</value>
        public virtual IAvaloniaList<Mod> Mods { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [reset view].
        /// </summary>
        /// <value><c>true</c> if [reset view]; otherwise, <c>false</c>.</value>
        public virtual bool ResetView { get; protected set; }

        /// <summary>
        /// Gets or sets the selected mods.
        /// </summary>
        /// <value>The selected mods.</value>
        public virtual IAvaloniaList<Mod> SelectedMods { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show reset conflicts].
        /// </summary>
        /// <value><c>true</c> if [show reset conflicts]; otherwise, <c>false</c>.</value>
        public virtual bool ShowResetConflicts { get; protected set; }

        /// <summary>
        /// Gets or sets the show reset conflicts title.
        /// </summary>
        /// <value>The show reset conflicts title.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ModFilter.ShowResetConflicts)]
        public virtual string ShowResetConflictsTitle { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show self conflicts].
        /// </summary>
        /// <value><c>true</c> if [show self conflicts]; otherwise, <c>false</c>.</value>
        public virtual bool ShowSelfConflicts { get; protected set; }

        /// <summary>
        /// Gets or sets the show self conflicts title.
        /// </summary>
        /// <value>The show self conflicts title.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ModFilter.ShowSelfConflicts)]
        public virtual string ShowSelfConflictsTitle { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Forces the close popup.
        /// </summary>
        public virtual void ForceClosePopup()
        {
            IsOpen = false;
        }

        /// <summary>
        /// Sets the conflict result.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="activeMods">The active mods.</param>
        /// <param name="collectionName">Name of the collection.</param>
        public virtual void SetConflictResult(IConflictResult conflictResult, IReadOnlyList<string> activeMods, string collectionName)
        {
            async Task valuesSet()
            {
                await Task.Delay(50);
                settingValues = false;
            }
            this.collectionName = collectionName;
            ConflictResult = conflictResult;
            previousIgnoredPath = conflictResult?.IgnoredPaths ?? string.Empty;
            isOpen?.Dispose();
            isOpen = this.WhenAnyValue(p => p.IsOpen).Where(p => p).Subscribe(s =>
            {
                settingValues = true;
                Mods.Clear();
                SelectedMods.Clear();
                Mods.AddRange(activeMods.Select(p => new Mod() { Name = p }));
                var ignoredMods = modPatchCollectionService.GetIgnoredMods(ConflictResult).Select(p => new Mod() { Name = p }).ToList();
                var selectedMods = Mods.ToList().Except(ignoredMods);
                SelectedMods.AddRange(selectedMods);
                shouldToggleGameMods = false;
                previousIgnoreGameMods = IgnoreGameMods = modPatchCollectionService.ShouldIgnoreGameMods(conflictResult).GetValueOrDefault();
                previousShowSelfConflicts = ShowSelfConflicts = modPatchCollectionService.ShouldShowSelfConflicts(conflictResult).GetValueOrDefault();
                previousShowResetConflicts = ShowResetConflicts = modPatchCollectionService.ShouldShowResetConflicts(conflictResult).GetValueOrDefault();
                ResetView = false;
                ResetView = true;
                valuesSet().ConfigureAwait(false);
            }).DisposeWith(Disposables);
        }

        /// <summary>
        /// Ignores the mods.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected async Task IgnoreModsAsync(CancellationToken token)
        {
            if (ConflictResult == null)
            {
                return;
            }
            await Task.Delay(100, token);
            if (!token.IsCancellationRequested)
            {
                await Dispatcher.UIThread.SafeInvokeAsync(async () =>
                {
                    var mods = Mods.Except(SelectedMods).ToList();
                    modPatchCollectionService.AddModsToIgnoreList(ConflictResult, mods.Select(p => p.Name).ToList());
                    if (shouldToggleGameMods)
                    {
                        modPatchCollectionService.ToggleIgnoreGameMods(ConflictResult);
                        shouldToggleGameMods = false;
                    }
                    if (shouldToggleSelfConflicts)
                    {
                        modPatchCollectionService.ToggleSelfModConflicts(ConflictResult);
                        shouldToggleSelfConflicts = false;
                    }
                    if (shouldToggleShowResetConflicts)
                    {
                        modPatchCollectionService.ToggleShowResetConflicts(ConflictResult);
                        shouldToggleShowResetConflicts = false;
                    }
                    if (previousIgnoredPath == null)
                    {
                        previousIgnoredPath = string.Empty;
                    }
                    if (previousIgnoredPath.Equals(ConflictResult.IgnoredPaths ?? string.Empty))
                    {
                        return;
                    }
                    await modPatchCollectionService.SaveIgnoredPathsAsync(ConflictResult, collectionName);
                    IgnoreGameMods = modPatchCollectionService.ShouldIgnoreGameMods(ConflictResult).GetValueOrDefault();
                    ShowSelfConflicts = modPatchCollectionService.ShouldShowSelfConflicts(ConflictResult).GetValueOrDefault();
                    ShowResetConflicts = modPatchCollectionService.ShouldShowResetConflicts(ConflictResult).GetValueOrDefault();
                    previousIgnoredPath = ConflictResult.IgnoredPaths;
                    HasSavedState = true;
                    HasSavedState = false;
                });
            }
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            async Task saveState()
            {
                if (syncingToken != null)
                {
                    syncingToken.Cancel();
                }
                syncingToken = new CancellationTokenSource();
                if (shouldToggleGameMods)
                {
                    previousIgnoreGameMods = IgnoreGameMods;
                }
                if (shouldToggleSelfConflicts)
                {
                    previousShowSelfConflicts = ShowSelfConflicts;
                }
                if (shouldToggleShowResetConflicts)
                {
                    previousShowResetConflicts = ShowResetConflicts;
                }
                await IgnoreModsAsync(syncingToken.Token).ConfigureAwait(false);
            }

            Mods = new AvaloniaList<Mod>();
            SelectedMods = new AvaloniaList<Mod>();

            CloseCommand = ReactiveCommand.Create(() =>
            {
                ForceClosePopup();
            }).DisposeWith(disposables);

            ModFilterCommand = ReactiveCommand.Create(() =>
            {
                IsOpen = true;
            }).DisposeWith(disposables);

            SelectedMods.CollectionChanged += (sender, args) =>
            {
                if (!settingValues)
                {
                    saveState().ConfigureAwait(false);
                }
            };

            this.WhenAnyValue(p => p.IgnoreGameMods).Subscribe(s =>
            {
                if (!settingValues && previousIgnoreGameMods != IgnoreGameMods)
                {
                    shouldToggleGameMods = true;
                    saveState().ConfigureAwait(false);
                }
            }).DisposeWith(disposables);

            this.WhenAnyValue(m => m.ShowSelfConflicts).Subscribe(s =>
            {
                if (!settingValues && previousShowSelfConflicts != ShowSelfConflicts)
                {
                    shouldToggleSelfConflicts = true;
                    saveState().ConfigureAwait(false);
                }
            }).DisposeWith(disposables);

            this.WhenAnyValue(m => m.ShowResetConflicts).Subscribe(s =>
            {
                if (!settingValues && previousShowResetConflicts != ShowResetConflicts)
                {
                    shouldToggleShowResetConflicts = true;
                    saveState().ConfigureAwait(false);
                }
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class Mods.
        /// </summary>
        public class Mod
        {
            #region Properties

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
            /// </summary>
            /// <param name="obj">The object to compare with the current object.</param>
            /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
            public override bool Equals(object obj)
            {
                if (obj != null && obj is Mod mod)
                {
                    var name = mod.Name ?? string.Empty;
                    var thisName = Name ?? string.Empty;
                    return name.Equals(thisName, StringComparison.Ordinal);
                }
                return base.Equals(obj);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }

            #endregion Methods
        }

        #endregion Classes
    }
}
