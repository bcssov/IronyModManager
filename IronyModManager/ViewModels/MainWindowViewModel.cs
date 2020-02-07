// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 02-07-2020
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Common.ViewModels;
using IronyModManager.DI;
using System.Linq;
using IronyModManager.Shared;

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
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        public MainWindowViewModel()
        {
            Main = DIResolver.Get<MainControlViewModel>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the language selector.
        /// </summary>
        /// <value>The language selector.</value>
        public virtual MainControlViewModel Main { get; protected set; }

        #endregion Properties
    }
}
