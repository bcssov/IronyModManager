// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using IronyModManager.Implementation.AppState;
using IronyModManager.Implementation.MessageBus;
using IronyModManager.Services.Common;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Composes the separate collaborators owned by one Installed Mods control.
    /// </summary>
    public class InstalledModsCoordinatorFactory(IAppStateService appStateService,
        SortOrderControlViewModel modSelectedSortOrder, SortOrderControlViewModel modNameSortOrder,
        SortOrderControlViewModel modVersionSortOrder, SearchModsControlViewModel filterMods,
        EvalModAchievementCompatibilityHandler achievementHandler) : IInstalledModsCoordinatorFactory
    {
        /// <inheritdoc />
        public virtual InstalledModsCoordinators Create() => new(
            new InstalledModsPresentationCoordinator(appStateService, modSelectedSortOrder, modNameSortOrder,
                modVersionSortOrder, filterMods),
            new InstalledModsEventCoordinator(achievementHandler));
    }
}
