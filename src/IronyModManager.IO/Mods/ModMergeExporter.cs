// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 06-19-2020
//
// Last Modified By : Mario
// Last Modified On : 06-19-2020
// ***********************************************************************
// <copyright file="ModMergeExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Shared;

namespace IronyModManager.IO.Mods
{
    /// <summary>
    /// Class ModMergeExporter.
    /// Implements the <see cref="IronyModManager.IO.Common.Mods.IModMergeExporter" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Mods.IModMergeExporter" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class ModMergeExporter : IModMergeExporter
    {
        #region Fields

        /// <summary>
        /// The definition information providers
        /// </summary>
        private readonly IEnumerable<IDefinitionInfoProvider> definitionInfoProviders;

        /// <summary>
        /// The reader
        /// </summary>
        private readonly IReader reader;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModMergeExporter" /> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="definitionInfoProviders">The definition information providers.</param>
        public ModMergeExporter(IReader reader, IEnumerable<IDefinitionInfoProvider> definitionInfoProviders)
        {
            this.reader = reader;
            this.definitionInfoProviders = definitionInfoProviders;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// export definitions as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="ArgumentNullException">ExportPath.</exception>
        /// <exception cref="ArgumentNullException">Definitions.</exception>
        /// <exception cref="ArgumentNullException">ExportPath.</exception>
        /// <exception cref="ArgumentNullException">Definitions.</exception>
        public async Task<bool> ExportDefinitionsAsync(ModMergeExporterParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.ExportPath))
            {
                throw new ArgumentNullException("ExportPath.");
            }
            if (parameters.Definitions == null || parameters.Definitions.Count() == 0)
            {
                throw new ArgumentNullException("Definitions.");
            }

            var results = new List<bool>();
            if (parameters.Definitions?.Count() > 0)
            {
                results.Add(await CopyBinariesAsync(parameters.Definitions.Where(p => p.ValueType == Parser.Common.ValueType.Binary),
                    parameters.ExportPath));
                results.Add(await WriteTextContentAsync(parameters.Definitions.Where(p => p.ValueType != Parser.Common.ValueType.Binary),
                    parameters.ExportPath, parameters.Game));
            }
            return results.All(p => p);
        }

        /// <summary>
        /// Copies the binaries asynchronous.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="exportPath">The patch root path.</param>
        /// <param name="checkIfExists">The check if exists.</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.Boolean&gt;.</returns>
        private async Task<bool> CopyBinariesAsync(IEnumerable<IDefinition> definitions, string exportPath)
        {
            var tasks = new List<Task>();
            var streams = new List<Stream>();

            var retry = new RetryStrategy();

            static async Task<bool> copyStream(Stream s, FileStream fs)
            {
                await s.CopyToAsync(fs);
                return true;
            }

            foreach (var def in definitions)
            {
                var outPath = Path.Combine(exportPath, def.File);
                var stream = reader.GetStream(def.ModPath, def.File);
                if (!Directory.Exists(Path.GetDirectoryName(outPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                }
                if (File.Exists(outPath))
                {
                    File.Delete(outPath);
                }
                var fs = new FileStream(outPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
                tasks.Add(retry.RetryActionAsync(() =>
                {
                    return copyStream(stream, fs);
                }));
                streams.Add(stream);
                streams.Add(fs);
            }
            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
                foreach (var fs in streams)
                {
                    fs.Close();
                    await fs.DisposeAsync();
                }
            }
            return true;
        }

        /// <summary>
        /// Writes the text content asynchronous.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        /// <param name="exportPath">The export path.</param>
        /// <param name="game">The game.</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.Boolean&gt;.</returns>
        private async Task<bool> WriteTextContentAsync(IEnumerable<IDefinition> definitions, string exportPath, string game)
        {
            var tasks = new List<Task>();
            List<bool> results = new List<bool>();
            var retry = new RetryStrategy();

            foreach (var item in definitions)
            {
                var infoProvider = definitionInfoProviders.FirstOrDefault(p => p.CanProcess(game));
                if (infoProvider != null)
                {
                    var fileName = item.File;
                    var outPath = Path.Combine(exportPath, fileName);
                    if (File.Exists(outPath))
                    {
                        File.Delete(outPath);
                    }
                    if (!Directory.Exists(Path.GetDirectoryName(outPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                    }
                    tasks.Add(retry.RetryActionAsync(async () =>
                    {
                        await File.WriteAllTextAsync(outPath, item.Code, infoProvider.GetEncoding(item));
                        return true;
                    }));
                    results.Add(true);
                }
                else
                {
                    results.Add(false);
                }
            }
            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
            }

            return results.All(p => p);
        }

        #endregion Methods
    }
}
