// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-08-2025
//
// Last Modified By : Mario
// Last Modified On : 01-08-2025
// ***********************************************************************
// <copyright file="ModCollectionChangeRequestHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Common.Events;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Implementation.MessageBus.Events
{
    /// <summary>
    /// Class ModCollectionChangeRequestHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Common.Events.ModCollectionChangeRequestEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Common.Events.ModCollectionChangeRequestEvent}" />
    public class ModCollectionChangeRequestHandler : BaseMessageBusConsumer<ModCollectionChangeRequestEvent>
    {
    }
}
