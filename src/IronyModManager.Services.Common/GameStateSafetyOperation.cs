// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// ***********************************************************************
namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Defines the filesystem trust semantics of a service operation.
    /// </summary>
    public enum GameStateSafetyOperation
    {
        /// <summary>
        /// The operation reads filesystem state that must be authoritative.
        /// </summary>
        TrustedRead,

        /// <summary>
        /// The operation mutates filesystem-backed game or mod state.
        /// </summary>
        Mutation
    }
}
