// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 01-15-2020
// ***********************************************************************
// <copyright file="ThemeControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using IronyModManager.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class ThemeControlViewModel.
    /// Implements the <see cref="IronyModManager.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.ViewModels.BaseViewModel" />
    public class ThemeControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The theme service
        /// </summary>
        private readonly IThemeService themeService;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeControlViewModel" /> class.
        /// </summary>
        /// <param name="themeService">The theme service.</param>
        public ThemeControlViewModel(IThemeService themeService)
        {
            this.themeService = themeService;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text => "Night Mode";

        /// <summary>
        /// Gets or sets a value indicating whether [toggle enabled].
        /// </summary>
        /// <value><c>true</c> if [toggle enabled]; otherwise, <c>false</c>.</value>
        [Reactive]
        public bool ToggleDarkThemeEnabled { get; set; }

        /// <summary>
        /// Gets the toggle theme.
        /// </summary>
        /// <value>The toggle theme.</value>
        [Reactive]
        public ReactiveCommand<Unit, Unit> ToggleTheme { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            var themes = themeService.Get();

            ToggleDarkThemeEnabled = themes.FirstOrDefault(p => p.IsSelected).Type == Models.Enums.Theme.Dark;

            var toggleEnabled = themes.ToSourceList().Connect().WhenAnyPropertyChanged().Subscribe(p =>
             {
                 if (p.IsSelected)
                 {
                     ToggleDarkThemeEnabled = p.Type == Models.Enums.Theme.Dark;
                 }
             }).DisposeWith(disposables);

            ToggleTheme = ReactiveCommand.Create(() =>
            {
                foreach (var item in themes)
                {
                    item.IsSelected = !item.IsSelected;
                }
                themeService.Save(themes);
            }).DisposeWith(disposables);
        }

        #endregion Methods
    }
}
