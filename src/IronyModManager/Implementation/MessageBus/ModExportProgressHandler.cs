// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-30-2021
//
// Last Modified By : Mario
// Last Modified On : 05-30-2021
// ***********************************************************************
// <copyright file="ModExportProgressHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.IO.Common.MessageBus;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Implementation.MessageBus
{
    /// <summary>
    /// Class ModExportProgressHandler.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.IO.Common.MessageBus.ModExportProgressEvent}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseMessageBusConsumer{IronyModManager.IO.Common.MessageBus.ModExportProgressEvent}" />
    public class ModExportProgressHandler : BaseMessageBusConsumer<ModExportProgressEvent>
    {
    }
}
