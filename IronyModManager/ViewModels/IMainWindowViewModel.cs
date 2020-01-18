// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 01-18-2020
// ***********************************************************************
// <copyright file="IMainWindowViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia.Controls;
using IronyModManager.Common.ViewModels;
using IronyModManager.ViewModels.Controls;

namespace IronyModManager.ViewModels
{
    /// <summary>
    /// Interface IMainWindowViewModel
    /// Implements the <see cref="IronyModManager.Common.ViewModels.IViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.IViewModel" />
    public interface IMainWindowViewModel : IViewModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the main window.
        /// </summary>
        /// <value>The main window.</value>
        Window MainWindow { get; set; }

        /// <summary>
        /// Gets the theme selector.
        /// </summary>
        /// <value>The theme selector.</value>
        IThemeControlViewModel ThemeSelector { get; }

        #endregion Properties
    }
}
