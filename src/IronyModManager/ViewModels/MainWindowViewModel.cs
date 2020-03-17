// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 03-17-2020
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Reactive.Disposables;
using IronyModManager.Common.Events;
using IronyModManager.Common.ViewModels;
using IronyModManager.DI;
using System.Linq;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class MainWindowViewModel : BaseViewModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        public MainWindowViewModel()
        {
            Main = DIResolver.Get<MainControlViewModel>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance has progress.
        /// </summary>
        /// <value><c>true</c> if this instance has progress; otherwise, <c>false</c>.</value>
        public virtual bool HasProgress { get; protected set; }

        /// <summary>
        /// Gets or sets the language selector.
        /// </summary>
        /// <value>The language selector.</value>
        public virtual MainControlViewModel Main { get; protected set; }

        /// <summary>
        /// Gets or sets the overlay message.
        /// </summary>
        /// <value>The overlay message.</value>
        public virtual string OverlayMessage { get; protected set; }

        /// <summary>
        /// Gets or sets the overlay message progress.
        /// </summary>
        /// <value>The overlay message progress.</value>
        public virtual string OverlayMessageProgress { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [overlay visible].
        /// </summary>
        /// <value><c>true</c> if [overlay visible]; otherwise, <c>false</c>.</value>
        public virtual bool OverlayVisible { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            MessageBus.Current.Listen<OverlayEventArgs>()
                .Subscribe(s =>
                {
                    OverlayMessage = s.Message;
                    OverlayVisible = s.IsVisible;
                    OverlayMessageProgress = s.MessageProgress;
                    HasProgress = !string.IsNullOrWhiteSpace(s.MessageProgress);
                }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
