// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 06-10-2020
// ***********************************************************************
// <copyright file="MessageBusRegistration.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using SlimMessageBus.Host.Config;
using SlimMessageBus.Host.Memory;

namespace IronyModManager.DI.MessageBus
{
    /// <summary>
    /// Class MessageBusRegistration.
    /// </summary>
    public static class MessageBusRegistration
    {
        #region Methods

        /// <summary>
        /// Registers this instance.
        /// </summary>
        public static void Register()
        {
            var builder = MessageBusBuilder.Create()
                .WithDependencyResolver(new MessageBusDependencyResolver())
                .WithProviderMemory(new MemoryMessageBusSettings()
                {
                    EnableMessageSerialization = false
                });
            var mbus = new IronyMessageBus(builder.Build());
            DIContainer.Container.RegisterInstance(mbus);
        }

        #endregion Methods
    }
}
