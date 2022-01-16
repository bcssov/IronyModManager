// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 11-16-2021
//
// Last Modified By : Mario
// Last Modified On : 01-16-2022
// ***********************************************************************
// <copyright file="LongToDateTimeHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb;
using RepoDb.Interfaces;

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
        /// <param name="property">The property.</param>
        /// <returns>DateTime.</returns>
        public DateTime Get(long input, ClassProperty property)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(input).DateTime;
        }

        /// <summary>
        /// Sets the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="property">The property.</param>
        /// <returns>System.Int64.</returns>
        public long Set(DateTime input, ClassProperty property)
        {
            return Convert.ToInt64((input - DateTime.UnixEpoch).TotalMilliseconds);
        }

        #endregion Methods
    }
}
