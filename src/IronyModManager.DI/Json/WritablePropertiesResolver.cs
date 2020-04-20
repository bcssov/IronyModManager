// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 04-19-2020
//
// Last Modified By : Mario
// Last Modified On : 04-19-2020
// ***********************************************************************
// <copyright file="WritablePropertiesResolver.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IronyModManager.DI.Json
{
    /// <summary>
    /// Class WritablePropertiesResolver.
    /// Implements the <see cref="Newtonsoft.Json.Serialization.DefaultContractResolver" />
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.Serialization.DefaultContractResolver" />
    public class WritablePropertiesResolver : DefaultContractResolver
    {
        #region Methods

        /// <summary>
        /// Creates properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.
        /// </summary>
        /// <param name="type">The type to create properties for.</param>
        /// <param name="memberSerialization">The member serialization mode for the type.</param>
        /// <returns>Properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.</returns>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            return props.Where(p => p.Writable).ToList();
        }

        #endregion Methods
    }
}
