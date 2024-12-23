// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-22-2020
//
// Last Modified By : Mario
// Last Modified On : 12-23-2024
// ***********************************************************************
// <copyright file="ModObject.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;

namespace IronyModManager.Parser.Mod
{
    /// <summary>
    /// Class ModObject.
    /// Implements the <see cref="IronyModManager.Shared.Models.IModObject" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.Models.IModObject" />
    public class ModObject : IModObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets the additional data.
        /// </summary>
        /// <value>The additional data.</value>
        public IDictionary<string, object> AdditionalData { get; set; }

        /// <summary>
        /// Gets or sets the dependencies.
        /// </summary>
        /// <value>The dependencies.</value>
        [DescriptorProperty("dependencies")]
        public IEnumerable<string> Dependencies { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        [DescriptorProperty("path", "archive", ".zip", ".bin")]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the json identifier.
        /// </summary>
        /// <value>The json identifier.</value>
        public string JsonId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DescriptorProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the picture.
        /// </summary>
        /// <value>The picture.</value>
        [DescriptorProperty("picture")]
        public string Picture { get; set; }

        /// <summary>
        /// Gets or sets the relationship data.
        /// </summary>
        /// <value>The relationship data.</value>
        public IEnumerable<IDictionary<string, object>> RelationshipData { get; set; }

        /// <summary>
        /// Gets or sets the remote identifier.
        /// </summary>
        /// <value>The remote identifier.</value>
        [DescriptorProperty("remote_file_id")]
        public long? RemoteId { get; set; }

        /// <summary>
        /// Gets or sets the replacement path.
        /// </summary>
        /// <value>The replacement path.</value>
        [DescriptorProperty("replace_path", true)]
        public IEnumerable<string> ReplacePath { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        [DescriptorProperty("tags")]
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the user dir.
        /// </summary>
        /// <value>The user dir.</value>
        [DescriptorProperty("user_dir", true)]
        public IEnumerable<string> UserDir { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [DescriptorProperty("supported_version")]
        public string Version { get; set; }

        #endregion Properties
    }
}
