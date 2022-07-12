// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 07-12-2022
// ***********************************************************************
// <copyright file="IFileInfo.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.IO.Common.Readers
{
    /// <summary>
    /// Interface IFileInfo
    /// </summary>
    public interface IFileInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        IEnumerable<string> Content { get; set; }

        /// <summary>
        /// Gets or sets the content sha.
        /// </summary>
        /// <value>The content sha.</value>
        string ContentSHA { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        string FileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is binary.
        /// </summary>
        /// <value><c>true</c> if this instance is binary; otherwise, <c>false</c>.</value>
        bool IsBinary { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the last modified.
        /// </summary>
        /// <value>The last modified.</value>
        public DateTime? LastModified { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public long Size { get; set; }

        #endregion Properties
    }
}
