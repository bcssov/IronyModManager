// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 08-14-2020
//
// Last Modified By : Mario
// Last Modified On : 08-14-2020
// ***********************************************************************
// <copyright file="ModFileMergeProgressHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Services.Common.MessageBus;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Implementation.MessageBus
{
    /// <summary>
    /// Class ModFileMergeProgressHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Services.Common.MessageBus.ModFileMergeProgressEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Services.Common.MessageBus.ModFileMergeProgressEvent}" />
    public class ModFileMergeProgressHandler : BaseMessageBusConsumer<ModFileMergeProgressEvent>
    {
    }
}
