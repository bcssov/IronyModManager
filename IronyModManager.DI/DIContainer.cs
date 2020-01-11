// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 01-10-2020
// ***********************************************************************
// <copyright file="DIContainer.cs" company="IronyModManager.DI">
//     Copyright (c) Mario. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using SimpleInjector;
using System;
using System.Collections.Generic;

namespace IronyModManager.DI
{
    /// <summary>
    /// Class DIContainer.
    /// </summary>
    public static class DIContainer
    {
        #region Properties

        // TODO: Container is leaking, see in the future if I can fix the design requirements. In my defense I've never used this configuration before so it's a learning experience...
        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public static Container Container { get; private set; }

        /// <summary>
        /// Gets the plugins path.
        /// </summary>
        /// <value>The plugins path.</value>
        public static string PluginsPath { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initializes the specified container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="pluginsPath">The plugins path.</param>
        internal static void Init(Container container, string pluginsPath)
        {
            Container = container;
            PluginsPath = pluginsPath;
        }

        #endregion Methods
    }
}