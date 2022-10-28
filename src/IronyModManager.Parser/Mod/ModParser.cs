// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-22-2020
//
// Last Modified By : Mario
// Last Modified On : 10-28-2022
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

namespace IronyModManager.Parser.Mod
{
    /// <summary>
    /// Class ModParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Mod.IModParser" />
    /// Implements the <see cref="IronyModManager.Parser.BaseGenericObjectParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.BaseGenericObjectParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Mod.IModParser" />
    public class ModParser : BaseGenericObjectParser, IModParser
    {
        #region Fields

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModParser" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="codeParser">The code parser.</param>
        public ModParser(ILogger logger, ICodeParser codeParser) : base(codeParser)
        {
            this.logger = logger;
        }

        #endregion Constructors

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
                _ => ParseDescriptorMod(lines),
            };
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
        private IModObject ParseJsonMetadata(IEnumerable<string> lines)
        {
            var obj = DIResolver.Get<IModObject>();
            try
            {
                var json = string.Join(Environment.NewLine, lines);
                var result = JsonConvert.DeserializeObject<JsonMetadata>(json, new JsonSerializerSettings()
                {
                    Error = (sender, error) => error.ErrorContext.Handled = true,
                    NullValueHandling = NullValueHandling.Ignore
                });
                obj.ReplacePath = result.ReplacePaths;
                obj.UserDir = result.UserDir;
                obj.Name = result.Name;
                obj.Version = !string.IsNullOrWhiteSpace(result.SupportedGameVersion) ? result.SupportedGameVersion : result.Version;
                obj.Tags = result.Tags;
                if (long.TryParse(result.Id, out var id))
                {
                    obj.RemoteId = id;
                }
                obj.Dependencies = result.Relationships;
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
        private class JsonMetadata
        {
            #region Properties

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
            /// Gets or sets the relationships.
            /// </summary>
            /// <value>The relationships.</value>
            [JsonProperty("relationships")]
            public List<string> Relationships { get; set; }

            /// <summary>
            /// Gets or sets the replace paths.
            /// </summary>
            /// <value>The replace paths.</value>
            [JsonProperty("replace_paths")]
            public List<string> ReplacePaths { get; set; }

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
            /// Gets or sets the user dir.
            /// </summary>
            /// <value>The user dir.</value>
            [JsonProperty("user_dir")]
            public List<string> UserDir { get; set; }

            /// <summary>
            /// Gets or sets the version.
            /// </summary>
            /// <value>The version.</value>
            [JsonProperty("version")]
            public string Version { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
