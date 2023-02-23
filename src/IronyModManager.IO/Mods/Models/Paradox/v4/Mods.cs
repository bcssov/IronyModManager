// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 02-23-2023
// ***********************************************************************
// <copyright file="Mods.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.IO.Mods.Models.Paradox.PropertyHandlers;
using RepoDb.Attributes;

namespace IronyModManager.IO.Mods.Models.Paradox.v4
{
    /// <summary>
    /// Class Mods.
    /// Implements the <see cref="IronyModManager.IO.Mods.Models.Paradox.IPdxMod" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Mods.Models.Paradox.IPdxMod" />
    [Map("mods")]
    internal class Mods : IPdxMod
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
        /// Initializes a new instance of the <see cref="Mods" /> class.
        /// </summary>
        public Mods()
        {
            Tags = new List<string>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the arch.
        /// </summary>
        /// <value>The arch.</value>
        [Map("arch")]
        public string Arch { get; set; }

        /// <summary>
        /// Gets or sets the archive path.
        /// </summary>
        /// <value>The archive path.</value>
        [Map("archivePath")]
        public string ArchivePath { get; set; }

        /// <summary>
        /// Gets or sets the cause.
        /// </summary>
        /// <value>The cause.</value>
        [Map("cause")]
        public string Cause { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Map("description")]
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the dir path.
        /// </summary>
        /// <value>The dir path.</value>
        [Map("dirPath")]
        public string DirPath { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        [Map("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the game registry identifier.
        /// </summary>
        /// <value>The game registry identifier.</value>
        [Map("gameRegistryId")]
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
        [Map("id"), Primary]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is new.
        /// </summary>
        /// <value><c>null</c> if [is new] contains no value, <c>true</c> if [is new]; otherwise, <c>false</c>.</value>
        [Map("isNew"), PropertyHandler(typeof(ObjectToBoolHandler))]
        public bool IsNew { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Map("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the os.
        /// </summary>
        /// <value>The os.</value>
        [Map("os")]
        public string OS { get; set; }

        /// <summary>
        /// Gets or sets the PDX identifier.
        /// </summary>
        /// <value>The PDX identifier.</value>
        [Map("pdxId")]
        public string PdxId { get; set; }

        /// <summary>
        /// Gets or sets the repository path.
        /// </summary>
        /// <value>The repository path.</value>
        [Map("repositoryPath")]
        public string RepositoryPath { get; set; }

        /// <summary>
        /// Gets or sets the required version.
        /// </summary>
        /// <value>The required version.</value>
        [Map("requiredVersion")]
        public string RequiredVersion { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        [Map("source")]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        [Map("status")]
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
        [Map("steamId")]
        public string SteamId { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        [Map("tags")]
        [PropertyHandler(typeof(JsonStringToListHandler))]
        public List<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail path.
        /// </summary>
        /// <value>The thumbnail path.</value>
        [Map("thumbnailUrl")]
        public string ThumbnailPath { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail URL.
        /// </summary>
        /// <value>The thumbnail URL.</value>
        [Map("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Gets or sets the time updated.
        /// </summary>
        /// <value>The time updated.</value>
        [Map("timeUpdated")]
        public long? TimeUpdated { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [Map("version")]
        public string Version { get; set; }

        #endregion Properties
    }
}
