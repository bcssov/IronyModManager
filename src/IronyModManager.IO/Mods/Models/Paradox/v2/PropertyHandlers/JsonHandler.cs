// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 08-12-2020
// ***********************************************************************
// <copyright file="JsonHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RepoDb;
using RepoDb.Interfaces;

namespace IronyModManager.IO.Mods.Models.Paradox.v2.PropertyHandlers
{
    /// <summary>
    /// Class JsonHandler.
    /// Implements the <see cref="RepoDb.Interfaces.IPropertyHandler{System.String, System.Collections.Generic.List{System.String}}" />
    /// </summary>
    /// <seealso cref="RepoDb.Interfaces.IPropertyHandler{System.String, System.Collections.Generic.List{System.String}}" />
    internal class JsonHandler : IPropertyHandler<string, List<string>>
    {
        #region Methods

        /// <summary>
        /// Gets the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="property">The property.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public List<string> Get(string input, ClassProperty property)
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
        /// <param name="property">The property.</param>
        /// <returns>System.String.</returns>
        public string Set(List<string> input, ClassProperty property)
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
