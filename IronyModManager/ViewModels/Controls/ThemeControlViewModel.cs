// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 01-14-2020
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
using System.Reactive.Linq;
using DynamicData;
using IronyModManager.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class ThemeControlViewModel.
    /// Implements the <see cref="IronyModManager.ViewModels.ViewModelBase" />
    /// </summary>
    /// <seealso cref="IronyModManager.ViewModels.ViewModelBase" />
    public class ThemeControlViewModel : ViewModelBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeControlViewModel" /> class.
        /// </summary>
        /// <param name="themeService">The theme service.</param>
        public ThemeControlViewModel(IThemeService themeService)
        {
            var themes = themeService.Get();

            var toggleEnabled = themes
                .AsObservableChangeSet(p => p)
                .ToCollection()
                .Select(p => p.FirstOrDefault(s => s.IsSelected).Type == Models.Enums.Theme.Dark)
                .Subscribe(p =>
                {
                    ToggleEnabled = p;
                });

            ToggleTheme = ReactiveCommand.Create(() =>
            {
                foreach (var item in themes)
                {
                    item.IsSelected = !item.IsSelected;
                }
                themeService.Save(themes);
            });
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
        public bool ToggleEnabled { get; set; }

        /// <summary>
        /// Gets the toggle theme.
        /// </summary>
        /// <value>The toggle theme.</value>
        public ReactiveCommand<Unit, Unit> ToggleTheme { get; }

        #endregion Properties
    }
}
