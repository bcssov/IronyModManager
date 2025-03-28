// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 11-16-2021
//
// Last Modified By : Mario
// Last Modified On : 03-28-2025
// ***********************************************************************
// <copyright file="LongToDateTimeHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.Shared;
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
            try
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(input).DateTime;
            }
            catch (Exception e)
            {
                var logger = DIResolver.Get<ILogger>();
                logger.Error(new ArgumentOutOfRangeException($"Invalid timestamp {input} detected, logging for posterity. Defaulting to disco era value.", e));

                // If junk data, send it back to the disco era (1970)
                return new DateTime(1970, 1, 1); // Unix epoch date
            }
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
