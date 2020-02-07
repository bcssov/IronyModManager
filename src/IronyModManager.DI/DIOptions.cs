// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 01-15-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2020
// ***********************************************************************
// <copyright file="DIOptions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using SimpleInjector;

namespace IronyModManager.DI
{
    /// <summary>
    /// Class DIOptions.
    /// </summary>
    public class DIOptions
    {
        #region Properties

        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        /// <value>The container.</value>
        public Container Container { get; set; }

        /// <summary>
        /// Gets or sets the module types.
        /// </summary>
        /// <value>The module types.</value>
        public IEnumerable<Type> ModuleTypes { get; set; }

        /// <summary>
        /// Gets or sets the name of the plugin path and.
        /// </summary>
        /// <value>The name of the plugin path and.</value>
        public string PluginPathAndName { get; set; }

        /// <summary>
        /// Gets or sets the plugin types.
        /// </summary>
        /// <value>The plugin types.</value>
        public IEnumerable<Type> PluginTypes { get; set; }

        #endregion Properties
    }
}
