// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-14-2021
//
// Last Modified By : Mario
// Last Modified On : 10-29-2022
// ***********************************************************************
// <copyright file="DLCExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IronyModManager.IO.Common.DLC;
using IronyModManager.IO.Mods.Exporter;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;

namespace IronyModManager.IO.DLC
{
    /// <summary>
    /// Class DLCExporter.
    /// Implements the <see cref="IronyModManager.IO.Common.DLC.IDLCExporter" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.DLC.IDLCExporter" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class DLCExporter : IDLCExporter
    {
        #region Fields

        /// <summary>
        /// The json exporter
        /// </summary>
        private readonly JsonExporter jsonExporter;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DLCExporter" /> class.
        /// </summary>
        public DLCExporter()
        {
            jsonExporter = new JsonExporter();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Exports the DLC asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="System.ArgumentException">Invalid descriptor type.</exception>
        public Task<bool> ExportDLCAsync(DLCParameters parameters)
        {
            if (parameters.DescriptorType == Common.DescriptorType.None)
            {
                throw new ArgumentException("Invalid descriptor type.");
            }
            return jsonExporter.ExportDLCAsync(parameters);
        }

        /// <summary>
        /// Gets the disabled DLC asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;IReadOnlyCollection&lt;IDLCObject&gt;&gt;.</returns>
        /// <exception cref="System.ArgumentException">Invalid descriptor type.</exception>
        public Task<IReadOnlyCollection<IDLCObject>> GetDisabledDLCAsync(DLCParameters parameters)
        {
            if (parameters.DescriptorType == Common.DescriptorType.None)
            {
                throw new ArgumentException("Invalid descriptor type.");
            }
            return jsonExporter.GetDisabledDLCAsync(parameters);
        }

        #endregion Methods
    }
}
