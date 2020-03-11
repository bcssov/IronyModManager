// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-02-2020
//
// Last Modified By : Mario
// Last Modified On : 03-11-2020
// ***********************************************************************
// <copyright file="SearchModsControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation;
using IronyModManager.Localization.Attributes;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class SearchModsControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    public class SearchModsControlViewModel : BaseViewModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the clear text.
        /// </summary>
        /// <value>The clear text.</value>
        [StaticLocalization(LocalizationResources.Filter.Clear)]
        public virtual string ClearText { get; protected set; }

        /// <summary>
        /// Gets or sets the clear text command.
        /// </summary>
        /// <value>The clear text command.</value>
        public virtual ReactiveCommand<Unit, Unit> ClearTextCommand { get; protected set; }

        /// <summary>
        /// Gets or sets down arrow command.
        /// </summary>
        /// <value>Down arrow command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<bool>> DownArrowCommand { get; protected set; }

        /// <summary>
        /// Gets or sets down arrow text.
        /// </summary>
        /// <value>Down arrow text.</value>
        [StaticLocalization(LocalizationResources.Filter.DownArrow)]
        public virtual string DownArrowText { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show arrows].
        /// </summary>
        /// <value><c>true</c> if [show arrows]; otherwise, <c>false</c>.</value>
        public virtual bool ShowArrows { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public virtual string Text { get; set; }

        /// <summary>
        /// Gets or sets up arrow command.
        /// </summary>
        /// <value>Up arrow command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<bool>> UpArrowCommand { get; protected set; }

        /// <summary>
        /// Gets or sets up arrow text.
        /// </summary>
        /// <value>Up arrow text.</value>
        [StaticLocalization(LocalizationResources.Filter.UpArrow)]
        public virtual string UpArrowText { get; protected set; }

        /// <summary>
        /// Gets or sets the watermark text.
        /// </summary>
        /// <value>The watermark text.</value>
        public virtual string WatermarkText { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            var arrowEnabled = this.WhenAnyValue(v => v.Text, v => !string.IsNullOrWhiteSpace(v));

            ClearTextCommand = ReactiveCommand.Create(() =>
            {
                Text = string.Empty;
            }).DisposeWith(disposables);

            UpArrowCommand = ReactiveCommand.Create(() =>
            {
                return new CommandResult<bool>(true, CommandState.Success);
            }, arrowEnabled).DisposeWith(disposables);

            DownArrowCommand = ReactiveCommand.Create(() =>
            {
                return new CommandResult<bool>(false, CommandState.Success);
            }, arrowEnabled).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
