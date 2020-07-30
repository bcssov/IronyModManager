// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-24-2020
//
// Last Modified By : Mario
// Last Modified On : 07-30-2020
// ***********************************************************************
// <copyright file="ModCompareSelectorControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using IronyModManager.Common;
using IronyModManager.Common.ViewModels;
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
        /// The mod service
        /// </summary>
        private readonly IModPatchCollectionService modPatchCollectionService;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModCompareSelectorControlViewModel" /> class.
        /// </summary>
        /// <param name="modPatchCollectionService">The mod patch collection service.</param>
        public ModCompareSelectorControlViewModel(IModPatchCollectionService modPatchCollectionService)
        {
            this.modPatchCollectionService = modPatchCollectionService;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the name of the collection.
        /// </summary>
        /// <value>The name of the collection.</value>
        public virtual string CollectionName { get; set; }

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
        }

        /// <summary>
        /// add virtual definition as an asynchronous operation.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        protected async Task AddVirtualDefinitionAsync(IEnumerable<IDefinition> definitions)
        {
            if (!IsBinaryConflict)
            {
                var col = definitions.OrderBy(p => SelectedModsOrder.IndexOf(p.ModName)).ToHashSet();
                var priorityDefinition = modPatchCollectionService.EvalDefinitionPriority(col);
                if (priorityDefinition?.Definition == null)
                {
                    priorityDefinition.Definition = col.First();
                }
                var newDefinition = await modPatchCollectionService.CreatePatchDefinitionAsync(priorityDefinition.Definition, CollectionName);
                if (newDefinition != null)
                {
                    col.Add(newDefinition);
                }
                VirtualDefinitions = col.ToObservableCollection();
                // No reason to anymore not select a default definition on either side, wait a bit first to allow the UI to settle down
                await Task.Delay(100);
                LeftSelectedDefinition = VirtualDefinitions.FirstOrDefault(p => p != newDefinition && p != priorityDefinition.Definition);
                RightSelectedDefinition = newDefinition;
            }
            else
            {
                VirtualDefinitions = definitions.OrderBy(p => SelectedModsOrder.IndexOf(p.ModName)).ToHashSet();
                // No reason to anymore not select a default definition on either side, wait a bit first to allow the UI to settle down
                await Task.Delay(100);
                LeftSelectedDefinition = definitions.ElementAt(0);
                RightSelectedDefinition = definitions.ElementAt(1);
            }
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
                }
            }).DisposeWith(disposables);
            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
