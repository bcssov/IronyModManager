// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-15-2020
//
// Last Modified By : Mario
// Last Modified On : 01-15-2020
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
using ReactiveUI;

namespace IronyModManager.Common.Views
{
    /// <summary>
    /// Class BaseControl.
    /// Implements the <see cref="Avalonia.ReactiveUI.ReactiveUserControl{TViewModel}" />
    /// </summary>
    /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
    /// <seealso cref="Avalonia.ReactiveUI.ReactiveUserControl{TViewModel}" />
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
