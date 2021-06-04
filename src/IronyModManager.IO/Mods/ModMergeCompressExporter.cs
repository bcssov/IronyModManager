// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 11-26-2020
//
// Last Modified By : Mario
// Last Modified On : 05-31-2021
// ***********************************************************************
// <copyright file="ModMergeCompressExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Ionic.Zip;
using IronyModManager.IO.Common.Mods;
using IronyModManager.Shared;

namespace IronyModManager.IO.Mods
{
    /// <summary>
    /// Class ModMergeCompressExporter.
    /// Implements the <see cref="IronyModManager.IO.Common.Mods.IModMergeCompressExporter" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Mods.IModMergeCompressExporter" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class ModMergeCompressExporter : IModMergeCompressExporter
    {
        #region Fields

        /// <summary>
        /// The object lock
        /// </summary>
        private static readonly object objectLock = new { };

        /// <summary>
        /// The queue
        /// </summary>
        private readonly ConcurrentDictionary<long, ZipFile> queue;

        /// <summary>
        /// The identifier
        /// </summary>
        private long id = 0;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModMergeCompressExporter" /> class.
        /// </summary>
        public ModMergeCompressExporter()
        {
            queue = new ConcurrentDictionary<long, ZipFile>();
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when [processed file].
        /// </summary>
        public event EventHandler ProcessedFile;

        #endregion Events

        #region Methods

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <exception cref="ArgumentNullException">parameters</exception>
        public void AddFile(ModMergeCompressExporterParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            queue.TryGetValue(parameters.QueueId, out var value);
            value.AddEntry(parameters.FileName, parameters.Stream);
        }

        /// <summary>
        /// Finalizes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="exportPath">The export path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Finalize(long id, string exportPath)
        {
            void saveProgress(object sender, SaveProgressEventArgs e)
            {
                switch (e.EventType)
                {
                    case ZipProgressEventType.Saving_AfterWriteEntry:
                        if (ProcessedFile != null)
                        {
                            ProcessedFile?.Invoke(this, EventArgs.Empty);
                        }
                        break;

                    default:
                        break;
                }
            }

            if (queue.TryRemove(id, out var value))
            {
                value.SaveProgress += saveProgress;
                value.Save(exportPath);
                value.SaveProgress -= saveProgress;
                value.Dispose();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns>IArchive.</returns>
        public long Start()
        {
            var zip = new ZipFile();
            lock (objectLock)
            {
                id++;
                queue.TryAdd(id, zip);
            }
            return id;
        }

        #endregion Methods
    }
}
