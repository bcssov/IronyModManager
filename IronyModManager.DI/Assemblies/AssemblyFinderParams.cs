// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 01-12-2020
//
// Last Modified By : Mario
// Last Modified On : 01-17-2020
// ***********************************************************************
// <copyright file="AssemblyFinderParams.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;

namespace IronyModManager.DI.Assemblies
{
    /// <summary>
    /// Class AssemblyFinderParams.
    /// </summary>
    internal class AssemblyFinderParams
    {
        #region Properties

        /// <summary>
        /// Gets or sets the assembly pattern match.
        /// </summary>
        /// <value>The assembly pattern match.</value>
        public string AssemblyPatternMatch { get; set; }

        /// <summary>
        /// Gets or sets the embeded resource key.
        /// </summary>
        /// <value>The embeded resource key.</value>
        public string EmbededResourceKey { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the search option.
        /// </summary>
        /// <value>The search option.</value>
        public SearchOption SearchOption { get; set; }

        /// <summary>
        /// Gets or sets the shared types.
        /// </summary>
        /// <value>The shared types.</value>
        public IEnumerable<Type> SharedTypes { get; set; }

        #endregion Properties
    }
}
