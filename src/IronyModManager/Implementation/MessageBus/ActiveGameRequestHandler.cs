// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-07-2020
//
// Last Modified By : Mario
// Last Modified On : 09-07-2020
// ***********************************************************************
// <copyright file="ActiveGameRequestHandler.cs" company="Mario">
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
    /// Class ActiveGameRequestHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Common.Events.ActiveGameRequestEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Common.Events.ActiveGameRequestEvent}" />
    public class ActiveGameRequestHandler : BaseMessageBusConsumer<ActiveGameRequestEvent>
    {
    }
}
