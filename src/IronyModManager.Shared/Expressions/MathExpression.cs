// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-06-2025
//
// Last Modified By : Mario
// Last Modified On : 02-06-2025
// ***********************************************************************
// <copyright file="MathExpression.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using info.lundin.math;

namespace IronyModManager.Shared.Expressions
{
    /// <summary>
    /// Class MathExpression.
    /// </summary>
    public static class MathExpression
    {
        #region Properties

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        /// <value>The culture.</value>
        public static CultureInfo Culture { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the parser.
        /// </summary>
        /// <returns>ExpressionParser.</returns>
        public static ExpressionParser GetParser()
        {
            var parser = new ExpressionParser { Culture = Culture };
            return parser;
        }

        #endregion Methods
    }
}
