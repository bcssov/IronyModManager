// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 03-28-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2022
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
        /// Determines whether [is ho i4] [the specified arguments].
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if [is ho i4] [the specified arguments]; otherwise, <c>false</c>.</returns>
        public static bool IsHOI4(this CanParseArgs args)
        {
            return args.GameType.Equals(Shared.Constants.GamesTypes.HeartsOfIron4.Id, StringComparison.OrdinalIgnoreCase);
        }

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
