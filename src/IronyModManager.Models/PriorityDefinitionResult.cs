// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 04-27-2020
//
// Last Modified By : Mario
// Last Modified On : 10-17-2024
// ***********************************************************************
// <copyright file="PriorityDefinitionResult.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Models.Common;
using IronyModManager.Shared.Models;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class PriorityDefinitionResult.
    /// Implements the <see cref="IronyModManager.Models.Common.IPriorityDefinitionResult" />
    /// Implements the <see cref="IronyModManager.Models.Common.BaseModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.BaseModel" />
    /// <seealso cref="IronyModManager.Models.Common.IPriorityDefinitionResult" />
    public class PriorityDefinitionResult : BaseModel, IPriorityDefinitionResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>The definition.</value>
        public virtual IDefinition Definition { get; set; }

        /// <summary>
        /// Gets or sets the definition order.
        /// </summary>
        /// <value>The definition order.</value>
        public virtual IEnumerable<IDefinition> DefinitionOrder { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public virtual string FileName { get; set; }

        /// <summary>
        /// Gets or sets the type of the priority.
        /// </summary>
        /// <value>The type of the priority.</value>
        public virtual DefinitionPriorityType PriorityType { get; set; }

        #endregion Properties
    }
}
