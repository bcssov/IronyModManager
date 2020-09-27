// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 09-17-2020
//
// Last Modified By : Mario
// Last Modified On : 09-17-2020
// ***********************************************************************
// <copyright file="IUnpacker.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace IronyModManager.IO.Common.Updater
{
    /// <summary>
    /// Interface IUnpacker
    /// </summary>
    public interface IUnpacker
    {
        #region Methods

        /// <summary>
        /// Unpacks the update asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> UnpackUpdateAsync(string path);

        #endregion Methods
    }
}
