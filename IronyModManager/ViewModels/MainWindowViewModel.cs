// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 01-20-2020
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Reactive.Disposables;
using Avalonia.Controls;
using IronyModManager.Common.Events;
using IronyModManager.Common.ViewModels;
using IronyModManager.DI;
using IronyModManager.Services.Common;
using System.Linq;
using ReactiveUI;

namespace IronyModManager.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    public class MainWindowViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The theme setter
        /// </summary>
        private static readonly Func<Window, bool, bool> themeSetter = (window, isToggled) =>
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
        /// The previous locale
        /// </summary>
        private string previousLocale;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        public MainWindowViewModel()
        {
            Main = DIResolver.Get<MainControlViewModel>();
            previousLocale = DIResolver.Get<ILanguagesService>().GetSelected().Abrv;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the language selector.
        /// </summary>
        /// <value>The language selector.</value>
        public virtual MainControlViewModel Main { get; protected set; }

        /// <summary>
        /// Gets or sets the main window.
        /// </summary>
        /// <value>The main window.</value>
        public virtual Window MainWindow { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            themeSetter(MainWindow, Main.ThemeSelector.ToggleDarkThemeEnabled);

            var themeChanged = this.WhenAnyValue(p => p.Main.ThemeSelector.ToggleDarkThemeEnabled).Subscribe(p =>
            {
                themeSetter(MainWindow, p);
            }).DisposeWith(disposables);

            var languageChanged = this.WhenAnyValue(p => p.Main.LanguageSelector.SelectedLanguage).Subscribe(p =>
            {
                if (Main?.ThemeSelector?.IsActivated == true && previousLocale != p.Abrv)
                {
                    var args = new LocaleChangedEventArgs()
                    {
                        Locale = p.Abrv,
                        OldLocale = previousLocale
                    };
                    previousLocale = DIResolver.Get<ILanguagesService>().GetSelected().Abrv;
                    MessageBus.Current.SendMessage(args);
                };
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
