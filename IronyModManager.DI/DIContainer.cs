// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 01-11-2020
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
    internal static class DIContainer
    {
        #region Properties

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        internal static Container Container { get; private set; }

        /// <summary>
        /// Gets the plugins path.
        /// </summary>
        /// <value>The plugins path.</value>
        internal static string PluginsPath { get; private set; }

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
