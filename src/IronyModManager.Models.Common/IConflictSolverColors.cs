// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 03-18-2024
//
// Last Modified By : Mario
// Last Modified On : 03-18-2024
// ***********************************************************************
// <copyright file="IConflictSolverColors.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// An conflict solver colors interface.
    /// </summary>
    public interface IConflictSolverColors : IModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the color of the conflict solver deleted line.
        /// </summary>
        /// <value>The color of the conflict solver deleted line.</value>
        string ConflictSolverDeletedLineColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the conflict solver imaginary line.
        /// </summary>
        /// <value>The color of the conflict solver imaginary line.</value>
        string ConflictSolverImaginaryLineColor { get; set; }

        /// <summary>
        /// Gets or sets a value representing the conflict solver inserted line color.
        /// </summary>
        /// <value>The conflict solver inserted line color.</value>
        string ConflictSolverInsertedLineColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the conflict solver modified line.
        /// </summary>
        /// <value>The color of the conflict solver modified line.</value>
        string ConflictSolverModifiedLineColor { get; set; }

        #endregion Properties
    }
}
