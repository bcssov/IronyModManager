// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 11-27-2020
//
// Last Modified By : Mario
// Last Modified On : 11-27-2020
// ***********************************************************************
// <copyright file="ModCompressMergeProgressHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using System;
using IronyModManager.Services.Common.MessageBus;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Implementation.MessageBus
{
    /// <summary>
    /// Class ModCompressMergeProgressHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Services.Common.MessageBus.ModCompressMergeProgressEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Services.Common.MessageBus.ModCompressMergeProgressEvent}" />
    public class ModCompressMergeProgressHandler : BaseMessageBusConsumer<ModCompressMergeProgressEvent>
    {
    }
}
