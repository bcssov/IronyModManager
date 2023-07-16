
// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 06-26-2023
//
// Last Modified By : Mario
// Last Modified On : 06-26-2023
// ***********************************************************************
// <copyright file="ProcessedArgs.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Shared.Models
{

    /// <summary>
    /// Class ProcessedArgs.
    /// </summary>
    public class ProcessedArgs
    {
        #region Properties

        /// <summary>
        /// Gets or sets the current.
        /// </summary>
        /// <value>The current.</value>
        public int Current { get; set; }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        /// <value>The total.</value>
        public int Total { get; set; }

        #endregion Properties
    }
}
