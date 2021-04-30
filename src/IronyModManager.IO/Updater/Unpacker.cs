// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 09-17-2020
//
// Last Modified By : Mario
// Last Modified On : 04-30-2021
// ***********************************************************************
// <copyright file="Unpacker.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.IO.Common;
using IronyModManager.IO.Common.MessageBus;
using IronyModManager.IO.Common.Updater;
using IronyModManager.Shared;
using IronyModManager.Shared.MessageBus;
using SharpCompress.Archives;

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
        /// The message bus
        /// </summary>
        private readonly IMessageBus messageBus;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Unpacker" /> class.
        /// </summary>
        /// <param name="messageBus">The message bus.</param>
        public Unpacker(IMessageBus messageBus)
        {
            this.messageBus = messageBus;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Unpacks the update asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public async Task<string> UnpackUpdateAsync(string path)
        {
            if (!File.Exists(path))
            {
                return string.Empty;
            }
            var extractPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
            // Cleanup path were extracting to
            if (Directory.Exists(extractPath))
            {
                DiskOperations.DeleteDirectory(extractPath, true);
            }
            using var fileStream = File.OpenRead(path);
            using var reader = ArchiveFactory.Open(fileStream);
            var all = reader.Entries.Where(entry => !entry.IsDirectory);
            double total = all.Count();
            double processed = 0;
            var lastPercentage = 0;
            foreach (var entry in all)
            {
                entry.WriteToDirectory(extractPath, ZipExtractionOpts.GetExtractionOptions());
                processed++;
                var progress = GetProgressPercentage(total, processed);
                if (progress != lastPercentage)
                {
                    await messageBus.PublishAsync(new UpdateUnpackProgressEvent(progress));
                }
                lastPercentage = progress;
            }
            return extractPath;
        }

        /// <summary>
        /// Gets the progress percentage.
        /// </summary>
        /// <param name="total">The total.</param>
        /// <param name="processed">The processed.</param>
        /// <param name="maxPerc">The maximum perc.</param>
        /// <returns>System.Int32.</returns>
        protected virtual int GetProgressPercentage(double total, double processed, int maxPerc = 100)
        {
            var perc = Convert.ToInt32(processed / total * 100);
            if (perc < 0)
            {
                perc = 0;
            }
            else if (perc > maxPerc)
            {
                perc = maxPerc;
            }
            return perc;
        }

        #endregion Methods
    }
}
