// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 06-11-2020
// ***********************************************************************
// <copyright file="WritingStateOperationHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.IO.Common.MessageBus;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Implementation.MessageBus
{
    /// <summary>
    /// Class WritingStateOperationHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseIronyMessageBusConsumer{IronyModManager.IO.Common.MessageBus.WritingStateOperationEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseIronyMessageBusConsumer{IronyModManager.IO.Common.MessageBus.WritingStateOperationEvent}" />
    public class WritingStateOperationHandler : BaseIronyMessageBusConsumer<WritingStateOperationEvent>
    {
    }
}
