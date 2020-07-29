// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 07-29-2020
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using IronyModManager.Common.Events;
using IronyModManager.Common.ViewModels;
using IronyModManager.DI;
using System.Linq;
using IronyModManager.Implementation.MessageBus;
using IronyModManager.Shared;

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
        #region Fields

        /// <summary>
        /// The overlay progress handler
        /// </summary>
        private readonly OverlayProgressHandler overlayProgressHandler;

        /// <summary>
        /// The writing state operation handler
        /// </summary>
        private readonly WritingStateOperationHandler writingStateOperationHandler;

        /// <summary>
        /// The overlay disposable
        /// </summary>
        private IDisposable overlayDisposable;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        public MainWindowViewModel()
        {
            Main = DIResolver.Get<MainControlViewModel>();
            MainVisible = true;
            MainOpacity = 1;
            ConflictSolver = DIResolver.Get<MainConflictSolverControlViewModel>();
            writingStateOperationHandler = DIResolver.Get<WritingStateOperationHandler>();
            overlayProgressHandler = DIResolver.Get<OverlayProgressHandler>();
            overlayDisposable = overlayProgressHandler.Message.Subscribe(s =>
            {
                OverlayMessage = s.Message;
                OverlayVisible = s.IsVisible;
                OverlayMessageProgress = s.MessageProgress;
                HasProgress = !string.IsNullOrWhiteSpace(s.MessageProgress);
            });
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the conflict solver.
        /// </summary>
        /// <value>The conflict solver.</value>
        public virtual MainConflictSolverControlViewModel ConflictSolver { get; protected set; }

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
        /// Gets or sets the main opacity.
        /// </summary>
        /// <value>The main opacity.</value>
        public virtual double MainOpacity { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [main visible].
        /// </summary>
        /// <value><c>true</c> if [main visible]; otherwise, <c>false</c>.</value>
        public virtual bool MainVisible { get; protected set; }

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
        /// Triggers the manual overlay.
        /// </summary>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        /// <param name="message">The message.</param>
        public void TriggerManualOverlay(bool isVisible, string message)
        {
            OverlayMessage = message;
            OverlayVisible = isVisible;
        }

        /// <summary>
        /// animate transition as an asynchronous operation.
        /// </summary>
        /// <param name="mainVisible">if set to <c>true</c> [main visible].</param>
        protected virtual async Task AnimateTransitionAsync(bool mainVisible)
        {
            // For the love of me I cannot find a good animation type from Avalonia. All I can say is WTF Avalonia.
            MainVisible = mainVisible;
            MainOpacity = 0;
            while (MainOpacity < 1.0)
            {
                var opacity = MainOpacity + 0.02;
                if (opacity > 1.0)
                {
                    opacity = 1;
                }
                MainOpacity = opacity;
                await Task.Delay(5);
            }
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            overlayDisposable?.Dispose();
            overlayDisposable = overlayProgressHandler.Message.Subscribe(s =>
            {
                OverlayMessage = s.Message;
                OverlayVisible = s.IsVisible;
                OverlayMessageProgress = s.MessageProgress;
                HasProgress = !string.IsNullOrWhiteSpace(s.MessageProgress);
            }).DisposeWith(disposables);
            ReactiveUI.MessageBus.Current.Listen<NavigationEventArgs>()
                .Subscribe(s =>
                {
                    ReactiveUI.MessageBus.Current.SendMessage(new ForceClosePopulsEventArgs());
                    switch (s.State)
                    {
                        case NavigationState.ConflictSolver:
                            ConflictSolver.SelectedModCollection = s.SelectedCollection;
                            ConflictSolver.SelectedModsOrder = s.SelectedMods;
                            ConflictSolver.Conflicts = s.Results;
                            ConflictSolver.Reset();
                            AnimateTransitionAsync(false).ConfigureAwait(true);
                            break;

                        default:
                            AnimateTransitionAsync(true).ConfigureAwait(true);
                            Main.Reset();
                            break;
                    }
                }).DisposeWith(disposables);

            writingStateOperationHandler.Message.Subscribe(s =>
            {
                TriggerPreventShutdown(!s.CanShutdown);
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
