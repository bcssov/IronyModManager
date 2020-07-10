// ***********************************************************************
// Assembly         : IronyModManager.Common
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 07-10-2020
// ***********************************************************************
// <copyright file="BaseViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using IronyModManager.Common.Events;
using IronyModManager.DI;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.Common.ViewModels
{
    /// <summary>
    /// Class BaseViewModel.
    /// Implements the <see cref="ReactiveUI.ReactiveObject" />
    /// Implements the <see cref="IronyModManager.Common.ViewModels.IViewModel" />
    /// Implements the <see cref="ReactiveUI.IActivatableViewModel" />
    /// </summary>
    /// <seealso cref="ReactiveUI.ReactiveObject" />
    /// <seealso cref="IronyModManager.Common.ViewModels.IViewModel" />
    /// <seealso cref="ReactiveUI.IActivatableViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public abstract class BaseViewModel : ReactiveObject, IViewModel, IActivatableViewModel
    {
        #region Fields

        /// <summary>
        /// Gets or sets a value indicating whether this instance is activated.
        /// </summary>
        /// <value><c>true</c> if this instance is activated; otherwise, <c>false</c>.</value>
        private bool isActivated;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseViewModel" /> class.
        /// </summary>
        public BaseViewModel()
        {
            Activator = DIResolver.Get<ViewModelActivator>();
            MessageBus = DIResolver.Get<Shared.MessageBus.IMessageBus>();
            this.WhenActivated((CompositeDisposable disposables) =>
            {
                Disposables = disposables;
                OnActivated(disposables);
                IsActivated = true;
            });
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the activator.
        /// </summary>
        /// <value>The activator.</value>
        public ViewModelActivator Activator { get; }

        /// <summary>
        /// Gets the actual type.
        /// </summary>
        /// <value>The actual type.</value>
        public virtual Type ActualType => GetType();

        /// <summary>
        /// Gets or sets a value indicating whether this instance is activated.
        /// </summary>
        /// <value><c>true</c> if this instance is activated; otherwise, <c>false</c>.</value>
        public bool IsActivated
        {
            get => isActivated;
            protected set => this.RaiseAndSetIfChanged(ref isActivated, value);
        }

        /// <summary>
        /// Gets the message bus.
        /// </summary>
        /// <value>The message bus.</value>
        public Shared.MessageBus.IMessageBus MessageBus { get; }

        /// <summary>
        /// Gets the disposables.
        /// </summary>
        /// <value>The disposables.</value>
        protected CompositeDisposable Disposables { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [locale changed].
        /// </summary>
        /// <param name="newLocale">The new locale.</param>
        /// <param name="oldLocale">The old locale.</param>
        public virtual void OnLocaleChanged(string newLocale, string oldLocale)
        {
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        public void OnPropertyChanged(string methodName)
        {
            IReactiveObjectExtensions.RaisePropertyChanged(this, methodName);
        }

        /// <summary>
        /// Called when [property changing].
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        public void OnPropertyChanging(string methodName)
        {
            IReactiveObjectExtensions.RaisePropertyChanging(this, methodName);
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected virtual void OnActivated(CompositeDisposable disposables)
        {
            ReactiveUI.MessageBus.Current.Listen<LocaleChangedEventArgs>()
                .Subscribe(x =>
                {
                    OnLocaleChanged(x.Locale, x.OldLocale);
                }).DisposeWith(disposables);
            ReactiveUI.MessageBus.Current.Listen<SelectedGameChangedEventArgs>()
                .Subscribe(t =>
                {
                    OnSelectedGameChanged(t.Game);
                }).DisposeWith(disposables);
        }

        /// <summary>
        /// Called when [selected game changed].
        /// </summary>
        /// <param name="game">The game.</param>
        protected virtual void OnSelectedGameChanged(IGame game)
        {
        }

        /// <summary>
        /// Triggers the overlay.
        /// </summary>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        /// <param name="message">The message.</param>
        /// <param name="progress">The progress.</param>
        protected virtual void TriggerOverlay(bool isVisible, string message = Constants.EmptyParam, string progress = Constants.EmptyParam)
        {
            TriggerOverlayAsync(isVisible, message, progress).ConfigureAwait(false);
        }

        /// <summary>
        /// Triggers the overlay asynchronous.
        /// </summary>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        /// <param name="message">The message.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>Task.</returns>
        protected virtual async Task TriggerOverlayAsync(bool isVisible, string message = Constants.EmptyParam, string progress = Constants.EmptyParam)
        {
            var args = new OverlayProgressEvent()
            {
                IsVisible = isVisible,
                Message = message,
                MessageProgress = progress
            };
            await MessageBus.PublishAsync(args);
            await Task.Run(() =>
            {
                ReactiveUI.MessageBus.Current.SendMessage(new ForceClosePopulsEventArgs());
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Triggers the prevent shutdown.
        /// </summary>
        /// <param name="cannotShutdown">if set to <c>true</c> [cannot shutdown].</param>
        protected virtual void TriggerPreventShutdown(bool cannotShutdown)
        {
            Task.Run(() =>
            {
                var args = new ShutdownStateEventArgs()
                {
                    PreventShutdown = cannotShutdown
                };
                ReactiveUI.MessageBus.Current.SendMessage(args);
            }).ConfigureAwait(false);
        }

        #endregion Methods
    }
}
