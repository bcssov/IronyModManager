// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 08-23-2021
// ***********************************************************************
// <copyright file="Language.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Models.Common;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class Language.
    /// Implements the <see cref="IronyModManager.Models.Common.BaseModel" />
    /// Implements the <see cref="IronyModManager.Models.Common.ILanguage" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.BaseModel" />
    /// <seealso cref="IronyModManager.Models.Common.ILanguage" />
    public class Language : BaseModel, ILanguage
    {
        #region Properties

        /// <summary>
        /// Gets or sets the abrv.
        /// </summary>
        /// <value>The abrv.</value>
        public virtual string Abrv { get; set; }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        public virtual string Font { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
        public virtual bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the supported name block.
        /// </summary>
        /// <value>The supported name block.</value>
        public virtual string SupportedNameBlock { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether the specified term is match.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns><c>true</c> if the specified term is match; otherwise, <c>false</c>.</returns>
        public bool IsMatch(string term)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return false;
            }
            term ??= string.Empty;
            return Name.StartsWith(term, StringComparison.OrdinalIgnoreCase);
        }

        #endregion Methods
    }
}
