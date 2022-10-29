// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-22-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2022
// ***********************************************************************
// <copyright file="ModObject.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
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
        /// Gets or sets the remote identifier.
        /// </summary>
        /// <value>The remote identifier.</value>
        [DescriptorProperty("remote_file_id")]
        public long? RemoteId { get; set; }

        /// <summary>
        /// Gets or sets the replace path.
        /// </summary>
        /// <value>The replace path.</value>
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
