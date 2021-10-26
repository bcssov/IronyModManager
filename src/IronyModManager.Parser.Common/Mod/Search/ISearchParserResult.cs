// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 10-24-2021
//
// Last Modified By : Mario
// Last Modified On : 10-26-2021
// ***********************************************************************
// <copyright file="ISearchParserResult.cs" company="Mario">
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
    /// Interface SearchParserResult
    /// </summary>
    public interface ISearchParserResult
    {
#nullable enable

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [achievement compatible].
        /// </summary>
        /// <value><c>null</c> if [achievement compatible] contains no value, <c>true</c> if [achievement compatible]; otherwise, <c>false</c>.</value>
        bool? AchievementCompatible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value><c>null</c> if [is selected] contains no value, <c>true</c> if [is selected]; otherwise, <c>false</c>.</value>
        bool? IsSelected { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        SourceType Source { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        Version? Version { get; set; }

        #endregion Properties

#nullable disable
    }
}
