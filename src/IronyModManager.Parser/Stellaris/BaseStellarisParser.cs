// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-18-2020
// ***********************************************************************
// <copyright file="BaseStellarisParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Parser.Stellaris
{
    /// <summary>
    /// Class BaseStellarisParser.
    /// Implements the <see cref="IronyModManager.Parser.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.IGameParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.IGameParser" />
    public abstract class BaseStellarisParser : BaseParser, IGameParser
    {
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
