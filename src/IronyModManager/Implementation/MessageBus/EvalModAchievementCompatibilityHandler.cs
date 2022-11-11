// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 11-05-2022
//
// Last Modified By : Mario
// Last Modified On : 11-05-2022
// ***********************************************************************
// <copyright file="EvalModAchievementCompatibilityHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Implementation.MessageBus.Events;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Implementation.MessageBus
{
    /// <summary>
    /// Class EvalModAchievementCompatibilityHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Implementation.MessageBus.Events.EvalModAchievementsCompatibilityEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Implementation.MessageBus.Events.EvalModAchievementsCompatibilityEvent}" />
    public class EvalModAchievementCompatibilityHandler : BaseMessageBusConsumer<EvalModAchievementsCompatibilityEvent>
    {
    }
}
