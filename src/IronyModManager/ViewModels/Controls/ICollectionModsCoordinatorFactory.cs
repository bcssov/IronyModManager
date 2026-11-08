// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Creates the collaborator scope owned by one Collection Mods control.
    /// </summary>
    public interface ICollectionModsCoordinatorFactory
    {
        /// <summary>
        /// Creates the strongly typed collaborator set.
        /// </summary>
        CollectionModsCoordinators Create();
    }
}
