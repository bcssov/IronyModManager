// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 04-18-2020
//
// Last Modified By : Mario
// Last Modified On : 04-18-2020
// ***********************************************************************
// <copyright file="IParserMap.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Parser.Common
{
    /// <summary>
    /// Interface IParserMap
    /// </summary>
    public interface IParserMap
    {
        #region Properties

        /// <summary>
        /// Gets or sets the directory path.
        /// </summary>
        /// <value>The directory path.</value>
        string DirectoryPath { get; set; }

        /// <summary>
        /// Gets or sets the preferred parser.
        /// </summary>
        /// <value>The preferred parser.</value>
        string PreferredParser { get; set; }

        #endregion Properties
    }
}
