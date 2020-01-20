// ***********************************************************************
// Assembly         : IronyModManager.Common
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 01-19-2020
// ***********************************************************************
// <copyright file="BaseViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Reactive.Disposables;
using IronyModManager.DI;
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
    public abstract class BaseViewModel : ReactiveObject, IViewModel, IActivatableViewModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseViewModel"/> class.
        /// </summary>
        public BaseViewModel()
        {
            Activator = DIResolver.Get<ViewModelActivator>();
            this.WhenActivated((CompositeDisposable disposables) =>
            {
                OnActivated(disposables);
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

        #endregion Properties

        #region Methods

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
        }

        #endregion Methods
    }
}
