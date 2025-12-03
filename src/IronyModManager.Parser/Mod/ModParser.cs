// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-22-2020
//
// Last Modified By : Mario
// Last Modified On : 12-02-2025
// ***********************************************************************
// <copyright file="ModParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IronyModManager.Parser.Mod
{
    /// <summary>
    /// Class ModParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Mod.IModParser" />
    /// Implements the <see cref="IronyModManager.Parser.BaseGenericObjectParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.BaseGenericObjectParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Mod.IModParser" />
    /// <remarks>Initializes a new instance of the <see cref="ModParser" /> class.</remarks>
    public class ModParser(ILogger logger, ICodeParser codeParser) : BaseGenericObjectParser(codeParser), IModParser
    {
        #region Fields

        /// <summary>
        /// The display name key
        /// </summary>
        private const string DisplayNameKey = "display_name";

        /// <summary>
        /// The resource type key
        /// </summary>
        private const string ResourceTypeKey = "resource_type";

        /// <summary>
        /// The resource type key value
        /// </summary>
        private const string ResourceTypeKeyValue = "mod";

        /// <summary>
        /// The json serializer settings
        /// </summary>
        private static JsonSerializerSettings jsonSerializerSettings;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger = logger;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Parses the specified lines.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <param name="descriptorModType">Type of the descriptor mod.</param>
        /// <returns>IModObject.</returns>
        public IModObject Parse(IEnumerable<string> lines, DescriptorModType descriptorModType)
        {
            return descriptorModType switch
            {
                DescriptorModType.JsonMetadata => ParseJsonMetadata(lines),
                DescriptorModType.JsonMetadataV2 => ParseJsonMetadata(lines), // Has not changed but they dropped pdx launcher in EU5
                _ => ParseDescriptorMod(lines)
            };
        }

        /// <summary>
        /// Gets the json serializer settings.
        /// </summary>
        /// <returns>JsonSerializerSettings.</returns>
        private JsonSerializerSettings GetJsonSerializerSettings()
        {
            jsonSerializerSettings ??= new JsonSerializerSettings { Error = (_, error) => error.ErrorContext.Handled = true, NullValueHandling = NullValueHandling.Ignore, Converters = [new JsonMetaDataConverter()] };
            return jsonSerializerSettings;
        }

        /// <summary>
        /// Parses the descriptor mod.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <returns>IModObject.</returns>
        private IModObject ParseDescriptorMod(IEnumerable<string> lines)
        {
            var data = ParseCode(lines);
            var obj = DIResolver.Get<IModObject>();
            if (data != null)
            {
                obj.ReplacePath = GetKeyedValues<string>(data.Values, "replace_path");
                obj.UserDir = GetKeyedValues<string>(data.Values, "user_dir");
                obj.FileName = GetValue<string>(data.Values, "path", "archive") ?? string.Empty;
                obj.Picture = GetValue<string>(data.Values, "picture") ?? string.Empty;
                obj.Name = GetValue<string>(data.Values, "name") ?? string.Empty;
                obj.Version = GetValue<string>(data.Values, "supported_version", "version") ?? string.Empty;
                obj.Tags = GetValues<string>(data.Values, "tags");
                obj.RemoteId = GetValue<long?>(data.Values, "remote_file_id");
                obj.Dependencies = GetValues<string>(data.Values, "dependencies");
            }

            return obj;
        }

        /// <summary>
        /// Parses the json metadata.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <returns>IModObject.</returns>
        /// <exception cref="System.AggregateException"></exception>
        private IModObject ParseJsonMetadata(IEnumerable<string> lines)
        {
            var obj = DIResolver.Get<IModObject>();
            try
            {
                var json = string.Join(Environment.NewLine, lines);
                var result = JsonConvert.DeserializeObject<JsonMetadataBase>(json, GetJsonSerializerSettings());

                if (result!.GameCustomData != null)
                {
                    if (result.GameCustomData.TryGetValue(Shared.Constants.JsonMetadataReplacePaths, out var replacePath))
                    {
                        if (replacePath is JArray jArray)
                        {
                            obj.ReplacePath = jArray.ToObject<List<string>>();
                        }
                    }

                    if (result.GameCustomData.TryGetValue(Shared.Constants.DescriptorUserDir, out var userDir))
                    {
                        if (userDir is JArray jArray)
                        {
                            obj.UserDir = jArray.ToObject<List<string>>();
                        }
                    }

                    obj.AdditionalData = result.GameCustomData.Where(p => p.Key != Shared.Constants.JsonMetadataReplacePaths && p.Key != Shared.Constants.DescriptorUserDir).ToDictionary(p => p.Key, p => p.Value);
                }

                obj.FileName = result.Path;
                obj.Name = result.Name;
                obj.Version = result.SupportedGameVersion;
                obj.Tags = result.Tags;
                if (long.TryParse(result.Id, out var id))
                {
                    obj.RemoteId = id;
                }

                obj.JsonId = result.Id;

                switch (result)
                {
                    case JsonMetadata metadata:
                        obj.Dependencies = metadata.Relationships;
                        break;
                    case JsonMetadataV2 metadataV2:
                    {
                        var dependencies = new List<string>();
                        if (metadataV2.Relationships != null)
                        {
                            foreach (var relationship in metadataV2.Relationships)
                            {
                                if (relationship.TryGetValue(ResourceTypeKey, out var resTypeVal))
                                {
                                    if (resTypeVal != null)
                                    {
                                        var resTypeStr = resTypeVal.ToString();
                                        if (resTypeStr!.Equals(ResourceTypeKeyValue, StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (relationship.TryGetValue(DisplayNameKey, out var displayName))
                                            {
                                                if (displayName != null)
                                                {
                                                    dependencies.Add(displayName.ToString());
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            obj.RelationshipData = metadataV2.Relationships;
                        }

                        if (dependencies.Count > 0)
                        {
                            obj.Dependencies = dependencies;
                        }

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return obj;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class JsonMetadata.
        /// </summary>
        private class JsonMetadata : JsonMetadataBase
        {
            #region Properties

            /// <summary>
            /// Gets or sets the relationships.
            /// </summary>
            /// <value>The relationships.</value>
            [JsonProperty("relationships")]
            public List<string> Relationships { get; set; }

            #endregion Properties
        }

        /// <summary>
        /// Class JsonMetadataBase.
        /// </summary>
        private abstract class JsonMetadataBase
        {
            #region Properties

            /// <summary>
            /// Gets or sets the game custom data.
            /// </summary>
            /// <value>The game custom data.</value>
            [JsonProperty("game_custom_data")]
            public Dictionary<string, object> GameCustomData { get; set; }

            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            /// <value>The identifier.</value>
            [JsonProperty("id")]
            public string Id { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            [JsonProperty("name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the path.
            /// </summary>
            /// <value>The path.</value>
            [JsonProperty("path")]
            public string Path { get; set; }

            /// <summary>
            /// Gets or sets the short description.
            /// </summary>
            /// <value>The short description.</value>
            [JsonProperty("short_description")]
            public string ShortDescription { get; set; }

            /// <summary>
            /// Gets or sets the supported game version.
            /// </summary>
            /// <value>The supported game version.</value>
            [JsonProperty("supported_game_version")]
            public string SupportedGameVersion { get; set; }

            /// <summary>
            /// Gets or sets the tags.
            /// </summary>
            /// <value>The tags.</value>
            [JsonProperty("tags")]
            public List<string> Tags { get; set; }

            /// <summary>
            /// Gets or sets the version.
            /// </summary>
            /// <value>The version.</value>
            [JsonProperty("version")]
            public string Version { get; set; }

            #endregion Properties
        }

        /// <summary>
        /// Class JsonMetaDataConverter.
        /// Implements the <see cref="Newtonsoft.Json.JsonConverter{IronyModManager.Parser.Mod.ModParser.JsonMetadataBase}" />
        /// </summary>
        /// <seealso cref="Newtonsoft.Json.JsonConverter{IronyModManager.Parser.Mod.ModParser.JsonMetadataBase}" />
        private class JsonMetaDataConverter : JsonConverter<JsonMetadataBase>
        {
            #region Methods

            /// <summary>
            /// Reads the JSON representation of the object.
            /// </summary>
            /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
            /// <param name="objectType">Type of the object.</param>
            /// <param name="existingValue">The existing value of object being read. If there is no existing value then <c>null</c> will be used.</param>
            /// <param name="hasExistingValue">The existing value has a value.</param>
            /// <param name="serializer">The calling serializer.</param>
            /// <returns>The object value.</returns>
            public override JsonMetadataBase ReadJson(JsonReader reader, Type objectType, JsonMetadataBase existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var obj = JObject.Load(reader);
                var relationshipToken = obj["relationships"];
                JsonMetadataBase result = null;
                if (relationshipToken != null)
                {
                    result = relationshipToken.Type switch
                    {
                        JTokenType.Array => CheckArrayType(relationshipToken as JArray),
                        _ => new JsonMetadataV2()
                    };
                }

                result ??= new JsonMetadataV2();
                serializer.Populate(obj.CreateReader(), result);
                return result;
            }

            /// <summary>
            /// Writes the JSON representation of the object.
            /// </summary>
            /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
            /// <param name="value">The value.</param>
            /// <param name="serializer">The calling serializer.</param>
            /// <exception cref="System.NotSupportedException"></exception>
            public override void WriteJson(JsonWriter writer, JsonMetadataBase value, JsonSerializer serializer)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Checks the type of the array.
            /// </summary>
            /// <param name="token">The token.</param>
            /// <returns>JsonMetadataBase.</returns>
            private JsonMetadataBase CheckArrayType(JToken token)
            {
                if (token.All(p => p.Type == JTokenType.String))
                {
                    return new JsonMetadata();
                }

                return new JsonMetadataV2();
            }

            #endregion Methods
        }

        /// <summary>
        /// Class JsonMetadataV2.
        /// </summary>
        private class JsonMetadataV2 : JsonMetadataBase
        {
            #region Properties

            /// <summary>
            /// Gets or sets the relationships.
            /// </summary>
            /// <value>The relationships.</value>
            [JsonProperty("relationships")]
            public List<Dictionary<string, object>> Relationships { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
