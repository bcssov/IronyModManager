// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 02-08-2022
// ***********************************************************************
// <copyright file="StoreConverter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.DI.Assemblies;
using IronyModManager.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IronyModManager.Storage.Store
{
    /// <summary>
    /// Class StoreConverter.
    /// Implements the <see cref="Newtonsoft.Json.JsonConverter" />
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    [ExcludeFromCoverage("Modified version of Jot item, no need for us to test it.")]
    internal class StoreConverter : JsonConverter
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON.
        /// </summary>
        /// <value><c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON; otherwise, <c>false</c>.</value>
        public override bool CanRead
        {
            get { return true; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(StoreItem);
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
            reader.Read();
            var typeName = reader.ReadAsString();
            object instance = null;
            Type deserializeType = null;
            if (typeName.Equals(Constants.StoreDateId))
            {
                deserializeType = typeof(DateTime);
            }
            else if (typeName.StartsWith(nameof(IEnumerable)))
            {
                var splitType = typeName.Split(Constants.EnumerableOpenTag, StringSplitOptions.RemoveEmptyEntries)[1].Replace(Constants.EnumerableCloseTag, string.Empty).Trim();
                var type = AssemblyManager.FindType(splitType);
                var resolvedType = DIResolver.GetImplementationType(type);
                instance = Activator.CreateInstance(typeof(List<>).MakeGenericType(resolvedType));
            }
            else
            {
                var type = AssemblyManager.FindType(typeName);
                instance = DIResolver.Get(type);
            }

            reader.Read();
            var propName = reader.ReadAsString();

            reader.Read();
            reader.Read();
            if (deserializeType == null)
            {
                serializer.Populate(reader, instance);
            }
            else
            {
                instance = serializer.Deserialize(reader, deserializeType);
            }

            reader.Read();

            return new StoreItem() { Name = propName, Value = instance, Type = typeName };
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var converters = serializer.Converters.ToArray();
            var jObject = JObject.FromObject(value);
            jObject.WriteTo(writer, converters);
        }

        #endregion Methods
    }
}
