// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// ***********************************************************************

using System.Collections.Generic;
using IronyModManager.Models.Common;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Represents an installed-mod refresh together with its authority.
    /// </summary>
    public sealed class InstalledModsResult
    {
        /// <summary>
        /// Gets or sets whether every configured discovery source completed successfully.
        /// </summary>
        public bool IsAuthoritative { get; set; }

        /// <summary>
        /// Gets or sets the installed mods. On failure this is the last known-good value, when available.
        /// </summary>
        public IEnumerable<IMod> Mods { get; set; }
    }
}
