// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-09-2020
//
// Last Modified By : Mario
// Last Modified On : 05-12-2020
// ***********************************************************************
// <copyright file="ModifyCollectionControlViewModel.cs" company="Mario">
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
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class ModifyCollectionControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ModifyCollectionControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The mod collection service
        /// </summary>
        private readonly IModCollectionService modCollectionService;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyCollectionControlViewModel" /> class.
        /// </summary>
        /// <param name="modCollectionService">The mod collection service.</param>
        public ModifyCollectionControlViewModel(IModCollectionService modCollectionService)
        {
            this.modCollectionService = modCollectionService;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the active collection.
        /// </summary>
        /// <value>The active collection.</value>
        public virtual IModCollection ActiveCollection { get; set; }

        /// <summary>
        /// Gets or sets the duplicate.
        /// </summary>
        /// <value>The duplicate.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Duplicate)]
        public virtual string Duplicate { get; protected set; }

        /// <summary>
        /// Gets or sets the duplicate command.
        /// </summary>
        /// <value>The duplicate command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<bool>> DuplicateCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the rename.
        /// </summary>
        /// <value>The rename.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Rename)]
        public virtual string Rename { get; protected set; }

        /// <summary>
        /// Gets or sets the rename command.
        /// </summary>
        /// <value>The rename command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<bool>> RenameCommand { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            RenameCommand = ReactiveCommand.Create(() =>
            {
                return new CommandResult<bool>(true, CommandState.Success);
            }).DisposeWith(disposables);

            DuplicateCommand = ReactiveCommand.Create(() =>
            {
                if (ActiveCollection != null)
                {
                    var collections = modCollectionService.GetAll();
                    var count = collections.Where(p => p.Name.Equals(ActiveCollection.Name)).Count();
                    var name = $"{ActiveCollection.Name} ({count})";
                    while (collections.Any(p => p.Name.Equals(name)))
                    {
                        count++;
                        name = $"{ActiveCollection.Name} ({count})";
                    }
                    var copied = modCollectionService.Create();
                    copied.IsSelected = true;
                    copied.Mods = ActiveCollection.Mods;
                    copied.Name = name;
                    if (modCollectionService.Save(copied))
                    {
                        return new CommandResult<bool>(false, CommandState.Success);
                    }
                    else
                    {
                        return new CommandResult<bool>(false, CommandState.Failed);
                    }
                }
                return new CommandResult<bool>(false, CommandState.NotExecuted);
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
