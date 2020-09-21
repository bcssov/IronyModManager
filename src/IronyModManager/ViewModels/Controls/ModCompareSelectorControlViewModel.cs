// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-24-2020
//
// Last Modified By : Mario
// Last Modified On : 09-21-2020
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
using System.Threading.Tasks;
using IronyModManager.Common;
using IronyModManager.Common.ViewModels;
using IronyModManager.DI;
using IronyModManager.Implementation.Actions;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
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
        /// The mod service
        /// </summary>
        private readonly IModPatchCollectionService modPatchCollectionService;

        /// <summary>
        /// The previous definitions
        /// </summary>
        private IEnumerable<IDefinition> previousDefinitions = null;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModCompareSelectorControlViewModel" /> class.
        /// </summary>
        /// <param name="appAction">The application action.</param>
        /// <param name="modPatchCollectionService">The mod patch collection service.</param>
        public ModCompareSelectorControlViewModel(IAppAction appAction, IModPatchCollectionService modPatchCollectionService)
        {
            this.modPatchCollectionService = modPatchCollectionService;
            this.appAction = appAction;
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
        /// Gets or sets a value indicating whether this instance is binary conflict.
        /// </summary>
        /// <value><c>true</c> if this instance is binary conflict; otherwise, <c>false</c>.</value>
        public virtual bool IsBinaryConflict { get; set; }

        /// <summary>
        /// Gets or sets the left selected definition.
        /// </summary>
        /// <value>The left selected definition.</value>
        public virtual IDefinition LeftSelectedDefinition { get; set; }

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
        public virtual IDefinition RightSelectedDefinition { get; set; }

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
                ConflictPath = modPatchCollectionService.ResolveFullDefinitionPath(definition);
            }
        }

        /// <summary>
        /// add virtual definition as an asynchronous operation.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        protected async Task AddVirtualDefinitionAsync(IEnumerable<IDefinition> definitions)
        {
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
                    var newDefinition = await modPatchCollectionService.CreatePatchDefinitionAsync(priorityDefinition.Definition, CollectionName);
                    if (newDefinition != null)
                    {
                        col.Add(newDefinition);
                    }
                    VirtualDefinitions = col.ToObservableCollection();
                    if (VirtualDefinitions.Count() > 0)
                    {
                        // No reason to anymore not select a default definition on either side, wait a bit first to allow the UI to settle down
                        await Task.Delay(100);
                        LeftSelectedDefinition = null;
                        RightSelectedDefinition = null;
                        await Task.Delay(50);
                        LeftSelectedDefinition = VirtualDefinitions.FirstOrDefault(p => p != newDefinition && p != priorityDefinition.Definition);
                        RightSelectedDefinition = newDefinition;
                    }
                }
                else
                {
                    VirtualDefinitions = definitions.OrderBy(p => SelectedModsOrder.IndexOf(p.ModName)).ToHashSet();
                    if (VirtualDefinitions.Count() > 0)
                    {
                        // No reason to anymore not select a default definition on either side, wait a bit first to allow the UI to settle down
                        await Task.Delay(100);
                        LeftSelectedDefinition = null;
                        RightSelectedDefinition = null;
                        await Task.Delay(50);
                        LeftSelectedDefinition = definitions.ElementAt(0);
                        RightSelectedDefinition = definitions.ElementAt(1);
                    }
                }
            }
            previousDefinitions = definitions;
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
                    AddVirtualDefinitionAsync(s).ConfigureAwait(true);
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

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
