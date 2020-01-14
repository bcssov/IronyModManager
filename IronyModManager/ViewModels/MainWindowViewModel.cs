// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 01-13-2020
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.ViewModels.Controls;

namespace IronyModManager.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="IronyModManager.ViewModels.ViewModelBase" />
    /// </summary>
    /// <seealso cref="IronyModManager.ViewModels.ViewModelBase" />
    public class MainWindowViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets my property.
        /// </summary>
        /// <value>My property.</value>
        public ThemeControlViewModel ThemeSelector => DI.DIResolver.Get<ThemeControlViewModel>();

        #endregion Properties
    }
}
