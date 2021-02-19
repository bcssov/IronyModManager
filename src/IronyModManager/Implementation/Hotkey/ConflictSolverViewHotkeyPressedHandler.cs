// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-18-2021
//
// Last Modified By : Mario
// Last Modified On : 02-18-2021
// ***********************************************************************
// <copyright file="ConflictSolverViewHotkeyPressedHandler.cs" company="Mario">
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
    /// Class ConflictSolverViewHotkeyPressedHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Implementation.Hotkey.ConflictSolverViewHotkeyPressedEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Implementation.Hotkey.ConflictSolverViewHotkeyPressedEvent}" />
    public class ConflictSolverViewHotkeyPressedHandler : BaseMessageBusConsumer<ConflictSolverViewHotkeyPressedEvent>
    {
    }
}
