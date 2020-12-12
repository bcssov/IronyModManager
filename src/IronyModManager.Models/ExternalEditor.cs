// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 12-07-2020
//
// Last Modified By : Mario
// Last Modified On : 12-07-2020
// ***********************************************************************
// <copyright file="ExternalEditor.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using System;
using IronyModManager.Models.Common;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class ExternalEditor.
    /// Implements the <see cref="IronyModManager.Models.Common.IExternalEditor" />
    /// Implements the <see cref="IronyModManager.Models.Common.BaseModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.BaseModel" />
    /// <seealso cref="IronyModManager.Models.Common.IExternalEditor" />
    public class ExternalEditor : BaseModel, IExternalEditor
    {
        #region Properties

        /// <summary>
        /// Gets or sets the external editor location.
        /// </summary>
        /// <value>The external editor location.</value>
        public virtual string ExternalEditorLocation { get; set; }

        /// <summary>
        /// Gets or sets the external editor parameters.
        /// </summary>
        /// <value>The external editor parameters.</value>
        public virtual string ExternalEditorParameters { get; set; }

        #endregion Properties
    }
}
