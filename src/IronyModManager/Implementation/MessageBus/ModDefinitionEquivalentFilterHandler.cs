// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using IronyModManager.Services.Common.MessageBus;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Implementation.MessageBus
{
    /// <summary>
    /// Consumes equivalent-conflict filter progress events.
    /// </summary>
    public class ModDefinitionEquivalentFilterHandler : BaseMessageBusConsumer<ModDefinitionEquivalentFilterEvent>
    {
    }
}
