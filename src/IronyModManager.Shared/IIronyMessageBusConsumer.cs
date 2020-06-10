// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 06-10-2020
// ***********************************************************************
// <copyright file="IIronyMessageBusConsumer.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using SlimMessageBus;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Interface IIronyMessageBusConsumer
    /// Implements the <see cref="SlimMessageBus.IConsumer{TMessage}" />
    /// </summary>
    /// <typeparam name="TMessage">The type of the t message.</typeparam>
    /// <seealso cref="SlimMessageBus.IConsumer{TMessage}" />
    public interface IIronyMessageBusConsumer<in TMessage> : IConsumer<TMessage>
    {
    }
}
