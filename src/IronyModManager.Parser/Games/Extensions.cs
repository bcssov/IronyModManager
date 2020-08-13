// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 03-28-2020
//
// Last Modified By : Mario
// Last Modified On : 08-13-2020
// ***********************************************************************
// <copyright file="Extensions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Parser.Common.Args;

namespace IronyModManager.Parser.Games
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    public static class Extensions
    {
        #region Methods

        /// <summary>
        /// Determines whether the specified arguments is stellaris.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if the specified arguments is stellaris; otherwise, <c>false</c>.</returns>
        public static bool IsStellaris(this CanParseArgs args)
        {
            return args.GameType.Equals(Shared.Constants.GamesTypes.Stellaris.Id, StringComparison.OrdinalIgnoreCase);
        }

        #endregion Methods
    }
}
