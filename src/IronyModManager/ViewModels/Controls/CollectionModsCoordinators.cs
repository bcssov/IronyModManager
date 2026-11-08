// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Represents the separate behavioral collaborators owned by one Collection Mods control.
    /// </summary>
    public sealed class CollectionModsCoordinators(CollectionModMembership membership,
        CollectionModReconciliationCoordinator reconciliation, CollectionOperationProgressCoordinator operations,
        CollectionModsPresentationCoordinator presentation, CollectionModsEventCoordinator events)
    {
        /// <summary>
        /// Gets the collection membership policy.
        /// </summary>
        public CollectionModMembership Membership { get; } = membership;

        /// <summary>
        /// Gets the authoritative collection reconciliation coordinator.
        /// </summary>
        public CollectionModReconciliationCoordinator Reconciliation { get; } = reconciliation;

        /// <summary>
        /// Gets the operation progress coordinator.
        /// </summary>
        public CollectionOperationProgressCoordinator Operations { get; } = operations;

        /// <summary>
        /// Gets the collection presentation coordinator.
        /// </summary>
        public CollectionModsPresentationCoordinator Presentation { get; } = presentation;

        /// <summary>
        /// Gets the activation-scoped Collection Mods event coordinator.
        /// </summary>
        public CollectionModsEventCoordinator Events { get; } = events;
    }
}
