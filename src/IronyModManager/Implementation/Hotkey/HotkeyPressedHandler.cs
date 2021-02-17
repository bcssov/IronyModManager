// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-17-2021
//
// Last Modified By : Mario
// Last Modified On : 02-17-2021
// ***********************************************************************
// <copyright file="HotkeyPressedHandler.cs" company="Mario">
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
    /// Class HotkeyPressedHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Implementation.Hotkey.HotkeyPressedEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Implementation.Hotkey.HotkeyPressedEvent}" />
    public class HotkeyPressedHandler : BaseMessageBusConsumer<HotkeyPressedEvent>
    {
    }
}
