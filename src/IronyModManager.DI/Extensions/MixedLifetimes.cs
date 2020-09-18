// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 09-18-2020
//
// Last Modified By : Mario
// Last Modified On : 09-18-2020
// ***********************************************************************
// <copyright file="MixedLifetimes.cs" company="Mario">
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
    /// Class MixedLifetimes.
    /// </summary>
    public static class MixedLifetimes
    {
        #region Methods

        /// <summary>
        /// Registers the without transient warning.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container">The container.</param>
        public static void RegisterWithoutMixedLifetimeWarning<T>(this Container container) where T : class
        {
            container.Register<T>();
            var registration = container.GetRegistration(typeof(T)).Registration;
            registration.SuppressDiagnosticWarning(SimpleInjector.Diagnostics.DiagnosticType.LifestyleMismatch, "Sometimes lifetimes can be mixed.");
        }

        /// <summary>
        /// Registers the without transient warning.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the t2.</typeparam>
        /// <param name="container">The container.</param>
        public static void RegisterWithoutMixedLifetimeWarning<T, T2>(this Container container) where T : class where T2 : class, T
        {
            container.Register<T, T2>();
            var registration = container.GetRegistration(typeof(T)).Registration;
            registration.SuppressDiagnosticWarning(SimpleInjector.Diagnostics.DiagnosticType.LifestyleMismatch, "Sometimes lifetimes can be mixed.");
        }

        #endregion Methods
    }
}
