// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 07-11-2022
//
// Last Modified By : Mario
// Last Modified On : 07-11-2022
// ***********************************************************************
// <copyright file="IParadoxLauncher.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IronyModManager.IO.Common.Platforms
{
    /// <summary>
    /// Interface IParadoxLauncher
    /// </summary>
    public interface IParadoxLauncher
    {
        #region Methods

        /// <summary>
        /// Determines whether [is running asynchronous].
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> IsRunningAsync();

        #endregion Methods
    }
}
