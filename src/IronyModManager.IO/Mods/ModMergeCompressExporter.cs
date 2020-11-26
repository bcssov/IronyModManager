// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 11-26-2020
//
// Last Modified By : Mario
// Last Modified On : 11-26-2020
// ***********************************************************************
// <copyright file="ModMergeCompressExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Concurrent;
using IronyModManager.IO.Common.Mods;
using IronyModManager.Shared;
using SharpCompress.Archives;
using SharpCompress.Common;

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
        private readonly ConcurrentDictionary<long, IWritableArchive> queue;

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
            queue = new ConcurrentDictionary<long, IWritableArchive>();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <exception cref="NullReferenceException">parameters</exception>
        public void AddFile(ModMergeCompressExporterParameters parameters)
        {
            if (parameters == null)
            {
                throw new NullReferenceException("parameters");
            }
            queue.TryGetValue(parameters.QueueId, out var value);
            value.AddEntry(parameters.FileName, parameters.Stream, true);
        }

        /// <summary>
        /// Finalizes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Finalize(long id)
        {
            if (queue.TryRemove(id, out var value))
            {
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
            var zip = ArchiveFactory.Create(ArchiveType.Zip);
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
