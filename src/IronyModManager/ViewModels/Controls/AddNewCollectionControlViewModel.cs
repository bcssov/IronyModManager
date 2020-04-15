// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-05-2020
//
// Last Modified By : Mario
// Last Modified On : 04-15-2020
// ***********************************************************************
// <copyright file="AddNewCollectionControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation;
using IronyModManager.Localization.Attributes;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class AddNewCollectionControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class AddNewCollectionControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The mod collection service
        /// </summary>
        private readonly IModCollectionService modCollectionService;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddNewCollectionControlViewModel" /> class.
        /// </summary>
        /// <param name="modCollectionService">The mod collection service.</param>
        public AddNewCollectionControlViewModel(IModCollectionService modCollectionService)
        {
            this.modCollectionService = modCollectionService;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the cancel.
        /// </summary>
        /// <value>The cancel.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Cancel)]
        public virtual string Cancel { get; protected set; }

        /// <summary>
        /// Gets or sets the cancel command.
        /// </summary>
        /// <value>The cancel command.</value>
        public virtual ReactiveCommand<Unit, Unit> CancelCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the create.
        /// </summary>
        /// <value>The create.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.OK)]
        public virtual string Create { get; protected set; }

        /// <summary>
        /// Gets or sets the create command.
        /// </summary>
        /// <value>The create command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<string>> CreateCommand { get; protected set; }

        /// <summary>
        /// Creates new collectionname.
        /// </summary>
        /// <value>The new name of the collection.</value>
        public virtual string NewCollectionName { get; set; }

        /// <summary>
        /// Creates new collectionwatermark.
        /// </summary>
        /// <value>The new collection watermark.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Watermark)]
        public virtual string NewCollectionWatermark { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            var createEnabled = this.WhenAnyValue(v => v.NewCollectionName, v => !string.IsNullOrWhiteSpace(v));

            CreateCommand = ReactiveCommand.Create(() =>
            {
                if (!string.IsNullOrWhiteSpace(NewCollectionName))
                {
                    var colName = NewCollectionName.Trim();
                    var collections = modCollectionService.GetAll();
                    if (collections != null && !collections.Any(s => s.Name.Equals(NewCollectionName)))
                    {
                        var collection = modCollectionService.Create();
                        collection.Name = colName;
                        collection.IsSelected = true;
                        if (modCollectionService.Save(collection))
                        {
                            NewCollectionName = string.Empty;
                            return new CommandResult<string>(collection.Name, CommandState.Success);
                        }
                    }
                    else
                    {
                        return new CommandResult<string>(colName, CommandState.Exists);
                    }
                }
                return new CommandResult<string>(!string.IsNullOrEmpty(NewCollectionName) ? NewCollectionName.Trim() : string.Empty, CommandState.Failed);
            }, createEnabled).DisposeWith(disposables);

            CancelCommand = ReactiveCommand.Create(() =>
            {
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
