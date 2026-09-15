// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// ***********************************************************************

using System;
using System.Threading.Tasks;
using IronyModManager.Models.Common;

namespace IronyModManager.Services.Common
{
    /// <summary>
        /// Owns process-lifetime filesystem safety state per game.
    /// </summary>
    public interface IGameStateSafetyService
    {
        /// <summary>
        /// Occurs once when a game first enters locked state.
        /// </summary>
        event Action<GameStateLockInfo> GameLocked;

        /// <summary>
        /// Begins controlled revalidation after an explicit filesystem configuration change.
        /// The returned lock must be supplied when completing successful revalidation.
        /// </summary>
        GameStateLockInfo BeginRevalidation(IGame game, string context);

        /// <summary>
        /// Returns whether the supplied lock still owns the current configuration revalidation.
        /// </summary>
        bool IsCurrentRevalidation(IGame game, GameStateLockInfo revalidationLock);

        /// <summary>
        /// Clears the exact lock observed when controlled revalidation began, but only after
        /// the caller has produced and installed an authoritative result.
        /// </summary>
        bool CompleteRevalidation(IGame game, GameStateLockInfo revalidationLock, Action beforeUnlock = null);

        /// <summary>
        /// Replaces the exact current provisional lock with a classified revalidation failure.
        /// A stale revalidation cannot change a newer lock or its retained reason.
        /// </summary>
        bool LockRevalidationFailure(IGame game, GameStateLockInfo revalidationLock, GameStateLockReason reason,
            string context, Exception exception = null);

        /// <summary>
        /// Executes a filesystem mutation unless the game is locked and converts expected access failures into a lock.
        /// </summary>
        Task<T> ExecuteMutationAsync<T>(IGame game, Func<Task<T>> mutation, T rejectedResult, string context);

        /// <summary>
        /// Executes a filesystem-dependent read unless the game is locked and converts expected access failures into a discovery lock.
        /// </summary>
        Task<T> ExecuteReadAsync<T>(IGame game, Func<Task<T>> read, T rejectedResult, string context);

        /// <summary>
        /// Gets the retained first lock reason for a game.
        /// </summary>
        GameStateLockInfo GetLock(IGame game);

        /// <summary>
        /// Returns whether a game is locked.
        /// </summary>
        bool IsLocked(IGame game);

        /// <summary>
        /// Converts a classified filesystem access failure into the specified game lock.
        /// Returns false without changing state when the exception is not a filesystem trust failure.
        /// </summary>
        bool LockIfFileSystemAccessFailure(IGame game, GameStateLockReason reason, string context, Exception exception);

        /// <summary>
        /// Locks a game for the remainder of the process. The first failure reason is retained.
        /// A provisional configuration-change lock may be replaced by the revalidation failure.
        /// </summary>
        bool Lock(IGame game, GameStateLockReason reason, string context, Exception exception = null);
    }
}
