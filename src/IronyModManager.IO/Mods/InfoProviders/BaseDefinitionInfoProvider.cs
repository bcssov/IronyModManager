// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 04-04-2020
//
// Last Modified By : Mario
// Last Modified On : 05-23-2024
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

        /// <summary>
        /// The ut f8 body name
        /// </summary>
        private const string UTF8BodyName = "utf-8";

        /// <summary>
        /// The UTF8
        /// </summary>
        private static Encoding utf8;

        /// <summary>
        /// The UTF8 bom
        /// </summary>
        private static Encoding utf8Bom;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the fios paths.
        /// </summary>
        /// <value>The fios paths.</value>
        public abstract IReadOnlyCollection<string> FIOSPaths { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is fully implemented.
        /// </summary>
        /// <value><c>true</c> if this instance is fully implemented; otherwise, <c>false</c>.</value>
        public abstract bool IsFullyImplemented { get; }

        /// <summary>
        /// Gets a value indicating whether [supports inline scripts].
        /// </summary>
        /// <value><c>true</c> if [supports inline scripts]; otherwise, <c>false</c>.</value>
        public abstract bool SupportsInlineScripts { get; }

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
                return GetUTF8Encoding();
            }

            return GetUTF8BomEncoding();
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
        /// Determines whether [is valid encoding] [the specified path].
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns><c>true</c> if [is valid encoding] [the specified path]; otherwise, <c>false</c>.</returns>
        public virtual bool IsValidEncoding(string path, Shared.EncodingInfo encoding)
        {
            var sanitizedPath = path ?? string.Empty;
            if (sanitizedPath.StartsWith(Shared.Constants.LocalizationDirectory, StringComparison.OrdinalIgnoreCase))
            {
                return HasValidUTF8BOMEncoding(encoding);
            }

            return true;
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
            var fileNames = new List<string> { proposedFilename };
            fileNames.AddRange(definition.GeneratedFileNames);
            var counter = 0;
            if (isFIOS)
            {
                fileNames = fileNames.OrderBy(p => p, StringComparer.Ordinal).ToList();
                var characterPrefix = Path.GetFileName(fileNames.FirstOrDefault())!.ToCharArray().MinBy(p => (int)p);
                var newFileName = proposedFilename;
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
                var characterPrefix = Path.GetFileName(fileNames.FirstOrDefault())!.ToCharArray().MaxBy(p => (int)p);
                var newFileName = proposedFilename;
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
        /// <exception cref="System.ArgumentException">Invalid type.</exception>
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
                return GenerateWholeTextFileName(definition);
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
            var file = fileName;
            if (!string.IsNullOrWhiteSpace(definition.FileNameSuffix))
            {
                file = $"{Path.GetFileNameWithoutExtension(fileName)}_{definition.FileNameSuffix}{Path.GetExtension(fileName)}";
            }

            if (definition.ParentDirectory.Contains(Shared.Constants.LocalizationReplaceDirectory, StringComparison.OrdinalIgnoreCase))
            {
                var proposedFileName = Path.Combine(definition.ParentDirectory, $"{LIOSName}{file.GenerateValidFileName()}");
                return EnsureRuleEnforced(definition, proposedFileName, false);
            }
            else
            {
                var proposedFileName = Path.Combine(definition.ParentDirectory, Shared.Constants.LocalizationReplaceDirectory, $"{LIOSName}{file.GenerateValidFileName()}");
                return EnsureRuleEnforced(definition, proposedFileName, false);
            }
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
                if (definition.ValueType == ValueType.OverwrittenObjectSingleFile)
                {
                    return Path.Combine(definition.ParentDirectory, $"{FIOSName}{fileName.GenerateValidFileName()}");
                }

                return Path.Combine(definition.ParentDirectory, $"{FIOSName}{definition.Order:D8}{fileName.GenerateValidFileName()}");
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
                if (definition.ValueType == ValueType.OverwrittenObjectSingleFile)
                {
                    return Path.Combine(definition.ParentDirectory, $"{LIOSName}{fileName.GenerateValidFileName()}");
                }

                return Path.Combine(definition.ParentDirectory, $"{LIOSName}{definition.Order:D8}{fileName.GenerateValidFileName()}");
            }

            return Path.Combine(definition.ParentDirectory, $"{LIOSName}{fileName.GenerateValidFileName()}");
        }

        /// <summary>
        /// Determines whether [has valid ut f8 bom encoding] [the specified encoding information].
        /// </summary>
        /// <param name="encodingInfo">The encoding information.</param>
        /// <returns><c>true</c> if [has valid ut f8 bom encoding] [the specified encoding information]; otherwise, <c>false</c>.</returns>
        protected virtual bool HasValidUTF8BOMEncoding(Shared.EncodingInfo encodingInfo)
        {
            if (encodingInfo != null)
            {
                return encodingInfo.Encoding.Equals(UTF8BodyName, StringComparison.OrdinalIgnoreCase) && encodingInfo.HasBOM;
            }

            return false;
        }

        /// <summary>
        /// Gets the ut f8 bom encoding.
        /// </summary>
        /// <returns>Encoding.</returns>
        private Encoding GetUTF8BomEncoding()
        {
            utf8Bom ??= new UTF8Encoding(true);
            return utf8Bom;
        }

        /// <summary>
        /// Gets the ut f8 encoding.
        /// </summary>
        /// <returns>Encoding.</returns>
        private Encoding GetUTF8Encoding()
        {
            utf8 ??= new UTF8Encoding(false);
            return utf8;
        }

        #endregion Methods
    }
}
