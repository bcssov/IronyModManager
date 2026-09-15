// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// ***********************************************************************
using System;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Defines independently composable standardized invocation safeguards.
    /// </summary>
    [Flags]
    public enum GameStateSafetyEnforcement
    {
        /// <summary>
        /// No standardized invocation safeguard.
        /// </summary>
        None = 0,

        /// <summary>
        /// Reject invocation when the captured game is locked.
        /// </summary>
        RejectWhenLocked = 1,

        /// <summary>
        /// Reject invocation when no game can be captured.
        /// </summary>
        RejectWhenGameUnavailable = 2,

        /// <summary>
        /// Convert a classified filesystem failure into a game lock and declared rejection result.
        /// </summary>
        LockOnFileSystemFailure = 4,

        /// <summary>
        /// Rejects invocations that have no game or whose game is already locked, but leaves
        /// filesystem-failure classification to the operation.
        /// </summary>
        EntryOnly = RejectWhenLocked | RejectWhenGameUnavailable,

        /// <summary>
        /// Apply all standardized safeguards.
        /// </summary>
        Standard = RejectWhenLocked | RejectWhenGameUnavailable | LockOnFileSystemFailure
    }
}
