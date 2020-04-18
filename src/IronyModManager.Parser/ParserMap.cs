// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 04-18-2020
//
// Last Modified By : Mario
// Last Modified On : 04-18-2020
// ***********************************************************************
// <copyright file="ParserMap.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Parser.Common;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Class ParserMap.
    /// Implements the <see cref="IronyModManager.Parser.Common.IParserMap" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.IParserMap" />
    public class ParserMap : IParserMap
    {
        #region Properties

        /// <summary>
        /// Gets or sets the directory path.
        /// </summary>
        /// <value>The directory path.</value>
        public string DirectoryPath { get; set; }

        /// <summary>
        /// Gets or sets the preferred parser.
        /// </summary>
        /// <value>The preferred parser.</value>
        public string PreferredParser { get; set; }

        #endregion Properties
    }
}
