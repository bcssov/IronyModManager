// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-19-2020
// ***********************************************************************
// <copyright file="Extensions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Common.ViewModels;
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
        public static void RegisterLocalization<T>(this Container container) where T : BaseViewModel
        {
            container.Register<T>();
            container.InterceptWith<LocalizationInterceptor<T>>(x => x == typeof(T), false);
        }

        #endregion Methods
    }
}
