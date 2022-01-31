// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 01-31-2022
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using IronyModManager.Common.Events;
using IronyModManager.Common.ViewModels;
using IronyModManager.DI;
using IronyModManager.Implementation.Hotkey;
using IronyModManager.Implementation.MessageBus;
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
        #region Fields

        /// <summary>
        /// The overlay stack
        /// </summary>
        protected readonly List<OverlayQueue> overlayStack = new();

        /// <summary>
        /// The loop running
        /// </summary>
        protected bool loopRunning = false;

        /// <summary>
        /// The hotkey manager
        /// </summary>
        private readonly IHotkeyManager hotkeyManager;

        /// <summary>
        /// The overlay progress handler
        /// </summary>
        private readonly OverlayProgressHandler overlayProgressHandler;

        /// <summary>
        /// The queue lock
        /// </summary>
        private readonly object queueLock = new { };

        /// <summary>
        /// The writing state operation handler
        /// </summary>
        private readonly WritingStateOperationHandler writingStateOperationHandler;

        /// <summary>
        /// The current message identifier
        /// </summary>
        private long? currentMessageId;

        /// <summary>
        /// The last message identifier
        /// </summary>
        private long lastMessageId = 0;

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
            hotkeyManager = DIResolver.Get<IHotkeyManager>();
            if (!StaticResources.IsVerifyingContainer)
            {
                BindOverlay();
            }
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

        /// <summary>
        /// Gets or sets the register hotkey command.
        /// </summary>
        /// <value>The register hotkey command.</value>
        public virtual ReactiveCommand<string, Unit> RegisterHotkeyCommand { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Triggers the manual overlay.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        /// <param name="message">The message.</param>
        public void TriggerManualOverlay(long id, bool isVisible, string message)
        {
            QueueOverlay(new OverlayProgressEvent()
            {
                Id = id,
                IsVisible = isVisible,
                Message = message
            });
        }

        /// <summary>
        /// animate transition as an asynchronous operation.
        /// </summary>
        /// <param name="mainVisible">if set to <c>true</c> [main visible].</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
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
        /// Binds the overlay.
        /// </summary>
        protected void BindOverlay()
        {
            InitOverlayLoop();
            overlayDisposable?.Dispose();
            overlayDisposable = overlayProgressHandler.Subscribe(s =>
            {
                QueueOverlay(s);
            });
            if (Disposables != null)
            {
                overlayDisposable.DisposeWith(Disposables);
            }
        }

        /// <summary>
        /// Initializes the overlay loop.
        /// </summary>
        protected void InitOverlayLoop()
        {
            if (!loopRunning)
            {
                loopRunning = true;
                Task.Run(() => OverlayLoopAsync().ConfigureAwait(false));
            }
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            var state = NavigationState.Main;
            BindOverlay();

            ReactiveUI.MessageBus.Current.Listen<NavigationEventArgs>()
                .Subscribe(s =>
                {
                    ReactiveUI.MessageBus.Current.SendMessage(new ForceClosePopulsEventArgs());
                    state = s.State;
                    switch (s.State)
                    {
                        case NavigationState.ReadOnlyConflictSolver:
                        case NavigationState.ConflictSolver:
                            ConflictSolver.SelectedModCollection = s.SelectedCollection;
                            ConflictSolver.SelectedModsOrder = s.SelectedMods;
                            ConflictSolver.Conflicts = s.Results;
                            ConflictSolver.Initialize(s.State == NavigationState.ReadOnlyConflictSolver);
                            AnimateTransitionAsync(false).ConfigureAwait(true);
                            break;

                        default:
                            AnimateTransitionAsync(true).ConfigureAwait(true);
                            Main.Reset();
                            break;
                    }
                }).DisposeWith(disposables);

            writingStateOperationHandler.Subscribe(s =>
            {
                TriggerPreventShutdown(!s.CanShutdown);
            }).DisposeWith(disposables);

            RegisterHotkeyCommand = ReactiveCommand.CreateFromTask(async (string key) =>
            {
                if (!OverlayVisible)
                {
                    await hotkeyManager.HotKeyPressedAsync(state, key);
                }
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// overlay loop as an asynchronous operation.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected async Task OverlayLoopAsync()
        {
            void setOverlayProperties(OverlayProgressEvent e)
            {
                if (e.IsVisible != OverlayVisible)
                {
                    OverlayVisible = e.IsVisible;
                }
                if (e.Message != OverlayMessage)
                {
                    OverlayMessage = e.Message;
                }
                if (e.MessageProgress != OverlayMessageProgress)
                {
                    OverlayMessageProgress = e.MessageProgress;
                    HasProgress = !string.IsNullOrWhiteSpace(e.MessageProgress);
                }
            }
            while (true)
            {
                await Task.Delay(2);
                lock (queueLock)
                {
                    var now = DateTime.Now;
                    if (overlayStack.Any(p => now >= p.DateAdded) && currentMessageId.HasValue)
                    {
                        var overlays = overlayStack.Where(p => now >= p.DateAdded && p.Event.Id == currentMessageId.GetValueOrDefault()).OrderBy(p => p.DateAdded).ToList();
                        if (overlays.Count > 0)
                        {
                            OverlayQueue overlay = null;
                            if (overlays.Any(p => p.Event.IsVisible != OverlayVisible))
                            {
                                if (overlays.Any(p => p.Event.IsVisible == false))
                                {
                                    overlayStack.RemoveAll(p => p.Event.Id <= currentMessageId.GetValueOrDefault());
                                    if (overlayStack.Count > 0)
                                    {
                                        currentMessageId = overlayStack.OrderByDescending(p => p.Event.Id).FirstOrDefault().Event.Id;
                                    }
                                    else
                                    {
                                        currentMessageId = null;
                                    }
                                    if (currentMessageId.HasValue)
                                    {
                                        lastMessageId = currentMessageId.GetValueOrDefault();
                                    }
                                    else
                                    {
                                        lastMessageId++;
                                    }
                                    overlay = overlays.FirstOrDefault(p => p.Event.IsVisible == false);
                                }
                                else
                                {
                                    overlay = overlays.FirstOrDefault(p => p.Event.IsVisible != OverlayVisible);
                                    if (overlay != null)
                                    {
                                        overlayStack.Remove(overlay);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var item in overlays)
                                {
                                    overlayStack.Remove(item);
                                }
                                overlay = overlays.LastOrDefault();
                            }
                            if (overlay != null)
                            {
                                setOverlayProperties(overlay.Event);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Queues the overlay.
        /// </summary>
        /// <param name="e">The e.</param>
        protected void QueueOverlay(OverlayProgressEvent e)
        {
            lock (queueLock)
            {
                if (!overlayStack.Any(p => p.Event == e))
                {
                    if (!currentMessageId.HasValue && e.Id < lastMessageId)
                    {
                        return;
                    }
                    if (!currentMessageId.HasValue && OverlayVisible != e.IsVisible && e.IsVisible)
                    {
                        currentMessageId = e.Id;
                        lastMessageId = e.Id;
                    }
                    overlayStack.Add(new OverlayQueue() { Event = e, DateAdded = DateTime.Now });
                }
            }
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class OverlayQueue.
        /// </summary>
        protected class OverlayQueue
        {
            #region Properties

            /// <summary>
            /// Gets or sets the date added.
            /// </summary>
            /// <value>The date added.</value>
            public DateTime DateAdded { get; set; }

            /// <summary>
            /// Gets or sets the event.
            /// </summary>
            /// <value>The event.</value>
            public OverlayProgressEvent Event { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
