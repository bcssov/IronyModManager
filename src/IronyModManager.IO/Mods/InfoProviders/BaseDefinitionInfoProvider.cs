// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 04-04-2020
//
// Last Modified By : Mario
// Last Modified On : 07-15-2020
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
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Shared;

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
        /// Gets the encoding.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>Encoding.</returns>
        public virtual Encoding GetEncoding(IDefinition definition)
        {
            EnsureValidType(definition);
            if (!definition.ParentDirectory.StartsWith(Constants.LocalizationDirectory, StringComparison.OrdinalIgnoreCase))
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
            EnsureValidType(definition);
            var fileName = definition.ValueType == Parser.Common.ValueType.WholeTextFile ? Path.GetFileName(definition.File) : $"{definition.Id}{Path.GetExtension(definition.File)}";
            string proposedFileName = string.Empty;
            if (definition.ValueType == Parser.Common.ValueType.WholeTextFile)
            {
                return definition.File;
            }
            else if (FIOSPaths.Any(p => definition.ParentDirectory.EndsWith(p, StringComparison.OrdinalIgnoreCase)))
            {
                proposedFileName = Path.Combine(definition.ParentDirectory, $"{FIOSName}{fileName.GenerateValidFileName()}");
                return EnsureRuleEnforced(definition, proposedFileName, true);
            }
            else if (definition.ParentDirectory.StartsWith(Constants.LocalizationDirectory, StringComparison.OrdinalIgnoreCase))
            {
                if (definition.ParentDirectory.Contains(Constants.LocalizationReplaceDirectory, StringComparison.OrdinalIgnoreCase))
                {
                    proposedFileName = Path.Combine(definition.ParentDirectory, $"{LIOSName}{fileName.GenerateValidFileName()}");
                    return EnsureRuleEnforced(definition, proposedFileName, false);
                }
                else
                {
                    return Path.Combine(definition.ParentDirectory, Constants.LocalizationReplaceDirectory, $"{LIOSName}{fileName.GenerateValidFileName()}");
                }
            }
            proposedFileName = Path.Combine(definition.ParentDirectory, $"{LIOSName}{fileName.GenerateValidFileName()}");
            return EnsureRuleEnforced(definition, proposedFileName, false);
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
            if (definition.ValueType == Parser.Common.ValueType.Variable || definition.ValueType == Parser.Common.ValueType.Namespace)
            {
                throw new ArgumentException("Invalid type.");
            }
        }

        #endregion Methods
    }
}
