// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-15-2020
//
// Last Modified By : Mario
// Last Modified On : 07-04-2021
// ***********************************************************************
// <copyright file="BaseWindow.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using IronyModManager.Common.ViewModels;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.Common.Views
{
    /// <summary>
    /// Class BaseWindow.
    /// Implements the <see cref="Avalonia.ReactiveUI.ReactiveWindow{TViewModel}" />
    /// Implements the <see cref="IronyModManager.Common.Views.IBaseWindow" />
    /// </summary>
    /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
    /// <seealso cref="Avalonia.ReactiveUI.ReactiveWindow{TViewModel}" />
    /// <seealso cref="IronyModManager.Common.Views.IBaseWindow" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public abstract class BaseWindow<TViewModel> : ReactiveWindow<TViewModel>, IBaseWindow where TViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The is activated property
        /// </summary>
        public static readonly StyledProperty<bool> IsActivatedProperty = AvaloniaProperty.Register<BaseWindow<TViewModel>, bool>(nameof(IsActivated), true);

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWindow{TViewModel}" /> class.
        /// </summary>
        public BaseWindow()
        {
            if (!Design.IsDesignMode)
            {
                this.WhenActivated(disposables =>
                {
                    Disposables = disposables;
                    OnActivated(disposables);
                    IsActivated = true;
                });
                PropertyChanged += BaseWindow_PropertyChanged;
                PositionChanged += BaseWindow_PositionChanged;
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
        /// Gets a value indicating whether this instance is center screen.
        /// </summary>
        /// <value><c>true</c> if this instance is center screen; otherwise, <c>false</c>.</value>
        public bool IsCenterScreen { get; private set; }

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
        }

        /// <summary>
        /// Handles the PositionChanged event of the BaseWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PixelPointEventArgs" /> instance containing the event data.</param>
        private void BaseWindow_PositionChanged(object sender, PixelPointEventArgs e)
        {
            IsCenterScreen = false;
        }

        /// <summary>
        /// Handles the PropertyChanged event of the BaseWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="AvaloniaPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void BaseWindow_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == WindowStartupLocationProperty)
            {
                IsCenterScreen = WindowStartupLocation == WindowStartupLocation.CenterScreen;
            }
        }

        #endregion Methods
    }
}
