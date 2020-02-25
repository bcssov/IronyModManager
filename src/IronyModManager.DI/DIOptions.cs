// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 01-15-2020
//
// Last Modified By : Mario
// Last Modified On : 02-25-2020
// ***********************************************************************
// <copyright file="DIOptions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
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
        /// Gets or sets the name of the plugin path and.
        /// </summary>
        /// <value>The name of the plugin path and.</value>
        public string PluginPathAndName { get; set; }

        #endregion Properties
    }
}
