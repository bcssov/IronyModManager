// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// ***********************************************************************

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Describes the effective game load-order apply outcome without conflating skipped virtual members with failure.
    /// </summary>
    public class ModApplyResult
    {
        /// <summary>
        /// Gets or sets whether the effective load order was written successfully.
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Gets or sets the number of virtual collection members excluded from the effective load order.
        /// </summary>
        public int SkippedVirtualMods { get; set; }
    }
}
