// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 01-17-2020
//
// Last Modified By : Mario
// Last Modified On : 01-17-2020
// ***********************************************************************
// <copyright file="IAddin.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Interface IAddin
    /// </summary>
    public interface IAddin
    {
        #region Properties

        /// <summary>
        /// Gets the dependencies.
        /// </summary>
        /// <value>The dependencies.</value>
        IEnumerable<string> Dependencies { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        #endregion Properties
    }
}
