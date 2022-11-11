// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 11-16-2021
//
// Last Modified By : Mario
// Last Modified On : 11-04-2022
// ***********************************************************************
// <copyright file="LongToDateTimeHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Interfaces;
using RepoDb.Options;

namespace IronyModManager.IO.Mods.Models.Paradox.PropertyHandlers
{
    /// <summary>
    /// Class ObjectToDateTimeHandler.
    /// Implements the <see cref="RepoDb.Interfaces.IPropertyHandler{System.Object, System.DateTime}" />
    /// </summary>
    /// <seealso cref="RepoDb.Interfaces.IPropertyHandler{System.Object, System.DateTime}" />
    internal class LongToDateTimeHandler : IPropertyHandler<long, DateTime>
    {
        #region Methods

        /// <summary>
        /// Gets the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="options">The options.</param>
        /// <returns>DateTime.</returns>
        public DateTime Get(long input, PropertyHandlerGetOptions options)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(input).DateTime;
        }

        /// <summary>
        /// Sets the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.Int64.</returns>
        public long Set(DateTime input, PropertyHandlerSetOptions options)
        {
            return Convert.ToInt64((input - DateTime.UnixEpoch).TotalMilliseconds);
        }

        #endregion Methods
    }
}
