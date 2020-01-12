// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 01-12-2020
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Services;

namespace IronyModManager.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="IronyModManager.ViewModels.ViewModelBase" />
    /// </summary>
    /// <seealso cref="IronyModManager.ViewModels.ViewModelBase" />
    public class MainWindowViewModel : ViewModelBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="model">The model.</param>
        public MainWindowViewModel(IPreferencesService service, SampleControlViewModel model)
        {
            Sample = model;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the greeting.
        /// </summary>
        /// <value>The greeting.</value>
        public virtual string Greeting => "Welcome to Avalonia!";

        /// <summary>
        /// Gets the sample.
        /// </summary>
        /// <value>The sample.</value>
        public SampleControlViewModel Sample { get; }

        #endregion Properties
    }
}
