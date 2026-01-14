// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-15-2020
//
// Last Modified By : Mario
// Last Modified On : 12-08-2025
// ***********************************************************************
// <copyright file="BaseControl.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using IronyModManager.Common.Events;
using IronyModManager.Common.ViewModels;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.Common.Views
{
    /// <summary>
    /// Class BaseControl.
    /// Implements the <see cref="Avalonia.ReactiveUI.ReactiveUserControl{TViewModel}" />
    /// </summary>
    /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
    /// <seealso cref="Avalonia.ReactiveUI.ReactiveUserControl{TViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public abstract class BaseControl<TViewModel> : ReactiveUserControl<TViewModel> where TViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The is activated property
        /// </summary>
        public static readonly StyledProperty<bool> IsActivatedProperty = AvaloniaProperty.Register<BaseControl<TViewModel>, bool>(nameof(IsActivated), true);

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseControl{TViewModel}" /> class.
        /// </summary>
        public BaseControl()
        {
            if (!Design.IsDesignMode)
            {
                this.WhenActivated(disposables =>
                {
                    Disposables = disposables;
                    OnActivated(disposables);
                    IsActivated = true;
                });
            }
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is activated.
        /// </summary>
        /// <value><c>true</c> if this instance is activated; otherwise, <c>false</c>.</value>
        public bool IsActivated
        {
            get => GetValue(IsActivatedProperty);
            protected set => SetValue(IsActivatedProperty, value);
        }

        /// <summary>
        /// Gets the disposables.
        /// </summary>
        /// <value>The disposables.</value>
        protected CompositeDisposable Disposables { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected virtual void OnActivated(CompositeDisposable disposables)
        {
            MessageBus.Current.Listen<LocaleChangedEventArgs>()
                .Subscribe(x =>
                {
                    OnLocaleChanged(x.Locale, x.OldLocale);
                }).DisposeWith(disposables);
            MessageBus.Current.Listen<SelectedGameChangedEventArgs>()
                .Subscribe(t =>
                {
                    OnSelectedGameChanged(t.Game);
                }).DisposeWith(disposables);
        }

        /// <summary>
        /// Called when [locale changed].
        /// </summary>
        /// <param name="newLocale">The new locale.</param>
        /// <param name="oldLocale">The old locale.</param>
        protected virtual void OnLocaleChanged(string newLocale, string oldLocale)
        {
        }

        /// <summary>
        /// Called when [selected game changed].
        /// </summary>
        /// <param name="game">The game.</param>
        protected virtual void OnSelectedGameChanged(IGame game)
        {
        }

        #endregion Methods
    }
}
