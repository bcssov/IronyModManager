// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-25-2020
//
// Last Modified By : Mario
// Last Modified On : 03-25-2020
// ***********************************************************************
// <copyright file="MergeViewerBinaryControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using IronyModManager.Common.ViewModels;
using IronyModManager.Localization.Attributes;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class MergeViewerBinaryControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    public class MergeViewerBinaryControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The block selected
        /// </summary>
        private const string BlockSelected = "BlockSelected";

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the binary file.
        /// </summary>
        /// <value>The binary file.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.BinaryFile)]
        public virtual string BinaryFile { get; protected set; }

        /// <summary>
        /// Gets or sets the take left.
        /// </summary>
        /// <value>The take left.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.TakeLeft)]
        public virtual string TakeLeft { get; protected set; }

        /// <summary>
        /// Gets or sets the take left class.
        /// </summary>
        /// <value>The take left class.</value>
        public virtual string TakeLeftClass { get; protected set; }

        /// <summary>
        /// Gets or sets the take left command.
        /// </summary>
        /// <value>The take left command.</value>
        public virtual ReactiveCommand<Unit, Unit> TakeLeftCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the take right.
        /// </summary>
        /// <value>The take right.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.TakeRight)]
        public virtual string TakeRight { get; protected set; }

        /// <summary>
        /// Gets or sets the take right class.
        /// </summary>
        /// <value>The take right class.</value>
        public virtual string TakeRightClass { get; protected set; }

        /// <summary>
        /// Gets or sets the take right command.
        /// </summary>
        /// <value>The take right command.</value>
        public virtual ReactiveCommand<Unit, Unit> TakeRightCommand { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            TakeLeftCommand = ReactiveCommand.Create(() =>
            {
                TakeRightClass = string.Empty;
                TakeLeftClass = BlockSelected;
            }).DisposeWith(disposables);

            TakeRightCommand = ReactiveCommand.Create(() =>
            {
                TakeLeftClass = string.Empty;
                TakeRightClass = BlockSelected;
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
