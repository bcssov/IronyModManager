// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 07-10-2020
//
// Last Modified By : Mario
// Last Modified On : 07-10-2020
// ***********************************************************************
// <copyright file="OverlayProgressHandler.cs" company="Mario">
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
    /// Class OverlayProgressHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Common.Events.OverlayProgressEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Common.Events.OverlayProgressEvent}" />
    public class OverlayProgressHandler : BaseMessageBusConsumer<OverlayProgressEvent>
    {
    }
}
