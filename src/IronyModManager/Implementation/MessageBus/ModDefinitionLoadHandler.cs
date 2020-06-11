// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 06-11-2020
//
// Last Modified By : Mario
// Last Modified On : 06-11-2020
// ***********************************************************************
// <copyright file="ModDefinitionLoadHandler.cs" company="Mario">
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
    /// Class ModDefinitionLoadHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Services.Common.MessageBus.ModDefinitionLoadEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Services.Common.MessageBus.ModDefinitionLoadEvent}" />
    public class ModDefinitionLoadHandler : BaseMessageBusConsumer<ModDefinitionLoadEvent>
    {
    }
}
