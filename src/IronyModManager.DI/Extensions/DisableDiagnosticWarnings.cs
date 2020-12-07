// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 09-18-2020
//
// Last Modified By : Mario
// Last Modified On : 12-07-2020
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
        /// <summary>
        /// Removes the mixed lifetime warning.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container">The container.</param>
#pragma warning disable IDE0060 // Remove unused parameter

        #region Methods

        public static void RemoveMixedLifetimeWarning<T>(this Container container) where T : class
#pragma warning restore IDE0060 // Remove unused parameter
        {
            DIContainer.QueueSuppressionType(typeof(T), SimpleInjector.Diagnostics.DiagnosticType.LifestyleMismatch, "Sometimes lifetimes can be mixed.");
        }

        /// <summary>
        /// Removes the transient warning.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container">The container.</param>
#pragma warning disable IDE0060 // Remove unused parameter

        public static void RemoveTransientWarning<T>(this Container container) where T : class
#pragma warning restore IDE0060 // Remove unused parameter
        {
            DIContainer.QueueSuppressionType(typeof(T), SimpleInjector.Diagnostics.DiagnosticType.DisposableTransientComponent, "Transient disposable is okay at times.");
        }

        #endregion Methods
    }
}
