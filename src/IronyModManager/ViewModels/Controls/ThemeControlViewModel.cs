// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 07-10-2020
// ***********************************************************************
// <copyright file="ThemeControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using IronyModManager.Common.Events;
using IronyModManager.Common.ViewModels;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class ThemeControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ThemeControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The theme service
        /// </summary>
        private readonly IThemeService themeService;

        /// <summary>
        /// The previous theme
        /// </summary>
        private ITheme previousTheme;

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
        /// Gets or sets the selected theme.
        /// </summary>
        /// <value>The selected theme.</value>
        [AutoRefreshLocalization]
        public virtual ITheme SelectedTheme { get; protected set; }

        /// <summary>
        /// Gets or sets the themes.
        /// </summary>
        /// <value>The themes.</value>
        [AutoRefreshLocalization]
        public virtual IEnumerable<ITheme> Themes { get; protected set; }

        /// <summary>
        /// Gets the theme text.
        /// </summary>
        /// <value>The theme text.</value>
        [StaticLocalization(LocalizationResources.Themes.Name)]
        public virtual string ThemeText { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            Bind();

            var changed = this.WhenAnyValue(p => p.SelectedTheme).Where(p => p != null && Themes?.Count() > 0).Subscribe(p =>
            {
                if (themeService.SetSelected(Themes, p))
                {
                    if (previousTheme != p)
                    {
                        var args = new ThemeChangedEventArgs()
                        {
                            Theme = p
                        };
                        ReactiveUI.MessageBus.Current.SendMessage(args);
                        previousTheme = p;
                    }
                }
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// Binds this instance.
        /// </summary>
        private void Bind()
        {
            Themes = themeService.Get();

            previousTheme = SelectedTheme = Themes.FirstOrDefault(p => p.IsSelected);
        }

        #endregion Methods
    }
}
