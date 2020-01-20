// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 01-20-2020
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
using IronyModManager.Common;
using IronyModManager.Common.ViewModels;
using IronyModManager.Localization;
using IronyModManager.Services;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class ThemeControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
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
        [Localization("NightMode")]
        public virtual string Text => "NightMode";

        /// <summary>
        /// Gets or sets a value indicating whether [toggle dark theme enabled].
        /// </summary>
        /// <value><c>true</c> if [toggle dark theme enabled]; otherwise, <c>false</c>.</value>
        public virtual bool ToggleDarkThemeEnabled { get; set; }

        /// <summary>
        /// Gets the toggle theme.
        /// </summary>
        /// <value>The toggle theme.</value>
        public virtual ReactiveCommand<Unit, Unit> ToggleTheme { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            var themes = themeService.Get();

            ToggleDarkThemeEnabled = themes.FirstOrDefault(p => p.IsSelected).Type == Models.Common.Enums.Theme.Dark;

            var toggleEnabled = themes.ToSourceList().Connect().WhenPropertyChanged(p => p.IsSelected).Subscribe(p =>
            {
                if (p.Sender.IsSelected)
                {
                    ToggleDarkThemeEnabled = p.Sender.Type == Models.Common.Enums.Theme.Dark;
                }
            }).DisposeWith(disposables);

            ToggleTheme = ReactiveCommand.Create(() =>
            {
                foreach (var item in themes)
                {
                    item.IsSelected = !item.IsSelected;
                }
                themeService.Save(themes.FirstOrDefault(p => p.IsSelected));
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
