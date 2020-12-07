// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 12-07-2020
//
// Last Modified By : Mario
// Last Modified On : 12-07-2020
// ***********************************************************************
// <copyright file="IExternalEditor.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Linq;
using System.Collections.Generic;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Interface IExternalEditor
    /// Implements the <see cref="IronyModManager.Models.Common.IModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.IModel" />
    public interface IExternalEditor : IModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the external editor location.
        /// </summary>
        /// <value>The external editor location.</value>
        string ExternalEditorLocation { get; set; }

        /// <summary>
        /// Gets or sets the external editor parameters.
        /// </summary>
        /// <value>The external editor parameters.</value>
        string ExternalEditorParameters { get; set; }

        #endregion Properties
    }
}
