// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using System.Collections.Concurrent;
using System.Collections.Generic;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Owns collection resolution policy while installed-mod state is authoritative.
    /// </summary>
    public class CollectionModReconciliationCoordinator(IModService modService, IGameStateSafetyService gameStateSafetyService)
    {
        private readonly MissingModNotificationPolicy missingModNotificationPolicy = new();
        private readonly ConcurrentDictionary<string, IEnumerable<IMod>> previousCollectionMods = new();

        /// <summary>
        /// Gets whether collection reconciliation may use the current installed-mod projection.
        /// </summary>
        public virtual bool CanReconcile(IGame game)
        {
            return !gameStateSafetyService.IsLocked(game);
        }

        /// <summary>
        /// Resolves persisted collection members only when the filesystem state is authoritative.
        /// </summary>
        public virtual bool TryResolve(IGame game, IEnumerable<IMod> installedMods, IModCollection collection, out IReadOnlyCollection<IMod> resolvedMods)
        {
            resolvedMods = [];
            if (!CanReconcile(game))
            {
                return false;
            }

            previousCollectionMods.TryGetValue(collection.Name, out var previousMods);
            resolvedMods = modService.ResolveCollectionMods(installedMods, collection, previousMods);
            return true;
        }

        /// <summary>
        /// Gives the import-specific warning ownership when its refresh is authoritative.
        /// </summary>
        public virtual void BeginImport(IGame game, string collectionName)
        {
            if (CanReconcile(game))
            {
                missingModNotificationPolicy.BeginImport(collectionName);
            }
        }

        /// <summary>
        /// Gets whether an ordinary missing-mod warning should be shown.
        /// </summary>
        public virtual bool ShouldShowOrdinaryWarning(string activeGameType, string collectionGameType,
            string collectionName, IEnumerable<string> missingModIdentities)
        {
            return missingModNotificationPolicy.ShouldShowOrdinaryWarning(activeGameType, collectionGameType,
                collectionName, missingModIdentities);
        }

        /// <summary>
        /// Gets the last resolved collection projection used for identity preservation and undo state.
        /// </summary>
        public virtual IEnumerable<IMod> GetPreviousCollectionMods(string collectionName)
        {
            previousCollectionMods.TryGetValue(collectionName, out var mods);
            return mods;
        }

        /// <summary>
        /// Records the last resolved collection projection.
        /// </summary>
        public virtual void RecordCollectionMods(string collectionName, IEnumerable<IMod> mods)
        {
            previousCollectionMods.AddOrUpdate(collectionName, mods, (_, _) => mods);
        }

        /// <summary>
        /// Clears prior in-memory collection projections after a full reset.
        /// </summary>
        public virtual void Reset()
        {
            previousCollectionMods.Clear();
        }
    }
}
