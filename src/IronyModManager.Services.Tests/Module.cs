// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 01-31-2020
//
// Last Modified By : Mario
// Last Modified On : 01-31-2020
// ***********************************************************************
// <copyright file="Module.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************


using System;
using System.Collections.Generic;
using IronyModManager.Shared;

namespace IronyModManager.Service.Tests
{
    /// <summary>
    /// Class Module.
    /// Implements the <see cref="IronyModManager.Shared.IModule" />
    /// Implements the <see cref="IModule" />
    /// </summary>
    /// <seealso cref="IModule" />
    /// <seealso cref="IronyModManager.Shared.IModule" />
    public class Module : IModule
    {
        #region Properties

        /// <summary>
        /// Gets the dependencies.
        /// </summary>
        /// <value>The dependencies.</value>
        public IEnumerable<string> Dependencies => new List<string>() { };

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        /// s
        public string Name => typeof(Module).Namespace;

        #endregion Properties
    }
}
