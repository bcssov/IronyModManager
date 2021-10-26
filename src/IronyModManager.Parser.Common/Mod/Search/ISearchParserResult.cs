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
        /// Gets or sets the achievement compatible.
        /// </summary>
        /// <value>The achievement compatible.</value>
        [DescriptorProperty(Fields.Achievements)]
        BoolFilterResult AchievementCompatible { get; set; }

        /// <summary>
        /// Gets or sets the is selected.
        /// </summary>
        /// <value>The is selected.</value>
        [DescriptorProperty(Fields.Selected)]
        BoolFilterResult IsSelected { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DescriptorProperty("name")]
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        [DescriptorProperty(Fields.Source)]
        SourceTypeResult Source { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [DescriptorProperty(Fields.Version)]
        Version? Version { get; set; }

        #endregion Properties

#nullable disable
    }
}
