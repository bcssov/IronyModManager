// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// ***********************************************************************

using System;
using System.Collections.Generic;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// The Merge Compress output archive cannot be overwritten.
    /// </summary>
    public class MergeCompressArchiveUnavailableException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergeCompressArchiveUnavailableException"/> class.
        /// </summary>
        /// <param name="archiveNames">The unavailable archive display names.</param>
        public MergeCompressArchiveUnavailableException(IReadOnlyCollection<string> archiveNames)
            : base($"Merge Compress archive is unavailable: {string.Join(", ", archiveNames ?? [])}")
        {
            ArchiveNames = archiveNames ?? [];
        }

        /// <summary>
        /// Gets the unavailable archive display names.
        /// </summary>
        public IReadOnlyCollection<string> ArchiveNames { get; }
    }
}
