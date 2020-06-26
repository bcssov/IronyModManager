// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 06-08-2020
//
// Last Modified By : Mario
// Last Modified On : 06-26-2020
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
using System.Threading.Tasks;
using Avalonia.Collections;
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
        /// The syncing selected mods
        /// </summary>
        private bool syncingSelectedMods = false;

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
        public virtual IAvaloniaList<string> Mods { get; set; }

        /// <summary>
        /// Gets or sets the selected mods.
        /// </summary>
        /// <value>The selected mods.</value>
        public virtual IAvaloniaList<string> SelectedMods { get; protected set; }

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
            this.collectionName = collectionName;
            ConflictResult = conflictResult;
            previousIgnoredPath = conflictResult.IgnoredPaths;
            isOpen?.Dispose();
            isOpen = this.WhenAnyValue(p => p.IsOpen).Where(p => p && !syncingSelectedMods).Subscribe(s =>
            {
                Mods.Clear();
                SelectedMods.Clear();
                Mods.AddRange(activeMods);
                SelectedMods.AddRange(activeMods.Except(modPatchCollectionService.GetIgnoredMods(ConflictResult)).ToList());
            }).DisposeWith(Disposables);
        }

        /// <summary>
        /// Ignores the mods.
        /// </summary>
        protected async Task IgnoreModsAsync()
        {
            await Task.Delay(100);
            var mods = Mods.Except(SelectedMods).ToList();
            modPatchCollectionService.AddModsToIgnoreList(ConflictResult, mods);
            if (previousIgnoredPath == null)
            {
                previousIgnoredPath = string.Empty;
            }
            if (previousIgnoredPath.Equals(ConflictResult.IgnoredPaths ?? string.Empty))
            {
                syncingSelectedMods = false;
                return;
            }
            await modPatchCollectionService.SaveIgnoredPathsAsync(ConflictResult, collectionName);
            previousIgnoredPath = ConflictResult.IgnoredPaths;
            HasSavedState = true;
            syncingSelectedMods = false;
            HasSavedState = false;
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            Mods = new AvaloniaList<string>();
            SelectedMods = new AvaloniaList<string>();

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
                if (syncingSelectedMods)
                {
                    return;
                }
                syncingSelectedMods = true;
                IgnoreModsAsync().ConfigureAwait(false);
            };

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
