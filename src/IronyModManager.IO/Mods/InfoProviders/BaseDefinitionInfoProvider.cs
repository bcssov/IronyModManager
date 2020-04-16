// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 04-04-2020
//
// Last Modified By : Mario
// Last Modified On : 04-16-2020
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

        /// <summary>
        /// The localization
        /// </summary>
        protected const string Localization = "localisation";

        /// <summary>
        /// The localization replace
        /// </summary>
        protected const string LocalizationReplace = "replace";

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
        /// Gets the encoding.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>Encoding.</returns>
        public virtual Encoding GetEncoding(IDefinition definition)
        {
            EnsureValidType(definition);
            if (!definition.ParentDirectory.StartsWith(Localization))
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
            if (FIOSPaths.Any(p => p.EndsWith(definition.ParentDirectory, StringComparison.OrdinalIgnoreCase)))
            {
                return Path.Combine(definition.ParentDirectory, $"{FIOSName}{fileName.GenerateValidFileName()}");
            }
            else if (definition.ParentDirectory.StartsWith(Localization, StringComparison.OrdinalIgnoreCase))
            {
                if (definition.ParentDirectory.Contains(LocalizationReplace, StringComparison.OrdinalIgnoreCase))
                {
                    return Path.Combine(definition.ParentDirectory, fileName.GenerateValidFileName());
                }
                else
                {
                    return Path.Combine(definition.ParentDirectory, LocalizationReplace, fileName.GenerateValidFileName());
                }
            }
            return Path.Combine(definition.ParentDirectory, $"{LIOSName}{fileName.GenerateValidFileName()}");
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
