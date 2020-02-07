// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 02-07-2020
// ***********************************************************************
// <copyright file="LanguageControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
    /// Class LanguageControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class LanguageControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The language service
        /// </summary>
        private readonly ILanguagesService languageService;

        /// <summary>
        /// The previous language
        /// </summary>
        private ILanguage PreviousLanguage;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageControlViewModel" /> class.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        public LanguageControlViewModel(ILanguagesService languageService)
        {
            this.languageService = languageService;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the languages.
        /// </summary>
        /// <value>The languages.</value>
        [AutoRefreshLocalization]
        public virtual IEnumerable<ILanguage> Languages { get; protected set; }

        /// <summary>
        /// Gets the language text.
        /// </summary>
        /// <value>The language text.</value>
        [StaticLocalization(LocalizationResources.Languages.Name)]
        public virtual string LanguageText { get; }

        /// <summary>
        /// Gets or sets the selected language.
        /// </summary>
        /// <value>The selected language.</value>
        [AutoRefreshLocalization]
        public virtual ILanguage SelectedLanguage { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            Languages = languageService.Get();

            PreviousLanguage = SelectedLanguage = Languages.FirstOrDefault(p => p.IsSelected);

            var lanuageChanged = this.WhenAnyValue(p => p.SelectedLanguage).Subscribe(p =>
            {
                if (Languages?.Count() > 0 && p != null)
                {
                    if (languageService.SetSelected(Languages, p))
                    {
                        if (PreviousLanguage != p)
                        {
                            var args = new LocaleChangedEventArgs()
                            {
                                Locale = p.Abrv,
                                OldLocale = PreviousLanguage.Abrv
                            };
                            MessageBus.Current.SendMessage(args);
                            PreviousLanguage = p;
                        }
                    }
                }
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
