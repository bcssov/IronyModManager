// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-23-2020
// ***********************************************************************
// <copyright file="Extensions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.DI.Extensions;
using SimpleInjector;

namespace IronyModManager.Localization
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    public static class Extensions
    {
        #region Methods

        /// <summary>
        /// Registers the localization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container">The container.</param>
        public static void RegisterLocalization<T>(this Container container) where T : class, ILocalizableViewModel
        {
            container.Register<T>();
            container.InterceptWith<LocalizationInterceptor<T>>(x => x == typeof(T), false);
        }

        /// <summary>
        /// Registers the localization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <param name="container">The container.</param>
        public static void RegisterLocalization<T, T2>(this Container container) where T : class, ILocalizableModel where T2 : class, T
        {
            container.Register<T, T2>();
            container.InterceptWith<LocalizationInterceptor<T>>(x => x == typeof(T), false);
        }

        #endregion Methods
    }
}
