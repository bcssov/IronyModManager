// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using IronyModManager.IO.Common.MessageBus;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Implementation.MessageBus
{
    /// <summary>
    /// Handles patch mod rename progress events.
    /// </summary>
    public class PatchModRenameProgressHandler : BaseMessageBusConsumer<PatchModRenameProgressEvent>
    {
    }
}
