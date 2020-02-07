// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-15-2020
//
// Last Modified By : Mario
// Last Modified On : 02-07-2020
// ***********************************************************************
// <copyright file="BaseControl.cs" company="Mario">
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
    /// Class BaseControl.
    /// Implements the <see cref="Avalonia.ReactiveUI.ReactiveUserControl{TViewModel}" />
    /// </summary>
    /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
    /// <seealso cref="Avalonia.ReactiveUI.ReactiveUserControl{TViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public abstract class BaseControl<TViewModel> : ReactiveUserControl<TViewModel> where TViewModel : BaseViewModel
    {
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

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected virtual void OnActivated(IDisposable disposables)
        {
            IsActivated = true;
        }

        #endregion Methods
    }
}
