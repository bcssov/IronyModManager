// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 02-14-2021
// ***********************************************************************
// <copyright file="MainControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Common.ViewModels;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;

namespace IronyModManager.ViewModels
{
    /// <summary>
    /// Class MainControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class MainControlViewModel : BaseViewModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainControlViewModel" /> class.
        /// </summary>
        /// <param name="themeControl">The theme control.</param>
        /// <param name="languageControl">The language control.</param>
        /// <param name="gameControl">The game control.</param>
        /// <param name="modControl">The mod control.</param>
        /// <param name="options">The options.</param>
        /// <param name="actions">The actions.</param>
        public MainControlViewModel(ThemeControlViewModel themeControl,
            LanguageControlViewModel languageControl,
            GameControlViewModel gameControl,
            ModHolderControlViewModel modControl,
            OptionsControlViewModel options, ActionsControlViewModel actions)
        {
            ThemeSelector = themeControl;
            LanguageSelector = languageControl;
            GameSelector = gameControl;
            ModHolder = modControl;
            Options = options;
            Actions = actions;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the actions.
        /// </summary>
        /// <value>The actions.</value>
        public virtual ActionsControlViewModel Actions { get; protected set; }

        /// <summary>
        /// Gets or sets the game selector.
        /// </summary>
        /// <value>The game selector.</value>
        public virtual GameControlViewModel GameSelector { get; protected set; }

        /// <summary>
        /// Gets or sets the language selector.
        /// </summary>
        /// <value>The language selector.</value>
        public virtual LanguageControlViewModel LanguageSelector { get; protected set; }

        /// <summary>
        /// Gets or sets the mod holder.
        /// </summary>
        /// <value>The mod holder.</value>
        public virtual ModHolderControlViewModel ModHolder { get; protected set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>The options.</value>
        public virtual OptionsControlViewModel Options { get; protected set; }

        /// <summary>
        /// Gets the theme selector.
        /// </summary>
        /// <value>The theme selector.</value>
        public virtual ThemeControlViewModel ThemeSelector { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public virtual void Reset()
        {
            ModHolder.Reset();
        }

        #endregion Methods
    }
}
