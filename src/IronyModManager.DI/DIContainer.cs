// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2020
// ***********************************************************************
// <copyright file="DIContainer.cs" company="IronyModManager.DI">
//     Copyright (c) Mario. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
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
        /// Gets the module types.
        /// </summary>
        /// <value>The module types.</value>
        internal static IEnumerable<Type> ModuleTypes { get; private set; }

        /// <summary>
        /// Gets the name of the plugin path and.
        /// </summary>
        /// <value>The name of the plugin path and.</value>
        internal static string PluginPathAndName { get; private set; }

        /// <summary>
        /// Gets the plugin types.
        /// </summary>
        /// <value>The plugin types.</value>
        internal static IEnumerable<Type> PluginTypes { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Verifies this instance.
        /// </summary>
        public static void Verify()
        {
            Container.Verify();
        }

        /// <summary>
        /// Initializes the specified container.
        /// </summary>
        /// <param name="opts">The opts.</param>
        internal static void Init(DIOptions opts)
        {
            Container = opts.Container;
            PluginPathAndName = opts.PluginPathAndName;
            ModuleTypes = opts.ModuleTypes;
            PluginTypes = opts.PluginTypes;
        }

        #endregion Methods
    }
}
