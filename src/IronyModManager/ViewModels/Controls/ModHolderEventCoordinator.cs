// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using System;
using IronyModManager.Common.Events;
using IronyModManager.Implementation.MessageBus;
using IronyModManager.Implementation.MessageBus.Events;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Owns the MessageBus streams consumed by one Mod Holder activation scope.
    /// </summary>
    public class ModHolderEventCoordinator(GameUserDirectoryChangedHandler directoryChangedHandler,
        ModListInstallRefreshRequestHandler installRefreshHandler)
    {
        /// <summary>
        /// Subscribes to explicit game-directory configuration changes.
        /// </summary>
        public virtual IDisposable SubscribeDirectoryChanges(Action<GameUserDirectoryChangedEvent> handler) =>
            directoryChangedHandler.Subscribe(handler);

        /// <summary>
        /// Subscribes to installed-mod refresh requests.
        /// </summary>
        public virtual IDisposable SubscribeInstallRefresh(Action<ModListInstallRefreshRequestEvent> handler) =>
            installRefreshHandler.Subscribe(handler);
    }
}
