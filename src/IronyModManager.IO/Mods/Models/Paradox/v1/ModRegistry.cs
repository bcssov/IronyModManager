// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-14-2020
//
// Last Modified By : Mario
// Last Modified On : 09-26-2020
// ***********************************************************************
// <copyright file="ModRegistry.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.IO.Mods.Models.Paradox.Common;
using IronyModManager.Shared;
using Newtonsoft.Json;

namespace IronyModManager.IO.Mods.Models.Paradox.v1
{
    /// <summary>
    /// Class ModRegistry.
    /// Implements the <see cref="IronyModManager.IO.Mods.Models.Paradox.IPdxMod" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Mods.Models.Paradox.IPdxMod" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    internal class ModRegistry : IPdxMod
    {
        #region Fields

        /// <summary>
        /// The game registry identifier
        /// </summary>
        private string gameRegistryId = string.Empty;

        /// <summary>
        /// The status
        /// </summary>
        private string status = string.Empty;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModRegistry" /> class.
        /// </summary>
        public ModRegistry()
        {
            Tags = new List<string>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the archive path.
        /// </summary>
        /// <value>The archive path.</value>
        [JsonProperty("archivePath")]
        public string ArchivePath { get; set; }

        /// <summary>
        /// Gets or sets the cause.
        /// </summary>
        /// <value>The cause.</value>
        [JsonProperty("cause")]
        public string Cause { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the dir path.
        /// </summary>
        /// <value>The dir path.</value>
        [JsonProperty("dirPath")]
        public string DirPath { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the game registry identifier.
        /// </summary>
        /// <value>The game registry identifier.</value>
        [JsonProperty("gameRegistryId")]
        public string GameRegistryId
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(gameRegistryId))
                {
                    return gameRegistryId;
                }
                return string.Empty;
            }
            set
            {
                gameRegistryId = value;
            }
        }

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
        /// Gets or sets the PDX identifier.
        /// </summary>
        /// <value>The PDX identifier.</value>
        [JsonProperty("pdxId")]
        public string PdxId { get; set; }

        /// <summary>
        /// Gets or sets the repository path.
        /// </summary>
        /// <value>The repository path.</value>
        [JsonProperty("repositoryPath")]
        public string RepositoryPath { get; set; }

        /// <summary>
        /// Gets or sets the required version.
        /// </summary>
        /// <value>The required version.</value>
        [JsonProperty("requiredVersion")]
        public string RequiredVersion { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        [JsonProperty("source")]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        [JsonProperty("status")]
        public string Status
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(status))
                {
                    return status;
                }
                return string.Empty;
            }
            set
            {
                status = value;
            }
        }

        /// <summary>
        /// Gets or sets the steam identifier.
        /// </summary>
        /// <value>The steam identifier.</value>
        [JsonProperty("steamId")]
        public string SteamId { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail path.
        /// </summary>
        /// <value>The thumbnail path.</value>
        [JsonProperty("thumbnailPath")]
        public string ThumbnailPath { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail URL.
        /// </summary>
        /// <value>The thumbnail URL.</value>
        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Gets or sets the time updated.
        /// </summary>
        /// <value>The time updated.</value>
        [JsonProperty("timeUpdated")]
        public long? TimeUpdated { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [JsonProperty("version")]
        public string Version { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class ModRegistryCollection.
    /// Implements the <see cref="System.Collections.Generic.Dictionary{System.String, IronyModManager.IO.Mods.Models.Paradox.v1.ModRegistry}" />
    /// Implements the <see cref="IronyModManager.IO.Mods.Models.Paradox.Common.IPdxFormat" />
    /// </summary>
    /// <seealso cref="System.Collections.Generic.Dictionary{System.String, IronyModManager.IO.Mods.Models.Paradox.v1.ModRegistry}" />
    /// <seealso cref="IronyModManager.IO.Mods.Models.Paradox.Common.IPdxFormat" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    internal class ModRegistryCollection : Dictionary<string, ModRegistry>, IPdxFormat
    {
    }
}
