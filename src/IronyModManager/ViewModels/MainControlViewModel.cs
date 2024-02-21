// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 02-21-2024
// ***********************************************************************
// <copyright file="MainControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <param name="themeControl">The theme control.</param>
    /// <param name="languageControl">The language control.</param>
    /// <param name="gameControl">The game control.</param>
    /// <param name="modControl">The mod control.</param>
    /// <param name="options">The options.</param>
    /// <param name="actions">The actions.</param>
    /// <remarks>Initializes a new instance of the <see cref="MainControlViewModel" /> class.</remarks>
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class MainControlViewModel(
        ThemeControlViewModel themeControl,
        LanguageControlViewModel languageControl,
        GameControlViewModel gameControl,
        ModHolderControlViewModel modControl,
        OptionsControlViewModel options,
        ActionsControlViewModel actions) : BaseViewModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the actions.
        /// </summary>
        /// <value>The actions.</value>
        public virtual ActionsControlViewModel Actions { get; protected set; } = actions;

        /// <summary>
        /// Gets or sets the game selector.
        /// </summary>
        /// <value>The game selector.</value>
        public virtual GameControlViewModel GameSelector { get; protected set; } = gameControl;

        /// <summary>
        /// Gets or sets the language selector.
        /// </summary>
        /// <value>The language selector.</value>
        public virtual LanguageControlViewModel LanguageSelector { get; protected set; } = languageControl;

        /// <summary>
        /// Gets or sets the mod holder.
        /// </summary>
        /// <value>The mod holder.</value>
        public virtual ModHolderControlViewModel ModHolder { get; protected set; } = modControl;

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>The options.</value>
        public virtual OptionsControlViewModel Options { get; protected set; } = options;

        /// <summary>
        /// Gets the theme selector.
        /// </summary>
        /// <value>The theme selector.</value>
        public virtual ThemeControlViewModel ThemeSelector { get; protected set; } = themeControl;

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
