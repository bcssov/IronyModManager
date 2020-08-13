// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 06-11-2020
//
// Last Modified By : Mario
// Last Modified On : 08-13-2020
// ***********************************************************************
// <copyright file="ConflictSolverResetConflictsControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using IronyModManager.Common;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class ConflictSolverResetConflictsControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ConflictSolverResetConflictsControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The custom value
        /// </summary>
        private const string CustomValue = "custom";

        /// <summary>
        /// The ignored value
        /// </summary>
        private const string IgnoredValue = "ignored";

        /// <summary>
        /// The resolved value
        /// </summary>
        private const string ResolvedValue = "resolved";

        /// <summary>
        /// The mod patch collection service
        /// </summary>
        private readonly IModPatchCollectionService modPatchCollectionService;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictSolverResetConflictsControlViewModel" /> class.
        /// </summary>
        /// <param name="modPatchCollectionService">The mod patch collection service.</param>
        public ConflictSolverResetConflictsControlViewModel(IModPatchCollectionService modPatchCollectionService)
        {
            this.modPatchCollectionService = modPatchCollectionService;
            Modes = new List<Mode>()
            {
                new Mode()
                {
                    Name = Resolved,
                    Value = ResolvedValue
                },
                new Mode()
                {
                    Name = Ignored,
                    Value = IgnoredValue
                },
                new Mode()
                {
                    Name = Custom,
                    Value = CustomValue
                }
            };
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the close.
        /// </summary>
        /// <value>The close.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ResetConflicts.Close)]
        public virtual string Close { get; protected set; }

        /// <summary>
        /// Gets or sets the close command.
        /// </summary>
        /// <value>The close command.</value>
        public virtual ReactiveCommand<Unit, Unit> CloseCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the collection.
        /// </summary>
        /// <value>The name of the collection.</value>
        public virtual string CollectionName { get; protected set; }

        /// <summary>
        /// Gets or sets the conflicts.
        /// </summary>
        /// <value>The conflicts.</value>
        public virtual IConflictResult Conflicts { get; protected set; }

        /// <summary>
        /// Gets or sets the custom.
        /// </summary>
        /// <value>The custom.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ResetConflicts.Custom)]
        public virtual string Custom { get; protected set; }

        /// <summary>
        /// Gets or sets the hierarchical definitions.
        /// </summary>
        /// <value>The hierarchical definitions.</value>
        public virtual IEnumerable<IHierarchicalDefinitions> HierarchicalDefinitions { get; protected set; }

        /// <summary>
        /// Gets or sets the ignored.
        /// </summary>
        /// <value>The ignored.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ResetConflicts.Ignored)]
        public virtual string Ignored { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public virtual bool IsOpen { get; protected set; }

        /// <summary>
        /// Gets or sets the modes.
        /// </summary>
        /// <value>The modes.</value>
        public virtual IEnumerable<Mode> Modes { get; protected set; }

        /// <summary>
        /// Gets or sets the index of the previous conflict.
        /// </summary>
        /// <value>The index of the previous conflict.</value>
        public virtual int? PreviousConflictIndex { get; set; }

        /// <summary>
        /// Gets or sets the reset.
        /// </summary>
        /// <value>The reset.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ResetConflicts.Reset)]
        public virtual string Reset { get; protected set; }

        /// <summary>
        /// Gets or sets the reset command.
        /// </summary>
        /// <value>The reset command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<bool>> ResetCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the reset conflicts.
        /// </summary>
        /// <value>The reset conflicts.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ResetConflicts.Title)]
        public virtual string ResetConflicts { get; protected set; }

        /// <summary>
        /// Gets or sets the reset conflicts command.
        /// </summary>
        /// <value>The reset conflicts command.</value>
        public virtual ReactiveCommand<Unit, Unit> ResetConflictsCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the resolved.
        /// </summary>
        /// <value>The resolved.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ResetConflicts.Resolved)]
        public virtual string Resolved { get; protected set; }

        /// <summary>
        /// Gets or sets the selected parent hierarchical definition.
        /// </summary>
        /// <value>The selected parent hierarchical definition.</value>
        public virtual IHierarchicalDefinitions SelectedHierarchicalDefinition { get; set; }

        /// <summary>
        /// Gets or sets the selected mode.
        /// </summary>
        /// <value>The selected mode.</value>
        public virtual Mode SelectedMode { get; protected set; }

        /// <summary>
        /// Gets or sets the selected parent hierarchical definition.
        /// </summary>
        /// <value>The selected parent hierarchical definition.</value>
        public virtual IHierarchicalDefinitions SelectedParentHierarchicalDefinition { get; set; }

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
        /// Refreshes this instance.
        /// </summary>
        public virtual void Refresh()
        {
            Bind(GetHierarchicalDefinitions(SelectedMode));
        }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="collectionName">Name of the collection.</param>
        public virtual void SetParameters(IConflictResult conflictResult, string collectionName)
        {
            Conflicts = conflictResult;
            CollectionName = collectionName;
            Bind(GetHierarchicalDefinitions(SelectedMode));
        }

        /// <summary>
        /// Resets the view.
        /// </summary>
        /// <param name="hierarchicalDefinitions">The hierarchical definitions.</param>
        protected virtual void Bind(IEnumerable<IHierarchicalDefinitions> hierarchicalDefinitions)
        {
            var index = PreviousConflictIndex;
            PreviousConflictIndex = null;
            if (hierarchicalDefinitions != null)
            {
                HierarchicalDefinitions = hierarchicalDefinitions.ToObservableCollection();
                if (HierarchicalDefinitions.Count() > 0 && SelectedHierarchicalDefinition == null)
                {
                    SelectedParentHierarchicalDefinition = HierarchicalDefinitions.FirstOrDefault();
                }
                if (SelectedParentHierarchicalDefinition != null)
                {
                    var conflictName = SelectedParentHierarchicalDefinition.Name;
                    SelectedParentHierarchicalDefinition = null;
                    var newSelected = HierarchicalDefinitions.FirstOrDefault(p => p.Name.Equals(conflictName));
                    if (newSelected != null)
                    {
                        PreviousConflictIndex = index;
                        if (PreviousConflictIndex.GetValueOrDefault() > (newSelected.Children.Count - 1))
                        {
                            PreviousConflictIndex = newSelected.Children.Count - 1;
                        }
                        SelectedParentHierarchicalDefinition = newSelected;
                    }
                    if (SelectedParentHierarchicalDefinition.Children.Count > 0)
                    {
                        SelectedHierarchicalDefinition = SelectedParentHierarchicalDefinition.Children.FirstOrDefault();
                    }
                }
            }
            else
            {
                HierarchicalDefinitions = null;
            }
        }

        /// <summary>
        /// Gets the hierarchical definitions.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns>IEnumerable&lt;IHierarchicalDefinitions&gt;.</returns>
        protected IEnumerable<IHierarchicalDefinitions> GetHierarchicalDefinitions(Mode mode)
        {
            if (Conflicts != null && Conflicts.IgnoredConflicts != null && Conflicts.ResolvedConflicts != null)
            {
                if (mode?.Value == IgnoredValue)
                {
                    return Conflicts.IgnoredConflicts.GetHierarchicalDefinitions();
                }
                else if (mode?.Value == ResolvedValue)
                {
                    return Conflicts.ResolvedConflicts.GetHierarchicalDefinitions();
                }
                else if (mode?.Value == CustomValue)
                {
                    return Conflicts.CustomConflicts.GetHierarchicalDefinitions();
                }
            }
            return null;
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            SelectedMode = Modes.FirstOrDefault();

            CloseCommand = ReactiveCommand.Create(() =>
            {
                ForceClosePopup();
            }).DisposeWith(disposables);

            ResetConflictsCommand = ReactiveCommand.Create(() =>
            {
                IsOpen = true;
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.SelectedMode).Subscribe(s =>
            {
                Bind(GetHierarchicalDefinitions(s));
            }).DisposeWith(disposables);

            ResetCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (SelectedHierarchicalDefinition != null && !string.IsNullOrWhiteSpace(SelectedHierarchicalDefinition.Key))
                {
                    if (SelectedMode?.Value == IgnoredValue)
                    {
                        var result = await modPatchCollectionService.ResetIgnoredConflictAsync(Conflicts, SelectedHierarchicalDefinition.Key, CollectionName);
                        if (result)
                        {
                            Bind(GetHierarchicalDefinitions(SelectedMode));
                            return new CommandResult<bool>(true, CommandState.Success);
                        }
                    }
                    else if (SelectedMode?.Value == ResolvedValue)
                    {
                        var result = await modPatchCollectionService.ResetResolvedConflictAsync(Conflicts, SelectedHierarchicalDefinition.Key, CollectionName);
                        if (result)
                        {
                            Bind(GetHierarchicalDefinitions(SelectedMode));
                            return new CommandResult<bool>(true, CommandState.Success);
                        }
                    }
                    else if (SelectedMode?.Value == CustomValue)
                    {
                        var result = await modPatchCollectionService.ResetCustomConflictAsync(Conflicts, SelectedHierarchicalDefinition.Key, CollectionName);
                        if (result)
                        {
                            Bind(GetHierarchicalDefinitions(SelectedMode));
                            return new CommandResult<bool>(true, CommandState.Success);
                        }
                    }
                }
                return new CommandResult<bool>(false, CommandState.NotExecuted);
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class Mode.
        /// </summary>
        public class Mode
        {
            #region Properties

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            /// <value>The value.</value>
            public string Value { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
