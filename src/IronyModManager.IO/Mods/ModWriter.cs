// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-31-2020
//
// Last Modified By : Mario
// Last Modified On : 12-05-2020
// ***********************************************************************
// <copyright file="ModWriter.cs" company="Mario">
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
using IronyModManager.IO.Mods.Exporter;
using IronyModManager.Shared;
using Nito.AsyncEx;

namespace IronyModManager.IO.Mods
{
    /// <summary>
    /// Class ModWriter.
    /// Implements the <see cref="IronyModManager.IO.Common.Mods.IModWriter" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Mods.IModWriter" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class ModWriter : IModWriter
    {
        #region Fields

        /// <summary>
        /// The write lock
        /// </summary>
        private static readonly AsyncLock writeLock = new AsyncLock();

        /// <summary>
        /// The json exporter
        /// </summary>
        private readonly JsonExporter jsonExporter;

        /// <summary>
        /// The sqlite exporter
        /// </summary>
        private readonly SQLiteExporter sqliteExporter;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModWriter" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ModWriter(ILogger logger)
        {
            jsonExporter = new JsonExporter();
            sqliteExporter = new SQLiteExporter(logger);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// apply mods as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> ApplyModsAsync(ModWriterParameters parameters)
        {
            Task<bool>[] tasks;
            using (var mutex = await writeLock.LockAsync())
            {
                tasks = new Task<bool>[]
                {
                    Task.Run(async() => await sqliteExporter.ExportAsync(parameters)),
                    Task.Run(async() => await jsonExporter.ExportAsync(parameters))
                };
                await Task.WhenAll(tasks);
            }

            return tasks.All(p => p.Result);
        }

        /// <summary>
        /// Creates the mod directory asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> CreateModDirectoryAsync(ModWriterParameters parameters)
        {
            var fullPath = Path.Combine(parameters.RootDirectory, parameters.Path);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// Deletes the descriptor asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> DeleteDescriptorAsync(ModWriterParameters parameters)
        {
            var fullPath = Path.Combine(parameters.RootDirectory, parameters.Mod.DescriptorFile);
            if (File.Exists(fullPath))
            {
                _ = new System.IO.FileInfo(fullPath)
                {
                    IsReadOnly = false
                };
                File.Delete(fullPath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// Descriptors the exists asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> DescriptorExistsAsync(ModWriterParameters parameters)
        {
            var fullPath = Path.Combine(parameters.RootDirectory, parameters.Mod.DescriptorFile);
            if (File.Exists(fullPath))
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// Mods the directory exists asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> ModDirectoryExistsAsync(ModWriterParameters parameters)
        {
            var fullPath = Path.Combine(parameters.RootDirectory, parameters.Path);
            if (!Directory.Exists(fullPath))
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(Directory.EnumerateFileSystemEntries(fullPath).Any());
        }

        /// <summary>
        /// Purges the mod directory asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="deleteAll">if set to <c>true</c> [delete all].</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> PurgeModDirectoryAsync(ModWriterParameters parameters, bool deleteAll = false)
        {
            var fullPath = Path.Combine(parameters.RootDirectory ?? string.Empty, parameters.Path ?? string.Empty);
            if (Directory.Exists(fullPath))
            {
                if (!deleteAll)
                {
                    var files = Directory.EnumerateFiles(fullPath, "*", SearchOption.TopDirectoryOnly);
                    foreach (var item in files)
                    {
                        File.Delete(item);
                    }
                }
                else
                {
                    Directory.Delete(fullPath, true);
                }
                return Task.FromResult(true);
            }
            else if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// Sets the descriptor lock asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="isLocked">if set to <c>true</c> [is locked].</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> SetDescriptorLockAsync(ModWriterParameters parameters, bool isLocked)
        {
            var fullPath = Path.Combine(parameters.RootDirectory, parameters.Mod.DescriptorFile);
            if (File.Exists(fullPath))
            {
                _ = new System.IO.FileInfo(fullPath)
                {
                    IsReadOnly = isLocked
                };
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// write descriptor as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="writeDescriptorInModDirectory">if set to <c>true</c> [write descriptor in mod directory].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public async Task<bool> WriteDescriptorAsync(ModWriterParameters parameters, bool writeDescriptorInModDirectory)
        {
            async Task<bool> writeDescriptors()
            {
                // If needed I've got a much more complex serializer, it is written for Kerbal Space Program but the structure seems to be the same though this is much more simpler
                var fullPath = Path.Combine(parameters.RootDirectory ?? string.Empty, parameters.Path ?? string.Empty);
                await writeDescriptor(fullPath);
                // Attempt to fix issues where the game decides to delete local zipped mod descriptors (I'm assuming this happens to all pdx games)
                if (parameters.LockDescriptor)
                {
                    if (File.Exists(fullPath))
                    {
                        _ = new System.IO.FileInfo(fullPath)
                        {
                            IsReadOnly = true
                        };
                    }
                }
                if (writeDescriptorInModDirectory)
                {
                    var modPath = Path.Combine(parameters.Mod.FileName, Shared.Constants.DescriptorFile);
                    await writeDescriptor(modPath);
                }
                return true;
            }
            async Task<bool> writeDescriptor(string fullPath)
            {
                bool? state = null;
                if (File.Exists(fullPath))
                {
                    var fileInfo = new System.IO.FileInfo(fullPath);
                    state = fileInfo.IsReadOnly;
                    fileInfo.IsReadOnly = false;
                }
                using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                var result = await WriteDescriptorToStreamAsync(parameters, fs);
                if (state.HasValue)
                {
                    var fileInfo = new System.IO.FileInfo(fullPath);
                    fileInfo.IsReadOnly = state.GetValueOrDefault();
                }
                return result;
            }

            var retry = new RetryStrategy();
            return await retry.RetryActionAsync(writeDescriptors);
        }

        /// <summary>
        /// write descriptor to stream as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> WriteDescriptorToStreamAsync(ModWriterParameters parameters, Stream stream)
        {
            using var sw = new StreamWriter(stream, leaveOpen: true);
            var props = parameters.Mod.GetType().GetProperties().Where(p => Attribute.IsDefined(p, typeof(DescriptorPropertyAttribute)));
            foreach (var prop in props)
            {
                var attr = Attribute.GetCustomAttribute(prop, typeof(DescriptorPropertyAttribute), true) as DescriptorPropertyAttribute;
                var val = prop.GetValue(parameters.Mod, null);
                if (val is IEnumerable<string> col)
                {
                    if (col.Any())
                    {
                        if (attr.KeyedArray)
                        {
                            foreach (var item in col)
                            {
                                await sw.WriteLineAsync($"{attr.PropertyName}=\"{item}\"");
                            }
                        }
                        else
                        {
                            await sw.WriteLineAsync($"{attr.PropertyName}={{");
                            foreach (var item in col)
                            {
                                await sw.WriteLineAsync($"\t\"{item}\"");
                            }
                            await sw.WriteLineAsync("}");
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(val != null ? val.ToString() : string.Empty))
                    {
                        if (attr.AlternateNameEndsWithCondition?.Count() > 0 && attr.AlternateNameEndsWithCondition.Any(p => val.ToString().EndsWith(p, StringComparison.OrdinalIgnoreCase)))
                        {
                            await sw.WriteLineAsync($"{attr.AlternatePropertyName}=\"{val}\"");
                        }
                        else
                        {
                            await sw.WriteLineAsync($"{attr.PropertyName}=\"{val}\"");
                        }
                    }
                }
            }
            await sw.FlushAsync();
            return true;
        }

        #endregion Methods
    }
}
