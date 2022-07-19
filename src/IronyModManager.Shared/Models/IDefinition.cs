// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 07-12-2022
// ***********************************************************************
// <copyright file="IDefinition.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using CodexMicroORM.Core.Collections;
using Newtonsoft.Json;

namespace IronyModManager.Shared.Models
{
    /// <summary>
    /// Interface IDefinition
    /// Implements the <see cref="CodexMicroORM.Core.Collections.ICEFIndexedListItem" />
    /// Implements the <see cref="IronyModManager.Shared.Models.IQueryableModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.Models.IQueryableModel" />
    /// <seealso cref="CodexMicroORM.Core.Collections.ICEFIndexedListItem" />
    public interface IDefinition : ICEFIndexedListItem, IQueryableModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the additional file names.
        /// </summary>
        /// <value>The additional file names.</value>
        IList<string> AdditionalFileNames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow duplicate].
        /// </summary>
        /// <value><c>true</c> if [allow duplicate]; otherwise, <c>false</c>.</value>
        bool AllowDuplicate { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        string Code { get; set; }

        /// <summary>
        /// Gets or sets the code separator.
        /// </summary>
        /// <value>The code separator.</value>
        [JsonIgnore]
        string CodeSeparator { get; set; }

        /// <summary>
        /// Gets or sets the code tag.
        /// </summary>
        /// <value>The code tag.</value>
        [JsonIgnore]
        string CodeTag { get; set; }

        /// <summary>
        /// Gets or sets the content sha.
        /// </summary>
        /// <value>The content sha.</value>
        string ContentSHA { get; set; }

        /// <summary>
        /// Gets or sets the custom priority order.
        /// </summary>
        /// <value>The custom priority order.</value>
        int CustomPriorityOrder { get; set; }

        /// <summary>
        /// Gets the definition sha.
        /// </summary>
        /// <value>The definition sha.</value>
        string DefinitionSHA { get; set; }

        /// <summary>
        /// Gets or sets the dependencies.
        /// </summary>
        /// <value>The dependencies.</value>
        IEnumerable<string> Dependencies { get; set; }

        /// <summary>
        /// Gets or sets the disk file.
        /// </summary>
        /// <value>The disk file.</value>
        string DiskFile { get; set; }

        /// <summary>
        /// Gets the disk file ci.
        /// </summary>
        /// <value>The disk file ci.</value>
        string DiskFileCI { get; }

        /// <summary>
        /// Gets or sets the error column.
        /// </summary>
        /// <value>The error column.</value>
        long? ErrorColumn { get; set; }

        /// <summary>
        /// Gets or sets the error line.
        /// </summary>
        /// <value>The error line.</value>
        long? ErrorLine { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [exists in last file].
        /// </summary>
        /// <value><c>true</c> if [exists in last file]; otherwise, <c>false</c>.</value>
        bool ExistsInLastFile { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>The file.</value>
        string File { get; set; }

        /// <summary>
        /// Gets the file ci.
        /// </summary>
        /// <value>The file ci.</value>
        string FileCI { get; }

        /// <summary>
        /// Gets or sets the file name suffix.
        /// </summary>
        /// <value>The file name suffix.</value>
        string FileNameSuffix { get; set; }

        /// <summary>
        /// Gets or sets the generated file names.
        /// </summary>
        /// <value>The generated file names.</value>
        [JsonIgnore]
        IList<string> GeneratedFileNames { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is custom patch.
        /// </summary>
        /// <value><c>true</c> if this instance is custom patch; otherwise, <c>false</c>.</value>
        bool IsCustomPatch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is from game.
        /// </summary>
        /// <value><c>true</c> if this instance is from game; otherwise, <c>false</c>.</value>
        bool IsFromGame { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is placeholder.
        /// </summary>
        /// <value><c>true</c> if this instance is placeholder; otherwise, <c>false</c>.</value>
        bool IsPlaceholder { get; set; }

        /// <summary>
        /// Gets or sets the last modified.
        /// </summary>
        /// <value>The last modified.</value>
        [JsonIgnore]
        DateTime? LastModified { get; set; }

        /// <summary>
        /// Gets or sets the name of the mod.
        /// </summary>
        /// <value>The name of the mod.</value>
        string ModName { get; set; }

        /// <summary>
        /// Gets or sets the mod path.
        /// </summary>
        /// <value>The mod path.</value>
        [JsonIgnore]
        string ModPath { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        int Order { get; set; }

        /// <summary>
        /// Gets or sets the original code.
        /// </summary>
        /// <value>The original code.</value>
        [JsonIgnore]
        string OriginalCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the original file.
        /// </summary>
        /// <value>The name of the original file.</value>
        string OriginalFileName { get; set; }

        /// <summary>
        /// Gets or sets the name of the original mod.
        /// </summary>
        /// <value>The name of the original mod.</value>
        string OriginalModName { get; set; }

        /// <summary>
        /// Gets or sets the overwritten file names.
        /// </summary>
        /// <value>The overwritten file names.</value>
        IList<string> OverwrittenFileNames { get; set; }

        /// <summary>
        /// Gets the parent directory.
        /// </summary>
        /// <value>The parent directory.</value>
        string ParentDirectory { get; }

        /// <summary>
        /// Gets the parent directory ci.
        /// </summary>
        /// <value>The parent directory ci.</value>
        string ParentDirectoryCI { get; }

        /// <summary>
        /// Gets or sets the type of the reset.
        /// </summary>
        /// <value>The type of the reset.</value>
        [JsonIgnore]
        ResetType ResetType { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        [JsonIgnore]
        IList<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        string Type { get; set; }

        /// <summary>
        /// Gets the type and identifier.
        /// </summary>
        /// <value>The type and identifier.</value>
        string TypeAndId { get; }

        /// <summary>
        /// Gets or sets the used parser.
        /// </summary>
        /// <value>The used parser.</value>
        string UsedParser { get; set; }

        /// <summary>
        /// Gets or sets the type of the value.
        /// </summary>
        /// <value>The type of the value.</value>
        ValueType ValueType { get; set; }

        /// <summary>
        /// Gets or sets the variables.
        /// </summary>
        /// <value>The variables.</value>
        [JsonIgnore]
        IEnumerable<IDefinition> Variables { get; set; }

        /// <summary>
        /// Gets the virtual localization directory ci.
        /// </summary>
        /// <value>The virtual localization directory ci.</value>
        [JsonIgnore]
        string VirtualParentDirectory { get; }

        /// <summary>
        /// Gets the virtual parent directory ci.
        /// </summary>
        /// <value>The virtual parent directory ci.</value>
        [JsonIgnore]
        string VirtualParentDirectoryCI { get; }

        /// <summary>
        /// Gets or sets the virtual localization directory.
        /// </summary>
        /// <value>The virtual localization directory.</value>
        string VirtualPath { get; set; }

        #endregion Properties
    }
}
