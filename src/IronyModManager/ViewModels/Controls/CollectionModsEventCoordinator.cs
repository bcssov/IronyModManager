// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using System;
using IronyModManager.Common.Events;
using IronyModManager.Implementation.Hotkey;
using IronyModManager.Implementation.MessageBus.Events;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Owns the MessageBus streams consumed by one Collection Mods activation scope.
    /// </summary>
    public class CollectionModsEventCoordinator(ModCollectionChangeRequestHandler collectionChangeHandler,
        MainViewHotkeyPressedHandler hotkeyPressedHandler)
    {
        /// <summary>
        /// Subscribes to collection-change requests.
        /// </summary>
        public virtual IDisposable SubscribeCollectionChanges(Action<ModCollectionChangeRequestEvent> handler) =>
            collectionChangeHandler.Subscribe(handler);

        /// <summary>
        /// Subscribes to Collection Mods hotkeys.
        /// </summary>
        public virtual IDisposable SubscribeHotkeys(Action<MainViewHotkeyPressedEvent> handler) =>
            hotkeyPressedHandler.Subscribe(handler);
    }
}
