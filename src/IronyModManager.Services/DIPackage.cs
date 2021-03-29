// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 03-27-2021
// ***********************************************************************
// <copyright file="DIPackage.cs" company="IronyModManager.Services">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.DI.Extensions;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace IronyModManager.Services
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
            container.Register<IPreferencesService, PreferencesService>();
            container.Register<IThemeService, ThemeService>();
            container.Register<ILanguagesService, LanguagesService>();
            container.Register<IWindowStateService, WindowStateService>();
            container.Register<IGameService, GameService>();
            container.Register<IModService, ModService>();
            container.Register<IAppStateService, AppStateService>();
            container.Register<IModCollectionService, ModCollectionService>();
            container.Register<IModPatchCollectionService, ModPatchCollectionService>();
            container.Register<IModMergeService, ModMergeService>();
            container.Register<IUpdaterService, UpdaterService>();
            container.RemoveMixedLifetimeWarning<IUpdaterService>();
            container.Register<IExternalEditorService, ExternalEditorService>();
            container.Register<IPermissionCheckService, PermissionCheckService>();
            container.Register<IDLCService, DLCService>();
            container.Register<INotificationPositionSettingsService, NotificationPositionSettingsService>();
            container.Register<IPromptNotificationsService, PromptNotificationsService>();
            container.Register<IReportExportService, ReportExportService>();
        }

        #endregion Methods
    }
}
