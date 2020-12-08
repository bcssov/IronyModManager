// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 12-07-2020
//
// Last Modified By : Mario
// Last Modified On : 12-08-2020
// ***********************************************************************
// <copyright file="IExternalEditorFiles.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using System;
using IronyModManager.Shared;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Interface IExternalEditorFiles
    /// Implements the <see cref="IronyModManager.Models.Common.IModel" />
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.IModel" />
    /// <seealso cref="System.IDisposable" />
    public interface IExternalEditorFiles : IModel, IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets or sets the left difference.
        /// </summary>
        /// <value>The left difference.</value>
        ITempFile LeftDiff { get; set; }

        /// <summary>
        /// Gets or sets the right difference.
        /// </summary>
        /// <value>The right difference.</value>
        ITempFile RightDiff { get; set; }

        #endregion Properties
    }
}
