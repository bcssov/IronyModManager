// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using System;
using System.Reactive.Disposables;
using IronyModManager.Common;
using IronyModManager.Implementation.MessageBus;
using IronyModManager.Localization;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Owns conflict-analysis progress subscriptions and their step-to-overlay presentation policy.
    /// </summary>
    public class ConflictAnalysisProgressCoordinator(ModDefinitionLoadHandler modDefinitionLoadHandler,
        ModDefinitionInvalidReplaceHandler modDefinitionInvalidReplaceHandler, GameIndexProgressHandler gameIndexProgressHandler,
        GameDefinitionLoadProgressHandler gameDefinitionLoadProgressHandler, ModDefinitionAnalyzeHandler modDefinitionAnalyzeHandler,
        ModDefinitionEquivalentFilterHandler modDefinitionEquivalentFilterHandler,
        ModDefinitionPatchLoadHandler modDefinitionPatchLoadHandler, ILocalizationManager localizationManager)
    {
        private IDisposable definitionAnalyzeLoadHandler;
        private IDisposable definitionEquivalentFilterHandler;
        private IDisposable definitionLoadHandler;
        private IDisposable definitionSyncHandler;
        private IDisposable gameDefinitionLoadHandler;
        private IDisposable gameIndexHandler;
        private IDisposable modInvalidReplaceHandler;

        /// <summary>
        /// Subscribes to one conflict-analysis run and binds every subscription to the caller's activation lifetime.
        /// </summary>
        public virtual void Subscribe(long id, CompositeDisposable disposables, int totalSteps, Action<long, bool, string, string> showOverlay)
        {
            definitionLoadHandler?.Dispose();
            definitionLoadHandler = modDefinitionLoadHandler.Subscribe(s =>
            {
                ShowProgress(id, totalSteps, 1, s.Percentage, LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Loading_Definitions, showOverlay);
            }).DisposeWith(disposables);

            modInvalidReplaceHandler?.Dispose();
            modInvalidReplaceHandler = modDefinitionInvalidReplaceHandler.Subscribe(s =>
            {
                ShowProgress(id, totalSteps, 2, s.Percentage, LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Replacing_Definitions, showOverlay);
            }).DisposeWith(disposables);

            gameIndexHandler?.Dispose();
            gameIndexHandler = gameIndexProgressHandler.Subscribe(s =>
            {
                ShowProgress(id, totalSteps, 3, s.Percentage, LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Indexing_Game, showOverlay);
            }).DisposeWith(disposables);

            gameDefinitionLoadHandler?.Dispose();
            gameDefinitionLoadHandler = gameDefinitionLoadProgressHandler.Subscribe(s =>
            {
                ShowProgress(id, totalSteps, 4, s.Percentage, LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Loading_Game_Definitions, showOverlay);
            }).DisposeWith(disposables);

            definitionAnalyzeLoadHandler?.Dispose();
            definitionAnalyzeLoadHandler = modDefinitionAnalyzeHandler.Subscribe(s =>
            {
                ShowProgress(id, totalSteps, totalSteps == 7 ? 5 : 3, s.Percentage, LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Analyzing_Conflicts, showOverlay);
            }).DisposeWith(disposables);

            definitionEquivalentFilterHandler?.Dispose();
            definitionEquivalentFilterHandler = modDefinitionEquivalentFilterHandler.Subscribe(s =>
            {
                ShowProgress(id, totalSteps, totalSteps == 7 ? 6 : 4, s.Percentage, LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Filtering_Equivalent_Conflicts, showOverlay);
            }).DisposeWith(disposables);

            definitionSyncHandler?.Dispose();
            definitionSyncHandler = modDefinitionPatchLoadHandler.Subscribe(s =>
            {
                ShowProgress(id, totalSteps, totalSteps == 7 ? 7 : 5, s.Percentage, LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Analyzing_Resolved_Conflicts, showOverlay);
            }).DisposeWith(disposables);
        }

        /// <summary>
        /// Ends subscriptions for the current analysis run.
        /// </summary>
        public virtual void Reset()
        {
            definitionAnalyzeLoadHandler?.Dispose();
            definitionEquivalentFilterHandler?.Dispose();
            definitionLoadHandler?.Dispose();
            definitionSyncHandler?.Dispose();
            gameDefinitionLoadHandler?.Dispose();
            gameIndexHandler?.Dispose();
            modInvalidReplaceHandler?.Dispose();
        }

        private void ShowProgress(long id, int totalSteps, int step, double percentage, string messageResource, Action<long, bool, string, string> showOverlay)
        {
            var message = localizationManager.GetResource(messageResource);
            var overlayProgress = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Mod_Actions.ConflictSolver.Overlay_Conflict_Solver_Progress),
                new { PercentDone = percentage.ToLocalizedPercentage(), Count = step, TotalCount = totalSteps });
            showOverlay(id, true, message, overlayProgress);
        }
    }
}
