// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 02-25-2020
// ***********************************************************************
// <copyright file="FileInfo.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Shared;

namespace IronyModManager.IO
{
    /// <summary>
    /// Class FileInfo.
    /// Implements the <see cref="IronyModManager.IO.Common.Readers.IFileInfo" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Readers.IFileInfo" />
    [ExcludeFromCoverage("Simple model.")]
    public class FileInfo : IFileInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public IEnumerable<string> Content { get; set; }

        /// <summary>
        /// Gets or sets the content sha.
        /// </summary>
        /// <value>The content sha.</value>
        public string ContentSHA { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is binary.
        /// </summary>
        /// <value><c>true</c> if this instance is binary; otherwise, <c>false</c>.</value>
        public bool IsBinary { get; set; }

        #endregion Properties
    }
}
