// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// ***********************************************************************

namespace IronyModManager.Services.Common.MessageBus
{
    /// <summary>
    /// Reports progress while representationally equivalent definition conflicts are filtered.
    /// </summary>
    public class ModDefinitionEquivalentFilterEvent : ModDefinitionProcessEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModDefinitionEquivalentFilterEvent" /> class.
        /// </summary>
        public ModDefinitionEquivalentFilterEvent(double percentage) : base(percentage)
        {
        }
    }
}
