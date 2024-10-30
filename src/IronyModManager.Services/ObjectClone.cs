// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 05-14-2023
//
// Last Modified By : Mario
// Last Modified On : 10-30-2024
// ***********************************************************************
// <copyright file="ObjectClone.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class ObjectClone.
    /// Implements the <see cref="IObjectClone" />
    /// </summary>
    /// <seealso cref="IObjectClone" />
    public class ObjectClone : IObjectClone
    {
        #region Methods

        /// <summary>
        /// Clones the definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="includeCode">if set to <c>true</c> [include code].</param>
        /// <returns>IDefinition.</returns>
        public IDefinition CloneDefinition(IDefinition definition, bool includeCode)
        {
            var newDefinition = DIResolver.Get<IDefinition>();
            if (includeCode)
            {
                newDefinition.Code = definition.Code;
            }

            newDefinition.ContentSHA = definition.ContentSHA;
            newDefinition.DefinitionSHA = definition.DefinitionSHA;
            newDefinition.Dependencies = definition.Dependencies;
            newDefinition.ErrorColumn = definition.ErrorColumn;
            newDefinition.ErrorLine = definition.ErrorLine;
            newDefinition.ErrorMessage = definition.ErrorMessage;
            newDefinition.File = definition.File;
            newDefinition.GeneratedFileNames = definition.GeneratedFileNames;
            newDefinition.OverwrittenFileNames = definition.OverwrittenFileNames;
            newDefinition.AdditionalFileNames = definition.AdditionalFileNames;
            newDefinition.Id = definition.Id;
            newDefinition.ModName = definition.ModName;
            newDefinition.Type = definition.Type;
            newDefinition.UsedParser = definition.UsedParser;
            newDefinition.ValueType = definition.ValueType;
            newDefinition.Tags = definition.Tags;
            newDefinition.OriginalCode = definition.OriginalCode;
            newDefinition.CodeSeparator = definition.CodeSeparator;
            newDefinition.CodeTag = definition.CodeTag;
            newDefinition.Order = definition.Order;
            newDefinition.OriginalModName = definition.OriginalModName;
            newDefinition.OriginalFileName = definition.OriginalFileName;
            newDefinition.DiskFile = definition.DiskFile;
            newDefinition.Variables = definition.Variables;
            newDefinition.ExistsInLastFile = definition.ExistsInLastFile;
            newDefinition.VirtualPath = definition.VirtualPath;
            newDefinition.CustomPriorityOrder = definition.CustomPriorityOrder;
            newDefinition.IsCustomPatch = definition.IsCustomPatch;
            newDefinition.IsFromGame = definition.IsFromGame;
            newDefinition.AllowDuplicate = definition.AllowDuplicate;
            newDefinition.ResetType = definition.ResetType;
            newDefinition.FileNameSuffix = definition.FileNameSuffix;
            newDefinition.IsPlaceholder = definition.IsPlaceholder;
            newDefinition.LastModified = definition.LastModified;
            newDefinition.OriginalId = definition.OriginalId;
            newDefinition.UseSimpleValidation = definition.UseSimpleValidation;
            newDefinition.IsSpecialFolder = definition.IsSpecialFolder;
            newDefinition.MergeType = definition.MergeType;
            newDefinition.ContainsInlineIdentifier = definition.ContainsInlineIdentifier;
            return newDefinition;
        }

        /// <summary>
        /// Partials the clone definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="copyAdditionalFilenames">if set to <c>true</c> [copy additional filenames].</param>
        /// <returns>IDefinition.</returns>
        public IDefinition PartialCloneDefinition(IDefinition definition, bool copyAdditionalFilenames = true)
        {
            var copy = DIResolver.Get<IDefinition>();
            if (copyAdditionalFilenames)
            {
                copy.AdditionalFileNames = definition.AdditionalFileNames;
            }

            copy.DiskFile = definition.DiskFile;
            copy.File = definition.File;
            copy.Id = definition.Id;
            copy.ModName = definition.ModName;
            copy.Tags = definition.Tags;
            copy.Type = definition.Type;
            copy.ValueType = definition.ValueType;
            copy.IsFromGame = definition.IsFromGame;
            copy.Order = definition.Order;
            copy.OriginalFileName = definition.OriginalFileName;
            copy.ResetType = definition.ResetType;
            copy.FileNameSuffix = definition.FileNameSuffix;
            copy.IsPlaceholder = definition.IsPlaceholder;
            copy.UseSimpleValidation = definition.UseSimpleValidation;
            copy.IsSpecialFolder = definition.IsSpecialFolder;
            copy.MergeType = definition.MergeType;
            copy.ContainsInlineIdentifier = definition.ContainsInlineIdentifier;
            return copy;
        }

        #endregion Methods
    }
}
