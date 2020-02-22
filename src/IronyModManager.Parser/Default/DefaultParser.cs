// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 02-22-2020
// ***********************************************************************
// <copyright file="DefaultParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Linq;
using System.Collections.Generic;

namespace IronyModManager.Parser.Default
{
    /// <summary>
    /// Class DefaultParser.
    /// Implements the <see cref="IronyModManager.Parser.Default.BaseParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Default.BaseParser" />
    public class DefaultParser : BaseParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultParser"/> class.
        /// </summary>
        /// <param name="textParser">The text parser.</param>
        public DefaultParser(ITextParser textParser) : base(textParser)
        {
        }

        #endregion Constructors
    }
}
