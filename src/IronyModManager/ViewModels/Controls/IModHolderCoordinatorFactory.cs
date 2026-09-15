// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Creates the collaborator scope owned by one Mod Holder control.
    /// </summary>
    public interface IModHolderCoordinatorFactory
    {
        /// <summary>
        /// Creates the strongly typed collaborator set.
        /// </summary>
        ModHolderCoordinators Create();
    }
}
