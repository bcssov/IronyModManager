// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 09-18-2020
//
// Last Modified By : Mario
// Last Modified On : 09-18-2020
// ***********************************************************************
// <copyright file="DisableDiagnosticWarnings.cs" company="Mario">
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
    /// Class DisableDiagnosticWarnings.
    /// </summary>
    public static class DisableDiagnosticWarnings
    {
        #region Methods

        /// <summary>
        /// Removes the mixed lifetime warning.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container">The container.</param>
        public static void RemoveMixedLifetimeWarning<T>(this Container container) where T : class
        {
            var registration = container.GetRegistration(typeof(T)).Registration;
            registration.SuppressDiagnosticWarning(SimpleInjector.Diagnostics.DiagnosticType.LifestyleMismatch, "Sometimes lifetimes can be mixed.");
        }

        /// <summary>
        /// Removes the transient warning.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container">The container.</param>
        public static void RemoveTransientWarning<T>(this Container container) where T : class
        {
            var registration = container.GetRegistration(typeof(T)).Registration;
            registration.SuppressDiagnosticWarning(SimpleInjector.Diagnostics.DiagnosticType.DisposableTransientComponent, "Transient disposable is okay at times.");
        }

        #endregion Methods
    }
}
