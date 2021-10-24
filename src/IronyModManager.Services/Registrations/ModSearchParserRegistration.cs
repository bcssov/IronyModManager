// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 10-24-2021
//
// Last Modified By : Mario
// Last Modified On : 10-24-2021
// ***********************************************************************
// <copyright file="ModSearchParserRegistration.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.Localization;
using IronyModManager.Parser.Common.Mod.Search;
using IronyModManager.Services.Common;
using IronyModManager.Shared;

namespace IronyModManager.Services.Registrations
{
    /// <summary>
    /// Class ModSearchParserRegistration.
    /// Implements the <see cref="IronyModManager.Shared.PostStartup" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.PostStartup" />
    public class ModSearchParserRegistration : PostStartup
    {
        #region Methods

        /// <summary>
        /// Called when [post startup].
        /// </summary>
        public override void OnPostStartup()
        {
            var languages = DIResolver.Get<ILanguagesService>().Get();
            var manager = DIResolver.Get<ILocalizationManager>();
            var translationKeys = LocalizationRegistry.GetTranslationKeys();
            foreach (var key in translationKeys)
            {
                foreach (var language in languages)
                {
                    LocalizationRegistry.RegisterTranslation(language.Abrv, key, manager.GetResource(language.Abrv, key));
                }
            }
        }

        #endregion Methods
    }
}
