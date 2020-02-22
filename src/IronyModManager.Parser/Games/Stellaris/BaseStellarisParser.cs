// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-22-2020
// ***********************************************************************
// <copyright file="BaseStellarisParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Parser.Default;

namespace IronyModManager.Parser.Games.Stellaris
{
    /// <summary>
    /// Class BaseStellarisParser.
    /// Implements the <see cref="IronyModManager.Parser.Default.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.Games.IGameParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Default.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.Games.IGameParser" />
    public abstract class BaseStellarisParser : BaseParser, IGameParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseStellarisParser"/> class.
        /// </summary>
        /// <param name="textParser">The text parser.</param>
        public BaseStellarisParser(ITextParser textParser) : base(textParser)
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public abstract bool CanParse(CanParseArgs args);

        /// <summary>
        /// Determines whether the specified arguments is stellaris.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if the specified arguments is stellaris; otherwise, <c>false</c>.</returns>
        protected virtual bool IsStellaris(CanParseArgs args)
        {
            return args.GameType.Equals(Shared.Constants.GamesTypes.Stellaris, StringComparison.OrdinalIgnoreCase);
        }

        #endregion Methods
    }
}
