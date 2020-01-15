// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-15-2020
//
// Last Modified By : Mario
// Last Modified On : 01-15-2020
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
using IronyModManager.ViewModels;
using ReactiveUI;

namespace IronyModManager.Views
{
    /// <summary>
    /// Class BaseWindow.
    /// Implements the <see cref="Avalonia.Controls.Window" />
    /// Implements the <see cref="Avalonia.ReactiveUI.ReactiveWindow{TViewModel}" />
    /// </summary>
    /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
    /// <seealso cref="Avalonia.ReactiveUI.ReactiveWindow{TViewModel}" />
    /// <seealso cref="Avalonia.Controls.Window" />
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
