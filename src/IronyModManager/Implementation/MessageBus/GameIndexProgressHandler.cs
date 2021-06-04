// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-28-2021
//
// Last Modified By : Mario
// Last Modified On : 05-28-2021
// ***********************************************************************
// <copyright file="GameIndexProgressHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Services.Common.MessageBus;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Implementation.MessageBus
{
    /// <summary>
    /// Class GameIndexProgressHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Services.Common.MessageBus.GameIndexProgressEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Services.Common.MessageBus.GameIndexProgressEvent}" />
    public class GameIndexProgressHandler : BaseMessageBusConsumer<GameIndexProgressEvent>
    {
    }
}
