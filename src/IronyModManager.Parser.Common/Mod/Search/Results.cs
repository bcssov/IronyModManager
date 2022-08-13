// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 10-26-2021
//
// Last Modified By : Mario
// Last Modified On : 08-12-2022
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
    /// Class BaseFilterResult.
    /// </summary>
    public abstract class BaseFilterResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BaseFilterResult" /> is negate.
        /// </summary>
        /// <value><c>true</c> if negate; otherwise, <c>false</c>.</value>
        public bool Negate { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class BoolFilterResult.
    /// </summary>
    public class BoolFilterResult : BaseFilterResult
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
    public class CanParseResult : BaseFilterResult
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
    /// Class NameFilterResult.
    /// Implements the <see cref="IronyModManager.Parser.Common.Mod.Search.BaseFilterResult" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Mod.Search.BaseFilterResult" />
    public class NameFilterResult : BaseFilterResult
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NameFilterResult"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public NameFilterResult(string text)
        {
            Text = text;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class SourceTypeResult.
    /// </summary>
    public class SourceTypeResult : BaseFilterResult
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

#nullable enable

    /// <summary>
    /// Class VersionTypeResult.
    /// Implements the <see cref="IronyModManager.Parser.Common.Mod.Search.BaseFilterResult" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Mod.Search.BaseFilterResult" />
    public class VersionTypeResult : BaseFilterResult
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionTypeResult" /> class.
        /// </summary>
        /// <param name="version">The version.</param>
        public VersionTypeResult(Shared.Version? version)
        {
            Version = version;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        public Shared.Version? Version { get; set; }

        #endregion Properties
    }

#nullable disable
}
