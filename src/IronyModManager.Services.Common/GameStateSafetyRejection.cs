// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// ***********************************************************************
namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Defines an explicit, supported result when a guarded invocation is rejected.
    /// </summary>
    public enum GameStateSafetyRejection
    {
        /// <summary>
        /// Returns <see langword="false" />.
        /// </summary>
        False,

        /// <summary>
        /// Returns <see langword="null" />.
        /// </summary>
        Null,

        /// <summary>
        /// Completes a non-generic task successfully without invoking the target.
        /// </summary>
        CompletedTask
    }
}
