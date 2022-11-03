// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-22-2020
//
// Last Modified By : Mario
// Last Modified On : 11-03-2022
// ***********************************************************************
// <copyright file="IModObject.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Shared.Models
{
    /// <summary>
    /// Interface IModObject
    /// </summary>
    public interface IModObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets the additional data.
        /// </summary>
        /// <value>The additional data.</value>
        IDictionary<string, object> AdditionalData { get; set; }

        /// <summary>
        /// Gets or sets the dependencies.
        /// </summary>
        /// <value>The dependencies.</value>
        IEnumerable<string> Dependencies { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        string FileName { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the picture.
        /// </summary>
        /// <value>The picture.</value>
        string Picture { get; set; }

        /// <summary>
        /// Gets or sets the remote identifier.
        /// </summary>
        /// <value>The remote identifier.</value>
        long? RemoteId { get; set; }

        /// <summary>
        /// Gets or sets the replace path.
        /// </summary>
        /// <value>The replace path.</value>
        IEnumerable<string> ReplacePath { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the user dir.
        /// </summary>
        /// <value>The user dir.</value>
        IEnumerable<string> UserDir { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        string Version { get; set; }

        #endregion Properties
    }
}
