// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-15-2020
//
// Last Modified By : Mario
// Last Modified On : 03-01-2020
// ***********************************************************************
// <copyright file="BaseWindow.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
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
    /// </summary>
    /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
    /// <seealso cref="Avalonia.ReactiveUI.ReactiveWindow{TViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public abstract class BaseWindow<TViewModel> : ReactiveWindow<TViewModel> where TViewModel : BaseViewModel
    {
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
                    IsActivated = true;
                    OnActivated(disposables);
                });
            }
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is activated.
        /// </summary>
        /// <value><c>true</c> if this instance is activated; otherwise, <c>false</c>.</value>
        public bool IsActivated { get; protected set; }

        /// <summary>
        /// Gets the disposables.
        /// </summary>
        /// <value>The disposables.</value>
        protected IDisposable Disposables { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected virtual void OnActivated(IDisposable disposables)
        {
        }

        #endregion Methods
    }
}
