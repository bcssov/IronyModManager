// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 12-07-2020
// ***********************************************************************
// <copyright file="DIContainer.cs" company="IronyModManager.DI">
//     Copyright (c) Mario. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using SimpleInjector;

namespace IronyModManager.DI
{
    /// <summary>
    /// Class DIContainer.
    /// </summary>
    public static class DIContainer
    {
        #region Properties

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        internal static Container Container { get; private set; }

        /// <summary>
        /// Gets the name of the plugin path and.
        /// </summary>
        /// <value>The name of the plugin path and.</value>
        internal static string PluginPathAndName { get; private set; }

        /// <summary>
        /// Gets the suppression type queues.
        /// </summary>
        /// <value>The suppression type queues.</value>
        private static List<WarningSuppressionTypeQueue> SuppressionTypeQueues { get; } = new List<WarningSuppressionTypeQueue>();

        #endregion Properties

        #region Methods

        /// <summary>
        /// Finishes this instance.
        /// </summary>
        /// <param name="skipVerify">if set to <c>true</c> [skip verify].</param>
        public static void Finish(bool skipVerify = false)
        {
            if (SuppressionTypeQueues.Any())
            {
                foreach (var item in SuppressionTypeQueues)
                {
                    var registration = Container.GetRegistration(item.Type, false).Registration;
                    if (registration != null)
                    {
                        registration.SuppressDiagnosticWarning(item.DiagnosticType, item.Reason);
                    }
                }
            }
#if DEBUG
            if (!skipVerify)
            {
                Container.Verify();
            }
#endif
        }

        /// <summary>
        /// Initializes the specified container.
        /// </summary>
        /// <param name="opts">The opts.</param>
        internal static void Init(DIOptions opts)
        {
            Container = opts.Container;
            PluginPathAndName = opts.PluginPathAndName;
        }

        /// <summary>
        /// Queues the type of the suppression.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="diagnosticType">Type of the diagnostic.</param>
        /// <param name="reason">The reason.</param>
        internal static void QueueSuppressionType(Type type, SimpleInjector.Diagnostics.DiagnosticType diagnosticType, string reason)
        {
            if (!SuppressionTypeQueues.Any(t => t.Type.Equals(type) && !(t.DiagnosticType == diagnosticType)))
            {
                SuppressionTypeQueues.Add(new WarningSuppressionTypeQueue()
                {
                    DiagnosticType = diagnosticType,
                    Reason = reason,
                    Type = type
                });
            }
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class WarningSuppressionTypeQueue.
        /// </summary>
        private class WarningSuppressionTypeQueue
        {
            #region Properties

            /// <summary>
            /// Gets or sets the type of the diagnostic.
            /// </summary>
            /// <value>The type of the diagnostic.</value>
            public SimpleInjector.Diagnostics.DiagnosticType DiagnosticType { get; set; }

            /// <summary>
            /// Gets or sets the reason.
            /// </summary>
            /// <value>The reason.</value>
            public string Reason { get; set; }

            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            /// <value>The type.</value>
            public Type Type { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
