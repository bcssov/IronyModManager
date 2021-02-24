// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-21-2021
//
// Last Modified By : Mario
// Last Modified On : 02-21-2021
// ***********************************************************************
// <copyright file="SuspendHotkeysHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Implementation.Hotkey
{
    /// <summary>
    /// Class SuspendHotkeysHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Implementation.Hotkey.SuspendHotkeysEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Implementation.Hotkey.SuspendHotkeysEvent}" />
    public class SuspendHotkeysHandler : BaseMessageBusConsumer<SuspendHotkeysEvent>
    {
    }
}
