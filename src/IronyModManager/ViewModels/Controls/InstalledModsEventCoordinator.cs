// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using System;
using IronyModManager.Implementation.MessageBus;
using IronyModManager.Implementation.MessageBus.Events;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Owns the MessageBus stream consumed by one Installed Mods activation scope.
    /// </summary>
    public class InstalledModsEventCoordinator(EvalModAchievementCompatibilityHandler achievementHandler)
    {
        /// <summary>
        /// Subscribes to achievement-compatibility requests.
        /// </summary>
        public virtual IDisposable SubscribeAchievementChecks(Action<EvalModAchievementsCompatibilityEvent> handler) =>
            achievementHandler.Subscribe(handler);
    }
}
