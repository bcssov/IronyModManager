// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using System.Collections.Generic;
using System.Linq;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Separates persisted collection membership from effective enablement of a backing installed mod.
    /// </summary>
    public class CollectionModMembership(IModService modService)
    {
        /// <summary>
        /// Returns members that must be persisted. Membership is defined by the collection projection supplied by the
        /// caller, independently from the transient selection state of its backing mod objects.
        /// </summary>
        public virtual IReadOnlyCollection<IMod> GetPersistedMembers(IEnumerable<IMod> mods)
        {
            return mods?.ToList() ?? [];
        }

        /// <summary>
        /// Restores runtime selection from persisted collection membership after authoritative resolution.
        /// Installed mods outside the collection remain unselected, while every resolved collection member retains
        /// its membership independently from installed availability.
        /// </summary>
        public virtual void RestoreSelection(IEnumerable<IMod> installedMods, IEnumerable<IMod> collectionMembers)
        {
            foreach (var mod in installedMods ?? [])
            {
                mod.IsSelected = false;
            }

            foreach (var mod in collectionMembers ?? [])
            {
                mod.IsSelected = true;
            }
        }

        /// <summary>
        /// Intentionally removes a virtual placeholder from collection membership.
        /// </summary>
        public virtual IList<IMod> RemoveVirtual(IList<IMod> mods, IMod mod)
        {
            if (mod?.IsVirtual != true)
            {
                return mods?.ToList() ?? [];
            }

            return mods?.Where(p => !modService.AreModDefinitionsEquivalent(p, mod)).ToList() ?? [];
        }

        /// <summary>
        /// Disables all real mods while retaining virtual collection members.
        /// </summary>
        public virtual IList<IMod> DisableAllRealMods(IList<IMod> mods)
        {
            foreach (var mod in mods?.Where(p => !p.IsVirtual) ?? [])
            {
                mod.IsSelected = false;
            }

            return mods?.Where(p => p.IsVirtual).ToList() ?? [];
        }
    }
}
