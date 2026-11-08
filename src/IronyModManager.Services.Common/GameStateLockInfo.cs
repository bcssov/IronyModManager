// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// ***********************************************************************

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Describes the first filesystem safety lock observed for a game.
    /// </summary>
    public sealed class GameStateLockInfo
    {
        /// <summary>
        /// Gets or sets the diagnostic context.
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Gets or sets the exception type, when applicable.
        /// </summary>
        public string ExceptionType { get; set; }

        /// <summary>
        /// Gets or sets the stable game key.
        /// </summary>
        public string GameType { get; set; }

        /// <summary>
        /// Gets or sets the lock reason.
        /// </summary>
        public GameStateLockReason Reason { get; set; }
    }
}
