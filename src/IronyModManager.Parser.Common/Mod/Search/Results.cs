// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 10-26-2021
//
// Last Modified By : Mario
// Last Modified On : 10-26-2021
// ***********************************************************************
// <copyright file="Results.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Parser.Common.Mod.Search
{
    /// <summary>
    /// Class BoolFilterResult.
    /// </summary>
    public class BoolFilterResult
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BoolFilterResult" /> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public BoolFilterResult(bool? result)
        {
            Result = result;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>The result.</value>
        public bool? Result { get; }

        #endregion Properties
    }

    /// <summary>
    /// Class SourceTypeResult.
    /// </summary>
    public class SourceTypeResult
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceTypeResult" /> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public SourceTypeResult(SourceType result)
        {
            Result = result;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>The result.</value>
        public SourceType Result { get; }

        #endregion Properties
    }
}
