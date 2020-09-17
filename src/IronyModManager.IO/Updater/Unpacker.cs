// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 09-17-2020
//
// Last Modified By : Mario
// Last Modified On : 09-17-2020
// ***********************************************************************
// <copyright file="Unpacker.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.IO.Common.Updater;
using IronyModManager.Shared;
using SharpCompress.Archives;
using SharpCompress.Readers;

namespace IronyModManager.IO.Updater
{
    /// <summary>
    /// Class Unpacker.
    /// Implements the <see cref="IronyModManager.IO.Common.Updater.IUnpacker" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Updater.IUnpacker" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class Unpacker : IUnpacker
    {
        #region Fields

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Unpacker" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public Unpacker(ILogger logger)
        {
            this.logger = logger;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Unpacks the update asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public Task<string> UnpackUpdateAsync(string path)
        {
            if (!File.Exists(path))
            {
                return Task.FromResult(string.Empty);
            }
            var extractPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
            string parseUsingReaderFactory()
            {
                using var fileStream = File.OpenRead(path);
                using var reader = ReaderFactory.Open(fileStream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        reader.WriteEntryToDirectory(extractPath, ZipExtractionOpts.GetExtractionOptions());
                    }
                }
                return extractPath;
            }

            string parseUsingArchiveFactory()
            {
                using var fileStream = File.OpenRead(path);
                using var reader = ArchiveFactory.Open(fileStream);
                foreach (var entry in reader.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(extractPath, ZipExtractionOpts.GetExtractionOptions());
                }
                return extractPath;
            }
            try
            {
                return Task.FromResult(parseUsingReaderFactory());
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Task.FromResult(parseUsingArchiveFactory());
            }
        }

        #endregion Methods
    }
}
