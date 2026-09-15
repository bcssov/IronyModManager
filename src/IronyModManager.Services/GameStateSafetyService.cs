// ***********************************************************************
// Assembly         : IronyModManager.Services
// ***********************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using IronyModManager.IO.Common.FileSystem;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;

namespace IronyModManager.Services
{
    /// <summary>
    /// Process-lifetime source of truth for per-game filesystem safety locks.
    /// </summary>
    public class GameStateSafetyService(IFileSystemStateProbe fileSystemStateProbe, ILogger logger) : IGameStateSafetyService
    {
        private readonly ConcurrentDictionary<string, GameStateLockInfo> locks = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, object> revalidationGates = new(StringComparer.OrdinalIgnoreCase);

        /// <inheritdoc />
        public event Action<GameStateLockInfo> GameLocked;

        /// <inheritdoc />
        public virtual GameStateLockInfo BeginRevalidation(IGame game, string context)
        {
            if (string.IsNullOrWhiteSpace(game?.Type))
            {
                return null;
            }

            var provisional = CreateLockInfo(game, GameStateLockReason.ConfigurationChanged, context, null);
            lock (GetGate(game.Type))
            {
                locks[game.Type] = provisional;
            }

            return provisional;
        }

        /// <inheritdoc />
        public virtual bool IsCurrentRevalidation(IGame game, GameStateLockInfo revalidationLock)
        {
            if (string.IsNullOrWhiteSpace(game?.Type) || revalidationLock == null)
            {
                return false;
            }

            lock (GetGate(game.Type))
            {
                return IsCurrentRevalidationUnsafe(game.Type, revalidationLock);
            }
        }

        /// <inheritdoc />
        public virtual bool CompleteRevalidation(IGame game, GameStateLockInfo revalidationLock, Action beforeUnlock = null)
        {
            if (string.IsNullOrWhiteSpace(game?.Type) || revalidationLock == null)
            {
                return false;
            }

            lock (GetGate(game.Type))
            {
                if (!IsCurrentRevalidationUnsafe(game.Type, revalidationLock))
                {
                    return false;
                }

                beforeUnlock?.Invoke();
                return ((ICollection<KeyValuePair<string, GameStateLockInfo>>)locks)
                    .Remove(new KeyValuePair<string, GameStateLockInfo>(game.Type, revalidationLock));
            }
        }

        /// <inheritdoc />
        public virtual bool LockRevalidationFailure(IGame game, GameStateLockInfo revalidationLock,
            GameStateLockReason reason, string context, Exception exception = null)
        {
            if (string.IsNullOrWhiteSpace(game?.Type) || revalidationLock == null)
            {
                return false;
            }

            var info = CreateLockInfo(game, reason, context, exception);
            lock (GetGate(game.Type))
            {
                if (!IsCurrentRevalidationUnsafe(game.Type, revalidationLock) ||
                    !locks.TryUpdate(game.Type, info, revalidationLock))
                {
                    return false;
                }
            }

            NotifyLocked(info, exception);
            return true;
        }

        /// <inheritdoc />
        public virtual async Task<T> ExecuteMutationAsync<T>(IGame game, Func<Task<T>> mutation, T rejectedResult, string context)
        {
            if (game == null || IsLocked(game))
            {
                return rejectedResult;
            }

            try
            {
                return await mutation();
            }
            catch (Exception exception) when (fileSystemStateProbe.IsFileSystemAccessFailure(exception))
            {
                Lock(game, GameStateLockReason.WriteAccessFailure, context, exception);
                return rejectedResult;
            }
        }

        /// <inheritdoc />
        public virtual async Task<T> ExecuteReadAsync<T>(IGame game, Func<Task<T>> read, T rejectedResult, string context)
        {
            if (game == null || IsLocked(game))
            {
                return rejectedResult;
            }

            try
            {
                return await read();
            }
            catch (Exception exception) when (fileSystemStateProbe.IsFileSystemAccessFailure(exception))
            {
                Lock(game, GameStateLockReason.DiscoveryUnavailable, context, exception);
                return rejectedResult;
            }
        }

        /// <inheritdoc />
        public virtual GameStateLockInfo GetLock(IGame game)
        {
            if (string.IsNullOrWhiteSpace(game?.Type))
            {
                return null;
            }

            locks.TryGetValue(game.Type, out var result);
            return result;
        }

        /// <inheritdoc />
        public virtual bool IsLocked(IGame game)
        {
            return GetLock(game) != null;
        }

        /// <inheritdoc />
        public virtual bool LockIfFileSystemAccessFailure(IGame game, GameStateLockReason reason, string context,
            Exception exception)
        {
            if (!fileSystemStateProbe.IsFileSystemAccessFailure(exception))
            {
                return false;
            }

            Lock(game, reason, context, exception);
            return true;
        }

        /// <inheritdoc />
        public virtual bool Lock(IGame game, GameStateLockReason reason, string context, Exception exception = null)
        {
            if (string.IsNullOrWhiteSpace(game?.Type))
            {
                return false;
            }

            var info = CreateLockInfo(game, reason, context, exception);
            lock (GetGate(game.Type))
            {
                if (locks.TryGetValue(game.Type, out var existing))
                {
                    if (existing.Reason != GameStateLockReason.ConfigurationChanged)
                    {
                        return false;
                    }

                    locks[game.Type] = info;
                }
                else
                {
                    locks[game.Type] = info;
                }
            }

            NotifyLocked(info, exception);
            return true;
        }

        private object GetGate(string gameType)
        {
            return revalidationGates.GetOrAdd(gameType, _ => new object());
        }

        private bool IsCurrentRevalidationUnsafe(string gameType, GameStateLockInfo revalidationLock)
        {
            return locks.TryGetValue(gameType, out var current) && ReferenceEquals(current, revalidationLock) &&
                   current.Reason == GameStateLockReason.ConfigurationChanged;
        }

        private void NotifyLocked(GameStateLockInfo info, Exception exception)
        {
            if (exception != null)
            {
                logger.Error(exception);
            }

            GameLocked?.Invoke(info);
        }

        private static GameStateLockInfo CreateLockInfo(IGame game, GameStateLockReason reason, string context, Exception exception)
        {
            return new GameStateLockInfo
            {
                Context = context ?? string.Empty,
                ExceptionType = exception?.GetType().FullName ?? string.Empty,
                GameType = game.Type,
                Reason = reason
            };
        }
    }
}
