// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-22-2020
//
// Last Modified By : Mario
// Last Modified On : 09-22-2020
// ***********************************************************************
// <copyright file="GameUserDirectoryChangedHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Common.Events;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Implementation.MessageBus
{
    /// <summary>
    /// Class GameUserDirectoryChangedHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Common.Events.GameUserDirectoryChangedEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Common.Events.GameUserDirectoryChangedEvent}" />
    public class GameUserDirectoryChangedHandler : BaseMessageBusConsumer<GameUserDirectoryChangedEvent>
    {
    }
}
