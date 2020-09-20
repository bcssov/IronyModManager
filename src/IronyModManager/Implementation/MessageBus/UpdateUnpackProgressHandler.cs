// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-20-2020
//
// Last Modified By : Mario
// Last Modified On : 09-20-2020
// ***********************************************************************
// <copyright file="UpdateUnpackProgressHandler.cs" company="Mario">
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
    /// Class UpdateUnpackProgressHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.IO.Common.MessageBus.UpdateUnpackProgressEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.IO.Common.MessageBus.UpdateUnpackProgressEvent}" />
    public class UpdateUnpackProgressHandler : BaseMessageBusConsumer<UpdateUnpackProgressEvent>
    {
    }
}
