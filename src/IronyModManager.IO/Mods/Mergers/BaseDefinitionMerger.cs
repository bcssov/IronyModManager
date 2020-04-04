// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 04-04-2020
//
// Last Modified By : Mario
// Last Modified On : 04-04-2020
// ***********************************************************************
// <copyright file="BaseDefinitionMerger.cs" company="Mario">
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

namespace IronyModManager.IO.Mods.Mergers
{
    /// <summary>
    /// Class BaseDefinitionMerger.
    /// Implements the <see cref="IronyModManager.IO.Common.Mods.IDefinitionMerger" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Mods.IDefinitionMerger" />
    public abstract class BaseDefinitionMerger : IDefinitionMerger
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
        /// <param name="definitions">The definitions.</param>
        /// <returns>Encoding.</returns>
        public virtual Encoding GetEncoding(IEnumerable<IDefinition> definitions)
        {
            EnsureAllSameType(definitions);
            var definition = definitions.FirstOrDefault(p => p.ValueType != Parser.Common.ValueType.Namespace && p.ValueType != Parser.Common.ValueType.Variable);
            if (!definition.ParentDirectory.StartsWith(Localization))
            {
                return new UTF8Encoding(false);
            }
            return new UTF8Encoding(true);
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <returns>System.String.</returns>
        public virtual string GetFileName(IEnumerable<IDefinition> definitions)
        {
            EnsureAllSameType(definitions);
            var definition = definitions.FirstOrDefault(p => p.ValueType != Parser.Common.ValueType.Namespace && p.ValueType != Parser.Common.ValueType.Variable);
            var fileName = definition.ValueType == Parser.Common.ValueType.WholeTextFile ? Path.GetFileName(definition.File) : $"{definition.Id}.{Path.GetExtension(definition.File)}";
            if (FIOSPaths.Any(p => p.EndsWith(definition.ParentDirectory, StringComparison.OrdinalIgnoreCase)))
            {
                return Path.Combine(definition.ParentDirectory, $"{FIOSName}{CleanFileName(fileName)}");
            }
            else if (definition.ParentDirectory.EndsWith(Localization, StringComparison.OrdinalIgnoreCase))
            {
                return Path.Combine(definition.ParentDirectory, LocalizationReplace, CleanFileName(fileName));
            }
            return Path.Combine(definition.ParentDirectory, $"{LIOSName}{CleanFileName(fileName)}");
        }

        /// <summary>
        /// Merges the content.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <returns>System.String.</returns>
        public virtual string MergeContent(IEnumerable<IDefinition> definitions)
        {
            EnsureAllSameType(definitions);
            var namespaces = definitions.Where(p => p.ValueType == Parser.Common.ValueType.Namespace);
            var variables = definitions.Where(p => p.ValueType == Parser.Common.ValueType.Variable);
            var other = definitions.Where(p => p.ValueType != Parser.Common.ValueType.Namespace && p.ValueType != Parser.Common.ValueType.Namespace);
            StringBuilder sb = new StringBuilder();
            AppendLine(sb, namespaces);
            AppendLine(sb, variables);
            AppendLine(sb, other);
            return string.Empty;
        }

        /// <summary>
        /// Appends the line.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="lines">The lines.</param>
        protected virtual void AppendLine(StringBuilder sb, IEnumerable<IDefinition> lines)
        {
            if (lines?.Count() > 0)
            {
                sb.AppendJoin(Environment.NewLine, lines.Select(p => p.Code));
            }
        }

        /// <summary>
        /// Cleans the name of the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>System.String.</returns>
        protected virtual string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, character) => current.Replace(character.ToString(), string.Empty));
        }

        /// <summary>
        /// Ensures the type of all same.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <exception cref="ArgumentException">Not same types</exception>
        protected virtual void EnsureAllSameType(IEnumerable<IDefinition> definitions)
        {
            var toVerify = definitions.Where(p => p.ValueType != Parser.Common.ValueType.Variable && p.ValueType != Parser.Common.ValueType.Namespace);
            if (toVerify.GroupBy(p => p.Type).Count() > 1)
            {
                throw new ArgumentException("Not same types");
            }
        }

        #endregion Methods
    }
}
