// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-22-2020
//
// Last Modified By : Mario
// Last Modified On : 04-10-2020
// ***********************************************************************
// <copyright file="ModObject.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Shared;

namespace IronyModManager.Parser.Mod
{
    /// <summary>
    /// Class ModObject.
    /// Implements the <see cref="IronyModManager.Parser.Common.Mod.IModObject" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Mod.IModObject" />
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
        [DescriptorProperty("path", "archive", ".zip")]
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
        public int? RemoteId { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        [DescriptorProperty("tags")]
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [DescriptorProperty("supported_version")]
        public string Version { get; set; }

        #endregion Properties
    }
}
