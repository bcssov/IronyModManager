// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-22-2021
//
// Last Modified By : Mario
// Last Modified On : 02-22-2021
// ***********************************************************************
// <copyright file="ModListInstallRefreshRequestHandler.cs" company="Mario">
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
    /// Class ModListInstallRefreshRequestHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Implementation.MessageBus.Events.ModListInstallRefreshRequestEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Implementation.MessageBus.Events.ModListInstallRefreshRequestEvent}" />
    public class ModListInstallRefreshRequestHandler : BaseMessageBusConsumer<ModListInstallRefreshRequestEvent>
    {
    }
}
