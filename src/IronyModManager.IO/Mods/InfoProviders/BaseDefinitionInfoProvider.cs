// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 04-04-2020
//
// Last Modified By : Mario
// Last Modified On : 10-25-2021
// ***********************************************************************
// <copyright file="BaseDefinitionInfoProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IronyModManager.IO.Common.Mods;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.IO.Mods.InfoProviders
{
    /// <summary>
    /// Class BaseDefinitionInfoProvider.
    /// Implements the <see cref="IronyModManager.IO.Common.Mods.IDefinitionInfoProvider" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Mods.IDefinitionInfoProvider" />
    public abstract class BaseDefinitionInfoProvider : IDefinitionInfoProvider
    {
        #region Fields

        /// <summary>
        /// The fios name
        /// </summary>
        protected const string FIOSName = "!!!_";

        /// <summary>
        /// The lios name
        /// </summary>
        protected const string LIOSName = "zzz_";

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the fios paths.
        /// </summary>
        /// <value>The fios paths.</value>
        public abstract IReadOnlyCollection<string> FIOSPaths { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Values the tuple.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>CanProcess.</returns>
        public abstract bool CanProcess(string game);

        /// <summary>
        /// Definitions the uses fios rules.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool DefinitionUsesFIOSRules(IDefinition definition)
        {
            return FIOSPaths.Any(p => definition.ParentDirectory.EndsWith(p, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the name of the disk file.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>System.String.</returns>
        public string GetDiskFileName(IDefinition definition)
        {
            return GenerateFileName(definition, true);
        }

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>Encoding.</returns>
        public virtual Encoding GetEncoding(IDefinition definition)
        {
            EnsureValidType(definition);
            if (!definition.ParentDirectory.StartsWith(Shared.Constants.LocalizationDirectory, StringComparison.OrdinalIgnoreCase))
            {
                return new UTF8Encoding(false);
            }
            return new UTF8Encoding(true);
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>System.String.</returns>
        public virtual string GetFileName(IDefinition definition)
        {
            return GenerateFileName(definition, false);
        }

        /// <summary>
        /// Ensures the rule enforced.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="proposedFilename">The proposed filename.</param>
        /// <param name="isFIOS">if set to <c>true</c> [is fios].</param>
        /// <returns>System.String.</returns>
        protected virtual string EnsureRuleEnforced(IDefinition definition, string proposedFilename, bool isFIOS)
        {
            var fileNames = new List<string>() { proposedFilename };
            fileNames.AddRange(definition.GeneratedFileNames);
            int counter = 0;
            if (isFIOS)
            {
                fileNames = fileNames.OrderBy(p => p, StringComparer.Ordinal).ToList();
                var characterPrefix = Path.GetFileName(fileNames.FirstOrDefault()).First();
                string newFileName = proposedFilename;
                while (definition.GeneratedFileNames.Any(f => f.Equals(fileNames.FirstOrDefault())))
                {
                    var fileName = Path.GetFileName(newFileName);
                    newFileName = newFileName.Replace(fileName, $"{characterPrefix}{fileName}");
                    fileNames.Add(newFileName);
                    fileNames = fileNames.OrderBy(p => p, StringComparer.Ordinal).ToList();
                    counter++;
                    if (counter > 10)
                    {
                        return proposedFilename;
                    }
                }
            }
            else
            {
                fileNames = fileNames.OrderByDescending(p => p, StringComparer.Ordinal).ToList();
                var characterPrefix = Path.GetFileName(fileNames.FirstOrDefault()).First();
                string newFileName = proposedFilename;
                while (definition.GeneratedFileNames.Any(f => f.Equals(fileNames.FirstOrDefault())))
                {
                    var fileName = Path.GetFileName(newFileName);
                    newFileName = newFileName.Replace(fileName, $"{characterPrefix}{fileName}");
                    fileNames.Add(newFileName);
                    fileNames = fileNames.OrderByDescending(p => p, StringComparer.Ordinal).ToList();
                    counter++;
                    if (counter > 10)
                    {
                        return proposedFilename;
                    }
                }
            }

            return fileNames.FirstOrDefault();
        }

        /// <summary>
        /// Ensures the type of all same.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <exception cref="ArgumentException">Invalid type.</exception>
        protected virtual void EnsureValidType(IDefinition definition)
        {
            if (definition.ValueType == ValueType.Variable || definition.ValueType == ValueType.Namespace)
            {
                throw new ArgumentException("Invalid type.");
            }
        }

        /// <summary>
        /// Generates the name of the file.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="requestDiskFileName">if set to <c>true</c> [request true file name].</param>
        /// <returns>System.String.</returns>
        protected virtual string GenerateFileName(IDefinition definition, bool requestDiskFileName)
        {
            EnsureValidType(definition);
            var fileName = definition.ValueType == ValueType.WholeTextFile ? Path.GetFileName(definition.File) : $"{definition.Id}{Path.GetExtension(definition.File)}";
            if (definition.ValueType == ValueType.WholeTextFile)
            {
                GenerateWholeTextFileName(definition);
            }
            else if (FIOSPaths.Any(p => definition.ParentDirectory.EndsWith(p, StringComparison.OrdinalIgnoreCase)))
            {
                return GenerateFIOSFileName(definition, requestDiskFileName, fileName);
            }
            else if (definition.ParentDirectory.StartsWith(Shared.Constants.LocalizationDirectory, StringComparison.OrdinalIgnoreCase))
            {
                return GenerateLocalizationFileName(definition, fileName);
            }
            return GenerateLIOSFileName(definition, requestDiskFileName, fileName);
        }

        /// <summary>
        /// Generates the name of the fios file.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="requestDiskFileName">if set to <c>true</c> [request disk file name].</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>System.String.</returns>
        protected virtual string GenerateFIOSFileName(IDefinition definition, bool requestDiskFileName, string fileName)
        {
            var fiosFileName = GetFIOSFileName(definition, fileName, requestDiskFileName);
            return EnsureRuleEnforced(definition, fiosFileName, true);
        }

        /// <summary>
        /// Generates the name of the lios file.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="requestDiskFileName">if set to <c>true</c> [request disk file name].</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>System.String.</returns>
        protected virtual string GenerateLIOSFileName(IDefinition definition, bool requestDiskFileName, string fileName)
        {
            var liosFileName = GetLIOSFileName(definition, fileName, requestDiskFileName);
            return EnsureRuleEnforced(definition, liosFileName, false);
        }

        /// <summary>
        /// Generates the name of the localization file.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>System.String.</returns>
        protected virtual string GenerateLocalizationFileName(IDefinition definition, string fileName)
        {
            if (definition.ParentDirectory.Contains(Shared.Constants.LocalizationReplaceDirectory, StringComparison.OrdinalIgnoreCase))
            {
                var proposedFileName = Path.Combine(definition.ParentDirectory, $"{LIOSName}{fileName.GenerateValidFileName()}");
                return EnsureRuleEnforced(definition, proposedFileName, false);
            }
            else
            {
                var proposedFileName = Path.Combine(definition.ParentDirectory, Shared.Constants.LocalizationReplaceDirectory, $"{LIOSName}{fileName.GenerateValidFileName()}");
                return EnsureRuleEnforced(definition, proposedFileName, false);
            }
        }

        /// <summary>
        /// Generates the name hash.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <param name="takeLeadingFileName">if set to <c>true</c> [take leading file name].</param>
        /// <returns>System.String.</returns>
        protected virtual string GenerateNameHash(string hash, bool takeLeadingFileName = false)
        {
            hash = Path.GetFileNameWithoutExtension(hash);
            if (string.IsNullOrWhiteSpace(hash))
            {
                return hash;
            }
            var length = 2;
            if (hash.Length < 2)
            {
                length = hash.Length - 1;
            }
            if (takeLeadingFileName)
            {
                return $"{hash.Substring(0, length)}{hash.GenerateShortFileNameHashId()}";
            }
            return hash.GenerateShortFileNameHashId();
        }

        /// <summary>
        /// Generates the unique file name signature.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>System.String.</returns>
        protected virtual string GenerateUniqueFileNameSignature(string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);
            var length = 6;
            if (fileName.Length < 6)
            {
                length = fileName.Length - 1;
            }
            return fileName.Substring(0, length);
        }

        /// <summary>
        /// Generates the name of the whole text file.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>System.String.</returns>
        protected virtual string GenerateWholeTextFileName(IDefinition definition)
        {
            return definition.File;
        }

        /// <summary>
        /// Gets the name of the fios file.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="requestDiskFileName">if set to <c>true</c> [request disk file name].</param>
        /// <returns>System.String.</returns>
        protected virtual string GetFIOSFileName(IDefinition definition, string fileName, bool requestDiskFileName)
        {
            if (requestDiskFileName)
            {
                if (definition.ValueType != ValueType.OverWrittenObjectWithPreserveFileName)
                {
                    if (definition.ValueType == ValueType.OverwrittenObjectSingleFile)
                    {
                        return Path.Combine(definition.ParentDirectory, $"{FIOSName}{fileName.GenerateValidFileName()}");
                    }
                    return Path.Combine(definition.ParentDirectory, $"{FIOSName}{GenerateNameHash(definition.OriginalModName)}{(string.IsNullOrWhiteSpace(definition.OriginalFileName) ? GenerateNameHash(fileName, true) : GenerateNameHash(definition.OriginalFileName, true))}{definition.Order:D4}{fileName.GenerateValidFileName()}");
                }
                else
                {
                    return Path.Combine(definition.ParentDirectory, $"{FIOSName}{(string.IsNullOrWhiteSpace(definition.OriginalFileName) ? GenerateUniqueFileNameSignature(fileName) : GenerateUniqueFileNameSignature(definition.OriginalFileName))}{definition.Order:D4}{fileName.GenerateValidFileName()}");
                }
            }
            return Path.Combine(definition.ParentDirectory, $"{FIOSName}{fileName.GenerateValidFileName()}");
        }

        /// <summary>
        /// Gets the name of the lios file.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="requestDiskFileName">if set to <c>true</c> [request disk file name].</param>
        /// <returns>System.String.</returns>
        protected virtual string GetLIOSFileName(IDefinition definition, string fileName, bool requestDiskFileName)
        {
            if (requestDiskFileName)
            {
                if (definition.ValueType != ValueType.OverWrittenObjectWithPreserveFileName)
                {
                    if (definition.ValueType == ValueType.OverwrittenObjectSingleFile)
                    {
                        return Path.Combine(definition.ParentDirectory, $"{LIOSName}{fileName.GenerateValidFileName()}");
                    }
                    return Path.Combine(definition.ParentDirectory, $"{LIOSName}{GenerateNameHash(definition.OriginalModName)}{(string.IsNullOrWhiteSpace(definition.OriginalFileName) ? GenerateNameHash(fileName, true) : GenerateNameHash(definition.OriginalFileName, true))}{definition.Order:D4}{fileName.GenerateValidFileName()}");
                }
                else
                {
                    return Path.Combine(definition.ParentDirectory, $"{LIOSName}{(string.IsNullOrWhiteSpace(definition.OriginalFileName) ? GenerateUniqueFileNameSignature(fileName) : GenerateUniqueFileNameSignature(definition.OriginalFileName))}{definition.Order:D4}{fileName.GenerateValidFileName()}");
                }
            }
            return Path.Combine(definition.ParentDirectory, $"{LIOSName}{fileName.GenerateValidFileName()}");
        }

        #endregion Methods
    }
}
