// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.MessageBus;
using IronyModManager.Localization;
using IronyModManager.Services.Common;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Composes the separate collaborators owned by one Mod Holder control.
    /// </summary>
    public class ModHolderCoordinatorFactory(IGameStateSafetyService gameStateSafetyService,
        ModDefinitionLoadHandler modDefinitionLoadHandler,
        ModDefinitionInvalidReplaceHandler modDefinitionInvalidReplaceHandler,
        GameIndexProgressHandler gameIndexProgressHandler, GameDefinitionLoadProgressHandler gameDefinitionLoadProgressHandler,
        ModDefinitionAnalyzeHandler modDefinitionAnalyzeHandler, ModDefinitionEquivalentFilterHandler modDefinitionEquivalentFilterHandler,
        ModDefinitionPatchLoadHandler modDefinitionPatchLoadHandler,
        ILocalizationManager localizationManager, INotificationAction notificationAction,
        GameUserDirectoryChangedHandler directoryChangedHandler,
        ModListInstallRefreshRequestHandler installRefreshHandler) : IModHolderCoordinatorFactory
    {
        /// <inheritdoc />
        public virtual ModHolderCoordinators Create() => new(
            new ModStateReconciliationCoordinator(gameStateSafetyService),
            new ConflictAnalysisProgressCoordinator(modDefinitionLoadHandler, modDefinitionInvalidReplaceHandler,
                gameIndexProgressHandler, gameDefinitionLoadProgressHandler, modDefinitionAnalyzeHandler,
                modDefinitionEquivalentFilterHandler, modDefinitionPatchLoadHandler, localizationManager),
            new ModApplyResultPresenter(localizationManager, notificationAction),
            new ModHolderEventCoordinator(directoryChangedHandler, installRefreshHandler));
    }
}
