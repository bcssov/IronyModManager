
// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 06-25-2023
//
// Last Modified By : Mario
// Last Modified On : 06-25-2023
// ***********************************************************************
// <copyright file="MessageBusMemoryProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlimMessageBus.Host;
using SlimMessageBus.Host.Memory;

namespace IronyModManager.DI.MessageBus
{

    /// <summary>
    /// Class MessageBusMemoryProvider.
    /// Implements the <see cref="MemoryMessageBus" />
    /// </summary>
    /// <seealso cref="MemoryMessageBus" />
    internal class MessageBusMemoryProvider : MemoryMessageBus
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBusMemoryProvider"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public MessageBusMemoryProvider(MessageBusSettings settings) : base(settings, new MemoryMessageBusSettings() { EnableMessageSerialization = false })
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Asserts the settings.
        /// </summary>
        protected override void AssertSettings()
        {
            // Sigh, let's fix the validation mess
            foreach (var consumerSettings in Settings.Consumers)
            {
                if (consumerSettings.ConsumerMethodInfo != null && consumerSettings.ConsumerMethod == null)
                {
                    consumerSettings.ConsumerMethod = ReflectionUtils.GenerateMethodCallToFunc<Func<object, object, Task>>(consumerSettings.ConsumerMethodInfo, consumerSettings.ConsumerType, typeof(Task), consumerSettings.MessageType);
                }
            }
            base.AssertSettings();
        }

        #endregion Methods
    }
}
