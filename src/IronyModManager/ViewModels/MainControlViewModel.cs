// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 02-08-2020
// ***********************************************************************
// <copyright file="MainControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Common.ViewModels;
using IronyModManager.DI;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;

namespace IronyModManager.ViewModels
{
    /// <summary>
    /// Class MainControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class MainControlViewModel : BaseViewModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainControlViewModel" /> class.
        /// </summary>
        public MainControlViewModel()
        {
            ThemeSelector = DIResolver.Get<ThemeControlViewModel>();
            LanguageSelector = DIResolver.Get<LanguageControlViewModel>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the language selector.
        /// </summary>
        /// <value>The language selector.</value>
        public virtual LanguageControlViewModel LanguageSelector { get; protected set; }

        /// <summary>
        /// Gets the theme selector.
        /// </summary>
        /// <value>The theme selector.</value>
        public virtual ThemeControlViewModel ThemeSelector { get; protected set; }

        #endregion Properties
    }
}
