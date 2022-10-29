// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 10-29-2022
//
// Last Modified By : Mario
// Last Modified On : 10-29-2022
// ***********************************************************************
// <copyright file="GameExeChangedHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Common.Events;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Implementation.MessageBus
{
    /// <summary>
    /// Class GameExeChangedHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Common.Events.GameExeChangedEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Common.Events.GameExeChangedEvent}" />
    public class GameExeChangedHandler : BaseMessageBusConsumer<GameExeChangedEvent>
    {
    }
}
