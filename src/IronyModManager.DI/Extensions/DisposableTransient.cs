// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 05-11-2020
//
// Last Modified By : Mario
// Last Modified On : 05-11-2020
// ***********************************************************************
// <copyright file="DisposableTransient.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using SimpleInjector;

namespace IronyModManager.DI.Extensions
{
    /// <summary>
    /// Class DisposableTransient.
    /// </summary>
    public static class DisposableTransient
    {
        #region Methods

        /// <summary>
        /// Registers the without transient warning.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container">The container.</param>
        public static void RegisterWithoutTransientWarning<T>(this Container container) where T : class
        {
            container.Register<T>();
            var registration = container.GetRegistration(typeof(T)).Registration;
            registration.SuppressDiagnosticWarning(SimpleInjector.Diagnostics.DiagnosticType.DisposableTransientComponent, "Transient disposable is okay at times.");
        }

        /// <summary>
        /// Registers the without transient warning.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <param name="container">The container.</param>
        public static void RegisterWithoutTransientWarning<T, T2>(this Container container) where T : class where T2 : class, T
        {
            container.Register<T, T2>();
            var registration = container.GetRegistration(typeof(T)).Registration;
            registration.SuppressDiagnosticWarning(SimpleInjector.Diagnostics.DiagnosticType.DisposableTransientComponent, "Transient disposable is okay at times.");
        }

        #endregion Methods
    }
}
