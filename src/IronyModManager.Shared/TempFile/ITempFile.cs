// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 12-07-2020
//
// Last Modified By : Mario
// Last Modified On : 02-13-2021
// ***********************************************************************
// <copyright file="ITempFile.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Linq;
using System.Collections.Generic;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Interface ITempFile
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface ITempFile : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <value>The file.</value>
        string File { get; }

        /// <summary>
        /// Gets or sets the temporary directory.
        /// </summary>
        /// <value>The temporary directory.</value>
        string TempDirectory { get; set; }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        string Text { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Creates the specified path.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>System.String.</returns>
        string Create(string fileName = Constants.EmptyParam);

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Delete();

        #endregion Methods
    }
}
