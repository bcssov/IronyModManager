// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// ***********************************************************************

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Identifies why a game's mod state became non-authoritative.
    /// </summary>
    public enum GameStateLockReason
    {
        /// <summary>
        /// An explicit filesystem configuration change is being authoritatively revalidated.
        /// </summary>
        ConfigurationChanged,

        /// <summary>
        /// A read or discovery source became unavailable.
        /// </summary>
        DiscoveryUnavailable,

        /// <summary>
        /// A configured, authoritative discovery source was missing.
        /// </summary>
        ExpectedSourceMissing,

        /// <summary>
        /// A filesystem write or access operation failed.
        /// </summary>
        WriteAccessFailure
    }
}
