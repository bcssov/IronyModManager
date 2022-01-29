// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-24-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2022
// ***********************************************************************
// <copyright file="ModCompareSelectorControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using IronyModManager.Common;
using IronyModManager.Common.ViewModels;
using IronyModManager.DI;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.Hotkey;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class ModCompareSelectorControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ModCompareSelectorControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The application action
        /// </summary>
        private readonly IAppAction appAction;

        /// <summary>
        /// The hotkey pressed handler
        /// </summary>
        private readonly ConflictSolverViewHotkeyPressedHandler hotkeyPressedHandler;

        /// <summary>
        /// The mod service
        /// </summary>
        private readonly IModPatchCollectionService modPatchCollectionService;

        /// <summary>
        /// The adding virtual definition
        /// </summary>
        private bool addingVirtualDefinition = false;

        /// <summary>
        /// The cancel
        /// </summary>
        private CancellationTokenSource cancel = null;

        /// <summary>
        /// The previous definitions
        /// </summary>
        private IEnumerable<IDefinition> previousDefinitions = null;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModCompareSelectorControlViewModel" /> class.
        /// </summary>
        /// <param name="hotkeyPressedHandler">The hotkey pressed handler.</param>
        /// <param name="appAction">The application action.</param>
        /// <param name="modPatchCollectionService">The mod patch collection service.</param>
        public ModCompareSelectorControlViewModel(ConflictSolverViewHotkeyPressedHandler hotkeyPressedHandler,
            IAppAction appAction, IModPatchCollectionService modPatchCollectionService)
        {
            this.modPatchCollectionService = modPatchCollectionService;
            this.appAction = appAction;
            this.hotkeyPressedHandler = hotkeyPressedHandler;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the name of the collection.
        /// </summary>
        /// <value>The name of the collection.</value>
        public virtual string CollectionName { get; set; }

        /// <summary>
        /// Gets or sets the invalid conflict path.
        /// </summary>
        /// <value>The invalid conflict path.</value>
        public virtual string ConflictPath { get; protected set; }

        /// <summary>
        /// Gets or sets the definitions.
        /// </summary>
        /// <value>The definitions.</value>
        public virtual IEnumerable<IDefinition> Definitions { get; set; }

        /// <summary>
        /// Gets or sets the definition selection.
        /// </summary>
        /// <value>The definition selection.</value>
        public virtual CompareSelection DefinitionSelection { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is binary conflict.
        /// </summary>
        /// <value><c>true</c> if this instance is binary conflict; otherwise, <c>false</c>.</value>
        public virtual bool IsBinaryConflict { get; set; }

        /// <summary>
        /// Gets or sets the left selected definition.
        /// </summary>
        /// <value>The left selected definition.</value>
        public virtual IDefinition LeftSelectedDefinition { get; protected set; }

        /// <summary>
        /// Gets or sets the open directory.
        /// </summary>
        /// <value>The open directory.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.CompareSelectorContextMenu.OpenDirectory)]
        public virtual string OpenDirectory { get; protected set; }

        /// <summary>
        /// Gets or sets the open directory command.
        /// </summary>
        /// <value>The open directory command.</value>
        public virtual ReactiveCommand<Unit, Unit> OpenDirectoryCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the open file.
        /// </summary>
        /// <value>The open file.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.CompareSelectorContextMenu.OpenFile)]
        public virtual string OpenFile { get; protected set; }

        /// <summary>
        /// Gets or sets the open file command.
        /// </summary>
        /// <value>The open file command.</value>
        public virtual ReactiveCommand<Unit, Unit> OpenFileCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the right selected definition.
        /// </summary>
        /// <value>The right selected definition.</value>
        public virtual IDefinition RightSelectedDefinition { get; protected set; }

        /// <summary>
        /// Gets or sets the selected mods order.
        /// </summary>
        /// <value>The selected mods order.</value>
        public virtual IList<string> SelectedModsOrder { get; set; }

        /// <summary>
        /// Gets or sets the virtual definitions.
        /// </summary>
        /// <value>The virtual definitions.</value>
        public virtual IEnumerable<IDefinition> VirtualDefinitions { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            LeftSelectedDefinition = null;
            RightSelectedDefinition = null;
            DefinitionSelection = null;
            VirtualDefinitions = null;
            previousDefinitions = null;
        }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="definition">The definition.</param>
        public virtual void SetParameters(IDefinition definition)
        {
            ConflictPath = string.Empty;
            if (definition != null)
            {
                if (!modPatchCollectionService.IsPatchMod(definition.ModName))
                {
                    ConflictPath = modPatchCollectionService.ResolveFullDefinitionPath(definition);
                }
            }
        }

        /// <summary>
        /// add virtual definition as an asynchronous operation.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="token">The token.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected async Task AddVirtualDefinitionAsync(IEnumerable<IDefinition> definitions, CancellationToken token)
        {
            addingVirtualDefinition = true;
            if (previousDefinitions != definitions)
            {
                if (!IsBinaryConflict)
                {
                    var col = definitions.OrderBy(p => SelectedModsOrder.IndexOf(p.ModName)).ToHashSet();
                    var priorityDefinition = modPatchCollectionService.EvalDefinitionPriority(col);
                    if (priorityDefinition == null || priorityDefinition.Definition == null)
                    {
                        if (priorityDefinition == null)
                        {
                            priorityDefinition = DIResolver.Get<IPriorityDefinitionResult>();
                            priorityDefinition.PriorityType = DefinitionPriorityType.None;
                        }
                        if (priorityDefinition.Definition == null)
                        {
                            priorityDefinition.Definition = col.First();
                        }
                    }
                    IDefinition newDefinition = null;
                    if (priorityDefinition.PriorityType != DefinitionPriorityType.NoProvider)
                    {
                        newDefinition = await modPatchCollectionService.CreatePatchDefinitionAsync(priorityDefinition.Definition, CollectionName);
                        if (newDefinition != null)
                        {
                            col.Add(newDefinition);
                        }
                    }
                    VirtualDefinitions = col.ToObservableCollection();
                    var virtualDefinitions = VirtualDefinitions;
                    if (virtualDefinitions.Any())
                    {
                        // No reason to anymore not select a default definition on either side, wait a bit first to allow the UI to settle down
                        await Task.Delay(100, token);
                        if (!token.IsCancellationRequested)
                        {
                            DefinitionSelection = null;
                            LeftSelectedDefinition = null;
                            RightSelectedDefinition = null;
                        }
                        await Task.Delay(50, token);
                        if (!token.IsCancellationRequested)
                        {
                            int virtualDefCount = virtualDefinitions.Count();
                            var left = newDefinition != null ? virtualDefinitions.FirstOrDefault(p => p != newDefinition && p != priorityDefinition.Definition && !(virtualDefCount >= 4 && p.IsFromGame)) : virtualDefinitions.FirstOrDefault(p => p != priorityDefinition.Definition);
                            if (left == null)
                            {
                                left = virtualDefinitions.FirstOrDefault(p => p != newDefinition && p != priorityDefinition.Definition);
                            }
                            var right = newDefinition ?? priorityDefinition.Definition;
                            DefinitionSelection = new CompareSelection(left, right);
                            LeftSelectedDefinition = left;
                            RightSelectedDefinition = right;
                        }
                    }
                }
                else
                {
                    VirtualDefinitions = definitions.OrderBy(p => SelectedModsOrder.IndexOf(p.ModName)).ToHashSet();
                    var virtualDefinitions = VirtualDefinitions;
                    var priorityDefinition = modPatchCollectionService.EvalDefinitionPriority(virtualDefinitions);
                    if (priorityDefinition == null || priorityDefinition.Definition == null)
                    {
                        if (priorityDefinition == null)
                        {
                            priorityDefinition = DIResolver.Get<IPriorityDefinitionResult>();
                            priorityDefinition.PriorityType = DefinitionPriorityType.None;
                        }
                        if (priorityDefinition.Definition == null)
                        {
                            priorityDefinition.Definition = virtualDefinitions.First();
                        }
                    }
                    if (virtualDefinitions.Any())
                    {
                        // No reason to anymore not select a default definition on either side, wait a bit first to allow the UI to settle down
                        await Task.Delay(100, token);
                        if (!token.IsCancellationRequested)
                        {
                            DefinitionSelection = null;
                            LeftSelectedDefinition = null;
                            RightSelectedDefinition = null;
                        }
                        await Task.Delay(50, token);
                        if (!token.IsCancellationRequested)
                        {
                            var left = virtualDefinitions.FirstOrDefault(p => p != priorityDefinition.Definition);
                            var right = priorityDefinition.Definition;
                            DefinitionSelection = new CompareSelection(left, right);
                            LeftSelectedDefinition = left;
                            RightSelectedDefinition = right;
                        }
                    }
                }
            }
            previousDefinitions = definitions;
            addingVirtualDefinition = false;
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            this.WhenAnyValue(s => s.Definitions).Subscribe(s =>
            {
                if (s != null)
                {
                    if (cancel != null)
                    {
                        cancel.Cancel();
                    }
                    cancel = new CancellationTokenSource();
                    AddVirtualDefinitionAsync(s, cancel.Token).ConfigureAwait(true);
                }
                else
                {
                    VirtualDefinitions = null;
                    previousDefinitions = null;
                }
            }).DisposeWith(disposables);

            OpenDirectoryCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (!string.IsNullOrWhiteSpace(ConflictPath))
                {
                    await appAction.OpenAsync(Path.GetDirectoryName(ConflictPath));
                }
            }).DisposeWith(disposables);

            OpenFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (!string.IsNullOrWhiteSpace(ConflictPath))
                {
                    await appAction.OpenAsync(ConflictPath);
                }
            }).DisposeWith(disposables);

            hotkeyPressedHandler.Subscribe(m =>
            {
                SelectDefinitionByHotkey(m.Hotkey);
            }).DisposeWith(disposables);

            this.WhenAnyValue(p => p.LeftSelectedDefinition, p => p.RightSelectedDefinition, (left, right) => new CompareSelection(left, right)).Where(p => p != DefinitionSelection && p.LeftSelectedDefinition != null && p.RightSelectedDefinition != null).Subscribe(s =>
            {
                DefinitionSelection = s;
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// Selects the definition by hotkey.
        /// </summary>
        /// <param name="hotKey">The hot key.</param>
        protected virtual void SelectDefinitionByHotkey(Enums.HotKeys hotKey)
        {
            if (addingVirtualDefinition)
            {
                return;
            }
            var selectLeft = false;
            int? index = null;
            switch (hotKey)
            {
                case Enums.HotKeys.Ctrl_1:
                    index = 0;
                    selectLeft = true;
                    break;

                case Enums.HotKeys.Ctrl_Shift_1:
                    index = 0;
                    break;

                case Enums.HotKeys.Ctrl_2:
                    index = 1;
                    selectLeft = true;
                    break;

                case Enums.HotKeys.Ctrl_Shift_2:
                    index = 1;
                    break;

                case Enums.HotKeys.Ctrl_3:
                    index = 2;
                    selectLeft = true;
                    break;

                case Enums.HotKeys.Ctrl_Shift_3:
                    index = 2;
                    break;

                case Enums.HotKeys.Ctrl_4:
                    index = 3;
                    selectLeft = true;
                    break;

                case Enums.HotKeys.Ctrl_Shift_4:
                    index = 3;
                    break;

                case Enums.HotKeys.Ctrl_5:
                    index = 4;
                    selectLeft = true;
                    break;

                case Enums.HotKeys.Ctrl_Shift_5:
                    index = 4;
                    break;

                case Enums.HotKeys.Ctrl_6:
                    index = 5;
                    selectLeft = true;
                    break;

                case Enums.HotKeys.Ctrl_Shift_6:
                    index = 5;
                    break;

                case Enums.HotKeys.Ctrl_7:
                    index = 6;
                    selectLeft = true;
                    break;

                case Enums.HotKeys.Ctrl_Shift_7:
                    index = 6;
                    break;

                case Enums.HotKeys.Ctrl_8:
                    index = 7;
                    selectLeft = true;
                    break;

                case Enums.HotKeys.Ctrl_Shift_8:
                    index = 7;
                    break;

                case Enums.HotKeys.Ctrl_9:
                    index = 8;
                    selectLeft = true;
                    break;

                case Enums.HotKeys.Ctrl_Shift_9:
                    index = 8;
                    break;

                case Enums.HotKeys.Ctrl_0:
                    index = 9;
                    selectLeft = true;
                    break;

                case Enums.HotKeys.Ctrl_Shift_0:
                    index = 9;
                    break;

                default:
                    break;
            }
            if (index.HasValue && VirtualDefinitions?.Count() - 1 >= index.GetValueOrDefault())
            {
                Dispatcher.UIThread.SafeInvoke(() =>
                {
                    var left = DefinitionSelection.LeftSelectedDefinition;
                    var right = DefinitionSelection.RightSelectedDefinition;
                    if (selectLeft)
                    {
                        left = VirtualDefinitions.ToList()[index.GetValueOrDefault()];
                    }
                    else
                    {
                        right = VirtualDefinitions.ToList()[index.GetValueOrDefault()];
                    }
                    DefinitionSelection = new CompareSelection(left, right);
                    LeftSelectedDefinition = left;
                    RightSelectedDefinition = right;
                });
            }
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class CompareSelection.
        /// </summary>
        public class CompareSelection
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="CompareSelection" /> class.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            public CompareSelection(IDefinition left, IDefinition right)
            {
                LeftSelectedDefinition = left;
                RightSelectedDefinition = right;
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets or sets the left selected definition.
            /// </summary>
            /// <value>The left selected definition.</value>
            public virtual IDefinition LeftSelectedDefinition { get; set; }

            /// <summary>
            /// Gets or sets the right selected definition.
            /// </summary>
            /// <value>The right selected definition.</value>
            public virtual IDefinition RightSelectedDefinition { get; set; }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
            /// </summary>
            /// <param name="obj">The object to compare with the current object.</param>
            /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
            public override bool Equals(object obj)
            {
                if (obj is CompareSelection cs)
                {
                    return Equals(cs);
                }
                return base.Equals(obj);
            }

            /// <summary>
            /// Equalses the specified object.
            /// </summary>
            /// <param name="obj">The object.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            public bool Equals(CompareSelection obj)
            {
                if (obj == null)
                {
                    return false;
                }
                return obj.LeftSelectedDefinition == LeftSelectedDefinition && obj.RightSelectedDefinition == RightSelectedDefinition;
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
            public override int GetHashCode()
            {
                return HashCode.Combine(LeftSelectedDefinition, RightSelectedDefinition);
            }

            #endregion Methods
        }

        #endregion Classes
    }
}
