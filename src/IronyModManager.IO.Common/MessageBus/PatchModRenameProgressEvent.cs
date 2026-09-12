// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// ***********************************************************************

using IronyModManager.Shared.MessageBus;

namespace IronyModManager.IO.Common.MessageBus
{
    /// <summary>
    /// Reports progress while renaming a patch mod.
    /// </summary>
    public class PatchModRenameProgressEvent : BaseNonAwaitableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PatchModRenameProgressEvent"/> class.
        /// </summary>
        /// <param name="percentage">The percentage of completed work.</param>
        public PatchModRenameProgressEvent(double percentage)
        {
            Percentage = percentage;
        }

        /// <summary>
        /// Gets the percentage of completed work.
        /// </summary>
        public double Percentage { get; }
    }
}
