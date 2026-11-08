// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using IronyModManager.Implementation.AppState;
using IronyModManager.Implementation.Hotkey;
using IronyModManager.Implementation.MessageBus;
using IronyModManager.Implementation.MessageBus.Events;
using IronyModManager.Localization;
using IronyModManager.Services.Common;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Composes the separate collaborators owned by one Collection Mods control.
    /// </summary>
    public class CollectionModsCoordinatorFactory(IModService modService, IGameStateSafetyService gameStateSafetyService,
        ModExportProgressHandler modExportProgressHandler, PatchModRenameProgressHandler patchModRenameProgressHandler,
        ModReportExportHandler modReportExportHandler, ILocalizationManager localizationManager,
        IAppStateService appStateService, IScrollState scrollState, SearchModsControlViewModel searchMods,
        SortOrderControlViewModel modNameSortOrder, ModCollectionChangeRequestHandler collectionChangeHandler,
        MainViewHotkeyPressedHandler hotkeyPressedHandler) : ICollectionModsCoordinatorFactory
    {
        /// <inheritdoc />
        public virtual CollectionModsCoordinators Create()
        {
            return new CollectionModsCoordinators(
                new CollectionModMembership(modService),
                new CollectionModReconciliationCoordinator(modService, gameStateSafetyService),
                new CollectionOperationProgressCoordinator(modExportProgressHandler, patchModRenameProgressHandler,
                    modReportExportHandler, localizationManager),
                new CollectionModsPresentationCoordinator(appStateService, scrollState, searchMods, modNameSortOrder),
                new CollectionModsEventCoordinator(collectionChangeHandler, hotkeyPressedHandler));
        }
    }
}
