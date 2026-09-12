// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// ***********************************************************************

using System.Collections.Generic;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Describes whether Merge Compress may begin without overwriting an unavailable archive.
    /// </summary>
    public class MergeCompressPreflightResult
    {
        /// <summary>
        /// Gets or sets the unavailable archive display names.
        /// </summary>
        public IReadOnlyCollection<string> UnavailableArchiveNames { get; set; } = [];

        /// <summary>
        /// Gets a value indicating whether Merge Compress may proceed.
        /// </summary>
        public bool CanProceed => UnavailableArchiveNames.Count == 0;
    }
}
