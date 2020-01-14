// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 01-14-2020
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using Avalonia.Controls;
using IronyModManager.ViewModels.Controls;
using ReactiveUI;

namespace IronyModManager.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="IronyModManager.ViewModels.ViewModelBase" />
    /// Implements the <see cref="ReactiveUI.IActivatableViewModel" />
    /// </summary>
    /// <seealso cref="ReactiveUI.IActivatableViewModel" />
    /// <seealso cref="IronyModManager.ViewModels.ViewModelBase" />
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// The theme setter
        /// </summary>
        private static Func<Window, bool, bool> themeSetter = (window, isToggled) =>
        {
            if (window != null)
            {
                var style = isToggled ? Constants.Themes.DarkTheme : Constants.Themes.LightTheme;

                window.Styles.Clear();
                window.Styles.Add(style);
                return true;
            }
            return false;
        };

        /// <summary>
        /// Gets or sets the main window.
        /// </summary>
        /// <value>The main window.</value>
        private Window mainWindow;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        /// <param name="themeSelector">The theme selector.</param>
        public MainWindowViewModel(ThemeControlViewModel themeSelector)
        {
            ThemeSelector = themeSelector;

            var toggleEnabled = this.WhenAnyValue(p => p.ThemeSelector.ToggleDarkThemeEnabled).Subscribe(p =>
            {
                themeSetter(MainWindow, p);
            });
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the main window.
        /// </summary>
        /// <value>The main window.</value>
        public Window MainWindow
        {
            get
            {
                return mainWindow;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref mainWindow, value);
                themeSetter(value, ThemeSelector.ToggleDarkThemeEnabled);
            }
        }

        /// <summary>
        /// Gets or sets my property.
        /// </summary>
        /// <value>My property.</value>
        public ThemeControlViewModel ThemeSelector { get; }

        #endregion Properties
    }
}
