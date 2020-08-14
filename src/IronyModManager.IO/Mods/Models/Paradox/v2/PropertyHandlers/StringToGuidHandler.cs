// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 08-13-2020
// ***********************************************************************
// <copyright file="StringToGuidHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using RepoDb;
using RepoDb.Interfaces;

namespace IronyModManager.IO.Mods.Models.Paradox.v2.PropertyHandlers
{
    /// <summary>
    /// Class StringToGuidHandler.
    /// Implements the <see cref="RepoDb.Interfaces.IPropertyHandler{System.String, System.Guid}" />
    /// </summary>
    /// <seealso cref="RepoDb.Interfaces.IPropertyHandler{System.String, System.Guid}" />
    internal class StringToGuidHandler : IPropertyHandler<string, Guid>
    {
        #region Methods

        /// <summary>
        /// Gets the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="property">The property.</param>
        /// <returns>Guid.</returns>
        public Guid Get(string input, ClassProperty property)
        {
            Guid.TryParse(input, out var output);
            return output;
        }

        /// <summary>
        /// Sets the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="property">The property.</param>
        /// <returns>System.String.</returns>
        public string Set(Guid input, ClassProperty property)
        {
            return input.ToString();
        }

        #endregion Methods
    }
}
