// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 06-20-2020
//
// Last Modified By : Mario
// Last Modified On : 08-14-2020
// ***********************************************************************
// <copyright file="ModDefinitionMergeProgressHandler.cs" company="Mario">
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
    /// Class ModMergeProgressHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Services.Common.MessageBus.ModDefinitionMergeProgressEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Services.Common.MessageBus.ModDefinitionMergeProgressEvent}" />
    public class ModDefinitionMergeProgressHandler : BaseMessageBusConsumer<ModDefinitionMergeProgressEvent>
    {
    }
}
