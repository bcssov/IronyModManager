// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 11-26-2020
//
// Last Modified By : Mario
// Last Modified On : 11-26-2020
// ***********************************************************************
// <copyright file="ModMergeCompressExporterParameters.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace IronyModManager.IO.Common.Mods
{
    /// <summary>
    /// Class ModMergeCompressExporterParameters.
    /// </summary>
    public class ModMergeCompressExporterParameters
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the queue identifier.
        /// </summary>
        /// <value>The queue identifier.</value>
        public long QueueId { get; set; }

        /// <summary>
        /// Gets or sets the stream.
        /// </summary>
        /// <value>The stream.</value>
        public Stream Stream { get; set; }

        #endregion Properties
    }
}
