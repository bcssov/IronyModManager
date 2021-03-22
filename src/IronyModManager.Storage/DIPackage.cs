// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 03-16-2021
// ***********************************************************************
// <copyright file="DIPackage.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using IronyModManager.DI.Extensions;
using IronyModManager.Shared;
using IronyModManager.Storage.Common;
using SimpleInjector.Packaging;
using Container = SimpleInjector.Container;

namespace IronyModManager.Storage
{
    /// <summary>
    /// Class DIPackage.
    /// Implements the <see cref="SimpleInjector.Packaging.IPackage" />
    /// </summary>
    /// <seealso cref="SimpleInjector.Packaging.IPackage" />
    [ExcludeFromCoverage("Should not test external DI.")]
    public class DIPackage : IPackage
    {
        #region Fields

        /// <summary>
        /// The property changed
        /// </summary>
        private static readonly string PropChanged = nameof(INotifyPropertyChanged.PropertyChanged);

        #endregion Fields

        #region Methods

        /// <summary>
        /// Registers the set of services in the specified <paramref name="container" />.
        /// </summary>
        /// <param name="container">The container the set of services is registered into.</param>
        public void RegisterServices(Container container)
        {
            container.Register<IThemeType, ThemeType>();
            container.Register<IGameType, GameType>();
            container.RegisterSingleton<IDatabase, Database>();
            container.InterceptWith<PropertyChangedInterceptor<IDatabase>>(x => x == typeof(IDatabase), true);

            var tracker = new Jot.Tracker(new JsonStore());
            tracker.Configure<Database>().PersistOn(PropChanged);
            container.RegisterInitializer(d =>
            {
                tracker.Track(d.Instance);
            }, ctx => ctx.Registration.ImplementationType == typeof(Database));

            container.Register<IStorageProvider, Storage>();
            container.Register<INotificationPositionType, NotificationPositionType>();
        }

        #endregion Methods
    }
}
