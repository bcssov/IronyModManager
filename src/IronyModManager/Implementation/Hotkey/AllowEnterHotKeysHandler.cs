// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-11-2022
//
// Last Modified By : Mario
// Last Modified On : 02-11-2022
// ***********************************************************************
// <copyright file="AllowEnterHotKeysHandler.cs" company="Mario">
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
    /// Class AllowEnterHotKeysHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{AllowEnterHotKeysEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{AllowEnterHotKeysEvent}" />
    public class AllowEnterHotKeysHandler : BaseMessageBusConsumer<AllowEnterHotKeysEvent>
    {
    }
}
