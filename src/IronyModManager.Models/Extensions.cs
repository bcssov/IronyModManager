// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 05-11-2020
//
// Last Modified By : Mario
// Last Modified On : 05-11-2020
// ***********************************************************************
// <copyright file="Extensions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.DI.Extensions;
using IronyModManager.Models.Common;
using SimpleInjector;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    public static class Extensions
    {
        #region Methods

        /// <summary>
        /// Registers the model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <param name="container">The container.</param>
        public static void RegisterModel<T, T2>(this Container container) where T : class, IModel where T2 : BaseModel, T
        {
            container.Register<T, T2>();
            container.InterceptWith<PropertyChangedInterceptor<T>>(x => x == typeof(T), true);
        }

        /// <summary>
        /// Registers the model without transient warning.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <param name="container">The container.</param>
        public static void RegisterModelWithoutTransientWarning<T, T2>(this Container container) where T : class, IModel where T2 : BaseModel, T
        {
            container.Register<T, T2>();
            container.InterceptWith<PropertyChangedInterceptor<T>>(x => x == typeof(T), true);
            var registration = container.GetRegistration(typeof(T)).Registration;
            registration.SuppressDiagnosticWarning(SimpleInjector.Diagnostics.DiagnosticType.DisposableTransientComponent, "Transient disposable is okay at times.");
        }

        #endregion Methods
    }
}
