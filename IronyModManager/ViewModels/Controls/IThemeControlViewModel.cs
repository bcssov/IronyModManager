// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 01-18-2020
// ***********************************************************************
// <copyright file="IThemeControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Reactive;
using IronyModManager.Common.ViewModels;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Interface IThemeControlViewModel
    /// Implements the <see cref="IronyModManager.Common.ViewModels.IViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.IViewModel" />
    public interface IThemeControlViewModel : IViewModel
    {
        #region Properties

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        string Text { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [toggle dark theme enabled].
        /// </summary>
        /// <value><c>true</c> if [toggle dark theme enabled]; otherwise, <c>false</c>.</value>
        bool ToggleDarkThemeEnabled { get; set; }

        /// <summary>
        /// Gets the toggle theme.
        /// </summary>
        /// <value>The toggle theme.</value>
        ReactiveCommand<Unit, Unit> ToggleTheme { get; }

        #endregion Properties
    }
}
