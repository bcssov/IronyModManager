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
        /// <param name="result">if set to <c>true</c> [result].</param>
        public BoolFilterResult(bool? result)
        {
            Result = result;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this <see cref="BoolFilterResult" /> is result.
        /// </summary>
        /// <value><c>null</c> if [result] contains no value, <c>true</c> if [result]; otherwise, <c>false</c>.</value>
        public bool? Result { get; }

        #endregion Properties
    }

    /// <summary>
    /// Class CanParseResult.
    /// </summary>
    public class CanParseResult
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CanParseResult" /> class.
        /// </summary>
        /// <param name="result">if set to <c>true</c> [result].</param>
        /// <param name="mappedStaticField">The mapped static field.</param>
        public CanParseResult(bool result, string mappedStaticField)
        {
            MappedStaticField = mappedStaticField;
            Result = result;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the mapped static field.
        /// </summary>
        /// <value>The mapped static field.</value>
        public string MappedStaticField { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="CanParseResult" /> is result.
        /// </summary>
        /// <value><c>true</c> if result; otherwise, <c>false</c>.</value>
        public bool Result { get; }

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
