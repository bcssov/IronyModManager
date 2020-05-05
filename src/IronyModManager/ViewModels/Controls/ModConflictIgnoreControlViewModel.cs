// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-05-2020
//
// Last Modified By : Mario
// Last Modified On : 05-05-2020
// ***********************************************************************
// <copyright file="ModConflictIgnoreControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Reactive;
using System;
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
    /// Class ModConflictIgnoreControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ModConflictIgnoreControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The mod service
        /// </summary>
        private readonly IModService modService;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModConflictIgnoreControlViewModel" /> class.
        /// </summary>
        /// <param name="modService">The mod service.</param>
        public ModConflictIgnoreControlViewModel(IModService modService)
        {
            this.modService = modService;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the cancel.
        /// </summary>
        /// <value>The cancel.</value>
        [StaticLocalization(LocalizationResources.ConflictIgnore.Cancel)]
        public virtual string Cancel { get; protected set; }

        /// <summary>
        /// Gets or sets the cancel command.
        /// </summary>
        /// <value>The cancel command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<bool>> CancelCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the collection.
        /// </summary>
        /// <value>The name of the collection.</value>
        public virtual string CollectionName { get; set; }

        /// <summary>
        /// Gets or sets the conflict result.
        /// </summary>
        /// <value>The conflict result.</value>
        public virtual IConflictResult ConflictResult { get; set; }

        /// <summary>
        /// Gets or sets the save.
        /// </summary>
        /// <value>The save.</value>
        [StaticLocalization(LocalizationResources.ConflictIgnore.Save)]
        public virtual string Save { get; protected set; }

        /// <summary>
        /// Gets or sets the save command.
        /// </summary>
        /// <value>The save command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<bool>> SaveCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the watermark.
        /// </summary>
        /// <value>The watermark.</value>
        [StaticLocalization(LocalizationResources.ConflictIgnore.Watermark)]
        public virtual string Watermark { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await modService.SaveIgnoredPathsAsync(ConflictResult, CollectionName);
                return new CommandResult<bool>(result, result ? CommandState.Success : CommandState.Failed);
            }).DisposeWith(disposables);

            CancelCommand = ReactiveCommand.Create(() =>
            {
                return new CommandResult<bool>(false, CommandState.NotExecuted);
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
