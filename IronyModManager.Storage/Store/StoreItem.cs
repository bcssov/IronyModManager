// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2020
// ***********************************************************************
// <copyright file="StoreItem.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace IronyModManager.Storage.Store
{
    /// <summary>
    /// Class StoreItem.
    /// </summary>
    // Modified version of Jot item, no need for us to test it
    [ExcludeFromCodeCoverage]
    internal class StoreItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [JsonProperty(Order = 2)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the parent.
        /// </summary>
        /// <value>The type of the parent.</value>
        [JsonProperty(Order = 1)]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [JsonProperty(Order = 3)]
        public object Value { get; set; }

        #endregion Properties
    }
}
