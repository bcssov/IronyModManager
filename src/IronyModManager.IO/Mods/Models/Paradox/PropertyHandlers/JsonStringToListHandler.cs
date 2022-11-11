// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 11-04-2022
// ***********************************************************************
// <copyright file="JsonStringToListHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RepoDb.Interfaces;
using RepoDb.Options;

namespace IronyModManager.IO.Mods.Models.Paradox.PropertyHandlers
{
    /// <summary>
    /// Class StringToListHandler.
    /// Implements the <see cref="RepoDb.Interfaces.IPropertyHandler{System.String, System.Collections.Generic.List{System.String}}" />
    /// </summary>
    /// <seealso cref="RepoDb.Interfaces.IPropertyHandler{System.String, System.Collections.Generic.List{System.String}}" />
    internal class JsonStringToListHandler : IPropertyHandler<string, List<string>>
    {
        #region Methods

        /// <summary>
        /// Gets the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="options">The options.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public List<string> Get(string input, PropertyHandlerGetOptions options)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new List<string>();
            }
            var array = new JArray(input);
            return array.ToObject<List<string>>();
        }

        /// <summary>
        /// Sets the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="options">The options.</param>
        /// <returns>System.String.</returns>
        public string Set(List<string> input, PropertyHandlerSetOptions options)
        {
            if (input == null || input.Count == 0)
            {
                return new JArray(new List<string>()).ToString();
            }
            return new JArray(input).ToString();
        }

        #endregion Methods
    }
}
