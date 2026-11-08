// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Owns the ordering boundary between installed-mod refresh and collection reconciliation.
    /// </summary>
    public class ModStateReconciliationCoordinator(IGameStateSafetyService gameStateSafetyService)
    {
        /// <summary>
        /// Gets whether the specified game's mod state is locked.
        /// </summary>
        public virtual bool IsLocked(IGame game)
        {
            return gameStateSafetyService.IsLocked(game);
        }

        /// <summary>
        /// Subscribes to process-lifetime game lock transitions while leaving subscription lifetime to the caller.
        /// </summary>
        public virtual IDisposable SubscribeToGameLocks(Action<GameStateLockInfo> onGameLocked)
        {
            gameStateSafetyService.GameLocked += onGameLocked;
            return Disposable.Create(() => gameStateSafetyService.GameLocked -= onGameLocked);
        }

        /// <summary>
        /// Gets whether an explicit filesystem configuration change is being revalidated.
        /// </summary>
        public virtual bool IsRevalidating { get; protected set; }

        /// <summary>
        /// Reconciles a direct Mods publication only when it is independently authoritative.
        /// Refresh-driven publications wait for refresh completion instead.
        /// </summary>
        public virtual bool ReconcileModsPublication(IGame game, bool refreshing, bool authoritative, Action reconcile)
        {
            if (IsRevalidating || refreshing || !authoritative || gameStateSafetyService.IsLocked(game))
            {
                return false;
            }

            reconcile();
            return true;
        }

        /// <summary>
        /// Reconciles exactly once when an authoritative refresh completes.
        /// </summary>
        public virtual bool ReconcileRefreshPublication(IGame game, bool refreshing, bool authoritative, Action reconcile)
        {
            if (IsRevalidating || refreshing || !authoritative || gameStateSafetyService.IsLocked(game))
            {
                return false;
            }

            reconcile();
            return true;
        }

        /// <summary>
        /// Keeps the game locked while changed roots are scanned, an authoritative result is installed,
        /// and collection state is reconciled from that result.
        /// </summary>
        public virtual async Task<bool> RevalidateConfigurationAsync(IGame game,
            Func<GameStateLockInfo, Func<bool>, Task<bool>> refresh, Action reset,
            Action<GameStateLockInfo> reconcile,
            Func<GameStateLockInfo, Func<bool>, Task<bool>> synchronize = null)
        {
            var revalidationLock = gameStateSafetyService.BeginRevalidation(game, "Filesystem configuration changed");
            IsRevalidating = true;
            try
            {
                bool isCurrent() => gameStateSafetyService.IsCurrentRevalidation(game, revalidationLock);
                if (synchronize != null && (!await synchronize(revalidationLock, isCurrent) || !isCurrent()))
                {
                    return false;
                }

                if (!await refresh(revalidationLock, isCurrent) || !isCurrent())
                {
                    return false;
                }

                if (!gameStateSafetyService.CompleteRevalidation(game, revalidationLock, () =>
                    {
                        reset?.Invoke();
                        reconcile(revalidationLock);
                    }))
                {
                    return false;
                }

                return true;
            }
            finally
            {
                IsRevalidating = false;
            }
        }
    }
}
