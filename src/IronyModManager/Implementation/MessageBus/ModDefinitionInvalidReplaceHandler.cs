// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 12-08-2020
//
// Last Modified By : Mario
// Last Modified On : 12-08-2020
// ***********************************************************************
// <copyright file="ModDefinitionInvalidReplaceHandler.cs" company="Mario">
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
    /// Class ModDefinitionInvalidReplaceHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Services.Common.MessageBus.ModDefinitionInvalidReplaceEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.Services.Common.MessageBus.ModDefinitionInvalidReplaceEvent}" />
    public class ModDefinitionInvalidReplaceHandler : BaseMessageBusConsumer<ModDefinitionInvalidReplaceEvent>
    {
    }
}
