// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Creates the collaborator scope owned by one Installed Mods control.
    /// </summary>
    public interface IInstalledModsCoordinatorFactory
    {
        /// <summary>
        /// Creates the strongly typed collaborator set.
        /// </summary>
        InstalledModsCoordinators Create();
    }
}
