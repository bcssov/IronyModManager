// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Represents the separate collaborators owned by one Installed Mods control.
    /// </summary>
    public sealed class InstalledModsCoordinators(InstalledModsPresentationCoordinator presentation,
        InstalledModsEventCoordinator events)
    {
        public InstalledModsPresentationCoordinator Presentation { get; } = presentation;

        public InstalledModsEventCoordinator Events { get; } = events;
    }
}
