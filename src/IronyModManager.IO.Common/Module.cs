﻿// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 02-25-2020
//
// Last Modified By : Mario
// Last Modified On : 08-29-2021
// ***********************************************************************
// <copyright file="Module.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using IronyModManager.Shared;

namespace IronyModManager.IO.Common
{
    /// <summary>
    /// Class Module.
    /// Implements the <see cref="IronyModManager.Shared.IModule" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.IModule" />
    [ExcludeFromCoverage("Module info should not be tested.")]
    public class Module : IModule
    {
        #region Properties

        /// <summary>
        /// Gets the dependencies.
        /// </summary>
        /// <value>The dependencies.</value>
        public IEnumerable<string> Dependencies => new List<string> { typeof(Shared.Module).Name, typeof(IronyModManager.Models.Common.Module).Name };

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name => typeof(Module).Namespace;

        #endregion Properties
    }
}
