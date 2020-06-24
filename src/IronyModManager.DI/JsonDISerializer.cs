// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 04-07-2020
//
// Last Modified By : Mario
// Last Modified On : 06-24-2020
// ***********************************************************************
// <copyright file="JsonDISerializer.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.DI.Json;
using Newtonsoft.Json;

namespace IronyModManager.DI
{
    /// <summary>
    /// Class JsonDISerializer.
    /// </summary>
    public static class JsonDISerializer
    {
        #region Fields

        /// <summary>
        /// The converters
        /// </summary>
        private static readonly List<JsonConverter> converters = new List<JsonConverter>() { new JsonDIConverter() };

        /// <summary>
        /// The resolver
        /// </summary>
        private static readonly WritablePropertiesResolver resolver = new WritablePropertiesResolver();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Deserializes the specified text.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">The text.</param>
        /// <returns>T.</returns>
        public static T Deserialize<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text, GetSettings(true));
        }

        /// <summary>
        /// Serializes the specified model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">The model.</param>
        /// <returns>System.String.</returns>
        public static string Serialize<T>(T model)
        {
            return JsonConvert.SerializeObject(model, Formatting.None, GetSettings(false));
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <param name="includeConverter">if set to <c>true</c> [include converter].</param>
        /// <returns>JsonSerializerSettings.</returns>
        private static JsonSerializerSettings GetSettings(bool includeConverter)
        {
            if (includeConverter)
            {
                return new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = resolver,
                    Converters = converters
                };
            }
            else
            {
                return new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = resolver
                };
            }
        }

        #endregion Methods
    }
}
