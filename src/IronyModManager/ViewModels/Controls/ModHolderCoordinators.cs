// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Represents the separate collaborators owned by one Mod Holder control.
    /// </summary>
    public sealed class ModHolderCoordinators(ModStateReconciliationCoordinator reconciliation,
        ConflictAnalysisProgressCoordinator conflictProgress, ModApplyResultPresenter applyResultPresenter,
        ModHolderEventCoordinator events)
    {
        public ModStateReconciliationCoordinator Reconciliation { get; } = reconciliation;

        public ConflictAnalysisProgressCoordinator ConflictProgress { get; } = conflictProgress;

        public ModApplyResultPresenter ApplyResultPresenter { get; } = applyResultPresenter;

        public ModHolderEventCoordinator Events { get; } = events;
    }
}
