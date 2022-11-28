// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 11-28-2022
//
// Last Modified By : Mario
// Last Modified On : 11-28-2022
// ***********************************************************************
// <copyright file="AppViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using IronyModManager.Common;
using IronyModManager.DI;
using IronyModManager.Platform.Fonts;
using IronyModManager.Services.Common;
using IronyModManager.Views;

namespace IronyModManager.ViewModels
{
    /// <summary>
    /// Class AppViewModel.
    /// </summary>
    public class AppViewModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppViewModel" /> class.
        /// </summary>
        public AppViewModel()
        {
            AboutCommand = MiniCommand.CreateFromTask(async () =>
            {
                AboutDialog dialog = new();
                dialog.FontFamily = ResolveFont().GetFontFamily();
                Avalonia.Controls.Window mainWindow = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
                await dialog.ShowDialog(mainWindow);
            });
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the about command.
        /// </summary>
        /// <value>The about command.</value>
        public MiniCommand AboutCommand { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Resolves the font.
        /// </summary>
        /// <returns>IFontFamily.</returns>
        private static IFontFamily ResolveFont()
        {
            var langService = DIResolver.Get<ILanguagesService>();
            var language = langService.GetSelected();
            var fontResolver = DIResolver.Get<IFontFamilyManager>();
            var font = fontResolver.ResolveFontFamily(language.Font);
            return font;
        }

        #endregion Methods
    }
}
