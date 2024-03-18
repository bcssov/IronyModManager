// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 03-18-2024
//
// Last Modified By : Mario
// Last Modified On : 03-18-2024
// ***********************************************************************
// <copyright file="ConflictSolverColors.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Models.Common;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class ConflictSolverColors.
    /// Implements the <see cref="BaseModel" />
    /// Implements the <see cref="IConflictSolverColors" />
    /// </summary>
    /// <seealso cref="BaseModel" />
    /// <seealso cref="IConflictSolverColors" />
    public class ConflictSolverColors : BaseModel, IConflictSolverColors
    {
        #region Properties

        /// <summary>
        /// Gets or sets the color of the conflict solver deleted line.
        /// </summary>
        /// <value>The color of the conflict solver deleted line.</value>
        public virtual string ConflictSolverDeletedLineColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the conflict solver imaginary line.
        /// </summary>
        /// <value>The color of the conflict solver imaginary line.</value>
        public virtual string ConflictSolverImaginaryLineColor { get; set; }

        /// <summary>
        /// Gets or sets a value representing the conflict solver inserted line color.
        /// </summary>
        /// <value>The conflict solver inserted line color.</value>
        public virtual string ConflictSolverInsertedLineColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the conflict solver modified line.
        /// </summary>
        /// <value>The color of the conflict solver modified line.</value>
        public virtual string ConflictSolverModifiedLineColor { get; set; }

        #endregion Properties
    }
}
