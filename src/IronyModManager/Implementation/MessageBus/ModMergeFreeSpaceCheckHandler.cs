// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-17-2021
//
// Last Modified By : Mario
// Last Modified On : 03-17-2021
// ***********************************************************************
// <copyright file="ModMergeFreeSpaceCheckHandler.cs" company="Mario">
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
    /// Class ModMergeFreeSpaceCheckHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Services.Common.MessageBus.ModMergeFreeSpaceCheckEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Services.Common.MessageBus.ModMergeFreeSpaceCheckEvent}" />
    public class ModMergeFreeSpaceCheckHandler : BaseMessageBusConsumer<ModMergeFreeSpaceCheckEvent>
    {
    }
}
