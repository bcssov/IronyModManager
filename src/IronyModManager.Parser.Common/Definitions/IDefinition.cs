// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common.Definitions
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 11-24-2020
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

namespace IronyModManager.Parser.Common.Definitions
{
    /// <summary>
    /// Interface IDefinition
    /// Implements the <see cref="CodexMicroORM.Core.Collections.ICEFIndexedListItem" />
    /// </summary>
    /// <seealso cref="CodexMicroORM.Core.Collections.ICEFIndexedListItem" />
    public interface IDefinition : ICEFIndexedListItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets the additional file names.
        /// </summary>
        /// <value>The additional file names.</value>
        IList<string> AdditionalFileNames { get; set; }

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
        [JsonIgnore]
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

        #endregion Properties
    }
}
