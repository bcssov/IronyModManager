// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 07-14-2022
//
// Last Modified By : Mario
// Last Modified On : 07-14-2022
// ***********************************************************************
// <copyright file="DIConverter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.Shared;
using Newtonsoft.Json;

namespace IronyModManager.Storage.Store
{
    /// <summary>
    /// Class DIConverter.
    /// </summary>
    [ExcludeFromCoverage("Modified version of Jot item, no need for us to test it.")]
    internal class DIConverter : Newtonsoft.Json.JsonConverter
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
        public override bool CanConvert(Type objectType)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(objectType) && objectType.IsInterface)
            {
                var registration = DIResolver.GetImplementationType(objectType);
                return registration != null;
            }
            return false;
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var instance = DIResolver.Get(objectType);
            serializer.Populate(reader, instance);
            return instance;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        #endregion Methods
    }
}
