// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 10-24-2021
//
// Last Modified By : Mario
// Last Modified On : 07-13-2022
// ***********************************************************************
// <copyright file="SearchParserResult.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common.Mod.Search;

namespace IronyModManager.Parser.Mod.Search
{
    /// <summary>
    /// Class SearchParserResult.
    /// Implements the <see cref="IronyModManager.Parser.Common.Mod.Search.ISearchParserResult" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Mod.Search.ISearchParserResult" />
    public class SearchParserResult : ISearchParserResult
    {
#nullable enable

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchParserResult" /> class.
        /// </summary>
        public SearchParserResult()
        {
            AchievementCompatible = new BoolFilterResult(null);
            IsSelected = new BoolFilterResult(null);
            Source = new List<SourceTypeResult>();
            Version = new List<Version?>();
            Name = new List<string>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [achievement compatible].
        /// </summary>
        /// <value><c>null</c> if [achievement compatible] contains no value, <c>true</c> if [achievement compatible]; otherwise, <c>false</c>.</value>
        [DescriptorProperty(Fields.Achievements)]
        public BoolFilterResult AchievementCompatible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value><c>null</c> if [is selected] contains no value, <c>true</c> if [is selected]; otherwise, <c>false</c>.</value>
        [DescriptorProperty(Fields.Selected)]
        public BoolFilterResult IsSelected { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public IList<string> Name { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        [DescriptorProperty(Fields.Source, true)]
        public IList<SourceTypeResult> Source { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [DescriptorProperty(Fields.Version, true)]
        public IList<Version?> Version { get; set; }

        #endregion Properties

#nullable disable
    }
}
