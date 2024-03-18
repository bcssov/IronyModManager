// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 01-15-2020
//
// Last Modified By : Mario
// Last Modified On : 03-18-2024
// ***********************************************************************
// <copyright file="DIPackage.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.DI.Extensions;
using IronyModManager.Localization;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class DIPackage.
    /// Implements the <see cref="SimpleInjector.Packaging.IPackage" />
    /// </summary>
    /// <seealso cref="SimpleInjector.Packaging.IPackage" />
    [ExcludeFromCoverage("Should not test external DI.")]
    public class DIPackage : IPackage
    {
        #region Methods

        /// <summary>
        /// Registers the set of services in the specified <paramref name="container" />.
        /// </summary>
        /// <param name="container">The container the set of services is registered into.</param>
        public void RegisterServices(Container container)
        {
            container.RegisterModel<IPreferences, Preferences>();
            container.RegisterLocalization<ITheme, Theme>();
            container.RegisterLocalization<ILanguage, Language>();
            container.RegisterModel<IWindowState, WindowState>();
            container.RegisterLocalization<IGame, Game>();
            container.RegisterLocalization<IMod, Mod>();
            container.RegisterModel<IAppState, AppState>();
            container.RegisterModel<IModCollection, ModCollection>();
            container.RegisterModelWithoutTransientWarning<IConflictResult, ConflictResult>();
            container.RegisterModel<IPriorityDefinitionResult, PriorityDefinitionResult>();
            container.RegisterModel<IGameSettings, GameSettings>();
            container.RegisterModel<IUpdateSettings, UpdateSettings>();
            container.RegisterModel<IHashFileReport, HashFileReport>();
            container.RegisterModel<IHashReport, HashReport>();
            container.RegisterModel<IExternalEditor, ExternalEditor>();
            container.RegisterModel<IExternalEditorFiles, ExternalEditorFiles>();
            container.RemoveTransientWarning<IExternalEditorFiles>();
            container.RegisterModel<IModInstallationResult, ModInstallationResult>();
            container.RegisterModel<IPermissionCheckResult, PermissionCheckResult>();
            container.RegisterLocalization<IDLC, DLC>();
            container.RegisterLocalization<INotificationPosition, NotificationPosition>();
            container.RegisterModel<IPromptNotifications, PromptNotifications>();
            container.RegisterModel<IValidateResult, ValidateResult>();
            container.Register<IModCollectionSourceInfo, ModCollectionSourceInfo>();
            container.RegisterModel<IModIgnoreConfiguration, ModIgnoreConfiguration>();
            container.RegisterLocalization<IGameLanguage, GameLanguage>();
            container.RegisterModel<IConflictSolverColors, ConflictSolverColors>();
        }

        #endregion Methods
    }
}
