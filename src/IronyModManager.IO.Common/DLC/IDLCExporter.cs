// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 02-14-2021
//
// Last Modified By : Mario
// Last Modified On : 02-14-2021
// ***********************************************************************
// <copyright file="IDLCExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.Shared.Models;

namespace IronyModManager.IO.Common.DLC
{
    /// <summary>
    /// Interface IDLCExporter
    /// </summary>
    public interface IDLCExporter
    {
        #region Methods

        /// <summary>
        /// Exports the DLC asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ExportDLCAsync(DLCParameters parameters);

        /// <summary>
        /// Gets the disabled DLC asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;IReadOnlyCollection&lt;IDLCObject&gt;&gt;.</returns>
        Task<IReadOnlyCollection<IDLCObject>> GetDisabledDLCAsync(DLCParameters parameters);

        #endregion Methods
    }
}
